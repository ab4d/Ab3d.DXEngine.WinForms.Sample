namespace Ab3d.DXEngine.WinForms.Sample
{
    partial class DXViewportViewRenderControlSample
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
            this.stopRotationButton = new System.Windows.Forms.Button();
            this.startRotationButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.renderControlHostPanel = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // stopRotationButton
            // 
            this.stopRotationButton.Location = new System.Drawing.Point(6, 41);
            this.stopRotationButton.Name = "stopRotationButton";
            this.stopRotationButton.Size = new System.Drawing.Size(107, 23);
            this.stopRotationButton.TabIndex = 1;
            this.stopRotationButton.Text = "Stop rotation";
            this.stopRotationButton.UseVisualStyleBackColor = true;
            this.stopRotationButton.Click += new System.EventHandler(this.stopRotationButton_Click);
            // 
            // startRotationButton
            // 
            this.startRotationButton.Location = new System.Drawing.Point(6, 12);
            this.startRotationButton.Name = "startRotationButton";
            this.startRotationButton.Size = new System.Drawing.Size(107, 23);
            this.startRotationButton.TabIndex = 2;
            this.startRotationButton.Text = "Start rotation";
            this.startRotationButton.UseVisualStyleBackColor = true;
            this.startRotationButton.Click += new System.EventHandler(this.startRotationButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.startRotationButton);
            this.panel1.Controls.Add(this.stopRotationButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(745, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(125, 449);
            this.panel1.TabIndex = 3;
            // 
            // renderControlHostPanel
            // 
            this.renderControlHostPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderControlHostPanel.Location = new System.Drawing.Point(0, 0);
            this.renderControlHostPanel.Name = "renderControlHostPanel";
            this.renderControlHostPanel.Size = new System.Drawing.Size(745, 449);
            this.renderControlHostPanel.TabIndex = 4;
            // 
            // DXViewportViewRenderControlSample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(870, 449);
            this.Controls.Add(this.renderControlHostPanel);
            this.Controls.Add(this.panel1);
            this.Name = "DXViewportViewRenderControlSample";
            this.Text = "DXViewportView RenderControl sample";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button stopRotationButton;
        private System.Windows.Forms.Button startRotationButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel renderControlHostPanel;
    }
}