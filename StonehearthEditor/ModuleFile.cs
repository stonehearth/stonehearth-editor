using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System;

namespace StonehearthEditor
{
   public enum FileType
   {
      UNKNOWN = 0,
      LUA = 1,
      JSON = 2,
   };
   public class ModuleFile
   {
      private Module mModule;
      private string mAlias;
      private string mOriginalFilePath;
      private string mRootFile;
      private FileData mFileData = null;
      private FileType mType = FileType.UNKNOWN;
      public bool IsModified = false;

      public ModuleFile(Module module, string alias, string filePath)
      {
         mModule = module;
         mAlias = alias;
         mOriginalFilePath = filePath;
         mRootFile = JsonHelper.GetFileFromFileJson(filePath, module.Path);
         DetermineFileType();
      }

      private void DetermineFileType()
      {
         if (mRootFile.EndsWith(".lua"))
         {
            mType = FileType.LUA;
         } else if (mRootFile.EndsWith(".json"))
         {
            mType = FileType.JSON;
         }
      }

      public void TryLoad()
      {
         if (mType != FileType.JSON) // only load Json
         {
            return;
         }
         mFileData = new JsonFileData(ResolvedPath);
         mFileData.Load();
      }

      public void FillDependencyListItems(ListView listView)
      {
         listView.Items.Clear();
         List<ModuleFile> dependencies = mFileData != null ? mFileData.LinkedAliases : null;
         if (dependencies != null)
         {
            foreach(ModuleFile dependency in dependencies)
            {
               listView.Items.Add(dependency.Module.Name + ":" + dependency.Name);
            }
         }
         List<string> fileDep = mFileData != null ? mFileData.LinkedFilePaths : null;
         if (fileDep != null)
         {
            foreach (string filePath in fileDep)
            {
               listView.Items.Add(filePath);
            }
         }
      }

      public void SetFlatFileData(string newFileData)
      {
         mFileData.TrySetFlatFileData(newFileData);
         IsModified = true;
      }

      public void TrySaveFile()
      {
         if (IsModified)
         {
            try
            {
               using (StreamWriter wr = new StreamWriter(ResolvedPath, false, new UTF8Encoding(false)))
               {
                  string jsonAsString = mFileData.FlatFileData;
                  wr.Write(jsonAsString);
               }
               IsModified = false;
            }
            catch (Exception e)
            {
               Console.WriteLine("Could not write to file " + ResolvedPath + " because of exception: " + e.Message);
            }
         }
      }

      public TreeNode GetTreeNode()
      {
         TreeNode treeNode = new TreeNode(Name);
         if (mFileData != null)
         {
            mFileData.UpdateTreeNode(treeNode);
         }
         return treeNode;
      }

      public string Name
      {
         get { return mAlias; }
      }
      public Module Module
      {
         get { return mModule; }
      }

      public FileType FileType
      {
         get { return mType; }
      }
      public FileData FileData
      {
         get { return mFileData; }
      }
      public string FlatFileData
      {
         get { return mFileData.FlatFileData; }
      }

      public string OriginalPath
      {
         get { return mOriginalFilePath; }
      }

      public string ResolvedPath
      {
         get { return mRootFile; }
      }
   }
}
