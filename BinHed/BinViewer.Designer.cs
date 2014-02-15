namespace BinHed
{
    partial class BinViewer
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

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
           ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
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
             // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Size = new System.Drawing.Size(736, 419);
            this.splitContainer1.SplitterDistance = 354;
            this.splitContainer1.TabIndex = 0;
            // 
            // grid1
            // 
            this.grid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid1.LineNumberHex = false;
            this.grid1.Location = new System.Drawing.Point(0, 0);
            this.grid1.Name = "grid1";
            this.grid1.SelectedColumn = 0;
            this.grid1.SelectedLine = 0;
            this.grid1.SelectedRow = 0;
            this.grid1.Size = new System.Drawing.Size(354, 419);
            this.grid1.TabIndex = 0;
            this.grid1.UseCompatibleStateImageBehavior = false;
            this.grid1.SubItemSelected += new System.EventHandler(this.grid1_SubItemSelected);
            this.grid1.LineSelected += new System.EventHandler(this.grid1_LineSelected);
            // 
            // grid2
            // 
            this.grid2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid2.LineNumberHex = false;
            this.grid2.Location = new System.Drawing.Point(0, 0);
            this.grid2.Name = "grid2";
            this.grid2.SelectedColumn = 0;
            this.grid2.SelectedLine = 0;
            this.grid2.SelectedRow = 0;
            this.grid2.Size = new System.Drawing.Size(378, 419);
            this.grid2.TabIndex = 0;
            this.grid2.UseCompatibleStateImageBehavior = false;
            this.grid2.SubItemSelected += new System.EventHandler(this.grid2_SubItemSelected);
            this.grid2.LineSelected += new System.EventHandler(this.grid2_LineSelected);
            // 
            // BinViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "BinViewer";
            this.Size = new System.Drawing.Size(736, 419);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
