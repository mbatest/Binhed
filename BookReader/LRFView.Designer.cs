using BookReader;

namespace BinHed
{
    partial class LRFView
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

        #region Windows Form Designer generated tagCode

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the tagCode editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lrfViewer1 = new LRFViewer();
            this.SuspendLayout();
            // 
            // lrfViewer1
            // 
            this.lrfViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lrfViewer1.Location = new System.Drawing.Point(0, 0);
            this.lrfViewer1.Name = "lrfViewer1";
            this.lrfViewer1.Size = new System.Drawing.Size(531, 427);
            this.lrfViewer1.TabIndex = 0;
            // 
            // LRFView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 427);
            this.Controls.Add(this.lrfViewer1);
            this.Name = "LRFView";
            this.Text = "LRFView";
            this.ResumeLayout(false);

        }

        #endregion

        private LRFViewer lrfViewer1;
    }
}