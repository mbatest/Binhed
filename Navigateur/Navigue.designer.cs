namespace Navigateur
{
    partial class Navigue
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Navigue));
            this.repertoire = new System.Windows.Forms.TreeView();
            this.listeImages = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // repertoire
            // 
            this.repertoire.Dock = System.Windows.Forms.DockStyle.Fill;
            this.repertoire.ImageIndex = 0;
            this.repertoire.ImageList = this.listeImages;
            this.repertoire.Location = new System.Drawing.Point(0, 0);
            this.repertoire.Name = "repertoire";
            this.repertoire.SelectedImageIndex = 0;
            this.repertoire.Size = new System.Drawing.Size(392, 363);
            this.repertoire.TabIndex = 0;
            this.repertoire.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.repertoire_AfterSelect);
            // 
            // listeImages
            // 
            this.listeImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("listeImages.ImageStream")));
            this.listeImages.TransparentColor = System.Drawing.Color.Transparent;
            this.listeImages.Images.SetKeyName(0, "Folder_Back.ico");
            this.listeImages.Images.SetKeyName(1, "folder_open.ico");
            this.listeImages.Images.SetKeyName(2, "generic_picture.ico");
            // 
            // Navigue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.repertoire);
            this.Name = "Navigue";
            this.Size = new System.Drawing.Size(392, 363);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView repertoire;
        private System.Windows.Forms.ImageList listeImages;
    }
}
