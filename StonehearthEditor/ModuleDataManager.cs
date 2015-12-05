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
      public ModuleDataManager(string modsDirectoryPath)
      {
         mModsDirectoryPath = JsonHelper.NormalizeSystemPath(modsDirectoryPath);
         sInstance = this;
      }

      public string ModsDirectoryPath
      {
         get { return mModsDirectoryPath; }
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
            ICollection<ModuleFile> aliases = module.GetAliases();
            List<TreeNode> nodes = new List<TreeNode>();
            foreach (ModuleFile alias in aliases)
            {
               TreeNode newNode = alias.GetTreeNode(searchTerm);
               if (newNode != null)
               {
                  nodes.Add(newNode);
               }
            }
            if (nodes.Count > 0)
            {
               TreeNode treeNode = new TreeNode(module.Name, nodes.ToArray());
               treeNode.ImageIndex = 100;
               treeNode.SelectedImageIndex = 100;
               filteredNodes.Add(treeNode);
               treeNode.ExpandAll();
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
         if (path.Length <= 1)
         {
            return null;
         }
         Module module = mModules[path[0]];
         ModuleFile file = module.GetAliasFile(path[1]);
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
      public bool ExecuteClone(ModuleFile module, string newName, HashSet<string> unwantedItems)
      {
         return module.Clone(module.ShortName, newName, unwantedItems, true);
      }

      public bool ExecuteClone(FileData file, string newName, HashSet<string> unwantedItems)
      {
         ModuleFile owningFile = (file as IModuleFileData).GetModuleFile();
         if (owningFile != null)
         {
            return ExecuteClone(owningFile, newName, unwantedItems);
         }
         string newPath = file.Path.Replace(file.GetNameForCloning(), newName);
         return file.Clone(newPath, file.GetNameForCloning(), newName, unwantedItems, true);
      }

      public HashSet<string> PreviewCloneDependencies(ModuleFile module, string newName)
      {
         HashSet<string> alreadyCloned = new HashSet<string>();
         module.Clone(module.ShortName, newName, alreadyCloned, false);
         return alreadyCloned;
      }
      public HashSet<string> PreviewCloneDependencies(FileData file, string newName)
      {
         ModuleFile owningFile = (file as IModuleFileData).GetModuleFile();
         if (owningFile != null)
         {
            return PreviewCloneDependencies(owningFile, newName);
         }
         HashSet<string> alreadyCloned = new HashSet<string>();
         string newPath = file.Path.Replace(file.GetNameForCloning(), newName);

         file.Clone(newPath, file.GetNameForCloning(), newName, alreadyCloned, false);
         return alreadyCloned;
      }
   }
}
