using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StonehearthEditor
{
    public class Module : IDisposable
    {
        private string mPath;
        private string mName;
        private JObject mManifestJson;
        private FileSystemWatcher mFileWatcher;
        private DateTime mLastReadTime = DateTime.MinValue;
        private JObject mEnglishLocalizationJson;
        private bool mShowingManifestModifiedDialog = false;

        // dictionary of aliases, components, and controllers
        private Dictionary<string, Dictionary<string, ModuleFile>> mModuleFiles = new Dictionary<string, Dictionary<string, ModuleFile>>();

        public Module(string modPath)
        {
            mPath = modPath;
            mName = modPath.Substring(modPath.LastIndexOf('/') + 1);
        }

        public ICollection<ModuleFile> GetAliases()
        {
            if (!mModuleFiles.ContainsKey("aliases"))
            {
                return new List<ModuleFile>();
            }

            return mModuleFiles["aliases"].Values;
        }

        public string Name
        {
            get { return mName; }
        }

        public string Path
        {
            get { return mPath; }
        }

        public JObject EnglishLocalizationJson
        {
            get { return mEnglishLocalizationJson; }
        }

        public void InitializeFromManifest()
        {
            string modManifestPath = Path + "/manifest.json";

            if (System.IO.File.Exists(modManifestPath))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(modManifestPath, Encoding.UTF8))
                    {
                        string fileString = sr.ReadToEnd();
                        mManifestJson = JObject.Parse(fileString);

                        AddModuleFiles("aliases");
                        AddModuleFiles("aliases", "deprecated_aliases");
                        AddModuleFiles("components");
                        AddModuleFiles("controllers");
                    }

                    mFileWatcher = new FileSystemWatcher(Path, "manifest.json");
                    mFileWatcher.NotifyFilter = NotifyFilters.LastWrite;
                    mFileWatcher.Changed += new FileSystemEventHandler(OnChanged);
                    mFileWatcher.EnableRaisingEvents = true;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Failure while reading manifest file " + modManifestPath + ". Error: " + e.Message);
                }
            }

            string englishLocalizationFilePath = Path + "/locales/en.json";
            if (System.IO.File.Exists(englishLocalizationFilePath))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(englishLocalizationFilePath, Encoding.UTF8))
                    {
                        string fileString = sr.ReadToEnd();
                        mEnglishLocalizationJson = JObject.Parse(fileString);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Exception while reading localization json " + englishLocalizationFilePath + ". Error: " + e.Message);
                }
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (mShowingManifestModifiedDialog)
            {
                return;
            }
            // if the manifest has changed
            // Applications fire multiple events during the file writing process, so only show message once if there is a new write
            DateTime lastWriteTime = File.GetLastWriteTime(Path + "/manifest.json");
            if (lastWriteTime != mLastReadTime)
            {
                mShowingManifestModifiedDialog = true;
                MessageBox.Show("The manifest for module " + Name + " has changed! You should reload SHED! (Press F5)");
                mLastReadTime = lastWriteTime;
                mShowingManifestModifiedDialog = false;
            }
        }

        public void LoadFiles()
        {
            foreach (Dictionary<string, ModuleFile> dict in mModuleFiles.Values)
            {
                foreach (ModuleFile moduleFile in dict.Values)
                {
                    moduleFile.TryLoad();
                }
            }
        }

        public ModuleFile GetAliasFile(string alias)
        {
            return GetModuleFile("aliases", alias);
        }

        public ModuleFile GetModuleFile(string fileType, string alias)
        {
            ModuleFile returned = null;
            if (mModuleFiles.ContainsKey(fileType))
            {
                mModuleFiles[fileType].TryGetValue(alias, out returned);
            }

            return returned;
        }

        // Add alias to manifest under manifest file type (aliases, components, controllers)
        public void AddToManifest(string alias, string path, string manifestEntryType = "aliases")
        {
            JToken aliases = mManifestJson[manifestEntryType];
            if (aliases == null)
            {
                mManifestJson.Add(manifestEntryType, new JObject());
                aliases = mManifestJson[manifestEntryType];
            }

            JObject aliasesObject = aliases as JObject;
            if (aliasesObject.Property(alias) == null)
            {
                // Only add the alias if it doesn't already exist
                aliasesObject.Add(alias, path);
                Sort(aliasesObject);
            }
        }

        public void RemoveFromManifest(string manifestEntryType, string alias)
        {
            JObject aliases = mManifestJson[manifestEntryType] as JObject;
            JProperty aliasProperty = aliases.Property(alias);

            if (aliasProperty == null && manifestEntryType == "aliases")
            {
                aliases = mManifestJson["deprecated_aliases"] as JObject;
                aliasProperty = aliases.Property(alias);
            }

            if (aliasProperty != null)
            {
                aliasProperty.Remove();
            }
        }

        public void WriteManifestToFile()
        {
            string manifestPath = Path + "/manifest.json";
            mFileWatcher.EnableRaisingEvents = false;
            using (StreamWriter wr = new StreamWriter(manifestPath, false, new UTF8Encoding(false)))
            {
                using (JsonTextWriter jsonTextWriter = new JsonTextWriter(wr))
                {
                    jsonTextWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
                    jsonTextWriter.Indentation = 3;
                    jsonTextWriter.IndentChar = ' ';

                    JsonSerializer jsonSeralizer = new JsonSerializer();
                    jsonSeralizer.Serialize(jsonTextWriter, mManifestJson);
                }
            }
            mFileWatcher.EnableRaisingEvents = true;
        }

        public void WriteEnglishLocalizationToFile()
        {
            string englishLocalizationFilePath = Path + "/locales/en.json";
            using (StreamWriter wr = new StreamWriter(englishLocalizationFilePath, false, new UTF8Encoding(false)))
            {
                using (JsonTextWriter jsonTextWriter = new JsonTextWriter(wr))
                {
                    jsonTextWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
                    jsonTextWriter.Indentation = 3;
                    jsonTextWriter.IndentChar = ' ';

                    JsonSerializer jsonSeralizer = new JsonSerializer();
                    jsonSeralizer.Serialize(jsonTextWriter, mEnglishLocalizationJson);
                }
            }
        }

        public void PostLoadFixup()
        {
            foreach (ModuleFile moduleFile in GetAliases())
            {
                moduleFile.PostLoadFixup();
            }
        }

        public TreeNode FilterAliasTree(string searchTerm)
        {
            TreeNode root = new TreeNode(Name);
            root.ImageIndex = 100;
            root.SelectedImageIndex = 100;
            root.ExpandAll();
            bool hasItems = false;

            foreach (KeyValuePair<string, Dictionary<string, ModuleFile>> pair in mModuleFiles)
            {
                TreeNode subRoot = new TreeNode(pair.Key);

                // Expand all sub tree nodes
                subRoot.ExpandAll();

                subRoot.SelectedImageIndex = 100;
                subRoot.ImageIndex = 100;
                foreach (ModuleFile alias in pair.Value.Values)
                {
                    TreeNode newNode = alias.GetTreeNode(searchTerm);
                    if (newNode != null)
                    {
                        subRoot.Nodes.Add(newNode);
                    }
                }

                if (subRoot.Nodes.Count > 0)
                {
                    hasItems = true;
                    root.Nodes.Add(subRoot);
                }
            }

            return (searchTerm == "" || searchTerm == null || hasItems) ? root : null;
        }

        public bool IsAliasDeprecated(string alias)
        {
            JToken deprecatedAliases = mManifestJson["deprecated_aliases"];
            if (deprecatedAliases != null)
            {
                return deprecatedAliases[alias] != null;
            }

            return false;
        }

        public void Dispose()
        {
            foreach (Dictionary<string, ModuleFile> dictionary in mModuleFiles.Values)
            {
                foreach (ModuleFile moduleFile in dictionary.Values)
                {
                    moduleFile.Dispose();
                }

                dictionary.Clear();
            }

            mModuleFiles.Clear();

            if (mFileWatcher != null)
            {
                mFileWatcher.EnableRaisingEvents = false;
                mFileWatcher.Dispose();
            }
        }

        private void AddModuleFiles(string fileType, string key = null)
        {
            JToken fileTypes = mManifestJson[key ?? fileType];
            if (fileTypes != null)
            {
                Dictionary<string, ModuleFile> dictionary;
                if (mModuleFiles.ContainsKey(fileType))
                {
                    dictionary = mModuleFiles[fileType];
                }
                else
                {
                    dictionary = new Dictionary<string, ModuleFile>();
                    mModuleFiles[fileType] = dictionary;
                }

                foreach (JToken item in fileTypes.Children())
                {
                    JProperty alias = item as JProperty;
                    string name = alias.Name.Trim();
                    string value = alias.Value.ToString().Trim();

                    ModuleFile moduleFile = new ModuleFile(this, name, value);
                    moduleFile.IsDeprecated = IsAliasDeprecated(name);
                    dictionary.Add(name, moduleFile);
                }
            }
        }

        private void Sort(JObject jObj)
        {
            List<JProperty> properties = new List<JProperty>(jObj.Properties());
            foreach (var prop in properties)
            {
                prop.Remove();
            }

            properties.Sort((a, b) => a.Name.CompareTo(b.Name));

            foreach (var prop in properties)
            {
                jObj.Add(prop);
            }
        }
    }
}
