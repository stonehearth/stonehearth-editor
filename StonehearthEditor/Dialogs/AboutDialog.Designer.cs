namespace StonehearthEditor.Dialogs
{
    partial class AboutDialog
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
            this.SHEDLabel = new System.Windows.Forms.Label();
            this.SHEDHomePageLinkLabel = new System.Windows.Forms.LinkLabel();
            this.SHEDLabel2 = new System.Windows.Forms.Label();
            this.moddingGuideLabel = new System.Windows.Forms.Label();
            this.moddingGuideLinkLabel = new System.Windows.Forms.LinkLabel();
            this.OKbutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SHEDLabel
            // 
            this.SHEDLabel.Location = new System.Drawing.Point(13, 13);
            this.SHEDLabel.Name = "SHEDLabel";
            this.SHEDLabel.Size = new System.Drawing.Size(369, 22);
            this.SHEDLabel.TabIndex = 0;
            this.SHEDLabel.Text = "SHED is an open source program. You can find it here:";
            // 
            // SHEDHomePageLinkLabel
            // 
            this.SHEDHomePageLinkLabel.AutoSize = true;
            this.SHEDHomePageLinkLabel.Location = new System.Drawing.Point(13, 48);
            this.SHEDHomePageLinkLabel.Name = "SHEDHomePageLinkLabel";
            this.SHEDHomePageLinkLabel.Size = new System.Drawing.Size(321, 17);
            this.SHEDHomePageLinkLabel.TabIndex = 1;
            this.SHEDHomePageLinkLabel.TabStop = true;
            this.SHEDHomePageLinkLabel.Text = "https://github.com/stonehearth/stonehearth-editor";
            this.SHEDHomePageLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SHEDHomePageLinkLabel_LinkClicked);
            // 
            // SHEDLabel2
            // 
            this.SHEDLabel2.AutoSize = true;
            this.SHEDLabel2.Location = new System.Drawing.Point(12, 79);
            this.SHEDLabel2.Name = "SHEDLabel2";
            this.SHEDLabel2.Size = new System.Drawing.Size(393, 17);
            this.SHEDLabel2.TabIndex = 2;
            this.SHEDLabel2.Text = "Feel free to fork the project and improve it for other modders.";
            // 
            // moddingGuideLabel
            // 
            this.moddingGuideLabel.AutoSize = true;
            this.moddingGuideLabel.Location = new System.Drawing.Point(12, 119);
            this.moddingGuideLabel.Name = "moddingGuideLabel";
            this.moddingGuideLabel.Size = new System.Drawing.Size(321, 17);
            this.moddingGuideLabel.TabIndex = 3;
            this.moddingGuideLabel.Text = "Here\'s the official modding guide for Stonehearth:";
            // 
            // moddingGuideLinkLabel
            // 
            this.moddingGuideLinkLabel.AutoSize = true;
            this.moddingGuideLinkLabel.Location = new System.Drawing.Point(12, 154);
            this.moddingGuideLinkLabel.Name = "moddingGuideLinkLabel";
            this.moddingGuideLinkLabel.Size = new System.Drawing.Size(353, 17);
            this.moddingGuideLinkLabel.TabIndex = 4;
            this.moddingGuideLinkLabel.TabStop = true;
            this.moddingGuideLinkLabel.Text = "https://stonehearth.github.io/modding_guide/index.html";
            this.moddingGuideLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.moddingGuideLinkLabel_LinkClicked);
            // 
            // OKbutton
            // 
            this.OKbutton.Location = new System.Drawing.Point(155, 201);
            this.OKbutton.Name = "OKbutton";
            this.OKbutton.Size = new System.Drawing.Size(128, 40);
            this.OKbutton.TabIndex = 5;
            this.OKbutton.Text = "OK";
            this.OKbutton.UseVisualStyleBackColor = true;
            this.OKbutton.Click += new System.EventHandler(this.OKbutton_Click);
            // 
            // AboutDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 253);
            this.Controls.Add(this.OKbutton);
            this.Controls.Add(this.moddingGuideLinkLabel);
            this.Controls.Add(this.moddingGuideLabel);
            this.Controls.Add(this.SHEDLabel2);
            this.Controls.Add(this.SHEDHomePageLinkLabel);
            this.Controls.Add(this.SHEDLabel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About Stonehearth Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label SHEDLabel;
        private System.Windows.Forms.LinkLabel SHEDHomePageLinkLabel;
        private System.Windows.Forms.Label SHEDLabel2;
        private System.Windows.Forms.Label moddingGuideLabel;
        private System.Windows.Forms.LinkLabel moddingGuideLinkLabel;
        private System.Windows.Forms.Button OKbutton;
    }
}