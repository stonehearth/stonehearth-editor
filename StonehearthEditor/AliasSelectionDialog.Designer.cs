namespace StonehearthEditor
{
   partial class AliasSelectionDialog
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

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.filterTextBox = new System.Windows.Forms.TextBox();
         this.listBox = new System.Windows.Forms.ListBox();
         this.filterLabel = new System.Windows.Forms.Label();
         this.acceptButton = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // filterTextBox
         // 
         this.filterTextBox.Location = new System.Drawing.Point(44, 3);
         this.filterTextBox.Name = "filterTextBox";
         this.filterTextBox.Size = new System.Drawing.Size(239, 20);
         this.filterTextBox.TabIndex = 0;
         this.filterTextBox.TextChanged += new System.EventHandler(this.filterTextBox_TextChanged);
         // 
         // listBox
         // 
         this.listBox.FormattingEnabled = true;
         this.listBox.Location = new System.Drawing.Point(3, 29);
         this.listBox.Name = "listBox";
         this.listBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
         this.listBox.Size = new System.Drawing.Size(280, 199);
         this.listBox.TabIndex = 1;
         this.listBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseDoubleClick);
         // 
         // filterLabel
         // 
         this.filterLabel.AutoSize = true;
         this.filterLabel.Location = new System.Drawing.Point(6, 6);
         this.filterLabel.Name = "filterLabel";
         this.filterLabel.Size = new System.Drawing.Size(32, 13);
         this.filterLabel.TabIndex = 2;
         this.filterLabel.Text = "Filter:";
         // 
         // acceptButton
         // 
         this.acceptButton.Location = new System.Drawing.Point(208, 234);
         this.acceptButton.Name = "acceptButton";
         this.acceptButton.Size = new System.Drawing.Size(75, 23);
         this.acceptButton.TabIndex = 3;
         this.acceptButton.Text = "Select";
         this.acceptButton.UseVisualStyleBackColor = true;
         this.acceptButton.Click += new System.EventHandler(this.acceptButton_Click);
         // 
         // AliasSelectionDialog
         // 
         this.AcceptButton = this.acceptButton;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(284, 261);
         this.Controls.Add(this.acceptButton);
         this.Controls.Add(this.filterLabel);
         this.Controls.Add(this.listBox);
         this.Controls.Add(this.filterTextBox);
         this.Name = "AliasSelectionDialog";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "Select Alias";
         this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AliasSelectionDialog_FormClosed);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.TextBox filterTextBox;
      private System.Windows.Forms.ListBox listBox;
      private System.Windows.Forms.Label filterLabel;
      private System.Windows.Forms.Button acceptButton;
   }
}
