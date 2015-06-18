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
            this.removeUncheckedStreamsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.selectLikeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invertSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.parentsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.childrenToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.parentsWChildrenSelectedToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.parentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.childrenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.parentsWChildrenSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.collapseTreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameIPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fDlg = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCopyTable = new System.Windows.Forms.ToolStripMenuItem();
            this.runScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitLargePCAPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ConglomerateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gZIPDecompressFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unchunkExportedBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchContentBodyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tHexView = new System.Windows.Forms.TabPage();
            this.he = new Axrhexed.AxHexEd();
            this.tTextView = new System.Windows.Forms.TabPage();
            this.rtf = new System.Windows.Forms.RichTextBox();
            this.tWebView = new System.Windows.Forms.TabPage();
            this.wb = new AxSHDocVw.AxWebBrowser();
            this.tImageView = new System.Windows.Forms.TabPage();
            this.pict = new System.Windows.Forms.PictureBox();
            this.Details = new System.Windows.Forms.TabPage();
            this.txtDetails = new System.Windows.Forms.TextBox();
            this.mnuLvPopup = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copySelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lvDNS = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.lvIPs = new System.Windows.Forms.ListView();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.saveDlg = new System.Windows.Forms.SaveFileDialog();
            this.btnParse = new System.Windows.Forms.Button();
            this.pb = new System.Windows.Forms.ProgressBar();
            this.pb2 = new System.Windows.Forms.ProgressBar();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.lblFilter = new System.Windows.Forms.Label();
            this.lvFiltered = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.lv = new System.Windows.Forms.ListView();
            this.WebRequests = new System.Windows.Forms.ColumnHeader();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tHexView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.he)).BeginInit();
            this.tTextView.SuspendLayout();
            this.tWebView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.wb)).BeginInit();
            this.tImageView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pict)).BeginInit();
            this.Details.SuspendLayout();
            this.mnuLvPopup.SuspendLayout();
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
            this.btnBrowsePcap.Font = new System.Drawing.Font("Arial Black", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowsePcap.Location = new System.Drawing.Point(537, 33);
            this.btnBrowsePcap.Name = "btnBrowsePcap";
            this.btnBrowsePcap.Size = new System.Drawing.Size(42, 22);
            this.btnBrowsePcap.TabIndex = 2;
            this.btnBrowsePcap.Text = "...";
            this.btnBrowsePcap.UseVisualStyleBackColor = true;
            this.btnBrowsePcap.Click += new System.EventHandler(this.btnBrowsePcap_Click);
            // 
            // tv
            // 
            this.tv.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tv.CheckBoxes = true;
            this.tv.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tv.HideSelection = false;
            this.tv.Location = new System.Drawing.Point(12, 69);
            this.tv.Name = "tv";
            this.tv.Size = new System.Drawing.Size(377, 512);
            this.tv.TabIndex = 6;
            this.tv.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tv_MouseUp);
            this.tv.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tv_AfterSelect);
            this.tv.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tv_NodeMouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveAsToolStripMenuItem,
            this.extractStreamsToolStripMenuItem,
            this.removeStreamToolStripMenuItem,
            this.removeUncheckedStreamsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.selectLikeToolStripMenuItem,
            this.invertSelectionToolStripMenuItem,
            this.clearSelectionToolStripMenuItem,
            this.checkToolStripMenuItem,
            this.uncheckToolStripMenuItem,
            this.toolStripMenuItem1,
            this.collapseTreeToolStripMenuItem,
            this.expandAllToolStripMenuItem,
            this.renameIPToolStripMenuItem,
            this.resetColorsToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(223, 324);
            // 
            // extractStreamsToolStripMenuItem
            // 
            this.extractStreamsToolStripMenuItem.Name = "extractStreamsToolStripMenuItem";
            this.extractStreamsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.extractStreamsToolStripMenuItem.Text = "Extract Checked Data Blocks";
            this.extractStreamsToolStripMenuItem.Click += new System.EventHandler(this.extractStreamsToolStripMenuItem_Click);
            // 
            // removeStreamToolStripMenuItem
            // 
            this.removeStreamToolStripMenuItem.Name = "removeStreamToolStripMenuItem";
            this.removeStreamToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.removeStreamToolStripMenuItem.Text = "Remove Checked Streams";
            this.removeStreamToolStripMenuItem.Click += new System.EventHandler(this.removeStreamToolStripMenuItem_Click);
            // 
            // removeUncheckedStreamsToolStripMenuItem
            // 
            this.removeUncheckedStreamsToolStripMenuItem.Name = "removeUncheckedStreamsToolStripMenuItem";
            this.removeUncheckedStreamsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.removeUncheckedStreamsToolStripMenuItem.Text = "Remove Unchecked Streams";
            this.removeUncheckedStreamsToolStripMenuItem.Click += new System.EventHandler(this.removeUncheckedStreamsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(219, 6);
            // 
            // selectLikeToolStripMenuItem
            // 
            this.selectLikeToolStripMenuItem.Name = "selectLikeToolStripMenuItem";
            this.selectLikeToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.selectLikeToolStripMenuItem.Text = "Select Like";
            this.selectLikeToolStripMenuItem.Click += new System.EventHandler(this.selectLikeToolStripMenuItem_Click_1);
            // 
            // invertSelectionToolStripMenuItem
            // 
            this.invertSelectionToolStripMenuItem.Name = "invertSelectionToolStripMenuItem";
            this.invertSelectionToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.invertSelectionToolStripMenuItem.Text = "Invert Selection";
            this.invertSelectionToolStripMenuItem.Click += new System.EventHandler(this.invertSelectionToolStripMenuItem_Click);
            // 
            // clearSelectionToolStripMenuItem
            // 
            this.clearSelectionToolStripMenuItem.Name = "clearSelectionToolStripMenuItem";
            this.clearSelectionToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.clearSelectionToolStripMenuItem.Text = "Clear Selection";
            this.clearSelectionToolStripMenuItem.Click += new System.EventHandler(this.clearSelectionToolStripMenuItem_Click);
            // 
            // checkToolStripMenuItem
            // 
            this.checkToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allToolStripMenuItem1,
            this.parentsToolStripMenuItem1,
            this.childrenToolStripMenuItem1,
            this.parentsWChildrenSelectedToolStripMenuItem1});
            this.checkToolStripMenuItem.Name = "checkToolStripMenuItem";
            this.checkToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.checkToolStripMenuItem.Text = "Check";
            // 
            // allToolStripMenuItem1
            // 
            this.allToolStripMenuItem1.Name = "allToolStripMenuItem1";
            this.allToolStripMenuItem1.Size = new System.Drawing.Size(220, 22);
            this.allToolStripMenuItem1.Text = "All";
            this.allToolStripMenuItem1.Click += new System.EventHandler(this.allToolStripMenuItem1_Click);
            // 
            // parentsToolStripMenuItem1
            // 
            this.parentsToolStripMenuItem1.Name = "parentsToolStripMenuItem1";
            this.parentsToolStripMenuItem1.Size = new System.Drawing.Size(220, 22);
            this.parentsToolStripMenuItem1.Text = "Parents";
            this.parentsToolStripMenuItem1.Click += new System.EventHandler(this.parentsToolStripMenuItem1_Click);
            // 
            // childrenToolStripMenuItem1
            // 
            this.childrenToolStripMenuItem1.Name = "childrenToolStripMenuItem1";
            this.childrenToolStripMenuItem1.Size = new System.Drawing.Size(220, 22);
            this.childrenToolStripMenuItem1.Text = "Children";
            this.childrenToolStripMenuItem1.Click += new System.EventHandler(this.childrenToolStripMenuItem1_Click);
            // 
            // parentsWChildrenSelectedToolStripMenuItem1
            // 
            this.parentsWChildrenSelectedToolStripMenuItem1.Name = "parentsWChildrenSelectedToolStripMenuItem1";
            this.parentsWChildrenSelectedToolStripMenuItem1.Size = new System.Drawing.Size(220, 22);
            this.parentsWChildrenSelectedToolStripMenuItem1.Text = "Parents w/Children Selected";
            this.parentsWChildrenSelectedToolStripMenuItem1.Click += new System.EventHandler(this.parentsWChildrenSelectedToolStripMenuItem1_Click);
            // 
            // uncheckToolStripMenuItem
            // 
            this.uncheckToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allToolStripMenuItem,
            this.parentsToolStripMenuItem,
            this.childrenToolStripMenuItem,
            this.parentsWChildrenSelectedToolStripMenuItem});
            this.uncheckToolStripMenuItem.Name = "uncheckToolStripMenuItem";
            this.uncheckToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.uncheckToolStripMenuItem.Text = "Uncheck";
            // 
            // allToolStripMenuItem
            // 
            this.allToolStripMenuItem.Name = "allToolStripMenuItem";
            this.allToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.allToolStripMenuItem.Text = "All";
            this.allToolStripMenuItem.Click += new System.EventHandler(this.allToolStripMenuItem_Click);
            // 
            // parentsToolStripMenuItem
            // 
            this.parentsToolStripMenuItem.Name = "parentsToolStripMenuItem";
            this.parentsToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.parentsToolStripMenuItem.Text = "Parents";
            this.parentsToolStripMenuItem.Click += new System.EventHandler(this.parentsToolStripMenuItem_Click);
            // 
            // childrenToolStripMenuItem
            // 
            this.childrenToolStripMenuItem.Name = "childrenToolStripMenuItem";
            this.childrenToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.childrenToolStripMenuItem.Text = "Children";
            this.childrenToolStripMenuItem.Click += new System.EventHandler(this.childrenToolStripMenuItem_Click);
            // 
            // parentsWChildrenSelectedToolStripMenuItem
            // 
            this.parentsWChildrenSelectedToolStripMenuItem.Name = "parentsWChildrenSelectedToolStripMenuItem";
            this.parentsWChildrenSelectedToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.parentsWChildrenSelectedToolStripMenuItem.Text = "Parents w/Children Selected";
            this.parentsWChildrenSelectedToolStripMenuItem.Click += new System.EventHandler(this.parentsWChildrenSelectedToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(219, 6);
            // 
            // collapseTreeToolStripMenuItem
            // 
            this.collapseTreeToolStripMenuItem.Name = "collapseTreeToolStripMenuItem";
            this.collapseTreeToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.collapseTreeToolStripMenuItem.Text = "Collapse All";
            this.collapseTreeToolStripMenuItem.Click += new System.EventHandler(this.collapseTreeToolStripMenuItem_Click);
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.expandAllToolStripMenuItem.Text = "Expand all";
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
            // 
            // renameIPToolStripMenuItem
            // 
            this.renameIPToolStripMenuItem.Name = "renameIPToolStripMenuItem";
            this.renameIPToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.renameIPToolStripMenuItem.Text = "Rename IP";
            this.renameIPToolStripMenuItem.Click += new System.EventHandler(this.renameIPToolStripMenuItem_Click);
            // 
            // resetColorsToolStripMenuItem
            // 
            this.resetColorsToolStripMenuItem.Name = "resetColorsToolStripMenuItem";
            this.resetColorsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.resetColorsToolStripMenuItem.Text = "Reset Colors";
            this.resetColorsToolStripMenuItem.Click += new System.EventHandler(this.resetColorsToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1187, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCopyTable,
            this.runScriptToolStripMenuItem,
            this.splitLargePCAPToolStripMenuItem,
            this.ConglomerateToolStripMenuItem,
            this.gZIPDecompressFileToolStripMenuItem,
            this.unchunkExportedBlockToolStripMenuItem,
            this.searchContentBodyToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // mnuCopyTable
            // 
            this.mnuCopyTable.Name = "mnuCopyTable";
            this.mnuCopyTable.Size = new System.Drawing.Size(271, 22);
            this.mnuCopyTable.Text = "Copy Table";
            this.mnuCopyTable.Click += new System.EventHandler(this.mnuCopyTable_Click);
            // 
            // runScriptToolStripMenuItem
            // 
            this.runScriptToolStripMenuItem.Name = "runScriptToolStripMenuItem";
            this.runScriptToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.runScriptToolStripMenuItem.Text = "Run Script";
            this.runScriptToolStripMenuItem.Click += new System.EventHandler(this.runScriptToolStripMenuItem_Click);
            // 
            // splitLargePCAPToolStripMenuItem
            // 
            this.splitLargePCAPToolStripMenuItem.Name = "splitLargePCAPToolStripMenuItem";
            this.splitLargePCAPToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.splitLargePCAPToolStripMenuItem.Text = "Split Large PCAP";
            this.splitLargePCAPToolStripMenuItem.Click += new System.EventHandler(this.splitLargePCAPToolStripMenuItem_Click);
            // 
            // ConglomerateToolStripMenuItem
            // 
            this.ConglomerateToolStripMenuItem.Name = "ConglomerateToolStripMenuItem";
            this.ConglomerateToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.ConglomerateToolStripMenuItem.Text = "Conglomerate Streams by Dest IP:Port";
            this.ConglomerateToolStripMenuItem.Click += new System.EventHandler(this.seperToolStripMenuItem_Click);
            // 
            // gZIPDecompressFileToolStripMenuItem
            // 
            this.gZIPDecompressFileToolStripMenuItem.Name = "gZIPDecompressFileToolStripMenuItem";
            this.gZIPDecompressFileToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.gZIPDecompressFileToolStripMenuItem.Text = "GZIP Decompress File";
            this.gZIPDecompressFileToolStripMenuItem.Click += new System.EventHandler(this.gZIPDecompressFileToolStripMenuItem_Click);
            // 
            // unchunkExportedBlockToolStripMenuItem
            // 
            this.unchunkExportedBlockToolStripMenuItem.Name = "unchunkExportedBlockToolStripMenuItem";
            this.unchunkExportedBlockToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.unchunkExportedBlockToolStripMenuItem.Text = "unchunk exported block";
            this.unchunkExportedBlockToolStripMenuItem.Click += new System.EventHandler(this.unchunkExportedBlockToolStripMenuItem_Click);
            // 
            // searchContentBodyToolStripMenuItem
            // 
            this.searchContentBodyToolStripMenuItem.Name = "searchContentBodyToolStripMenuItem";
            this.searchContentBodyToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.searchContentBodyToolStripMenuItem.Text = "Search Content Body";
            this.searchContentBodyToolStripMenuItem.Click += new System.EventHandler(this.searchContentBodyToolStripMenuItem_Click);
            // 
            // tabs
            // 
            this.tabs.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabs.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tabs.Controls.Add(this.tHexView);
            this.tabs.Controls.Add(this.tTextView);
            this.tabs.Controls.Add(this.tWebView);
            this.tabs.Controls.Add(this.tImageView);
            this.tabs.Controls.Add(this.Details);
            this.tabs.Location = new System.Drawing.Point(395, 69);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(780, 512);
            this.tabs.TabIndex = 9;
            this.tabs.SelectedIndexChanged += new System.EventHandler(this.tabs_SelectedIndexChanged);
            // 
            // tHexView
            // 
            this.tHexView.Controls.Add(this.he);
            this.tHexView.Location = new System.Drawing.Point(4, 4);
            this.tHexView.Name = "tHexView";
            this.tHexView.Padding = new System.Windows.Forms.Padding(3);
            this.tHexView.Size = new System.Drawing.Size(772, 486);
            this.tHexView.TabIndex = 0;
            this.tHexView.Text = "HexView";
            this.tHexView.UseVisualStyleBackColor = true;
            // 
            // he
            // 
            this.he.Enabled = true;
            this.he.Location = new System.Drawing.Point(3, 9);
            this.he.Name = "he";
            this.he.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("he.OcxState")));
            this.he.Size = new System.Drawing.Size(766, 471);
            this.he.TabIndex = 8;
            // 
            // tTextView
            // 
            this.tTextView.Controls.Add(this.rtf);
            this.tTextView.Location = new System.Drawing.Point(4, 4);
            this.tTextView.Name = "tTextView";
            this.tTextView.Padding = new System.Windows.Forms.Padding(3);
            this.tTextView.Size = new System.Drawing.Size(772, 486);
            this.tTextView.TabIndex = 1;
            this.tTextView.Text = "TextView";
            this.tTextView.UseVisualStyleBackColor = true;
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
            // tWebView
            // 
            this.tWebView.Controls.Add(this.wb);
            this.tWebView.Location = new System.Drawing.Point(4, 4);
            this.tWebView.Name = "tWebView";
            this.tWebView.Padding = new System.Windows.Forms.Padding(3);
            this.tWebView.Size = new System.Drawing.Size(772, 486);
            this.tWebView.TabIndex = 2;
            this.tWebView.Text = "WebView";
            this.tWebView.UseVisualStyleBackColor = true;
            // 
            // wb
            // 
            this.wb.Enabled = true;
            this.wb.Location = new System.Drawing.Point(9, 10);
            this.wb.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("wb.OcxState")));
            this.wb.Size = new System.Drawing.Size(752, 465);
            this.wb.TabIndex = 0;
            // 
            // tImageView
            // 
            this.tImageView.Controls.Add(this.pict);
            this.tImageView.Location = new System.Drawing.Point(4, 4);
            this.tImageView.Name = "tImageView";
            this.tImageView.Padding = new System.Windows.Forms.Padding(3);
            this.tImageView.Size = new System.Drawing.Size(772, 486);
            this.tImageView.TabIndex = 3;
            this.tImageView.Text = "ImageView";
            this.tImageView.UseVisualStyleBackColor = true;
            // 
            // pict
            // 
            this.pict.Location = new System.Drawing.Point(8, 11);
            this.pict.Name = "pict";
            this.pict.Size = new System.Drawing.Size(754, 463);
            this.pict.TabIndex = 0;
            this.pict.TabStop = false;
            // 
            // Details
            // 
            this.Details.Controls.Add(this.txtDetails);
            this.Details.Location = new System.Drawing.Point(4, 4);
            this.Details.Name = "Details";
            this.Details.Padding = new System.Windows.Forms.Padding(3);
            this.Details.Size = new System.Drawing.Size(772, 486);
            this.Details.TabIndex = 4;
            this.Details.Text = "Details";
            this.Details.UseVisualStyleBackColor = true;
            // 
            // txtDetails
            // 
            this.txtDetails.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDetails.Location = new System.Drawing.Point(12, 13);
            this.txtDetails.Multiline = true;
            this.txtDetails.Name = "txtDetails";
            this.txtDetails.Size = new System.Drawing.Size(747, 459);
            this.txtDetails.TabIndex = 0;
            // 
            // mnuLvPopup
            // 
            this.mnuLvPopup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copySelectedToolStripMenuItem,
            this.copyAllToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.searchToolStripMenuItem});
            this.mnuLvPopup.Name = "mnuLvPopup";
            this.mnuLvPopup.Size = new System.Drawing.Size(155, 92);
            // 
            // copySelectedToolStripMenuItem
            // 
            this.copySelectedToolStripMenuItem.Name = "copySelectedToolStripMenuItem";
            this.copySelectedToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.copySelectedToolStripMenuItem.Text = "Copy Selected";
            this.copySelectedToolStripMenuItem.Click += new System.EventHandler(this.copySelectedToolStripMenuItem_Click);
            // 
            // copyAllToolStripMenuItem
            // 
            this.copyAllToolStripMenuItem.Name = "copyAllToolStripMenuItem";
            this.copyAllToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.copyAllToolStripMenuItem.Text = "Copy All";
            this.copyAllToolStripMenuItem.Click += new System.EventHandler(this.copyAllToolStripMenuItem_Click);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.searchToolStripMenuItem.Text = "Search";
            this.searchToolStripMenuItem.Click += new System.EventHandler(this.searchToolStripMenuItem_Click);
            // 
            // lvDNS
            // 
            this.lvDNS.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lvDNS.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvDNS.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvDNS.Location = new System.Drawing.Point(12, 587);
            this.lvDNS.Name = "lvDNS";
            this.lvDNS.Size = new System.Drawing.Size(230, 169);
            this.lvDNS.TabIndex = 11;
            this.lvDNS.UseCompatibleStateImageBehavior = false;
            this.lvDNS.View = System.Windows.Forms.View.Details;
            this.lvDNS.SelectedIndexChanged += new System.EventHandler(this.lvDNS_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "DNSRequests";
            this.columnHeader1.Width = 200;
            // 
            // lvIPs
            // 
            this.lvIPs.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lvIPs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lvIPs.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvIPs.Location = new System.Drawing.Point(248, 587);
            this.lvIPs.Name = "lvIPs";
            this.lvIPs.Size = new System.Drawing.Size(230, 169);
            this.lvIPs.TabIndex = 12;
            this.lvIPs.UseCompatibleStateImageBehavior = false;
            this.lvIPs.View = System.Windows.Forms.View.Details;
            this.lvIPs.SelectedIndexChanged += new System.EventHandler(this.lvIPs_SelectedIndexChanged);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Unique IP Addresses";
            this.columnHeader2.Width = 200;
            // 
            // btnParse
            // 
            this.btnParse.Location = new System.Drawing.Point(587, 34);
            this.btnParse.Name = "btnParse";
            this.btnParse.Size = new System.Drawing.Size(77, 22);
            this.btnParse.TabIndex = 13;
            this.btnParse.Text = "Load";
            this.btnParse.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnParse.UseVisualStyleBackColor = true;
            this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
            // 
            // pb
            // 
            this.pb.Location = new System.Drawing.Point(680, 36);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(494, 13);
            this.pb.TabIndex = 14;
            // 
            // pb2
            // 
            this.pb2.Location = new System.Drawing.Point(680, 52);
            this.pb2.Name = "pb2";
            this.pb2.Size = new System.Drawing.Size(495, 15);
            this.pb2.TabIndex = 15;
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(531, 727);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(239, 20);
            this.txtFilter.TabIndex = 19;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            // 
            // lblFilter
            // 
            this.lblFilter.AutoSize = true;
            this.lblFilter.Location = new System.Drawing.Point(495, 730);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(29, 13);
            this.lblFilter.TabIndex = 18;
            this.lblFilter.Text = "Filter";
            // 
            // lvFiltered
            // 
            this.lvFiltered.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lvFiltered.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.lvFiltered.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvFiltered.Location = new System.Drawing.Point(704, 607);
            this.lvFiltered.Name = "lvFiltered";
            this.lvFiltered.Size = new System.Drawing.Size(377, 104);
            this.lvFiltered.TabIndex = 17;
            this.lvFiltered.UseCompatibleStateImageBehavior = false;
            this.lvFiltered.View = System.Windows.Forms.View.Details;
            this.lvFiltered.Visible = false;
            this.lvFiltered.SelectedIndexChanged += new System.EventHandler(this.lvFiltered_SelectedIndexChanged);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Filtered Web Requests";
            this.columnHeader3.Width = 800;
            // 
            // lv
            // 
            this.lv.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.WebRequests});
            this.lv.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lv.Location = new System.Drawing.Point(484, 587);
            this.lv.Name = "lv";
            this.lv.Size = new System.Drawing.Size(691, 138);
            this.lv.TabIndex = 16;
            this.lv.UseCompatibleStateImageBehavior = false;
            this.lv.View = System.Windows.Forms.View.Details;
            this.lv.SelectedIndexChanged += new System.EventHandler(this.lv_SelectedIndexChanged);
            // 
            // WebRequests
            // 
            this.WebRequests.Text = "Web Requests";
            this.WebRequests.Width = 800;
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1187, 761);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.lblFilter);
            this.Controls.Add(this.lvFiltered);
            this.Controls.Add(this.pb2);
            this.Controls.Add(this.lv);
            this.Controls.Add(this.pb);
            this.Controls.Add(this.btnParse);
            this.Controls.Add(this.lvIPs);
            this.Controls.Add(this.lvDNS);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.tv);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBrowsePcap);
            this.Controls.Add(this.txtPcap);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Visual TCPRecon";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabs.ResumeLayout(false);
            this.tHexView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.he)).EndInit();
            this.tTextView.ResumeLayout(false);
            this.tWebView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.wb)).EndInit();
            this.tImageView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pict)).EndInit();
            this.Details.ResumeLayout(false);
            this.Details.PerformLayout();
            this.mnuLvPopup.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.OpenFileDialog dlg;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtPcap;
        private System.Windows.Forms.Button btnBrowsePcap;
        public System.Windows.Forms.TreeView tv;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem extractStreamsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeStreamToolStripMenuItem;
        public System.Windows.Forms.FolderBrowserDialog fDlg;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyTable;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tHexView;
        private System.Windows.Forms.TabPage tTextView;
        private Axrhexed.AxHexEd he;
        private System.Windows.Forms.RichTextBox rtf;
        private System.Windows.Forms.TabPage tWebView;
        private System.Windows.Forms.ContextMenuStrip mnuLvPopup;
        private System.Windows.Forms.ToolStripMenuItem copySelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseTreeToolStripMenuItem;
        private System.Windows.Forms.TabPage tImageView;
        private System.Windows.Forms.PictureBox pict;
        private AxSHDocVw.AxWebBrowser wb;
        public System.Windows.Forms.ListView lvDNS;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ToolStripMenuItem selectLikeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem invertSelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearSelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        public System.Windows.Forms.ListView lvIPs;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeUncheckedStreamsToolStripMenuItem;
        public System.Windows.Forms.SaveFileDialog saveDlg;
        private System.Windows.Forms.ToolStripMenuItem uncheckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem parentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem childrenToolStripMenuItem;
        private System.Windows.Forms.Button btnParse;
        private System.Windows.Forms.ToolStripMenuItem parentsWChildrenSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ConglomerateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem parentsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem childrenToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem parentsWChildrenSelectedToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem renameIPToolStripMenuItem;
        private System.Windows.Forms.TabPage Details;
        private System.Windows.Forms.TextBox txtDetails;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        public System.Windows.Forms.ProgressBar pb;
        private System.Windows.Forms.ToolStripMenuItem splitLargePCAPToolStripMenuItem;
        public System.Windows.Forms.ProgressBar pb2;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label lblFilter;
        public System.Windows.Forms.ListView lvFiltered;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        public System.Windows.Forms.ListView lv;
        private System.Windows.Forms.ColumnHeader WebRequests;
        private System.Windows.Forms.ToolStripMenuItem gZIPDecompressFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unchunkExportedBlockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchContentBodyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetColorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
    }
}

