namespace DESCADA.Test
{
    partial class frmJianKong
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
            RealPlayWnd = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)RealPlayWnd).BeginInit();
            SuspendLayout();
            // 
            // RealPlayWnd
            // 
            RealPlayWnd.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            RealPlayWnd.Location = new System.Drawing.Point(2, 1);
            RealPlayWnd.Margin = new System.Windows.Forms.Padding(6);
            RealPlayWnd.Name = "RealPlayWnd";
            RealPlayWnd.Size = new System.Drawing.Size(724, 425);
            RealPlayWnd.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            RealPlayWnd.TabIndex = 31;
            RealPlayWnd.TabStop = false;
            // 
            // frmJianKong
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(731, 423);
            Controls.Add(RealPlayWnd);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Name = "frmJianKong";
            Text = "frmJianKong";
            ((System.ComponentModel.ISupportInitialize)RealPlayWnd).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox RealPlayWnd;
    }
}