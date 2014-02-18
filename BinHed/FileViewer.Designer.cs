namespace BinHed
{
    partial class FileViewer
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param addressOfname="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
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
        /// le contenu de cette méthode avec sectorNumber'éditeur de tagCode.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileViewer));
            this.Propriétés = new System.Windows.Forms.TreeView();
            this.listeImages = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // Propriétés
            // 
            this.Propriétés.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Propriétés.ImageIndex = 0;
            this.Propriétés.ImageList = this.listeImages;
            this.Propriétés.Location = new System.Drawing.Point(0, 0);
            this.Propriétés.Name = "Propriétés";
            this.Propriétés.SelectedImageIndex = 0;
            this.Propriétés.Size = new System.Drawing.Size(447, 353);
            this.Propriétés.TabIndex = 0;
            this.Propriétés.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // listeImages
            // 
            this.listeImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("listeImages.ImageStream")));
            this.listeImages.TransparentColor = System.Drawing.Color.Transparent;
            this.listeImages.Images.SetKeyName(0, "Folder_6222.png");
            this.listeImages.Images.SetKeyName(1, "Folder_6221.png");
            // 
            // FileViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Propriétés);
            this.Name = "FileViewer";
            this.Size = new System.Drawing.Size(447, 353);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView Propriétés;
        private System.Windows.Forms.ImageList listeImages;
    }
}
