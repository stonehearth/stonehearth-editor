using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace StonehearthEditor
{
    public partial class PreviewCloneDialog : Form
    {
        public interface IDialogCallback
        {
            // Returns true if we can close the dialog
            bool OnAccept(HashSet<string> unwantedItems);

            void OnCancelled(HashSet<string> unwantedItems);

            HashSet<string> GetSavedUnwantedItems();
        }

        private IDialogCallback mCallback;
        private HashSet<string> mSet;

        public PreviewCloneDialog(string title, HashSet<string> set, IDialogCallback callback)
        {
            InitializeComponent();
            Text = title;
            dependenciesListBox.Items.Clear();
            mSet = set;

            HashSet<string> unwantedItems = callback.GetSavedUnwantedItems();
            foreach (string item in mSet)
            {
                bool isChecked = unwantedItems != null ? !unwantedItems.Contains(item) : true;
                dependenciesListBox.Items.Add(item, isChecked);
            }

            mCallback = callback;
        }

        private HashSet<string> GetUnwantedItems()
        {
            HashSet<string> unwantedItems = new HashSet<string>(mSet);
            foreach (object item in dependenciesListBox.CheckedItems)
            {
                unwantedItems.Remove(item.ToString());
            }

            return unwantedItems;
        }

        private void acceptButton_Click(object sender, EventArgs e)
        {
            if (mCallback != null)
            {
                bool isSuccess = mCallback.OnAccept(GetUnwantedItems());
                if (isSuccess)
                {
                    mCallback = null;
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (mCallback != null)
            {
                mCallback.OnCancelled(GetUnwantedItems());
                mCallback = null;
            }
        }

        private void PreviewCloneDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mCallback != null)
            {
                mCallback.OnCancelled(GetUnwantedItems());
                mCallback = null;
            }
        }

        private void checkAllCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            bool checkStatus = checkAllCheckbox.Checked; // This is the current status after checked changed

            for (int i = 0; i < dependenciesListBox.Items.Count; i++)
            {
                dependenciesListBox.SetItemChecked(i, checkStatus);
            }
        }
    }
}
