namespace StonehearthEditor
{
    partial class ModDirectorySettingsDialog
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
            this.baseModsLabel = new System.Windows.Forms.Label();
            this.baseModsPathTextbox = new System.Windows.Forms.TextBox();
            this.changeBaseModsPathButton = new System.Windows.Forms.Button();
            this.steamUploadsPathLabel = new System.Windows.Forms.Label();
            this.steamUploadsPathTextbox = new System.Windows.Forms.TextBox();
            this.changeSteamUploadsPathButton = new System.Windows.Forms.Button();
            this.applyButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // baseModsLabel
            // 
            this.baseModsLabel.AutoSize = true;
            this.baseModsLabel.Location = new System.Drawing.Point(13, 13);
            this.baseModsLabel.Name = "baseModsLabel";
            this.baseModsLabel.Size = new System.Drawing.Size(371, 17);
            this.baseModsLabel.TabIndex = 0;
            this.baseModsLabel.Text = "Base mods root directory (where the stonehearth mod is):";
            // 
            // baseModsPathTextbox
            // 
            this.baseModsPathTextbox.Location = new System.Drawing.Point(16, 34);
            this.baseModsPathTextbox.Name = "baseModsPathTextbox";
            this.baseModsPathTextbox.Size = new System.Drawing.Size(452, 22);
            this.baseModsPathTextbox.TabIndex = 1;
            // 
            // changeBaseModsPathButton
            // 
            this.changeBaseModsPathButton.Location = new System.Drawing.Point(474, 30);
            this.changeBaseModsPathButton.Name = "changeBaseModsPathButton";
            this.changeBaseModsPathButton.Size = new System.Drawing.Size(120, 30);
            this.changeBaseModsPathButton.TabIndex = 2;
            this.changeBaseModsPathButton.Text = "Browse...";
            this.changeBaseModsPathButton.UseVisualStyleBackColor = true;
            this.changeBaseModsPathButton.Click += new System.EventHandler(this.changeBaseModsPathButton_Click);
            // 
            // steamUploadsPathLabel
            // 
            this.steamUploadsPathLabel.AutoSize = true;
            this.steamUploadsPathLabel.Location = new System.Drawing.Point(13, 84);
            this.steamUploadsPathLabel.Name = "steamUploadsPathLabel";
            this.steamUploadsPathLabel.Size = new System.Drawing.Size(309, 17);
            this.steamUploadsPathLabel.TabIndex = 3;
            this.steamUploadsPathLabel.Text = "Additional mods directory (e.g. steam_uploads):";
            // 
            // steamUploadsPathTextbox
            // 
            this.steamUploadsPathTextbox.Location = new System.Drawing.Point(16, 109);
            this.steamUploadsPathTextbox.Name = "steamUploadsPathTextbox";
            this.steamUploadsPathTextbox.Size = new System.Drawing.Size(452, 22);
            this.steamUploadsPathTextbox.TabIndex = 4;
            // 
            // changeSteamUploadsPathButton
            // 
            this.changeSteamUploadsPathButton.Location = new System.Drawing.Point(474, 104);
            this.changeSteamUploadsPathButton.Name = "changeSteamUploadsPathButton";
            this.changeSteamUploadsPathButton.Size = new System.Drawing.Size(120, 32);
            this.changeSteamUploadsPathButton.TabIndex = 5;
            this.changeSteamUploadsPathButton.Text = "Browse...";
            this.changeSteamUploadsPathButton.UseVisualStyleBackColor = true;
            this.changeSteamUploadsPathButton.Click += new System.EventHandler(this.changeSteamUploadsPathButton_Click);
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(402, 182);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(215, 34);
            this.applyButton.TabIndex = 6;
            this.applyButton.Text = "Apply and reload";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // ModDirectorySettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 228);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.changeSteamUploadsPathButton);
            this.Controls.Add(this.steamUploadsPathTextbox);
            this.Controls.Add(this.steamUploadsPathLabel);
            this.Controls.Add(this.changeBaseModsPathButton);
            this.Controls.Add(this.baseModsPathTextbox);
            this.Controls.Add(this.baseModsLabel);
            this.Name = "ModDirectorySettingsDialog";
            this.Text = "Mods directories";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label baseModsLabel;
        private System.Windows.Forms.TextBox baseModsPathTextbox;
        private System.Windows.Forms.Button changeBaseModsPathButton;
        private System.Windows.Forms.Label steamUploadsPathLabel;
        private System.Windows.Forms.TextBox steamUploadsPathTextbox;
        private System.Windows.Forms.Button changeSteamUploadsPathButton;
        private System.Windows.Forms.Button applyButton;
    }
}