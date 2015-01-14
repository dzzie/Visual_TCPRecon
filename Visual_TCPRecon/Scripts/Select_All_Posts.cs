namespace Scripts
{
    using Visual_TCPRecon.Interfaces;
    using Visual_TCPRecon;
    using System;
    using System.Windows.Forms;
    
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
	        int i=0;
            //component.DoSomething("Hello from MyCustomScript.");
            Form1 f = component.Parent;
            foreach (TreeNode n in f.tv.Nodes){
	            foreach (TreeNode nn in n.Nodes)
                {
		         	if(nn.Text.IndexOf("POST") >= 0){
			         	 nn.Checked = true;
			         	 n.Expand();
			         	 i++;	
		         	}   
	         	}
            }
            MessageBox.Show(i.ToString());
        }
    }
}
