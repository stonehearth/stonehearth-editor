using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StonehearthEditor
{
    public partial class CloneDialog : Form
    {
        public interface IDialogCallback
        {
            // Returns true if we can close the dialog
            bool OnAccept(CloneObjectParameters cloneParameters);

            void onCancelled();
        }

        private IDialogCallback mCallback;

        public CloneDialog(string clonedObjectName, string initialText)
        {
            InitializeComponent();
            Text = "Clone: " + clonedObjectName;

            int index = clonedObjectName.IndexOf(':');
            if (index != -1 && clonedObjectName.Length > index)
            {
                // Since user must specify a new alias, if alias doesn't contain
                // the file name (initialText), make a new row with the old alias name as text to replace
                string oldAlias = clonedObjectName.Substring(index + 1);
                if (!oldAlias.Contains(initialText))
                {
                    AddNewRow(oldAlias);
                }
            }

            AddNewRow(initialText);
            AcceptButton = cloneButton;

            this.ActiveControl = parametersTable.GetControlFromPosition(1, 1);
            ////parametersTable.GetControlFromPosition(1, 1).Focus();
        }

        public void SetCallback(IDialogCallback callback)
        {
            mCallback = callback;
        }

        private void cloneDialogButton_Click(object sender, EventArgs e)
        {
            if (mCallback != null)
            {
                CloneObjectParameters parameters = new CloneObjectParameters();

                for (int row = 0; row < parametersTable.RowCount; row++)
                {
                    TextBox original = parametersTable.GetControlFromPosition(0, row) as TextBox;
                    TextBox replacement = parametersTable.GetControlFromPosition(1, row) as TextBox;
                    if ((original != null) &&
                        (replacement != null) &&
                        (!string.IsNullOrWhiteSpace(original.Text)) &&
                        (!string.IsNullOrWhiteSpace(replacement.Text)))
                    {
                        parameters.AddStringReplacement(original.Text, replacement.Text);
                    }
                }

                bool isSuccess = mCallback.OnAccept(parameters);
                if (isSuccess)
                {
                    mCallback = null;
                    Close();
                }
            }
        }

        private void CloneDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mCallback != null)
            {
                mCallback.onCancelled();
                mCallback = null;
            }
        }

        private void AddParamButton_Click(object sender, EventArgs e)
        {
            AddNewRow(null);
        }

        private void AddNewRow(string originalName)
        {
            TextBox newOriginalParam = new TextBox();
            newOriginalParam.Dock = DockStyle.Fill;
            if (originalName != null)
            {
                newOriginalParam.Text = originalName;
            }

            TextBox newReplacementParam = new TextBox();
            newReplacementParam.Dock = DockStyle.Fill;

            parametersTable.RowCount = parametersTable.RowCount + 1;
            parametersTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 25f));
            parametersTable.Controls.Add(newOriginalParam, 0, parametersTable.RowCount - 1);
            parametersTable.Controls.Add(newReplacementParam, 1, parametersTable.RowCount - 1);
            newOriginalParam.Focus();
        }
    }
}
