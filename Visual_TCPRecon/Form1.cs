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
using Microsoft.VisualBasic;
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
        ListView selLV;

        public Form1()
        {
            InitializeComponent();
            Form1_Resize(null, null);
            lv.ContextMenuStrip = mnuLvPopup;
            lvDNS.ContextMenuStrip = mnuLvPopup;
            lvIPs.ContextMenuStrip = mnuLvPopup;
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

        private void Complete(List<string> ips)
        {
            ListViewItem li = null;
            List<TreeNode> rem = new List<TreeNode>();

            foreach (string s in ips) lvIPs.Items.Add(s);

            foreach (TreeNode n in tv.Nodes)
            {
                if (n.Nodes.Count == 0)
                {
                    rem.Add(n);
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
                                nn.Text = "   " + nn.Text + string.Format("   - 0x{0:x} bytes", db.length);
                                if (db.isGZip) nn.Text += " w/gzip";
                            }
                        }

                    }
                }
            }
            
            foreach (TreeNode n in rem) tv.Nodes.Remove(n);
             
            lvDNS.Columns[0].Text = "DNS Requests: " + lvDNS.Items.Count;
            lv.Columns[0].Text = "Web Requests: " + lv.Items.Count;
            TimeSpan totalTime = (DateTime.Now - startTime);
            this.Text = "  Pcap size: " + FileSizeToHumanReadable(txtPcap.Text) + string.Format("      Processing time: {0} seconds", totalTime.TotalSeconds);

        }

        private void DNS(cDNS dns)
        {
            lvDNS.Items.Add(dns.dnsName);
        }

        #endregion

        public static string InputBox(string msg){  return InputBox(msg, "", ""); }
        public static string InputBox(string msg, string title, string defaultVal)
        {
            string tmp = Interaction.InputBox(msg, title, defaultVal, -1, -1);
            if (tmp == null) tmp = "";
            return tmp;
        }

        private void btnBrowsePcap_Click(object sender, EventArgs e)
        {
            dlg.Filter = "Pcap files (*.pcap)|*.pcap";
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

            ReconManager rm = new ReconManager(NewStream, NewNode, Complete, DNS, capFile, outDir, this);
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

                if (tv.SelectedNode != null /*&& tv.SelectedNode.Nodes.Count > 0*/)
                {
                    contextMenuStrip1.Show(tv, e.Location);
                }
            }
        }

        private void removeStreamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for( int ndx = tv.Nodes.Count; ndx > 0; ndx--)
            {
              TreeNode node = tv.Nodes[ndx-1];
              if (node.Checked)
              {
                  foreach (TreeNode nn in node.Nodes)
                  {
                      foreach (ListViewItem li in lv.Items)
                      {
                          TreeNode n = (TreeNode)li.Tag;
                          if (n == nn)
                          {
                              lv.Items.Remove(li);
                              break;
                          }
                      }
                  }
                  tv.Nodes.Remove(node);
              }
            }
            lv.Columns[0].Text = "Web Requests: " + lv.Items.Count;
        }

        private void extractStreamsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            fDlg.SelectedPath = outDir;
            if(fDlg.ShowDialog() != DialogResult.OK) return;

            string pDir = fDlg.SelectedPath + "\\";
            
            int i = 0,j=0;
            foreach (TreeNode n in tv.Nodes)
            {
                if (n.Checked) //parent stream node, extract all its children
                {
                    foreach (TreeNode nn in n.Nodes)
                    {
                        DataBlock db = (DataBlock)nn.Tag;
                        if (db.LoadData())
                        {
                            db.SaveToFile(pDir + n.Text+ "_" + i + ".bin");
                            db.FreeData();
                            j++;
                        }
                        i++;
                    }
                }
                else //scan its subnodes to see if any of them are selected..
                {
                    foreach (TreeNode nn in n.Nodes)
                    {
                        if (nn.Checked)
                        {
                            DataBlock db = (DataBlock)nn.Tag;
                            if (db.LoadData())
                            {
                                db.SaveToFile(pDir + n.Text + "_" + i + ".bin");
                                db.FreeData();
                                j++;
                            }
                            i++;
                        }
                    }
                }
            }

            MessageBox.Show(string.Format("Extraction Complete {0}/{1} blocks extracted.", j, i));
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
                //lvDNS.Left = this.Width - lvDNS.Width - 20;
                lv.Width = this.Width - lv.Left - 20; // -lvDNS.Width;
                
                lv.Height = this.Height - lv.Top - 40;
                lvDNS.Height = lv.Height;
                lvIPs.Height = lv.Height;
                tv.Top = 75;
                tv.Height =  lv.Top  - tv.Top - 20  ;
                tabs.Top = tv.Top;

                tv.Width = this.Width - tv.Left - tabs.Width - 20;
                tabs.Left =  tv.Left+ tv.Width  + 10;

                tabs.Height = tv.Height;
                he.Height = tabs.Height - 40;
                rtf.Height = he.Height;

                lv.Columns[0].Width = lv.Width - 10;
                lvDNS.Columns[0].Width = lvDNS.Width - 10;

            }catch(Exception ex){}


        }

        private void lv_SelectedIndexChanged(object sender, EventArgs e)
        {
            selLV = lv;
            try
            {
                ListViewItem li = lv.SelectedItems[0];
                TreeNode n = (TreeNode)li.Tag;
                tv.CollapseAll();
                tv.SelectedNode = n;
                TvNodeClick(n);
                n.EnsureVisible();
                tabs_SelectedIndexChanged(null, null);
            }
            catch (Exception ex) { }

        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selLV == null) return;
            foreach (ListViewItem li in selLV.Items) li.Selected = true;
        }

        private void copySelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selLV == null) return;
            string tmp = "";
            foreach (ListViewItem li in selLV.SelectedItems)
            {
                tmp += li.Text + "\r\n";
            }
            Clipboard.Clear();
            if(tmp.Length > 0) Clipboard.SetText(tmp);
        }

        private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selLV == null) return;
            string tmp = "";
            foreach (ListViewItem li in selLV.Items)
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

            string result = String.Format("{0:0.##} {1}", len, sizes[order]);
            return result;
        }

        private void lvDNS_SelectedIndexChanged(object sender, EventArgs e)
        {
            selLV = lvDNS;
        }

        private void lvIPs_SelectedIndexChanged(object sender, EventArgs e)
        {
            selLV = lvIPs;
        }

        private void selectLikeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            string match = InputBox("Enter IP or port to select");
            if (match.Length == 0) return;
            foreach (TreeNode n in tv.Nodes)
            {
                n.Checked = (n.Text.IndexOf(match, StringComparison.CurrentCultureIgnoreCase) != -1); 
            }
        }

        private void invertSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (TreeNode n in tv.Nodes) n.Checked = !n.Checked;
        }

        private void clearSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (TreeNode n in tv.Nodes) n.Checked = false;
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void mnuCopyTable_Click(object sender, EventArgs e)
        {
            StringBuilder tmp = new StringBuilder("\r\n");

            foreach (TreeNode n in tv.Nodes)
            {
                tmp.Append(n.Text + "\r\n");
                foreach (TreeNode nn in n.Nodes)
                {
                    tmp.Append("\t"+ nn.Text + "\r\n");
                }
                tmp.Append("\r\n");
            }
            Clipboard.Clear();
            Clipboard.SetText(tmp.ToString());
        }

        

    }
}
