namespace StonehearthEditor
{
   partial class ManifestView
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManifestView));
         this.splitContainer2 = new System.Windows.Forms.SplitContainer();
         this.treeView = new System.Windows.Forms.TreeView();
         this.searchPanel = new System.Windows.Forms.Panel();
         this.searchBox = new System.Windows.Forms.TextBox();
         this.searchButton = new System.Windows.Forms.Button();
         this.splitContainer3 = new System.Windows.Forms.SplitContainer();
         this.filePreviewTabs = new System.Windows.Forms.TabControl();
         this.openFileButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
         this.panel3 = new System.Windows.Forms.Panel();
         this.dependenciesListView = new System.Windows.Forms.ListView();
         this.dependenciesLabel = new System.Windows.Forms.Label();
         this.panel2 = new System.Windows.Forms.Panel();
         this.iconView = new System.Windows.Forms.PictureBox();
         this.selectedFilePathTextBox = new System.Windows.Forms.TextBox();
         this.aliasContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.aliasContextDuplicate = new System.Windows.Forms.ToolStripMenuItem();
         this.makeFineVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.copyFullAliasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.manifestImageList = new System.Windows.Forms.ImageList(this.components);
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
         this.splitContainer2.Panel1.SuspendLayout();
         this.splitContainer2.Panel2.SuspendLayout();
         this.splitContainer2.SuspendLayout();
         this.searchPanel.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
         this.splitContainer3.Panel1.SuspendLayout();
         this.splitContainer3.Panel2.SuspendLayout();
         this.splitContainer3.SuspendLayout();
         this.panel3.SuspendLayout();
         this.panel2.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.iconView)).BeginInit();
         this.aliasContextMenu.SuspendLayout();
         this.SuspendLayout();
         // 
         // splitContainer2
         // 
         this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainer2.Location = new System.Drawing.Point(0, 0);
         this.splitContainer2.Name = "splitContainer2";
         // 
         // splitContainer2.Panel1
         // 
         this.splitContainer2.Panel1.Controls.Add(this.treeView);
         this.splitContainer2.Panel1.Controls.Add(this.searchPanel);
         // 
         // splitContainer2.Panel2
         // 
         this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
         this.splitContainer2.Panel2.Controls.Add(this.selectedFilePathTextBox);
         this.splitContainer2.Size = new System.Drawing.Size(681, 472);
         this.splitContainer2.SplitterDistance = 222;
         this.splitContainer2.TabIndex = 3;
         // 
         // treeView
         // 
         this.treeView.ContextMenuStrip = this.aliasContextMenu;
         this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.treeView.HideSelection = false;
         this.treeView.ImageIndex = 0;
         this.treeView.ImageList = this.manifestImageList;
         this.treeView.Location = new System.Drawing.Point(0, 26);
         this.treeView.Name = "treeView";
         this.treeView.SelectedImageIndex = 0;
         this.treeView.Size = new System.Drawing.Size(222, 446);
         this.treeView.TabIndex = 1;
         this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
         this.treeView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseClick);
         // 
         // searchPanel
         // 
         this.searchPanel.Controls.Add(this.searchBox);
         this.searchPanel.Controls.Add(this.searchButton);
         this.searchPanel.Dock = System.Windows.Forms.DockStyle.Top;
         this.searchPanel.Location = new System.Drawing.Point(0, 0);
         this.searchPanel.Name = "searchPanel";
         this.searchPanel.Size = new System.Drawing.Size(222, 26);
         this.searchPanel.TabIndex = 5;
         // 
         // searchBox
         // 
         this.searchBox.Dock = System.Windows.Forms.DockStyle.Fill;
         this.searchBox.Location = new System.Drawing.Point(0, 0);
         this.searchBox.Name = "searchBox";
         this.searchBox.Size = new System.Drawing.Size(199, 20);
         this.searchBox.TabIndex = 3;
         this.searchBox.WordWrap = false;
         this.searchBox.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
         // 
         // searchButton
         // 
         this.searchButton.Dock = System.Windows.Forms.DockStyle.Right;
         this.searchButton.Image = ((System.Drawing.Image)(resources.GetObject("searchButton.Image")));
         this.searchButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
         this.searchButton.Location = new System.Drawing.Point(199, 0);
         this.searchButton.Margin = new System.Windows.Forms.Padding(0);
         this.searchButton.Name = "searchButton";
         this.searchButton.Size = new System.Drawing.Size(23, 26);
         this.searchButton.TabIndex = 4;
         this.searchButton.UseVisualStyleBackColor = true;
         this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
         // 
         // splitContainer3
         // 
         this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainer3.Location = new System.Drawing.Point(0, 20);
         this.splitContainer3.Name = "splitContainer3";
         // 
         // splitContainer3.Panel1
         // 
         this.splitContainer3.Panel1.Controls.Add(this.filePreviewTabs);
         this.splitContainer3.Panel1.Controls.Add(this.openFileButtonPanel);
         // 
         // splitContainer3.Panel2
         // 
         this.splitContainer3.Panel2.Controls.Add(this.panel3);
         this.splitContainer3.Panel2.Controls.Add(this.panel2);
         this.splitContainer3.Size = new System.Drawing.Size(455, 452);
         this.splitContainer3.SplitterDistance = 304;
         this.splitContainer3.TabIndex = 3;
         // 
         // filePreviewTabs
         // 
         this.filePreviewTabs.Dock = System.Windows.Forms.DockStyle.Fill;
         this.filePreviewTabs.Location = new System.Drawing.Point(0, 33);
         this.filePreviewTabs.Name = "filePreviewTabs";
         this.filePreviewTabs.SelectedIndex = 0;
         this.filePreviewTabs.Size = new System.Drawing.Size(304, 419);
         this.filePreviewTabs.TabIndex = 2;
         // 
         // openFileButtonPanel
         // 
         this.openFileButtonPanel.Dock = System.Windows.Forms.DockStyle.Top;
         this.openFileButtonPanel.Location = new System.Drawing.Point(0, 0);
         this.openFileButtonPanel.Name = "openFileButtonPanel";
         this.openFileButtonPanel.Size = new System.Drawing.Size(304, 33);
         this.openFileButtonPanel.TabIndex = 0;
         // 
         // panel3
         // 
         this.panel3.BackColor = System.Drawing.Color.Transparent;
         this.panel3.Controls.Add(this.dependenciesListView);
         this.panel3.Controls.Add(this.dependenciesLabel);
         this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
         this.panel3.Location = new System.Drawing.Point(0, 232);
         this.panel3.Name = "panel3";
         this.panel3.Size = new System.Drawing.Size(147, 220);
         this.panel3.TabIndex = 5;
         // 
         // dependenciesListView
         // 
         this.dependenciesListView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.dependenciesListView.Location = new System.Drawing.Point(0, 13);
         this.dependenciesListView.MultiSelect = false;
         this.dependenciesListView.Name = "dependenciesListView";
         this.dependenciesListView.Size = new System.Drawing.Size(147, 207);
         this.dependenciesListView.TabIndex = 2;
         this.dependenciesListView.UseCompatibleStateImageBehavior = false;
         this.dependenciesListView.View = System.Windows.Forms.View.List;
         this.dependenciesListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dependenciesListView_MouseDoubleClick);
         // 
         // dependenciesLabel
         // 
         this.dependenciesLabel.AutoSize = true;
         this.dependenciesLabel.Dock = System.Windows.Forms.DockStyle.Top;
         this.dependenciesLabel.Location = new System.Drawing.Point(0, 0);
         this.dependenciesLabel.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
         this.dependenciesLabel.Name = "dependenciesLabel";
         this.dependenciesLabel.Size = new System.Drawing.Size(79, 13);
         this.dependenciesLabel.TabIndex = 3;
         this.dependenciesLabel.Text = "Dependencies:";
         // 
         // panel2
         // 
         this.panel2.Controls.Add(this.iconView);
         this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
         this.panel2.Location = new System.Drawing.Point(0, 0);
         this.panel2.Name = "panel2";
         this.panel2.Size = new System.Drawing.Size(147, 232);
         this.panel2.TabIndex = 4;
         // 
         // iconView
         // 
         this.iconView.Location = new System.Drawing.Point(3, 0);
         this.iconView.Name = "iconView";
         this.iconView.Size = new System.Drawing.Size(232, 232);
         this.iconView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
         this.iconView.TabIndex = 3;
         this.iconView.TabStop = false;
         // 
         // selectedFilePathTextBox
         // 
         this.selectedFilePathTextBox.Dock = System.Windows.Forms.DockStyle.Top;
         this.selectedFilePathTextBox.Location = new System.Drawing.Point(0, 0);
         this.selectedFilePathTextBox.Name = "selectedFilePathTextBox";
         this.selectedFilePathTextBox.ReadOnly = true;
         this.selectedFilePathTextBox.Size = new System.Drawing.Size(455, 20);
         this.selectedFilePathTextBox.TabIndex = 4;
         // 
         // aliasContextMenu
         // 
         this.aliasContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aliasContextDuplicate,
            this.makeFineVersionToolStripMenuItem,
            this.copyFullAliasToolStripMenuItem});
         this.aliasContextMenu.Name = "aliasContextMenu";
         this.aliasContextMenu.Size = new System.Drawing.Size(195, 70);
         // 
         // aliasContextDuplicate
         // 
         this.aliasContextDuplicate.Name = "aliasContextDuplicate";
         this.aliasContextDuplicate.Size = new System.Drawing.Size(194, 22);
         this.aliasContextDuplicate.Text = "Clone";
         this.aliasContextDuplicate.Click += new System.EventHandler(this.aliasContextMenuDuplicate_Click);
         // 
         // makeFineVersionToolStripMenuItem
         // 
         this.makeFineVersionToolStripMenuItem.Name = "makeFineVersionToolStripMenuItem";
         this.makeFineVersionToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
         this.makeFineVersionToolStripMenuItem.Text = "Make Fine Version";
         this.makeFineVersionToolStripMenuItem.Click += new System.EventHandler(this.makeFineVersionToolStripMenuItem_Click);
         // 
         // copyFullAliasToolStripMenuItem
         // 
         this.copyFullAliasToolStripMenuItem.Name = "copyFullAliasToolStripMenuItem";
         this.copyFullAliasToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
         this.copyFullAliasToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
         this.copyFullAliasToolStripMenuItem.Text = "Copy Full Alias";
         this.copyFullAliasToolStripMenuItem.Click += new System.EventHandler(this.copyFullAliasToolStripMenuItem_Click);
         // 
         // manifestImageList
         // 
         this.manifestImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("manifestImageList.ImageStream")));
         this.manifestImageList.TransparentColor = System.Drawing.Color.Transparent;
         this.manifestImageList.Images.SetKeyName(0, "none.png");
         this.manifestImageList.Images.SetKeyName(1, "entity.png");
         this.manifestImageList.Images.SetKeyName(2, "buff.png");
         this.manifestImageList.Images.SetKeyName(3, "ai_pack.png");
         this.manifestImageList.Images.SetKeyName(4, "effect.png");
         this.manifestImageList.Images.SetKeyName(5, "recipe.png");
         this.manifestImageList.Images.SetKeyName(6, "command.png");
         this.manifestImageList.Images.SetKeyName(7, "animation.png");
         this.manifestImageList.Images.SetKeyName(8, "encounter.png");
         this.manifestImageList.Images.SetKeyName(9, "job.png");
         // 
         // ManifestView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.splitContainer2);
         this.Name = "ManifestView";
         this.Size = new System.Drawing.Size(681, 472);
         this.splitContainer2.Panel1.ResumeLayout(false);
         this.splitContainer2.Panel2.ResumeLayout(false);
         this.splitContainer2.Panel2.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
         this.splitContainer2.ResumeLayout(false);
         this.searchPanel.ResumeLayout(false);
         this.searchPanel.PerformLayout();
         this.splitContainer3.Panel1.ResumeLayout(false);
         this.splitContainer3.Panel2.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
         this.splitContainer3.ResumeLayout(false);
         this.panel3.ResumeLayout(false);
         this.panel3.PerformLayout();
         this.panel2.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.iconView)).EndInit();
         this.aliasContextMenu.ResumeLayout(false);
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.SplitContainer splitContainer2;
      private System.Windows.Forms.TreeView treeView;
      private System.Windows.Forms.Panel searchPanel;
      private System.Windows.Forms.TextBox searchBox;
      private System.Windows.Forms.Button searchButton;
      private System.Windows.Forms.SplitContainer splitContainer3;
      private System.Windows.Forms.TabControl filePreviewTabs;
      private System.Windows.Forms.FlowLayoutPanel openFileButtonPanel;
      private System.Windows.Forms.Panel panel3;
      private System.Windows.Forms.ListView dependenciesListView;
      private System.Windows.Forms.Label dependenciesLabel;
      private System.Windows.Forms.Panel panel2;
      private System.Windows.Forms.PictureBox iconView;
      private System.Windows.Forms.TextBox selectedFilePathTextBox;
      private System.Windows.Forms.ContextMenuStrip aliasContextMenu;
      private System.Windows.Forms.ToolStripMenuItem aliasContextDuplicate;
      private System.Windows.Forms.ToolStripMenuItem makeFineVersionToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem copyFullAliasToolStripMenuItem;
      private System.Windows.Forms.ImageList manifestImageList;
   }
}
