using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpPcap.WinPcap;
using SharpPcap.LibPcap;
using PacketDotNet.Tcp;
using System.IO;
using VTcpRecon;
using System.Threading;
using Microsoft.VisualBasic;
using Visual_TCPRecon.Interfaces;
using System.Diagnostics;
using System.IO.Compression;

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
        public DataBlock curdb = null;
        Object blankUrl = "about:blank";
        ListView selLV;
        TreeNode curNode;
        ReconManager rm;

        public Form1()
        {
            InitializeComponent();
            Form1_Resize(null, null);
            lv.ContextMenuStrip = mnuLvPopup;
            lvFiltered.ContextMenuStrip = mnuLvPopup;
            lvDNS.ContextMenuStrip = mnuLvPopup;
            lvIPs.ContextMenuStrip = mnuLvPopup;

            string[] args = Environment.GetCommandLineArgs();
            foreach (string a in args)
            {
                if (File.Exists(a) && Path.GetExtension(a).ToLower() == ".pcap")
                {
                    txtPcap.Text = a;
                    btnParse_Click(null,null);
                    break;
                }
            }
        }

        //#region reconManager callbacks
        private string getParentNodeName(TcpRecon recon) {
            string nText = Path.GetFileName(recon.dumpFile);
            return getParentNodeName(nText);
        }

        private string getParentNodeName(string nText) 
        {
            if (ConglomerateToolStripMenuItem.Checked)
            {
                int a = nText.LastIndexOf("  ");
                int b = nText.LastIndexOf("--");
                if (a > 0)
                {
                    string temp = nText.Substring(0, a);
                    if (b > 0) temp += " --" + nText.Substring(b + 2);
                    nText = temp;
                }
            }
            return nText;
        }

        public void NewStream(TcpRecon recon)
        {
            TreeNode n = null;
            string nText = getParentNodeName(recon);
            n = tv.Nodes.Add(recon.HashCode, nText);
            n.Tag = recon;
            tv.Refresh();
        }

        private void NewNode(DataBlock db)
        {
            TreeNode n = null;
            string display = string.Format("{0:x},{1:x}, srcPort:{2}", db.startOffset, db.length, db.recon.LastSourcePort);
            n = tv.Nodes[db.recon.HashCode];
            TreeNode nn = n.Nodes.Add(display);
            nn.Tag = db;
        }

        private void Complete(List<string> ips)
        {
            ListViewItem li = null;
            List<TreeNode> rem = new List<TreeNode>();
            int ii = 0;

            if (rm.ErrorMessage.Length > 0)
            {
                this.Text = rm.ErrorMessage;
                this.Refresh();
                return;
            }

            this.Text = "Loading Complete now parsing...";
            this.Refresh();

            foreach (string s in ips) lvIPs.Items.Add(s);

            if (ConglomerateToolStripMenuItem.Checked)
            {
                this.Text = "Conglomerating streams option checked...";
                this.Refresh();
                Application.DoEvents();

            startOver:
                ii = 0;

                foreach (TreeNode n in tv.Nodes)
                {
                    ii++;
                    if (ii % 2 == 0) setpb(ii, tv.Nodes.Count);

                    foreach (TreeNode n2 in tv.Nodes)
                    {
                        if (n2 != n)
                        {
                            if (n2.Text == n.Text)
                            {
                                foreach (TreeNode n3 in n2.Nodes)
                                {
                                    TreeNode n4 = n.Nodes.Add(n3.Text);
                                    n4.Tag = n3.Tag;
                                }
                                tv.Nodes.Remove(n2);
                                goto startOver; //or you get invalid object reference..removing from treeviews is tricky..
                            }
                        }
                    }
                }
            }

            this.Text = "Scanning for http content..";
            this.Refresh();
            ii = 0;

            foreach (TreeNode n in tv.Nodes)//parent node shows stream info..
            {
                ii++;
                if (ii % 2 == 0) setpb(ii, tv.Nodes.Count);
                int st = Environment.TickCount;

                if (n.Nodes.Count == 0)
                {
                    rem.Add(n);
                }
                else
                {
                    n.Text += "  (" + n.Nodes.Count + ")";
                    /*tv.Refresh();
                    this.Refresh();
                    Application.DoEvents();*/

                    setpb(0, 0, 2);
                    int ni = 0;
                    foreach (TreeNode nn in n.Nodes) //each subnode holds the actual data stream details..
                    {
                        //if (System.Diagnostics.Debugger.IsAttached && (Environment.TickCount - st) > 5000) System.Diagnostics.Debugger.Break();
                        ni++;
                        if(ni%10==0) setpb(ni, n.Nodes.Count,2);

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
                                if (db.isChunked) nn.Text += " chunked ";
                                if (db.isGZip) nn.Text += " w/gzip";
                            }
                        }

                    }
                }
            }

            this.Text = "Pruning tree...";
            this.Refresh();
            foreach (TreeNode n in rem) tv.Nodes.Remove(n);
             
            lvDNS.Columns[0].Text = "DNS Requests: " + lvDNS.Items.Count;
            lv.Columns[0].Text = "Web Requests: " + lv.Items.Count;
            TimeSpan totalTime = (DateTime.Now - startTime);
            this.Text = "  Pcap size: " + FileSizeToHumanReadable(txtPcap.Text) + string.Format("      Processing time: {0} seconds", totalTime.TotalSeconds);
            pb.Value = 0;
            pb2.Value = 0;

        }

        public void setpb(double cur, double max)
        {
            setpb(cur, max,1);
        }

        public void setpb(double cur, double max, int index)
        {
            ProgressBar p = index == 2 ? pb2 : pb;
            double pcent = cur / max * 100 ;
            if (Double.IsNaN(pcent)) pcent = 0;
            if (pcent > 100) pcent = 100;
            p.Value = (int)pcent;
            p.Refresh();
            this.Refresh();
            Application.DoEvents();
        }

        private void DNS(cDNS dns)
        {
            lvDNS.Items.Add(dns.dnsName);
        }

        //#endregion

        public static string InputBox(string msg){  return InputBox(msg, "", ""); }
        public static string InputBox(string msg, string title, string defaultVal)
        {
            string tmp = Interaction.InputBox(msg, title, defaultVal, -1, -1);
            if (tmp == null) tmp = "";
            return tmp;
        }

        private void btnBrowsePcap_Click(object sender, EventArgs e)
        {
            if (txtPcap.Text.Length > 0)
            {
                try
                {
                    dlg.InitialDirectory = Path.GetDirectoryName(txtPcap.Text);
                }
                catch (Exception ee) { ;}
            }
            dlg.Filter = "Pcap files (*.pcap)|*.pcap";
            dlg.FileName = System.Diagnostics.Debugger.IsAttached ? "test.pcap" : "";
            if(dlg.ShowDialog() != DialogResult.OK) return;
            txtPcap.Text = dlg.FileName;

            FileInfo f = new FileInfo(dlg.FileName);
            long s1 = f.Length;

            if (s1 > 8000000) //about 8mb
                    this.Text = "This file is large, consider splitting it into smaller pcaps using Tools menu...";
            else
                btnParse_Click(sender, e);

        }

        private void btnParse_Click(object sender, EventArgs e)
        {


            string blank = "";
            tv.Nodes.Clear();
            he.LoadString(ref blank);
            lv.Items.Clear();
            lvIPs.Items.Clear();
            lvFiltered.Items.Clear();
            lvDNS.Items.Clear();

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

            rm = new ReconManager(NewStream, NewNode, Complete, DNS, capFile, outDir, this);
            Thread mThread = new Thread(new ThreadStart(rm.ProcessPcap));
            mThread.IsBackground = true;
            mThread.Start();


            
        }

        public void setNodeColor(TreeNode n, int color)
        {
            switch (color)
            {
                case 1: n.BackColor = Color.Red; break;
                case 2: n.BackColor = Color.Yellow; break;
                case 3: n.BackColor = Color.White; break;
            }
        }

        private void tv_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TvNodeClick(e.Node);
        }

        public void TvNodeClick(TreeNode n)
        {
            TcpRecon tr = null;
            bool viewOnly = true;
            
            curNode = n;

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
            for (int ndx = tv.Nodes.Count; ndx > 0; ndx--)
            {
                TreeNode node = tv.Nodes[ndx - 1];
                bool parentSelected = node.Checked;

                for (int ndx2 = node.Nodes.Count; ndx2 > 0; ndx2--)
                {
                    TreeNode nn = node.Nodes[ndx2 - 1];
                    if (nn.Checked || parentSelected)
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
                        nn.Remove();
                    }
                }
                if (node.Nodes.Count==0) tv.Nodes.Remove(node); //no children left..remove it..
                
            }
            lv.Columns[0].Text = "Web Requests: " + lv.Items.Count;
            if (txtFilter.Text.Length > 0) txtFilter_TextChanged(null, null);

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

        public void tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (curdb == null) return;
            //hex tab always loaded

            pict.Image = null;
            wb.Navigate2(ref blankUrl);
            string tmp = "";

            if (tabs.SelectedTab.Text == "TextView")
            {
                rtf.Text = ""; 
                rtf.SelectionStart = 0;
                rtf.Text = curdb.GetBody().Replace('\0',' ');
            }

            if (tabs.SelectedTab.Text == "Details") txtDetails.Text = curdb.GetDetails();

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


                lvDNS.Height = this.Height - lv.Top - 40;
                lvIPs.Height = this.Height - lv.Top - 40;
                lv.Height = this.Height - lv.Top - 40 - txtFilter.Height;

                tv.Top = 75;
                tv.Height =  lv.Top  - tv.Top - 20  ;
                tabs.Top = tv.Top;

                tv.Width = this.Width - tv.Left - tabs.Width - 20;
                tabs.Left =  tv.Left+ tv.Width  + 10;

                tabs.Height = tv.Height;
                he.Height = tabs.Height - 40;
                rtf.Height = he.Height;
                txtDetails.Height = he.Height;

                lv.Columns[0].Width = lv.Width - 10;
                lvDNS.Columns[0].Width = lvDNS.Width - 10;

                MatchSize(lv, lvFiltered);
                txtFilter.Top = lv.Top + lv.Height +5 ;
                lblFilter.Top = txtFilter.Top;

            }catch(Exception ex){}


        }

        private void MatchSize(ListView lv, ListView lv2)
        {
            lv2.Top = lv.Top;
            lv2.Left = lv.Left;
            lv2.Width = lv.Width;
            lv2.Height = lv.Height;
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
                foreach (TreeNode nn in n.Nodes) nn.Checked = (nn.Text.IndexOf(match, StringComparison.CurrentCultureIgnoreCase) != -1); 
            }
        }

        private void invertSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (TreeNode n in tv.Nodes)
            {
                n.Checked = !n.Checked;
                foreach (TreeNode nn in n.Nodes) nn.Checked = !nn.Checked;
            }
        }

        private void clearSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (TreeNode n in tv.Nodes)
            {
                n.Checked = false;
                foreach (TreeNode nn in n.Nodes) nn.Checked = false;
            }
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

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (TreeNode n in tv.Nodes) n.Expand();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (txtPcap.Text.Length == 0)
            { //no command line args..
                if (File.Exists(Visual_TCPRecon.Properties.Settings.Default.lastPath)) txtPcap.Text = Visual_TCPRecon.Properties.Settings.Default.lastPath;
            }
            ConglomerateToolStripMenuItem.Checked = Visual_TCPRecon.Properties.Settings.Default.byPort;
             
        }

        private void runScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // This is exposed to the scripts
            IScriptableComponent component = new DummyComponent();
            component.Parent = this;

            //var compiledAssemblyPath = Path.Combine(Environment.CurrentDirectory, ScriptsDirectory, CompiledScriptsAssemblyName);
            //var scriptFiles = Directory.EnumerateFiles(ScriptsDirectory, "*.cs", SearchOption.AllDirectories).ToArray();

            dlg.Filter = "CSharp Script files (*.cs)|*.cs";
            dlg.FileName = "";// System.Diagnostics.Debugger.IsAttached ? "test.pcap" : "";

            string scriptDir = Application.StartupPath;

            if(Directory.Exists(scriptDir + "\\Visual_TCPRecon\\Scripts\\")){
                scriptDir += "\\Visual_TCPRecon\\Scripts\\";
            }else{
                for (int i = 0; i < 5; i++)
                {
                    if (!Directory.Exists(scriptDir + "\\Scripts")) scriptDir = Path.GetDirectoryName(scriptDir);
                }
                if (Directory.Exists(scriptDir + "\\Scripts")) scriptDir = scriptDir + "\\scripts";
            }

            dlg.InitialDirectory = scriptDir;
            if (dlg.ShowDialog() != DialogResult.OK) return;

            System.Reflection.Assembly scriptAssembly;
            try
            {
                scriptAssembly = Helper.CompileAssembly(dlg.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error compiling script: " + ex.Message);
                return;
            }

            try
            {
                // Find all types that implement the IScript interface in the compiled assembly
                var scriptTypes = Helper.GetTypesImplementingInterface(scriptAssembly, typeof(IScript));

                foreach (var scriptType in scriptTypes)
                {
                    // Creates instances of type and pass component to the constructor
                    var script = (IScript)Activator.CreateInstance(scriptType);
                    script.Run(component);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error executing script: " + ex.Message);
                return;
            }
        }

        private void tv_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (TreeNode n in tv.Nodes)
            {
                n.Checked = false;
                foreach (TreeNode nn in n.Nodes) nn.Checked = false;
            }
        }

        private void parentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (TreeNode n in tv.Nodes) n.Checked = false;
        }

        private void childrenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (TreeNode n in tv.Nodes)
            {
                foreach (TreeNode nn in n.Nodes) nn.Checked = false;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Visual_TCPRecon.Properties.Settings.Default.byPort = ConglomerateToolStripMenuItem.Checked;
            Visual_TCPRecon.Properties.Settings.Default.lastPath = txtPcap.Text;
            Properties.Settings.Default.Save();
        }

        private void parentsWChildrenSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (TreeNode n in tv.Nodes)
            {
                bool childSelected = false;
                foreach (TreeNode nn in n.Nodes)
                {
                    if (nn.Checked) { childSelected = true; break; }
                }
                if (childSelected) n.Checked = false;
            }
        }

        private void seperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConglomerateToolStripMenuItem.Checked = !ConglomerateToolStripMenuItem.Checked;
        }

        private void allToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (TreeNode n in tv.Nodes)
            {
                n.Checked = true;
                foreach (TreeNode nn in n.Nodes) nn.Checked = true;
            }
        }

        private void parentsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (TreeNode n in tv.Nodes) n.Checked = true;
        }

        private void childrenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (TreeNode n in tv.Nodes)
            {
                foreach (TreeNode nn in n.Nodes) nn.Checked = true;
            }
        }

        private void parentsWChildrenSelectedToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (TreeNode n in tv.Nodes)
            {
                bool childSelected = false;
                foreach (TreeNode nn in n.Nodes)
                {
                    if (nn.Checked) { childSelected = true; break; }
                }
                if (childSelected) n.Checked = true;
            }
        }

        private void renameIPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string def="";
            if (curNode != null) { def = curNode.Text; }
            def = InputBox("Rename IP\n\nFormat IP,NewName", "RenameIP", def);
            if (def.Length == 0) return;

            string[] parts = def.Split("->"); //extension method..

            if (parts.Length != 2)
            {
                MessageBox.Show("Invalid format");
                return;
            }

            parts[0] = parts[0].Trim();
            parts[1] = parts[1].Trim();
            
            if (parts[0].Length == 0 || parts[1].Length == 0)
            {
                MessageBox.Show("cannot replace with empty string");
                return;
            }

            foreach (TreeNode n in tv.Nodes)
            {
                if(n.Text.IndexOf(parts[0]) >= 0) n.Text = n.Text.Replace(parts[0], parts[1]); 
            }

        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string match = InputBox("Search for");
            if (match.Length == 0) return;
            FrmList fl = new FrmList();
            fl.parent = this;
            fl.lv.Items.Clear();

            foreach (ListViewItem li in selLV.Items)
            {
                if (li.Text.IndexOf(match, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    ListViewItem li2 = fl.lv.Items.Add(li.Text);
                    li2.Tag = li.Tag;
                }
            }

            fl.Show();


           
        }

        private void splitLargePCAPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string tcpDump = Application.StartupPath;

            for (int i = 0; i < 5; i++)
            {
                if (!File.Exists(tcpDump + "\\tcpdump.exe")) tcpDump = Path.GetDirectoryName(tcpDump);
            }

            if(!File.Exists(tcpDump + "\\tcpdump.exe")){
                MessageBox.Show("Could not locate tcpdump in: " + tcpDump);
                return;
            }

            tcpDump += "\\tcpdump.exe";
            
            dlg.Filter = "Pcap files (*.pcap)|*.pcap";
            dlg.FileName = txtPcap.Text;
            if (dlg.ShowDialog() != DialogResult.OK) return;

            string inFile = dlg.FileName;
            string outDir = Path.GetDirectoryName(inFile) + "\\pcap_split\\";
            if(!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);
            
            string outFile = outDir + "\\split.pcap";

            //tcpdump -r old_file -w new_files -C 10
            ShellAndWait(tcpDump, "-r \"" + inFile + "\" -w \"" + outFile + "\" -C 5");

            string[] files = Directory.GetFiles(outDir);
            int cnt=0;
            foreach (string f in files)
            {
                if (Path.GetExtension(f) != ".pcap")
                {
                    string i = Path.GetExtension(f).Replace(".pcap", "");
                    string n = Path.GetDirectoryName(f) + "\\split" + i + ".pcap";
                    if(File.Exists(n)) File.Delete(n);
                    File.Move(f,n);
                    cnt++;
                }
            }
            MessageBox.Show("File has been split into " + (cnt+1)  + " segments");

            txtPcap.Text = outDir + "\\split.pcap";


        }

        public void ShellAndWait(string binPath, string args)
        {
            ProcessStartInfo pInfo = new ProcessStartInfo();
            pInfo.FileName = binPath;
            pInfo.Arguments = args;
            Process p = Process.Start(pInfo);
            //p.WaitForInputIdle();
            p.WaitForExit();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            if (txtFilter.Text.Length == 0) { lvFiltered.Visible = false; return; }
            lvFiltered.Visible = true;
            lvFiltered.Items.Clear();

            foreach (ListViewItem li in lv.Items)
            {
                if (li.Text.IndexOf(txtFilter.Text, StringComparison.CurrentCultureIgnoreCase) != -1)
                {
                    ListViewItem li2 = lvFiltered.Items.Add(li.Text);
                    li2.Tag = li.Tag;
                }
            }

            lvFiltered.Columns[0].Text = "Filtered Web Requests: " + lvFiltered.Items.Count;

        }

        private void lvFiltered_SelectedIndexChanged(object sender, EventArgs e)
        {
            selLV = lvFiltered;
            try
            {
                ListViewItem li = lvFiltered.SelectedItems[0];
                TreeNode n = (TreeNode)li.Tag;
                tv.CollapseAll();
                tv.SelectedNode = n;
                TvNodeClick(n);
                n.EnsureVisible();
                tabs_SelectedIndexChanged(null, null);
            }
            catch (Exception ex) { }

        }

        private void removeUncheckedStreamsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            for (int ndx = tv.Nodes.Count; ndx > 0; ndx--)
            {
                TreeNode node = tv.Nodes[ndx - 1];
                bool removeParent = true;

                if (!node.Checked)
                {
                    for (int ndx2 = node.Nodes.Count; ndx2 > 0; ndx2--)
                    {
                        TreeNode nn = node.Nodes[ndx2 - 1];
                        if (nn.Checked) removeParent = false;
                        if (!nn.Checked)
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
                            nn.Remove();
                        }
                    }
                    if(removeParent) tv.Nodes.Remove(node);
                }
            }
            lv.Columns[0].Text = "Web Requests: " + lv.Items.Count;
            if (txtFilter.Text.Length > 0) txtFilter_TextChanged(null, null);
        }

        public void DecompressFile()
        {

            dlg.Filter = "All files (*.*)|*.*";
            dlg.FileName = "";
            if (dlg.ShowDialog() != DialogResult.OK) return;

            string inFile = dlg.FileName;
            string outDir = Path.GetDirectoryName(inFile);
            string outFile = outDir + "\\" + Path.GetFileNameWithoutExtension(dlg.FileName) + ".decomp";


            byte[] gzip = File.ReadAllBytes(inFile);

            try
            {
                // Create a GZIP stream with decompression mode.
                // ... Then create a buffer and write into while reading from the GZIP stream.
                using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
                {
                    const int size = 4096;
                    byte[] buffer = new byte[size];
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = stream.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        //return memory.ToArray();
                        File.WriteAllBytes(outFile, memory.ToArray());
                        MessageBox.Show("Decompression success");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error decompressing stream?");
            }


        }

        private void gZIPDecompressFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DecompressFile();
        }

        private void unchunkExportedBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string tcpDump = Application.StartupPath;

            for (int i = 0; i < 5; i++)
            {
                if (!File.Exists(tcpDump + "\\unchunk.exe")) tcpDump = Path.GetDirectoryName(tcpDump);
            }

            if (!File.Exists(tcpDump + "\\unchunk.exe"))
            {
                MessageBox.Show("Could not locate unchunk.exe in: " + tcpDump);
                return;
            }

            tcpDump += "\\unchunk.exe";

            ProcessStartInfo pInfo = new ProcessStartInfo();
            pInfo.FileName = tcpDump;

            try
            {
                pInfo.Arguments = Path.GetDirectoryName(txtPcap.Text);
            }
            catch (Exception ex) {
                /*
                   this is the piddley shit throw an error over everything crap i hate about 
                   .net. also this is where on error resume next is very useful....
                   who the fuck cares if this fails due to bad path or empty or whatever
                 */
            }

            Process p = Process.Start(pInfo);


        }

        private void searchContentBodyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pb.Value = 0;
            pb2.Value = 0;
            int i = 0, j = 0;

            string match = InputBox("Search for");
            if (match.Length == 0) return;
            FrmList fl = new FrmList();
            fl.parent = this;
            fl.lv.Items.Clear();
            
            foreach (TreeNode n in tv.Nodes)
            {
                i++;
                setpb(i, tv.Nodes.Count, 1);
                n.Checked = false;

                foreach (TreeNode nn in n.Nodes)
                {
                    j++;
                    setpb(j, n.Nodes.Count, 2);
                    nn.Checked = false;

                    DataBlock db = (DataBlock)nn.Tag;
                    if (db.LoadData())
                    {
                        string body = db.HttpHeader + "\r\n\r\n" + db.GetBody();

                        if (body.IndexOf(match, StringComparison.CurrentCultureIgnoreCase) > 0)
                        {
                            string txt = db.HttpFirstLine;
                            if (txt.Length == 0) txt = "Data: " + body.Length.ToString();
                            setNodeColor(nn, 1);
                            setNodeColor(n, 2);
                            nn.Checked = true;
                            ListViewItem li2 = fl.lv.Items.Add(txt);
                            li2.Tag = nn;
                        }

                        db.FreeData();
                    }
                }
            }
            pb.Value = 0;
            pb2.Value = 0;
            fl.Show();
        }

        private void resetColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (TreeNode n in tv.Nodes)
            {
                n.BackColor = Color.White;
                foreach (TreeNode nn in n.Nodes)
                {
                    nn.BackColor = Color.White;
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (curNode == null) return;
            if (saveDlg.ShowDialog() != DialogResult.OK) return;

            try{
                DataBlock db = (DataBlock)curNode.Tag;
                if (db.LoadData())
                {
                    byte[] body = db.GetBinaryBody();
                    File.WriteAllBytes(saveDlg.FileName,body);
                    db.FreeData();
                }
                MessageBox.Show("Saved!");
            }catch(Exception ex)
            {
                MessageBox.Show("Failed: " + ex.Message);
            }

        }



    }

    
    class DummyComponent : IScriptableComponent
    {
        private Form1 f;

        public Form1 Parent
        {
            get { return f; }
            set { f = value; }
        }

        public void DoSomething(string p)
        {
           // f.textBox1.Text = p;
        }
    }


}
