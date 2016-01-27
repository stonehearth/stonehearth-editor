namespace StonehearthEditor
{
   partial class CloneDialog
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
         this.textReplacementsLabel = new System.Windows.Forms.Label();
         this.panel1 = new System.Windows.Forms.Panel();
         this.panel2 = new System.Windows.Forms.Panel();
         this.panel3 = new System.Windows.Forms.Panel();
         this.cloneButton = new System.Windows.Forms.Button();
         this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
         this.originalText1 = new System.Windows.Forms.TextBox();
         this.replacementText1 = new System.Windows.Forms.TextBox();
         this.label1 = new System.Windows.Forms.Label();
         this.panel1.SuspendLayout();
         this.panel2.SuspendLayout();
         this.panel3.SuspendLayout();
         this.tableLayoutPanel1.SuspendLayout();
         this.SuspendLayout();
         // 
         // textReplacementsLabel
         // 
         this.textReplacementsLabel.AutoSize = true;
         this.textReplacementsLabel.Location = new System.Drawing.Point(3, 9);
         this.textReplacementsLabel.Name = "textReplacementsLabel";
         this.textReplacementsLabel.Size = new System.Drawing.Size(148, 13);
         this.textReplacementsLabel.TabIndex = 2;
         this.textReplacementsLabel.Text = "Clone this object by replacing:";
         // 
         // panel1
         // 
         this.panel1.Controls.Add(this.textReplacementsLabel);
         this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
         this.panel1.Location = new System.Drawing.Point(0, 0);
         this.panel1.Name = "panel1";
         this.panel1.Size = new System.Drawing.Size(520, 29);
         this.panel1.TabIndex = 3;
         // 
         // panel2
         // 
         this.panel2.Controls.Add(this.tableLayoutPanel1);
         this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
         this.panel2.Location = new System.Drawing.Point(0, 29);
         this.panel2.Name = "panel2";
         this.panel2.Size = new System.Drawing.Size(520, 162);
         this.panel2.TabIndex = 4;
         // 
         // panel3
         // 
         this.panel3.Controls.Add(this.cloneButton);
         this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.panel3.Location = new System.Drawing.Point(0, 191);
         this.panel3.Name = "panel3";
         this.panel3.Size = new System.Drawing.Size(520, 35);
         this.panel3.TabIndex = 0;
         // 
         // cloneButton
         // 
         this.cloneButton.Location = new System.Drawing.Point(433, 3);
         this.cloneButton.Name = "cloneButton";
         this.cloneButton.Size = new System.Drawing.Size(75, 23);
         this.cloneButton.TabIndex = 0;
         this.cloneButton.Text = "Clone!";
         this.cloneButton.UseVisualStyleBackColor = true;
         this.cloneButton.Click += new System.EventHandler(this.cloneDialogButton_Click);
         // 
         // tableLayoutPanel1
         // 
         this.tableLayoutPanel1.ColumnCount = 3;
         this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 87.45763F));
         this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.54237F));
         this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 224F));
         this.tableLayoutPanel1.Controls.Add(this.originalText1, 0, 0);
         this.tableLayoutPanel1.Controls.Add(this.replacementText1, 2, 0);
         this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
         this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
         this.tableLayoutPanel1.Name = "tableLayoutPanel1";
         this.tableLayoutPanel1.RowCount = 2;
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.28395F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 82.71605F));
         this.tableLayoutPanel1.Size = new System.Drawing.Size(520, 162);
         this.tableLayoutPanel1.TabIndex = 0;
         // 
         // originalText1
         // 
         this.originalText1.Location = new System.Drawing.Point(3, 3);
         this.originalText1.Name = "originalText1";
         this.originalText1.Size = new System.Drawing.Size(245, 20);
         this.originalText1.TabIndex = 0;
         // 
         // replacementText1
         // 
         this.replacementText1.Location = new System.Drawing.Point(298, 3);
         this.replacementText1.Name = "replacementText1";
         this.replacementText1.Size = new System.Drawing.Size(219, 20);
         this.replacementText1.TabIndex = 1;
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(261, 0);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(26, 13);
         this.label1.TabIndex = 2;
         this.label1.Text = "with";
         // 
         // CloneDialog
         // 
         this.AcceptButton = this.cloneButton;
         this.ClientSize = new System.Drawing.Size(520, 226);
         this.Controls.Add(this.panel2);
         this.Controls.Add(this.panel3);
         this.Controls.Add(this.panel1);
         this.Name = "CloneDialog";
         this.Text = "Cloning: ";
         this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CloneDialog_FormClosed);
         this.panel1.ResumeLayout(false);
         this.panel1.PerformLayout();
         this.panel2.ResumeLayout(false);
         this.panel3.ResumeLayout(false);
         this.tableLayoutPanel1.ResumeLayout(false);
         this.tableLayoutPanel1.PerformLayout();
         this.ResumeLayout(false);

      }

      #endregion
      private System.Windows.Forms.Label textReplacementsLabel;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.Panel panel2;
      private System.Windows.Forms.Panel panel3;
      private System.Windows.Forms.Button cloneButton;
      private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
      private System.Windows.Forms.TextBox originalText1;
      private System.Windows.Forms.TextBox replacementText1;
      private System.Windows.Forms.Label label1;
   }
}