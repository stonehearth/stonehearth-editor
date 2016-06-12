using System;
using System.Collections.Generic;
using Microsoft.Msagl.Drawing;

namespace StonehearthEditor
{
    public class CampPieceNodeData : NodeData
    {
        public override NodeData Clone(GameMasterNode nodeFile)
        {
            throw new NotImplementedException();
        }

        public override void LoadData(Dictionary<string, GameMasterNode> allNodes)
        {
        }

        public override void PostLoadFixup()
        {
            if (NodeFile.Json["script_info"] != null)
            {
                FixupLoot("script_info.loot_chests.*.loot_drops");
            }
            else
            {
                FixupLoot("*.*.loot_drops");
            }
        }

        protected override void UpdateOutEdges(Graph graph)
        {
            throw new NotImplementedException();
        }
    }
}
