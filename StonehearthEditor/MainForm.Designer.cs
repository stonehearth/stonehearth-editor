namespace StonehearthEditor
{
   partial class MainForm
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
         this.tabControl = new System.Windows.Forms.TabControl();
         this.manifestTab = new System.Windows.Forms.TabPage();
         this.manifestView = new StonehearthEditor.ManifestView();
         this.encounterTab = new System.Windows.Forms.TabPage();
         this.splitContainer1 = new System.Windows.Forms.SplitContainer();
         this.encounterTabRightSide = new System.Windows.Forms.Panel();
         this.graphViewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
         this.encounterGraphContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.copyGameMasterNode = new System.Windows.Forms.ToolStripMenuItem();
         this.addNewGameMasterNode = new System.Windows.Forms.ToolStripMenuItem();
         this.deleteNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.encounterRightSideFilePath = new System.Windows.Forms.TextBox();
         this.panel1 = new System.Windows.Forms.Panel();
         this.nodeInfoJsonPreview = new System.Windows.Forms.RichTextBox();
         this.nodeInfoPanel = new System.Windows.Forms.Panel();
         this.openEncounterFileButton = new System.Windows.Forms.Button();
         this.nodeInfoSubType = new System.Windows.Forms.Label();
         this.nodeInfoType = new System.Windows.Forms.Label();
         this.nodeInfoName = new System.Windows.Forms.Label();
         this.encounterTreeView = new System.Windows.Forms.TreeView();
         this.toolStrip1 = new System.Windows.Forms.ToolStrip();
         this.toolstripSaveButton = new System.Windows.Forms.ToolStripButton();
         this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.i18nTooltip = new System.Windows.Forms.ToolTip(this.components);
         this.saveNewEncounterNodeDialog = new System.Windows.Forms.SaveFileDialog();
         this.modsFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
         this.mainFormMenu = new System.Windows.Forms.MenuStrip();
         this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.saveAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.changeModDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.tabControl.SuspendLayout();
         this.manifestTab.SuspendLayout();
         this.encounterTab.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
         this.splitContainer1.Panel1.SuspendLayout();
         this.splitContainer1.Panel2.SuspendLayout();
         this.splitContainer1.SuspendLayout();
         this.encounterTabRightSide.SuspendLayout();
         this.encounterGraphContextMenu.SuspendLayout();
         this.panel1.SuspendLayout();
         this.nodeInfoPanel.SuspendLayout();
         this.toolStrip1.SuspendLayout();
         this.mainFormMenu.SuspendLayout();
         this.SuspendLayout();
         // 
         // tabControl
         // 
         this.tabControl.Controls.Add(this.manifestTab);
         this.tabControl.Controls.Add(this.encounterTab);
         this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tabControl.Location = new System.Drawing.Point(0, 24);
         this.tabControl.Name = "tabControl";
         this.tabControl.SelectedIndex = 0;
         this.tabControl.Size = new System.Drawing.Size(1123, 581);
         this.tabControl.TabIndex = 3;
         this.tabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl_Selected);
         // 
         // manifestTab
         // 
         this.manifestTab.Controls.Add(this.manifestView);
         this.manifestTab.Location = new System.Drawing.Point(4, 22);
         this.manifestTab.Name = "manifestTab";
         this.manifestTab.Padding = new System.Windows.Forms.Padding(3);
         this.manifestTab.Size = new System.Drawing.Size(1115, 555);
         this.manifestTab.TabIndex = 0;
         this.manifestTab.Text = "Manifest";
         this.manifestTab.UseVisualStyleBackColor = true;
         // 
         // manifestView
         // 
         this.manifestView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.manifestView.Location = new System.Drawing.Point(3, 3);
         this.manifestView.Name = "manifestView";
         this.manifestView.Size = new System.Drawing.Size(1109, 549);
         this.manifestView.TabIndex = 0;
         // 
         // encounterTab
         // 
         this.encounterTab.Controls.Add(this.splitContainer1);
         this.encounterTab.Controls.Add(this.encounterTreeView);
         this.encounterTab.Controls.Add(this.toolStrip1);
         this.encounterTab.Location = new System.Drawing.Point(4, 22);
         this.encounterTab.Name = "encounterTab";
         this.encounterTab.Padding = new System.Windows.Forms.Padding(3);
         this.encounterTab.Size = new System.Drawing.Size(1115, 555);
         this.encounterTab.TabIndex = 1;
         this.encounterTab.Text = "Encounter Designer";
         this.encounterTab.UseVisualStyleBackColor = true;
         // 
         // splitContainer1
         // 
         this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainer1.Location = new System.Drawing.Point(253, 28);
         this.splitContainer1.Name = "splitContainer1";
         this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
         // 
         // splitContainer1.Panel1
         // 
         this.splitContainer1.Panel1.Controls.Add(this.encounterTabRightSide);
         // 
         // splitContainer1.Panel2
         // 
         this.splitContainer1.Panel2.Controls.Add(this.panel1);
         this.splitContainer1.Panel2.Controls.Add(this.nodeInfoPanel);
         this.splitContainer1.Size = new System.Drawing.Size(859, 524);
         this.splitContainer1.SplitterDistance = 356;
         this.splitContainer1.TabIndex = 3;
         // 
         // encounterTabRightSide
         // 
         this.encounterTabRightSide.Controls.Add(this.graphViewer);
         this.encounterTabRightSide.Controls.Add(this.encounterRightSideFilePath);
         this.encounterTabRightSide.Dock = System.Windows.Forms.DockStyle.Fill;
         this.encounterTabRightSide.Location = new System.Drawing.Point(0, 0);
         this.encounterTabRightSide.Name = "encounterTabRightSide";
         this.encounterTabRightSide.Size = new System.Drawing.Size(859, 356);
         this.encounterTabRightSide.TabIndex = 1;
         // 
         // graphViewer
         // 
         this.graphViewer.ArrowheadLength = 10D;
         this.graphViewer.AsyncLayout = false;
         this.graphViewer.AutoScroll = true;
         this.graphViewer.BackwardEnabled = false;
         this.graphViewer.BuildHitTree = true;
         this.graphViewer.ContextMenuStrip = this.encounterGraphContextMenu;
         this.graphViewer.CurrentLayoutMethod = Microsoft.Msagl.GraphViewerGdi.LayoutMethod.UseSettingsOfTheGraph;
         this.graphViewer.Dock = System.Windows.Forms.DockStyle.Fill;
         this.graphViewer.EdgeInsertButtonVisible = true;
         this.graphViewer.FileName = "";
         this.graphViewer.ForwardEnabled = false;
         this.graphViewer.Graph = null;
         this.graphViewer.InsertingEdge = false;
         this.graphViewer.LayoutAlgorithmSettingsButtonVisible = true;
         this.graphViewer.LayoutEditingEnabled = true;
         this.graphViewer.Location = new System.Drawing.Point(0, 20);
         this.graphViewer.LooseOffsetForRouting = 0.25D;
         this.graphViewer.MouseHitDistance = 0.05D;
         this.graphViewer.Name = "graphViewer";
         this.graphViewer.NavigationVisible = true;
         this.graphViewer.NeedToCalculateLayout = true;
         this.graphViewer.OffsetForRelaxingInRouting = 0.6D;
         this.graphViewer.PaddingForEdgeRouting = 8D;
         this.graphViewer.PanButtonPressed = false;
         this.graphViewer.SaveAsImageEnabled = true;
         this.graphViewer.SaveAsMsaglEnabled = true;
         this.graphViewer.SaveButtonVisible = true;
         this.graphViewer.SaveGraphButtonVisible = true;
         this.graphViewer.SaveInVectorFormatEnabled = true;
         this.graphViewer.Size = new System.Drawing.Size(859, 336);
         this.graphViewer.TabIndex = 0;
         this.graphViewer.TightOffsetForRouting = 0.125D;
         this.graphViewer.ToolBarIsVisible = true;
         this.graphViewer.Transform = ((Microsoft.Msagl.Core.Geometry.Curves.PlaneTransformation)(resources.GetObject("graphViewer.Transform")));
         this.graphViewer.UndoRedoButtonsVisible = true;
         this.graphViewer.WindowZoomButtonPressed = false;
         this.graphViewer.ZoomF = 1D;
         this.graphViewer.ZoomFraction = 0.5D;
         this.graphViewer.ZoomWhenMouseWheelScroll = true;
         this.graphViewer.ZoomWindowThreshold = 0.05D;
         this.graphViewer.EdgeAdded += new System.EventHandler(this.graphViewer_EdgeAdded);
         this.graphViewer.EdgeRemoved += new System.EventHandler(this.graphViewer_EdgeRemoved);
         this.graphViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.graphViewer_MouseMove);
         this.graphViewer.MouseClick += new System.Windows.Forms.MouseEventHandler(this.graphViewer_MouseClick);
         this.graphViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.graphViewer_MouseDown);
         this.graphViewer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.graphViewer_MouseUp);
         // 
         // encounterGraphContextMenu
         // 
         this.encounterGraphContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyGameMasterNode,
            this.addNewGameMasterNode,
            this.deleteNodeToolStripMenuItem});
         this.encounterGraphContextMenu.Name = "encounterGraphContextMenu";
         this.encounterGraphContextMenu.Size = new System.Drawing.Size(156, 70);
         // 
         // copyGameMasterNode
         // 
         this.copyGameMasterNode.Enabled = false;
         this.copyGameMasterNode.Name = "copyGameMasterNode";
         this.copyGameMasterNode.Size = new System.Drawing.Size(155, 22);
         this.copyGameMasterNode.Text = "Clone Node";
         this.copyGameMasterNode.Click += new System.EventHandler(this.copyGameMasterNode_Click);
         // 
         // addNewGameMasterNode
         // 
         this.addNewGameMasterNode.Enabled = false;
         this.addNewGameMasterNode.Name = "addNewGameMasterNode";
         this.addNewGameMasterNode.Size = new System.Drawing.Size(155, 22);
         this.addNewGameMasterNode.Text = "Add New Node";
         this.addNewGameMasterNode.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.addNewGameMasterNode_DropDownItemClicked);
         // 
         // deleteNodeToolStripMenuItem
         // 
         this.deleteNodeToolStripMenuItem.Name = "deleteNodeToolStripMenuItem";
         this.deleteNodeToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
         this.deleteNodeToolStripMenuItem.Text = "Delete";
         this.deleteNodeToolStripMenuItem.Click += new System.EventHandler(this.deleteNodeToolStripMenuItem_Click);
         // 
         // encounterRightSideFilePath
         // 
         this.encounterRightSideFilePath.Dock = System.Windows.Forms.DockStyle.Top;
         this.encounterRightSideFilePath.Location = new System.Drawing.Point(0, 0);
         this.encounterRightSideFilePath.MaximumSize = new System.Drawing.Size(0, 20);
         this.encounterRightSideFilePath.MinimumSize = new System.Drawing.Size(0, 20);
         this.encounterRightSideFilePath.Name = "encounterRightSideFilePath";
         this.encounterRightSideFilePath.ReadOnly = true;
         this.encounterRightSideFilePath.Size = new System.Drawing.Size(859, 20);
         this.encounterRightSideFilePath.TabIndex = 2;
         // 
         // panel1
         // 
         this.panel1.Controls.Add(this.nodeInfoJsonPreview);
         this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.panel1.Location = new System.Drawing.Point(0, 30);
         this.panel1.Name = "panel1";
         this.panel1.Size = new System.Drawing.Size(859, 134);
         this.panel1.TabIndex = 5;
         // 
         // nodeInfoJsonPreview
         // 
         this.nodeInfoJsonPreview.Dock = System.Windows.Forms.DockStyle.Fill;
         this.nodeInfoJsonPreview.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.nodeInfoJsonPreview.Location = new System.Drawing.Point(0, 0);
         this.nodeInfoJsonPreview.Margin = new System.Windows.Forms.Padding(10);
         this.nodeInfoJsonPreview.Name = "nodeInfoJsonPreview";
         this.nodeInfoJsonPreview.Size = new System.Drawing.Size(859, 134);
         this.nodeInfoJsonPreview.TabIndex = 4;
         this.nodeInfoJsonPreview.Text = "";
         this.nodeInfoJsonPreview.WordWrap = false;
         this.nodeInfoJsonPreview.Leave += new System.EventHandler(this.nodeInfoJsonPreview_Leave);
         this.nodeInfoJsonPreview.MouseMove += new System.Windows.Forms.MouseEventHandler(this.nodeInfoJsonPreview_MouseMove);
         // 
         // nodeInfoPanel
         // 
         this.nodeInfoPanel.BackColor = System.Drawing.Color.Silver;
         this.nodeInfoPanel.Controls.Add(this.openEncounterFileButton);
         this.nodeInfoPanel.Controls.Add(this.nodeInfoSubType);
         this.nodeInfoPanel.Controls.Add(this.nodeInfoType);
         this.nodeInfoPanel.Controls.Add(this.nodeInfoName);
         this.nodeInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
         this.nodeInfoPanel.Location = new System.Drawing.Point(0, 0);
         this.nodeInfoPanel.Name = "nodeInfoPanel";
         this.nodeInfoPanel.Size = new System.Drawing.Size(859, 30);
         this.nodeInfoPanel.TabIndex = 1;
         // 
         // openEncounterFileButton
         // 
         this.openEncounterFileButton.Location = new System.Drawing.Point(606, 2);
         this.openEncounterFileButton.Name = "openEncounterFileButton";
         this.openEncounterFileButton.Size = new System.Drawing.Size(75, 23);
         this.openEncounterFileButton.TabIndex = 4;
         this.openEncounterFileButton.Text = "Open File";
         this.openEncounterFileButton.UseVisualStyleBackColor = true;
         this.openEncounterFileButton.Click += new System.EventHandler(this.openEncounterFileButton_Click);
         // 
         // nodeInfoSubType
         // 
         this.nodeInfoSubType.AutoSize = true;
         this.nodeInfoSubType.Location = new System.Drawing.Point(330, 13);
         this.nodeInfoSubType.Name = "nodeInfoSubType";
         this.nodeInfoSubType.Size = new System.Drawing.Size(92, 13);
         this.nodeInfoSubType.TabIndex = 3;
         this.nodeInfoSubType.Text = "nodeInfoSubType";
         this.nodeInfoSubType.Click += new System.EventHandler(this.nodeInfoSubType_Click);
         // 
         // nodeInfoType
         // 
         this.nodeInfoType.AutoSize = true;
         this.nodeInfoType.Location = new System.Drawing.Point(237, 13);
         this.nodeInfoType.Name = "nodeInfoType";
         this.nodeInfoType.Size = new System.Drawing.Size(73, 13);
         this.nodeInfoType.TabIndex = 2;
         this.nodeInfoType.Text = "nodeInfoType";
         // 
         // nodeInfoName
         // 
         this.nodeInfoName.AutoSize = true;
         this.nodeInfoName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.nodeInfoName.Location = new System.Drawing.Point(14, 9);
         this.nodeInfoName.Name = "nodeInfoName";
         this.nodeInfoName.Size = new System.Drawing.Size(97, 17);
         this.nodeInfoName.TabIndex = 0;
         this.nodeInfoName.Text = "Select a Node";
         // 
         // encounterTreeView
         // 
         this.encounterTreeView.Dock = System.Windows.Forms.DockStyle.Left;
         this.encounterTreeView.FullRowSelect = true;
         this.encounterTreeView.HideSelection = false;
         this.encounterTreeView.Location = new System.Drawing.Point(3, 28);
         this.encounterTreeView.Name = "encounterTreeView";
         this.encounterTreeView.PathSeparator = "/";
         this.encounterTreeView.Size = new System.Drawing.Size(250, 524);
         this.encounterTreeView.TabIndex = 0;
         this.encounterTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.encounterTreeView_AfterSelect);
         // 
         // toolStrip1
         // 
         this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripSaveButton});
         this.toolStrip1.Location = new System.Drawing.Point(3, 3);
         this.toolStrip1.Name = "toolStrip1";
         this.toolStrip1.Size = new System.Drawing.Size(1109, 25);
         this.toolStrip1.TabIndex = 4;
         this.toolStrip1.Text = "toolStrip1";
         // 
         // toolstripSaveButton
         // 
         this.toolstripSaveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.toolstripSaveButton.Image = ((System.Drawing.Image)(resources.GetObject("toolstripSaveButton.Image")));
         this.toolstripSaveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.toolstripSaveButton.Name = "toolstripSaveButton";
         this.toolstripSaveButton.Size = new System.Drawing.Size(23, 22);
         this.toolstripSaveButton.Text = "Save modified files";
         this.toolstripSaveButton.Click += new System.EventHandler(this.toolstripSaveButton_Click);
         // 
         // saveToolStripMenuItem
         // 
         this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
         this.saveToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
         // 
         // saveNewEncounterNodeDialog
         // 
         this.saveNewEncounterNodeDialog.DefaultExt = "json";
         this.saveNewEncounterNodeDialog.Filter = "Json Files|*.json";
         this.saveNewEncounterNodeDialog.RestoreDirectory = true;
         this.saveNewEncounterNodeDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveNewEncounterNodeDialog_FileOk);
         // 
         // modsFolderBrowserDialog
         // 
         this.modsFolderBrowserDialog.Description = "Stonehearth Mods Root Directory";
         this.modsFolderBrowserDialog.ShowNewFolderButton = false;
         // 
         // mainFormMenu
         // 
         this.mainFormMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
         this.mainFormMenu.Location = new System.Drawing.Point(0, 0);
         this.mainFormMenu.Name = "mainFormMenu";
         this.mainFormMenu.Size = new System.Drawing.Size(1123, 24);
         this.mainFormMenu.TabIndex = 2;
         this.mainFormMenu.Text = "menuStrip1";
         // 
         // fileToolStripMenuItem
         // 
         this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveAllToolStripMenuItem,
            this.reloadToolStripMenuItem,
            this.changeModDirectoryToolStripMenuItem});
         this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
         this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
         this.fileToolStripMenuItem.Text = "File";
         // 
         // saveAllToolStripMenuItem
         // 
         this.saveAllToolStripMenuItem.Name = "saveAllToolStripMenuItem";
         this.saveAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
         this.saveAllToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
         this.saveAllToolStripMenuItem.Text = "Save All";
         this.saveAllToolStripMenuItem.Click += new System.EventHandler(this.saveAllToolStripMenuItem_Click);
         // 
         // reloadToolStripMenuItem
         // 
         this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
         this.reloadToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
         this.reloadToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
         this.reloadToolStripMenuItem.Text = "Reload";
         this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
         // 
         // changeModDirectoryToolStripMenuItem
         // 
         this.changeModDirectoryToolStripMenuItem.Name = "changeModDirectoryToolStripMenuItem";
         this.changeModDirectoryToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
         this.changeModDirectoryToolStripMenuItem.Text = "Change Mod Directory";
         // 
         // StonehearthEditor
         // 
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
         this.ClientSize = new System.Drawing.Size(1123, 605);
         this.Controls.Add(this.tabControl);
         this.Controls.Add(this.mainFormMenu);
         this.KeyPreview = true;
         this.MainMenuStrip = this.mainFormMenu;
         this.Name = "StonehearthEditor";
         this.Text = "Stonehearth Editor";
         this.Load += new System.EventHandler(this.Form1_Load);
         this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.StonehearthEditor_KeyDown);
         this.tabControl.ResumeLayout(false);
         this.manifestTab.ResumeLayout(false);
         this.encounterTab.ResumeLayout(false);
         this.encounterTab.PerformLayout();
         this.splitContainer1.Panel1.ResumeLayout(false);
         this.splitContainer1.Panel2.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
         this.splitContainer1.ResumeLayout(false);
         this.encounterTabRightSide.ResumeLayout(false);
         this.encounterTabRightSide.PerformLayout();
         this.encounterGraphContextMenu.ResumeLayout(false);
         this.panel1.ResumeLayout(false);
         this.nodeInfoPanel.ResumeLayout(false);
         this.nodeInfoPanel.PerformLayout();
         this.toolStrip1.ResumeLayout(false);
         this.toolStrip1.PerformLayout();
         this.mainFormMenu.ResumeLayout(false);
         this.mainFormMenu.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion
      private System.Windows.Forms.TabControl tabControl;
      private System.Windows.Forms.TabPage encounterTab;
      private System.Windows.Forms.TreeView encounterTreeView;
      private System.Windows.Forms.Panel encounterTabRightSide;
      private Microsoft.Msagl.GraphViewerGdi.GViewer graphViewer;
      private System.Windows.Forms.Panel nodeInfoPanel;
      private System.Windows.Forms.Label nodeInfoName;
      private System.Windows.Forms.Label nodeInfoSubType;
      private System.Windows.Forms.Label nodeInfoType;
      private System.Windows.Forms.TextBox encounterRightSideFilePath;
      private System.Windows.Forms.RichTextBox nodeInfoJsonPreview;
      private System.Windows.Forms.ContextMenuStrip encounterGraphContextMenu;
      private System.Windows.Forms.ToolStripMenuItem copyGameMasterNode;
      private System.Windows.Forms.ToolStripMenuItem addNewGameMasterNode;
      private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
      private System.Windows.Forms.SplitContainer splitContainer1;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.ToolStrip toolStrip1;
      private System.Windows.Forms.ToolStripButton toolstripSaveButton;
      private System.Windows.Forms.ToolTip i18nTooltip;
      private System.Windows.Forms.SaveFileDialog saveNewEncounterNodeDialog;
      private System.Windows.Forms.FolderBrowserDialog modsFolderBrowserDialog;
      private System.Windows.Forms.Button openEncounterFileButton;
      private System.Windows.Forms.ToolStripMenuItem deleteNodeToolStripMenuItem;
      private System.Windows.Forms.MenuStrip mainFormMenu;
      private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem saveAllToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem changeModDirectoryToolStripMenuItem;
      private System.Windows.Forms.TabPage manifestTab;
      private ManifestView manifestView;
   }
}

