using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Effects
{
    class EffectsJsObject
    {
        public string EffectKind { get; set; }

        public string Json { get; set; }

        public void Save(object obj)
        {

        }

        public void Preview(object obj)
        {
            obj.ToString();
        }
    }
}
