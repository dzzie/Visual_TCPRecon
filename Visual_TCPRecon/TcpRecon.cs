using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Visual_TCPRecon;
using PacketDotNet;
using SharpPcap;

//author  : Saar Yahalom, 21 Sep 2007 
//original: http://www.codeproject.com/Articles/20501/TCP-Session-Reconstruction-Tool

// Translated from the file follow.c from WireShark source code
// the code can be found at: http://www.wireshark.org/download.html

namespace VTcpRecon
{
 /* here we are going to try and reconstruct the data portion of a TCP
session. We will try and handle duplicates, TCP fragments, and out
of order packets in a smart way. */

/// <summary>
/// A class that represent a node in a linked list that holds partial Tcp session
/// fragments
/// </summary>
internal class tcp_frag {
	public ulong    seq = 0;
    public ulong    len = 0;
    public ulong    data_len = 0;
    public byte[]   data = null;
    public tcp_frag next = null;
};


public class TcpRecon
{
    // holds two linked list of the session data, one for each direction    
    tcp_frag[] frags = new tcp_frag[2];
    // holds the last sequence number for each direction
    ulong[] seq = new ulong[2];
    long[]  src_addr = new long[2];
    uint[]  src_port = new uint[2];
    bool    empty_tcp_stream = true;
    uint[]  tcp_port = new uint[2];
    uint[]  bytes_written = new uint[2];
    System.IO.FileStream data_out_file = null;
    bool    incomplete_tcp_stream = false;
    bool    closed = false;
    public string dumpFile = "";

    //these next three and used externally by the UI
    public long LastSavedOffset = 0;
    public bool isComplete = false;
    public int LastSourcePort = 0;     //set by ui code after it detects a packetWritten...

    public long CurrentOffset = 0;
    public bool PacketWritten = false;
    public long PreviousPacketEndOffset = 0;
   
    public string StreamStartTimeStamp;
    public string relativeTimeStamp;

    public string GetDetails()
    {
        string t = "Stream Details\r\n" + "-".Repeat(20);
        t += "\r\nFile: " + Path.GetFileName(dumpFile);
        t += "\r\nDir: " + Path.GetDirectoryName(dumpFile);
        t += "\r\nStart Epoch Time: " + StreamStartTimeStamp;
        t += "\r\nrelative Time: " + relativeTimeStamp;
        return t+"\r\n\r\n";
    }

    public string HashCode
    {
        get { return this.GetHashCode().ToString("X"); }
    }

    public bool IncompleteStream
    {
        get { return incomplete_tcp_stream; }
    }   
    public bool EmptyStream
    {
        get { return empty_tcp_stream; }
    }

    public TcpRecon(string filename)
    {
        dumpFile = filename;
        reset_tcp_reassembly();
        data_out_file = new System.IO.FileStream(filename, System.IO.FileMode.Create);
    }

    /// <summary>
    /// Cleans up the class and frees resources
    /// </summary>
    public void Close()
    {
        if (!closed)
        {
            data_out_file.Close();
            reset_tcp_reassembly();
            closed = true;
        }
    }
    
    ~TcpRecon()
    {
        Close();
    }

    private void write_packet_data(int index, byte[] data)
    {
        if (data.Length == 0) return;

        PacketWritten = true;
        empty_tcp_stream = false;
        PreviousPacketEndOffset = CurrentOffset;
        CurrentOffset += data.Length;
        
        data_out_file.Write(data, 0, data.Length);
        bytes_written[index] += (uint)data.Length;
      
    }

    public void ReassemblePacket(TcpPacket tcpPacket)
    {

        PacketWritten = false;
        ulong length = (ulong)(tcpPacket.Bytes.Length - tcpPacket.Header.Length ); //todo check me

        if (length == 0) return;

        IpPacket ipPacket = (IpPacket)tcpPacket.ParentPacket;

        reassemble_tcp((ulong)tcpPacket.SequenceNumber, length,
                       tcpPacket.PayloadData, (ulong)tcpPacket.PayloadData.Length, tcpPacket.Syn,
                       ipPacket.SourceAddress.Address,ipPacket.DestinationAddress.Address ,
                       (uint)tcpPacket.SourcePort, (uint)tcpPacket.DestinationPort);
    }

   
    private void reassemble_tcp( ulong sequence,    ulong length,   byte[] data,
		                         ulong data_length, bool synflag,   long net_src,
		                         long net_dst,      uint srcport,   uint dstport) 
    {
       long srcx, dstx;
       int src_index, j;
       bool first = false;
       ulong newseq;
       tcp_frag tmp_frag;

       src_index = -1;

       /* Now check if the packet is for this connection. */
        srcx = net_src;
        dstx = net_dst;

       /* Check to see if we have seen this source IP and port before.
       (Yes, we have to check both source IP and port; the connection
       might be between two different ports on the same machine.) */
       for( j=0; j<2; j++ ) {
           if (src_addr[j] == srcx && src_port[j] == srcport ) {
	           src_index = j;
           }
       }
       /* we didn't find it if src_index == -1 */
       if( src_index < 0 ) {
           /* assign it to a src_index and get going */
           for( j=0; j<2; j++ ) {
	           if( src_port[j] == 0 ) {
		           src_addr[j] = srcx;
		           src_port[j] = srcport;
		           src_index = j;
		           first = true;
		           break;
	           }
           }
       }
       if( src_index < 0 ) {
           throw new Exception("ERROR in reassemble_tcp: Too many addresses!");
       }

       if( data_length < length ) {
           incomplete_tcp_stream = true;
       }

       /* now that we have filed away the srcs, lets get the sequence number stuff
       figured out */
       if( first ) {
           /* this is the first time we have seen this src's sequence number */
           seq[src_index] = sequence + length;
           if( synflag ) {
	           seq[src_index]++;
           }
           /* write out the packet data */
           write_packet_data( src_index, data );
           return;
       }
       /* if we are here, we have already seen this src, let's
       try and figure out if this packet is in the right place */
       if( sequence < seq[src_index] ) {
           /* this sequence number seems dated, but
           check the end to make sure it has no more
           info than we have already seen */
           newseq = sequence + length;
           if( newseq > seq[src_index] ) {
	           ulong new_len;

	           /* this one has more than we have seen. let's get the
	           payload that we have not seen. */

	           new_len = seq[src_index] - sequence;

	           if ( data_length <= new_len ) {
		           data = null;
		           data_length = 0;
		           incomplete_tcp_stream = true;
	           } else {
		           data_length -= new_len;
                   byte[] tmpData = new byte[data_length];
                   for(ulong i=0; i<data_length; i++)
                       tmpData[i] = data[i + new_len];

                   data = tmpData;
	           }
	           sequence = seq[src_index];
	           length = newseq - seq[src_index];

	           /* this will now appear to be right on time :) */
           }
       }
       if ( sequence == seq[src_index] ) {
           /* right on time */
           seq[src_index] += length;
           if( synflag ) seq[src_index]++;
           if( data != null ) {
	           write_packet_data( src_index, data );
           }
           /* done with the packet, see if it caused a fragment to fit */
           while( check_fragments( src_index ) )
	           ;
       }
       else {
           /* out of order packet */
           if(data_length > 0 && sequence > seq[src_index] ) {
	           tmp_frag = new tcp_frag();
	           tmp_frag.data = data;
	           tmp_frag.seq = sequence;
	           tmp_frag.len = length;
	           tmp_frag.data_len = data_length;
	           
	           if( frags[src_index] != null ) {
		           tmp_frag.next = frags[src_index];
	           } else {
		           tmp_frag.next = null;
	           }
	           frags[src_index] = tmp_frag;
           }
       }
    } /* end reassemble_tcp */

    /* here we search through all the frag we have collected to see if
    one fits */
    bool check_fragments( int index ) {
        tcp_frag prev = null;
        tcp_frag current;
        current = frags[index];
        while( current != null ) {
	        if( current.seq == seq[index] ) {
		        /* this fragment fits the stream */
		        if( current.data != null ) {
			        write_packet_data( index, current.data );
		        }
		        seq[index] += current.len;
		        if( prev != null) {
			        prev.next = current.next;
		        } else {
			        frags[index] = current.next;
		        }
		        current.data = null;
		        current = null;
		        return true;
	        }
	        prev = current;
	        current = current.next;
        }
        return false;
    }

    // cleans the linked list
    void reset_tcp_reassembly()
    {
        tcp_frag current, next;
        int i;

        empty_tcp_stream = true;
        incomplete_tcp_stream = false;
        for( i=0; i<2; i++ ) {
	        seq[i] = 0;
	        src_addr[i] = 0;
	        src_port[i] = 0;
	        tcp_port[i] = 0;
	        bytes_written[i] = 0;
	        current = frags[i];
	        while( current != null ) {
		        next = current.next;
		        current.data = null;
		        current = null;
		        current = next;
	        }
	        frags[i] = null;
        }
    }

}

}
