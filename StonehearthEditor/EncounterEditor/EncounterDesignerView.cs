using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Newtonsoft.Json.Linq;

namespace StonehearthEditor
{
    public partial class EncounterDesignerView : UserControl, IGraphOwner, IReloadable
    {
        private DNode mSelectedDNode = null;
        private GameMasterNode mSelectedNode = null;
        private TreeNode mSelectedCampaign = null;
        private double mPreviousMouseX;
        private double mPreviousMouseY;
        private FilePreview mNodePreview = null;
        private string mSelectedNewScriptNode = null;
        private DNode mHoveredDNode = null;

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
            if (mSelectedNode == node)
            {
                return;
            }

            mSelectedNode = node;

            if (mNodePreview != null)
            {
                if (!mNodePreview.TrySetFileDataFromTextbox())
                {
                    // Don't allow switching nodes if the file is invalid.
                    // This is terrible, but since we currently don't allow writing out invalid JSON,
                    // switching away could cause data loss.
                    return;
                }

                editorInfoSplitter.Panel1.Controls.Remove(mNodePreview);
            }

            if (node != null)
            {
                // Add a text editor.
                mNodePreview = new FilePreview(this, node.FileData);
                mNodePreview.Dock = DockStyle.Fill;
                editorInfoSplitter.Panel1.Controls.Add(mNodePreview);
                UpdateValidationSchema();
                mNodePreview.OnModifiedChanged += (bool isModified) =>
                {
                    if (mSelectedNode?.FileData == mNodePreview.FileData)
                    {
                        mSelectedNode.IsModified = isModified;
                        mSelectedNode.NodeData.UpdateGraphNode(mSelectedDNode.DrawingNode);
                        graphViewer.Invalidate(mSelectedDNode);
                    }
                };

                // Add some extra labels to the text editor toolbar.
                var nodeNameLabel = mNodePreview.toolStrip.Items.Add(node.Name + (node.NodeType == GameMasterNodeType.ENCOUNTER ? (" (" + ((EncounterNodeData)node.NodeData).EncounterType + ")") : ""));
                nodeNameLabel.Margin = new Padding(24, 0, 0, 0);
                nodeNameLabel.Enabled = false;
                var nodePathLabel = mNodePreview.toolStrip.Items.Add(node.Path);
                nodePathLabel.Enabled = false;
                nodePathLabel.Alignment = ToolStripItemAlignment.Right;

                // Set up context menu.
                copyGameMasterNode.Text = "Clone " + node.Name;
                copyGameMasterNode.Enabled = true;
                deleteNodeToolStripMenuItem.Visible = true;
                if (node.Owner == null)
                {
                    moveToArcMenuItem.Visible = true;
                    moveToArcMenuItem.DropDownItems.Clear();
                    var dm = GameMasterDataManager.GetInstance();
                    foreach (var arc in dm.GetAllNodesOfType(GameMasterNodeType.ARC))
                    {
                        if (arc.Owner == dm.GraphRoot)
                        {
                            moveToArcMenuItem.DropDownItems.Add(arc.Name).Tag = arc;
                        }
                    }
                }
                else
                {
                    moveToArcMenuItem.Visible = false;
                }

                // Set up the details panel.
                PopulateFileDetails(node);
                editorInfoSplitter.Panel2Collapsed = fileDetailsListBox.Items.Count == 0;
            }
            else
            {
                copyGameMasterNode.Text = "Clone Node";
                copyGameMasterNode.Enabled = false;
                moveToArcMenuItem.Visible = false;
                deleteNodeToolStripMenuItem.Visible = false;
                PopulateFileDetails(null);
                editorInfoSplitter.Panel2Collapsed = true;
            }
        }

        private void UpdateValidationSchema()
        {
            if (mNodePreview == null || mSelectedNode == null)
            {
                return;
            }

            var encounterNode = mSelectedNode.NodeData as EncounterNodeData;
            if (encounterNode == null)
            {
                return;
            }

            var schema = GameMasterDataManager.GetInstance().GetEncounterSchema(encounterNode.EncounterType);
            if (schema != null)
            {
                var suggester = mNodePreview.SetValidationSchema(schema);
                suggester.AddCustomSource("http://stonehearth.net/schemas/encounters/elements/edge.json", GetAllEdges);
                suggester.AddCustomSource("http://stonehearth.net/schemas/encounters/elements/node.json", GetAllNodes);
            }
        }

        private IEnumerable<string> GetAllEdges()
        {
            if (graphViewer.Graph == null)
            {
                yield break;
            }

            SortedSet<string> allEdges = new SortedSet<string>();
            foreach (var node in graphViewer.Graph.Nodes)
            {
                GameMasterNode nodeData = GameMasterDataManager.GetInstance().GetGameMasterNode(node.Id);
                var encounterNode = nodeData?.NodeData as EncounterNodeData;
                if (encounterNode != null)
                {
                    foreach (var edge in encounterNode.OutEdgeStrings)
                    {
                        allEdges.Add(edge);
                    }
                }
            }

            foreach (var edge in allEdges)
            {
                yield return '"' + edge + '"';
            }
        }

        private IEnumerable<string> GetAllNodes()
        {
            if (graphViewer.Graph == null)
            {
                yield break;
            }

            SortedSet<string> allNodes = new SortedSet<string>();
            foreach (var node in graphViewer.Graph.Nodes)
            {
                GameMasterNode nodeData = GameMasterDataManager.GetInstance().GetGameMasterNode(node.Id);
                if (nodeData != null)
                {
                    allNodes.Add(nodeData.Name);
                }
            }

            foreach (var node in allNodes)
            {
                yield return '"' + node + '"';
            }
        }

        private static string[] kAttributesOfInterest = new string[]
        {
            "max_health",
            "speed",
            "menace",
            "courage",
            "additive_armor_modifier",
            "muscle",
            "exp_reward"
        };

        private void PopulateFileDetails(GameMasterNode node)
        {
            fileDetailsListBox.Items.Clear();
            if (node == null)
            {
                return;
            }

            Dictionary<string, float> stats = new Dictionary<string, float>();

            if (node.NodeType == GameMasterNodeType.ENCOUNTER)
            {
                EncounterNodeData encounterData = node.NodeData as EncounterNodeData;
                if (encounterData.EncounterType == "create_mission" || encounterData.EncounterType == "city_raid")
                {
                    JToken members = node.Json.SelectToken("create_mission_info.mission.members");

                    if (members == null)
                    {
                        JToken missions = node.Json.SelectToken("city_raid_info.missions");
                        foreach (JProperty content in missions.Children())
                        {
                            // Only gets stats for the first mission of city raids
                            members = content.Value["members"];
                        }
                    }

                    int maxEnemies = 0;
                    float totalWeaponBaseDamage = 0;

                    Dictionary<string, float> allStats = new Dictionary<string, float>();
                    foreach (string attribute in kAttributesOfInterest)
                    {
                        allStats[attribute] = 0;
                    }

                    if (members != null)
                    {
                        foreach (JToken member in members.Children())
                        {
                            // grab name, max number of members, and tuning
                            JProperty memberProperty = member as JProperty;
                            if (memberProperty != null)
                            {
                                JValue jMax = memberProperty.Value.SelectToken("from_population.max") as JValue;
                                int max = 0;
                                if (jMax != null)
                                {
                                    max = jMax.Value<int>();
                                    maxEnemies = maxEnemies + max;
                                }

                                JValue tuning = memberProperty.Value.SelectToken("tuning") as JValue;
                                if (tuning != null)
                                {
                                    string alias = tuning.ToString();
                                    ModuleFile tuningFile = ModuleDataManager.GetInstance().GetModuleFile(alias);
                                    if (tuningFile != null)
                                    {
                                        JsonFileData jsonFileData = tuningFile.FileData as JsonFileData;
                                        if (jsonFileData != null)
                                        {
                                            foreach (string attribute in kAttributesOfInterest)
                                            {
                                                JValue jAttribute = jsonFileData.Json.SelectToken("attributes." + attribute) as JValue;
                                                if (jAttribute != null)
                                                {
                                                    allStats[attribute] = allStats[attribute] + (max * jAttribute.Value<int>());
                                                }
                                            }

                                            JArray weapon = jsonFileData.Json.SelectToken("equipment.weapon") as JArray;
                                            if (weapon != null)
                                            {
                                                foreach (JValue weaponAlias in weapon.Children())
                                                {
                                                    ModuleFile weaponModuleFile = ModuleDataManager.GetInstance().GetModuleFile(weaponAlias.ToString());
                                                    if (weaponModuleFile != null)
                                                    {
                                                        JToken baseDamage = (weaponModuleFile.FileData as JsonFileData).Json.SelectToken("entity_data.stonehearth:combat:weapon_data.base_damage");
                                                        if (baseDamage != null)
                                                        {
                                                            totalWeaponBaseDamage = totalWeaponBaseDamage + (max * baseDamage.Value<int>());
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    fileDetailsListBox.Items.Add("max enemies : " + maxEnemies);
                    fileDetailsListBox.Items.Add("total weapon damage : " + totalWeaponBaseDamage);
                    foreach (string attribute in kAttributesOfInterest)
                    {
                        fileDetailsListBox.Items.Add("total " + attribute + " : " + allStats[attribute]);
                    }

                    fileDetailsListBox.Items.Add("average weapon damage : " + totalWeaponBaseDamage / maxEnemies);

                    foreach (string attribute in kAttributesOfInterest)
                    {
                        fileDetailsListBox.Items.Add("average " + attribute + " : " + allStats[attribute] / maxEnemies);
                    }
                }
            }
        }

        private void SetBranchFocused(DNode root, bool highlighted)
        {
            // TODO: The specific styling choices should be handled by EncounterNodeRenderer (and an EncounterEdgeRenderer)
            //       rather than set explicitly here.
            // Fade out (or restore) all nodes and edges that are not in the focused branch.
            var toHighlight = FindAllConnectedObjects(root);
            var desiredAlpha = highlighted ? (byte)128 : (byte)255;
            foreach (var edgeOrNode in graphViewer.Entities)
            {
                if (!toHighlight.Contains(edgeOrNode))
                {
                    if (edgeOrNode is IViewerNode)
                    {
                        Color color = (edgeOrNode as IViewerNode).Node.Attr.FillColor;
                        color.A = desiredAlpha;
                        (edgeOrNode as IViewerNode).Node.Attr.FillColor = color;

                        color = (edgeOrNode as IViewerNode).Node.Attr.Color;
                        color.A = desiredAlpha;
                        (edgeOrNode as IViewerNode).Node.Attr.Color = color;

                        color = (edgeOrNode as IViewerNode).Node.Label.FontColor;
                        color.A = desiredAlpha;
                        (edgeOrNode as IViewerNode).Node.Label.FontColor = color;
                    }
                    else if (edgeOrNode is IViewerEdge)
                    {
                        Color color = (edgeOrNode as IViewerEdge).Edge.Attr.Color;
                        color.A = desiredAlpha;
                        (edgeOrNode as IViewerEdge).Edge.Attr.Color = color;
                    }
                }

                graphViewer.Invalidate(edgeOrNode);
            }
        }

        private void SetBranchHighlighted(DNode root, bool highlighted)
        {
            // Highlight (or restore) all nodes and edges that are not in the highlighted branch.
            var toHighlight = FindAllConnectedObjects(root);
            var lineWidthDelta = highlighted ? +2 : -2;
            var desiredBlueChannelValue = highlighted ? (byte)255 : (byte)0;
            foreach (var edgeOrNode in graphViewer.Entities)
            {
                if (toHighlight.Contains(edgeOrNode))
                {
                    if (edgeOrNode is IViewerNode)
                    {
                        (edgeOrNode as IViewerNode).Node.Attr.LineWidth += lineWidthDelta;
                        var color = (edgeOrNode as IViewerNode).Node.Attr.Color;
                        color.B = desiredBlueChannelValue;
                        (edgeOrNode as IViewerNode).Node.Attr.Color = color;
                    }
                    else if (edgeOrNode is IViewerEdge)
                    {
                        (edgeOrNode as IViewerEdge).Edge.Attr.LineWidth += lineWidthDelta;
                        var color = (edgeOrNode as IViewerEdge).Edge.Attr.Color;
                        color.B = desiredBlueChannelValue;
                        (edgeOrNode as IViewerEdge).Edge.Attr.Color = color;
                    }
                }

                graphViewer.Invalidate(edgeOrNode);
            }
        }

        private HashSet<IViewerObject> FindAllConnectedObjects(DNode root)
        {
            // Find all accessible nodes.
            var result = new HashSet<IViewerObject>();
            var queue = new List<IViewerNode>();
            queue.Add(root);
            while (queue.Count > 0)
            {
                var node = queue.Last();
                queue.RemoveAt(queue.Count - 1);
                if (!result.Contains(node))
                {
                    result.Add(node);
                    result.UnionWith(node.OutEdges);
                    foreach (var edge in node.OutEdges)
                    {
                        queue.Add(edge.Target);
                    }
                }
            }

            return result;
        }

        private void graphViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                double differenceX = e.X - mPreviousMouseX;
                double differenceY = e.Y - mPreviousMouseY;

                mPreviousMouseX = e.X;
                mPreviousMouseY = e.Y;
                graphViewer.Pan(differenceX, differenceY);
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.None)
            {
                var dNode = graphViewer.ObjectUnderMouseCursor as DNode;
                if (mHoveredDNode != dNode)
                {
                    if (mHoveredDNode != null)
                    {
                        SetBranchHighlighted(mHoveredDNode, false);
                        mHoveredDNode = null;
                    }

                    if (dNode != null)
                    {
                        GameMasterNode nodeData = GameMasterDataManager.GetInstance().GetGameMasterNode(dNode.DrawingNode.Id);
                        if (nodeData != null)
                        {
                            mHoveredDNode = dNode;
                            SetBranchHighlighted(mHoveredDNode, true);
                        }
                    }
                }
            }
        }

        private void graphViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                mPreviousMouseX = e.X;
                mPreviousMouseY = e.Y;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Left ||
                     e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (graphViewer.Graph != null)
                {
                    var dNode = graphViewer.ObjectUnderMouseCursor as DNode;
                    if (dNode != null)
                    {
                        GameMasterNode nodeData = GameMasterDataManager.GetInstance().GetGameMasterNode(dNode.DrawingNode.Id);
                        if (nodeData != null)
                        {
                            if (mSelectedDNode != null)
                            {
                                SetBranchFocused(mSelectedDNode, false);
                                mSelectedDNode.DrawingNode.Attr.LineWidth = 1;
                                graphViewer.Invalidate(mSelectedDNode);
                            }

                            mSelectedDNode = dNode;
                            SetBranchFocused(mSelectedDNode, true);
                            mSelectedDNode.DrawingNode.Attr.LineWidth = 7;
                            graphViewer.Invalidate(mSelectedDNode);

                            UpdateSelectedNodeInfo(nodeData);
                        }
                    }
                }
            }
        }

        private void addNewGameMasterNode_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem clickedItem = e.ClickedItem;
            if (clickedItem != null && GameMasterDataManager.GetInstance().GraphRoot != null)
            {
                mSelectedNewScriptNode = clickedItem.Text;
                encounterGraphContextMenu.Hide();
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

        private void deleteNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedNode != null)
            {
                encounterGraphContextMenu.Hide();
                string path = mSelectedNode.Path;
                DialogResult result = MessageBox.Show("Are you sure you want to delete " + path + "?", "Confirm Delete", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    // Unparent.
                    var encounter = mSelectedNode.NodeData as EncounterNodeData;
                    if (encounter != null)
                    {
                        var owner = mSelectedNode.Owner;
                        if (owner?.NodeData is ArcNodeData)
                        {
                            (owner.NodeData as ArcNodeData).RemoveEncounter(encounter);
                        }
                    }

                    // Delete the actual file.
                    System.IO.File.Delete(path);

                    // Reinitialize.
                    GameMasterNode currentCampaign = GameMasterDataManager.GetInstance().GraphRoot;
                    string currentCampaignName = currentCampaign != null ? currentCampaign.Name : null;
                    string currentCampaignMod = currentCampaign != null ? currentCampaign.Module : null;
                    Initialize();
                    if (currentCampaignName != null)
                    {
                        GameMasterDataManager.GetInstance().SelectCampaign(this, currentCampaignMod, currentCampaignName);
                    }
                    GameMasterDataManager.GetInstance().RefreshGraph(this);
                }
            }
        }

        private void encounterTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            GameMasterDataManager.GetInstance().OnCampaignSelected(this, e.Node);
            mSelectedCampaign = e.Node;
            mSelectedDNode = null;
            UpdateSelectedNodeInfo(null);

            if (GameMasterDataManager.GetInstance().GraphRoot != null)
            {
                addNewGameMasterNode.Enabled = true;
            }
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
        }

        public void Reload()
        {
            // Reload the encounter designer.
            GameMasterDataManager.GetInstance().ClearModifiedFlags();
            GameMasterDataManager.GetInstance().RefreshGraph(this);
            encounterTreeView.Refresh();
            if (mNodePreview != null)
            {
                // This can be null.
                mNodePreview.Refresh();
            }

            Initialize();

            // Reselect previously selected campaign if we had one open in the graph view before reloading
            if (mSelectedCampaign != null)
            {
                GameMasterDataManager.GetInstance().OnCampaignSelected(this, mSelectedCampaign);
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

            public void OnCancelled()
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

        private void moveToArcMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (mSelectedNode == null || mSelectedNode.Owner != null || !(mSelectedNode.NodeData is EncounterNodeData))
            {
                return;
            }

            var selectedNodeId = mSelectedNode.Id;
            mSelectedNode.Owner = e.ClickedItem.Tag as GameMasterNode;
            (mSelectedNode.Owner.NodeData as ArcNodeData).AddEncounter(mSelectedNode.NodeData as EncounterNodeData);
            mSelectedNode.Owner.IsModified = true;
            GameMasterDataManager.GetInstance().SaveModifiedFiles();
            GameMasterDataManager.GetInstance().RefreshGraph(this);
        }

        private void graphViewer_MouseLeave(object sender, EventArgs e)
        {
            if (mHoveredDNode != null)
            {
                SetBranchHighlighted(mHoveredDNode, false);
                mHoveredDNode = null;
            }
        }
    }
}
