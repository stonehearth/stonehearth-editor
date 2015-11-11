namespace StonehearthEditor
{
   partial class FilePreview
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
         this.components = new System.ComponentModel.Container();
         this.textBox = new System.Windows.Forms.RichTextBox();
         this.i18nTooltip = new System.Windows.Forms.ToolTip(this.components);
         this.filePreviewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.filePreviewContextMenu.SuspendLayout();
         this.SuspendLayout();
         // 
         // textBox
         // 
         this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
         this.textBox.Location = new System.Drawing.Point(0, 0);
         this.textBox.Name = "textBox";
         this.textBox.Size = new System.Drawing.Size(150, 150);
         this.textBox.TabIndex = 0;
         this.textBox.Text = "";
         this.textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
         this.textBox.Leave += new System.EventHandler(this.textBox_Leave);
         this.textBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.textBox_MouseMove);
         // 
         // filePreviewContextMenu
         // 
         this.filePreviewContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem});
         this.filePreviewContextMenu.Name = "filePreviewContextMenu";
         this.filePreviewContextMenu.Size = new System.Drawing.Size(139, 26);
         // 
         // saveToolStripMenuItem
         // 
         this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
         this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
         this.saveToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
         this.saveToolStripMenuItem.Text = "Save";
         this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
         // 
         // FilePreview
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ContextMenuStrip = this.filePreviewContextMenu;
         this.Controls.Add(this.textBox);
         this.Name = "FilePreview";
         this.filePreviewContextMenu.ResumeLayout(false);
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.RichTextBox textBox;
      private System.Windows.Forms.ToolTip i18nTooltip;
      private System.Windows.Forms.ContextMenuStrip filePreviewContextMenu;
      private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
   }
}
