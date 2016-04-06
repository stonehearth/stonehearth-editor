using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace StonehearthEditor
{
    public class Module : IDisposable
    {
        private string mPath;
        private string mName;
        private JObject mManifestJson;
        private Dictionary<string, Dictionary<string, ModuleFile>> mModuleFiles = new Dictionary<string, Dictionary<string, ModuleFile>>();

        private JObject mEnglishLocalizationJson;

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
            string stonehearthModManifest = Path + "/manifest.json";

            if (System.IO.File.Exists(stonehearthModManifest))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(stonehearthModManifest, Encoding.UTF8))
                    {
                        string fileString = sr.ReadToEnd();
                        mManifestJson = JObject.Parse(fileString);

                        AddModuleFiles("aliases");
                        AddModuleFiles("components");
                        AddModuleFiles("controllers");
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Failure while reading manifest file " + stonehearthModManifest + ". Error: " + e.Message);
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

        private void AddModuleFiles(string fileType)
        {
            JToken fileTypes = mManifestJson[fileType];
            if (fileTypes != null)
            {
                Dictionary<string, ModuleFile> dictionary = new Dictionary<string, ModuleFile>();
                mModuleFiles[fileType] = dictionary;
                foreach (JToken item in fileTypes.Children())
                {
                    JProperty alias = item as JProperty;
                    string name = alias.Name.Trim();
                    string value = alias.Value.ToString().Trim();

                    ModuleFile moduleFile = new ModuleFile(this, name, value);
                    dictionary.Add(name, moduleFile);
                }
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

        public void AddToManifest(string alias, string path)
        {
            JToken aliases = mManifestJson["aliases"];
            if (aliases == null)
            {
                mManifestJson.Add("aliases", null);
                aliases = mManifestJson["aliases"];
            }

            JObject aliasesObject = aliases as JObject;
            if (aliasesObject.Property(alias) == null)
            {
                // Only add the alias if it doesn't already exist
                aliasesObject.Add(alias, path);
                Sort(aliasesObject);
            }
        }

        public void RemoveFromManifest(string parent, string alias)
        {
            JObject aliases = mManifestJson[parent] as JObject;
            JProperty aliasProperty = aliases.Property(alias);
            if (aliasProperty != null)
            {
                aliasProperty.Remove();
            }
        }

        public void WriteManifestToFile()
        {
            string manifestPath = Path + "/manifest.json";
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
                if (pair.Key == "aliases")
                {
                    subRoot.ExpandAll();
                }

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

            return hasItems ? root : null;
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
        }
    }
}
