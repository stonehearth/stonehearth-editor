using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Newtonsoft.Json.Linq;

namespace StonehearthEditor
{
    public partial class EncounterDesignerView : UserControl, IGraphOwner, IReloadable
    {
        private static double kMaxDrag = 20;

        private GameMasterNode mSelectedNode = null;
        private Timer refreshGraphTimer = null;
        private double mPreviousMouseX;
        private double mPreviousMouseY;
        private FilePreview mNodePreview = null;

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
                nodePath.Text = node.Path;
                nodeInfoSubType.Text = node.NodeType == GameMasterNodeType.ENCOUNTER ? ((EncounterNodeData)node.NodeData).EncounterType : "";

                if (mNodePreview != null)
                {
                    splitContainer2.Panel1.Controls.Remove(mNodePreview);
                }

                mNodePreview = new FilePreview(this, node.FileData);
                mNodePreview.Dock = DockStyle.Fill;
                splitContainer2.Panel1.Controls.Add(mNodePreview);

                copyGameMasterNode.Text = "Clone " + node.Name;
                copyGameMasterNode.Enabled = true;
                openEncounterFileButton.Visible = true;
                deleteNodeToolStripMenuItem.Visible = true;
                PopulateFileDetails(node);
            }
            else
            {
                mSelectedNode = null;
                nodeInfoName.Text = "Select a Node";
                encounterRightSideFilePath.Text = string.Empty;
                nodeInfoType.Text = string.Empty;
                nodeInfoSubType.Text = string.Empty;
                nodePath.Text = string.Empty;
                if (mNodePreview != null)
                {
                    splitContainer2.Panel1.Controls.Remove(mNodePreview);
                }

                copyGameMasterNode.Text = "Clone Node";
                copyGameMasterNode.Enabled = false;
                openEncounterFileButton.Visible = false;
                deleteNodeToolStripMenuItem.Visible = false;
                PopulateFileDetails(null);
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
                // remove details
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
            // TODO yshan: replace this
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
        }

        private void nodeInfoJsonPreview_MouseMove(object sender, MouseEventArgs e)
        {
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
        }

        public void Reload()
        {
            // Reload the encounter designer.
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
    }
}
