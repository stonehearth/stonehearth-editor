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
            this.moveToArcMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveNewEncounterNodeDialog = new System.Windows.Forms.SaveFileDialog();
            this.i18nTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.encounterTreeView = new System.Windows.Forms.TreeView();
            this.graphEditorSplitter = new System.Windows.Forms.SplitContainer();
            this.graphViewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            this.editorInfoSplitter = new System.Windows.Forms.SplitContainer();
            this.fileDetailsListBox = new System.Windows.Forms.ListBox();
            this.encounterGraphContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.graphEditorSplitter)).BeginInit();
            this.graphEditorSplitter.Panel1.SuspendLayout();
            this.graphEditorSplitter.Panel2.SuspendLayout();
            this.graphEditorSplitter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editorInfoSplitter)).BeginInit();
            this.editorInfoSplitter.Panel2.SuspendLayout();
            this.editorInfoSplitter.SuspendLayout();
            this.SuspendLayout();
            // 
            // encounterGraphContextMenu
            // 
            this.encounterGraphContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyGameMasterNode,
            this.addNewGameMasterNode,
            this.moveToArcMenuItem,
            this.deleteNodeToolStripMenuItem});
            this.encounterGraphContextMenu.Name = "encounterGraphContextMenu";
            this.encounterGraphContextMenu.Size = new System.Drawing.Size(156, 92);
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
            // moveToArcMenuItem
            // 
            this.moveToArcMenuItem.Name = "moveToArcMenuItem";
            this.moveToArcMenuItem.Size = new System.Drawing.Size(155, 22);
            this.moveToArcMenuItem.Text = "Parent to Arc";
            this.moveToArcMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.moveToArcMenuItem_DropDownItemClicked);
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
            // encounterTreeView
            // 
            this.encounterTreeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.encounterTreeView.FullRowSelect = true;
            this.encounterTreeView.HideSelection = false;
            this.encounterTreeView.Location = new System.Drawing.Point(0, 0);
            this.encounterTreeView.Name = "encounterTreeView";
            this.encounterTreeView.PathSeparator = "/";
            this.encounterTreeView.Size = new System.Drawing.Size(250, 498);
            this.encounterTreeView.TabIndex = 6;
            this.encounterTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.encounterTreeView_AfterSelect);
            // 
            // graphEditorSplitter
            // 
            this.graphEditorSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphEditorSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.graphEditorSplitter.Location = new System.Drawing.Point(250, 0);
            this.graphEditorSplitter.Name = "graphEditorSplitter";
            this.graphEditorSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // graphEditorSplitter.Panel1
            // 
            this.graphEditorSplitter.Panel1.Controls.Add(this.graphViewer);
            this.graphEditorSplitter.Panel1MinSize = 200;
            // 
            // graphEditorSplitter.Panel2
            // 
            this.graphEditorSplitter.Panel2.Controls.Add(this.editorInfoSplitter);
            this.graphEditorSplitter.Panel2MinSize = 200;
            this.graphEditorSplitter.Size = new System.Drawing.Size(875, 498);
            this.graphEditorSplitter.SplitterDistance = 200;
            this.graphEditorSplitter.TabIndex = 4;
            // 
            // graphViewer
            // 
            this.graphViewer.ArrowheadLength = 10D;
            this.graphViewer.AsyncLayout = false;
            this.graphViewer.AutoScroll = true;
            this.graphViewer.BackwardEnabled = false;
            this.graphViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.graphViewer.BuildHitTree = true;
            this.graphViewer.ContextMenuStrip = this.encounterGraphContextMenu;
            this.graphViewer.CurrentLayoutMethod = Microsoft.Msagl.GraphViewerGdi.LayoutMethod.UseSettingsOfTheGraph;
            this.graphViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphViewer.EdgeInsertButtonVisible = true;
            this.graphViewer.FileName = "";
            this.graphViewer.ForwardEnabled = false;
            this.graphViewer.Graph = null;
            this.graphViewer.InsertingEdge = false;
            this.graphViewer.LayoutAlgorithmSettingsButtonVisible = false;
            this.graphViewer.LayoutEditingEnabled = false;
            this.graphViewer.Location = new System.Drawing.Point(0, 0);
            this.graphViewer.LooseOffsetForRouting = 0.25D;
            this.graphViewer.MouseHitDistance = 0.05D;
            this.graphViewer.Name = "graphViewer";
            this.graphViewer.NavigationVisible = false;
            this.graphViewer.NeedToCalculateLayout = true;
            this.graphViewer.OffsetForRelaxingInRouting = 0.6D;
            this.graphViewer.PaddingForEdgeRouting = 8D;
            this.graphViewer.PanButtonPressed = false;
            this.graphViewer.SaveAsImageEnabled = false;
            this.graphViewer.SaveAsMsaglEnabled = false;
            this.graphViewer.SaveButtonVisible = false;
            this.graphViewer.SaveGraphButtonVisible = false;
            this.graphViewer.SaveInVectorFormatEnabled = false;
            this.graphViewer.Size = new System.Drawing.Size(875, 200);
            this.graphViewer.TabIndex = 0;
            this.graphViewer.TightOffsetForRouting = 0.125D;
            this.graphViewer.ToolBarIsVisible = false;
            this.graphViewer.Transform = ((Microsoft.Msagl.Core.Geometry.Curves.PlaneTransformation)(resources.GetObject("graphViewer.Transform")));
            this.graphViewer.UndoRedoButtonsVisible = false;
            this.graphViewer.WindowZoomButtonPressed = false;
            this.graphViewer.ZoomF = 1D;
            this.graphViewer.ZoomFraction = 0.5D;
            this.graphViewer.ZoomWhenMouseWheelScroll = true;
            this.graphViewer.ZoomWindowThreshold = 0.05D;
            this.graphViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.graphViewer_MouseMove);
            this.graphViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.graphViewer_MouseDown);
            this.graphViewer.MouseLeave += new System.EventHandler(this.graphViewer_MouseLeave);
            // 
            // editorInfoSplitter
            // 
            this.editorInfoSplitter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.editorInfoSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorInfoSplitter.Location = new System.Drawing.Point(0, 0);
            this.editorInfoSplitter.Name = "editorInfoSplitter";
            // 
            // editorInfoSplitter.Panel1
            // 
            this.editorInfoSplitter.Panel1.AutoScroll = true;
            // 
            // editorInfoSplitter.Panel2
            // 
            this.editorInfoSplitter.Panel2.Controls.Add(this.fileDetailsListBox);
            this.editorInfoSplitter.Panel2Collapsed = true;
            this.editorInfoSplitter.Size = new System.Drawing.Size(875, 294);
            this.editorInfoSplitter.SplitterDistance = 700;
            this.editorInfoSplitter.TabIndex = 0;
            // 
            // fileDetailsListBox
            // 
            this.fileDetailsListBox.BackColor = System.Drawing.Color.White;
            this.fileDetailsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileDetailsListBox.FormattingEnabled = true;
            this.fileDetailsListBox.Location = new System.Drawing.Point(0, 0);
            this.fileDetailsListBox.Name = "fileDetailsListBox";
            this.fileDetailsListBox.Size = new System.Drawing.Size(94, 98);
            this.fileDetailsListBox.TabIndex = 0;
            // 
            // EncounterDesignerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.graphEditorSplitter);
            this.Controls.Add(this.encounterTreeView);
            this.Name = "EncounterDesignerView";
            this.Size = new System.Drawing.Size(1125, 498);
            this.encounterGraphContextMenu.ResumeLayout(false);
            this.graphEditorSplitter.Panel1.ResumeLayout(false);
            this.graphEditorSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.graphEditorSplitter)).EndInit();
            this.graphEditorSplitter.ResumeLayout(false);
            this.editorInfoSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.editorInfoSplitter)).EndInit();
            this.editorInfoSplitter.ResumeLayout(false);
            this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.ContextMenuStrip encounterGraphContextMenu;
      private System.Windows.Forms.ToolStripMenuItem copyGameMasterNode;
      private System.Windows.Forms.ToolStripMenuItem addNewGameMasterNode;
      private System.Windows.Forms.ToolStripMenuItem deleteNodeToolStripMenuItem;
      private System.Windows.Forms.SaveFileDialog saveNewEncounterNodeDialog;
      private System.Windows.Forms.ToolTip i18nTooltip;
      private System.Windows.Forms.SplitContainer graphEditorSplitter;
      private Microsoft.Msagl.GraphViewerGdi.GViewer graphViewer;
      private System.Windows.Forms.TreeView encounterTreeView;
        private System.Windows.Forms.SplitContainer editorInfoSplitter;
        private System.Windows.Forms.ListBox fileDetailsListBox;
        private System.Windows.Forms.ToolStripMenuItem moveToArcMenuItem;
    }
}
