/*
 * this is a sample script which will run a decoder over the binary data extracted from
 * packets sent to a given ip. 
*/

namespace Scripts
{
    using Visual_TCPRecon.Interfaces;
    using Visual_TCPRecon;
    using System;
    using System.Windows.Forms;
    using System.IO;
    using System.Drawing;
    using VTcpRecon;
    using System.Text;

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
        public string InputBox(string msg, string title, string defaultVal)
    */

    class MyCustomScript : IScript
    {
        //this is just a sample...
        public void decode(byte[] data)
        {
            for (int i = 0; i <= data.Length - 1; i++)
            {
                data[i] ^= (byte)0x44;
            }

        }

        public void Run(IScriptableComponent component)
        {
	        int i=0, j=0, hits=0;
				        
            Form1 f = component.Parent;

            string C2 = f.InputBox("Enter the C2 IP to decode data for (can be partial string but be unique)", "Set C2", "");
            if (C2.Length == 0) return;

            string pDir = Path.GetDirectoryName(f.txtPcap.Text);
            string rep = pDir + "\\decoder_x_output.txt";
            if (File.Exists(rep)) File.Delete(rep);

            StreamWriter w = File.AppendText(rep);

            foreach (TreeNode n in f.tv.Nodes){
                i++; j = 0;
                f.setpb(i, f.tv.Nodes.Count, 1);

                TcpRecon recon = (TcpRecon)n.Tag;
                if (recon.dumpFile.IndexOf(C2) == -1) continue; //ip is embedded in dump file name...
                 
	            foreach (TreeNode nn in n.Nodes)
                {
                    j++;
                    f.setpb(j, n.Nodes.Count, 2);
                    hits++;
                    DataBlock db = (DataBlock)nn.Tag;
                    w.WriteLine(n.Text + " : " + nn.Text + "\r\n------------------------------------------------");

                    if (db.LoadData())
                    {
                        byte[] buf = null;
                        if (db.DataType == DataBlock.DataTypes.dtBinary)
                        {
                            buf = db.data; 
                        }
                        /*else
                        {
                            buf = db.GetBinaryBody();
                        }*/

                        if (buf != null && buf.Length > 0)
                        {
                            decode(buf);
                            w.WriteLine(HexDumper.HexDump(buf));
                        }
                        else
                        {
                            w.WriteLine("Buffer null or no data..");
                        }

                        db.FreeData();
                    }
                    else
                    {
                        w.WriteLine("Failed to load data...");
                    }

                    w.WriteLine("\r\n");
	         	}
            }

            f.pb.Value = 0;
            f.pb2.Value = 0;
            w.Close();

            if (hits > 0)
            {
                MessageBox.Show(hits.ToString() + " packets decoded.\nSaved as: " + rep);
            }
            else
            {
                MessageBox.Show("No packets found from the C2 you entered: " + C2);
            }


        }
    }
}
