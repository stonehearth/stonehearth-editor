using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json.Linq;

namespace StonehearthEditor
{
   public partial class ManifestView : UserControl
   {
      private FileData mSelectedFileData = null;
      public ManifestView()
      {
         InitializeComponent();
      }

      public void Initialize()
      {
         new ModuleDataManager(MainForm.kModsDirectoryPath);
         ModuleDataManager.GetInstance().Load();
         ModuleDataManager.GetInstance().FilterAliasTree(treeView, null);
      }

      private void aliasContextMenuDuplicate_Click(object sender, EventArgs e)
      {
         TreeNode selectedNode = treeView.SelectedNode;
         FileData selectedFileData = ModuleDataManager.GetInstance().GetSelectedFileData(treeView.SelectedNode);
         if (selectedFileData == null)
         {
            return;
         }
         if (!(selectedFileData is IModuleFileData))
         {
            return; // Don't know how to clone something not module file data
         }

         CloneAliasCallback callback = new CloneAliasCallback(this, selectedFileData);
         InputDialog dialog = new InputDialog("Clone " + selectedFileData.FileName, "Type name of duplicated. If recipe, leave out the '_recipe' at the end.", selectedFileData.GetNameForCloning(), "Clone!");
         dialog.SetCallback(callback);
         dialog.ShowDialog();
      }

      private void openFileButton_Click(object sender, EventArgs e)
      {
         // open file
         Button button = sender as Button;
         System.Diagnostics.Process.Start(@button.Name);
      }

      private void SetSelectedFileData(FileData file)
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
            JsonFileData fileData = mSelectedFileData as JsonFileData;
            if (fileData != null)
            {
               List<string> addedOpenFiles = new List<string>();
               bool hasImage = false;
               foreach (FileData openedFile in fileData.OpenedFiles)
               {
                  TabPage newTabPage = new TabPage();
                  newTabPage.Text = openedFile.FileName;
                  if (openedFile.IsModified)
                  {
                     newTabPage.Text = newTabPage.Text + "*";
                  }
                  if (openedFile.HasErrors)
                  {
                     newTabPage.ImageIndex = 0;
                     newTabPage.ToolTipText = openedFile.Errors;
                  }
                  FilePreview filePreview = new FilePreview(openedFile);
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
                        if (!hasImage)
                        {
                           if (System.IO.File.Exists(linkedFile.Value.Path))
                           {
                              iconView.ImageLocation = linkedFile.Value.Path;
                              hasImage = true;
                           }
                        }
                     }
                  }
               }
            }
         }
         else
         {
            mSelectedFileData = null;
            filePreviewTabs.TabPages.Clear();
            selectedFilePathTextBox.Text = "";
            dependenciesListBox.Items.Clear();
            openFileButtonPanel.Controls.Clear();
         }
      }
 
      private void searchBox_TextChanged(object sender, EventArgs e)
      {
         searchButton.PerformClick();
      }

      public void Reload()
      {
         // Reload the manifest tab.
         new ModuleDataManager(MainForm.kModsDirectoryPath);
         ModuleDataManager.GetInstance().Load();
         ModuleDataManager.GetInstance().FilterAliasTree(treeView, null);
         searchButton.PerformClick();

         if (Properties.Settings.Default.LastSelectedManifestPath != null)
         {
            FileData file = ModuleDataManager.GetInstance().GetSelectedFileData(Properties.Settings.Default.LastSelectedManifestPath);
            if (file != null && file.TreeNode != null)
            {
               treeView.SelectedNode = file.TreeNode;
            } 
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
         FileData file = ModuleDataManager.GetInstance().GetSelectedFileData(treeView.SelectedNode);
         if (file == null)
         {
            e.Cancel = true;
         }
         addIconicVersionToolStripMenuItem.Visible = CanAddEntityForm(file, "iconic");
         addGhostToolStripMenuItem.Visible = !CanAddEntityForm(file, "iconic") && CanAddEntityForm(file, "ghost");
         makeFineVersionToolStripMenuItem.Visible = CanAddFineVersion(file);
      }

      private void searchButton_Click(object sender, EventArgs e)
      {
         string searchTerm = searchBox.Text;
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
            } else
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
         string newName = moduleFile.ShortName + ":fine";
         HashSet<string> dependencies = ModuleDataManager.GetInstance().PreviewCloneDependencies(selectedFileData, newName);
         PreviewCloneAliasCallback callback = new PreviewCloneAliasCallback(this, selectedFileData, newName);
         PreviewCloneDialog dialog = new PreviewCloneDialog("Creating " + newName, dependencies, callback);
         dialog.ShowDialog();
      }
      private class CloneAliasCallback : InputDialog.IDialogCallback
      {
         private FileData mFileData;
         private ManifestView mViewer;
         public CloneAliasCallback(ManifestView viewer, FileData file)
         {
            mViewer = viewer;
            mFileData = file;
         }
         public void onCancelled()
         {
            // Do nothing. user cancelled
         }

         public bool OnAccept(string inputMessage)
         {
            // Do the cloning
            string potentialNewNodeName = inputMessage.Trim();
            if (potentialNewNodeName.Length <= 1)
            {
               MessageBox.Show("You must enter a name longer than 1 character for the clone!");
               return false;
            }
            if (potentialNewNodeName.Equals(mFileData.GetNameForCloning()))
            {
               MessageBox.Show("You must enter a new unique name for the clone!");
               return false;
            }
            HashSet<string> dependencies = ModuleDataManager.GetInstance().PreviewCloneDependencies(mFileData, potentialNewNodeName);
            PreviewCloneAliasCallback callback = new PreviewCloneAliasCallback(mViewer, mFileData, potentialNewNodeName);
            PreviewCloneDialog dialog = new PreviewCloneDialog("Creating " + potentialNewNodeName, dependencies, callback);
            dialog.ShowDialog();
            return true;
         }
      }

      private class PreviewCloneAliasCallback : PreviewCloneDialog.IDialogCallback
      {
         private FileData mFileData;
         private ManifestView mViewer;
         private string mNewName;
         public PreviewCloneAliasCallback(ManifestView viewer, FileData fileData, string newName)
         {
            mViewer = viewer;
            mFileData = fileData;
            mNewName = newName;
         }
         public void onCancelled()
         {
            // Do nothing. user cancelled
         }

         public bool OnAccept(HashSet<string> unwantedItems)
         {
            if (ModuleDataManager.GetInstance().ExecuteClone(mFileData, mNewName, unwantedItems))
            {
               mViewer.Reload();
            }
            return true;
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
               foreach (FileData data in jsonFileData.LinkedFileData.Values)
               {
                  if (data is QubicleFileData)
                  {
                     newQbFile = data.Path.Replace(".qb", "_iconic.qb");
                     data.Clone(newQbFile, data.FileName, data.FileName + "_iconic", new HashSet<string>(), true);
                  }
               }

               if (newQbFile != null)
               {
                  string relativePath = JsonHelper.MakeRelativePath(iconicFilePath, newQbFile);
                  iconicJson = iconicJson.Replace("default_iconic.qb", relativePath);
               }
               using (StreamWriter wr = new StreamWriter(iconicFilePath, false, new UTF8Encoding(false)))
               {
                  wr.Write(iconicJson);
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
               TryMoveJToken("model_variants", existingComponents, ghostComponents);
               TryMoveJToken("mob", existingComponents, ghostComponents);
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
            } else
            {
               (mixins as JArray).Add("file(" + originalFileName + "_ghost.json" + ")");
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
               if (mSelectedFileData.ReferencedByFileData.TryGetValue(selectedItem, out data))
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
            JsonFileData jsonFileData = node.Tag as JsonFileData;
            if (jsonFileData != null && jsonFileData.GetModuleFile() != null)
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
            JsonFileData jsonFileData = e.Node.Tag as JsonFileData;
            ModuleFile moduleFile = jsonFileData.GetModuleFile();
            string oldAlias = moduleFile.FullAlias;
            Module mod = moduleFile.Module;
            string newAlias = mod.Name + ":" + e.Label;

            // Update the references to use the new alias.
            foreach (FileData reference in jsonFileData.ReferencedByFileData.Values)
            {
               if (reference.FlatFileData != null) {
                  string updatedFlatFile = reference.FlatFileData.Replace(oldAlias, newAlias);
                  reference.TrySetFlatFileData(updatedFlatFile);
                  reference.TrySaveFile();
               }
            }
            
            mod.AddToManifest(e.Label, moduleFile.OriginalPath);
            mod.RemoveFromManifest(moduleFile.Name);
            mod.WriteManifestToFile();
            Reload();
         } else
         {
            e.CancelEdit = true;
         }
      }
   }
}
