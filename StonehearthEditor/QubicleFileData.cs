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
      private bool mHasQmo = false;

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
            string qmoPath = GetQmoPath();
            if (System.IO.File.Exists(qmoPath))
            {
               mHasQmo = true;
               QubicleFileData qmoFile = new QubicleFileData(qmoPath);
               mLinkedFileData.Add(qmoPath, qmoFile);
            }
         }
      }
      public void AddLinkingJsonFile(JsonFileData file)
      {
         mRelatedFiles.Add(file);
      }

      public override bool Clone(string newPath, CloneObjectParameters parameters, HashSet<string> alreadyCloned, bool execute)
      {
         if (execute)
         {
            string newDirectory = System.IO.Path.GetDirectoryName(newPath);
            System.IO.Directory.CreateDirectory(newDirectory);
            if (!System.IO.File.Exists(newPath))
            {
               System.IO.File.Copy(Path, newPath);
            }
         }
         string qmoPath = GetQmoPath();
         if (mIsQb && mLinkedFileData.ContainsKey(qmoPath))
         {
            string newQmoPath = newPath.Replace(".qb", ".qmo");
            if (!alreadyCloned.Contains(newQmoPath))
            {
               alreadyCloned.Add(newQmoPath);
               mLinkedFileData[qmoPath].Clone(newQmoPath, parameters, alreadyCloned, execute);
            }
         }
         return true;
      }

      private string GetQmoPath()
      {
         return mDirectory + "/" + FileName + ".qmo";
      }

      public string GetOpenFilePath()
      {
         if (mHasQmo)
         {
            return GetQmoPath();
      }
         return mPath;
      }
   }
}
