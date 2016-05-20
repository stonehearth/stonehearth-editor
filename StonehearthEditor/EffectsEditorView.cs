using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StonehearthEditor
{
    public partial class EffectsEditorView : UserControl, IReloadable
    {
        private static TreeNode mSelectedNode = null;
        private Dictionary<string, FileData[]> mFileDataMap = new Dictionary<string, FileData[]>();

        public EffectsEditorView()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            new ModuleDataManager(MainForm.kModsDirectoryPath);
            ModuleDataManager.GetInstance().Load();
            ModuleDataManager.GetInstance().LoadEffectsList(effectsEditorTreeView);
            ModuleDataManager.GetInstance().LoadCubemittersList(cubemittersTreeView);
        }

        public void Reload()
        {
            filePreviewTabs.TabPages.Clear();
            effectsEditorTreeView.Nodes.Clear();
            cubemittersTreeView.Nodes.Clear();
            ModuleDataManager.GetInstance().LoadEffectsList(effectsEditorTreeView);
            ModuleDataManager.GetInstance().LoadCubemittersList(cubemittersTreeView);
        }

        private void effectsOpenFileButton_Click(object sender, EventArgs e)
        {
            openEffectsFileDialog.InitialDirectory = MainForm.kModsDirectoryPath;
            openEffectsFileDialog.ShowDialog(this);
        }

        private void openEffectsFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            string filePath = openEffectsFileDialog.FileName;
            if (filePath == null)
            {
                return;
            }

            LoadFilePreview(filePath);
        }

        // Loads file preview from single FileData
        private void FillFilePreview(FileData fileData)
        {
            TabPage newTabPage = new TabPage();
            newTabPage.Text = fileData.FileName;

            // if (fileData.IsModified)
            // {
            //   newTabPage.Text = newTabPage.Text + "*";
            // }
            if (fileData.HasErrors)
            {
                newTabPage.ImageIndex = 0;
                newTabPage.ToolTipText = fileData.Errors;
            }

            FilePreview filePreview = new FilePreview(this, fileData);
            filePreview.Dock = DockStyle.Fill;
            newTabPage.Controls.Add(filePreview);
            filePreviewTabs.TabPages.Add(newTabPage);
        }

        // Load file preview from file path
        private void LoadFilePreview(string filePath)
        {
            filePreviewTabs.TabPages.Clear();
            FileData[] fileData = GetFileDataFromPath(filePath);

            foreach (FileData openedFile in fileData)
            {
                FillFilePreview(openedFile);
            }
        }

        private FileData[] GetFileDataFromPath(string filePath)
        {
            FileData[] fileData = { };
            mFileDataMap.TryGetValue(filePath, out fileData);
            if (fileData == null)
            {
                JsonFileData json = new JsonFileData(filePath);
                if (json != null)
                {
                    json.Load();
                    fileData = json.OpenedFiles.ToArray();
                    mFileDataMap[filePath] = fileData;
                }
            }

            return fileData;
        }

        private void effectsEditorTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            object fullPath = effectsEditorTreeView.SelectedNode.Tag;
            if (fullPath != null)
            {
                LoadFilePreview(fullPath.ToString());
            }
            else
            {
                // If no file data found, just clear file preview
                filePreviewTabs.TabPages.Clear();
            }

            effectsEditorTreeView.Focus();
        }

        private void effectsEditorTreeView_MouseClick(object sender, MouseEventArgs e)
        {
            effectsEditorTreeView.SelectedNode = effectsEditorTreeView.GetNodeAt(e.X, e.Y);
            mSelectedNode = effectsEditorTreeView.SelectedNode;
            CheckShowContextMenu(effectsEditorTreeView, e);
        }

        private void cubemittersTreeView_MouseClick(object sender, MouseEventArgs e)
        {
            cubemittersTreeView.SelectedNode = cubemittersTreeView.GetNodeAt(e.X, e.Y);
            mSelectedNode = cubemittersTreeView.SelectedNode;
            CheckShowContextMenu(cubemittersTreeView, e);
        }

        private void CheckShowContextMenu(TreeView treeView, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                effectsEditorContextMenuStrip.Show(treeView, new Point(e.X, e.Y));
            }
        }

        private void cubemittersTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            object fullPath = cubemittersTreeView.SelectedNode.Tag;
            if (fullPath != null)
            {
                LoadFilePreview(fullPath.ToString());
            }
            else
            {
                // If no file data found, just clear file preview
                filePreviewTabs.TabPages.Clear();
            }

            cubemittersTreeView.Focus();
        }

        private TreeView GetTreeView(int index)
        {
            if (index == 0)
            {
                return cubemittersTreeView;
            }

            return effectsEditorTreeView;
        }

        private void cloneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeView treeView = GetTreeView(treeViewTabControl.SelectedIndex);
            TreeNode selectedNode = treeView.SelectedNode;
            string filePath = selectedNode.Tag != null ? selectedNode.Tag.ToString() : null;

            if (filePath == null)
            {
                MessageBox.Show("Invalid effect file!");
                return;
            }

            FileData selectedFileData = GetFileDataFromPath(filePath).First<FileData>();
            CloneEffectFileCallback callback = new CloneEffectFileCallback(this, selectedFileData);
            CloneDialog dialog = new CloneDialog(selectedFileData.FileName, selectedFileData.GetNameForCloning());
            dialog.SetCallback(callback);
            dialog.ShowDialog();
        }

        // TODO: Refactor dialog/callback code so this isn't copy pasted from manifest view classes
        private class CloneEffectFileCallback : CloneDialog.IDialogCallback
        {
            private FileData mFileData;
            private EffectsEditorView mViewer;
            private PreviewCloneFileCallback mPreviewCallback;

            public CloneEffectFileCallback(EffectsEditorView viewer, FileData file)
            {
                mViewer = viewer;
                mFileData = file;
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
                if (potentialNewNodeName.Equals(originalName))
                {
                    MessageBox.Show("You must enter a new unique name for the clone!");
                    return false;
                }
                HashSet<string> dependencies = ModuleDataManager.GetInstance().PreviewCloneDependencies(mFileData, parameters);
                HashSet<string> savedUnwantedItems = mPreviewCallback != null ? mPreviewCallback.GetSavedUnwantedItems() : null;
                mPreviewCallback = new PreviewCloneFileCallback(mViewer, mFileData, parameters);
                mPreviewCallback.SetUnwantedItems(savedUnwantedItems);
                PreviewCloneDialog dialog = new PreviewCloneDialog("Creating " + potentialNewNodeName, dependencies, mPreviewCallback);
                DialogResult result = dialog.ShowDialog();
                if (result != DialogResult.OK)
                {
                    return false;
                }
                return true;
            }
        }

        private class PreviewCloneFileCallback : PreviewCloneDialog.IDialogCallback
        {
            private FileData mFileData;
            private EffectsEditorView mViewer;
            private CloneObjectParameters mParameters;
            private HashSet<string> savedUnwantedItems;

            public PreviewCloneFileCallback(EffectsEditorView viewer, FileData fileData, CloneObjectParameters parameters)
            {
                mViewer = viewer;
                mFileData = fileData;
                mParameters = parameters;
            }

            public void OnCancelled(HashSet<string> unwantedItems)
            {
                // Do nothing. user cancelled
                savedUnwantedItems = unwantedItems;
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
                return savedUnwantedItems;
            }

            public void SetUnwantedItems(HashSet<string> items)
            {
                savedUnwantedItems = items;
            }
        }

        public void newFileButton_Click(object sender, EventArgs e)
        {
            if (mSelectedNode != null)
            {
                string path = mSelectedNode.Tag != null ? JsonHelper.NormalizeSystemPath(mSelectedNode.Tag.ToString()) : null;
                if (path != null)
                {
                    saveEffectsFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(path);
                }
            }
            else
            {
                saveEffectsFileDialog.InitialDirectory = System.IO.Path.GetFullPath(ModuleDataManager.GetInstance().ModsDirectoryPath);
            }

            saveEffectsFileDialog.ShowDialog();
            saveEffectsFileDialog.RestoreDirectory = true;
        }

        private void saveEffectsFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            string directory = System.IO.Path.GetFullPath(saveEffectsFileDialog.FileName);
            using (StreamWriter wr = new StreamWriter(directory, false, new UTF8Encoding(false)))
            {
                wr.Write("{\n\n}");
            }

            Reload();
        }
    }
}
