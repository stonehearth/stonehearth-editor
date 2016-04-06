using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using StonehearthEditor.Properties;
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

        private NetWorthVisualizer mNetWorthVisualizer;

        public MainForm(string path)
        {
            if (path != null)
            {
                path = path.Trim();
                kModsDirectoryPath = JsonHelper.NormalizeSystemPath(path);
            }

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(kModsDirectoryPath))
            {
                chooseModDirectory();
            }

            LoadModFiles();
            int initialTab = (int)Properties.Settings.Default["InitialTab"];
            tabControl.SelectedIndex = initialTab;

            if (Properties.Settings.Default.MainFormSize != null)
            {
                this.Size = Properties.Settings.Default.MainFormSize;
            }
        }

        private void LoadModFiles()
        {
            manifestView.Initialize();
            encounterDesignerView.Initialize();
            entityBrowserView.Initialize();
        }

        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
            // e.TabPageIndex;
            Properties.Settings.Default["InitialTab"] = e.TabPageIndex;
            Properties.Settings.Default.Save();
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO yshan: revisit this
            GameMasterDataManager.GetInstance().SaveModifiedFiles();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reload();
        }

        public void Reload()
        {
            manifestView.Reload();
            entityBrowserView.Reload();
            if (mNetWorthVisualizer != null && !mNetWorthVisualizer.IsDisposed)
            {
                mNetWorthVisualizer.UpdateNetWorthData();
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Copy window size to app settings
            if (this.WindowState == FormWindowState.Normal)
            {
                Settings.Default.MainFormSize = this.Size;
            }
            else
            {
                Settings.Default.MainFormSize = this.RestoreBounds.Size;
            }

            Properties.Settings.Default.Save();
        }

        private void netWorthVisualizerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mNetWorthVisualizer != null && !mNetWorthVisualizer.IsDisposed)
            {
                mNetWorthVisualizer.UpdateNetWorthData();
                return;
            }

            mNetWorthVisualizer = new NetWorthVisualizer();
            mNetWorthVisualizer.SetManifestView(manifestView);
            ////mNetWorthVisualizer.UpdateNetWorthData();
            mNetWorthVisualizer.Show(this);
        }

        private void chooseModDirectory()
        {
            DialogResult result = modsFolderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string newPath = modsFolderBrowserDialog.SelectedPath;
                if (!string.IsNullOrWhiteSpace(newPath))
                {
                    kModsDirectoryPath = JsonHelper.NormalizeSystemPath(modsFolderBrowserDialog.SelectedPath);
                    Properties.Settings.Default["ModsDirectory"] = kModsDirectoryPath;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    MessageBox.Show("invalid mods directory");
                    return;
                }
            }
            else
            {
                MessageBox.Show("invalid mods directory");
                return;
            }
        }

        private void changeModDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chooseModDirectory();
        }
    }
}
