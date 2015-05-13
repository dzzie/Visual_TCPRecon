namespace Scripts
{
    using Visual_TCPRecon.Interfaces;
    using Visual_TCPRecon;
    using System;
    using System.Windows.Forms;
    using System.IO;
    using System.Drawing;

    /*
        public DataBlock curdb = null;
        public System.Windows.Forms.OpenFileDialog dlg;
        public System.Windows.Forms.TextBox txtPcap;
        public System.Windows.Forms.TreeView tv;
        public System.Windows.Forms.FolderBrowserDialog fDlg;
        public System.Windows.Forms.ListView lv;
        public System.Windows.Forms.ListView lvDNS;
        public System.Windows.Forms.ListView lvIPs;
        public System.Windows.Forms.SaveFileDialog saveDlg;
        public System.Windows.Forms.ProgressBar pb;
        
        public void setpb(double cur, double max, int index) //index either 1 or 2
        public void setNodeColor(TreeNode n, int color)
    */

    class MyCustomScript : IScript
    {
        private bool AnyInstr(string s, string csvTriggers)
        {
            string[] x = csvTriggers.Split(',');
            foreach (string xx in x)
            {
                if(s.IndexOf(xx, StringComparison.CurrentCultureIgnoreCase) > 0) return true;
            }
            return false;
        }

        public void Run(IScriptableComponent component)
        {
	        int i=0, j=0, hits=0;
			
	        //int Red = -65536; //fu system.drawing cant be found...
	        
            Form1 f = component.Parent;

            string pDir = Path.GetDirectoryName(f.txtPcap.Text);
            string rep = pDir + "\\sqli.txt";
            if (File.Exists(rep)) File.Delete(rep);

            StreamWriter w = File.AppendText(rep);


            foreach (TreeNode n in f.tv.Nodes){
                i++;
                f.setpb(i, f.tv.Nodes.Count, 1);
                n.Checked = false;

	            foreach (TreeNode nn in n.Nodes)
                {
                    j++;
                    f.setpb(j, n.Nodes.Count, 2);
                    nn.Checked = false;

                    DataBlock db = (DataBlock)nn.Tag;
                    if (db.LoadData())
                    {
                        string body = db.GetBody();
                        string fl = db.HttpFirstLine;

                        if (body.IndexOf("SqlException", StringComparison.CurrentCultureIgnoreCase) > 0)
                        {
                            //mssql and oracle
                            f.setNodeColor(nn,1);
                            w.Write("SQLException Found in: " + nn.Text + "\r\n");
                            hits++;
                            nn.Checked = true;
                        }
                        else if (body.IndexOf("SQL syntax", StringComparison.CurrentCultureIgnoreCase) > 0)
                        {
                            //mysql
                            f.setNodeColor(nn, 1);
                            w.Write("SQLException Found in: " + nn.Text + "\r\n");
                            hits++;
                            nn.Checked = true;
                        }

                        if (AnyInstr(fl,"500,408,401,403")) //error,timeout,unauthorized,forbidden  
                        {
                            f.setNodeColor(nn, 1);
                            w.Write("Http Error code found in: " + nn.Text + " " + fl + "\r\n");
                            hits++;
                            nn.Checked = true;
                        }

                        db.FreeData();
                    }
	         	}
            }

            f.pb.Value = 0;
            f.pb2.Value = 0;
            w.Close();

            if (hits > 0)
            {
                MessageBox.Show(hits.ToString() + " results found. The nodes have been checked.\nYou can prune tree using right click menu");

            }
            else
            {
                MessageBox.Show("quick scan had no results...");
            }


        }
    }
}
