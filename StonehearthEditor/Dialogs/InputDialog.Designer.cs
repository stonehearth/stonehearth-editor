namespace StonehearthEditor
{
   partial class InputDialog
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
            this.inputDialogTextBox = new System.Windows.Forms.TextBox();
            this.inputDialogLabel = new System.Windows.Forms.Label();
            this.inputDialogOkayButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // inputDialogTextBox
            // 
            this.inputDialogTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.inputDialogTextBox.Location = new System.Drawing.Point(3, 24);
            this.inputDialogTextBox.Name = "inputDialogTextBox";
            this.inputDialogTextBox.Size = new System.Drawing.Size(158, 20);
            this.inputDialogTextBox.TabIndex = 0;
            // 
            // inputDialogLabel
            // 
            this.inputDialogLabel.AutoSize = true;
            this.inputDialogLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.inputDialogLabel.Location = new System.Drawing.Point(3, 0);
            this.inputDialogLabel.Name = "inputDialogLabel";
            this.inputDialogLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 3);
            this.inputDialogLabel.Size = new System.Drawing.Size(158, 21);
            this.inputDialogLabel.TabIndex = 1;
            this.inputDialogLabel.Text = "Type the name of the new node";
            // 
            // inputDialogOkayButton
            // 
            this.inputDialogOkayButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.inputDialogOkayButton.Location = new System.Drawing.Point(83, 0);
            this.inputDialogOkayButton.Name = "inputDialogOkayButton";
            this.inputDialogOkayButton.Size = new System.Drawing.Size(75, 30);
            this.inputDialogOkayButton.TabIndex = 0;
            this.inputDialogOkayButton.Text = "Clone!";
            this.inputDialogOkayButton.UseVisualStyleBackColor = true;
            this.inputDialogOkayButton.Click += new System.EventHandler(this.inputDialogOkayButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.inputDialogOkayButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 50);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(158, 30);
            this.panel1.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.inputDialogLabel);
            this.flowLayoutPanel1.Controls.Add(this.inputDialogTextBox);
            this.flowLayoutPanel1.Controls.Add(this.panel1);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(5, 5);
            this.flowLayoutPanel1.MaximumSize = new System.Drawing.Size(1000, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(461, 83);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // InputDialog
            // 
            this.AcceptButton = this.inputDialogOkayButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(471, 93);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "InputDialog";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InputDialog";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.InputDialog_FormClosed);
            this.panel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

      }

      #endregion
      private System.Windows.Forms.TextBox inputDialogTextBox;
      private System.Windows.Forms.Label inputDialogLabel;
      private System.Windows.Forms.Button inputDialogOkayButton;
      private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}