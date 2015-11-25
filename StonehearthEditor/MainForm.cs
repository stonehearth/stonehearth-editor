using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace StonehearthEditor
{
   public interface IGraphOwner
   {
      void SetGraph(Microsoft.Msagl.Drawing.Graph graph);
   }

   public partial class MainForm : Form
   {
      public static string kModsDirectoryPath;

      public MainForm(string path)
      {
         kModsDirectoryPath = path;
         InitializeComponent();
      }

      private void Form1_Load(object sender, EventArgs e)
      {
         if (string.IsNullOrEmpty(kModsDirectoryPath))
         {
            DialogResult result = modsFolderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
               kModsDirectoryPath = modsFolderBrowserDialog.SelectedPath;
               Properties.Settings.Default["ModsDirectory"] = kModsDirectoryPath;
               Properties.Settings.Default.Save();
            }
         }
         LoadModFiles();
         int initialTab = (int) Properties.Settings.Default["InitialTab"];
         tabControl.SelectedIndex = initialTab;
      }

      private void LoadModFiles()
      {
         manifestView.Initialize();
         encounterDesignerView.Initialize();
      }

      private void tabControl_Selected(object sender, TabControlEventArgs e)
      {
         //e.TabPageIndex;
         Properties.Settings.Default["InitialTab"] = e.TabPageIndex;
         Properties.Settings.Default.Save();
      }

      private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
      {
         //TODO yshan: revisit this
         GameMasterDataManager.GetInstance().SaveModifiedFiles();
      }

      private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Reload();
      }

      public void Reload()
      {
         manifestView.Reload();
      }
   }
}
