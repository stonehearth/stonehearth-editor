using System;
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
        public static string kModsDirectoryPath;
        public static string kSteamUploadsDirectoryPath;

        private NetWorthVisualizer mNetWorthVisualizer;

        public MainForm(string path, string steamUploadsPath)
        {
            InitializeComponent();
            if (path != null)
            {
                path = path.Trim();
                kModsDirectoryPath = JsonHelper.NormalizeSystemPath(path);
            }
            if (steamUploadsPath != null)
            {
                steamUploadsPath = steamUploadsPath.Trim();
                kSteamUploadsDirectoryPath = JsonHelper.NormalizeSystemPath(steamUploadsPath);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(kModsDirectoryPath) || checkModsFolder(ref kModsDirectoryPath) != ModFolderCheckResult.VALID)
            {
                if (!chooseModDirectory()) {
                    Application.Exit();
                }
            }

            var splash = new LoadingSplash();
            splash.Show();
            Application.DoEvents();  // Hacky, but whatever.
            LoadModFiles();
            splash.Dispose();
            splash.Hide();
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
            recipesView.SaveModifiedFiles();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reload();
        }

        public void Reload()
        {
            // Show the splash on reload too since it takes a while
            var splash = new LoadingSplash();
            splash.Show();
            Application.DoEvents();  // Hacky, but whatever.

            manifestView.Reload();
            entityBrowserView.Reload();
            encounterDesignerView.Reload();
            effectsEditorView.Reload();
            recipesView.Reload();

            if (mNetWorthVisualizer != null && !mNetWorthVisualizer.IsDisposed)
            {
                mNetWorthVisualizer.UpdateNetWorthData();
            }

            splash.Dispose();
            splash.Hide();
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

        private bool chooseModDirectory()
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
                    var check = this.checkModsFolder(ref newPath);

                    if (check == ModFolderCheckResult.ZIPPED) {

                        if (MessageBox.Show("The chosen directory appears to have compressed mods. To use it, the mods must be uncompressed. Would you like to do that?", "Compressed mods directory", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes) {
                            var splash = new LoadingSplash();
                            splash.Show();
                            Application.DoEvents();  // Hacky, but whatever.
                            foreach (var path in Directory.EnumerateFiles(newPath)) {
                                if (path.EndsWith(".smod") && !Directory.Exists(path.Substring(0, path.Length - 5))) {
                                    System.IO.Compression.ZipFile.ExtractToDirectory(path, newPath);
                                }
                            }
                            splash.Hide();
                            splash.Dispose();
                            check = ModFolderCheckResult.VALID;
                        }
                    }

                    if (check == ModFolderCheckResult.VALID)
                    {
                        kModsDirectoryPath = JsonHelper.NormalizeSystemPath(newPath);
                        Properties.Settings.Default["ModsDirectory"] = kModsDirectoryPath;
                        Properties.Settings.Default.Save();
                        return true;
                    }
                    else if (Directory.Exists(newPath))
                    {
                        // If the directory does exist, but doesn't seem to be valid, make it a user choice
                        if (MessageBox.Show("The chosen directory does not appear to be a valid mods directory. Choose it anyway?", "Possibly invalid mods directory", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes) {
                            kModsDirectoryPath = JsonHelper.NormalizeSystemPath(newPath);
                            Properties.Settings.Default["ModsDirectory"] = kModsDirectoryPath;
                            return true;
                        }
                    }
                    else
                    {
                        if (MessageBox.Show("Invalid mods directory chosen. Try again?", "Invalid directory", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                            return false;
                    }
                }
                else
                {
                    if (MessageBox.Show("No mods directory selected. Try again?", "No directory selected.", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        return false;
                }
            }
            while (true); // repeat until we've got a proper directory or the user aborts
        }

        private enum ModFolderCheckResult
        {
            INVALID,
            VALID,
            ZIPPED
        }

        /// <summary>
        /// Checks if a folder is likely to be a valid mod folder.
        /// </summary>
        /// <param name="modsFolder">Full path to the folder that is to be checked.</param>
        /// <returns><c>true</c> if the folder is likely a valid mods folder, <c>false</c> otherwise.</returns>
        private ModFolderCheckResult checkModsFolder(ref string modsFolder)
        {
            if (!Directory.Exists(modsFolder))
                return ModFolderCheckResult.INVALID;

            // Are there zipped mods?
            if (Directory.EnumerateFiles(modsFolder).Any(file => file.EndsWith(".smod") && !Directory.Exists(file.Substring(0, file.Length - 5)))) {
                return ModFolderCheckResult.ZIPPED;
            }

            // If there is at least one directory that contains a manifest.json, it's probably a valid directory
            if (Directory.EnumerateDirectories(modsFolder).Any(subDir => File.Exists(Path.Combine(subDir, "manifest.json")))) {
                return ModFolderCheckResult.VALID;
            }

            // Maybe they've selected the SH root folder..?
            var subDirectory = Path.Combine(modsFolder, "mods");
            if (Directory.Exists(subDirectory) && this.checkModsFolder(ref subDirectory) != ModFolderCheckResult.INVALID)
            {
                modsFolder = subDirectory;
                return checkModsFolder(ref modsFolder);
            }

            return ModFolderCheckResult.INVALID;
        }

        private void changeModDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangePathsCallback callback = new ChangePathsCallback(this);
            string oldModsPath = kModsDirectoryPath;
            string oldSteamUploadsPath = kSteamUploadsDirectoryPath;
            ModDirectorySettingsDialog dialog = new ModDirectorySettingsDialog(kModsDirectoryPath, kSteamUploadsDirectoryPath);
            dialog.SetCallback(callback);
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                // Reload as long as the user clicked on the Apply and reload button
                Reload();
            }
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

        private class ChangePathsCallback : ModDirectorySettingsDialog.IDialogCallback
        {
            private MainForm form = null;

            public ChangePathsCallback(MainForm formInstance)
            {
                form = formInstance;
            }

            public void OnCancelled()
            {
                // Do nothing. user cancelled
            }

            public bool OnAccept(string newBaseModsPath, string newSteamUploadsPath)
            {
                bool modsPathAccepted = false;
                // First check that the mods directory is valid (we require it)
                var check = form.checkModsFolder(ref newBaseModsPath);
                if (check == ModFolderCheckResult.ZIPPED)
                {
                    if (UnzipMods(newBaseModsPath))
                    {
                        check = ModFolderCheckResult.VALID;
                    }
                }

                if (check == ModFolderCheckResult.VALID)
                {
                    kModsDirectoryPath = JsonHelper.NormalizeSystemPath(newBaseModsPath);
                    Properties.Settings.Default["ModsDirectory"] = kModsDirectoryPath;
                    Properties.Settings.Default.Save();
                    modsPathAccepted = true;
                }
                else if (Directory.Exists(newBaseModsPath))
                {
                    // If the directory does exist, but doesn't seem to be valid, make it a user choice
                    if (MessageBox.Show("The chosen directory does not appear to be a valid mods directory. It should contain the uncompressed stonehearth mod. Choose it anyway?", "Possibly invalid mods directory", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        kModsDirectoryPath = JsonHelper.NormalizeSystemPath(newBaseModsPath);
                        Properties.Settings.Default["ModsDirectory"] = kModsDirectoryPath;
                        modsPathAccepted = true;
                    }
                }
                else
                {
                    if (MessageBox.Show("Invalid mods directory chosen. Try again?", "Invalid directory", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                        modsPathAccepted = false;
                }

                // Check the steam_uploads directory (can be empty if the user is developing in the mods folder)
                var check2 = form.checkModsFolder(ref newSteamUploadsPath);
                if (check2 == ModFolderCheckResult.ZIPPED)
                {
                    if (UnzipMods(newSteamUploadsPath))
                    {
                        check2 = ModFolderCheckResult.VALID;
                    }
                }

                if (check2 == ModFolderCheckResult.VALID)
                {
                    kSteamUploadsDirectoryPath = JsonHelper.NormalizeSystemPath(newSteamUploadsPath);
                    Properties.Settings.Default["SteamUploadsDirectory"] = kSteamUploadsDirectoryPath;
                    Properties.Settings.Default.Save();
                }
                else if (Directory.Exists(newSteamUploadsPath))
                {
                    // If the directory does exist, but doesn't seem to be valid, make it a user choice
                    if (MessageBox.Show("The chosen directory does not appear to be a valid mods directory. Choose it anyway?", "Possibly invalid additional mods directory", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        kSteamUploadsDirectoryPath = JsonHelper.NormalizeSystemPath(newSteamUploadsPath);
                        Properties.Settings.Default["SteamUploadsDirectory"] = kSteamUploadsDirectoryPath;
                    }
                }
                else if (newSteamUploadsPath == string.Empty)
                {
                    // Accept cleaning the additional path to not show the mods in there
                    kSteamUploadsDirectoryPath = JsonHelper.NormalizeSystemPath(newSteamUploadsPath);
                    Properties.Settings.Default["SteamUploadsDirectory"] = kSteamUploadsDirectoryPath;
                }
                return modsPathAccepted;
            }

            private bool UnzipMods(string modsPath)
            {
                if (MessageBox.Show("The chosen directory (" + modsPath + ") \nappears to have compressed mods. To use it, the mods must be uncompressed. Would you like to do that?", "Compressed mods directory", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    var splash = new LoadingSplash();
                    splash.Show();
                    Application.DoEvents();  // Hacky, but whatever.
                    foreach (var path in Directory.EnumerateFiles(modsPath))
                    {
                        if (path.EndsWith(".smod") && !Directory.Exists(path.Substring(0, path.Length - 5)))
                        {
                            System.IO.Compression.ZipFile.ExtractToDirectory(path, modsPath);
                        }
                    }
                    splash.Hide();
                    splash.Dispose();
                    return true;
                }
                return false;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.AboutDialog dialog = new Dialogs.AboutDialog();
            dialog.Show();
        }
    }
}
