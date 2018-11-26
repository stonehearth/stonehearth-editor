using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace StonehearthEditor
{
    public partial class ModDirectorySettingsDialog : Form
    {
        public interface IDialogCallback
        {
            // Returns true if we can close the dialog
            bool OnAccept(string baseModsPath, string steamUploadsPath);

            void OnCancelled();
        }

        private IDialogCallback mCallback;

        public ModDirectorySettingsDialog(string baseModsPath, string steamUploadsPath)
        {
            InitializeComponent();

            if (baseModsPath != null)
            {
                baseModsPathTextbox.Text = baseModsPath;
            }

            if (steamUploadsPath != null)
            {
                steamUploadsPathTextbox.Text = steamUploadsPath;
            }
        }

        public void SetCallback(IDialogCallback callback)
        {
            mCallback = callback;
        }

        private void changeBaseModsPathButton_Click(object sender, EventArgs e)
        {
            var dialog = new FolderSelectDialog()
            {
                DirectoryPath = baseModsPathTextbox.Text ?? Environment.CurrentDirectory,
                Title = "Stonehearth Mods Root Directory"
            };

            DialogResult result = DialogResult.Abort;

            result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                baseModsPathTextbox.Text = dialog.DirectoryPath;
            }
        }

        private void changeSteamUploadsPathButton_Click(object sender, EventArgs e)
        {
            var dialog = new FolderSelectDialog()
            {
                DirectoryPath = steamUploadsPathTextbox.Text ?? Environment.CurrentDirectory,
                Title = "Steam Uploads Directory"
            };

            DialogResult result = DialogResult.Abort;

            result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                steamUploadsPathTextbox.Text = dialog.DirectoryPath;
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (mCallback != null)
            {
                bool isSuccess = mCallback.OnAccept(baseModsPathTextbox.Text, steamUploadsPathTextbox.Text);
                if (isSuccess)
                {
                    mCallback = null;
                    this.DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }
    }
}
