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

namespace Visual_TCPRecon
{
    class ReconManager
    {
        public string ErrorMessage = "";
        static string outDir = "";
        static string capFile = "";
        private Form1 owner;

        static Dictionary<Connection, TcpRecon> sharpPcapDict = new Dictionary<Connection, TcpRecon>();

        public delegate void _NewStream(TcpRecon recon);
        public delegate void _NewNode(DataBlock db);
        public delegate void _Complete();

        public _NewStream NewStream = null;
        public _NewNode NewNode = null;
        public _Complete Complete = null;

        public ReconManager(_NewStream ns, _NewNode nn, _Complete c,  string pcapFile, string outputDir, Form1 parent)
        {
            NewStream = ns;
            NewNode = nn;
            Complete = c;
            outDir = outputDir;
            capFile = pcapFile;
            owner = parent;
        }

        private void AddNewNode(TcpRecon recon)
        {
            int startAt = (int)recon.LastSavedOffset;
            int endAt = (int)recon.PreviousPacketEndOffset;
            if (recon.isComplete) endAt =(int)recon.CurrentOffset;

            DataBlock db = new DataBlock(recon.dumpFile, startAt, endAt - startAt, recon);
            owner.Invoke(NewNode, db);

            recon.LastSavedOffset = recon.PreviousPacketEndOffset;
        }

        // The callback function for the SharpPcap library
        private void device_PcapOnPacketArrival(object sender, Packet packet)
        {
            if (!(packet is TCPPacket)) return;

            TCPPacket tcpPacket = (TCPPacket)packet;
            Connection c = new Connection(tcpPacket);
            TcpRecon recon = null;

            if (!sharpPcapDict.ContainsKey(c))
            {
                c.generateFileName(outDir);
                recon = new TcpRecon(c.fileName);
                recon.LastSourcePort = tcpPacket.SourcePort;
                sharpPcapDict.Add(c, recon);
                owner.Invoke(NewStream,recon);
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
                return;// false;
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
            owner.Invoke(Complete);
            return;// true;

        }
    }
}
