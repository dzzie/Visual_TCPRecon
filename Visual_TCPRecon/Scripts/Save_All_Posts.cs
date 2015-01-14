namespace Scripts
{
    using Visual_TCPRecon.Interfaces;
    using Visual_TCPRecon;
    using System;
    using System.Windows.Forms;
    using System.IO;
    
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
    */
    
    class MyCustomScript : IScript
    {
        public void Run(IScriptableComponent component)
        {
	        Form1 f = component.Parent;
	        
	        int i=0, j=0;
            if (f.saveDlg.ShowDialog() != DialogResult.OK) return;
            
            foreach (TreeNode n in f.tv.Nodes){
	            foreach (TreeNode nn in n.Nodes)
                {
		         	if(nn.Text.IndexOf("POST") >= 0){
			         	    DataBlock db = (DataBlock)nn.Tag;
                            if (db.LoadData())
                            {
                                if( db.AppendToFile(f.saveDlg.FileName) ){
	                                 j++;
	                                 using (StreamWriter w = File.AppendText(f.saveDlg.FileName))
        							 {
	        							 w.Write("\r\n\r\n");
        							 }	    
                                 }
                                db.FreeData();
                            }
                        	i++;
		         	}   
	         	}
            }
            MessageBox.Show("total: "+i.ToString()+ " written: "+j.ToString());
        }
    }
}
