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
            file.FillDependencyListItems(dependenciesListView);
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
                  FilePreview filePreview = new FilePreview(openedFile);
                  filePreview.Dock = DockStyle.Fill;
                  newTabPage.Controls.Add(filePreview);
                  filePreviewTabs.TabPages.Add(newTabPage);

                  foreach (string linkedFilePath in openedFile.LinkedFilePaths)
                  {
                     if (addedOpenFiles.Contains(linkedFilePath))
                     {
                        continue;
                     }
                     addedOpenFiles.Add(linkedFilePath);
                     string extension = Path.GetExtension(linkedFilePath);
                     if (extension == ".qb")
                     {
                        string fileName = Path.GetFileNameWithoutExtension(linkedFilePath);
                        string qmoPath = JsonHelper.NormalizeSystemPath(Path.GetDirectoryName(linkedFilePath)) + "/" + fileName + ".qmo";
                        Button openFileButton = new Button();
                        openFileButton.Name = System.IO.File.Exists(qmoPath) ? qmoPath : linkedFilePath;
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
                     else if (extension == ".png")
                     {
                        if (!hasImage)
                        {
                           if (System.IO.File.Exists(linkedFilePath))
                           {
                              iconView.ImageLocation = linkedFilePath;
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
            dependenciesListView.Clear();
            openFileButtonPanel.Controls.Clear();
         }
      }
 
      private void searchBox_TextChanged(object sender, EventArgs e)
      {
         string searchTerm = searchBox.Text;
         ModuleDataManager.GetInstance().FilterAliasTree(treeView, searchTerm);
      }

      public void Reload()
      {
         // Reload the manifest tab.
         SetSelectedFileData(null);
         new ModuleDataManager(MainForm.kModsDirectoryPath);
         ModuleDataManager.GetInstance().Load();
         ModuleDataManager.GetInstance().FilterAliasTree(treeView, null);
      }
      private void dependenciesListView_MouseDoubleClick(object sender, MouseEventArgs e)
      {
         if (mSelectedFileData == null)
         {
            return;
         }
         ListViewItem item = dependenciesListView.GetItemAt(e.X, e.Y);
         string selectedItem = item.Text;
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
            foreach (TabPage tabPage in filePreviewTabs.TabPages)
            {
               string tabName = tabPage.Text.Replace("*", "").Trim();
               if (tabName.Equals(fileName))
               {
                  filePreviewTabs.SelectedTab = tabPage;
                  break;
               }
            }
         }
      }

      private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
      {
         FileData file = ModuleDataManager.GetInstance().GetSelectedFileData(treeView.SelectedNode);
         SetSelectedFileData(file);
         treeView.Focus();
      }

      private void aliasContextMenu_Opening(object sender, CancelEventArgs e)
      {
         FileData file = ModuleDataManager.GetInstance().GetSelectedFileData(treeView.SelectedNode);
         if (file == null)
         {
            e.Cancel = true;
         }
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

      private void makeFineVersionToolStripMenuItem_Click(object sender, EventArgs e)
      {
         TreeNode selectedNode = treeView.SelectedNode;
         FileData selectedFileData = ModuleDataManager.GetInstance().GetSelectedFileData(treeView.SelectedNode);
         if (selectedFileData == null)
         {
            return;
         }
         JsonFileData jsonFileData = selectedFileData as JsonFileData;
         if (jsonFileData == null)
         {
            return; // Don't know how to clone something not jsonFileData
         }
         ModuleFile moduleFile = jsonFileData.GetModuleFile();
         if (moduleFile == null || moduleFile.IsFineVersion || jsonFileData.JsonType != JSONTYPE.ENTITY)
         {
            return; // can only make fine version of a module file
         }
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
         if (dependenciesListView.SelectedItems.Count <= 0)
         {
            return;
         }
         ListViewItem item = dependenciesListView.SelectedItems[0];
         string selectedItem = item.Text;
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

      private void ManifestView_Leave(object sender, EventArgs e)
      {
         Properties.Settings.Default.ManifestViewTreeSplitterDistance = splitContainer2.SplitterDistance;
         Properties.Settings.Default.Save();
      }

      private void ManifestView_Load(object sender, EventArgs e)
      {
         splitContainer2.SplitterDistance = Properties.Settings.Default.ManifestViewTreeSplitterDistance;
      }

      private void addIconicVersionToolStripMenuItem_Click(object sender, EventArgs e)
      {
         TreeNode selectedNode = treeView.SelectedNode;
         FileData selectedFileData = ModuleDataManager.GetInstance().GetSelectedFileData(treeView.SelectedNode);
         if (selectedFileData == null)
         {
            return;
         }
         JsonFileData jsonFileData = selectedFileData as JsonFileData;
         if (jsonFileData == null)
         {
            return; // Don't know how to clone something not jsonFileData
         }
         jsonFileData.AddIconicVersion();
      }
   }
}
