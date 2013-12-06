using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tamir.IPLib.Packets;
using Tamir.IPLib;
using System.IO;
using VTcpRecon;

/*
 *  This code was modified by dzzie@yahoo.com from the base at:
 *  
 *  TCP Session Reconstruction Tool  
 *  author  : Saar Yahalom, 21 Sep 2007 
 *  original: http://www.codeproject.com/Articles/20501/TCP-Session-Reconstruction-Tool
 *  license : http://www.codeproject.com/info/cpol10.aspx
 * 
 *  dependancies: (included) make sure to run the cmd "regsvr hexed.ocx"
 *     Tamir.IPLib.SharpPcap.dll which contains:
 *          PacketDotNet   http://sourceforge.net/apps/mediawiki/packetnet/index.php?title=Main_Page
 *          SharpPcap      http://sourceforge.net/apps/mediawiki/sharppcap/index.php?title=Main_Page
 *     hexed.ocx -         https://github.com/dzzie/hexed
 *     managedLibnids.dll  http://www.codeproject.com/KB/IP/TcpRecon/Libnids-119_With_managedLibnids.zip
 *     winpcap             http://www.winpcap.org/
 *  
 */

namespace Visual_TCPRecon
{

    public partial class Form1 : Form
    {
        
        static string outDir = "";

        // Holds the file streams for each tcp session in case we use libnids
        static Dictionary<Connection, FileStream> nidsDict;
        static Dictionary<Connection, TcpRecon> sharpPcapDict = new Dictionary<Connection, TcpRecon>();
       

        public Form1()
        {
            InitializeComponent();
        }

        #region "Callback Functions"

        // The callback function for the managedLibnids library
        static void handleData(byte[] arr, UInt32 sourceIP, UInt16 sourcePort, UInt32 destinationIP, UInt16 destinationPort, bool urgent)
        {
            System.Net.IPAddress srcIp = new System.Net.IPAddress(sourceIP);
            System.Net.IPAddress dstIp = new System.Net.IPAddress(destinationIP);
            
            Connection c = new Connection(srcIp.ToString(), sourcePort, dstIp.ToString(), destinationPort);

            // create a new entry if the key does not exists
            if (!nidsDict.ContainsKey(c))
            {
                c.generateFileName(outDir);
                FileStream fStream = new FileStream(c.fileName, FileMode.Create);
                nidsDict.Add(c, fStream);
            }

            // write the new data to file
            nidsDict[c].Write(arr, 0, arr.Length); 
        }
        
        // The callback function for the SharpPcap library
        private void device_PcapOnPacketArrival(object sender, Packet packet)
        {
            if (!(packet is TCPPacket)) return;

            TCPPacket tcpPacket = (TCPPacket)packet;
            Connection c = new Connection(tcpPacket);
            TcpRecon recon = null;
            TreeNode n = null;

            if (!sharpPcapDict.ContainsKey(c))
            {
                c.generateFileName(outDir);
                recon = new TcpRecon(c.fileName);
                recon.LastSourcePort = tcpPacket.SourcePort;
                sharpPcapDict.Add(c, recon);
                n = tv.Nodes.Add(recon.HashCode, Path.GetFileName(c.fileName));
                n.Tag = recon;
            }else{
                recon = sharpPcapDict[c];
            }
            
            //long startAt = recon.CurrentOffset; //before any packet writes
            recon.ReassemblePacket(tcpPacket);  //can contain fragments and out of order packets 

            if (recon.PacketWritten) //reassembly/reordering complete data was saved this time..
            {
                if (recon.LastSourcePort != tcpPacket.SourcePort) //previous entry is now complete so lets add it.
                {
                    AddSubNode(recon);
                    recon.LastSourcePort = tcpPacket.SourcePort;
                }

                /*
                long endAt = recon.CurrentOffset;
                string display = string.Format("{0:x},{1:x}, sz:{2:x} srcPort:{3}", startAt, endAt, endAt - startAt, tcpPacket.SourcePort);
                n = tv.Nodes[recon.HashCode];
                n.Nodes.Add(display);*/

                 
            }

        }
        #endregion

        private void AddSubNode(TcpRecon recon)
        {
            TreeNode n = null;
            long startAt = recon.LastSavedOffset;
            long endAt = recon.PreviousPacketEndOffset;
            if (recon.isComplete) endAt = recon.CurrentOffset;

            int srcPort = recon.LastSourcePort;
            string display = string.Format("{0:x},{1:x}, srcPort:{2}", startAt, endAt - startAt, srcPort);

            n = tv.Nodes[recon.HashCode];
            n.Nodes.Add(display);
            recon.LastSavedOffset = recon.PreviousPacketEndOffset;

            tv.Refresh();
            this.Refresh();
        }

        private void btnBrowsePcap_Click(object sender, EventArgs e)
        {
            if(dlg.ShowDialog() != DialogResult.OK) return;
            txtPcap.Text = dlg.FileName;
            btnParse_Click(sender, e);
        }

        private void btnParse_Click(object sender, EventArgs e)
        {

            DateTime startTime = DateTime.Now;        

            string capFile = txtPcap.Text;
            if (!System.IO.File.Exists(capFile))
            {
                MessageBox.Show("Pcap file not found: " + txtPcap.Text);
                return;
            }

            string blank = "";
            tv.Nodes.Clear();
            he.LoadString(ref blank);

            string baseName = Path.GetFileNameWithoutExtension(capFile);
            outDir = Directory.GetParent(capFile).FullName;
            outDir += "\\" + baseName;
            if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);
            outDir += "\\";

            this.Text = "Loading pcap file...";
            this.Refresh();

            if (chkUselibnids.Checked)
            {
                MessageBox.Show("I havent done any updates on this block of code yet..");
                nidsDict = new Dictionary<Connection, FileStream>();
                managedLibnids.LibnidsWrapper.Run(capFile, new DataCallbackDelagate(handleData), new DataCallbackDelagate(handleData));
                foreach (FileStream fs in nidsDict.Values){fs.Close();}
                nidsDict.Clear();
            }
            else
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
                    MessageBox.Show("Error Loading pcap with SharpPcap: " + ex.Message);
                    return;
                }

                device.PcapOnPacketArrival += new SharpPcap.PacketArrivalEvent(device_PcapOnPacketArrival);
                device.PcapCapture(SharpPcap.INFINITE); //parse entire pcap until EOF
                device.PcapClose();

                foreach (TreeNode n in tv.Nodes)
                {
                    TcpRecon tr = (TcpRecon)n.Tag;
                    tr.isComplete = true;
                    if (tr.LastSavedOffset != tr.CurrentOffset) AddSubNode(tr);
                }

                foreach (TcpRecon tr in sharpPcapDict.Values){tr.Close();}
                sharpPcapDict.Clear();
            }

            //remove any top level nodes without children..
            foreach (TreeNode n in tv.Nodes) if (n.Nodes.Count == 0) n.Remove();

            DateTime finishTime = DateTime.Now;
            TimeSpan totalTime = (finishTime - startTime);

            this.Text = string.Format("Total reconstruct time: {0} seconds", totalTime.TotalSeconds);

        }

        private void tv_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TcpRecon tr = null;
            TreeNode n = e.Node;
            bool viewOnly = true;

            if (n.Nodes.Count > 0) 
            {
                tr = (TcpRecon)n.Tag;
                if(he.LoadedFile != tr.dumpFile) he.LoadFile(ref tr.dumpFile, ref viewOnly);
            }
            else
            {
                string[] sOff = n.Text.Split(',');
                int startAt = Convert.ToInt32(sOff[0], 16);
                int selLen = Convert.ToInt32(sOff[1], 16);

                tr = (TcpRecon)n.Parent.Tag;
                if (he.LoadedFile != tr.dumpFile) he.LoadFile(ref tr.dumpFile, ref viewOnly);

                he.scrollTo(startAt);
                he.set_SelStart(ref startAt);
                he.set_SelLength(ref selLen);
                
            }

        }

        private void tv_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                // Select the clicked node
                tv.SelectedNode = tv.GetNodeAt(e.X, e.Y);

                if (tv.SelectedNode != null && tv.SelectedNode.Nodes.Count > 0)
                {
                    contextMenuStrip1.Show(tv, e.Location);
                }
            }
        }

        private void removeStreamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode n = tv.SelectedNode;
            if (n != null) n.Remove();
        }

        private void extractStreamsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode n = tv.SelectedNode;
            if (n == null) return;

            fDlg.SelectedPath = outDir;
            if(fDlg.ShowDialog() != DialogResult.OK) return;

            string pDir = fDlg.SelectedPath + "\\";
            TcpRecon recon = null;
            if (n.Nodes.Count == 0)
            {
                MessageBox.Show("Not a parent node shouldnt happen");
                return;
            }

            recon = (TcpRecon)n.Tag;

            using (BinaryReader br = new BinaryReader(File.Open(recon.dumpFile, FileMode.Open)))
            {
                int i = 0;
                int maxSize = (int)br.BaseStream.Length;

                foreach (TreeNode nn in n.Nodes)
                {
                    string[] sOff = nn.Text.Split(',');
                    int startAt = Convert.ToInt32(sOff[0], 16);
                    int selLen = Convert.ToInt32(sOff[1], 16);

                    if (startAt + selLen > maxSize){
                        selLen = maxSize - startAt;
                    }

                    if (selLen > 0)
                    {
                        byte[] b = new byte[selLen];
                        br.BaseStream.Seek(startAt, SeekOrigin.Begin);
                        b = br.ReadBytes(selLen);
                        using (BinaryWriter bw = new BinaryWriter(File.Open(pDir + i + ".bin", FileMode.Create)))
                        {
                            bw.Write(b);
                        }
                    }
                    i++;
                }


            }


        }



    }
}
