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
      public EffectsEditorView()
      {
         InitializeComponent();
      }

      public void Initialize()
      {
      }

      public void Reload()
      {
      }

      private void effectsOpenFileButton_Click(object sender, EventArgs e)
      {
         //TreeNode selectedNode = treeView.SelectedNode;
         //if (selectedNode.Parent != null)
         //{
         //   selectedNode = selectedNode.Parent;
         //}
         //string moduleName = selectedNode.Text;

         //Module selectedMod = ModuleDataManager.GetInstance().GetMod(moduleName);
         //if (selectedMod != null)
         //{
         //   string initialDirectory;
         //   if (!mLastModuleLocations.TryGetValue(moduleName, out initialDirectory))
         //   {
         //      initialDirectory = System.IO.Path.GetFullPath(selectedMod.Path);
         //   }
         //   else
         //   {
         //      initialDirectory = System.IO.Path.GetFullPath(initialDirectory);
         //   }
         //   selectJsonFileDialog.InitialDirectory = initialDirectory;
         //   selectJsonFileDialog.Tag = selectedMod;
         //   selectJsonFileDialog.ShowDialog(this);
         //}

         openEffectsFileDialog.ShowDialog(this);
      }

      private void openEffectsFileDialog_FileOk(object sender, CancelEventArgs e)
      {
         string filePath = openEffectsFileDialog.FileName;
         if (filePath == null)
         {
            return;
         }
         JsonFileData json = new JsonFileData(filePath);
         if (json != null)
         {
            json.Load();
            foreach (FileData openedFile in json.OpenedFiles)
            {
               TabPage newTabPage = new TabPage();
               newTabPage.Text = openedFile.FileName;
               //if (openedFile.IsModified)
               //{
               //   newTabPage.Text = newTabPage.Text + "*";
               //}
               if (openedFile.HasErrors)
               {
                  newTabPage.ImageIndex = 0;
                  newTabPage.ToolTipText = openedFile.Errors;
               }
               FilePreview filePreview = new FilePreview(this, openedFile);
               filePreview.Dock = DockStyle.Fill;
               newTabPage.Controls.Add(filePreview);
               filePreviewTabs.TabPages.Add(newTabPage);
            }
         }
      }
   }
}
