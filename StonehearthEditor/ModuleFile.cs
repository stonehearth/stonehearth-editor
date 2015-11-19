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
      private bool mIsFineVersion = false;
      public ModuleFile(Module module, string alias, string filePath)
      {
         mModule = module;
         mAlias = alias;
         mOriginalFilePath = filePath;
         mRootFile = JsonHelper.GetFileFromFileJson(filePath, module.Path);
         int lastColon = Name.LastIndexOf(':');
         mShortName = lastColon > -1 ? Name.Substring(lastColon + 1) : Name;
         if (mShortName.Equals("fine"))
         {
            string oneBefore = Name.Substring(0, lastColon);
            int secondToLastColon = oneBefore.LastIndexOf(':');
            oneBefore = secondToLastColon > -1 ? oneBefore.Substring(secondToLastColon + 1) : oneBefore;
            mShortName = oneBefore;
            mIsFineVersion = true;
         }
         DetermineFileType();
      }

      public bool IsFineVersion
      {
         get { return mIsFineVersion; }
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
         JsonFileData fileData = new JsonFileData(ResolvedPath);
         mFileData = fileData;
         fileData.SetModuleFile(this);
         fileData.Load();
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

      public TreeNode GetTreeNode(string filter)
      {
         TreeNode treeNode = new TreeNode(Name);
         if (mFileData != null)
         {
            bool matchesFilter = mFileData.UpdateTreeNode(treeNode, filter);
            if (matchesFilter)
            {
               return treeNode;
            }
         }
         return null;
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

      public bool Clone(string oldName, string newFileName, HashSet<string> alreadyCloned, bool execute)
      {
         string newAlias = mAlias.Replace(oldName, newFileName);
         string newFileNameInPath = newFileName;
         if (newFileName.IndexOf(':') >= 0)
         {
            newFileNameInPath = newFileName.Replace(':', '_');
         }
         string newPath = ResolvedPath.Replace(oldName, newFileNameInPath);
         if (!FileData.Clone(newPath, oldName, newFileNameInPath, alreadyCloned, execute))
         {
            return false;
         }
         if (execute)
         {
            string fileLocation = "file(" + newPath.Replace(mModule.Path + "/", "") + ")";
            ModuleFile file = new ModuleFile(Module, newAlias, fileLocation);
            file.TryLoad();
            if (file.FileData != null)
            {
               mModule.WriteToManifestJson(newAlias, fileLocation);
               return true;
            }
         } else
         {
            return true;
         }
         return false;
      }

      public string ShortName {
         get { return mShortName; }
      }
      public string FullAlias
      {
         get { return mModule.Name + ':' + mAlias; }
      }
   }
}
