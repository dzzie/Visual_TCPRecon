using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Visual_TCPRecon
{
    public partial class FrmList : Form
    {

        public Form1 parent;
        
        public FrmList()
        {
            InitializeComponent();
        }

        private void lv_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ListViewItem li = lv.SelectedItems[0];
                TreeNode n = (TreeNode)li.Tag;
                parent.tv.CollapseAll();
                parent.tv.SelectedNode = n;
                parent.TvNodeClick(n);
                n.EnsureVisible();
                parent.tabs_SelectedIndexChanged(null, null);
            }
            catch (Exception ex) { }
        }

        private void FrmList_Resize(object sender, EventArgs e)
        {
            try
            {
                this.WebRequests.Width = lv.Width-5;
            }
            catch (Exception ex) { ;}
        }
    }
}
