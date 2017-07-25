using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace StonehearthEditor
{
    public partial class AliasSelectionDialog : Form
    {
        public interface IDialogCallback
        {
            // Returns true if we can close the dialog
            bool OnAccept(IEnumerable<string> aliases);

            void OnCancelled();
        }

        private class SimpleCallback : IDialogCallback
        {
            private Action<IEnumerable<string>> acceptCallback;

            public SimpleCallback(Action<IEnumerable<string>> acceptCallback)
            {
                this.acceptCallback = acceptCallback;
            }

            public bool OnAccept(IEnumerable<string> aliases)
            {
                acceptCallback(aliases);
                return true;
            }

            public void OnCancelled()
            {
                // Nothing!
            }
        }

        private HashSet<string> mAllAliases = new HashSet<string>();
        private IDialogCallback mCallback;

        public bool MultiSelect
        {
            get { return listBox.SelectionMode == SelectionMode.MultiExtended; }
            set { listBox.SelectionMode = value ? SelectionMode.MultiExtended : SelectionMode.One; }
        }

        public AliasSelectionDialog(Action<IEnumerable<string>> acceptCallback)
            : this(new SimpleCallback(acceptCallback))
        {
        }

        public AliasSelectionDialog(IDialogCallback callback)
        {
            InitializeComponent();
            mCallback = callback;
            foreach (Module mod in ModuleDataManager.GetInstance().GetAllModules())
            {
                foreach (ModuleFile alias in mod.GetAliases())
                {
                    mAllAliases.Add(alias.FullAlias);
                }
            }

            SetFilter(null);
        }

        private void SetFilter(string filter)
        {
            bool isEmpty = string.IsNullOrWhiteSpace(filter);
            listBox.Items.Clear();
            foreach (string alias in mAllAliases)
            {
                if (isEmpty || alias.Contains(filter))
                {
                    listBox.Items.Add(alias);
                }
            }
        }

        private void filterTextBox_TextChanged(object sender, EventArgs e)
        {
            SetFilter(filterTextBox.Text);
        }

        private void acceptButton_Click(object sender, EventArgs e)
        {
            if (mCallback != null)
            {
                // Casts the listbox items to strings (throws an exception should anything be uncastable),
                // then selects only unique (distinct) elements and passes that to OnAccept as IEnumerable<string>
                bool isSuccess = mCallback.OnAccept(listBox.SelectedItems.Cast<string>().Distinct());
                if (isSuccess)
                {
                    mCallback = null;
                    Close();
                }
            }
        }

        private void AliasSelectionDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mCallback != null)
            {
                mCallback.OnCancelled();
                mCallback = null;
            }
        }

        private void listBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            acceptButton.PerformClick();
        }
    }
}
