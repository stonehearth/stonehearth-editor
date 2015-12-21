using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StonehearthEditor
{
   class LuaFileData : FileData, IModuleFileData
   {
      private string mDirectory;
      private ModuleFile mOwner;

      public LuaFileData(string path)
      {
         mPath = path;
         mDirectory = JsonHelper.NormalizeSystemPath(System.IO.Path.GetDirectoryName(Path));
      }

      public override bool UpdateTreeNode(TreeNode node, string filter)
      {
         bool filterMatchesSelf = true;
         ModuleFile owner = GetModuleFile();
         if (!string.IsNullOrEmpty(filter) && owner != null && !owner.Name.Contains(filter))
         {
            filterMatchesSelf = false;
         }
         node.SelectedImageIndex = 1;
         node.ImageIndex = 1;
         return filterMatchesSelf;
      }

      protected override void LoadInternal()
      {
         return; // Do nothing
      }

      public override bool Clone(string newPath, string oldName, string newFileName, HashSet<string> alreadyCloned, bool execute)
      {
         return false;
      }

      public void SetModuleFile(ModuleFile moduleFile)
      {
         mOwner = moduleFile;
      }

      public ModuleFile GetModuleFile()
      {
         return mOwner;
      }
   }
}
