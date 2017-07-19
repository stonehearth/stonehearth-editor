using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor
{
    class NewAliasParameters
    {
        private string mManifestEntryType;
        private Module mSelectedMod;

        public NewAliasParameters(Module selectedMod, string manifestEntryType = "aliases")
        {
            mSelectedMod = selectedMod;
            mManifestEntryType = manifestEntryType;
        }

        public string ManifestEntryType
        {
            get { return mManifestEntryType; }
        }

        public Module SelectedMod
        {
            get { return mSelectedMod; }
        }
    }
}
