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
using System.Threading;

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
        DateTime startTime;
        DataBlock curdb = null;

        public Form1()
        {
            InitializeComponent();
        }

        #region reconManager callbacks
        public void NewStream(TcpRecon recon)
        {
            TreeNode n = tv.Nodes.Add(recon.HashCode, Path.GetFileName(recon.dumpFile));
            n.Tag = recon;
            tv.Refresh();
        }

        private void NewNode(DataBlock db)
        {
            string display = string.Format("{0:x},{1:x}, srcPort:{2}", db.startOffset, db.length, db.recon.LastSourcePort);
            TreeNode n = tv.Nodes[db.recon.HashCode];
            TreeNode nn = n.Nodes.Add(display);
            nn.Tag = db;
        }

        private void Complete()
        {
            //remove any top level nodes without children
            foreach (TreeNode n in tv.Nodes) if (n.Nodes.Count == 0) n.Remove();

            TimeSpan totalTime = (DateTime.Now - startTime);
            this.Text = string.Format("Total reconstruct time: {0} seconds", totalTime.TotalSeconds);

            //post processing here.. remove ssl?, extract http requests for summary? detect and handle gzip?

        }
        #endregion

        private void btnBrowsePcap_Click(object sender, EventArgs e)
        {
            if(dlg.ShowDialog() != DialogResult.OK) return;
            txtPcap.Text = dlg.FileName;
            btnParse_Click(sender, e);
        }

        private void btnParse_Click(object sender, EventArgs e)
        {

            startTime = DateTime.Now;        

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

            ReconManager rm = new ReconManager(NewStream, NewNode, Complete, capFile, outDir, this);
            Thread mThread = new Thread(new ThreadStart(rm.ProcessPcap));
            mThread.IsBackground = true;
            mThread.Start();
            
        }

        private void tv_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TcpRecon tr = null;
            TreeNode n = e.Node;
            bool viewOnly = true;

            if (curdb != null) { curdb.FreeData(); curdb = null; }

            if (n.Tag is TcpRecon) 
            {
                tr = (TcpRecon)n.Tag;
                if(he.LoadedFile != tr.dumpFile) he.LoadFile(ref tr.dumpFile, ref viewOnly);
            }
            else
            {
                DataBlock db = (DataBlock)n.Tag;
                curdb = db;

                if (he.LoadedFile != db.recon.dumpFile) he.LoadFile(ref db.recon.dumpFile, ref viewOnly);
                he.scrollTo(db.startOffset);
                he.set_SelStart(ref db.startOffset);
                he.set_SelLength(ref db.length);  
              
                if(tabs.SelectedTab.Name == "TextView") rtf.Text = curdb.AsString();


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
            
            if (n.Nodes.Count == 0)
            {
                MessageBox.Show("Not a parent node shouldnt happen");
                return;
            }

            int i = 0;
            foreach (TreeNode nn in n.Nodes)
            {
                DataBlock db = (DataBlock)nn.Tag;
                if ( db.LoadData() )
                {
                    db.SaveToFile(pDir + i + ".bin");
                    db.FreeData();
                }
                i++;
            }

            MessageBox.Show(string.Format("Extraction Complete {0}/{1} blocks extracted.", i, n.Nodes.Count));
        }

        private void scanForHTTPRequestsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            
            MessageBox.Show("todo");
            return;

            foreach (TreeNode n in tv.Nodes)
            {
                foreach (TreeNode nn in n.Nodes)
                {
                    //todo
                }
            }

        }

        private void tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (curdb == null) return;
            //hex tab always loaded
            if (tabs.SelectedTab.Name == "TextView") rtf.Text = curdb.AsString();
        }



    }
}
