namespace Ab3d.DXEngine.WinForms.Sample
{
    partial class PictureBoxSample
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.startRotateButton = new System.Windows.Forms.Button();
            this.stopRotateButton = new System.Windows.Forms.Button();
            this.anaglyphButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Location = new System.Drawing.Point(17, 16);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(990, 473);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // startRotateButton
            // 
            this.startRotateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.startRotateButton.Enabled = false;
            this.startRotateButton.Location = new System.Drawing.Point(13, 497);
            this.startRotateButton.Margin = new System.Windows.Forms.Padding(4);
            this.startRotateButton.Name = "startRotateButton";
            this.startRotateButton.Size = new System.Drawing.Size(167, 28);
            this.startRotateButton.TabIndex = 1;
            this.startRotateButton.Text = "Start rotate";
            this.startRotateButton.UseVisualStyleBackColor = true;
            this.startRotateButton.Click += new System.EventHandler(this.startRotateButton_Click);
            // 
            // stopRotateButton
            // 
            this.stopRotateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.stopRotateButton.Location = new System.Drawing.Point(188, 497);
            this.stopRotateButton.Margin = new System.Windows.Forms.Padding(4);
            this.stopRotateButton.Name = "stopRotateButton";
            this.stopRotateButton.Size = new System.Drawing.Size(167, 28);
            this.stopRotateButton.TabIndex = 2;
            this.stopRotateButton.Text = "Stop rotate";
            this.stopRotateButton.UseVisualStyleBackColor = true;
            this.stopRotateButton.Click += new System.EventHandler(this.stopRotateButton_Click);
            // 
            // anaglyphButton
            // 
            this.anaglyphButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.anaglyphButton.Location = new System.Drawing.Point(385, 497);
            this.anaglyphButton.Margin = new System.Windows.Forms.Padding(4);
            this.anaglyphButton.Name = "anaglyphButton";
            this.anaglyphButton.Size = new System.Drawing.Size(167, 28);
            this.anaglyphButton.TabIndex = 3;
            this.anaglyphButton.Text = "Show Anaglyph VR";
            this.anaglyphButton.UseVisualStyleBackColor = true;
            this.anaglyphButton.Click += new System.EventHandler(this.anaglyphButton_Click);
            // 
            // PictureBoxSample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 538);
            this.Controls.Add(this.anaglyphButton);
            this.Controls.Add(this.stopRotateButton);
            this.Controls.Add(this.startRotateButton);
            this.Controls.Add(this.pictureBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PictureBoxSample";
            this.Text = "Ab3d.DXEngine in PictureBox";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button startRotateButton;
        private System.Windows.Forms.Button stopRotateButton;
        private System.Windows.Forms.Button anaglyphButton;
    }
}