namespace Code
{
    partial class ShowCode
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
            this.codeLines = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // codeLines
            // 
            this.codeLines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeLines.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.codeLines.FullRowSelect = true;
            this.codeLines.GridLines = true;
            this.codeLines.Location = new System.Drawing.Point(0, 0);
            this.codeLines.Name = "codeLines";
            this.codeLines.Size = new System.Drawing.Size(346, 367);
            this.codeLines.TabIndex = 0;
            this.codeLines.UseCompatibleStateImageBehavior = false;
            this.codeLines.View = System.Windows.Forms.View.Details;
            // 
            // ShowCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.codeLines);
            this.Name = "ShowCode";
            this.Size = new System.Drawing.Size(346, 367);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView codeLines;
    }
}
