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
 * 
 * todo: conglomerate subnodes by srd/dst ip and dst port, dont care if src port increments. 
 */

namespace Visual_TCPRecon
{

    public partial class Form1 : Form
    {
        static string outDir = "";
        DateTime startTime;
        DataBlock curdb = null;
        Object blankUrl = "about:blank";

        public Form1()
        {
            InitializeComponent();
            Form1_Resize(null, null);
            lv.ContextMenuStrip = mnuLvPopup;
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
            ListViewItem li = null;

            foreach (TreeNode n in tv.Nodes)
            {
                if (n.Nodes.Count == 0)
                {
                    n.Remove();//remove any top level nodes without children
                }
                else
                {
                    n.Text += "  (" + n.Nodes.Count + ")";
                    foreach (TreeNode nn in n.Nodes)
                    {
                        DataBlock db = (DataBlock)nn.Tag;
                        db.DetectType();
                        if (db.DataType != DataBlock.DataTypes.dtBinary)
                        {
                            nn.Text = db.HttpFirstLine;
                            if (db.DataType == DataBlock.DataTypes.dtHttpReq)
                            {
                                li = lv.Items.Add(db.HttpFirstLine);
                                li.Tag = nn;
                            }
                            else
                            {//we have some extra display room with just short HTTP response code, so lets use it..
                                nn.Text += string.Format("   - 0x{0:x} bytes", db.length);
                                if (db.isGZip) nn.Text += " w/gzip";
                            }
                        }
                    }
                }
            }

            TimeSpan totalTime = (DateTime.Now - startTime);
            this.Text = "  Pcap size: " + FileSizeToHumanReadable(txtPcap.Text) + string.Format("      Processing time: {0} seconds", totalTime.TotalSeconds);

            //post processing here.. remove ssl?, extract http requests for summary? detect and handle gzip?

        }
        #endregion

        private void btnBrowsePcap_Click(object sender, EventArgs e)
        {
            dlg.FileName = System.Diagnostics.Debugger.IsAttached ? "test.pcap" : "";
            if(dlg.ShowDialog() != DialogResult.OK) return;
            txtPcap.Text = dlg.FileName;
            btnParse_Click(sender, e);
        }

        private void btnParse_Click(object sender, EventArgs e)
        {

            string blank = "";
            tv.Nodes.Clear();
            he.LoadString(ref blank);
            lv.Items.Clear();

            startTime = DateTime.Now;        

            string capFile = txtPcap.Text;
            if (!System.IO.File.Exists(capFile))
            {
                MessageBox.Show("Pcap file not found: " + txtPcap.Text);
                return;
            }
 
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
            TvNodeClick(e.Node);
        }

        private void TvNodeClick(TreeNode n)
        {
            TcpRecon tr = null;
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

                tabs_SelectedIndexChanged(null, null);


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

        private void tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (curdb == null) return;
            //hex tab always loaded

            pict.Image = null;
            wb.Navigate2(ref blankUrl);
            string tmp = "";

            if (tabs.SelectedTab.Text == "TextView") rtf.Text = curdb.GetBody();
            
            if (tabs.SelectedTab.Text == "ImageView")
            {
                tmp = curdb.BinaryBodyToTmpFile();
                try { pict.Load(tmp); File.Delete(tmp); }
                catch (Exception ex) { };
            }

            if (tabs.SelectedTab.Text == "WebView")
            {
                Object oTmp = (Object)curdb.BinaryBodyToTmpFile();
                try { wb.Navigate2(ref oTmp); /*File.Delete((string)oTmp);*/ }
                catch (Exception ex) { };
            }

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
             try
            {
                 
                lv.Width = this.Width - lv.Left - 20;
                lv.Columns[0].Width = lv.Width - 10;
                lv.Height = this.Height - lv.Top - 40;
                tv.Top = 75;
                tv.Height =  lv.Top  - tv.Top - 20  ;
                tabs.Top = tv.Top;

                tv.Width = this.Width - tv.Left - tabs.Width - 20;
                tabs.Left =  tv.Left+ tv.Width  + 10;

                tabs.Height = tv.Height;
                he.Height = tabs.Height - 40;
                rtf.Height = he.Height;

                /*
                tabs.Width = this.Width - tabs.Left - 30;
                he.Width = tabs.Width - 20;
                rtf.Width = he.Width;
                */

            }catch(Exception ex){}


        }

        private void lv_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ListViewItem li = lv.SelectedItems[0];
                TreeNode n = (TreeNode)li.Tag;
                tv.SelectedNode = n;
                TvNodeClick(n);
                n.EnsureVisible();
                tabs_SelectedIndexChanged(null, null);
            }
            catch (Exception ex) { }

        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem li in lv.Items) li.Selected = true;
        }

        private void copySelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string tmp = "";
            foreach (ListViewItem li in lv.SelectedItems)
            {
                tmp += li.Text + "\r\n";
            }
            Clipboard.Clear();
            if(tmp.Length > 0) Clipboard.SetText(tmp);
        }

        private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string tmp = "";
            foreach (ListViewItem li in lv.Items)
            {
                tmp += li.Text + "\r\n";
            }
            Clipboard.Clear();
            if (tmp.Length > 0) Clipboard.SetText(tmp);
        }

        private void collapseTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tv.CollapseAll();
        }

        public string FileSizeToHumanReadable(string path)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = new FileInfo(path).Length;
            int order = 0;
            while (len >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            string result = String.Format("{0:0.##} {1}", len, sizes[order]);
            return result;
        }

    }
}
