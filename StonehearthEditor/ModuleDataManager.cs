using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace StonehearthEditor
{
    public class ModuleDataManager : IDisposable
    {
        private static ModuleDataManager sInstance = null;

        public static ModuleDataManager GetInstance()
        {
            return sInstance;
        }

        private string mModsDirectoryPath;
        private Dictionary<string, Module> mModules = new Dictionary<string, Module>();

        private HashSet<FileData> mFilesWithErrors = new HashSet<FileData>();
        private HashSet<FileData> mModifiedFiles = new HashSet<FileData>();

        private Dictionary<string, int> mAverageMaterialCost = new Dictionary<string, int>();

        public ModuleDataManager(string modsDirectoryPath)
        {
            if (sInstance != null)
            {
                sInstance.Dispose();
                sInstance = null;
            }

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
                return mFilesWithErrors.Count > 0;
            }
        }

        public void AddErrorFile(FileData fileWithError)
        {
            mFilesWithErrors.Add(fileWithError);
        }

        public void SaveModifiedFiles()
        {
            HashSet<FileData> copy = new HashSet<FileData>(ModifiedFiles);
            foreach (FileData fileData in copy)
            {
                fileData.TrySaveFile();
            }
            ModifiedFiles.Clear();
        }

        public HashSet<FileData> GetErrorFiles()
        {
            return mFilesWithErrors;
        }

        public void Load()
        {
            // Parse Manifests
            string[] modFolders = Directory.GetDirectories(mModsDirectoryPath);
            if (modFolders == null)
            {
                return;
            }

            foreach (string modPath in modFolders)
            {
                string formatted = JsonHelper.NormalizeSystemPath(modPath);
                Module module = new Module(formatted);
                module.InitializeFromManifest();
                mModules.Add(module.Name, module);
            }

            foreach (Module module in mModules.Values)
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
            treeView.BeginUpdate(); // blocks repainting tree till all objects loaded

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

            // enables redrawing tree after all objects have been added
            treeView.EndUpdate();
        }

      public void LoadEffectsList(TreeView treeView)
      {
         treeView.BeginUpdate();
         treeView.Nodes.Clear();

         List<TreeNode> effects = new List<TreeNode>();

         AddTreeNodesByAlias(effects, "effects");
         AddTreeNodesByFolder(effects, "/data/effects");

         treeView.Nodes.AddRange(effects.ToArray());
         treeView.EndUpdate();
      }

      public void LoadCubemittersList(TreeView treeView)
      {
         treeView.BeginUpdate();
         treeView.Nodes.Clear();

         List<TreeNode> cubemitters = new List<TreeNode>();

         AddTreeNodesByAlias(cubemitters, "cubemitters");
         AddTreeNodesByFolder(cubemitters, "/data/horde/particles");

         treeView.Nodes.AddRange(cubemitters.ToArray());
         treeView.EndUpdate();
      }

      // Add tree nodes for aliases that contain the filter term
      private void AddTreeNodesByAlias(List<TreeNode> treeNodes, string filterTerm)
      {
         foreach (Module module in mModules.Values)
         {
            TreeNode node = module.FilterAliasTree(filterTerm);
            if (node != null)
            {
               treeNodes.Add(node);
            }
         }
      }

      // Add tree nodes in the specified folder (inside mods folder)
      private void AddTreeNodesByFolder(List<TreeNode> treeNodes, string folderName)
      {
         string[] modFolders = Directory.GetDirectories(mModsDirectoryPath);
         if (modFolders == null)
         {
            return;
         }

         // check all the mod folders
         foreach (string modPath in modFolders)
         {
            string searchDirectoryPath = JsonHelper.NormalizeSystemPath(modPath) + folderName;
            if (Directory.Exists(searchDirectoryPath))
            {
               // check all the folders
               foreach (string folderPath in Directory.EnumerateDirectories(searchDirectoryPath))
               {
                  string rootFolderName = System.IO.Path.GetFileName(folderPath);
                  TreeNode root = new TreeNode(rootFolderName);
                  root.ExpandAll();
                  // Append tree nodes from nested folders and files
                  AppendTreeNodes(root, folderPath);
                  treeNodes.Add(root);
               }
            }
         }
      }

      // Adds all files and directories in path to tree node root
      private void AppendTreeNodes(TreeNode root, string rootPath)
      {
         string[] filePaths = Directory.GetFiles(rootPath);
         string[] folderPaths = Directory.GetDirectories(rootPath);

         if (filePaths != null)
         {
            foreach (string filePath in filePaths)
            {
               if (root.Tag == null)
               {
                  root.Tag = JsonHelper.NormalizeSystemPath(filePath);
               }
               TreeNode node = new TreeNode(System.IO.Path.GetFileName(filePath));
               node.Tag = JsonHelper.NormalizeSystemPath(filePath);
               root.Nodes.Add(node);
            }
         }
         if (folderPaths != null)
         {
            foreach (string folderPath in folderPaths)
            {
               TreeNode subRoot = new TreeNode(System.IO.Path.GetFileName(folderPath));
               subRoot.Tag = JsonHelper.NormalizeSystemPath(folderPath);
               AppendTreeNodes(subRoot, folderPath);
            }
         }
      }

      // Returns an Object array with a map from alias to jsonfiledata and alias to modname
      public Object[] FilterJsonByTerm(ListView listView, string filterTerm)
      {
         Dictionary<string, JsonFileData> aliasJsonMap = new Dictionary<string, JsonFileData>();
         Dictionary<string, string> aliasModNameMap = new Dictionary<string, string>();
         foreach (Module module in mModules.Values)
         {
            foreach (ModuleFile moduleFile in module.GetAliases())
            {
               JsonFileData data = moduleFile.GetJsonFileDataByTerm(filterTerm);
               if (data != null)
               {
                  aliasJsonMap.Add(moduleFile.FullAlias, data);
                  aliasModNameMap.Add(moduleFile.FullAlias, module.Name);
               }
            }
         }

         return new object[] { aliasJsonMap, aliasModNameMap };
        }

        public object[] GetJsonsOfType(ListView listView, JSONTYPE jsonType)
        {
            Dictionary<string, JsonFileData> aliasJsonMap = new Dictionary<string, JsonFileData>();
            Dictionary<string, string> aliasModNameMap = new Dictionary<string, string>();
            foreach (Module module in mModules.Values)
            {
                foreach (ModuleFile moduleFile in module.GetAliases())
                {
                    JsonFileData data = moduleFile.FileData as JsonFileData;
                    if (data != null && data.JsonType == jsonType)
                    {
                        aliasJsonMap.Add(moduleFile.FullAlias, data);
                        aliasModNameMap.Add(moduleFile.FullAlias, module.Name);
                    }
                }
            }

            return new Object[] { aliasJsonMap, aliasModNameMap };
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

        public bool HasLocalizationKey(string key)
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
                    return true;
                }
            }

            return false;
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

        public int GetAverageMaterialCost(string material)
        {
            if (mAverageMaterialCost.ContainsKey(material))
            {
                return mAverageMaterialCost[material];
            }

            int sumCost = 0;
            int numItems = 0;
            string[] split = material.Split(' ');
            foreach (Module mod in ModuleDataManager.GetInstance().GetAllModules())
            {
                foreach (ModuleFile file in mod.GetAliases())
                {
                    JsonFileData data = file.FileData as JsonFileData;
                    if (data == null)
                    {
                        continue;
                    }

                    int netWorth = data.NetWorth;
                    if (netWorth <= 0)
                    {
                        continue;
                    }

                    JToken tags = data.Json.SelectToken("components.stonehearth:material.tags");
                    if (tags != null)
                    {
                        string tagString = tags.ToString();
                        string[] currentTagSplit = tagString.Split(' ');
                        HashSet<string> currentTagSet = new HashSet<string>(currentTagSplit);
                        bool isMaterial = true;
                        foreach (string tag in split)
                        {
                            if (!currentTagSet.Contains(tag))
                            {
                                isMaterial = false;
                                break;
                            }
                        }

                        if (isMaterial)
                        {
                            numItems++;
                            sumCost = sumCost + netWorth;
                        }
                    }
                }
            }

            if (numItems > 0)
            {
                int averageCost = sumCost / numItems;
                mAverageMaterialCost[material] = averageCost;
                return averageCost;
            }

            return 0;
        }

        // ModifiedFiles is way too large, and most files are getting re-saved without any changes
        // Leave getter here so we can breakpoint
        public HashSet<FileData> ModifiedFiles
        {
            get { return mModifiedFiles; }
        }

        public void Dispose()
        {
            foreach (Module module in mModules.Values)
            {
                module.Dispose();
            }

            mModules.Clear();
            mFilesWithErrors.Clear();
            mAverageMaterialCost.Clear();
        }
    }
}
