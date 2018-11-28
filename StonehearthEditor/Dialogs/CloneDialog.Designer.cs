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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CloneDialog));
            this.textReplacementsLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.AddParamButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.parametersTable = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.modListDropdown = new System.Windows.Forms.ComboBox();
            this.cloneToModLabel = new System.Windows.Forms.Label();
            this.sourceModLabel = new System.Windows.Forms.Label();
            this.cloneFromLabel = new System.Windows.Forms.Label();
            this.cloneButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.parametersTable.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // textReplacementsLabel
            // 
            this.textReplacementsLabel.AutoSize = true;
            this.textReplacementsLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.textReplacementsLabel.Location = new System.Drawing.Point(5, 5);
            this.textReplacementsLabel.Name = "textReplacementsLabel";
            this.textReplacementsLabel.Size = new System.Drawing.Size(197, 17);
            this.textReplacementsLabel.TabIndex = 2;
            this.textReplacementsLabel.Text = "Clone this object by replacing:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.AddParamButton);
            this.panel1.Controls.Add(this.textReplacementsLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(520, 35);
            this.panel1.TabIndex = 3;
            // 
            // AddParamButton
            // 
            this.AddParamButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.AddParamButton.Image = ((System.Drawing.Image)(resources.GetObject("AddParamButton.Image")));
            this.AddParamButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.AddParamButton.Location = new System.Drawing.Point(489, 5);
            this.AddParamButton.Name = "AddParamButton";
            this.AddParamButton.Size = new System.Drawing.Size(26, 25);
            this.AddParamButton.TabIndex = 3;
            this.AddParamButton.UseVisualStyleBackColor = true;
            this.AddParamButton.Click += new System.EventHandler(this.AddParamButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(263, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "with replacement text";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "original text . . .";
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.Controls.Add(this.parametersTable);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 35);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(520, 120);
            this.panel2.TabIndex = 4;
            // 
            // parametersTable
            // 
            this.parametersTable.AutoScroll = true;
            this.parametersTable.AutoSize = true;
            this.parametersTable.ColumnCount = 2;
            this.parametersTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.parametersTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.parametersTable.Controls.Add(this.label3, 1, 0);
            this.parametersTable.Controls.Add(this.label1, 0, 0);
            this.parametersTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parametersTable.Location = new System.Drawing.Point(0, 0);
            this.parametersTable.Name = "parametersTable";
            this.parametersTable.RowCount = 1;
            this.parametersTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.parametersTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.parametersTable.Size = new System.Drawing.Size(520, 120);
            this.parametersTable.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.modListDropdown);
            this.panel3.Controls.Add(this.cloneToModLabel);
            this.panel3.Controls.Add(this.sourceModLabel);
            this.panel3.Controls.Add(this.cloneFromLabel);
            this.panel3.Controls.Add(this.cloneButton);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 155);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(5);
            this.panel3.Size = new System.Drawing.Size(520, 35);
            this.panel3.TabIndex = 0;
            // 
            // modListDropdown
            // 
            this.modListDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modListDropdown.FormattingEnabled = true;
            this.modListDropdown.Location = new System.Drawing.Point(304, 8);
            this.modListDropdown.Name = "modListDropdown";
            this.modListDropdown.Size = new System.Drawing.Size(130, 24);
            this.modListDropdown.TabIndex = 4;
            // 
            // cloneToModLabel
            // 
            this.cloneToModLabel.AutoSize = true;
            this.cloneToModLabel.Location = new System.Drawing.Point(257, 11);
            this.cloneToModLabel.Name = "cloneToModLabel";
            this.cloneToModLabel.Size = new System.Drawing.Size(64, 17);
            this.cloneToModLabel.TabIndex = 3;
            this.cloneToModLabel.Text = "Clone to:";
            // 
            // sourceModLabel
            // 
            this.sourceModLabel.AutoSize = true;
            this.sourceModLabel.Location = new System.Drawing.Point(67, 11);
            this.sourceModLabel.Name = "sourceModLabel";
            this.sourceModLabel.Size = new System.Drawing.Size(86, 17);
            this.sourceModLabel.TabIndex = 2;
            this.sourceModLabel.Text = "source_mod";
            // 
            // cloneFromLabel
            // 
            this.cloneFromLabel.AutoSize = true;
            this.cloneFromLabel.Location = new System.Drawing.Point(5, 11);
            this.cloneFromLabel.Name = "cloneFromLabel";
            this.cloneFromLabel.Size = new System.Drawing.Size(80, 17);
            this.cloneFromLabel.TabIndex = 1;
            this.cloneFromLabel.Text = "Clone from:";
            // 
            // cloneButton
            // 
            this.cloneButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.cloneButton.Location = new System.Drawing.Point(440, 5);
            this.cloneButton.Name = "cloneButton";
            this.cloneButton.Size = new System.Drawing.Size(75, 25);
            this.cloneButton.TabIndex = 0;
            this.cloneButton.Text = "Clone!";
            this.cloneButton.UseVisualStyleBackColor = true;
            this.cloneButton.Click += new System.EventHandler(this.cloneDialogButton_Click);
            // 
            // CloneDialog
            // 
            this.AcceptButton = this.cloneButton;
            this.ClientSize = new System.Drawing.Size(520, 190);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Name = "CloneDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cloning: ";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CloneDialog_FormClosed);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.parametersTable.ResumeLayout(false);
            this.parametersTable.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

      }

      #endregion
      private System.Windows.Forms.Label textReplacementsLabel;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.Panel panel2;
      private System.Windows.Forms.Panel panel3;
      private System.Windows.Forms.Button cloneButton;
      private System.Windows.Forms.TableLayoutPanel parametersTable;
      private System.Windows.Forms.Button AddParamButton;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox modListDropdown;
        private System.Windows.Forms.Label cloneToModLabel;
        private System.Windows.Forms.Label sourceModLabel;
        private System.Windows.Forms.Label cloneFromLabel;
    }
}