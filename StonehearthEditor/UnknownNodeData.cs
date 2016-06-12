using System;
using System.Collections.Generic;
using Microsoft.Msagl.Drawing;

namespace StonehearthEditor
{
    public class UnknownNodeData : NodeData
    {
        public override NodeData Clone(GameMasterNode nodeFile)
        {
            throw new NotImplementedException();
        }

        public override void LoadData(Dictionary<string, GameMasterNode> allNodes)
        {
        }

        protected override void UpdateOutEdges(Graph graph)
        {
            throw new NotImplementedException();
        }
    }
}
