namespace BinHed
{
    partial class Editor
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

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec object_length'éditeur de tagCode.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor));
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.navigator = new Navigateur.Navigue();
            this.dataInspector1 = new BinHed.DataInspector();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.tabs = new BinHed.TabControlEx();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.refresh = new System.Windows.Forms.ToolStripButton();
            this.nouveauToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.ouvrirToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.dataInspector = new System.Windows.Forms.ToolStripButton();
            this.diskAvvess = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripButton = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.LeftToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.tabs.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainer2.Panel1Collapsed = true;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.toolStripContainer1);
            this.splitContainer2.Size = new System.Drawing.Size(902, 395);
            this.splitContainer2.SplitterDistance = 26;
            this.splitContainer2.TabIndex = 2;
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer4);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(870, 395);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // toolStripContainer1.LeftToolStripPanel
            // 
            this.toolStripContainer1.LeftToolStripPanel.Controls.Add(this.toolStrip1);
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(902, 395);
            this.toolStripContainer1.TabIndex = 4;
            this.toolStripContainer1.Text = "toolStripContainer1";
            this.toolStripContainer1.TopToolStripPanelVisible = false;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer4.Size = new System.Drawing.Size(870, 395);
            this.splitContainer4.SplitterDistance = 233;
            this.splitContainer4.TabIndex = 3;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.navigator);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataInspector1);
            this.splitContainer1.Panel2Collapsed = true;
            this.splitContainer1.Size = new System.Drawing.Size(233, 395);
            this.splitContainer1.SplitterDistance = 105;
            this.splitContainer1.TabIndex = 1;
            // 
            // navigator
            // 
            this.navigator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.navigator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navigator.Location = new System.Drawing.Point(0, 0);
            this.navigator.Name = "navigator";
            this.navigator.Size = new System.Drawing.Size(233, 395);
            this.navigator.TabIndex = 0;
            this.navigator.NavIndexChanged += new Navigateur.Navigue.SelectedIndexEvent(this.navigator_NavIndexChanged);
            // 
            // dataInspector1
            // 
            this.dataInspector1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataInspector1.Location = new System.Drawing.Point(0, 0);
            this.dataInspector1.Name = "dataInspector1";
            this.dataInspector1.Size = new System.Drawing.Size(96, 100);
            this.dataInspector1.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.tabs);
            this.splitContainer3.Panel2Collapsed = true;
            this.splitContainer3.Size = new System.Drawing.Size(633, 395);
            this.splitContainer3.SplitterDistance = 558;
            this.splitContainer3.TabIndex = 2;
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabPage1);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabs.Location = new System.Drawing.Point(0, 0);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(633, 395);
            this.tabs.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(625, 369);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refresh,
            this.nouveauToolStripButton,
            this.ouvrirToolStripButton,
            this.toolStripButton2,
            this.dataInspector,
            this.diskAvvess,
            this.toolStripSeparator2,
            this.ToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(32, 197);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
            // 
            // refresh
            // 
            this.refresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refresh.Image = global::BinHed.Properties.Resources.Refresh;
            this.refresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refresh.Name = "refresh";
            this.refresh.Size = new System.Drawing.Size(30, 20);
            this.refresh.Text = "Refresh";
            this.refresh.Click += new System.EventHandler(this.refresh_Click);
            // 
            // nouveauToolStripButton
            // 
            this.nouveauToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nouveauToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("nouveauToolStripButton.Image")));
            this.nouveauToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nouveauToolStripButton.Name = "nouveauToolStripButton";
            this.nouveauToolStripButton.Size = new System.Drawing.Size(30, 20);
            this.nouveauToolStripButton.Text = "&Nouveau";
            this.nouveauToolStripButton.Click += new System.EventHandler(this.nouveauToolStripButton_Click);
            // 
            // ouvrirToolStripButton
            // 
            this.ouvrirToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ouvrirToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("ouvrirToolStripButton.Image")));
            this.ouvrirToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ouvrirToolStripButton.Name = "ouvrirToolStripButton";
            this.ouvrirToolStripButton.Size = new System.Drawing.Size(30, 20);
            this.ouvrirToolStripButton.Text = "&Ouvrir";
            this.ouvrirToolStripButton.Click += new System.EventHandler(this.ouvrirToolStripButton_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = global::BinHed.Properties.Resources.Laptop;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(30, 20);
            this.toolStripButton2.Text = "Hardware description";
            this.toolStripButton2.Click += new System.EventHandler(this.hardwareToolStripMenuItem_Click);
            // 
            // dataInspector
            // 
            this.dataInspector.CheckOnClick = true;
            this.dataInspector.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.dataInspector.Image = global::BinHed.Properties.Resources.search;
            this.dataInspector.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.dataInspector.Name = "dataInspector";
            this.dataInspector.Size = new System.Drawing.Size(30, 20);
            this.dataInspector.Text = "Data inspector";
            this.dataInspector.Click += new System.EventHandler(this.dataInspectorToolStripMenuItem_Click);
            // 
            // diskAvvess
            // 
            this.diskAvvess.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.diskAvvess.Image = global::BinHed.Properties.Resources.UnknownDrive;
            this.diskAvvess.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.diskAvvess.Name = "diskAvvess";
            this.diskAvvess.Size = new System.Drawing.Size(30, 20);
            this.diskAvvess.Text = "Raw disk access";
            this.diskAvvess.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.diskAvvess.Click += new System.EventHandler(this.rawDiskAccessToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(30, 6);
            // 
            // ToolStripButton
            // 
            this.ToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripButton.Image")));
            this.ToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripButton.Name = "ToolStripButton";
            this.ToolStripButton.Size = new System.Drawing.Size(30, 20);
            this.ToolStripButton.Text = "&?";
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(902, 395);
            this.Controls.Add(this.splitContainer2);
            this.Name = "Editor";
            this.Text = "What\'s in my file ?";
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.LeftToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.LeftToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.tabs.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private Navigateur.Navigue navigator;
        private TabControlEx tabs;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private DataInspector dataInspector1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton ouvrirToolStripButton;
        private System.Windows.Forms.ToolStripButton ToolStripButton;
        private System.Windows.Forms.ToolStripButton diskAvvess;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton dataInspector;
        private System.Windows.Forms.ToolStripButton refresh;
        private System.Windows.Forms.ToolStripButton nouveauToolStripButton;
    }
}

