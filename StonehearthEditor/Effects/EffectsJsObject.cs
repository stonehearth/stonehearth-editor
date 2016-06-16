using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Effects
{
    class EffectsJsObject
    {
        public Action<string> saveAction { get; set; }

        public void Save(string obj)
        {
            saveAction(obj);
        }
    }
}
