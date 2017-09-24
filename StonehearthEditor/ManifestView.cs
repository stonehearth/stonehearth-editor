using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace StonehearthEditor
{
    public partial class ManifestView : UserControl, IReloadable, IFileDataSelectable
    {
        private const int kThumbnailSize = 20;
        private FileData mSelectedFileData = null;
        private Dictionary<string, string> mLastModuleLocations = new Dictionary<string, string>();
        private ErrorFileList mErrorFileListView;

        public ManifestView()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            ThumbnailCache.ClearCache();
            new ModuleDataManager(MainForm.kModsDirectoryPath);
            ModuleDataManager.GetInstance().Load();
            ModuleDataManager.GetInstance().FilterAliasTree(treeView, null);
            searchBox.BackColor = SystemColors.Window;

            if (ModuleDataManager.GetInstance().HasErrors)
            {
                toolStripStatusLabel1.Text = "Errors were encountered while loading manifests";
                toolStripStatusLabel1.ForeColor = Color.Red;
            }
            else
            {
                toolStripStatusLabel1.Text = "Successfully loaded manifests";
                toolStripStatusLabel1.ForeColor = Color.Black;
            }
        }

        private void aliasContextMenuDuplicate_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView.SelectedNode;
            FileData selectedFileData = ModuleDataManager.GetInstance().GetSelectedFileData(treeView.SelectedNode);
            if (selectedFileData == null)
            {
                return;
            }

            IModuleFileData moduleFile = selectedFileData as IModuleFileData;
            if (moduleFile == null)
            {
                return; // Don't know how to clone something not module file data
            }

            string manifestEntryType = selectedNode.Parent.Text;
            string name = moduleFile.GetModuleFile() != null ? moduleFile.GetModuleFile().FullAlias : selectedFileData.FileName;
            CloneAliasCallback callback = new CloneAliasCallback(this, selectedFileData, manifestEntryType);
            CloneDialog dialog = new CloneDialog(name, selectedFileData.GetNameForCloning());
            dialog.SetCallback(callback);
            dialog.ShowDialog();
        }

        private void openFileButton_Click(object sender, EventArgs eventArgs)
        {
            // open file
            Button button = sender as Button;
            string filePath = button.Name;
            if (filePath.EndsWith(".qmo") || filePath.EndsWith(".qb"))
            {
                // Find qubicle constructor.ini
                string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string qcIni = myDocuments + "/QubicleConstructor/1.0/QubicleConstructor.ini";
                if (System.IO.File.Exists(qcIni))
                {
                    try
                    {
                        string directory = System.IO.Path.GetDirectoryName(filePath);
                        string qcFile = "";
                        using (StreamReader sr = new StreamReader(qcIni))
                        {
                            qcFile = sr.ReadToEnd();
                        }

                        int folderIndex = qcFile.IndexOf("[Folder]");
                        string beforeFolder = "";
                        string afterFolder = string.Empty;
                        if (folderIndex >= 0)
                        {
                            beforeFolder = qcFile.Substring(0, folderIndex);
                            string folderString = qcFile.Substring(folderIndex + 8);
                            int endOfFolderString = folderString.IndexOf('[');
                            if (endOfFolderString >= 0)
                            {
                                afterFolder = folderString.Substring(endOfFolderString);
                            }
                        }
                        else
                        {
                            beforeFolder = qcFile;
                        }

                        StringBuilder newQcFile = new StringBuilder();
                        newQcFile.AppendLine(beforeFolder);
                        newQcFile.AppendLine("[Folder]");
                        newQcFile.AppendLine("Open=" + directory);
                        newQcFile.AppendLine("Save=" + directory);
                        newQcFile.AppendLine("Import=" + directory);
                        newQcFile.AppendLine("Export=" + directory);
                        newQcFile.AppendLine(afterFolder);
                        using (StreamWriter sw = new StreamWriter(qcIni))
                        {
                            sw.Write(newQcFile.ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Failure while reading QubicleConstructor ini file " + qcIni + ". Error: " + e.Message);
                    }
                }
            }

            System.Diagnostics.Process.Start(@filePath);
        }

        public void SetSelectedFileData(FileData file)
        {
            if (file != null)
            {
                if (file == mSelectedFileData)
                {
                    return;
                }

                filePreviewTabs.TabPages.Clear();
                openFileButtonPanel.Controls.Clear();
                iconView.ImageLocation = "";
                selectedFilePathTextBox.Text = file.Path;
                file.FillDependencyListItems(dependenciesListBox);
                file.FillReferencesListItems(referencesListBox);
                mSelectedFileData = file;
                if (mSelectedFileData is JsonFileData)
                {
                    OnJsonFileDataSelected();
                }
                else if (mSelectedFileData is LuaFileData)
                {
                    OnLuaFileDataSelected();
                }
            }
            else
            {
                mSelectedFileData = null;
                filePreviewTabs.TabPages.Clear();
                selectedFilePathTextBox.Text = "";
                dependenciesListBox.Items.Clear();
                referencesListBox.Items.Clear();
                openFileButtonPanel.Controls.Clear();
                iconView.ImageLocation = "";
            }
        }

        private void OnLuaFileDataSelected()
        {
            FileData openedFile = mSelectedFileData;
            TabPage newTabPage = new TabPage();
            newTabPage.Text = openedFile.FileName;
            FilePreview filePreview = new FilePreview(this, openedFile);
            filePreview.Dock = DockStyle.Fill;
            newTabPage.Controls.Add(filePreview);
            filePreviewTabs.TabPages.Add(newTabPage);
        }

        private void OnJsonFileDataSelected()
        {
            JsonFileData fileData = mSelectedFileData as JsonFileData;
            if (fileData.TreeNode != null)
            {
                treeView.SelectedNode = fileData.TreeNode;
            }

            List<string> addedOpenFiles = new List<string>();
            bool hasImage = false;
            foreach (FileData openedFile in fileData.OpenedFiles)
            {
                TabPage newTabPage = new TabPage();
                newTabPage.Text = openedFile.FileName;
                if (ModuleDataManager.GetInstance().ModifiedFiles.Contains(openedFile))
                {
                    newTabPage.Text = newTabPage.Text + "*";
                }

                if (openedFile.HasErrors)
                {
                    newTabPage.ImageIndex = 0;
                    newTabPage.ToolTipText = openedFile.Errors;
                }

                FilePreview filePreview = new FilePreview(this, openedFile);
                filePreview.Dock = DockStyle.Fill;
                newTabPage.Controls.Add(filePreview);
                filePreviewTabs.TabPages.Add(newTabPage);

                foreach (KeyValuePair<string, FileData> linkedFile in openedFile.LinkedFileData)
                {
                    if (addedOpenFiles.Contains(linkedFile.Key))
                    {
                        continue;
                    }

                    addedOpenFiles.Add(linkedFile.Key);

                    if (linkedFile.Value is QubicleFileData)
                    {
                        QubicleFileData qbFileData = linkedFile.Value as QubicleFileData;
                        string fileName = qbFileData.FileName;
                        Button openFileButton = new Button();
                        openFileButton.Name = qbFileData.GetOpenFilePath();
                        openFileButton.BackgroundImage = global::StonehearthEditor.Properties.Resources.qmofileicon_small;
                        openFileButton.BackgroundImageLayout = ImageLayout.None;
                        openFileButton.Text = Path.GetFileName(openFileButton.Name);
                        openFileButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        openFileButton.UseVisualStyleBackColor = true;
                        openFileButton.Click += new System.EventHandler(openFileButton_Click);
                        openFileButton.Padding = new Padding(22, 2, 2, 2);
                        openFileButton.AutoSize = true;
                        openFileButtonPanel.Controls.Add(openFileButton);
                    }
                    else if (linkedFile.Value is ImageFileData)
                    {
                        string imageFilePath = linkedFile.Value.Path;
                        if (System.IO.File.Exists(imageFilePath))
                        {
                            if (!hasImage)
                            {
                                iconView.ImageLocation = imageFilePath;
                                hasImage = true;
                            }

                            Button openFileButton = new Button();
                            openFileButton.Name = imageFilePath;
                            Image thumbnail = ThumbnailCache.GetThumbnail(imageFilePath);

                            openFileButton.BackgroundImage = thumbnail;
                            openFileButton.BackgroundImageLayout = ImageLayout.None;
                            openFileButton.Text = Path.GetFileName(openFileButton.Name);
                            openFileButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                            openFileButton.UseVisualStyleBackColor = true;
                            openFileButton.Click += new System.EventHandler(openFileButton_Click);
                            openFileButton.Padding = new Padding(22, 2, 2, 2);
                            openFileButton.AutoSize = true;
                            openFileButtonPanel.Controls.Add(openFileButton);
                        }
                    }
                }
            }
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            searchButton.PerformClick();
            if (searchBox.Text == string.Empty)
            {
                searchBox.BackColor = SystemColors.Window;
            }
        }

        public void Reload()
        {
            // Reload the manifest tab.
            Initialize();
            if (searchBox.Text == "error" && !ModuleDataManager.GetInstance().HasErrors)
            {
                searchBox.Text = "";
            }

            searchButton.PerformClick();

            if (Properties.Settings.Default.LastSelectedManifestPath != null)
            {
                FileData file = ModuleDataManager.GetInstance().GetSelectedFileData(Properties.Settings.Default.LastSelectedManifestPath);
                if (file != null && file.TreeNode != null)
                {
                    treeView.SelectedNode = file.TreeNode;
                }
            }
            else
            {
                SetSelectedFileData(null);
            }

            if (searchBox.Text == "")
            {
                searchBox.BackColor = SystemColors.Window;
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            FileData file = ModuleDataManager.GetInstance().GetSelectedFileData(treeView.SelectedNode);
            SetSelectedFileData(file);
            if (treeView.SelectedNode != null)
            {
                Properties.Settings.Default.LastSelectedManifestPath = treeView.SelectedNode.FullPath;
                Properties.Settings.Default.Save();
            }

            treeView.Focus();
        }

        private void aliasContextMenu_Opening(object sender, CancelEventArgs e)
        {
            TreeNode node = treeView.SelectedNode;
            aliasContextMenu.Visible = true;
            FileData file = ModuleDataManager.GetInstance().GetSelectedFileData(treeView.SelectedNode);
            if (file != null)
            {
                addIconicVersionToolStripMenuItem.Visible = CanAddEntityForm(file, "iconic");
                addGhostToolStripMenuItem.Visible = !CanAddEntityForm(file, "iconic") && CanAddEntityForm(file, "ghost");
                makeFineVersionToolStripMenuItem.Visible = CanAddFineVersion(file);
                removeFromManifestToolStripMenuItem.Visible = GetModuleFile(file) != null;
                aliasContextDuplicate.Visible = true;
                copyFullAliasToolStripMenuItem.Visible = true;
                addNewAliasToolStripMenuItem.Visible = false;
            }
            else
            {
                foreach (ToolStripItem item in aliasContextMenu.Items)
                {
                    item.Visible = false;
                }

                addNewAliasToolStripMenuItem.Visible = true;
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            string searchTerm = searchBox.Text;
            searchBox.BackColor = Color.Gold;
            ModuleDataManager.GetInstance().FilterAliasTree(treeView, searchTerm);
        }

        private void copyFullAliasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileData file = ModuleDataManager.GetInstance().GetSelectedFileData(treeView.SelectedNode);
            if (file != null)
            {
                IModuleFileData modFile = file as IModuleFileData;
                if (modFile != null && modFile.GetModuleFile() != null)
                {
                    Clipboard.SetText(modFile.GetModuleFile().FullAlias);
                }
                else
                {
                    Clipboard.SetText(file.Path);
                }
            }
        }

        private void treeView_MouseClick(object sender, MouseEventArgs e)
        {
            // Always select the clicked node
            treeView.SelectedNode = treeView.GetNodeAt(e.X, e.Y);
        }

        private bool CanAddFineVersion(FileData file)
        {
            JsonFileData jsonFileData = file as JsonFileData;
            if (jsonFileData == null)
            {
                return false; // Don't know how to clone something not jsonFileData
            }

            ModuleFile moduleFile = jsonFileData.GetModuleFile();
            if (moduleFile == null || moduleFile.IsFineVersion || jsonFileData.JsonType != JSONTYPE.ENTITY)
            {
                return false; // can only make fine version of a module file
            }

            string fineFullAlias = moduleFile.FullAlias + ":fine";
            if (ModuleDataManager.GetInstance().GetModuleFile(fineFullAlias) != null)
            {
                return false; // fine already exists
            }

            return true;
        }

        private ModuleFile GetModuleFile(FileData file)
        {
            IModuleFileData moduleFileData = file as IModuleFileData;
            if (moduleFileData == null)
            {
                return null; // only module file data can have modulefiles
            }

            return moduleFileData.GetModuleFile();
        }

        private void makeFineVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView.SelectedNode;
            FileData selectedFileData = ModuleDataManager.GetInstance().GetSelectedFileData(treeView.SelectedNode);
            if (!CanAddFineVersion(selectedFileData))
            {
                return;
            }

            JsonFileData jsonFileData = selectedFileData as JsonFileData;
            ModuleFile moduleFile = jsonFileData.GetModuleFile();
            CloneObjectParameters parameters = new CloneObjectParameters();
            parameters.AddStringReplacement(moduleFile.ShortName, moduleFile.ShortName + "_fine");
            parameters.AddAliasReplacement(moduleFile.ShortName + "_fine", moduleFile.ShortName + ":fine");
            HashSet<string> dependencies = ModuleDataManager.GetInstance().PreviewCloneDependencies(selectedFileData, parameters);
            PreviewCloneAliasCallback callback = new PreviewCloneAliasCallback(this, selectedFileData, parameters);
            PreviewCloneDialog dialog = new PreviewCloneDialog("Creating " + moduleFile.ShortName + ":fine", dependencies, callback);
            dialog.ShowDialog();
        }

        private class CloneAliasCallback : CloneDialog.IDialogCallback
        {
            private FileData mFileData;
            private ManifestView mViewer;
            private PreviewCloneAliasCallback mPreviewCallback;
            private string mManifestEntryType;

            public CloneAliasCallback(ManifestView viewer, FileData file, string manifestEntryType)
            {
                mViewer = viewer;
                mFileData = file;
                mManifestEntryType = manifestEntryType;
            }

            public void OnCancelled()
            {
                // Do nothing. user cancelled
            }

            public bool OnAccept(CloneObjectParameters parameters)
            {
                // Do the cloning
                string originalName = mFileData.GetNameForCloning();
                string potentialNewNodeName = parameters.TransformParameter(originalName);
                if (potentialNewNodeName.Length <= 1)
                {
                    MessageBox.Show("You must enter a name longer than 1 character for the clone!");
                    return false;
                }

                if (potentialNewNodeName.Equals(originalName) && parameters.SourceModule == parameters.TargetModule)
                {
                    MessageBox.Show("You must enter a new unique name for the clone or change the target mod!");
                    return false;
                }

                parameters.SetmanifestEntryType(mManifestEntryType);

                HashSet<string> dependencies = ModuleDataManager.GetInstance().PreviewCloneDependencies(mFileData, parameters);

                HashSet<string> savedUnwantedItems = mPreviewCallback != null ? mPreviewCallback.SavedUnwantedItems : null;
                mPreviewCallback = new PreviewCloneAliasCallback(mViewer, mFileData, parameters);
                mPreviewCallback.SavedUnwantedItems = savedUnwantedItems;
                PreviewCloneDialog dialog = new PreviewCloneDialog("Creating " + potentialNewNodeName, dependencies, mPreviewCallback);
                DialogResult result = dialog.ShowDialog();
                if (result != DialogResult.OK)
                {
                    return false;
                }

                return true;
            }
        }

        private class PreviewCloneAliasCallback : PreviewCloneDialog.IDialogCallback
        {
            public HashSet<string> SavedUnwantedItems { get; set; }

            private FileData mFileData;
            private ManifestView mViewer;
            private CloneObjectParameters mParameters;

            public PreviewCloneAliasCallback(ManifestView viewer, FileData fileData, CloneObjectParameters parameters)
            {
                mViewer = viewer;
                mFileData = fileData;
                mParameters = parameters;
            }

            public void OnCancelled(HashSet<string> unwantedItems)
            {
                // Do nothing. user cancelled
                SavedUnwantedItems = unwantedItems;
            }

            public bool OnAccept(HashSet<string> unwantedItems)
            {
                if (ModuleDataManager.GetInstance().ExecuteClone(mFileData, mParameters, unwantedItems))
                {
                    mViewer.Reload();
                }

                return true;
            }

            public HashSet<string> GetSavedUnwantedItems()
            {
                return SavedUnwantedItems;
            }
        }

        private void dependenciesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox.SelectedItem == null)
            {
                return;
            }

            string selectedItem = (string)listBox.SelectedItem;
            if (selectedItem.Contains(".png"))
            {
                // set the image
                string linkedFilePath = MainForm.kModsDirectoryPath + selectedItem;
                if (System.IO.File.Exists(linkedFilePath))
                {
                    iconView.ImageLocation = linkedFilePath;
                }
            }
        }

        private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            Properties.Settings.Default.ManifestViewTreeSplitterDistance = splitContainer2.SplitterDistance;
            Properties.Settings.Default.Save();
        }

        private void ManifestView_Load(object sender, EventArgs e)
        {
            splitContainer2.SplitterDistance = Properties.Settings.Default.ManifestViewTreeSplitterDistance;
        }

        private bool CanAddEntityForm(FileData file, string formName)
        {
            JsonFileData jsonFileData = file as JsonFileData;
            if (jsonFileData == null)
            {
                return false; // Don't know how to clone something not jsonFileData
            }

            if (jsonFileData.JsonType != JSONTYPE.ENTITY)
            {
                return false;
            }

            foreach (FileData openedJsonFile in jsonFileData.OpenedFiles)
            {
                if (openedJsonFile.Path.EndsWith("_" + formName + ".json"))
                {
                    return false; // already have an iconic
                }
            }

            return true;
        }

        private void addIconicVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView.SelectedNode;
            FileData selectedFileData = ModuleDataManager.GetInstance().GetSelectedFileData(treeView.SelectedNode);
            if (!CanAddEntityForm(selectedFileData, "iconic"))
            {
                return;
            }

            JsonFileData jsonFileData = selectedFileData as JsonFileData;
            string originalFileName = jsonFileData.FileName;
            string iconicFilePath = jsonFileData.Directory + "/" + originalFileName + "_iconic.json";

            try
            {
                string iconicJson = System.Text.Encoding.UTF8.GetString(StonehearthEditor.Properties.Resources.defaultIconic);
                if (iconicJson != null)
                {
                    // Get a linked qb file
                    string newQbFile = null;
                    JToken defaultModelVariant = jsonFileData.Json.SelectToken("components.model_variants.default");
                    string defaultModelVariantNames = defaultModelVariant != null ? defaultModelVariant.ToString() : "";
                    foreach (FileData data in jsonFileData.LinkedFileData.Values)
                    {
                        if (data is QubicleFileData)
                        {
                            string fileName = data.FileName + ".qb";
                            if (defaultModelVariantNames.Contains(fileName))
                            {
                                CloneObjectParameters parameters = new CloneObjectParameters();
                                parameters.AddStringReplacement(data.FileName, data.FileName + "_iconic");
                                newQbFile = data.Path.Replace(".qb", "_iconic.qb");
                                data.Clone(newQbFile, parameters, new HashSet<string>(), true);
                            }
                        }
                    }

                    if (newQbFile != null)
                    {
                        string relativePath = JsonHelper.MakeRelativePath(iconicFilePath, newQbFile);
                        iconicJson = iconicJson.Replace("default_iconic.qb", relativePath);
                    }

                    try
                    {
                        JObject parsedIconicJson = JObject.Parse(iconicJson);
                        iconicJson = JsonHelper.GetFormattedJsonString(parsedIconicJson); // put it in the parser and back again to make sure we get valid json.

                        using (StreamWriter wr = new StreamWriter(iconicFilePath, false, new UTF8Encoding(false)))
                        {
                            wr.Write(iconicJson);
                        }
                    }
                    catch (Exception e2)
                    {
                        MessageBox.Show("Unable to write new iconic file because " + e2.Message);
                        return;
                    }

                    JObject json = jsonFileData.Json;
                    JToken entityFormsComponent = json.SelectToken("components.stonehearth:entity_forms");
                    if (entityFormsComponent == null)
                    {
                        if (json["components"] == null)
                        {
                            json["components"] = new JObject();
                        }

                        JObject entityForms = new JObject();
                        json["components"]["stonehearth:entity_forms"] = entityForms;
                        entityFormsComponent = entityForms;
                    }

                   (entityFormsComponent as JObject).Add("iconic_form", "file(" + originalFileName + "_iconic.json" + ")");
                    jsonFileData.TrySetFlatFileData(jsonFileData.GetJsonFileString());
                    jsonFileData.TrySaveFile();
                    MessageBox.Show("Adding file " + iconicFilePath);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("Unable to add iconic file because " + ee.Message);
                return;
            }

            Reload();
        }

        private void searchBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                searchButton.PerformClick();
            }
        }

        private void TryMoveJToken(string name, JObject from, JObject to)
        {
            if (from[name] != null)
            {
                to[name] = from[name];
                from.Property(name).Remove();
            }
        }

        private void addGhostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView.SelectedNode;
            FileData selectedFileData = ModuleDataManager.GetInstance().GetSelectedFileData(treeView.SelectedNode);
            if (!CanAddEntityForm(selectedFileData, "ghost"))
            {
                return;
            }

            JsonFileData jsonFileData = selectedFileData as JsonFileData;
            string originalFileName = jsonFileData.FileName;
            string ghostFilePath = jsonFileData.Directory + "/" + originalFileName + "_ghost.json";

            try
            {
                JObject ghostJson = new JObject();
                ghostJson.Add("type", "entity");

                // Get a linked qb file
                string qbFilePath = null;
                foreach (FileData data in jsonFileData.LinkedFileData.Values)
                {
                    if (data is QubicleFileData)
                    {
                        qbFilePath = data.Path;
                    }
                }

                JObject ghostComponents = new JObject();
                ghostJson["components"] = ghostComponents;

                JObject json = jsonFileData.Json;
                JObject existingComponents = json["components"] as JObject;
                if (existingComponents != null)
                {
                    TryMoveJToken("unit_info", existingComponents, ghostComponents);
                    TryMoveJToken("render_info", existingComponents, ghostComponents);
                    TryMoveJToken("mob", existingComponents, ghostComponents);

                    // Only move the default model variant to the ghost:
                    JObject defaultModelVariant = existingComponents["model_variants"] as JObject;
                    if (defaultModelVariant != null && defaultModelVariant.Count == 1)
                    {
                        TryMoveJToken("model_variants", existingComponents, ghostComponents);
                    }
                    else
                    {
                        JObject modelVariants = new JObject();
                        ghostComponents["model_variants"] = modelVariants;
                        TryMoveJToken("default", defaultModelVariant, modelVariants);
                    }
                }

                string ghostJsonString = JsonHelper.GetFormattedJsonString(ghostJson);
                using (StreamWriter wr = new StreamWriter(ghostFilePath, false, new UTF8Encoding(false)))
                {
                    wr.Write(ghostJsonString);
                }

                JToken entityFormsComponent = json.SelectToken("components.stonehearth:entity_forms");
                if (entityFormsComponent == null)
                {
                    if (json["components"] == null)
                    {
                        json["components"] = new JObject();
                    }

                    JObject entityForms = new JObject();
                    json["components"]["stonehearth:entity_forms"] = entityForms;
                    entityFormsComponent = entityForms;
                }

                JToken mixins = json["mixins"];
                if (mixins == null)
                {
                    json.First.AddAfterSelf(new JProperty("mixins", "file(" + originalFileName + "_ghost.json" + ")"));
                }
                else
                {
                    JArray mixinsArray = mixins as JArray;
                    if (mixinsArray == null)
                    {
                        mixinsArray = new JArray();
                        json["mixins"] = mixinsArray;
                        mixinsArray.Add(mixins.ToString());
                    }

                    mixinsArray.Add("file(" + originalFileName + "_ghost.json" + ")");
                }

               (entityFormsComponent as JObject).Add("ghost_form", "file(" + originalFileName + "_ghost.json" + ")");
                jsonFileData.TrySetFlatFileData(jsonFileData.GetJsonFileString());
                jsonFileData.TrySaveFile();
                MessageBox.Show("Adding file " + ghostFilePath);
            }
            catch (Exception ee)
            {
                MessageBox.Show("Unable to add iconic file because " + ee.Message);
                return;
            }

            Reload();
        }

        private void dependenciesListBox_DoubleClick(object sender, EventArgs e)
        {
            if (mSelectedFileData == null)
            {
                return;
            }

            ListBox listBox = sender as ListBox;
            string selectedItem = (string)listBox.SelectedItem;
            if (selectedItem == null)
            {
                return;
            }

            if (selectedItem.Contains(":"))
            {
                // item is a alias item. we should navigate there.
                ModuleFile file = ModuleDataManager.GetInstance().GetModuleFile(selectedItem);
                if (file != null)
                {
                    SetSelectedFileData(file.FileData);
                }
            }
            else
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(selectedItem);
                bool foundFile = false;
                foreach (TabPage tabPage in filePreviewTabs.TabPages)
                {
                    string tabName = tabPage.Text.Replace("*", "").Trim();
                    if (tabName.Equals(fileName))
                    {
                        filePreviewTabs.SelectedTab = tabPage;
                        foundFile = true;
                        break;
                    }
                }

                if (!foundFile)
                {
                    FileData data;
                    bool hasFileData = false;
                    if (mSelectedFileData.ReferencedByFileData.TryGetValue(selectedItem, out data))
                    {
                        hasFileData = true;
                    }

                    string fullPath = ModuleDataManager.GetInstance().ModsDirectoryPath + selectedItem;
                    if (!hasFileData && mSelectedFileData.LinkedFileData.TryGetValue(fullPath, out data))
                    {
                        hasFileData = true;
                    }

                    if (hasFileData)
                    {
                        SetSelectedFileData(data);
                        if (data.TreeNode != null)
                        {
                            treeView.SelectedNode = data.TreeNode;
                        }
                    }
                }
            }
        }

        private void treeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            TreeNode node = e.Node;
            if (node.Tag != null)
            {
                IModuleFileData moduleFileData = node.Tag as IModuleFileData;
                if (moduleFileData != null && moduleFileData.GetModuleFile() != null)
                {
                    return; // okay to edit aliases
                }
            }

            e.CancelEdit = true;
        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label != null && e.Label.Length > 0)
            {
                FileData fileData = e.Node.Tag as FileData;
                IModuleFileData moduleFileData = e.Node.Tag as IModuleFileData;
                ModuleFile moduleFile = moduleFileData.GetModuleFile();
                string oldAlias = moduleFile.FullAlias;
                Module mod = moduleFile.Module;
                string newAlias = mod.Name + ":" + e.Label;

                // Update the references to use the new alias.
                foreach (FileData reference in fileData.ReferencedByFileData.Values)
                {
                    if (reference.FlatFileData != null)
                    {
                        string updatedFlatFile = reference.FlatFileData.Replace(oldAlias, newAlias);
                        reference.TrySetFlatFileData(updatedFlatFile);
                        reference.TrySaveFile();
                    }
                }

                mod.AddToManifest(e.Label, moduleFile.OriginalPath);
                mod.RemoveFromManifest("aliases", moduleFile.Alias);
                mod.WriteManifestToFile();
                Reload();
            }
            else
            {
                e.CancelEdit = true;
            }
        }

        private void removeFromManifestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView.SelectedNode;
            FileData selectedFileData = ModuleDataManager.GetInstance().GetSelectedFileData(treeView.SelectedNode);
            ModuleFile moduleFile = GetModuleFile(selectedFileData);
            if (moduleFile != null)
            {
                TreeNode parent = selectedNode.Parent;
                moduleFile.Module.RemoveFromManifest(parent.Text, moduleFile.Alias);
                moduleFile.Module.WriteManifestToFile();
                Reload();
            }
        }

        private void addNewAliasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView.SelectedNode;
            string manifestEntryType = "aliases";
            if (selectedNode.Parent != null)
            {
                manifestEntryType = selectedNode.Text;
                selectedNode = selectedNode.Parent;
            }

            string moduleName = selectedNode.Text;

            Module selectedMod = ModuleDataManager.GetInstance().GetMod(moduleName);
            if (selectedMod != null)
            {
                string initialDirectory;
                if (!mLastModuleLocations.TryGetValue(moduleName, out initialDirectory))
                {
                    initialDirectory = System.IO.Path.GetFullPath(selectedMod.Path);
                }
                else
                {
                    initialDirectory = System.IO.Path.GetFullPath(initialDirectory);
                }

                selectJsonFileDialog.InitialDirectory = initialDirectory;
                selectJsonFileDialog.Tag = new NewAliasParameters(selectedMod, manifestEntryType);
                selectJsonFileDialog.ShowDialog(this);
            }
        }

        private void selectJsonFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            string filePath = selectJsonFileDialog.FileName;
            if (filePath == null)
            {
                return;
            }

            filePath = JsonHelper.NormalizeSystemPath(filePath);
            NewAliasParameters parameters = selectJsonFileDialog.Tag as NewAliasParameters;
            Module selectedMod = parameters.SelectedMod;
            string manifestEntryType = parameters.ManifestEntryType;

            if (!filePath.Contains(selectedMod.Path))
            {
                MessageBox.Show("The file must be under the directory " + selectedMod.Path);
                return;
            }

            mLastModuleLocations[selectedMod.Name] = filePath;
            string shortPath = filePath.Replace(selectedMod.Path + "/", "");
            string[] pathSplit = shortPath.Split('/');
            string samplePath = string.Empty;
            for (int i = 1; i < (pathSplit.Length - 1); ++i)
            {
                if (string.IsNullOrEmpty(samplePath))
                {
                    samplePath = pathSplit[i];
                }
                else
                {
                    samplePath = samplePath + ':' + pathSplit[i];
                }
            }

            if (pathSplit.Length > 2)
            {
                // Make the file path not contain the .json part if it doesn't have to
                string fileName = pathSplit[pathSplit.Length - 1];
                string extension = System.IO.Path.GetExtension(fileName);
                if (extension == ".json")
                {
                    string folder = pathSplit[pathSplit.Length - 2];
                    if (folder.Equals(System.IO.Path.GetFileNameWithoutExtension(fileName)))
                    {
                        shortPath = shortPath.Replace("/" + fileName, "");
                    }
                }
                else if (extension == ".lua")
                {
                    string nameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileName);
                    if (nameWithoutExtension.EndsWith("_action"))
                    {
                        nameWithoutExtension = nameWithoutExtension.Substring(0, nameWithoutExtension.Length - 7);
                        samplePath = samplePath + ':' + nameWithoutExtension;
                    }
                }
            }

            NewAliasCallback callback = new NewAliasCallback(this, selectedMod, shortPath, manifestEntryType);
            InputDialog dialog = new InputDialog("Add New Alias", "Type the name of the alias for " + filePath, samplePath, "Add!");
            dialog.SetCallback(callback);
            dialog.ShowDialog();
        }

        private class NewAliasCallback : InputDialog.IDialogCallback
        {
            private ManifestView mOwner;
            private Module mModule;
            private string mFilePath;
            private string mManifestEntryType;

            public NewAliasCallback(ManifestView owner, Module module, string filePath, string manifestEntryType)
            {
                mOwner = owner;
                mModule = module;
                mFilePath = filePath;
                mManifestEntryType = manifestEntryType;
            }

            public void OnCancelled()
            {
                // Do nothing. user cancelled
            }

            public bool OnAccept(string inputMessage)
            {
                // Do the cloning
                string newAliasName = inputMessage.Trim();
                if (newAliasName.Length <= 1)
                {
                    MessageBox.Show("You must enter a name longer than 1 character for the new alias!");
                    return false;
                }

                if (mModule.GetAliasFile(newAliasName) != null)
                {
                    MessageBox.Show("An alias already exists with that name!");
                    return false;
                }

                mModule.AddToManifest(newAliasName, "file(" + mFilePath + ")", mManifestEntryType);
                mModule.WriteManifestToFile();
                mOwner.Reload();
                return true;
            }
        }

        private void toolStripStatusLabel1_DoubleClick(object sender, EventArgs e)
        {
            if (ModuleDataManager.GetInstance().HasErrors)
            {
                ModuleDataManager.GetInstance().FilterAliasTree(treeView, "error");
                searchButton.PerformClick();
            }
        }

        private void statusStrip_DoubleClick(object sender, EventArgs e)
        {
            if (ModuleDataManager.GetInstance().HasErrors)
            {
                searchBox.Text = "error";
                searchButton.PerformClick();
            }

            // open up error file list view
            if (mErrorFileListView == null || mErrorFileListView.IsDisposed)
            {
                mErrorFileListView = new ErrorFileList(this);
                mErrorFileListView.Show(this);
            }
        }
    }
}
