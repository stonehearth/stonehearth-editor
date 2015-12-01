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
      private bool mIsQb = true;

      public QubicleFileData(string path)
      {
         mPath = path;
         mDirectory = JsonHelper.NormalizeSystemPath(System.IO.Path.GetDirectoryName(Path));
         if (System.IO.Path.GetExtension(path).Equals(".qmo"))
         {
            mIsQb = false;
         }
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
         if (mIsQb)
         {
            // see if the qmo exists
            string qmoPath = mDirectory + FileName + ".qmo";
            if (System.IO.File.Exists(qmoPath))
            {
               QubicleFileData qmoFile = new QubicleFileData(qmoPath);
               mLinkedFileData.Add(qmoPath, qmoFile);
            }
         }
      }
      public void AddLinkingJsonFile(JsonFileData file)
      {
         mRelatedFiles.Add(file);
      }

      public override bool Clone(string newPath, string oldName, string newFileName, HashSet<string> alreadyCloned, bool execute)
      {
         if (execute)
         {
            string newDirectory = System.IO.Path.GetDirectoryName(newPath);
            System.IO.Directory.CreateDirectory(newDirectory);
            System.IO.File.Copy(Path, newPath);
         }
         string qmoPath = mDirectory + FileName + ".qmo";
         if (mIsQb && mLinkedFileData.ContainsKey(qmoPath))
         {
            alreadyCloned.Add(qmoPath);
            string newQmoPath = newPath.Replace(".qb", ".qmo");
            mLinkedFileData[qmoPath].Clone(newQmoPath, oldName, newFileName, alreadyCloned, execute);
         }
         return true;
      }
   }
}
