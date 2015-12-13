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
    public partial class frmDump : Form
    {
        public frmDump()
        {
            InitializeComponent();
        }

        private void frmDump_Load(object sender, EventArgs e)
        {

        }

        public void LoadData(string data)
        {
            textBox1.Text = data;
            textBox1.SelectionStart = 0;
            textBox1.SelectionLength = 0;
            this.Visible = true;
        }

    }
}
