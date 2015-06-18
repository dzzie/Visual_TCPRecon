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

//6.17.2015
//this class has been updated from code from Mark Woan's SessionViewer
//https://github.com/woanware/SessionViewer 

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
    tcp_frag[] _frags = new tcp_frag[2];
    // holds the last sequence number for each direction
    ulong[] _seq = new ulong[2];
    long[]  _srcAddr = new long[2];
    uint[]  _srcPort = new uint[2];
    uint[]  _tcpPort = new uint[2];
    

    public PosixTimeval TimestampFirstPacket { get; set; }
    public PosixTimeval TimestampLastPacket { get; set; }
//    private bool _lastPacketOutbound = true;
    public long DataSize { get; private set; }
    public bool HasFin { get; private set; }
//    private long _maxSize;
    private FileStream _storage = null;
    //private string _outputPath;
    public string dumpFile = "";
    bool    closed = false;

    //these next three and used externally by the UI
    public long LastSavedOffset = 0;
    public bool isComplete = false;
    public int LastSourcePort = 0;     //set by ui code after it detects a packetWritten...

    public bool PacketWritten = false;
    public long CurrentOffset = 0;
    public long PreviousPacketEndOffset = 0;
    public string StreamStartTimeStamp;
    public string relativeTimeStamp;

    /*
    uint[]  bytes_written = new uint[2];
    bool    empty_tcp_stream = true;
    System.IO.FileStream data_out_file = null;
    bool    incomplete_tcp_stream = false;
    */

    public string GetDetails()
    {
        string t = "Stream Details\r\n" + "-".Repeat(20);
        t += "\r\nFile: " + Path.GetFileName(dumpFile);
        t += "\r\nDir: " + Path.GetDirectoryName(dumpFile);
        t += "\r\nStart Epoch Time: " + TimestampFirstPacket;
        //t += "\r\nrelative Time: " + relativeTimeStamp;
        return t+"\r\n\r\n";
    }

    public string HashCode
    {
        get { return this.GetHashCode().ToString("X"); }
    }

    /*public bool IncompleteStream
    {
        get { return incomplete_tcp_stream; }
    }   
    public bool EmptyStream
    {
        get { return empty_tcp_stream; }
    }*/

    public TcpRecon(string filename)
    {
        dumpFile = filename;
        Reset();
        _storage = new System.IO.FileStream(filename, System.IO.FileMode.Create);
    }

    /// <summary>
    /// Cleans up the class and frees resources
    /// </summary>
    public void Close()
    {
        if (!closed)
        {
            _storage.Close();
            Reset();
            closed = true;
        }
    }
    
    ~TcpRecon()
    {
        Close();
    }

    public void ReassemblePacket(IpPacket ipPacket, TcpPacket tcpPacket, PosixTimeval timeVal)
    {
        try
        {

            ushort sourcePort = 0;
            ushort destinationPort = 0;

            if (ipPacket.Protocol == IPProtocolType.TCP)
            {
                PacketWritten = false; 
                sourcePort = tcpPacket.SourcePort;
                destinationPort = tcpPacket.DestinationPort;

                    // If the payload length is zero bail out
                ulong length = (ulong)(tcpPacket.PayloadData.Length);
                if (length == 0)
                {
                    //Console.WriteLine("Invalid length (TCP): " + ip.Source.ToString() + "#" + ip.Destination.ToString());
                    //return;
                }

                if (tcpPacket.Fin == true)
                {
                    this.HasFin = true;
                }

                //uint acknowledged = Convert.ToUInt32(tcpPacket.AcknowledgmentNumber);
                ReassembleTcp((ulong)tcpPacket.SequenceNumber,
                              tcpPacket.AcknowledgmentNumber,
                              length,
                              tcpPacket.PayloadData,
                              (ulong)tcpPacket.PayloadData.Length,
                              tcpPacket.Syn,
                              ipPacket.SourceAddress.Address,
                              ipPacket.DestinationAddress.Address,
                              (uint)tcpPacket.SourcePort,
                              (uint)tcpPacket.DestinationPort,
                              timeVal);
            }
           

            if (TimestampFirstPacket == null)
            {
                TimestampFirstPacket = timeVal;
            }

            TimestampLastPacket = timeVal;

           
        }
        catch (Exception ex)
        {
            //this.Log().Error(ex.ToString());
        }
    }

    /// <summary>
    /// Writes the payload data to the database
    /// </summary>
    /// <param name="net_src"></param>
    /// <param name="net_dst"></param>
    /// <param name="srcport"></param>
    /// <param name="dstport"></param>
    /// <param name="index"></param>
    /// <param name="data"></param>
    /// <param name="timestamp"></param>
    private void SavePacketData(long net_src,
                                long net_dst,
                                uint srcport,
                                uint dstport,
                                int index,
                                byte[] data,
                                PosixTimeval timestamp)
    {
        try
        {
            // Ignore empty packets
            if (data.Length == 0)
            {
                return;
            }

            if (data.Length == 1)
            {
                if (data[0] == 0)
                {
                    return;
                }
            }

            PacketWritten = true;
            PreviousPacketEndOffset = CurrentOffset;
            CurrentOffset += data.Length;
            DataSize += data.Length;

            _storage.Write(data, 0, data.Length);

            bool isOutBound = false;
            if (index == 0)
            {
                isOutBound = true;
            }


        }
        catch (Exception ex)
        {
            //this.Log().Error(ex.ToString());
        }
    }
 
   
    /// <summary>
    /// Reconstructs the tcp session
    /// </summary>
    /// <param name="sequence">Sequence number of the tcp packet</param>
    /// <param name="length">The size of the original packet data</param>
    /// <param name="data">The captured data</param>
    /// <param name="data_length">The length of the captured data</param>
    /// <param name="synflag"></param>
    /// <param name="net_src">The source ip address</param>
    /// <param name="net_dst">The destination ip address</param>
    /// <param name="srcport">The source port</param>
    /// <param name="dstport">The destination port</param>
    /// <param name="timestamp">Packet timestamp</param>
    private void ReassembleTcp(ulong sequence,
                               uint acknowledgement,
                               ulong length,
                               byte[] data,
                               ulong data_length,
                               bool synflag,
                               long net_src,
                               long net_dst,
                               uint srcport,
                               uint dstport,
                               PosixTimeval timestamp)
    {
        try
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
            for (j = 0; j < 2; j++)
            {
                if (_srcAddr[j] == srcx && _srcPort[j] == srcport)
                {
                    src_index = j;
                    //break;
                }
            }
            /* we didn't find it if src_index == -1 */
            if (src_index < 0)
            {
                /* assign it to a src_index and get going */
                for (j = 0; j < 2; j++)
                {
                    if (_srcPort[j] == 0)
                    {
                        _srcAddr[j] = srcx;
                        _srcPort[j] = srcport;
                        src_index = j;
                        first = true;
                        break;
                    }
                }
            }

            if (src_index < 0)
            {
                throw new Exception("Too many addresses!");
            }

            /* Before adding data for this flow to the data_out_file, check whether
             * this frame acks fragments that were already seen. This happens when
             * frames are not in the capture file, but were actually seen by the 
             * receiving host (Fixes bug 592).
             */
            if (_frags[1 - src_index] != null)
            {
                while (CheckFragments(net_src,
                                     net_dst,
                                     srcport,
                                     dstport,
                                     1 - src_index,
                                     timestamp,
                                     acknowledgement)) ;
            }

            /* now that we have filed away the srcs, lets get the sequence number stuff figured out */
            if (first)
            {
                /* this is the first time we have seen this src's sequence number */
                _seq[src_index] = sequence + length;
                if (synflag)
                {
                    _seq[src_index]++;
                }

                SavePacketData(net_src,
                               net_dst,
                               srcport,
                               dstport,
                               src_index,
                               data,
                               timestamp);
                return;
            }

            /* if we are here, we have already seen this src, let's
            try and figure out if this packet is in the right place */
            if (sequence < _seq[src_index])
            {
                /* this sequence number seems dated, but
                check the end to make sure it has no more
                info than we have already seen */
                newseq = sequence + length;
                if (newseq > _seq[src_index])
                {
                    ulong new_len;

                    /* this one has more than we have seen. let's get the
                    payload that we have not seen. */

                    new_len = _seq[src_index] - sequence;

                    if (data_length <= new_len)
                    {
                        data = null;
                        data_length = 0;
                    }
                    else
                    {
                        data_length -= new_len;
                        byte[] tmpData = new byte[data_length];

                        Buffer.BlockCopy(data, (int)new_len, tmpData, 0, (int)data_length);
                        //for (ulong i = 0; i < data_length; i++)
                        //{
                        //    tmpData[i] = data[i + new_len];
                        //}

                        data = tmpData;
                    }
                    sequence = _seq[src_index];
                    length = newseq - _seq[src_index];

                    /* this will now appear to be right on time :) */
                }
            }
            if (sequence == _seq[src_index])
            {
                /* right on time */
                _seq[src_index] += length;
                if (synflag)
                {
                    _seq[src_index]++;
                }

                if (data != null)
                {
                    SavePacketData(net_src,
                                   net_dst,
                                   srcport,
                                   dstport,
                                   src_index,
                                   data,
                                   timestamp);
                }
                /* done with the packet, see if it caused a fragment to fit */
                while (CheckFragments(net_src,
                                      net_dst,
                                      srcport,
                                      dstport,
                                      src_index,
                                      timestamp,
                                      acknowledgement)) ;
            }
            else
            {
                /* out of order packet */
                if (data_length > 0 && sequence > _seq[src_index])
                {
                    tmp_frag = new tcp_frag();
                    tmp_frag.data = data;
                    tmp_frag.seq = sequence;
                    tmp_frag.len = length;
                    tmp_frag.data_len = data_length;

                    if (_frags[src_index] != null)
                    {
                        tmp_frag.next = _frags[src_index];
                    }
                    else
                    {
                        tmp_frag.next = null;
                    }

                    _frags[src_index] = tmp_frag;
                }
            }
        }
        catch (Exception ex)
        {
            //this.Log().Error(ex.ToString());
        }
    }

    /// <summary>
    /// Search through all the frag we have collected to see if one fits 
    /// </summary>
    /// <param name="net_src"></param>
    /// <param name="net_dst"></param>
    /// <param name="srcport"></param>
    /// <param name="dstport"></param>
    /// <param name="index"></param>
    /// <param name="timestamp"></param>
    /// <param name="acknowledged"></param>
    /// <returns></returns>
    private bool CheckFragments(long net_src,
                                long net_dst,
                                uint srcport,
                                uint dstport,
                                int index,
                                PosixTimeval timestamp,
                                uint acknowledged)
    {
        try
        {
            tcp_frag prev = null;
            tcp_frag current;
            ulong lowest_seq;

            current = _frags[index];
            if (current != null)
            {
                lowest_seq = current.seq;
                while (current != null)
                {
                    if ((lowest_seq > current.seq))
                    {
                        lowest_seq = current.seq;
                    }

                    if (current.seq < _seq[index])
                    {
                        ulong newseq;
                        /* this sequence number seems dated, but
                           check the end to make sure it has no more
                           info than we have already seen */
                        newseq = current.seq + current.len;
                        if (newseq > _seq[index])
                        {
                            ulong new_pos;

                            /* this one has more than we have seen. let's get the
                             payload that we have not seen. This happens when 
                             part of this frame has been retransmitted */
                            new_pos = _seq[index] - current.seq;
                            if (current.data_len > new_pos)
                            {
                                //sc->dlen = current.data_len - new_pos;

                                byte[] tmpData = new byte[current.data_len - new_pos];

                                Buffer.BlockCopy(current.data, (int)new_pos, tmpData, 0, (int)(current.data_len - new_pos));

                                SavePacketData(net_src,
                                             net_dst,
                                             srcport,
                                             dstport,
                                             index,
                                             tmpData,
                                             timestamp);
                            }

                            _seq[index] += (current.len - new_pos);
                        }

                        /* Remove the fragment from the list as the "new" part of it
                        * has been processed or its data has been seen already in 
                        * another packet. */
                        if (prev != null)
                        {
                            prev.next = current.next;
                        }
                        else
                        {
                            _frags[index] = current.next;
                        }
                        return true;
                    }


                    if (current.seq == _seq[index])
                    {
                        /* this fragment fits the stream */
                        if (current.data != null)
                        {
                            SavePacketData(net_src,
                                           net_dst,
                                           srcport,
                                           dstport,
                                           index,
                                           current.data,
                                           timestamp);
                        }

                        _seq[index] += current.len;

                        if (prev != null)
                        {
                            prev.next = current.next;
                        }
                        else
                        {
                            _frags[index] = current.next;
                        }

                        current.data = null;
                        current = null;
                        return true;
                    }
                    prev = current;
                    current = current.next;
                }

                if (acknowledged > lowest_seq)
                {
                    /* There are frames missing in the capture file that were seen
                     * by the receiving host. Add dummy stream chunk with the data
                     * "[xxx bytes missing in capture file]".
                     */
                    //dummy_str = g_strdup_printf("[%d bytes missing in capture file]",
                    //  (int)(lowest_seq - seq[idx]));
                    //sc->dlen = (guint32)strlen(dummy_str);

                    byte[] dummyData = new byte[(lowest_seq - _seq[index])];

                    SavePacketData(net_src,
                                   net_dst,
                                   srcport,
                                   dstport,
                                   index,
                                   dummyData,
                                   timestamp);

                    _seq[index] = lowest_seq;
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            //this.Log().Error(ex.ToString());
            return false;
        }
    }

    /// <summary>
    /// Cleans the linked list
    /// </summary>
    private void Reset()
    {
        try
        {
            tcp_frag current, next;

            for (int i = 0; i < 2; i++)
            {
                _seq[i] = 0;
                _srcAddr[i] = 0;
                _srcPort[i] = 0;
                _tcpPort[i] = 0;
                current = _frags[i];

                while (current != null)
                {
                    next = current.next;
                    current.data = null;
                    current = null;
                    current = next;
                }

                _frags[i] = null;
            }
        }
        catch (Exception ex)
        {
            //this.Log().Error(ex.ToString());
        }
    }

}

}
