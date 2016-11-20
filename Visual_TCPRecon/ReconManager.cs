using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VTcpRecon;
//using Tamir.IPLib.Packets;
//using Tamir.IPLib;
using SharpPcap.WinPcap;
using SharpPcap.LibPcap;
using PacketDotNet;
using System.IO;
using System.Threading;
using SharpPcap;


//there is some ugliness in this file in the way you have to process and conglomerate packets and only
//know so after the next packet has already been processed. This class hides those details from the
//data consumer which sits above it for clarity.

//Each individual stream has its own instance of this class. Streams are denoted by source: dest IP -- source: dest port pairs
//Each stream is saved to an individual file consisting of multiple data blocks. Each data block is held in its own class
//Which tracks file offset and size. The data can be extracted through the main ui to individual files. The main ui
//Can also do a display trick where it conglomerates multiple streams (same ips and dest ports but different source ports 
//Like multiple web requests) under a single tree node. 

namespace Visual_TCPRecon
{
    class ReconManager
    {
        public string ErrorMessage = "";
        static string outDir = "";
        static string capFile = "";
        private Form1 owner;
        private List<string> ips = new List<string>(); //all ip's seen
        private TcpPacket curPacket;
        private PosixTimeval curPacketTime;
        private decimal firstTimeStamp = 0;

        public uint totalPackets =0;
        public uint totalTCPPackets = 0;

        static Dictionary<Connection, TcpRecon> sharpPcapDict = new Dictionary<Connection, TcpRecon>();

        public delegate void _NewStream(TcpRecon recon);
        public delegate void _NewNode(DataBlock db);
        public delegate void _DNS(cDNS dns);
        public delegate void _Complete(List<string> ips);

        public _NewStream NewStream = null;
        public _NewNode NewNode = null;
        public _Complete Complete = null;
        public _DNS DNS = null;

        public ReconManager(_NewStream ns, _NewNode nn, _Complete c, _DNS d,  string pcapFile, string outputDir, Form1 parent)
        {
            NewStream = ns;
            NewNode = nn;
            Complete = c;
            DNS = d;
            outDir = outputDir;
            capFile = pcapFile;
            owner = parent;
        }

        private bool IPExists(string ip)
        {
            foreach (string s in ips) if (s == ip) return true;
            return false;
        }

        private void AddNewNode(TcpRecon recon)
        {
            int startAt = (int)recon.LastSavedOffset;
            int endAt = (int)recon.PreviousPacketEndOffset;
            if (recon.isComplete) endAt =(int)recon.CurrentOffset;

            DataBlock db = new DataBlock(recon.dumpFile, startAt, endAt - startAt, recon);

            db.DestPort = recon.LastDestPort;
            db.SourcePort = recon.LastSourcePort;
            db.SourceAddress = recon.LastSourceAddress;
            db.DestinationAddress = recon.LastDestinationAddress;
            db.EpochTimeStamp = curPacketTime.Seconds.ToString() + "." + curPacketTime.MicroSeconds.ToString();

            /*string fu = firstTimeStamp_s.ToString() + "." + firstTimeStamp_ms.ToString();
            string fu2 = firstpacketTimeStamp_s.ToString() + "." + firstpacketTimeStamp_ms.ToString();
            decimal tmp = decimal.Parse(fu);
            decimal temp2 = decimal.Parse(fu2);
            decimal x = temp2 - tmp;
            db.relativeTimeStamp = x.ToString();
            firstpacketTimeStamp_s = 0;*/

            /*long hi = (long)curPacket.PcapHeader.Seconds - firstTimeStamp_s;
            long low = (long)curPacket.PcapHeader.MicroSeconds - firstTimeStamp_ms;
            db.relativeTimeStamp = hi.ToString() + "." + low.ToString();
            */

            owner.Invoke(NewNode, db); 

            recon.LastSavedOffset = recon.PreviousPacketEndOffset;
        }

        private void HandleDNS(Packet packet)
        {
            //right now we are only passing up dns requests where we were able to extract the name
            UdpPacket udp = (UdpPacket)packet;
            IpPacket ipPacket = (IpPacket)packet.ParentPacket;

            if (!IPExists("udp: " + ipPacket.DestinationAddress)) ips.Add("udp: " + ipPacket.DestinationAddress);
            if (!IPExists("udp: " + ipPacket.SourceAddress)) ips.Add("udp: " + ipPacket.SourceAddress);

            if (udp.DestinationPort == 53) //its a request
            {
                cDNS dns = new cDNS(udp.PayloadData);
                if (!dns.isResponse && dns.dnsName.Length > 0)
                {
                    owner.Invoke(DNS, dns); 
                }
            }

        }

        // The callback function for the SharpPcap library
        private void device_PcapOnPacketArrival(object sender, CaptureEventArgs e)
        {
            Packet packet;

            try
            {
                packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            }
            catch (Exception ex)
            {
                //System.Console.Write(ex.Message); //todo: sometimes get error raw packet not implemented?
                return;
            }

            if (firstTimeStamp == 0)
            {
                firstTimeStamp = decimal.Parse(e.Packet.Timeval.Seconds.ToString() + "." + e.Packet.Timeval.MicroSeconds.ToString());
            }

            totalPackets++;
            UdpPacket udpPacket = (UdpPacket)packet.Extract(typeof(UdpPacket));
            if (udpPacket != null)
            {
                HandleDNS(udpPacket);
                return;
            }
            
            IpPacket  ipPacket  = (IpPacket)packet.Extract(typeof(IpPacket));
            TcpPacket tcpPacket = (TcpPacket)packet.Extract(typeof(TcpPacket));
            
            if (tcpPacket == null) return;
            totalTCPPackets++;
            
            Connection c = new Connection(tcpPacket);
            TcpRecon recon = null;
            curPacket = tcpPacket;
            curPacketTime = e.Packet.Timeval;

            if (!sharpPcapDict.ContainsKey(c))
            {
                c.generateFileName(outDir);
                recon = new TcpRecon(c.fileName);
                
                //who started stream, who is receiving the stream..static for this stream..
                recon.ClientPort = tcpPacket.SourcePort;
                recon.ServerPort = tcpPacket.DestinationPort;
                recon.ClientAddress = ipPacket.SourceAddress.ToString(); 
                recon.ServerAddress = ipPacket.DestinationAddress.ToString();

                recon.StreamStartTimeStamp = e.Packet.Timeval.Seconds.ToString() + "." + e.Packet.Timeval.MicroSeconds.ToString();
                decimal curTime = decimal.Parse(recon.StreamStartTimeStamp);
                recon.relativeTimeStamp = (curTime - firstTimeStamp).ToString();

                //book kepping for datablock tracking per packet (changes)
                recon.LastSourcePort = tcpPacket.SourcePort;
                recon.LastDestPort = tcpPacket.DestinationPort;
                recon.LastSourceAddress = ipPacket.SourceAddress.ToString();
                recon.LastDestinationAddress = ipPacket.DestinationAddress.ToString();

                sharpPcapDict.Add(c, recon);

                if (!IPExists("tcp: " + ipPacket.DestinationAddress)) ips.Add("tcp: " + ipPacket.DestinationAddress);
                if (!IPExists("tcp: " + ipPacket.SourceAddress)) ips.Add("tcp: " + ipPacket.SourceAddress);
                owner.Invoke(NewStream, recon); 
            }else{
                recon = sharpPcapDict[c];
            }
            
            //can contain fragments and out of order packets 
            recon.ReassemblePacket(ipPacket.SourceAddress.Address, 
                                   ipPacket.DestinationAddress.Address, 
                                   tcpPacket, e.Packet.Timeval);  

            if (recon.PacketWritten) //reassembly/reordering complete data was saved this time..
            {
                if (recon.LastSourcePort != tcpPacket.SourcePort) //previous entry is now complete so lets add it.
                {
                    AddNewNode(recon);
                    //book keeping for DataBlock since we are starting new packet stats now...
                    recon.LastSourcePort = tcpPacket.SourcePort;
                    recon.LastDestPort = tcpPacket.DestinationPort;
                    recon.LastSourceAddress = ipPacket.SourceAddress.ToString();
                    recon.LastDestinationAddress = ipPacket.DestinationAddress.ToString();
                }
            }

        }

        public void ProcessPcap()
        {

            sharpPcapDict = new Dictionary<Connection, TcpRecon>();
            PcapDevice device;
            totalPackets = 0;
            totalTCPPackets = 0;

            try
            {
                device  = new SharpPcap.LibPcap.CaptureFileReaderDevice(capFile);
                device.Open();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error Loading pcap with SharpPcap: " + ex.Message;
                owner.Invoke(Complete, ips);
                return; 
            }

            device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_PcapOnPacketArrival);
            device.Capture(); //parse entire pcap until EOF
            device.Close();
                
            foreach (TcpRecon tr in sharpPcapDict.Values)
            {
                tr.isComplete = true;
                if (tr.LastSavedOffset != tr.CurrentOffset) AddNewNode(tr);
                tr.Close();
            }

            sharpPcapDict.Clear();
            owner.Invoke(Complete, ips);
             
            return; 

        }
    }
}
