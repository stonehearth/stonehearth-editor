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
      private string mShortName;
      private FileData mFileData = null;
      private FileType mType = FileType.UNKNOWN;
      public ModuleFile(Module module, string alias, string filePath)
      {
         mModule = module;
         mAlias = alias;
         mOriginalFilePath = filePath;
         mRootFile = JsonHelper.GetFileFromFileJson(filePath, module.Path);
         int lastColon = Name.LastIndexOf(':');
         mShortName = lastColon > -1 ? Name.Substring(lastColon + 1) : Name;
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

      public FileData GetFileData(string[] path)
      {
         if (path.Length == 2)
         {
            return this.FileData;
         }
         return FindFileData(FileData, path, 2);
      }
      
      private FileData FindFileData(FileData start, string[] path, int startIndex)
      {
         if (startIndex >= path.Length || start == null)
         {
            return start;
         }
         string subfileName = path[startIndex];
         FileData found = null;
         foreach (FileData openedFile in start.OpenedFiles)
         {
            if (openedFile.FileName.Equals(subfileName))
            {
               found = openedFile;
               break;
            }
         }
         if (found == null)
         {
            foreach (FileData openedFile in start.RelatedFiles)
            {
               if (openedFile.FileName.Equals(subfileName))
               {
                  found = openedFile;
                  break;
               }
            }
         }
         
         return FindFileData(found, path, startIndex + 1);
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

      public bool Clone(string newFileName, HashSet<string> alreadyCloned)
      {
         string newAlias = mAlias.Replace(mShortName, newFileName);
         string newPath = ResolvedPath.Replace(mShortName, newFileName);
         if (!FileData.Clone(newPath, mShortName, newFileName, alreadyCloned))
         {
            return false;
         }
         string fileLocation = "file(" + newPath.Replace(mModule.Path + "/", "") + ")";
         ModuleFile file = new ModuleFile(Module, newAlias, fileLocation);
         file.TryLoad();
         if (file.FileData != null)
         {
            mModule.WriteToManifestJson(newAlias, fileLocation);
            return true;
         }
         return false;
      }

      public string ShortName {
         get { return mShortName; }
      }
   }
}
