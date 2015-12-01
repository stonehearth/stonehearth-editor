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
      protected TreeNode mTreeNode;
      private string mFlatFileData;
      protected bool mIsModified = false;

      protected string mPath;
      protected List<ModuleFile> mLinkedAliases = new List<ModuleFile>();
      protected Dictionary<string, FileData> mLinkedFileData = new Dictionary<string, FileData>();
      protected List<FileData> mOpenedFiles = new List<FileData>();
      protected List<FileData> mRelatedFiles = new List<FileData>();

      public List<ModuleFile> LinkedAliases { get { return mLinkedAliases; } }
      public Dictionary<string, FileData> LinkedFileData { get { return mLinkedFileData; } }
      public List<FileData> OpenedFiles { get { return mOpenedFiles; } }
      public List<FileData> RelatedFiles { get { return mRelatedFiles; } }

      public TreeNode TreeNode {
         get { return mTreeNode; }
      }

      public string Path { get { return mPath; } }

      public string FileName
      {
         get { return System.IO.Path.GetFileNameWithoutExtension(Path); }
      }
      public bool IsModified
      {
         get { return mIsModified; }
      }
      public abstract bool UpdateTreeNode(TreeNode node, string filter);
      public void TrySetFlatFileData(string newData)
      {
         if (TryChangeFlatFileData(newData))
         {
            mFlatFileData = newData;
            mIsModified = true;
         }
      }
      public void TrySaveFile() {
         if (mIsModified)
         {
            try
            {
               using (StreamWriter wr = new StreamWriter(Path, false, new UTF8Encoding(false)))
               {
                  wr.Write(FlatFileData);
               }
               mIsModified = false;
            }
            catch (Exception e)
            {
               Console.WriteLine("Could not write to file " + Path + " because of exception: " + e.Message);
            }
         }
      }
      public void FillDependencyListItems(ListView listView)
      {
         listView.Items.Clear();
         foreach(string dependency in GetDependencies().Keys)
         {
            listView.Items.Add(dependency);
         }
      }

      protected Dictionary<string, FileData> GetDependencies()
      {
         Dictionary<string, FileData> dependencies = new Dictionary<string, FileData>();
         foreach (ModuleFile dependency in LinkedAliases)
         {
            string alias = dependency.Module.Name + ":" + dependency.Name;
            if (!dependencies.ContainsKey(alias))
            {
               dependencies.Add(alias, dependency.FileData);
            }
         }
         foreach (KeyValuePair<string, FileData> file in LinkedFileData)
         {
            string filePathWithoutBase = file.Key.Replace(ModuleDataManager.GetInstance().ModsDirectoryPath, "");
            if (!dependencies.ContainsKey(filePathWithoutBase))
            {
               dependencies.Add(filePathWithoutBase, file.Value);
            }
         }
         return dependencies;
      }

      public virtual void Load()
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
      protected FileData GetLinkedFileData(string path)
      {
         foreach(FileData data in OpenedFiles)
         {
            if (data.Path == path)
            {
               return data;
            }
         }
         foreach (FileData data in RelatedFiles)
         {
            if (data.Path == path)
            {
               return data;
            }
         }
         return null;
      }

      public virtual bool ShouldCloneDependency(string dependencyName, string oldName)
      {
         return dependencyName.Contains(oldName);
      }

      public virtual string GetNameForCloning()
      {
         return FileName;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="newPath"></param>
      /// <param name="oldName"></param>
      /// <param name="newFileName"></param>
      /// <param name="alreadyCloned"></param>
      /// <param name="execute">whether to actual execute the clone. otherwise, this is just a preview</param>
      /// <returns></returns>
      public virtual bool Clone(string newPath, string oldName, string newFileName, HashSet<string> alreadyCloned, bool execute)
      {
         //Ensure directory exists
         string directory = System.IO.Path.GetDirectoryName(newPath);
         alreadyCloned.Add(newPath);
         if (execute)
         {
            System.IO.Directory.CreateDirectory(directory);
         }
         // Figure out what dependency files need to exist
         foreach(KeyValuePair<string, FileData> dependencyKV in GetDependencies())
         {
            string dependencyName = dependencyKV.Key;
            FileData dependencyFile = dependencyKV.Value;
            if (ShouldCloneDependency(dependencyName, oldName))
            {
               // We want to clone this dependency
               IModuleFileData modFileData = dependencyFile as IModuleFileData;
               if (modFileData != null && modFileData.GetModuleFile() != null)
               {
                  // This dependency is an alias. Clone the alias.
                  ModuleFile linkedAlias = modFileData.GetModuleFile();
                  string aliasNewName = dependencyName.Replace(oldName, newFileName);
                  if (!alreadyCloned.Contains(aliasNewName))
                  {
                     alreadyCloned.Add(aliasNewName);
                     linkedAlias.Clone(oldName, newFileName, alreadyCloned, execute);
                  }
               } else
               {
                  // This dependency is just a FileData, clone the fileData.
                  string linkedPath = ModuleDataManager.GetInstance().ModsDirectoryPath + dependencyName;
                  string newDependencyPathName = newFileName;
                  string newDependencyPath = ModuleDataManager.GetInstance().ModsDirectoryPath + dependencyName.Replace(oldName, newDependencyPathName);
                  if (!alreadyCloned.Contains(newDependencyPath))
                  {
                     alreadyCloned.Add(newDependencyPath);
                     dependencyFile.Clone(newDependencyPath, oldName, newDependencyPathName, alreadyCloned, execute);
                  }
               }
            }
         }

         if (execute)
         {
            string newFlatFile = FlatFileData.Replace(oldName, newFileName);
            using (StreamWriter wr = new StreamWriter(newPath, false, new UTF8Encoding(false)))
            {
               wr.Write(newFlatFile);
            }
         }
         return true;
      }
   }
}
