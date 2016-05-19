using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StonehearthEditor
{
   public partial class EffectsEditorView : UserControl, IReloadable
    {
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
      }

      public void Reload()
      {
         ModuleDataManager.GetInstance().LoadEffectsList(effectsEditorTreeView);
         filePreviewTabs.TabPages.Clear();
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
         //if (fileData.IsModified)
         //{
         //   newTabPage.Text = newTabPage.Text + "*";
         //}
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

         FileData[] fileData = {};
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

         foreach (FileData openedFile in fileData)
         {
            FillFilePreview(openedFile);
         }
      }

      private void effectsEditorTreeView_MouseClick(object sender, MouseEventArgs e)
      {
         effectsEditorTreeView.SelectedNode = effectsEditorTreeView.GetNodeAt(e.X, e.Y);
      }

      private void effectsEditorTreeView_AfterSelect(object sender, TreeViewEventArgs e)
      {
         Object fullPath = effectsEditorTreeView.SelectedNode.Tag;
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
   }
}
