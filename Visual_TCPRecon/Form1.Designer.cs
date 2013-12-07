namespace Visual_TCPRecon
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.dlg = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPcap = new System.Windows.Forms.TextBox();
            this.btnBrowsePcap = new System.Windows.Forms.Button();
            this.chkUselibnids = new System.Windows.Forms.CheckBox();
            this.tv = new System.Windows.Forms.TreeView();
            this.he = new Axrhexed.AxHexEd();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extractStreamsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeStreamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fDlg = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scanForHTTPRequestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.he)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dlg
            // 
            this.dlg.FileName = "openFileDialog1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "PCAP File";
            // 
            // txtPcap
            // 
            this.txtPcap.Location = new System.Drawing.Point(76, 32);
            this.txtPcap.Name = "txtPcap";
            this.txtPcap.Size = new System.Drawing.Size(453, 20);
            this.txtPcap.TabIndex = 1;
            // 
            // btnBrowsePcap
            // 
            this.btnBrowsePcap.Location = new System.Drawing.Point(537, 33);
            this.btnBrowsePcap.Name = "btnBrowsePcap";
            this.btnBrowsePcap.Size = new System.Drawing.Size(42, 19);
            this.btnBrowsePcap.TabIndex = 2;
            this.btnBrowsePcap.Text = "...";
            this.btnBrowsePcap.UseVisualStyleBackColor = true;
            this.btnBrowsePcap.Click += new System.EventHandler(this.btnBrowsePcap_Click);
            // 
            // chkUselibnids
            // 
            this.chkUselibnids.AutoSize = true;
            this.chkUselibnids.Location = new System.Drawing.Point(606, 36);
            this.chkUselibnids.Name = "chkUselibnids";
            this.chkUselibnids.Size = new System.Drawing.Size(79, 17);
            this.chkUselibnids.TabIndex = 4;
            this.chkUselibnids.Text = "Use libNids";
            this.chkUselibnids.UseVisualStyleBackColor = true;
            this.chkUselibnids.Visible = false;
            // 
            // tv
            // 
            this.tv.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tv.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tv.Location = new System.Drawing.Point(12, 69);
            this.tv.Name = "tv";
            this.tv.Size = new System.Drawing.Size(357, 512);
            this.tv.TabIndex = 6;
            this.tv.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tv_MouseUp);
            this.tv.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tv_NodeMouseClick);
            // 
            // he
            // 
            this.he.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.he.Enabled = true;
            this.he.Location = new System.Drawing.Point(375, 69);
            this.he.Name = "he";
            this.he.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("he.OcxState")));
            this.he.Size = new System.Drawing.Size(765, 512);
            this.he.TabIndex = 7;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractStreamsToolStripMenuItem,
            this.removeStreamToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(163, 48);
            // 
            // extractStreamsToolStripMenuItem
            // 
            this.extractStreamsToolStripMenuItem.Name = "extractStreamsToolStripMenuItem";
            this.extractStreamsToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.extractStreamsToolStripMenuItem.Text = "Extract Streams";
            this.extractStreamsToolStripMenuItem.Click += new System.EventHandler(this.extractStreamsToolStripMenuItem_Click);
            // 
            // removeStreamToolStripMenuItem
            // 
            this.removeStreamToolStripMenuItem.Name = "removeStreamToolStripMenuItem";
            this.removeStreamToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.removeStreamToolStripMenuItem.Text = "Remove Stream";
            this.removeStreamToolStripMenuItem.Click += new System.EventHandler(this.removeStreamToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1155, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scanForHTTPRequestsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // scanForHTTPRequestsToolStripMenuItem
            // 
            this.scanForHTTPRequestsToolStripMenuItem.Name = "scanForHTTPRequestsToolStripMenuItem";
            this.scanForHTTPRequestsToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.scanForHTTPRequestsToolStripMenuItem.Text = "Scan For HTTP Requests";
            this.scanForHTTPRequestsToolStripMenuItem.Click += new System.EventHandler(this.scanForHTTPRequestsToolStripMenuItem_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1155, 607);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.tv);
            this.Controls.Add(this.he);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkUselibnids);
            this.Controls.Add(this.btnBrowsePcap);
            this.Controls.Add(this.txtPcap);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.he)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog dlg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPcap;
        private System.Windows.Forms.Button btnBrowsePcap;
        private System.Windows.Forms.CheckBox chkUselibnids;
        private System.Windows.Forms.TreeView tv;
        private Axrhexed.AxHexEd he;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem extractStreamsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeStreamToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog fDlg;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scanForHTTPRequestsToolStripMenuItem;
    }
}

