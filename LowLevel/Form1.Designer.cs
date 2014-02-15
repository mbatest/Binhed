namespace LowLevel
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
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
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.textBoxPath = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.secteur = new System.Windows.Forms.ToolStripTextBox();
            this.Dump = new System.Windows.Forms.ToolStripButton();
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textBoxPath,
            this.toolStripLabel4,
            this.secteur,
            this.Dump,
            this.back,
            this.next,
            this.toolStripLabel2,
            this.toolStripLabel3,
            this.textBoxOffset,
            this.textBoxSize,
            this.toolStripLabel5,
            this.toolStripLabel6,
            this.Cylinder,
            this.TrackN,
            this.toolStripLabel7,
            this.Sector});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(998, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
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
            this.Dump.Click += new System.EventHandler(this.buttonDump_Click);
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
            // Change
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
            this.textBoxSize.Size = new System.Drawing.Size(50, 25);
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
            this.Cylinder.Size = new System.Drawing.Size(80, 25);
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
            // ChsSector
            // 
            this.Sector.Enabled = false;
            this.Sector.Name = "Sector";
            this.Sector.Size = new System.Drawing.Size(40, 25);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.treeView1);
            this.splitContainer1.Size = new System.Drawing.Size(998, 325);
            this.splitContainer1.SplitterDistance = 606;
            this.splitContainer1.TabIndex = 5;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(388, 325);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form1";
            this.Size = new System.Drawing.Size(998, 350);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox textBoxOffset;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripTextBox textBoxSize;
        private System.Windows.Forms.ToolStripButton Dump;
        private System.Windows.Forms.ToolStripButton back;
        private System.Windows.Forms.ToolStripButton next;
        private System.Windows.Forms.ToolStripTextBox secteur;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripTextBox Cylinder;
        private System.Windows.Forms.ToolStripLabel toolStripLabel6;
        private System.Windows.Forms.ToolStripTextBox TrackN;
        private System.Windows.Forms.ToolStripLabel toolStripLabel7;
        private System.Windows.Forms.ToolStripTextBox Sector;
        private System.Windows.Forms.ToolStripDropDownButton textBoxPath;
        private System.Windows.Forms.TreeView treeView1;

    }
}

