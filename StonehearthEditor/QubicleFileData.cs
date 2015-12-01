using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StonehearthEditor
{
   class QubicleFileData : FileData
   {
      private string mDirectory;

      public QubicleFileData(string path)
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
         LoadInternal();
      }
      protected override void LoadInternal()
      {
         // see if the qmo exists
         string qmoPath = mDirectory + FileName + ".qmo";
         if (System.IO.File.Exists(qmoPath))
         {
            mLinkedFilePaths.Add(qmoPath);
         }
      }
      public void AddLinkingJsonFile(JsonFileData file)
      {
         mRelatedFiles.Add(file);
      }
   }
}
