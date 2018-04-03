using System;
using System.Windows.Forms;

namespace StonehearthEditor
{
    public partial class InputDialog : Form
    {
        public interface IDialogCallback
        {
            // Returns true if we can close the dialog
            bool OnAccept(string inputMessage);

            void OnCancelled();
        }

        private IDialogCallback mCallback;

        public InputDialog(string title, string message, string initialText, string buttonText)
        {
            InitializeComponent();
            Text = title;
            inputDialogLabel.Text = message;
            inputDialogOkayButton.Text = buttonText;
            inputDialogTextBox.Text = initialText;
            AcceptButton = inputDialogOkayButton;
        }

        public void SetCallback(IDialogCallback callback)
        {
            mCallback = callback;
        }

        private void inputDialogOkayButton_Click(object sender, EventArgs e)
        {
            if (mCallback != null)
            {
                bool isSuccess = mCallback.OnAccept(inputDialogTextBox.Text);
                if (isSuccess)
                {
                    mCallback = null;
                    Close();
                }
            }
        }

        private void InputDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mCallback != null)
            {
                mCallback.OnCancelled();
                mCallback = null;
            }
        }

        private void InputDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) {
                this.Close();
            }
        }
    }
}
