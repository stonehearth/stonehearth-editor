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
      private string mFlatFileData;
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

         if (System.IO.File.Exists(mRootFile))
         {
            using (StreamReader sr = new StreamReader(mRootFile, Encoding.UTF8))
            {
               mFlatFileData = sr.ReadToEnd();
               sr.BaseStream.Position = 0;
               sr.DiscardBufferedData();
               mFileData = new JsonFileData(this);
               mFileData.Load(mFlatFileData);
            }
         }
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
         List<FileData> fileDep = mFileData != null ? mFileData.LinkedFiles : null;
         if (fileDep != null)
         {
            foreach (FileData filePath in fileDep)
            {
               listView.Items.Add(filePath.Path);
            }
         }
      }

      public void SetFlatFileData(string newFileData)
      {
         mFlatFileData = newFileData;
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
                  string jsonAsString = mFlatFileData;
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
         get { return mFlatFileData; }
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
