using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

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
      private JsonFileData mFileData = null;
      private FileType mType = FileType.UNKNOWN;

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

      public List<ModuleFile> GetDependencies()
      {
         if (mFileData != null)
         {
            return mFileData.LinkedAliases;
         }
         return null;
      }

      public void FillDependencyListItems(ListView listView)
      {
         listView.Items.Clear();
         List<ModuleFile> dependencies = GetDependencies();
         if (dependencies != null)
         {
            foreach(ModuleFile dependency in dependencies)
            {
               listView.Items.Add(dependency.Module.Name + ":" + dependency.Name);
            }
         }
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
      public JsonFileData FileData
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
