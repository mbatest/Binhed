namespace BinHed
{
    partial class FileEdit
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param nameIndex="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec object_length'éditeur de tagCode.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileEdit));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.binaryView = new BinaryViewer.BinaryView();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.exv = new BinHed.FileViewer();
            this.mftEntries = new BinHed.TabControlEx();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.nouveauToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.ouvrirToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.enregistrerToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.imprimerToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.hexa = new System.Windows.Forms.ToolStripButton();
            this.showHex = new System.Windows.Forms.ToolStripButton();
            this.showText = new System.Windows.Forms.ToolStripButton();
            this.zoomOut = new System.Windows.Forms.ToolStripButton();
            this.zoomIn = new System.Windows.Forms.ToolStripButton();
            this.reAnalyse = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.couperToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.copierToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.collerToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.searchText = new System.Windows.Forms.ToolStripTextBox();
            this.searchType = new System.Windows.Forms.ToolStripSplitButton();
            this.hexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.binaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ansiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unicodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchButton = new System.Windows.Forms.ToolStripButton();
            this.suiteFichier = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.prcButton = new System.Windows.Forms.ToolStripButton();
            this.nextPage = new System.Windows.Forms.ToolStripButton();
            this.fileNameLabel = new System.Windows.Forms.ToolStripLabel();
            this.textBoxPath = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.secteur = new System.Windows.Forms.ToolStripTextBox();
            this.Dump = new System.Windows.Forms.ToolStripButton();
            this.parseBlock = new System.Windows.Forms.ToolStripButton();
            this.back = new System.Windows.Forms.ToolStripButton();
            this.next = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.textBoxOffset = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.textBoxSize = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.Cylinder = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel6 = new System.Windows.Forms.ToolStripLabel();
            this.TrackN = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel7 = new System.Windows.Forms.ToolStripLabel();
            this.Sector = new System.Windows.Forms.ToolStripTextBox();
            this.ToolStripButton = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(1633, 402);
            this.splitContainer1.SplitterDistance = 782;
            this.splitContainer1.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(782, 402);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.binaryView);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(774, 376);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // binaryView
            // 
            this.binaryView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.binaryView.Location = new System.Drawing.Point(3, 3);
            this.binaryView.Name = "binaryView";
            this.binaryView.PanelWidth = 664;
            this.binaryView.Size = new System.Drawing.Size(768, 370);
            this.binaryView.TabIndex = 0;
            this.binaryView.dataEvent += new Utils.DataEventHandler(this.view1_dataEvent);
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.exv);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.mftEntries);
            this.splitContainer3.Size = new System.Drawing.Size(847, 402);
            this.splitContainer3.SplitterDistance = 198;
            this.splitContainer3.TabIndex = 0;
            // 
            // exv
            // 
            this.exv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exv.Location = new System.Drawing.Point(0, 0);
            this.exv.Name = "exv";
            this.exv.Size = new System.Drawing.Size(847, 198);
            this.exv.TabIndex = 0;
            this.exv.dataSelected += new Utils.DataSelectedEventHandler(this.exv_dataSelected);
            this.exv.dataRequested += new BinHed.DataRequestEventHandler(this.exv_dataRequested);
            // 
            // mftEntries
            // 
            this.mftEntries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mftEntries.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.mftEntries.Location = new System.Drawing.Point(0, 0);
            this.mftEntries.Name = "mftEntries";
            this.mftEntries.SelectedIndex = 0;
            this.mftEntries.Size = new System.Drawing.Size(847, 200);
            this.mftEntries.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer2.Size = new System.Drawing.Size(1633, 431);
            this.splitContainer2.SplitterDistance = 25;
            this.splitContainer2.TabIndex = 4;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nouveauToolStripButton,
            this.ouvrirToolStripButton,
            this.enregistrerToolStripButton,
            this.imprimerToolStripButton,
            this.hexa,
            this.showHex,
            this.showText,
            this.zoomOut,
            this.zoomIn,
            this.reAnalyse,
            this.toolStripSeparator,
            this.couperToolStripButton,
            this.copierToolStripButton,
            this.collerToolStripButton,
            this.toolStripSeparator1,
            this.searchText,
            this.searchType,
            this.searchButton,
            this.suiteFichier,
            this.toolStripSeparator2,
            this.prcButton,
            this.nextPage,
            this.fileNameLabel,
            this.textBoxPath,
            this.toolStripLabel4,
            this.secteur,
            this.Dump,
            this.parseBlock,
            this.back,
            this.next,
            this.toolStripLabel2,
            this.textBoxOffset,
            this.toolStripLabel3,
            this.textBoxSize,
            this.toolStripLabel5,
            this.Cylinder,
            this.toolStripLabel6,
            this.TrackN,
            this.toolStripLabel7,
            this.Sector,
            this.ToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1633, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // nouveauToolStripButton
            // 
            this.nouveauToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nouveauToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("nouveauToolStripButton.Image")));
            this.nouveauToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nouveauToolStripButton.Name = "nouveauToolStripButton";
            this.nouveauToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.nouveauToolStripButton.Text = "&Nouveau";
            // 
            // ouvrirToolStripButton
            // 
            this.ouvrirToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ouvrirToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("ouvrirToolStripButton.Image")));
            this.ouvrirToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ouvrirToolStripButton.Name = "ouvrirToolStripButton";
            this.ouvrirToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.ouvrirToolStripButton.Text = "&Ouvrir";
            // 
            // enregistrerToolStripButton
            // 
            this.enregistrerToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.enregistrerToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("enregistrerToolStripButton.Image")));
            this.enregistrerToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.enregistrerToolStripButton.Name = "enregistrerToolStripButton";
            this.enregistrerToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.enregistrerToolStripButton.Text = "&Enregistrer";
            // 
            // imprimerToolStripButton
            // 
            this.imprimerToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.imprimerToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("imprimerToolStripButton.Image")));
            this.imprimerToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.imprimerToolStripButton.Name = "imprimerToolStripButton";
            this.imprimerToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.imprimerToolStripButton.Text = "&Imprimer";
            // 
            // hexa
            // 
            this.hexa.Checked = true;
            this.hexa.CheckOnClick = true;
            this.hexa.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hexa.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.hexa.Image = ((System.Drawing.Image)(resources.GetObject("hexa.Image")));
            this.hexa.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.hexa.Name = "hexa";
            this.hexa.Size = new System.Drawing.Size(23, 22);
            this.hexa.Text = "0x";
            this.hexa.Click += new System.EventHandler(this.hexa_Click);
            // 
            // showHex
            // 
            this.showHex.Checked = true;
            this.showHex.CheckOnClick = true;
            this.showHex.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showHex.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.showHex.Image = ((System.Drawing.Image)(resources.GetObject("showHex.Image")));
            this.showHex.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showHex.Name = "showHex";
            this.showHex.Size = new System.Drawing.Size(23, 22);
            this.showHex.Text = "H";
            this.showHex.Click += new System.EventHandler(this.showHex_Click);
            // 
            // showText
            // 
            this.showText.Checked = true;
            this.showText.CheckOnClick = true;
            this.showText.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.showText.Image = ((System.Drawing.Image)(resources.GetObject("showText.Image")));
            this.showText.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showText.Name = "showText";
            this.showText.Size = new System.Drawing.Size(23, 22);
            this.showText.Text = "T";
            this.showText.Click += new System.EventHandler(this.showText_Click);
            // 
            // zoomOut
            // 
            this.zoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomOut.Image = ((System.Drawing.Image)(resources.GetObject("zoomOut.Image")));
            this.zoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomOut.Name = "zoomOut";
            this.zoomOut.Size = new System.Drawing.Size(23, 22);
            this.zoomOut.Text = "toolStripButton1";
            this.zoomOut.Click += new System.EventHandler(this.zoomOut_Click);
            // 
            // zoomIn
            // 
            this.zoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomIn.Image = ((System.Drawing.Image)(resources.GetObject("zoomIn.Image")));
            this.zoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomIn.Name = "zoomIn";
            this.zoomIn.Size = new System.Drawing.Size(23, 22);
            this.zoomIn.Text = "toolStripButton1";
            this.zoomIn.Click += new System.EventHandler(this.zoomIn_Click);
            // 
            // reAnalyse
            // 
            this.reAnalyse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.reAnalyse.Image = ((System.Drawing.Image)(resources.GetObject("reAnalyse.Image")));
            this.reAnalyse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.reAnalyse.Name = "reAnalyse";
            this.reAnalyse.Size = new System.Drawing.Size(23, 22);
            this.reAnalyse.Text = "toolStripButton1";
            this.reAnalyse.Click += new System.EventHandler(this.reAnalyse_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // couperToolStripButton
            // 
            this.couperToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.couperToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("couperToolStripButton.Image")));
            this.couperToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.couperToolStripButton.Name = "couperToolStripButton";
            this.couperToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.couperToolStripButton.Text = "C&ouper";
            // 
            // copierToolStripButton
            // 
            this.copierToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copierToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("copierToolStripButton.Image")));
            this.copierToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copierToolStripButton.Name = "copierToolStripButton";
            this.copierToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.copierToolStripButton.Text = "Co&pier";
            // 
            // collerToolStripButton
            // 
            this.collerToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.collerToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("collerToolStripButton.Image")));
            this.collerToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.collerToolStripButton.Name = "collerToolStripButton";
            this.collerToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.collerToolStripButton.Text = "Co&ller";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // searchText
            // 
            this.searchText.Name = "searchText";
            this.searchText.Size = new System.Drawing.Size(80, 25);
            // 
            // searchType
            // 
            this.searchType.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.searchType.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hexToolStripMenuItem,
            this.binaryToolStripMenuItem,
            this.ansiToolStripMenuItem,
            this.unicodeToolStripMenuItem,
            this.addressToolStripMenuItem});
            this.searchType.Image = ((System.Drawing.Image)(resources.GetObject("searchType.Image")));
            this.searchType.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchType.Name = "searchType";
            this.searchType.Size = new System.Drawing.Size(43, 22);
            this.searchType.Text = "Hex";
            // 
            // hexToolStripMenuItem
            // 
            this.hexToolStripMenuItem.Checked = true;
            this.hexToolStripMenuItem.CheckOnClick = true;
            this.hexToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hexToolStripMenuItem.Name = "hexToolStripMenuItem";
            this.hexToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.hexToolStripMenuItem.Text = "Hex";
            this.hexToolStripMenuItem.Click += new System.EventHandler(this.hexToolStripMenuItem_Click);
            // 
            // binaryToolStripMenuItem
            // 
            this.binaryToolStripMenuItem.Name = "binaryToolStripMenuItem";
            this.binaryToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.binaryToolStripMenuItem.Text = "Binary";
            this.binaryToolStripMenuItem.Click += new System.EventHandler(this.hexToolStripMenuItem_Click);
            // 
            // ansiToolStripMenuItem
            // 
            this.ansiToolStripMenuItem.CheckOnClick = true;
            this.ansiToolStripMenuItem.Name = "ansiToolStripMenuItem";
            this.ansiToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.ansiToolStripMenuItem.Text = "Ansi";
            this.ansiToolStripMenuItem.Click += new System.EventHandler(this.hexToolStripMenuItem_Click);
            // 
            // unicodeToolStripMenuItem
            // 
            this.unicodeToolStripMenuItem.CheckOnClick = true;
            this.unicodeToolStripMenuItem.Name = "unicodeToolStripMenuItem";
            this.unicodeToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.unicodeToolStripMenuItem.Text = "Unicode";
            this.unicodeToolStripMenuItem.Click += new System.EventHandler(this.hexToolStripMenuItem_Click);
            // 
            // addressToolStripMenuItem
            // 
            this.addressToolStripMenuItem.CheckOnClick = true;
            this.addressToolStripMenuItem.Name = "addressToolStripMenuItem";
            this.addressToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.addressToolStripMenuItem.Text = "Address";
            this.addressToolStripMenuItem.Click += new System.EventHandler(this.hexToolStripMenuItem_Click);
            // 
            // searchButton
            // 
            this.searchButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.searchButton.Image = ((System.Drawing.Image)(resources.GetObject("searchButton.Image")));
            this.searchButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(23, 22);
            this.searchButton.Text = "Search";
            this.searchButton.Click += new System.EventHandler(this.searchText_TextChanged);
            // 
            // suiteFichier
            // 
            this.suiteFichier.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.suiteFichier.Image = ((System.Drawing.Image)(resources.GetObject("suiteFichier.Image")));
            this.suiteFichier.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.suiteFichier.Name = "suiteFichier";
            this.suiteFichier.Size = new System.Drawing.Size(23, 22);
            this.suiteFichier.Text = "Suite";
            this.suiteFichier.Click += new System.EventHandler(this.suiteFichier_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // prcButton
            // 
            this.prcButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.prcButton.Image = ((System.Drawing.Image)(resources.GetObject("prcButton.Image")));
            this.prcButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.prcButton.Name = "prcButton";
            this.prcButton.Size = new System.Drawing.Size(23, 22);
            this.prcButton.Text = "Decode prc";
            this.prcButton.Click += new System.EventHandler(this.prcButton_Click);
            // 
            // nextPage
            // 
            this.nextPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextPage.Image = ((System.Drawing.Image)(resources.GetObject("nextPage.Image")));
            this.nextPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextPage.Name = "nextPage";
            this.nextPage.Size = new System.Drawing.Size(23, 22);
            this.nextPage.Text = "nextPage";
            this.nextPage.Click += new System.EventHandler(this.nextPage_Click);
            // 
            // fileNameLabel
            // 
            this.fileNameLabel.Name = "fileNameLabel";
            this.fileNameLabel.Size = new System.Drawing.Size(0, 22);
            // 
            // textBoxPath
            // 
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(13, 22);
            this.textBoxPath.TextChanged += new System.EventHandler(this.textBoxPath_TextChanged);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(46, 22);
            this.toolStripLabel4.Text = "Secteur";
            // 
            // secteur
            // 
            this.secteur.Name = "secteur";
            this.secteur.Size = new System.Drawing.Size(80, 25);
            // 
            // Dump
            // 
            this.Dump.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Dump.Image = ((System.Drawing.Image)(resources.GetObject("Dump.Image")));
            this.Dump.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Dump.Name = "Dump";
            this.Dump.Size = new System.Drawing.Size(23, 22);
            this.Dump.Text = "toolStripButton1";
            this.Dump.Click += new System.EventHandler(this.Dump_Click);
            // 
            // parseBlock
            // 
            this.parseBlock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.parseBlock.Image = ((System.Drawing.Image)(resources.GetObject("parseBlock.Image")));
            this.parseBlock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.parseBlock.Name = "parseBlock";
            this.parseBlock.Size = new System.Drawing.Size(23, 22);
            this.parseBlock.Text = "Parse bloc";
            this.parseBlock.Visible = false;
            this.parseBlock.Click += new System.EventHandler(this.parseBlock_Click);
            // 
            // back
            // 
            this.back.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.back.Image = ((System.Drawing.Image)(resources.GetObject("back.Image")));
            this.back.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.back.Name = "back";
            this.back.Size = new System.Drawing.Size(23, 22);
            this.back.Text = "toolStripButton1";
            this.back.Click += new System.EventHandler(this.back_Click);
            // 
            // next
            // 
            this.next.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.next.Image = ((System.Drawing.Image)(resources.GetObject("next.Image")));
            this.next.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.next.Name = "next";
            this.next.Size = new System.Drawing.Size(23, 22);
            this.next.Text = "toolStripButton2";
            this.next.Click += new System.EventHandler(this.next_Click);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(39, 22);
            this.toolStripLabel2.Text = "Offset";
            // 
            // textBoxOffset
            // 
            this.textBoxOffset.Name = "textBoxOffset";
            this.textBoxOffset.Size = new System.Drawing.Size(100, 25);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(27, 22);
            this.toolStripLabel3.Text = "Size";
            // 
            // textBoxSize
            // 
            this.textBoxSize.Name = "textBoxSize";
            this.textBoxSize.Size = new System.Drawing.Size(30, 25);
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(51, 22);
            this.toolStripLabel5.Text = "Cylinder";
            // 
            // Cylinder
            // 
            this.Cylinder.Enabled = false;
            this.Cylinder.Name = "Cylinder";
            this.Cylinder.Size = new System.Drawing.Size(70, 25);
            // 
            // toolStripLabel6
            // 
            this.toolStripLabel6.Name = "toolStripLabel6";
            this.toolStripLabel6.Size = new System.Drawing.Size(36, 22);
            this.toolStripLabel6.Text = "Track";
            // 
            // TrackN
            // 
            this.TrackN.Enabled = false;
            this.TrackN.Name = "TrackN";
            this.TrackN.Size = new System.Drawing.Size(50, 25);
            // 
            // toolStripLabel7
            // 
            this.toolStripLabel7.Name = "toolStripLabel7";
            this.toolStripLabel7.Size = new System.Drawing.Size(40, 22);
            this.toolStripLabel7.Text = "Sector";
            // 
            // Sector
            // 
            this.Sector.Enabled = false;
            this.Sector.Name = "Sector";
            this.Sector.Size = new System.Drawing.Size(40, 25);
            // 
            // ToolStripButton
            // 
            this.ToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripButton.Image")));
            this.ToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripButton.Name = "ToolStripButton";
            this.ToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.ToolStripButton.Text = "&?";
            // 
            // FileEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer2);
            this.Name = "FileEdit";
            this.Size = new System.Drawing.Size(1633, 431);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton nouveauToolStripButton;
        private System.Windows.Forms.ToolStripButton ouvrirToolStripButton;
        private System.Windows.Forms.ToolStripButton enregistrerToolStripButton;
        private System.Windows.Forms.ToolStripButton imprimerToolStripButton;
        private System.Windows.Forms.ToolStripButton hexa;
        private System.Windows.Forms.ToolStripButton reAnalyse;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripButton couperToolStripButton;
        private System.Windows.Forms.ToolStripButton copierToolStripButton;
        private System.Windows.Forms.ToolStripButton collerToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton ToolStripButton;
        private System.Windows.Forms.ToolStripTextBox searchText;
        private System.Windows.Forms.ToolStripButton searchButton;
        private System.Windows.Forms.ToolStripButton suiteFichier;
        private System.Windows.Forms.ToolStripButton prcButton;
        private System.Windows.Forms.ToolStripButton nextPage;
        private System.Windows.Forms.ToolStripLabel fileNameLabel;
        private BinaryViewer.BinaryView binaryView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSplitButton searchType;
        private System.Windows.Forms.ToolStripMenuItem hexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ansiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unicodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem binaryToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripButton showText;
        private System.Windows.Forms.ToolStripButton showHex;
        private System.Windows.Forms.ToolStripButton zoomOut;
        private System.Windows.Forms.ToolStripButton zoomIn;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ToolStripDropDownButton textBoxPath;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripTextBox secteur;
        private System.Windows.Forms.ToolStripButton Dump;
        private System.Windows.Forms.ToolStripButton back;
        private System.Windows.Forms.ToolStripButton next;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripTextBox textBoxOffset;
        private System.Windows.Forms.ToolStripTextBox textBoxSize;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripLabel toolStripLabel6;
        private System.Windows.Forms.ToolStripTextBox Cylinder;
        private System.Windows.Forms.ToolStripTextBox TrackN;
        private System.Windows.Forms.ToolStripLabel toolStripLabel7;
        private System.Windows.Forms.ToolStripTextBox Sector;
        private System.Windows.Forms.ToolStripButton parseBlock;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private BinHed.TabControlEx mftEntries;
        private FileViewer exv;
    }
}
