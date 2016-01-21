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
         this.aliasContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.aliasContextDuplicate = new System.Windows.Forms.ToolStripMenuItem();
         this.addIconicVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.addGhostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.makeFineVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.copyFullAliasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.removeFromManifestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.addNewAliasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.manifestImageList = new System.Windows.Forms.ImageList(this.components);
         this.splitContainer2 = new System.Windows.Forms.SplitContainer();
         this.treeView = new System.Windows.Forms.TreeView();
         this.searchPanel = new System.Windows.Forms.Panel();
         this.searchBox = new System.Windows.Forms.TextBox();
         this.searchButton = new System.Windows.Forms.Button();
         this.splitContainer3 = new System.Windows.Forms.SplitContainer();
         this.filePreviewTabs = new System.Windows.Forms.TabControl();
         this.openFileButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
         this.referencesPanel = new System.Windows.Forms.Panel();
         this.referencesListBox = new System.Windows.Forms.ListBox();
         this.referencesListLabel = new System.Windows.Forms.Label();
         this.dependenciesPanel = new System.Windows.Forms.Panel();
         this.dependenciesListBox = new System.Windows.Forms.ListBox();
         this.dependenciesLabel = new System.Windows.Forms.Label();
         this.panel2 = new System.Windows.Forms.Panel();
         this.iconView = new System.Windows.Forms.PictureBox();
         this.selectedFilePathTextBox = new System.Windows.Forms.TextBox();
         this.selectJsonFileDialog = new System.Windows.Forms.OpenFileDialog();
         this.statusStrip = new System.Windows.Forms.StatusStrip();
         this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
         this.aliasContextMenu.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
         this.splitContainer2.Panel1.SuspendLayout();
         this.splitContainer2.Panel2.SuspendLayout();
         this.splitContainer2.SuspendLayout();
         this.searchPanel.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
         this.splitContainer3.Panel1.SuspendLayout();
         this.splitContainer3.Panel2.SuspendLayout();
         this.splitContainer3.SuspendLayout();
         this.referencesPanel.SuspendLayout();
         this.dependenciesPanel.SuspendLayout();
         this.panel2.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.iconView)).BeginInit();
         this.statusStrip.SuspendLayout();
         this.SuspendLayout();
         // 
         // aliasContextMenu
         // 
         this.aliasContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aliasContextDuplicate,
            this.addIconicVersionToolStripMenuItem,
            this.addGhostToolStripMenuItem,
            this.makeFineVersionToolStripMenuItem,
            this.copyFullAliasToolStripMenuItem,
            this.removeFromManifestToolStripMenuItem,
            this.addNewAliasToolStripMenuItem});
         this.aliasContextMenu.Name = "aliasContextMenu";
         this.aliasContextMenu.Size = new System.Drawing.Size(198, 158);
         this.aliasContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.aliasContextMenu_Opening);
         // 
         // aliasContextDuplicate
         // 
         this.aliasContextDuplicate.Name = "aliasContextDuplicate";
         this.aliasContextDuplicate.Size = new System.Drawing.Size(197, 22);
         this.aliasContextDuplicate.Text = "Clone";
         this.aliasContextDuplicate.Click += new System.EventHandler(this.aliasContextMenuDuplicate_Click);
         // 
         // addIconicVersionToolStripMenuItem
         // 
         this.addIconicVersionToolStripMenuItem.Name = "addIconicVersionToolStripMenuItem";
         this.addIconicVersionToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
         this.addIconicVersionToolStripMenuItem.Text = "Add Iconic";
         this.addIconicVersionToolStripMenuItem.Click += new System.EventHandler(this.addIconicVersionToolStripMenuItem_Click);
         // 
         // addGhostToolStripMenuItem
         // 
         this.addGhostToolStripMenuItem.Name = "addGhostToolStripMenuItem";
         this.addGhostToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
         this.addGhostToolStripMenuItem.Text = "Add Ghost";
         this.addGhostToolStripMenuItem.Click += new System.EventHandler(this.addGhostToolStripMenuItem_Click);
         // 
         // makeFineVersionToolStripMenuItem
         // 
         this.makeFineVersionToolStripMenuItem.Name = "makeFineVersionToolStripMenuItem";
         this.makeFineVersionToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
         this.makeFineVersionToolStripMenuItem.Text = "Make Fine Version";
         this.makeFineVersionToolStripMenuItem.Click += new System.EventHandler(this.makeFineVersionToolStripMenuItem_Click);
         // 
         // copyFullAliasToolStripMenuItem
         // 
         this.copyFullAliasToolStripMenuItem.Name = "copyFullAliasToolStripMenuItem";
         this.copyFullAliasToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
         this.copyFullAliasToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
         this.copyFullAliasToolStripMenuItem.Text = "Copy Full Alias";
         this.copyFullAliasToolStripMenuItem.Click += new System.EventHandler(this.copyFullAliasToolStripMenuItem_Click);
         // 
         // removeFromManifestToolStripMenuItem
         // 
         this.removeFromManifestToolStripMenuItem.Name = "removeFromManifestToolStripMenuItem";
         this.removeFromManifestToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
         this.removeFromManifestToolStripMenuItem.Text = "Remove From Manifest";
         this.removeFromManifestToolStripMenuItem.Click += new System.EventHandler(this.removeFromManifestToolStripMenuItem_Click);
         // 
         // addNewAliasToolStripMenuItem
         // 
         this.addNewAliasToolStripMenuItem.Name = "addNewAliasToolStripMenuItem";
         this.addNewAliasToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
         this.addNewAliasToolStripMenuItem.Text = "Add New Alias";
         this.addNewAliasToolStripMenuItem.Click += new System.EventHandler(this.addNewAliasToolStripMenuItem_Click);
         // 
         // manifestImageList
         // 
         this.manifestImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("manifestImageList.ImageStream")));
         this.manifestImageList.TransparentColor = System.Drawing.Color.Transparent;
         this.manifestImageList.Images.SetKeyName(0, "error.png");
         this.manifestImageList.Images.SetKeyName(1, "none.png");
         this.manifestImageList.Images.SetKeyName(2, "entity.png");
         this.manifestImageList.Images.SetKeyName(3, "buff.png");
         this.manifestImageList.Images.SetKeyName(4, "ai_pack.png");
         this.manifestImageList.Images.SetKeyName(5, "effect.png");
         this.manifestImageList.Images.SetKeyName(6, "recipe.png");
         this.manifestImageList.Images.SetKeyName(7, "command.png");
         this.manifestImageList.Images.SetKeyName(8, "animation.png");
         this.manifestImageList.Images.SetKeyName(9, "encounter.png");
         this.manifestImageList.Images.SetKeyName(10, "job.png");
         // 
         // splitContainer2
         // 
         this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
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
         this.splitContainer2.Size = new System.Drawing.Size(762, 547);
         this.splitContainer2.SplitterDistance = 200;
         this.splitContainer2.TabIndex = 3;
         this.splitContainer2.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer2_SplitterMoved);
         // 
         // treeView
         // 
         this.treeView.ContextMenuStrip = this.aliasContextMenu;
         this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.treeView.HideSelection = false;
         this.treeView.ImageIndex = 0;
         this.treeView.ImageList = this.manifestImageList;
         this.treeView.LabelEdit = true;
         this.treeView.Location = new System.Drawing.Point(0, 26);
         this.treeView.Name = "treeView";
         this.treeView.SelectedImageIndex = 1;
         this.treeView.ShowNodeToolTips = true;
         this.treeView.Size = new System.Drawing.Size(200, 521);
         this.treeView.TabIndex = 1;
         this.treeView.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView_BeforeLabelEdit);
         this.treeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView_AfterLabelEdit);
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
         this.searchPanel.Size = new System.Drawing.Size(200, 26);
         this.searchPanel.TabIndex = 5;
         // 
         // searchBox
         // 
         this.searchBox.Dock = System.Windows.Forms.DockStyle.Fill;
         this.searchBox.Location = new System.Drawing.Point(0, 0);
         this.searchBox.Name = "searchBox";
         this.searchBox.Size = new System.Drawing.Size(177, 20);
         this.searchBox.TabIndex = 3;
         this.searchBox.WordWrap = false;
         this.searchBox.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
         this.searchBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.searchBox_KeyPress);
         // 
         // searchButton
         // 
         this.searchButton.Dock = System.Windows.Forms.DockStyle.Right;
         this.searchButton.Image = ((System.Drawing.Image)(resources.GetObject("searchButton.Image")));
         this.searchButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
         this.searchButton.Location = new System.Drawing.Point(177, 0);
         this.searchButton.Margin = new System.Windows.Forms.Padding(0);
         this.searchButton.Name = "searchButton";
         this.searchButton.Size = new System.Drawing.Size(23, 26);
         this.searchButton.TabIndex = 4;
         this.searchButton.UseVisualStyleBackColor = true;
         this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
         // 
         // splitContainer3
         // 
         this.splitContainer3.DataBindings.Add(new System.Windows.Forms.Binding("SplitterDistance", global::StonehearthEditor.Properties.Settings.Default, "ManifestViewFileDependenciesSplitter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
         this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
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
         this.splitContainer3.Panel2.Controls.Add(this.referencesPanel);
         this.splitContainer3.Panel2.Controls.Add(this.dependenciesPanel);
         this.splitContainer3.Panel2.Controls.Add(this.panel2);
         this.splitContainer3.Size = new System.Drawing.Size(558, 527);
         this.splitContainer3.SplitterDistance = global::StonehearthEditor.Properties.Settings.Default.ManifestViewFileDependenciesSplitter;
         this.splitContainer3.TabIndex = 3;
         this.splitContainer3.TabStop = false;
         // 
         // filePreviewTabs
         // 
         this.filePreviewTabs.Dock = System.Windows.Forms.DockStyle.Fill;
         this.filePreviewTabs.ImageList = this.manifestImageList;
         this.filePreviewTabs.Location = new System.Drawing.Point(0, 33);
         this.filePreviewTabs.Name = "filePreviewTabs";
         this.filePreviewTabs.SelectedIndex = 0;
         this.filePreviewTabs.ShowToolTips = true;
         this.filePreviewTabs.Size = new System.Drawing.Size(318, 494);
         this.filePreviewTabs.TabIndex = 2;
         // 
         // openFileButtonPanel
         // 
         this.openFileButtonPanel.Dock = System.Windows.Forms.DockStyle.Top;
         this.openFileButtonPanel.Location = new System.Drawing.Point(0, 0);
         this.openFileButtonPanel.Name = "openFileButtonPanel";
         this.openFileButtonPanel.Size = new System.Drawing.Size(318, 33);
         this.openFileButtonPanel.TabIndex = 0;
         // 
         // referencesPanel
         // 
         this.referencesPanel.BackColor = System.Drawing.Color.Transparent;
         this.referencesPanel.Controls.Add(this.referencesListBox);
         this.referencesPanel.Controls.Add(this.referencesListLabel);
         this.referencesPanel.Dock = System.Windows.Forms.DockStyle.Top;
         this.referencesPanel.Location = new System.Drawing.Point(0, 355);
         this.referencesPanel.Name = "referencesPanel";
         this.referencesPanel.Size = new System.Drawing.Size(236, 130);
         this.referencesPanel.TabIndex = 6;
         // 
         // referencesListBox
         // 
         this.referencesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
         this.referencesListBox.FormattingEnabled = true;
         this.referencesListBox.HorizontalScrollbar = true;
         this.referencesListBox.Location = new System.Drawing.Point(0, 13);
         this.referencesListBox.MaximumSize = new System.Drawing.Size(500, 100);
         this.referencesListBox.MinimumSize = new System.Drawing.Size(200, 100);
         this.referencesListBox.Name = "referencesListBox";
         this.referencesListBox.Size = new System.Drawing.Size(236, 100);
         this.referencesListBox.TabIndex = 4;
         this.referencesListBox.TabStop = false;
         this.referencesListBox.SelectedIndexChanged += new System.EventHandler(this.dependenciesListView_SelectedIndexChanged);
         this.referencesListBox.DoubleClick += new System.EventHandler(this.dependenciesListBox_DoubleClick);
         // 
         // referencesListLabel
         // 
         this.referencesListLabel.AutoSize = true;
         this.referencesListLabel.Dock = System.Windows.Forms.DockStyle.Top;
         this.referencesListLabel.Location = new System.Drawing.Point(0, 0);
         this.referencesListLabel.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
         this.referencesListLabel.Name = "referencesListLabel";
         this.referencesListLabel.Size = new System.Drawing.Size(81, 13);
         this.referencesListLabel.TabIndex = 3;
         this.referencesListLabel.Text = "Referenced By:";
         // 
         // dependenciesPanel
         // 
         this.dependenciesPanel.BackColor = System.Drawing.Color.Transparent;
         this.dependenciesPanel.Controls.Add(this.dependenciesListBox);
         this.dependenciesPanel.Controls.Add(this.dependenciesLabel);
         this.dependenciesPanel.Dock = System.Windows.Forms.DockStyle.Top;
         this.dependenciesPanel.Location = new System.Drawing.Point(0, 232);
         this.dependenciesPanel.Name = "dependenciesPanel";
         this.dependenciesPanel.Size = new System.Drawing.Size(236, 123);
         this.dependenciesPanel.TabIndex = 5;
         // 
         // dependenciesListBox
         // 
         this.dependenciesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
         this.dependenciesListBox.FormattingEnabled = true;
         this.dependenciesListBox.HorizontalScrollbar = true;
         this.dependenciesListBox.Location = new System.Drawing.Point(0, 13);
         this.dependenciesListBox.MaximumSize = new System.Drawing.Size(500, 100);
         this.dependenciesListBox.MinimumSize = new System.Drawing.Size(200, 100);
         this.dependenciesListBox.Name = "dependenciesListBox";
         this.dependenciesListBox.Size = new System.Drawing.Size(236, 100);
         this.dependenciesListBox.TabIndex = 4;
         this.dependenciesListBox.TabStop = false;
         this.dependenciesListBox.SelectedValueChanged += new System.EventHandler(this.dependenciesListView_SelectedIndexChanged);
         this.dependenciesListBox.DoubleClick += new System.EventHandler(this.dependenciesListBox_DoubleClick);
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
         this.panel2.Size = new System.Drawing.Size(236, 232);
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
         this.selectedFilePathTextBox.Size = new System.Drawing.Size(558, 20);
         this.selectedFilePathTextBox.TabIndex = 4;
         // 
         // selectJsonFileDialog
         // 
         this.selectJsonFileDialog.DefaultExt = "json";
         this.selectJsonFileDialog.Filter = "JSON Files|*.json|Lua Files|*.lua";
         this.selectJsonFileDialog.RestoreDirectory = true;
         this.selectJsonFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.selectJsonFileDialog_FileOk);
         // 
         // statusStrip
         // 
         this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
         this.statusStrip.Location = new System.Drawing.Point(0, 547);
         this.statusStrip.Name = "statusStrip";
         this.statusStrip.Size = new System.Drawing.Size(762, 22);
         this.statusStrip.TabIndex = 4;
         this.statusStrip.Text = "statusStrip1";
         this.statusStrip.DoubleClick += new System.EventHandler(this.statusStrip_DoubleClick);
         // 
         // toolStripStatusLabel1
         // 
         this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
         this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
         this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
         this.toolStripStatusLabel1.DoubleClick += new System.EventHandler(this.toolStripStatusLabel1_DoubleClick);
         // 
         // ManifestView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.splitContainer2);
         this.Controls.Add(this.statusStrip);
         this.Name = "ManifestView";
         this.Size = new System.Drawing.Size(762, 569);
         this.Load += new System.EventHandler(this.ManifestView_Load);
         this.aliasContextMenu.ResumeLayout(false);
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
         this.referencesPanel.ResumeLayout(false);
         this.referencesPanel.PerformLayout();
         this.dependenciesPanel.ResumeLayout(false);
         this.dependenciesPanel.PerformLayout();
         this.panel2.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.iconView)).EndInit();
         this.statusStrip.ResumeLayout(false);
         this.statusStrip.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

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
      private System.Windows.Forms.Panel dependenciesPanel;
      private System.Windows.Forms.Label dependenciesLabel;
      private System.Windows.Forms.Panel panel2;
      private System.Windows.Forms.PictureBox iconView;
      private System.Windows.Forms.TextBox selectedFilePathTextBox;
      private System.Windows.Forms.ContextMenuStrip aliasContextMenu;
      private System.Windows.Forms.ToolStripMenuItem aliasContextDuplicate;
      private System.Windows.Forms.ToolStripMenuItem makeFineVersionToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem copyFullAliasToolStripMenuItem;
      private System.Windows.Forms.ImageList manifestImageList;
      private System.Windows.Forms.ToolStripMenuItem addIconicVersionToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem addGhostToolStripMenuItem;
      private System.Windows.Forms.ListBox dependenciesListBox;
      private System.Windows.Forms.Panel referencesPanel;
      private System.Windows.Forms.ListBox referencesListBox;
      private System.Windows.Forms.Label referencesListLabel;
      private System.Windows.Forms.ToolStripMenuItem removeFromManifestToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem addNewAliasToolStripMenuItem;
      private System.Windows.Forms.OpenFileDialog selectJsonFileDialog;
      private System.Windows.Forms.StatusStrip statusStrip;
      private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
   }
}
