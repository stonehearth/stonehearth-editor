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

    public class UnknownNodeData : NodeData
    {
        public override NodeData Clone(GameMasterNode nodeFile)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateOutEdges(Graph graph)
        {
            throw new NotImplementedException();
        }

        public override void LoadData(Dictionary<string, GameMasterNode> allNodes)
        {
        }
    }
}
