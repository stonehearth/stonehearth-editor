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
      private List<TreeNode> mModuleTreeNodes = new List<TreeNode>();
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

         GenerateAliasTree();
      }
      private void GenerateAliasTree()
      {
         foreach (Module module in mModules.Values)
         {
            ICollection<ModuleFile> aliases = module.GetAliases();
            List<TreeNode> nodes = new List<TreeNode>();
            foreach (ModuleFile alias in aliases)
            {
               if (alias.FileType == FileType.JSON && alias.FileData != null)
               {
                  TreeNode newNode = alias.GetTreeNode();
                  nodes.Add(newNode);
               }
            }

            if (nodes.Count > 0)
            {
               TreeNode treeNode = new TreeNode(module.Name, nodes.ToArray());
               treeNode.ExpandAll();
               treeNode.ImageIndex = 100;
               treeNode.SelectedImageIndex = 100;
               mModuleTreeNodes.Add(treeNode);
            }
         }
      }

      public void FilterAliasTree(TreeView treeView, string searchTerm)
      {
         treeView.BeginUpdate(); //blocks repainting tree till all objects loaded

         // filter
         treeView.Nodes.Clear();
         if (string.IsNullOrEmpty(searchTerm))
         {
            treeView.Nodes.AddRange(mModuleTreeNodes.ToArray());
         }
         else
         {
            List<TreeNode> filteredNodes = new List<TreeNode>();
            foreach (Module module in mModules.Values)
            {
               ICollection<ModuleFile> aliases = module.GetAliases();
               List<TreeNode> nodes = new List<TreeNode>();
               foreach (ModuleFile alias in aliases)
               {
                  if (alias.Name.Contains(searchTerm))
                  {
                     JSONTYPE type = alias.FileData != null ? (alias.FileData as JsonFileData).JsonType : JSONTYPE.NONE;
                     nodes.Add(new TreeNode(alias.Name, (int)type, (int)type));
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
         }
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
      public bool CloneAlias(ModuleFile module, string newName)
      {
         HashSet<string> alreadyCloned = new HashSet<string>();
         return module.Clone(newName, alreadyCloned);
      }
   }
}
