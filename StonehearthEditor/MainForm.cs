using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace StonehearthEditor
{
   public interface IGraphOwner
   {
      void SetGraph(Microsoft.Msagl.Drawing.Graph graph);
   }

   public partial class StonehearthEditor : Form, IGraphOwner
   {
      private static double kMaxDrag = 20;

      private string mModsDirectoryPath;

      private double mPreviousMouseX, mPreviousMouseY;
      private GameMasterNode mSelectedNode = null;
      private int mI18nTooltipLine = -1;

      public StonehearthEditor(string path)
      {
         mModsDirectoryPath = path;
         InitializeComponent();
      }

      private void Form1_Load(object sender, EventArgs e)
      {
         if (string.IsNullOrEmpty(mModsDirectoryPath))
         {
            DialogResult result = modsFolderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
               mModsDirectoryPath = modsFolderBrowserDialog.SelectedPath;
               Properties.Settings.Default["ModsDirectory"] = mModsDirectoryPath;
               Properties.Settings.Default.Save();
            }
         }
         LoadModFiles();
         int initialTab = (int) Properties.Settings.Default["InitialTab"];
         tabControl.SelectedIndex = initialTab;
      }

      private void LoadModFiles()
      {
         i18nTooltip.Show(string.Empty, nodeInfoJsonPreview, 0);
         new ModuleDataManager(mModsDirectoryPath);
         ModuleDataManager.GetInstance().Load();
         ModuleDataManager.GetInstance().FilterAliasTree(treeView, null);

         StartGameMasterDataManager();
      }
      private void StartGameMasterDataManager()
      {
         UpdateSelectedNodeInfo(null);
         graphViewer.Graph = null;
         new GameMasterDataManager();
         GameMasterDataManager.GetInstance().Load();
         addNewGameMasterNode.DropDownItems.Clear();
         foreach (string name in GameMasterDataManager.GetInstance().GetGenericScriptNodeNames())
         {
            addNewGameMasterNode.DropDownItems.Add(name);
         }
         encounterTreeView.Nodes.Clear();
         GameMasterDataManager.GetInstance().FillEncounterNodeTree(encounterTreeView);
      }

      public void SetGraph(Microsoft.Msagl.Drawing.Graph graph)
      {
         graphViewer.Graph = graph;
      }

      public string ModsDirectoryPath
      {
         get { return mModsDirectoryPath; }
      }

      private void aliasContextMenuDuplicate_Click(object sender, EventArgs e)
      {
         ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
         if (menuItem == null)
         {
            return;
         }
         TreeNode selectedNode = treeView.SelectedNode;
         if (selectedNode == null || selectedNode.Parent == null)
         {
            return;
         }
         string module = selectedNode.Parent.Text;
         string alias = selectedNode.Text;
         Module mod = ModuleDataManager.GetInstance().GetMod(module);
         if (mod == null)
         {
            return;
         }
         ModuleFile aliasFile = mod.GetAliasFile(alias);
         if (aliasFile == null)
         {
            return;
         }
         if (aliasFile.FileType != FileType.JSON)
         {
            return; // best not dupe non-json files
         }

         CloneAliasCallback callback = new CloneAliasCallback(this, aliasFile);
         int lastColon = alias.LastIndexOf(':');
         string aliasShortName = lastColon > -1 ? alias.Substring(lastColon + 1) : alias;
         InputDialog dialog = new InputDialog("Clone " + module + ":" + alias, "Type name of duplicated alias", aliasShortName, "Clone!");
         dialog.SetCallback(callback);
         dialog.ShowDialog();
      }

      private void manifestTreeView_OnMouseClick(object sender, MouseEventArgs e)
      {
         // Select the clicked node
         treeView.SelectedNode = treeView.GetNodeAt(e.X, e.Y);
         ModuleFile file = ModuleDataManager.GetInstance().GetSelectedModuleFile(treeView.SelectedNode);
         if (file != null)
         {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
               aliasContextMenu.Show(treeView, e.Location);
            }
            filePreviewBox.Text = file.FlatFileData;
            selectedFilePathLabel.Text = file.ResolvedPath;
            file.FillDependencyListItems(dependenciesListView);
         }
         else
         {
            filePreviewBox.Text = "";
            selectedFilePathLabel.Text = "";
         }
      }

      private void link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         Console.WriteLine("Link clicked: " + e.Link.LinkData.ToString());
      }

      private void filePreviewBox_TextChanged(object sender, EventArgs e)
      {

      }

      private void panel1_Paint(object sender, PaintEventArgs e)
      {

      }

      private void encounterTreeView_AfterSelect(object sender, TreeViewEventArgs e)
      {
         GameMasterDataManager.GetInstance().OnCampaignSelected(this, e.Node);
         addNewGameMasterNode.Enabled = true;
      }

      private void nodeInfoSubType_Click(object sender, EventArgs e)
      {

      }

      private void graphViewer_MouseClick(object sender, MouseEventArgs e)
      {
         
      }

      private void graphViewer_MouseDown(object sender, MouseEventArgs e)
      {
         if (graphViewer.Graph != null)
         {
            object obj = graphViewer.GetObjectAt(e.X, e.Y);
            var dnode = obj as DNode;
            if (dnode != null)
            {
               Node drawingNode = dnode.DrawingNode;
               GameMasterNode nodeData = GameMasterDataManager.GetInstance().GetGameMasterNode(drawingNode.Id);
               UpdateSelectedNodeInfo(nodeData);
            }
            else
            {
               UpdateSelectedNodeInfo(null);
            }
         }
      }

      private void UpdateSelectedNodeInfo(GameMasterNode node)
      {
         if (node != null)
         {
            mSelectedNode = node;
            nodeInfoName.Text = node.Name;
            encounterRightSideFilePath.Text = node.Path;
            nodeInfoType.Text = node.NodeType.ToString();
            nodeInfoSubType.Text = node.NodeType == GameMasterNodeType.ENCOUNTER ? ((EncounterNodeData)node.NodeData).EncounterType : "";
            nodeInfoJsonPreview.Text = node.GetJsonFileString();
            nodeInfoJsonPreview.ScrollToCaret();

            copyGameMasterNode.Text = "Clone " + node.Name;
            copyGameMasterNode.Enabled = true;
            openEncounterFileButton.Visible = true;
            deleteNodeToolStripMenuItem.Visible = true;
         } else
         {
            mSelectedNode = null;
            nodeInfoName.Text = "Select a Node";
            encounterRightSideFilePath.Text = string.Empty;
            nodeInfoType.Text = string.Empty;
            nodeInfoSubType.Text = string.Empty;
            nodeInfoJsonPreview.Text = string.Empty;

            copyGameMasterNode.Text = "Clone Node";
            copyGameMasterNode.Enabled = false;
            openEncounterFileButton.Visible = false;
            deleteNodeToolStripMenuItem.Visible = false;
         }
      }

      private void graphViewer_MouseUp(object sender, MouseEventArgs e)
      {
      }

      private void copyGameMasterNode_Click(object sender, EventArgs e)
      {
         if (mSelectedNode != null)
         {
            CloneDialogCallback callback = new CloneDialogCallback(this, mSelectedNode);
            InputDialog dialog = new InputDialog("Clone " + mSelectedNode.Name, "Type name of new node", mSelectedNode.Name, "Clone!");
            dialog.SetCallback(callback);
            dialog.ShowDialog();
         }
      }

      private void graphViewer_MouseMove(object sender, MouseEventArgs e)
      {
         if (e.Button == System.Windows.Forms.MouseButtons.Middle)
         {
            double differenceX = e.X - mPreviousMouseX;
            double differenceY = e.Y - mPreviousMouseY;

            differenceX = Math.Min(Math.Max(differenceX, -kMaxDrag), kMaxDrag);
            differenceY = Math.Min(Math.Max(differenceY, -kMaxDrag), kMaxDrag);

            mPreviousMouseX = e.X;
            mPreviousMouseY = e.Y;
            graphViewer.Pan(differenceX, differenceY);
         }
      }

      private Timer refreshGraphTimer = null;
      private void graphViewer_EdgeAdded(object sender, EventArgs e)
      {
         Edge edge = (Edge)sender;
         if (!GameMasterDataManager.GetInstance().TryAddEdge(edge.Source, edge.Target))
         {
            // Shouldn't add this edge. Undo it
            graphViewer.Undo();
         } else
         {
            GameMasterDataManager.GetInstance().SaveModifiedFiles();
            if (refreshGraphTimer == null)
            {
               refreshGraphTimer = new Timer();
               refreshGraphTimer.Interval = 100;
               refreshGraphTimer.Enabled = true;
               refreshGraphTimer.Tick += new EventHandler(OnRefreshTimerTick);
               refreshGraphTimer.Start();
            }  
         }
      }

      private void OnRefreshTimerTick(object sender, EventArgs e)
      {
         GameMasterDataManager.GetInstance().RefreshGraph(this);
         if (refreshGraphTimer != null)
         {
            refreshGraphTimer.Stop();
            refreshGraphTimer = null;
         }
      }

      private void graphViewer_EdgeRemoved(object sender, EventArgs e)
      {
         Console.WriteLine("edge removed!");
      }

      private void toolstripSaveButton_Click(object sender, EventArgs e)
      {
         // Save graph editor stuff
         GameMasterDataManager.GetInstance().SaveModifiedFiles();
      }

      private void nodeInfoJsonPreview_Leave(object sender, EventArgs e)
      {
         string json = nodeInfoJsonPreview.Text;
         if (mSelectedNode != null)
         {
            GameMasterDataManager.GetInstance().TryModifyJson(this, mSelectedNode, json);
         }
      }

      private void StonehearthEditor_KeyDown(object sender, KeyEventArgs e)
      {
         if (e.Control && e.KeyCode == Keys.S)
         {
            string json = nodeInfoJsonPreview.Text;
            if (mSelectedNode != null)
            {
               GameMasterDataManager.GetInstance().TryModifyJson(this, mSelectedNode, json);
            }
            GameMasterDataManager.GetInstance().SaveModifiedFiles();
         }
      }

      private void nodeInfoJsonPreview_MouseMove(object sender, MouseEventArgs e)
      {
         int charIndex = nodeInfoJsonPreview.GetCharIndexFromPosition(e.Location);
         int line = nodeInfoJsonPreview.GetLineFromCharIndex(charIndex);
         
         if (nodeInfoJsonPreview.Lines.Length <= line)
         {
            return;
         }
         if (mI18nTooltipLine == line)
         {
            return;
         }
         i18nTooltip.Hide(nodeInfoJsonPreview);

         mI18nTooltipLine = line;
         string lineString = nodeInfoJsonPreview.Lines[line];
         Regex matcher = new Regex(@"i18n\(([^)]+)\)");
         Match locMatch = matcher.Match(lineString);
         if (locMatch.Success)
         {
            string translated = ModuleDataManager.GetInstance().LocalizeString(locMatch.Groups[1].Value);
            translated = JsonHelper.WordWrap(translated, 100);
            i18nTooltip.Show(translated, nodeInfoJsonPreview, e.Location);
         }
         else
         {
            i18nTooltip.Hide(nodeInfoJsonPreview);
         }
      }

      private string mSelectedNewScriptNode = null;
      private void addNewGameMasterNode_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
      {
         ToolStripItem clickedItem = e.ClickedItem;
         if (clickedItem != null)
         {
            mSelectedNewScriptNode = clickedItem.Text;
            saveNewEncounterNodeDialog.InitialDirectory = Path.GetFullPath(GameMasterDataManager.GetInstance().GraphRoot.Directory);
            saveNewEncounterNodeDialog.ShowDialog(encounterTab);
         }
      }
      private void saveNewEncounterNodeDialog_FileOk(object sender, CancelEventArgs e)
      {
         string filePath = saveNewEncounterNodeDialog.FileName;
         if (filePath == null)
         {
            return;
         }
         filePath = JsonHelper.NormalizeSystemPath(filePath);
         GameMasterNode existingNode = GameMasterDataManager.GetInstance().GetGameMasterNode(filePath);
         if (existingNode != null)
         {
            MessageBox.Show("Cannot override an existing node. Either edit that node or create a new name.");
            return;
         }
         GameMasterDataManager.GetInstance().AddNewGenericScriptNode(this, mSelectedNewScriptNode, filePath);
      }

      private void openEncounterFileButton_Click(object sender, EventArgs e)
      {
         if (mSelectedNode!= null)
         {
            string path = mSelectedNode.Path;
            System.Diagnostics.Process.Start(@path);
         }
      }

      private void deleteNodeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (mSelectedNode != null)
         {
            string path = mSelectedNode.Path;
            DialogResult result = MessageBox.Show("Are you sure you want to delete " + path + "?", "Confirm Delete", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
               GameMasterNode currentCampaign = GameMasterDataManager.GetInstance().GraphRoot;
               string currentCampaignName = currentCampaign != null ? currentCampaign.Name : null;
               string currentCampaignMod = currentCampaign != null ? currentCampaign.Module : null;
               File.Delete(path);
               StartGameMasterDataManager();
               if (currentCampaignName != null)
               {
                  GameMasterDataManager.GetInstance().SelectCampaign(this, currentCampaignMod, currentCampaignName);
               }
            }
         }
      }

      private void tabControl_Selected(object sender, TabControlEventArgs e)
      {
         //e.TabPageIndex;
         Properties.Settings.Default["InitialTab"] = e.TabPageIndex;
         Properties.Settings.Default.Save();
      }

      private void searchBox_TextChanged(object sender, EventArgs e)
      {
         string searchTerm = searchBox.Text;
         ModuleDataManager.GetInstance().FilterAliasTree(treeView, searchTerm);
      }

      private class CloneDialogCallback : InputDialog.IDialogCallback
      {
         private GameMasterNode mNode;
         private StonehearthEditor mViewer;
         public CloneDialogCallback(StonehearthEditor viewer, GameMasterNode node)
         {
            mViewer = viewer;
            mNode = node;
         }
         public void onCancelled()
         {
            // Do nothing. user cancelled
         }

         public bool OnAccept(string inputMessage)
         {
            // Do the cloning
            string potentialNewNodeName = inputMessage.Trim();
            if (potentialNewNodeName.Length <= 1)
            {
               MessageBox.Show("You must enter a name longer than 1 character for the clone!");
               return false;
            }
            if (potentialNewNodeName.Equals(mNode.Name))
            {
               MessageBox.Show("You must enter a new unique name for the clone!");
               return false;
            }
            GameMasterDataManager.GetInstance().CloneNode(mViewer, mNode, potentialNewNodeName);
            return true;
         }
      }

      private class CloneAliasCallback : InputDialog.IDialogCallback
      {
         private ModuleFile mModuleFile;
         private StonehearthEditor mViewer;
         public CloneAliasCallback(StonehearthEditor viewer, ModuleFile moduleFile)
         {
            mViewer = viewer;
            mModuleFile = moduleFile;
         }
         public void onCancelled()
         {
            // Do nothing. user cancelled
         }

         public bool OnAccept(string inputMessage)
         {
            // Do the cloning
            string potentialNewNodeName = inputMessage.Trim();
            if (potentialNewNodeName.Length <= 1)
            {
               MessageBox.Show("You must enter a name longer than 1 character for the clone!");
               return false;
            }
            int lastColon = mModuleFile.Name.LastIndexOf(':');
            string aliasShortName = lastColon > -1 ? mModuleFile.Name.Substring(lastColon + 1) : mModuleFile.Name;
            if (potentialNewNodeName.Equals(aliasShortName))
            {
               MessageBox.Show("You must enter a new unique name for the clone!");
               return false;
            }
            ModuleDataManager.GetInstance().CloneAlias(mModuleFile, potentialNewNodeName);
            return true;
         }
      }
   }
}
