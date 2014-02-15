namespace Code
{
    partial class CertificateDisplay
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
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.certifView = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // certifView
            // 
            this.certifView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.certifView.Location = new System.Drawing.Point(0, 0);
            this.certifView.Name = "certifView";
            this.certifView.Size = new System.Drawing.Size(424, 310);
            this.certifView.TabIndex = 0;
            // 
            // CertificateDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.certifView);
            this.Name = "CertificateDisplay";
            this.Size = new System.Drawing.Size(424, 310);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView certifView;
    }
}
