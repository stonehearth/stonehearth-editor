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

      public abstract List<FileData> OpenedFiles { get; }

      public abstract string Path { get; }

      public string FileName
      {
         get { return System.IO.Path.GetFileNameWithoutExtension(Path); }
      }

      public abstract void UpdateTreeNode(TreeNode node);
      public void TrySetFlatFileData(string newData) {
         if (TryChangeFlatFileData(newData))
         {
            mFlatFileData = newData;
         }
      }
      public void TrySaveFile() {
         // TODO(yshan): actually save
      }
      public void FillDependencyListItems(ListView listView)
      {
         listView.Items.Clear();
         foreach (ModuleFile dependency in LinkedAliases)
         {
            listView.Items.Add(dependency.Module.Name + ":" + dependency.Name);
         }
         foreach (string filePath in LinkedFilePaths)
         {
            listView.Items.Add(filePath);
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
