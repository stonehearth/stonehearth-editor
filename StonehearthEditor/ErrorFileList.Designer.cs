namespace StonehearthEditor
{
   partial class ErrorFileList
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
         this.components = new System.ComponentModel.Container();
         this.errorsTextBox = new System.Windows.Forms.TextBox();
         this.listBox = new System.Windows.Forms.ListBox();
         this.panel1 = new System.Windows.Forms.Panel();
         this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
         this.errorsLabel = new System.Windows.Forms.Label();
         this.panel1.SuspendLayout();
         this.SuspendLayout();
         // 
         // errorsTextBox
         // 
         this.errorsTextBox.Location = new System.Drawing.Point(50, 6);
         this.errorsTextBox.Multiline = true;
         this.errorsTextBox.Name = "errorsTextBox";
         this.errorsTextBox.Size = new System.Drawing.Size(546, 119);
         this.errorsTextBox.TabIndex = 4;
         // 
         // listBox
         // 
         this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
         this.listBox.FormattingEnabled = true;
         this.listBox.Location = new System.Drawing.Point(0, 131);
         this.listBox.Name = "listBox";
         this.listBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
         this.listBox.Size = new System.Drawing.Size(608, 292);
         this.listBox.TabIndex = 5;
         this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
         this.listBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseDoubleClick);
         // 
         // panel1
         // 
         this.panel1.Controls.Add(this.errorsLabel);
         this.panel1.Controls.Add(this.errorsTextBox);
         this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
         this.panel1.Location = new System.Drawing.Point(0, 0);
         this.panel1.Name = "panel1";
         this.panel1.Size = new System.Drawing.Size(608, 131);
         this.panel1.TabIndex = 7;
         // 
         // errorsLabel
         // 
         this.errorsLabel.AutoSize = true;
         this.errorsLabel.Location = new System.Drawing.Point(3, 9);
         this.errorsLabel.Name = "errorsLabel";
         this.errorsLabel.Size = new System.Drawing.Size(34, 13);
         this.errorsLabel.TabIndex = 6;
         this.errorsLabel.Text = "Errors";
         // 
         // ErrorFileList
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(608, 423);
         this.Controls.Add(this.listBox);
         this.Controls.Add(this.panel1);
         this.Name = "ErrorFileList";
         this.Text = "ErrorFileList";
         this.panel1.ResumeLayout(false);
         this.panel1.PerformLayout();
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.TextBox errorsTextBox;
      private System.Windows.Forms.ListBox listBox;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.ToolTip toolTip1;
      private System.Windows.Forms.Label errorsLabel;
   }
}