using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace StonehearthEditor
{
    public abstract class FileData : IDisposable
    {
        protected TreeNode mTreeNode { get; set; }

        private string mFlatFileData;
        private bool isDisposing = false;
        private string mErrors = null;

        public List<ModuleFile> LinkedAliases { get; } = new List<ModuleFile>();

        public Dictionary<string, FileData> LinkedFileData { get; } = new Dictionary<string, FileData>();

        public List<FileData> OpenedFiles { get; } = new List<FileData>();

        public List<FileData> RelatedFiles { get; } = new List<FileData>();

        public Dictionary<string, FileData> ReferencedByFileData { get; } = new Dictionary<string, FileData>();

        protected FileData(string path)
        {
            this.Path = path;
        }

        public TreeNode TreeNode
        {
            get { return mTreeNode; }
        }

        public string Path { get; }

        public string FileName
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(Path); }
        }

        public virtual string Errors
        {
            get { return mErrors; }
        }

        public virtual bool HasErrors
        {
            get { return mErrors != null; }
        }

        public virtual void AddError(string error)
        {
            mErrors = mErrors + error + "\n";
        }

        public virtual bool UpdateTreeNode(TreeNode node, string filter)
        {
            if (HasErrors)
            {
                node.SelectedImageIndex = 0;
                node.ImageIndex = 0;
                node.ToolTipText = Errors;
            }

            return true;
        }

        public bool TrySetFlatFileData(string newData)
        {
            string newFlatFileData;
            if (TryChangeFlatFileData(newData, out newFlatFileData))
            {
                mFlatFileData = newFlatFileData;
                ModuleDataManager.GetInstance().ModifiedFiles.Add(this);
                return true;
            }

            return false;
        }

        public void TrySaveFile()
        {
            if (ModuleDataManager.GetInstance().ModifiedFiles.Contains(this))
            {
                try
                {
                    using (StreamWriter wr = new StreamWriter(Path, false, new UTF8Encoding(false)))
                    {
                        wr.Write(FlatFileData);
                    }

                    ModuleDataManager.GetInstance().ModifiedFiles.Remove(this);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not write to file " + Path + " because of exception: " + e.Message);
                }
            }
        }

        public void FillDependencyListItems(ListBox listView)
        {
            listView.Items.Clear();
            foreach (string dependency in GetDependencies().Keys)
            {
                listView.Items.Add(dependency);
            }
        }

        public void FillReferencesListItems(ListBox listView)
        {
            listView.Items.Clear();
            foreach (string reference in ReferencedByFileData.Keys)
            {
                listView.Items.Add(reference);
            }
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
            else
            {
                AddError("File " + Path + " does not exist.");
            }

            PostLoad();
        }

        public string FlatFileData
        {
            get { return mFlatFileData; }
        }

        public virtual bool ShouldCloneDependency(string dependencyName, CloneObjectParameters parameters)
        {
            return parameters.IsDependency(dependencyName);
        }

        public virtual string GetNameForCloning()
        {
            return FileName;
        }

        public override string ToString()
        {
            return Path;
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
        public virtual bool Clone(string newPath, CloneObjectParameters parameters, HashSet<string> alreadyCloned, bool execute)
        {
            // Ensure directory exists
            string directory = System.IO.Path.GetDirectoryName(newPath);
            alreadyCloned.Add(newPath);
            if (execute)
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            // Figure out what dependency files need to exist
            foreach (KeyValuePair<string, FileData> dependencyKV in GetDependencies())
            {
                string dependencyName = dependencyKV.Key;
                FileData dependencyFile = dependencyKV.Value;
                if (ShouldCloneDependency(dependencyName, parameters))
                {
                    // We want to clone this dependency
                    IModuleFileData modFileData = dependencyFile as IModuleFileData;
                    if (modFileData != null && modFileData.GetModuleFile() != null)
                    {
                        // This dependency is an alias. Clone the alias.
                        ModuleFile linkedAlias = modFileData.GetModuleFile();
                        string aliasNewName = parameters.TransformAlias(dependencyName);
                        if (!alreadyCloned.Contains(aliasNewName))
                        {
                            alreadyCloned.Add(aliasNewName);
                            linkedAlias.Clone(parameters, alreadyCloned, execute);
                        }
                    }
                    else
                    {
                        // This dependency is just a FileData, clone the fileData.
                        string linkedPath = ModuleDataManager.GetInstance().ModsDirectoryPath + dependencyName;
                        string newDependencyPath = ModuleDataManager.GetInstance().ModsDirectoryPath + parameters.TransformParameter(dependencyName);
                        if (!alreadyCloned.Contains(newDependencyPath))
                        {
                            alreadyCloned.Add(newDependencyPath);
                            dependencyFile.Clone(newDependencyPath, parameters, alreadyCloned, execute);
                        }
                    }
                }
            }

            if (execute)
            {
                string newFlatFile = parameters.TransformParameter(FlatFileData);
                using (StreamWriter wr = new StreamWriter(newPath, false, new UTF8Encoding(false)))
                {
                    wr.Write(newFlatFile);
                }
            }

            return true;
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

        // custom load call
        protected virtual void LoadInternal()
        {
        }

        protected virtual void PostLoad()
        {
        }

        protected virtual bool TryChangeFlatFileData(string newData, out string newFlatFileData)
        {
            newFlatFileData = newData;
            return true;
        }

        protected FileData GetLinkedFileData(string path)
        {
            foreach (FileData data in OpenedFiles)
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

#pragma warning disable SA1202 // Elements must be ordered by access
        public void Dispose()
#pragma warning restore SA1202 // Elements must be ordered by access
        {
            if (isDisposing)
            {
                return;
            }

            isDisposing = true;
            foreach (ModuleFile alias in LinkedAliases)
            {
                alias.Dispose();
            }

            LinkedAliases.Clear();

            foreach (FileData opened in OpenedFiles)
            {
                opened.Dispose();
            }

            OpenedFiles.Clear();

            foreach (FileData related in RelatedFiles)
            {
                related.Dispose();
            }

            RelatedFiles.Clear();

            foreach (FileData referenced in ReferencedByFileData.Values)
            {
                referenced.Dispose();
            }

            ReferencedByFileData.Clear();
        }
    }
}
