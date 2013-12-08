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
            this.tv = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extractStreamsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeStreamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fDlg = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scanForHTTPRequestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabs = new System.Windows.Forms.TabControl();
            this.HexView = new System.Windows.Forms.TabPage();
            this.TextView = new System.Windows.Forms.TabPage();
            this.WebView = new System.Windows.Forms.TabPage();
            this.he = new Axrhexed.AxHexEd();
            this.rtf = new System.Windows.Forms.RichTextBox();
            this.lst = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tabs.SuspendLayout();
            this.HexView.SuspendLayout();
            this.TextView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.he)).BeginInit();
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
            // tabs
            // 
            this.tabs.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabs.Controls.Add(this.HexView);
            this.tabs.Controls.Add(this.TextView);
            this.tabs.Controls.Add(this.WebView);
            this.tabs.Location = new System.Drawing.Point(375, 69);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(780, 512);
            this.tabs.TabIndex = 9;
            this.tabs.SelectedIndexChanged += new System.EventHandler(this.tabs_SelectedIndexChanged);
            // 
            // HexView
            // 
            this.HexView.Controls.Add(this.he);
            this.HexView.Location = new System.Drawing.Point(4, 4);
            this.HexView.Name = "HexView";
            this.HexView.Padding = new System.Windows.Forms.Padding(3);
            this.HexView.Size = new System.Drawing.Size(772, 486);
            this.HexView.TabIndex = 0;
            this.HexView.Text = "HexView";
            this.HexView.UseVisualStyleBackColor = true;
            // 
            // TextView
            // 
            this.TextView.Controls.Add(this.rtf);
            this.TextView.Location = new System.Drawing.Point(4, 4);
            this.TextView.Name = "TextView";
            this.TextView.Padding = new System.Windows.Forms.Padding(3);
            this.TextView.Size = new System.Drawing.Size(772, 486);
            this.TextView.TabIndex = 1;
            this.TextView.Text = "TextView";
            this.TextView.UseVisualStyleBackColor = true;
            // 
            // WebView
            // 
            this.WebView.Location = new System.Drawing.Point(4, 4);
            this.WebView.Name = "WebView";
            this.WebView.Padding = new System.Windows.Forms.Padding(3);
            this.WebView.Size = new System.Drawing.Size(772, 486);
            this.WebView.TabIndex = 2;
            this.WebView.Text = "WebView";
            this.WebView.UseVisualStyleBackColor = true;
            // 
            // he
            // 
            this.he.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.he.Enabled = true;
            this.he.Location = new System.Drawing.Point(3, 9);
            this.he.Name = "he";
            this.he.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("he.OcxState")));
            this.he.Size = new System.Drawing.Size(766, 471);
            this.he.TabIndex = 8;
            // 
            // rtf
            // 
            this.rtf.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtf.Location = new System.Drawing.Point(3, 6);
            this.rtf.Name = "rtf";
            this.rtf.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtf.Size = new System.Drawing.Size(766, 474);
            this.rtf.TabIndex = 0;
            this.rtf.Text = "";
            // 
            // lst
            // 
            this.lst.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lst.FormattingEnabled = true;
            this.lst.ItemHeight = 17;
            this.lst.Location = new System.Drawing.Point(12, 587);
            this.lst.Name = "lst";
            this.lst.Size = new System.Drawing.Size(1139, 106);
            this.lst.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1155, 706);
            this.Controls.Add(this.lst);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.tv);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBrowsePcap);
            this.Controls.Add(this.txtPcap);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabs.ResumeLayout(false);
            this.HexView.ResumeLayout(false);
            this.TextView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.he)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog dlg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPcap;
        private System.Windows.Forms.Button btnBrowsePcap;
        private System.Windows.Forms.TreeView tv;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem extractStreamsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeStreamToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog fDlg;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scanForHTTPRequestsToolStripMenuItem;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage HexView;
        private System.Windows.Forms.TabPage TextView;
        private Axrhexed.AxHexEd he;
        private System.Windows.Forms.RichTextBox rtf;
        private System.Windows.Forms.TabPage WebView;
        private System.Windows.Forms.ListBox lst;
    }
}

