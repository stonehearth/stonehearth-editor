using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using NJsonSchema;

namespace StonehearthEditor
{
    public class GameMasterDataManager
    {
        private static GameMasterDataManager sInstance = null;

        public static GameMasterDataManager GetInstance()
        {
            return sInstance;
        }

        private Dictionary<string, EncounterScriptFile> mCustomScriptNodes = new Dictionary<string, EncounterScriptFile>();
        private Dictionary<string, EncounterScriptFile> mGenericScriptNodes = new Dictionary<string, EncounterScriptFile>();
        private Dictionary<string, GameMasterNode> mGameMasterNodes = new Dictionary<string, GameMasterNode>();
        private List<GameMasterNode> mCampaignNodes = new List<GameMasterNode>();

        private Graph mGraph;
        private GameMasterNode mCurrentGraphRoot;

        private JsonSchema4 mDefaultSchema;

        public GameMasterDataManager()
        {
            sInstance = this;
        }

        public GameMasterNode GraphRoot
        {
            get { return mCurrentGraphRoot; }
        }

        public void Load()
        {
            ParseGenericEncounterScripts(MainForm.kModsDirectoryPath + "/stonehearth/services/server/game_master/controllers");
            var defaultSchemaPath = Application.StartupPath + "/schemas/encounters/encounter.json";
            using (StreamReader sr = new StreamReader(defaultSchemaPath, System.Text.Encoding.UTF8))
            {
                mDefaultSchema = JsonSchemaTools.ParseSchema(sr.ReadToEnd(), defaultSchemaPath);
            }

            // get the game master index location
            foreach (Module module in ModuleDataManager.GetInstance().GetAllModules())
            {
                ModuleFile gmIndex = module.GetAliasFile("data:gm_index");
                if (gmIndex != null)
                {
                    string folder = JsonHelper.NormalizeSystemPath(System.IO.Path.GetDirectoryName(gmIndex.ResolvedPath));
                    ParseEncounterScripts(module.Name, folder);
                    ParseNodeGraph(module.Name, folder);
                }
            }
        }

        public void OnCampaignSelected(IGraphOwner graphOwner, TreeNode selectedNode)
        {
            if (selectedNode.Parent != null && !selectedNode.Parent.Text.Equals("campaigns"))
            {
                string module = selectedNode.Parent.Text;
                SelectCampaign(graphOwner, selectedNode.Parent.Text, selectedNode.Text);
            }
        }

        public void SelectCampaign(IGraphOwner graphOwner, string module, string name)
        {
            foreach (GameMasterNode node in mCampaignNodes)
            {
                if (name.Equals(node.Name) && node.Module.Equals(module))
                {
                    mCurrentGraphRoot = node;
                    RefreshGraph(graphOwner);
                }
            }
        }

        public GameMasterNode GetGameMasterNode(string nodeId)
        {
            if (nodeId != null && mGameMasterNodes.ContainsKey(nodeId))
            {
                return mGameMasterNodes[nodeId];
            }

            return null;
        }

        public ICollection<EncounterScriptFile> GetGenericScriptNodes()
        {
            return mGenericScriptNodes.Values;
        }

        public JsonSchema4 GetEncounterSchema(string type)
        {
            var key = type + "_encounter";
            return (mGenericScriptNodes.ContainsKey(key) ? mGenericScriptNodes[key].Schema : null) ?? mDefaultSchema;
        }

        public ICollection<GameMasterNode> GetAllNodesOfType(GameMasterNodeType type)
        {
            List<GameMasterNode> result = new List<GameMasterNode>();
            foreach (GameMasterNode node in mGameMasterNodes.Values)
            {
                if (node.NodeType == type)
                {
                    result.Add(node);
                }
            }

            return result;
        }

        public bool CloneNode(IGraphOwner graphOwner, GameMasterNode original, string cloneName)
        {
            GameMasterNode newNode = original.Clone(cloneName);
            mGameMasterNodes.Add(newNode.Path, newNode);

            if (newNode.Owner == null)
            {
                CampaignNodeData campaignNodeData = mCurrentGraphRoot.NodeData as CampaignNodeData;
                campaignNodeData.OrphanedNodes.Add(newNode);
                newNode.Owner = mCurrentGraphRoot;
            }

            RefreshGraph(graphOwner);
            return false;
        }

        public bool TryAddEdge(string sourceId, string destinationId)
        {
            GameMasterNode source = GetGameMasterNode(sourceId);
            GameMasterNode destination = GetGameMasterNode(destinationId);
            return source.NodeData.AddOutEdge(destination);
        }

        public void TryModifyJson(IGraphOwner graphOwner, GameMasterNode node, string newJsonString)
        {
            if (node.TryModifyJson(newJsonString))
            {
                node.OnFileChanged(mGameMasterNodes);
                RefreshGraph(graphOwner);
            }
        }

        public void SaveModifiedFiles()
        {
            foreach (GameMasterNode node in mGameMasterNodes.Values)
            {
                if (node.IsModified)
                {
                    node.SaveIfNecessary();
                    node.Load(mGameMasterNodes);
                }
            }
        }

        public void ClearModifiedFlags()
        {
            foreach (GameMasterNode node in mGameMasterNodes.Values)
            {
                node.IsModified = false;
            }
        }

        public void RefreshGraph(IGraphOwner graphOwner)
        {
            mGraph = new Graph();
            mGraph.Directed = true;
            mGraph.LayoutAlgorithmSettings = new SugiyamaLayoutSettings();

            if (mCurrentGraphRoot != null)
            {
                HashSet<GameMasterNode> campaignNodes = new HashSet<GameMasterNode>();
                mCurrentGraphRoot.NodeData.GetRelatedNodes(campaignNodes);

                foreach (GameMasterNode node in campaignNodes)
                {
                    GameMasterNodeType nodeType = node.NodeType;
                    if (nodeType == GameMasterNodeType.CAMP_PIECE || nodeType == GameMasterNodeType.UNKNOWN)
                    {
                        // Do not add camp pieces or unknown node types.
                        continue;
                    }

                    if (node.NodeData != null)
                    {
                        node.NodeData.UpdateGraph(mGraph);
                    }
                }

                graphOwner.SetGraph(mGraph);
            }
        }

        public void FillEncounterNodeTree(TreeView treeView)
        {
            TreeNode campaignsTree = new TreeNode("campaigns");
            Dictionary<string, TreeNode> moduleTreeNodes = new Dictionary<string, TreeNode>();
            foreach (GameMasterNode node in mCampaignNodes)
            {
                TreeNode parent;
                if (!moduleTreeNodes.TryGetValue(node.Module, out parent))
                {
                    parent = new TreeNode(node.Module);
                    moduleTreeNodes[node.Module] = parent;
                    campaignsTree.Nodes.Add(parent);
                }

                TreeNode treeNode = new TreeNode(node.Name);
                parent.Nodes.Add(treeNode);
            }

            campaignsTree.ExpandAll();
            treeView.Nodes.Add(campaignsTree);
        }

        public bool AddNewGenericScriptNode(IGraphOwner owner, string scriptNodeName, string filePath)
        {
            if (mCurrentGraphRoot == null)
            {
                return false;
            }

            EncounterScriptFile scriptFile = mGenericScriptNodes[scriptNodeName];
            scriptFile.WriteDefaultToFile(filePath);
            GameMasterNode newNode = new GameMasterNode(mCurrentGraphRoot.Module, filePath);
            mGameMasterNodes.Add(newNode.Path, newNode);
            newNode.Load(mGameMasterNodes);

            string nodeName = Path.GetFileNameWithoutExtension(filePath);
            CampaignNodeData campaignNodeData = mCurrentGraphRoot.NodeData as CampaignNodeData;
            string arcsDir = Path.GetFullPath(Path.Combine(campaignNodeData.NodeFile.Path, "..", "arcs")).ToUpperInvariant();
            string filePathUpper = Path.GetFullPath(filePath).ToUpperInvariant();

            bool foundMatchingArc = false;
            foreach (var arcNode in campaignNodeData.GetAllArcs())
            {
                string arcDir = Path.GetFullPath(Path.Combine(arcNode.Path, "..")).ToUpperInvariant();
                if (filePathUpper.StartsWith(arcDir))
                {
                    // Add new node to arc's index file (ex. game_events_arc) using nodeName as the key
                    (arcNode.NodeData as ArcNodeData).AddEncounter(newNode.NodeData as EncounterNodeData);
                    newNode.Owner = arcNode;
                    foundMatchingArc = true;
                    break;
                }
            }

            if (!foundMatchingArc)
            {
                (mCurrentGraphRoot.NodeData as CampaignNodeData).OrphanedNodes.Add(newNode);
                newNode.Owner = mCurrentGraphRoot;
            }

            newNode.IsModified = true;
            newNode.SaveIfNecessary();
            RefreshGraph(owner);
            return true;
        }

        public bool DeleteNode(string nodePath)
        {
            GameMasterNode node = GetGameMasterNode(nodePath);
            if (node == null)
            {
                return false;
            }

            File.Delete(nodePath);
            mGameMasterNodes.Remove(nodePath);

            return false;
        }

        private void SearchForFileType(string directory, string fileType, List<string> luaFilesFound)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            string[] directories = Directory.GetDirectories(directory);
            if (directories == null)
            {
                return;
            }

            foreach (string d in directories)
            {
                string[] files = Directory.GetFiles(d, fileType);
                if (files != null)
                {
                    foreach (string f in files)
                    {
                        string formatted = JsonHelper.NormalizeSystemPath(f);
                        luaFilesFound.Add(formatted);
                    }

                    SearchForFileType(d, fileType, luaFilesFound);
                }
            }
        }

        private void ParseGenericEncounterScripts(string folderPath)
        {
            List<string> genericScriptFiles = new List<string>();
            SearchForFileType(folderPath, "*.lua", genericScriptFiles);
            foreach (string filepath in genericScriptFiles)
            {
                EncounterScriptFile file = new EncounterScriptFile(filepath);
                file.Load();
                mGenericScriptNodes.Add(file.Name, file);
            }
        }

        private static Dictionary<string, int> kCollisions = new Dictionary<string, int>();

        private void ParseEncounterScripts(string moduleName, string folderPath)
        {
            List<string> filePaths = new List<string>();
            SearchForFileType(folderPath, "*.lua", filePaths);

            foreach (string filepath in filePaths)
            {
                EncounterScriptFile file = new EncounterScriptFile(filepath);
                file.Load();
                string fileName = file.Name;

                if (mCustomScriptNodes.ContainsKey(fileName))
                {
                    int numCollisions = 0;
                    kCollisions.TryGetValue(fileName, out numCollisions);
                    numCollisions++;
                    kCollisions[fileName] = numCollisions;
                    fileName = fileName + "(" + numCollisions + ")";
                }

                mCustomScriptNodes.Add(fileName, file);
            }
        }

        private void ParseNodeGraph(string moduleName, string folderPath)
        {
            List<string> nodeFiles = new List<string>();
            SearchForFileType(folderPath, "*.json", nodeFiles);
            Dictionary<string, GameMasterNode> addedNodes = new Dictionary<string, GameMasterNode>();
            foreach (string filepath in nodeFiles)
            {
                GameMasterNode file = new GameMasterNode(moduleName, filepath);
                addedNodes.Add(file.Path, file);
            }

            foreach (GameMasterNode node in addedNodes.Values)
            {
                node.Load(addedNodes);
            }

            CampaignNodeData currentCampaignData = null;
            List<GameMasterNode> orphanedNodes = new List<GameMasterNode>();
            foreach (GameMasterNode node in addedNodes.Values)
            {
                if (node.NodeType == GameMasterNodeType.CAMPAIGN)
                {
                    currentCampaignData = node.NodeData as CampaignNodeData;
                    mCampaignNodes.Add(node);
                }
                else if (node.Owner == null)
                {
                    orphanedNodes.Add(node);
                }
            }

            if (currentCampaignData != null)
            {
                currentCampaignData.OrphanedNodes.AddRange(orphanedNodes);
            }
            else
            {
                MessageBox.Show("Could not find root campaign node in: " + folderPath);
            }

            mGameMasterNodes = mGameMasterNodes.Concat(addedNodes).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}
