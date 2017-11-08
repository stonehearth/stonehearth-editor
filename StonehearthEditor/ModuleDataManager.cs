using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static bool IsBaseMod(string modName)
        {
            return modName == "stonehearth" || modName == "rayyas_children";
        }

        private string mModsDirectoryPath;
        private Dictionary<string, Module> mModules = new Dictionary<string, Module>();

        private HashSet<FileData> mFilesWithErrors = new HashSet<FileData>();
        private HashSet<FileData> mModifiedFiles = new HashSet<FileData>();

        private Dictionary<string, int> mAverageMaterialCost = new Dictionary<string, int>();
        public HashSet<string> StockpileFilters = new HashSet<string>();

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

        public void PopulateTreeView(TreeView treeView, string alias, string folder)
        {
            treeView.BeginUpdate();
            treeView.Nodes.Clear();

            List<TreeNode> effects = new List<TreeNode>();
            if (alias != string.Empty)
            {
                AddTreeNodesByAlias(effects, alias);
            }

            AddTreeNodesByFolder(effects, folder);

            treeView.Nodes.AddRange(effects.ToArray());
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
                string modName = System.IO.Path.GetFileName(modPath);
                TreeNode modNode = new TreeNode(modName);
                bool hasChildren = false;
                string searchDirectoryPath = JsonHelper.NormalizeSystemPath(modPath) + folderName;
                if (Directory.Exists(searchDirectoryPath))
                {
                    // check all the folders
                    foreach (string folderPath in Directory.EnumerateDirectories(searchDirectoryPath))
                    {
                        hasChildren = true;
                        string rootFolderName = System.IO.Path.GetFileName(folderPath);
                        TreeNode root = new TreeNode(rootFolderName);
                        modNode.Nodes.Add(root); // Add effect folder node under mod name node
                        AppendTreeNodes(root, folderPath); // Append tree nodes from nested folders and files
                        root.ExpandAll(); // Expand the top level
                    }
                }

                if (hasChildren)
                {
                    treeNodes.Add(modNode);
                    modNode.ExpandAll();
                }
            }
        }

        // Adds all files and directories in path to tree node root
        private void AppendTreeNodes(TreeNode root, string rootPath)
        {
            string[] filePaths = Directory.GetFiles(rootPath);
            string[] folderPaths = Directory.GetDirectories(rootPath);

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

            foreach (string folderPath in folderPaths)
            {
                TreeNode subRoot = new TreeNode(System.IO.Path.GetFileName(folderPath));
                subRoot.Tag = JsonHelper.NormalizeSystemPath(folderPath);
                AppendTreeNodes(subRoot, folderPath);
                root.Nodes.Add(subRoot);
            }
        }

        public Dictionary<string, JsonFileData> GetJsonsByTerm(string filterTerm, bool baseModsOnly = false)
        {
            Dictionary<string, JsonFileData> aliasJsonMap = new Dictionary<string, JsonFileData>();
            foreach (Module module in mModules.Values)
            {
                if (baseModsOnly && !IsBaseMod(module.Name))
                {
                    continue;
                }

                foreach (ModuleFile moduleFile in module.GetAliases())
                {
                    JsonFileData data = moduleFile.GetJsonFileDataByTerm(filterTerm);
                    if (data != null)
                    {
                        aliasJsonMap.Add(moduleFile.FullAlias, data);
                    }
                }
            }

            return aliasJsonMap;
        }

        public Dictionary<string, JsonFileData> GetJsonsOfType(JSONTYPE jsonType, bool baseModsOnly = false)
        {
            Dictionary<string, JsonFileData> aliasJsonMap = new Dictionary<string, JsonFileData>();
            foreach (Module module in mModules.Values)
            {
                if (baseModsOnly && !IsBaseMod(module.Name))
                {
                    continue;
                }

                foreach (ModuleFile moduleFile in module.GetAliases())
                {
                    JsonFileData data = moduleFile.FileData as JsonFileData;
                    if (data != null && data.JsonType == jsonType)
                    {
                        aliasJsonMap.Add(moduleFile.FullAlias, data);
                    }
                }
            }

            return aliasJsonMap;
        }

        public Dictionary<string, JsonFileData> GetIconicJsons(bool baseModsOnly = false)
        {
            Dictionary<string, JsonFileData> entityJsonFiles = GetJsonsOfType(JSONTYPE.ENTITY, baseModsOnly);
            Dictionary<string, JsonFileData> iconicJsonFiles = new Dictionary<string, JsonFileData>();

            foreach (KeyValuePair<string, JsonFileData> entry in entityJsonFiles)
            {
                JsonFileData jsonFileData = entry.Value;
                JToken iconicForm = jsonFileData.Json.SelectToken("components.stonehearth:entity_forms.iconic_form");
                if (iconicForm != null)
                {

                    string iconicFilePath = JsonHelper.GetFileFromFileJson(iconicForm.ToString(), jsonFileData.Directory);
                    iconicFilePath = JsonHelper.NormalizeSystemPath(iconicFilePath);

                    JsonFileData iconicFileData = (JsonFileData)jsonFileData.OpenedFiles.Find(e => e is JsonFileData && e.Path == iconicFilePath);
                    if (iconicFileData != null)
                    {
                        iconicJsonFiles.Add(iconicFilePath, iconicFileData);
                    }
                }
            }

            return iconicJsonFiles;
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
            // FullPath is a property of treeviews that consists of the labels of all of the
            // tree nodes that must be navigated to reach the selected tree node
            string[] path = selected.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (path.Length <= 2 || !mModules.ContainsKey(path[0]))
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

            ModuleFile result;
            result = mod.GetModuleFile("aliases", alias);
            if (result != null)
            {
                return result;
            }

            result = mod.GetModuleFile("components", alias);
            if (result != null)
            {
                return result;
            }

            result = mod.GetModuleFile("controllers", alias);
            if (result != null)
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Attempts to resolve a reference.
        /// </summary>
        /// <param name="path">Reference that was given</param>
        /// <param name="parentDirectory">Parent directory, required to resolve relative paths.</param>
        /// <param name="moduleFile">The file that was found.</param>
        /// <returns><c>true</c> if the file was found, <c>false</c> otherwise.</returns>
        public bool TryGetModuleFile(string path, string parentDirectory, out ModuleFile moduleFile)
        {
            moduleFile = null;

            // Alias?
            if (path.Contains(":"))
            {
                moduleFile = this.GetModuleFile(path);
                return true;
            }
            else
            {
                var fileName = JsonHelper.GetFileFromFileJson(path, parentDirectory);

                // Is it a valid filename?
                if (!File.Exists(fileName))
                    return false; // Not a valid file => we don't stand a chance to begin with

                // Cut away the unrequired bits
                var simplifiedFileName = fileName.Replace(ModuleDataManager.GetInstance().ModsDirectoryPath, "").TrimStart(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);

                // Split it into mod/path within mod
                var parts = simplifiedFileName.Split(new[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar }, 2);

                var mod = ModuleDataManager.GetInstance().GetMod(parts[0]);

                if (mod == null)
                    return false;

                // The file exists, but isn't a global alias, so create a temporary entry so the caller can access it.
                moduleFile = new ModuleFile(mod, "<no-alias>", parts[1]);
                return true;

                ////// Get all aliases that match this file
                ////var aliases = mod.GetAliases().Where(alias => alias.ResolvedPath == fileName).ToList();
                ////return null;
            }
        }

        public ICollection<Module> GetAllModules()
        {
            return mModules.Values;
        }

        public string[] GetAllModuleNames()
        {
            List<string> tempList = new List<string>();

            foreach (Module module in mModules.Values)
            {
                tempList.Add(module.Name);
            }

            string[] modNames = tempList.ToArray();
            return modNames;
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
            string locPrefix = "i18n(";
            // Strip the i18n() from the key if it's there
            if (key.Contains(locPrefix))
            {
                int i18nLength = locPrefix.Length;
                key = key.Substring(i18nLength, key.Length - i18nLength - 1);
            }

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
                try
                {
                    JToken token = mod.EnglishLocalizationJson.SelectToken(key);
                    if (token != null)
                    {
                        return token.ToString();
                    }
                }
                catch (Exception)
                {
                    // A regular string that contains characters not allowed in XPaths, like space.
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
            // Code will only get here if owningFile is null, so calling the below will cause a null ref exception when indexing the owningFile
            // .Replace(owningFile.Module.Name, cloneParameters.TargetModule);

            file.Clone(newPath, cloneParameters, alreadyCloned, false);
            return alreadyCloned;
        }

        public bool ContainsStockpileMaterial(string tokens)
        {
            string[] split = tokens.Split(' ');
            foreach (string stockpileFilter in StockpileFilters)
            {
                foreach (string material in split)
                {
                    if (material.Equals(stockpileFilter))
                    {
                        return true;
                    }
                }
            }

            return false;
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

        public string GetModNameFromAlias(string fullAlias)
        {
            int indexOfColon = fullAlias.IndexOf(':');
            string modName = fullAlias.Substring(0, indexOfColon);
            if (GetMod(modName) == null)
            {
                throw new Exception("Input string does not have a valid mod name before the first colon");
            }

            return modName;
        }

        public bool ChangeEnglishLocValue(string locKey, string newValue)
        {
            string key = locKey;
            string[] split = key.Split(':');
            string modName = "stonehearth"; // default mod name

            if (split.Length > 1)
            {
                modName = split[0];
                key = split[1];
            }

            Module mod = GetMod(modName);
            if (mod == null)
            {
                MessageBox.Show("Could not change loc key. There is no mod named " + modName);
                return true;
            }

            JValue token = mod.EnglishLocalizationJson.SelectToken(key) as JValue;
            if (token == null)
            {
                // No such localization key. Try to add it.
                string[] tokenSplit = key.Split('.');
                JObject parent = mod.EnglishLocalizationJson;
                for (int i = 0; i < tokenSplit.Length - 1; ++i)
                {
                    if (parent == null)
                    {
                        break;
                    }

                    if (parent[tokenSplit[i]] == null)
                    {
                        parent[tokenSplit[i]] = new JObject();
                    }

                    parent = parent[tokenSplit[i]] as JObject;
                }

                if (parent == null)
                {
                    MessageBox.Show("Could not insert localization token " + locKey);
                    return true;
                }

                parent.Add(tokenSplit[tokenSplit.Length - 1], newValue);
            }
            else
            {
                token.Value = newValue;
            }

            mod.WriteEnglishLocalizationToFile();
            return true;
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
