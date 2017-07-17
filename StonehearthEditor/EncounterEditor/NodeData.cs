using System.Collections.Generic;
using Microsoft.Msagl.Drawing;

namespace StonehearthEditor
{
    public abstract class NodeData
    {
        public GameMasterNode NodeFile { get; set; }

        public abstract void LoadData(Dictionary<string, GameMasterNode> allNodes);

        public virtual void UpdateGraph(Graph graph)
        {
            Node graphNode = graph.AddNode(NodeFile.Id);
            graphNode.LabelText = NodeFile.Name.Replace("_", " ");
            UpdateGraphNode(graphNode);
            UpdateOutEdges(graph);
        }

        public virtual void UpdateGraphNode(Node graphNode)
        {
            graphNode.Attr.LabelWidthToHeightRatio = 1;
            graphNode.Attr.Shape = Shape.Box;
            graphNode.Attr.FillColor = GameMasterNode.kGreen;
            graphNode.Attr.LabelMargin = 6;
        }

        public virtual void GetRelatedNodes(HashSet<GameMasterNode> set)
        {
            set.Add(NodeFile);
        }

        public virtual void PostLoadFixup()
        {
        }

        public abstract NodeData Clone(GameMasterNode nodeFile);

        public virtual bool AddOutEdge(GameMasterNode nodeFile)
        {
            return false;
        }

        protected abstract void UpdateOutEdges(Graph graph);

        protected void MakeNodePrivate(Node node)
        {
            node.Attr.Shape = Shape.Box;
            node.Attr.LabelMargin = 3;
            node.Attr.FillColor = GameMasterNode.kPurple;
            node.Attr.Color = GameMasterNode.kPurple;
            node.Label.FontSize = 11;
        }

        protected void FixupLoot(string selector)
        {
            NodeFile.IsModified = JsonHelper.FixupLootTable(NodeFile.Json, selector);
            NodeFile.SaveIfNecessary();
        }
    }
}
