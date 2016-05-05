namespace StonehearthEditor
{
   partial class EncounterDesignerView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EncounterDesignerView));
            this.encounterGraphContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyGameMasterNode = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewGameMasterNode = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveNewEncounterNodeDialog = new System.Windows.Forms.SaveFileDialog();
            this.i18nTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.encounterTabRightSide = new System.Windows.Forms.Panel();
            this.graphViewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            this.encounterRightSideFilePath = new System.Windows.Forms.TextBox();
            this.nodeInfoPanel = new System.Windows.Forms.Panel();
            this.openEncounterFileButton = new System.Windows.Forms.Button();
            this.nodeInfoSubType = new System.Windows.Forms.Label();
            this.nodePath = new System.Windows.Forms.Label();
            this.nodeInfoType = new System.Windows.Forms.Label();
            this.nodeInfoName = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolstripSaveButton = new System.Windows.Forms.ToolStripButton();
            this.encounterTreeView = new System.Windows.Forms.TreeView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.fileDetailStats = new System.Windows.Forms.Panel();
            this.fileDetailsListBox = new System.Windows.Forms.ListBox();
            this.encounterGraphContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.encounterTabRightSide.SuspendLayout();
            this.nodeInfoPanel.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.fileDetailStats.SuspendLayout();
            this.SuspendLayout();
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
            // saveNewEncounterNodeDialog
            // 
            this.saveNewEncounterNodeDialog.DefaultExt = "json";
            this.saveNewEncounterNodeDialog.Filter = "Json Files|*.json";
            this.saveNewEncounterNodeDialog.RestoreDirectory = true;
            this.saveNewEncounterNodeDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveNewEncounterNodeDialog_FileOk);
            // 
            // splitContainer1
            // 
            this.splitContainer1.DataBindings.Add(new System.Windows.Forms.Binding("SplitterDistance", global::StonehearthEditor.Properties.Settings.Default, "EncounterDesignerViewTreeSplitterDistance", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(250, 25);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.encounterTabRightSide);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.nodeInfoPanel);
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(875, 473);
            this.splitContainer1.SplitterDistance = global::StonehearthEditor.Properties.Settings.Default.EncounterDesignerViewTreeSplitterDistance;
            this.splitContainer1.TabIndex = 4;
            // 
            // encounterTabRightSide
            // 
            this.encounterTabRightSide.Controls.Add(this.graphViewer);
            this.encounterTabRightSide.Controls.Add(this.encounterRightSideFilePath);
            this.encounterTabRightSide.Dock = System.Windows.Forms.DockStyle.Fill;
            this.encounterTabRightSide.Location = new System.Drawing.Point(0, 0);
            this.encounterTabRightSide.Name = "encounterTabRightSide";
            this.encounterTabRightSide.Size = new System.Drawing.Size(875, 321);
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
            this.graphViewer.Size = new System.Drawing.Size(875, 301);
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
            this.graphViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.graphViewer_MouseDown);
            this.graphViewer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.graphViewer_MouseUp);
            // 
            // encounterRightSideFilePath
            // 
            this.encounterRightSideFilePath.Dock = System.Windows.Forms.DockStyle.Top;
            this.encounterRightSideFilePath.Location = new System.Drawing.Point(0, 0);
            this.encounterRightSideFilePath.MaximumSize = new System.Drawing.Size(4, 20);
            this.encounterRightSideFilePath.MinimumSize = new System.Drawing.Size(4, 20);
            this.encounterRightSideFilePath.Name = "encounterRightSideFilePath";
            this.encounterRightSideFilePath.ReadOnly = true;
            this.encounterRightSideFilePath.Size = new System.Drawing.Size(4, 20);
            this.encounterRightSideFilePath.TabIndex = 2;
            // 
            // nodeInfoPanel
            // 
            this.nodeInfoPanel.BackColor = System.Drawing.Color.Silver;
            this.nodeInfoPanel.Controls.Add(this.openEncounterFileButton);
            this.nodeInfoPanel.Controls.Add(this.nodeInfoSubType);
            this.nodeInfoPanel.Controls.Add(this.nodePath);
            this.nodeInfoPanel.Controls.Add(this.nodeInfoType);
            this.nodeInfoPanel.Controls.Add(this.nodeInfoName);
            this.nodeInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.nodeInfoPanel.Location = new System.Drawing.Point(0, 0);
            this.nodeInfoPanel.Name = "nodeInfoPanel";
            this.nodeInfoPanel.Size = new System.Drawing.Size(875, 30);
            this.nodeInfoPanel.TabIndex = 1;
            // 
            // openEncounterFileButton
            // 
            this.openEncounterFileButton.Location = new System.Drawing.Point(477, 6);
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
            // 
            // nodePath
            // 
            this.nodePath.AutoSize = true;
            this.nodePath.Location = new System.Drawing.Point(581, 11);
            this.nodePath.Name = "nodePath";
            this.nodePath.Size = new System.Drawing.Size(50, 13);
            this.nodePath.TabIndex = 5;
            this.nodePath.Text = "path.json";
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
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripSaveButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1125, 25);
            this.toolStrip1.TabIndex = 5;
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
            // encounterTreeView
            // 
            this.encounterTreeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.encounterTreeView.FullRowSelect = true;
            this.encounterTreeView.HideSelection = false;
            this.encounterTreeView.Location = new System.Drawing.Point(0, 25);
            this.encounterTreeView.Name = "encounterTreeView";
            this.encounterTreeView.PathSeparator = "/";
            this.encounterTreeView.Size = new System.Drawing.Size(250, 473);
            this.encounterTreeView.TabIndex = 6;
            this.encounterTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.encounterTreeView_AfterSelect);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.AutoScroll = true;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.fileDetailStats);
            this.splitContainer2.Size = new System.Drawing.Size(875, 148);
            this.splitContainer2.SplitterDistance = 646;
            this.splitContainer2.TabIndex = 0;
            // 
            // fileDetailStats
            // 
            this.fileDetailStats.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fileDetailStats.Controls.Add(this.fileDetailsListBox);
            this.fileDetailStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileDetailStats.Location = new System.Drawing.Point(0, 0);
            this.fileDetailStats.Name = "fileDetailStats";
            this.fileDetailStats.Size = new System.Drawing.Size(225, 148);
            this.fileDetailStats.TabIndex = 1;
            // 
            // fileDetailsListBox
            // 
            this.fileDetailsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileDetailsListBox.FormattingEnabled = true;
            this.fileDetailsListBox.Location = new System.Drawing.Point(0, 0);
            this.fileDetailsListBox.Name = "fileDetailsListBox";
            this.fileDetailsListBox.Size = new System.Drawing.Size(223, 146);
            this.fileDetailsListBox.TabIndex = 0;
            // 
            // EncounterDesignerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.encounterTreeView);
            this.Controls.Add(this.toolStrip1);
            this.Name = "EncounterDesignerView";
            this.Size = new System.Drawing.Size(1125, 498);
            this.encounterGraphContextMenu.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.encounterTabRightSide.ResumeLayout(false);
            this.encounterTabRightSide.PerformLayout();
            this.nodeInfoPanel.ResumeLayout(false);
            this.nodeInfoPanel.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.fileDetailStats.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.ContextMenuStrip encounterGraphContextMenu;
      private System.Windows.Forms.ToolStripMenuItem copyGameMasterNode;
      private System.Windows.Forms.ToolStripMenuItem addNewGameMasterNode;
      private System.Windows.Forms.ToolStripMenuItem deleteNodeToolStripMenuItem;
      private System.Windows.Forms.SaveFileDialog saveNewEncounterNodeDialog;
      private System.Windows.Forms.ToolTip i18nTooltip;
      private System.Windows.Forms.SplitContainer splitContainer1;
      private System.Windows.Forms.Panel encounterTabRightSide;
      private Microsoft.Msagl.GraphViewerGdi.GViewer graphViewer;
      private System.Windows.Forms.TextBox encounterRightSideFilePath;
      private System.Windows.Forms.Panel nodeInfoPanel;
      private System.Windows.Forms.Button openEncounterFileButton;
      private System.Windows.Forms.Label nodeInfoSubType;
      private System.Windows.Forms.Label nodeInfoType;
      private System.Windows.Forms.Label nodeInfoName;
      private System.Windows.Forms.ToolStrip toolStrip1;
      private System.Windows.Forms.ToolStripButton toolstripSaveButton;
      private System.Windows.Forms.TreeView encounterTreeView;
      private System.Windows.Forms.Label nodePath;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel fileDetailStats;
        private System.Windows.Forms.ListBox fileDetailsListBox;
    }
}
