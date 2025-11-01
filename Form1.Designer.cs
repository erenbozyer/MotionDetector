namespace MotionDetector
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox picturePreview;
        private System.Windows.Forms.Button btnToggle;
        private System.Windows.Forms.Label lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.picturePreview = new System.Windows.Forms.PictureBox();
            this.btnToggle = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picturePreview)).BeginInit();
            this.SuspendLayout();
            // 
            // picturePreview
            // 
            this.picturePreview.Location = new System.Drawing.Point(12, 12);
            this.picturePreview.Name = "picturePreview";
            this.picturePreview.Size = new System.Drawing.Size(640, 480);
            this.picturePreview.TabIndex = 0;
            this.picturePreview.TabStop = false;
            // 
            // btnToggle
            // 
            this.btnToggle.Location = new System.Drawing.Point(12, 500);
            this.btnToggle.Name = "btnToggle";
            this.btnToggle.Size = new System.Drawing.Size(100, 30);
            this.btnToggle.TabIndex = 1;
            this.btnToggle.Text = "BAŞLA";
            this.btnToggle.UseVisualStyleBackColor = true;
            this.btnToggle.Click += new System.EventHandler(this.btnToggle_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(130, 507);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(74, 15);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Durum: Bekliyor";
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(664, 542);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnToggle);
            this.Controls.Add(this.picturePreview);
            this.Name = "Form1";
            this.Text = "Motion Detector";
            ((System.ComponentModel.ISupportInitialize)(this.picturePreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
