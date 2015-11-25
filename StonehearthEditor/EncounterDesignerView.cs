using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.Msagl.Drawing;
using System.Text.RegularExpressions;

namespace StonehearthEditor
{
   public partial class EncounterDesignerView : UserControl, IGraphOwner
   {
      private static double kMaxDrag = 20;

      private GameMasterNode mSelectedNode = null;
      private Timer refreshGraphTimer = null;
      private double mPreviousMouseX, mPreviousMouseY;
      private int mI18nTooltipLine = -1;

      public EncounterDesignerView()
      {
         InitializeComponent();
      }

      public void Initialize()
      {
         UpdateSelectedNodeInfo(null);
         graphViewer.Graph = null;
         new GameMasterDataManager();
         GameMasterDataManager.GetInstance().Load();
         addNewGameMasterNode.DropDownItems.Clear();
         foreach (EncounterScriptFile scriptFile in GameMasterDataManager.GetInstance().GetGenericScriptNodes())
         {
            if (scriptFile.DefaultJson.Length > 0)
            {
               addNewGameMasterNode.DropDownItems.Add(scriptFile.Name);
            }
         }
         encounterTreeView.Nodes.Clear();
         GameMasterDataManager.GetInstance().FillEncounterNodeTree(encounterTreeView);
      }

      public void SetGraph(Microsoft.Msagl.Drawing.Graph graph)
      {
         graphViewer.Graph = graph;
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
         }
         else
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


      private void graphViewer_EdgeAdded(object sender, EventArgs e)
      {
         Edge edge = (Edge)sender;
         if (!GameMasterDataManager.GetInstance().TryAddEdge(edge.Source, edge.Target))
         {
            // Shouldn't add this edge. Undo it
            graphViewer.Undo();
         }
         else
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
         //TODO yshan: replace this
         Console.WriteLine("edge removed!");
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

      private void graphViewer_MouseUp(object sender, MouseEventArgs e)
      {

      }

      private void nodeInfoJsonPreview_Leave(object sender, EventArgs e)
      {
         string json = nodeInfoJsonPreview.Text;
         if (mSelectedNode != null)
         {
            GameMasterDataManager.GetInstance().TryModifyJson(this, mSelectedNode, json);
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
         if (clickedItem != null && GameMasterDataManager.GetInstance().GraphRoot != null)
         {
            mSelectedNewScriptNode = clickedItem.Text;
            saveNewEncounterNodeDialog.InitialDirectory = System.IO.Path.GetFullPath(GameMasterDataManager.GetInstance().GraphRoot.Directory);
            saveNewEncounterNodeDialog.ShowDialog(this);
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
         if (mSelectedNode != null)
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
               System.IO.File.Delete(path);
               Initialize();
               if (currentCampaignName != null)
               {
                  GameMasterDataManager.GetInstance().SelectCampaign(this, currentCampaignMod, currentCampaignName);
               }
            }
         }
      }

      private void nodeInfoSubType_Click(object sender, EventArgs e)
      {

      }

      private void encounterTreeView_AfterSelect(object sender, TreeViewEventArgs e)
      {
         GameMasterDataManager.GetInstance().OnCampaignSelected(this, e.Node);
         if (GameMasterDataManager.GetInstance().GraphRoot != null)
         {
            addNewGameMasterNode.Enabled = true;
         }
      }

      private void toolstripSaveButton_Click(object sender, EventArgs e)
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

      private class CloneDialogCallback : InputDialog.IDialogCallback
      {
         private GameMasterNode mNode;
         private IGraphOwner mViewer;
         public CloneDialogCallback(IGraphOwner viewer, GameMasterNode node)
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
