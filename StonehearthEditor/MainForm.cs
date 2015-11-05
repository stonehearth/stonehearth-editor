using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
      }

      private void LoadModFiles()
      {
         i18nTooltip.Show(string.Empty, nodeInfoJsonPreview, 0);
         UpdateSelectedNodeInfo(null);
         new ModuleDataManager(mModsDirectoryPath);
         ModuleDataManager.GetInstance().Load();
         ModuleDataManager.GetInstance().FillAliasTree(manifestTreeView);

         new GameMasterDataManager();
         GameMasterDataManager.GetInstance().Load();

         foreach (string name in GameMasterDataManager.GetInstance().GetGenericScriptNodeNames())
         {
            addNewGameMasterNode.DropDownItems.Add(name);
         }
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
         if (menuItem != null)
         {
            Console.Write(menuItem.ToString());
         }
      }

      private void manifestTreeView_OnMouseClick(object sender, MouseEventArgs e)
      {
         // Select the clicked node
         manifestTreeView.SelectedNode = manifestTreeView.GetNodeAt(e.X, e.Y);
         ModuleFile file = ModuleDataManager.GetInstance().GetSelectedModuleFile(manifestTreeView.SelectedNode);
         if (file != null)
         {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
               aliasContextMenu.Show(manifestTreeView, e.Location);
            }
            filePreviewBox.Text = file.FlatFileData;
            selectedFilePathLabel.Text = file.ResolvedPath;
         }
         else
         {
            filePreviewBox.Text = "";
            selectedFilePathLabel.Text = "";
         }
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

      private void graphViewer_EdgeAdded(object sender, EventArgs e)
      {
         Edge edge = (Edge)sender;
         if (!GameMasterDataManager.GetInstance().TryAddEdge(edge.Source, edge.Target))
         {
            // Shouldn't add this edge. Undo it
            graphViewer.Undo();
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
   }
}
