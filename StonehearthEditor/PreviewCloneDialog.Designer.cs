namespace StonehearthEditor
{
   partial class PreviewCloneDialog
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
         this.dependenciesListBox = new System.Windows.Forms.CheckedListBox();
         this.label1 = new System.Windows.Forms.Label();
         this.panel1 = new System.Windows.Forms.Panel();
         this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
         this.acceptButton = new System.Windows.Forms.Button();
         this.cancelButton = new System.Windows.Forms.Button();
         this.panel1.SuspendLayout();
         this.flowLayoutPanel1.SuspendLayout();
         this.SuspendLayout();
         // 
         // dependenciesListBox
         // 
         this.dependenciesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
         this.dependenciesListBox.FormattingEnabled = true;
         this.dependenciesListBox.Location = new System.Drawing.Point(10, 10);
         this.dependenciesListBox.Margin = new System.Windows.Forms.Padding(10);
         this.dependenciesListBox.Name = "dependenciesListBox";
         this.dependenciesListBox.Size = new System.Drawing.Size(668, 264);
         this.dependenciesListBox.TabIndex = 0;
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Dock = System.Windows.Forms.DockStyle.Top;
         this.label1.Location = new System.Drawing.Point(0, 0);
         this.label1.Margin = new System.Windows.Forms.Padding(3, 20, 3, 0);
         this.label1.Name = "label1";
         this.label1.Padding = new System.Windows.Forms.Padding(10);
         this.label1.Size = new System.Drawing.Size(185, 33);
         this.label1.TabIndex = 1;
         this.label1.Text = "The following files will be created:";
         // 
         // panel1
         // 
         this.panel1.Controls.Add(this.dependenciesListBox);
         this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.panel1.Location = new System.Drawing.Point(0, 33);
         this.panel1.Name = "panel1";
         this.panel1.Padding = new System.Windows.Forms.Padding(10);
         this.panel1.Size = new System.Drawing.Size(688, 284);
         this.panel1.TabIndex = 2;
         // 
         // flowLayoutPanel1
         // 
         this.flowLayoutPanel1.Controls.Add(this.acceptButton);
         this.flowLayoutPanel1.Controls.Add(this.cancelButton);
         this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
         this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 317);
         this.flowLayoutPanel1.Name = "flowLayoutPanel1";
         this.flowLayoutPanel1.Size = new System.Drawing.Size(688, 31);
         this.flowLayoutPanel1.TabIndex = 1;
         // 
         // acceptButton
         // 
         this.acceptButton.Location = new System.Drawing.Point(610, 3);
         this.acceptButton.Name = "acceptButton";
         this.acceptButton.Size = new System.Drawing.Size(75, 23);
         this.acceptButton.TabIndex = 0;
         this.acceptButton.Text = "Accept";
         this.acceptButton.UseVisualStyleBackColor = true;
         this.acceptButton.Click += new System.EventHandler(this.acceptButton_Click);
         // 
         // cancelButton
         // 
         this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.cancelButton.Location = new System.Drawing.Point(529, 3);
         this.cancelButton.Name = "cancelButton";
         this.cancelButton.Size = new System.Drawing.Size(75, 23);
         this.cancelButton.TabIndex = 1;
         this.cancelButton.Text = "Cancel";
         this.cancelButton.UseVisualStyleBackColor = true;
         this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
         // 
         // PreviewCloneDialog
         // 
         this.AcceptButton = this.acceptButton;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.cancelButton;
         this.ClientSize = new System.Drawing.Size(688, 348);
         this.Controls.Add(this.panel1);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.flowLayoutPanel1);
         this.KeyPreview = true;
         this.Name = "PreviewCloneDialog";
         this.Text = "Clone Preview";
         this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PreviewCloneDialog_FormClosed);
         this.panel1.ResumeLayout(false);
         this.flowLayoutPanel1.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.CheckedListBox dependenciesListBox;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
      private System.Windows.Forms.Button acceptButton;
      private System.Windows.Forms.Button cancelButton;
   }
}