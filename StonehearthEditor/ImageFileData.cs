using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StonehearthEditor
{
   class ImageFileData : FileData
   {
      private string mDirectory;

      public ImageFileData(string path)
      {
         mPath = path;
         mDirectory = JsonHelper.NormalizeSystemPath(System.IO.Path.GetDirectoryName(Path));
      }

      public override bool UpdateTreeNode(TreeNode node, string filter)
      {
         return false; // Qubicle files
      }
      public override void Load()
      {
         // do not actually load the binary
         return;
      }
      protected override void LoadInternal()
      {
         return; // Do nothing
      }
      public void AddLinkingJsonFile(JsonFileData file)
      {
         mRelatedFiles.Add(file);
      }
   }
}
