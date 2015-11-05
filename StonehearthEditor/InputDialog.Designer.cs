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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputDialog));
         this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
         this.inputDialogTextBox = new System.Windows.Forms.TextBox();
         this.inputDialogLabel = new System.Windows.Forms.Label();
         this.inputDialogOkayButton = new System.Windows.Forms.Button();
         this.panel1 = new System.Windows.Forms.Panel();
         this.panel1.SuspendLayout();
         this.SuspendLayout();
         // 
         // printPreviewDialog1
         // 
         this.printPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
         this.printPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
         this.printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
         this.printPreviewDialog1.Enabled = true;
         this.printPreviewDialog1.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog1.Icon")));
         this.printPreviewDialog1.Name = "printPreviewDialog1";
         this.printPreviewDialog1.Visible = false;
         // 
         // inputDialogTextBox
         // 
         this.inputDialogTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
         this.inputDialogTextBox.Location = new System.Drawing.Point(5, 26);
         this.inputDialogTextBox.Name = "inputDialogTextBox";
         this.inputDialogTextBox.Size = new System.Drawing.Size(461, 20);
         this.inputDialogTextBox.TabIndex = 0;
         // 
         // inputDialogLabel
         // 
         this.inputDialogLabel.AutoSize = true;
         this.inputDialogLabel.Dock = System.Windows.Forms.DockStyle.Top;
         this.inputDialogLabel.Location = new System.Drawing.Point(5, 5);
         this.inputDialogLabel.Name = "inputDialogLabel";
         this.inputDialogLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 3);
         this.inputDialogLabel.Size = new System.Drawing.Size(158, 21);
         this.inputDialogLabel.TabIndex = 1;
         this.inputDialogLabel.Text = "Type the name of the new node";
         // 
         // inputDialogOkayButton
         // 
         this.inputDialogOkayButton.Dock = System.Windows.Forms.DockStyle.Right;
         this.inputDialogOkayButton.Location = new System.Drawing.Point(386, 0);
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
         this.panel1.Location = new System.Drawing.Point(5, 53);
         this.panel1.Name = "panel1";
         this.panel1.Size = new System.Drawing.Size(461, 30);
         this.panel1.TabIndex = 2;
         // 
         // InputDialog
         // 
         this.AcceptButton = this.inputDialogOkayButton;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(471, 88);
         this.Controls.Add(this.panel1);
         this.Controls.Add(this.inputDialogTextBox);
         this.Controls.Add(this.inputDialogLabel);
         this.Name = "InputDialog";
         this.Padding = new System.Windows.Forms.Padding(5);
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "InputDialog";
         this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.InputDialog_FormClosed);
         this.panel1.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
      private System.Windows.Forms.TextBox inputDialogTextBox;
      private System.Windows.Forms.Label inputDialogLabel;
      private System.Windows.Forms.Button inputDialogOkayButton;
      private System.Windows.Forms.Panel panel1;
   }
}