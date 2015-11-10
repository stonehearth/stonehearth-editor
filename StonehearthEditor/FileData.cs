using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StonehearthEditor
{
   public abstract class FileData
   {
      private string mFlatFileData;
      public abstract List<ModuleFile> LinkedAliases { get; }
      public abstract List<string> LinkedFilePaths { get; }

      public abstract string Path { get; }


      public abstract void UpdateTreeNode(TreeNode node);
      public void TrySetFlatFileData(string newData) {
         if (TryChangeFlatFileData(newData))
         {
            mFlatFileData = newData;
         }
      }

      public void Load()
      {
         if (System.IO.File.Exists(Path))
         {
            using (StreamReader sr = new StreamReader(Path, Encoding.UTF8))
            {
               mFlatFileData = sr.ReadToEnd();
               sr.BaseStream.Position = 0;
               sr.DiscardBufferedData();
               LoadInternal();
            }
         }
      }

      public string FlatFileData
      {
         get { return mFlatFileData; }
      }

      // custom load call
      protected virtual void LoadInternal() { }

      protected virtual bool TryChangeFlatFileData(string newData)
      {
         return true;
      }
   }
}
