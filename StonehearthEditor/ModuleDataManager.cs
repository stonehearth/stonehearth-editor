using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace StonehearthEditor
{
   public class ModuleDataManager
   {
      private static ModuleDataManager sInstance = null;
      public static ModuleDataManager GetInstance()
      {
         return sInstance;

      }
      private string mModsDirectoryPath;
      private Dictionary<String, Module> mModules = new Dictionary<String, Module>();

      private bool mHasErrors = false;
      public ModuleDataManager(string modsDirectoryPath)
      {
         mModsDirectoryPath = JsonHelper.NormalizeSystemPath(modsDirectoryPath);
         sInstance = this;
      }

      public string ModsDirectoryPath
      {
         get { return mModsDirectoryPath; }
      }

      public bool HasErrors
      {
         get
         {
            return mHasErrors;
         }

         set
         {
            mHasErrors = value;
         }
      }

      public void Load()
      {
         // Parse Manifests
         string[] modFolders = Directory.GetDirectories(mModsDirectoryPath);
         foreach (string modPath in modFolders)
         {
            string formatted = JsonHelper.NormalizeSystemPath(modPath);
            Module module = new Module(formatted);
            module.InitializeFromManifest();
            mModules.Add(module.Name, module);
         }

         foreach(Module module in mModules.Values)
         {
            module.LoadFiles();
         }

         foreach (Module module in mModules.Values)
         {
            module.PostLoadFixup();
         }
      }

      public void FilterAliasTree(TreeView treeView, string searchTerm)
      {
         treeView.BeginUpdate(); //blocks repainting tree till all objects loaded

         // filter
         treeView.Nodes.Clear();

         List<TreeNode> filteredNodes = new List<TreeNode>();
         foreach (Module module in mModules.Values)
         {
            TreeNode node = module.FilterAliasTree(searchTerm);
            if (node != null)
            {
               filteredNodes.Add(node);
            }
         }
         treeView.Nodes.AddRange(filteredNodes.ToArray());
         
         //enables redrawing tree after all objects have been added
         treeView.EndUpdate();
      }

      public FileData GetSelectedFileData(TreeNode selected)
      {
         if (selected != null)
         {
            string fullPath = selected.FullPath;
            return GetSelectedFileData(fullPath);
         }
         return null;
      }

      public bool IsModuleFileSelected(TreeNode selected)
      {
         if (selected == null)
         {
            return false;
         }
         string[] path = selected.FullPath.Split('\\');
         if (path.Length != 2)
         {
            return false;
         }
         return true;
      }

      public FileData GetSelectedFileData(string selected)
      {
         string[] path = selected.Split('\\');
         if (path.Length <= 2)
         {
            return null;
         }
         Module module = mModules[path[0]];
         ModuleFile file = module.GetModuleFile(path[1], path[2]);
         if (file != null)
         {
            return file.GetFileData(path);
         }
         return null;
      }

      public ModuleFile GetModuleFile(string fullAlias)
      {
         int indexOfColon = fullAlias.IndexOf(':');
         string module = fullAlias.Substring(0, indexOfColon);
         string alias = fullAlias.Substring(indexOfColon + 1);
         Module mod = ModuleDataManager.GetInstance().GetMod(module);
         if (mod == null)
         {
            return null;
         }
         return mod.GetAliasFile(alias);
      }


      public ICollection<Module> GetAllModules()
      {
         return mModules.Values;
      }

      public Module GetMod(string modName)
      {
         if (!mModules.ContainsKey(modName))
         {
            return null;
         }
         return mModules[modName];
      }

      public string LocalizeString(string key)
      {
         string[] split = key.Split(':');
         string modName = "stonehearth";
         if (split.Length > 1)
         {
            modName = split[0];
            key = split[1];
         }
         Module mod = GetMod(modName);
         if (mod != null)
         {
            JToken token = mod.EnglishLocalizationJson.SelectToken(key);
            if (token != null)
            {
               return token.ToString();
            }
         }
         return key;
      }

      // Call to clone an alias. top level. nested clone calls should call the module directly.
      public bool ExecuteClone(ModuleFile module, CloneObjectParameters parameters, HashSet<string> unwantedItems)
      {
         return module.Clone(parameters, unwantedItems, true);
      }

      public bool ExecuteClone(FileData file, CloneObjectParameters parameters, HashSet<string> unwantedItems)
      {
         ModuleFile owningFile = (file as IModuleFileData).GetModuleFile();
         if (owningFile != null)
         {
            return ExecuteClone(owningFile, parameters, unwantedItems);
         }
         string newPath = parameters.TransformParameter(file.Path);
         return file.Clone(newPath, parameters, unwantedItems, true);
      }

      public HashSet<string> PreviewCloneDependencies(ModuleFile module, CloneObjectParameters cloneParameters)
      {
         HashSet<string> alreadyCloned = new HashSet<string>();
         module.Clone(cloneParameters, alreadyCloned, false);
         return alreadyCloned;
      }
      public HashSet<string> PreviewCloneDependencies(FileData file, CloneObjectParameters cloneParameters)
      {
         ModuleFile owningFile = (file as IModuleFileData).GetModuleFile();
         if (owningFile != null)
         {
            return PreviewCloneDependencies(owningFile, cloneParameters);
         }
         HashSet<string> alreadyCloned = new HashSet<string>();
         string newPath = cloneParameters.TransformParameter(file.Path);

         file.Clone(newPath, cloneParameters, alreadyCloned, false);
         return alreadyCloned;
      }
   }
}
