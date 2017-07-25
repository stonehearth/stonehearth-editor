using System.Collections.Generic;
using Microsoft.Msagl.Drawing;

namespace StonehearthEditor
{
    public abstract class NodeData
    {
        public static readonly int kDesiredLabelWidth = 10;

        public GameMasterNode NodeFile { get; set; }

        public abstract void LoadData(Dictionary<string, GameMasterNode> allNodes);

        public virtual void UpdateGraph(Graph graph)
        {
            Node graphNode = graph.AddNode(NodeFile.Id);
            graphNode.LabelText = NodeFile.Name;
            UpdateGraphNode(graphNode);
            UpdateOutEdges(graph);
        }

        public virtual void UpdateGraphNode(Node node)
        {
            SetNodeDefaults(node);
            node.Attr.FillColor = GameMasterNode.kGreen;
            node.Attr.LabelMargin = 6;
        }

        protected void MakeNodePrivate(Node node)
        {
            SetNodeDefaults(node);
            node.Attr.FillColor = GameMasterNode.kPurple;
            node.Attr.Color = GameMasterNode.kPurple;
            node.Attr.LabelMargin = 3;
            node.Label.FontSize = 11;
        }

        protected void SetNodeDefaults(Node node)
        {
            node.LabelText = DecorateString(node.LabelText);
            node.Attr.Shape = Shape.Box;
            EncounterNodeRenderer.SetupNodeRendering(node);
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

        protected void FixupLoot(string selector)
        {
            NodeFile.IsModified = JsonHelper.FixupLootTable(NodeFile.Json, selector);
            NodeFile.SaveIfNecessary();
        }

        private string DecorateString(string rawName)
        {
            if (rawName.Length <= kDesiredLabelWidth)
            {
                return rawName.Replace("_", " ");
            }
            else
            {
                var parts = rawName.Split('_');
                var result = parts[0];
                var lineLength = parts[0].Length;
                for (int i = 1; i < parts.Length; ++i)
                {
                    if (lineLength > kDesiredLabelWidth)
                    {
                        result += '\n';
                        lineLength = 0;
                    }
                    else
                    {
                        result += ' ';
                        lineLength += 1;
                    }

                    result += parts[i];
                    lineLength += parts[i].Length;
                }

                return result;
            }
        }
    }
}
