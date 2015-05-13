using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VTcpRecon;
using Tamir.IPLib.Packets;
using Tamir.IPLib;
using System.IO;
using System.Threading;

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
        private TCPPacket curPacket;
        private decimal firstTimeStamp = 0;

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
            db.EpochTimeStamp = curPacket.PcapHeader.Seconds.ToString() + "." + curPacket.PcapHeader.MicroSeconds.ToString();

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
            UDPPacket udp = (UDPPacket)packet;
            if (!IPExists("udp: "+udp.DestinationAddress)) ips.Add("udp: "+udp.DestinationAddress);
            if (!IPExists("udp: "+udp.SourceAddress)) ips.Add("udp: "+udp.SourceAddress);

            if (udp.DestinationPort == 53) //its a request
            {
                cDNS dns = new cDNS(udp.UDPData);
                if (!dns.isResponse && dns.dnsName.Length > 0)
                {
                    owner.Invoke(DNS, dns); 
                }
            }

        }

        // The callback function for the SharpPcap library
        private void device_PcapOnPacketArrival(object sender, Packet packet)
        {

            if (firstTimeStamp == 0)
            {
                firstTimeStamp = decimal.Parse(packet.Timeval.Seconds.ToString() + "." + packet.Timeval.MicroSeconds.ToString());
            }

            if (packet is UDPPacket)
            {
                HandleDNS(packet);
                return;
            }
            
            if (!(packet is TCPPacket)) return;

            TCPPacket tcpPacket = (TCPPacket)packet;
            Connection c = new Connection(tcpPacket);
            TcpRecon recon = null;
            curPacket = tcpPacket;

            if (!sharpPcapDict.ContainsKey(c))
            {
                c.generateFileName(outDir);
                recon = new TcpRecon(c.fileName);
                recon.LastSourcePort = tcpPacket.SourcePort;
                recon.StreamStartTimeStamp = packet.PcapHeader.Seconds.ToString() + "." + packet.PcapHeader.MicroSeconds.ToString();
                decimal curTime = decimal.Parse(recon.StreamStartTimeStamp);
                recon.relativeTimeStamp = (curTime - firstTimeStamp).ToString();

                sharpPcapDict.Add(c, recon);
                if (!IPExists("tcp: " + tcpPacket.DestinationAddress)) ips.Add("tcp: " + tcpPacket.DestinationAddress);
                if (!IPExists("tcp: " + tcpPacket.SourceAddress)) ips.Add("tcp: " + tcpPacket.SourceAddress);
                owner.Invoke(NewStream, recon); 
            }else{
                recon = sharpPcapDict[c];
            }
            
            recon.ReassemblePacket(tcpPacket);  //can contain fragments and out of order packets 

            if (recon.PacketWritten) //reassembly/reordering complete data was saved this time..
            {
                if (recon.LastSourcePort != tcpPacket.SourcePort) //previous entry is now complete so lets add it.
                {
                    AddNewNode(recon);
                    recon.LastSourcePort = tcpPacket.SourcePort;
                }
            }

        }

        public void ProcessPcap()
        {

            sharpPcapDict = new Dictionary<Connection, TcpRecon>();
            PcapDevice device;

            try
            {
                device = SharpPcap.GetPcapOfflineDevice(capFile);
                device.PcapOpen();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error Loading pcap with SharpPcap: " + ex.Message;
                owner.Invoke(Complete, ips);
                return; 
            }

            device.PcapOnPacketArrival += new SharpPcap.PacketArrivalEvent(device_PcapOnPacketArrival);
            device.PcapCapture(SharpPcap.INFINITE); //parse entire pcap until EOF
            device.PcapClose();

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
