using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Msagl.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Linq;

namespace StonehearthEditor
{
    public class ArcNodeData : NodeData
    {
        private string mRarity;
        private Dictionary<string, string> mEncounters;
        private List<GameMasterNode> mEncounterFiles;
        public override void LoadData(Dictionary<string, GameMasterNode> allNodes)
        {
            mEncounters = new Dictionary<string, string>();
            mEncounterFiles = new List<GameMasterNode>();
            mRarity = NodeFile.Json["rarity"].ToString();
            mEncounters = JsonConvert.DeserializeObject<Dictionary<string, string>>(NodeFile.Json["encounters"].ToString());
            int lastIndexOfSlash = NodeFile.Path.LastIndexOf('/');
            string nodeFilePathWithoutFileName = NodeFile.Path.Substring(0, lastIndexOfSlash);
            foreach (string filename in mEncounters.Values)
            {
                string absoluteFilePath = JsonHelper.GetFileFromFileJson(filename, nodeFilePathWithoutFileName);
                GameMasterNode otherFile = null;
                if (allNodes.TryGetValue(absoluteFilePath, out otherFile))
                {
                    // this is a proper edge
                    otherFile.Owner = NodeFile;
                    mEncounterFiles.Add(otherFile);
                }
            }
        }

        protected override void UpdateOutEdges(Graph graph)
        {
            foreach (GameMasterNode file in mEncounterFiles)
            {
                if (file.NodeType == GameMasterNodeType.ENCOUNTER)
                {
                    EncounterNodeData nodeData = file.NodeData as EncounterNodeData;
                    if (nodeData.IsStartNode)
                    {
                        graph.AddEdge(NodeFile.Id, file.Id);
                    }
                }
            }
        }

        public List<GameMasterNode> GetEncountersWithInEdge(string inEdgeName)
        {
            List<GameMasterNode> inEdges = new List<GameMasterNode>();
            foreach (GameMasterNode file in mEncounterFiles)
            {
                if (file.NodeType == GameMasterNodeType.ENCOUNTER)
                {
                    EncounterNodeData nodeData = file.NodeData as EncounterNodeData;
                    if (nodeData.InEdge.Equals(inEdgeName))
                    {
                        inEdges.Add(file);
                    }
                }
            }
            return inEdges;
        }

        public override void GetRelatedNodes(HashSet<GameMasterNode> set)
        {
            base.GetRelatedNodes(set);
            foreach (GameMasterNode node in mEncounterFiles)
            {
                if (!set.Contains(node))
                {
                    set.Add(node);
                    node.NodeData.GetRelatedNodes(set);
                }
            }
        }

        public override NodeData Clone(GameMasterNode nodeFile)
        {
            return new ArcNodeData();
        }
        public void AddEncounter(EncounterNodeData encounter)
        {
            // TODO, get relative path
            GameMasterNode encounterNodeFile = encounter.NodeFile;
            string filePath = encounterNodeFile.Path;
            string selfPath = NodeFile.Directory + '/';
            filePath = "file(" + filePath.Replace(selfPath, "") + ")";
            mEncounters.Add(encounterNodeFile.Name, filePath);
            mEncounterFiles.Add(encounterNodeFile);
            NodeFile.Json["encounters"][encounterNodeFile.Name] = filePath;
            NodeFile.IsModified = true;
        }

        public override bool AddOutEdge(GameMasterNode nodeFile)
        {
            if (nodeFile.NodeType == GameMasterNodeType.ENCOUNTER)
            {
                EncounterNodeData encounterData = nodeFile.NodeData as EncounterNodeData;
                if (encounterData.IsStartNode && !mEncounterFiles.Contains(nodeFile))
                {
                    // can only add an edge between this and the encounter if the encounter is a start node
                    AddEncounter(encounterData);
                    return true;
                }
            }
            return false;
        }
    }
}
