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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilePreview));
         this.textBox = new System.Windows.Forms.RichTextBox();
         this.i18nTooltip = new System.Windows.Forms.ToolTip(this.components);
         this.filePreviewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStrip = new System.Windows.Forms.ToolStrip();
         this.insertAliasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.openFile = new System.Windows.Forms.ToolStripButton();
         this.openFolder = new System.Windows.Forms.ToolStripButton();
         this.saveFile = new System.Windows.Forms.ToolStripButton();
         this.localizeFile = new System.Windows.Forms.ToolStripButton();
         this.filePreviewContextMenu.SuspendLayout();
         this.toolStrip.SuspendLayout();
         this.SuspendLayout();
         // 
         // textBox
         // 
         this.textBox.ContextMenuStrip = this.filePreviewContextMenu;
         this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
         this.textBox.Location = new System.Drawing.Point(0, 25);
         this.textBox.Name = "textBox";
         this.textBox.Size = new System.Drawing.Size(150, 125);
         this.textBox.TabIndex = 0;
         this.textBox.Text = "";
         this.textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
         this.textBox.Leave += new System.EventHandler(this.textBox_Leave);
         this.textBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.textBox_MouseMove);
         // 
         // filePreviewContextMenu
         // 
         this.filePreviewContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.insertAliasToolStripMenuItem});
         this.filePreviewContextMenu.Name = "filePreviewContextMenu";
         this.filePreviewContextMenu.Size = new System.Drawing.Size(197, 48);
         // 
         // saveToolStripMenuItem
         // 
         this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
         this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
         this.saveToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
         this.saveToolStripMenuItem.Text = "Save";
         this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
         // 
         // toolStrip
         // 
         this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFile,
            this.openFolder,
            this.saveFile,
            this.localizeFile});
         this.toolStrip.Location = new System.Drawing.Point(0, 0);
         this.toolStrip.Name = "toolStrip";
         this.toolStrip.Size = new System.Drawing.Size(150, 25);
         this.toolStrip.TabIndex = 2;
         this.toolStrip.Text = "toolStrip1";
         // 
         // insertAliasToolStripMenuItem
         // 
         this.insertAliasToolStripMenuItem.Name = "insertAliasToolStripMenuItem";
         this.insertAliasToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Space)));
         this.insertAliasToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
         this.insertAliasToolStripMenuItem.Text = "Insert Alias";
         this.insertAliasToolStripMenuItem.Click += new System.EventHandler(this.insertAliasToolStripMenuItem_Click);
         // 
         // openFile
         // 
         this.openFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.openFile.Image = ((System.Drawing.Image)(resources.GetObject("openFile.Image")));
         this.openFile.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.openFile.Name = "openFile";
         this.openFile.Size = new System.Drawing.Size(23, 22);
         this.openFile.Text = "Open File in Text Editor";
         this.openFile.Click += new System.EventHandler(this.openFile_Click);
         // 
         // openFolder
         // 
         this.openFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.openFolder.Image = ((System.Drawing.Image)(resources.GetObject("openFolder.Image")));
         this.openFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.openFolder.Name = "openFolder";
         this.openFolder.Size = new System.Drawing.Size(23, 22);
         this.openFolder.Text = "Open Containing Folder";
         this.openFolder.Click += new System.EventHandler(this.openFolder_Click);
         // 
         // saveFile
         // 
         this.saveFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.saveFile.Image = ((System.Drawing.Image)(resources.GetObject("saveFile.Image")));
         this.saveFile.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.saveFile.Name = "saveFile";
         this.saveFile.Size = new System.Drawing.Size(23, 22);
         this.saveFile.Text = "Save File";
         this.saveFile.Click += new System.EventHandler(this.saveFile_Click);
         // 
         // localizeFile
         // 
         this.localizeFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.localizeFile.Image = ((System.Drawing.Image)(resources.GetObject("localizeFile.Image")));
         this.localizeFile.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.localizeFile.Name = "localizeFile";
         this.localizeFile.Size = new System.Drawing.Size(23, 22);
         this.localizeFile.Text = "Localize This File";
         this.localizeFile.Click += new System.EventHandler(this.localizeFile_Click);
         // 
         // FilePreview
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ContextMenuStrip = this.filePreviewContextMenu;
         this.Controls.Add(this.textBox);
         this.Controls.Add(this.toolStrip);
         this.Name = "FilePreview";
         this.filePreviewContextMenu.ResumeLayout(false);
         this.toolStrip.ResumeLayout(false);
         this.toolStrip.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.RichTextBox textBox;
      private System.Windows.Forms.ToolTip i18nTooltip;
      private System.Windows.Forms.ContextMenuStrip filePreviewContextMenu;
      private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
      private System.Windows.Forms.ToolStrip toolStrip;
      private System.Windows.Forms.ToolStripButton saveFile;
      private System.Windows.Forms.ToolStripButton openFolder;
      private System.Windows.Forms.ToolStripButton openFile;
      private System.Windows.Forms.ToolStripButton localizeFile;
      private System.Windows.Forms.ToolStripMenuItem insertAliasToolStripMenuItem;
   }
}
