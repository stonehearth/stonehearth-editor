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
    public class CampaignNodeData : NodeData
    {
        public List<GameMasterNode> OrphanedNodes { get; } = new List<GameMasterNode>();

        private string mRarity;
        private Dictionary<string, GameMasterNode> mArcTriggers;
        private Dictionary<string, GameMasterNode> mArcChallenges;
        private Dictionary<string, GameMasterNode> mArcClimaxes;
        private int mNumArcNodes = 0;

        // Return a list of all the arcs in the campaign
        public IList<GameMasterNode> GetAllArcs()
        {
            var ret = new List<GameMasterNode>();
            ret.AddRange(mArcTriggers.Values);
            ret.AddRange(mArcChallenges.Values);
            ret.AddRange(mArcClimaxes.Values);
            return ret;
        }

        public override void LoadData(Dictionary<string, GameMasterNode> allNodes)
        {
            mArcTriggers = new Dictionary<string, GameMasterNode>();
            mArcChallenges = new Dictionary<string, GameMasterNode>();
            mArcClimaxes = new Dictionary<string, GameMasterNode>();
            mNumArcNodes = 0;
            mRarity = NodeFile.Json["rarity"].ToString();
            JToken arcs = NodeFile.Json["arcs"];

            Dictionary<string, string> triggers = JsonConvert.DeserializeObject<Dictionary<string, string>>(arcs["trigger"].ToString());
            Dictionary<string, string> challenges = JsonConvert.DeserializeObject<Dictionary<string, string>>(arcs["challenge"].ToString());
            Dictionary<string, string> climaxes = JsonConvert.DeserializeObject<Dictionary<string, string>>(arcs["climax"].ToString());

            SetSelfAsOwner(triggers, mArcTriggers, allNodes);
            SetSelfAsOwner(challenges, mArcChallenges, allNodes);
            SetSelfAsOwner(climaxes, mArcClimaxes, allNodes);
        }

        public override void UpdateGraphNode(Node graphNode)
        {
            base.UpdateGraphNode(graphNode);
            graphNode.Attr.FillColor = Color.LightBlue;
        }

        public override void GetRelatedNodes(HashSet<GameMasterNode> set)
        {
            base.GetRelatedNodes(set);

            foreach (GameMasterNode node in mArcTriggers.Values)
            {
                if (!set.Contains(node))
                {
                    set.Add(node);
                    node.NodeData.GetRelatedNodes(set);
                }
            }

            foreach (GameMasterNode node in mArcChallenges.Values)
            {
                if (!set.Contains(node))
                {
                    set.Add(node);
                    node.NodeData.GetRelatedNodes(set);
                }
            }

            foreach (GameMasterNode node in mArcClimaxes.Values)
            {
                if (!set.Contains(node))
                {
                    set.Add(node);
                    node.NodeData.GetRelatedNodes(set);
                }
            }

            foreach (GameMasterNode node in OrphanedNodes)
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
            return new CampaignNodeData();
        }

        public override bool AddOutEdge(GameMasterNode nodeFile)
        {
            if (nodeFile.NodeType == GameMasterNodeType.ARC)
            {
                mArcTriggers.Add(nodeFile.Name, nodeFile);
                return true;
            }

            return false;
        }

        protected override void UpdateOutEdges(Graph graph)
        {
            foreach (GameMasterNode node in mArcTriggers.Values)
            {
                if (node.NodeType == GameMasterNodeType.ARC)
                {
                    Node triggerNode = graph.AddNode(NodeFile.Id + "#trigger");
                    triggerNode.LabelText = "trigger";
                    MakeNodePrivate(triggerNode);
                    graph.AddEdge(NodeFile.Id, triggerNode.Id);
                    graph.AddEdge(triggerNode.Id, node.Id);
                }
            }

            foreach (GameMasterNode node in mArcChallenges.Values)
            {
                if (node.NodeType == GameMasterNodeType.ARC)
                {
                    Node triggerNode = graph.AddNode(NodeFile.Id + "#challenge");
                    triggerNode.LabelText = "challenge";
                    MakeNodePrivate(triggerNode);
                    graph.AddEdge(NodeFile.Id, triggerNode.Id);
                    graph.AddEdge(triggerNode.Id, node.Id);
                }
            }

            foreach (GameMasterNode node in mArcClimaxes.Values)
            {
                if (node.NodeType == GameMasterNodeType.ARC)
                {
                    Node triggerNode = graph.AddNode(NodeFile.Id + "#climax");
                    triggerNode.LabelText = "climax";
                    MakeNodePrivate(triggerNode);
                    graph.AddEdge(NodeFile.Id, triggerNode.Id);
                    graph.AddEdge(triggerNode.Id, node.Id);
                }
            }
        }

        private void SetSelfAsOwner(Dictionary<string, string> children, Dictionary<string, GameMasterNode> toUpdate, Dictionary<string, GameMasterNode> allNodes)
        {
            int lastIndexOfSlash = NodeFile.Path.LastIndexOf('/');
            string nodeFilePathWithoutFileName = NodeFile.Path.Substring(0, lastIndexOfSlash);
            foreach (KeyValuePair<string, string> child in children)
            {
                string absoluteFilePath = JsonHelper.GetFileFromFileJson(child.Value, nodeFilePathWithoutFileName);
                GameMasterNode otherFile = null;
                if (allNodes.TryGetValue(absoluteFilePath, out otherFile))
                {
                    // this is a proper edge
                    otherFile.Owner = NodeFile;
                    toUpdate.Add(child.Key, otherFile);
                    mNumArcNodes++;
                }
            }
        }
    }
}
