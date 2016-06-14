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

        public string FilePath { get; set; }

        public void Save(string obj)
        {
            if (FilePath != null)
            {
                System.IO.File.WriteAllText(@FilePath, obj);
            }
        }

        public void Preview(string obj)
        {
            obj.ToString();
        }
    }
}
