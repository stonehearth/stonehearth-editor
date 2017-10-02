﻿using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using StonehearthEditor.Properties;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace StonehearthEditor
{
    public partial class MainForm : Form
    {
        public static string kModsDirectoryPath { get; set; }

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
            effectsEditorView.Initialize();
            recipesView.Initialize();
        }

        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
            // e.TabPageIndex;
            Properties.Settings.Default["InitialTab"] = e.TabPageIndex;
            Properties.Settings.Default.Save();
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameMasterDataManager.GetInstance().SaveModifiedFiles();
            ModuleDataManager.GetInstance().SaveModifiedFiles();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reload();
        }

        public void Reload()
        {
            manifestView.Reload();
            entityBrowserView.Reload();
            encounterDesignerView.Reload();
            effectsEditorView.Reload();
            recipesView.Reload();

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
            var dialog = new FolderSelectDialog()
            {
                DirectoryPath = kModsDirectoryPath ?? Environment.CurrentDirectory,
                Title = "Stonehearth Mods Root Directory"
            };

            DialogResult result = DialogResult.Abort;

            do
            {
                result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    string newPath = dialog.DirectoryPath;

                    if (this.checkModsFolder(ref newPath))
                    {
                        kModsDirectoryPath = JsonHelper.NormalizeSystemPath(newPath);
                        Properties.Settings.Default["ModsDirectory"] = kModsDirectoryPath;
                        Properties.Settings.Default.Save();
                        return;
                    }
                    else if (Directory.Exists(newPath))
                    {
                        // If the directory does exist, but doesn't seem to be valid, make it a user chocie
                        if (MessageBox.Show("The chosen directory does not appear to be a valid mods directory. Choose it anyway?", "Possibly invalid mods directory", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                            return;
                    }
                    else
                    {
                        if (MessageBox.Show("Invalid mods directory chosen. Try again?", "Invalid directory", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                            return;
                    }
                }
                else
                {
                    if (MessageBox.Show("No mods directory selected. Try again?", "No directory selected.", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        return;
                }
            }
            while (true); // repeat until we've got a proper directory or the user aborts
        }

        /// <summary>
        /// Checks if a folder is likely to be a valid mod folder.
        /// </summary>
        /// <param name="modsFolder">Full path to the folder that is to be checked.</param>
        /// <returns><c>true</c> if the folder is likely a valid mods folder, <c>false</c> otherwise.</returns>
        private bool checkModsFolder(ref string modsFolder)
        {
            if (!Directory.Exists(modsFolder))
                return false;

            // If there is at least one directory that contains a manifest.json, it's probably a valid directory
            if (Directory.EnumerateDirectories(modsFolder).Any(subDir => File.Exists(Path.Combine(subDir, "manifest.json"))))
                return true;

            // Maybe they've selected the SH root folder..?
            var subDirectory = Path.Combine(modsFolder, "mods");
            if (Directory.Exists(subDirectory) && this.checkModsFolder(ref subDirectory))
            {
                modsFolder = subDirectory;
                return true;
            }

            return false;
        }

        private void changeModDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chooseModDirectory();
        }

        private void createNewMod()
        {
            NewModCallback callback = new NewModCallback(this.manifestView);
            InputDialog dialog = new InputDialog("Create New Mod", "Type the name of your mod (lowercase only, no spaces):", "my_test_mod", "Create!");
            dialog.SetCallback(callback);
            dialog.ShowDialog();
        }

        private void newModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            createNewMod();
        }

        private class NewModCallback : InputDialog.IDialogCallback
        {
            private ManifestView mOwner;
            private string manifestTemplate = System.Text.Encoding.UTF8.GetString(StonehearthEditor.Properties.Resources.defaultManifest);
            private string modsDirectoryPath = ModuleDataManager.GetInstance().ModsDirectoryPath;

            public NewModCallback(ManifestView owner)
            {
                mOwner = owner;
            }

            public void OnCancelled()
            {
                // Do nothing. user cancelled
            }

            public bool OnAccept(string inputMessage)
            {
                if (checkNewModName(inputMessage) && manifestTemplate != null)
                {
                    string newModPath = modsDirectoryPath + "/" + inputMessage;
                    string newManifestPath = newModPath + "/manifest.json";
                    string newLocalesPath = newModPath + "/locales/en.json";

                    Directory.CreateDirectory(newModPath + "/locales");

                    using (StreamWriter wr = new StreamWriter(newManifestPath, false, new UTF8Encoding(false)))
                    {
                        using (JsonTextWriter jsonTextWriter = new JsonTextWriter(wr))
                        {
                            jsonTextWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
                            jsonTextWriter.Indentation = 3;
                            jsonTextWriter.IndentChar = ' ';

                            JsonSerializer jsonSerializer = new JsonSerializer();
                            jsonSerializer.Serialize(jsonTextWriter, JObject.Parse(manifestTemplate));
                        }
                    }

                    using (StreamWriter wr = new StreamWriter(newLocalesPath, false, new UTF8Encoding(false)))
                    {
                        using (JsonTextWriter jsonTextWriter = new JsonTextWriter(wr))
                        {
                            jsonTextWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
                            jsonTextWriter.Indentation = 3;
                            jsonTextWriter.IndentChar = ' ';

                            JsonSerializer jsonSerializer = new JsonSerializer();
                            jsonSerializer.Serialize(jsonTextWriter, new JObject());
                        }
                    }

                    // Need a way to autoselect the new mod treeItem after reload, to focus on it
                    mOwner.Reload();
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private bool checkNewModName(string modName)
            {
                bool isValid = true;

                if (ModuleDataManager.GetInstance().GetMod(modName) != null)
                {
                    MessageBox.Show("Mod already exists in your mods folder!");
                    isValid = false;
                }

                if (!Regex.IsMatch(modName, "^[a-z0-9_]*$"))
                {
                    MessageBox.Show("Mod name can only contain lowercase characters, numbers and underscore.");
                    isValid = false;
                }

                return isValid;
            }
        }
    }
}
