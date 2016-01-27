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
         bool OnAccept(string inputMessage);
         void onCancelled();
      }

      private IDialogCallback mCallback;

      public CloneDialog(string clonedObjectName, string initialText)
      {
         InitializeComponent();
         Text = "Clone: " + clonedObjectName;
         originalText1.Text = initialText;
         AcceptButton = cloneButton;
      }

      public void SetCallback(IDialogCallback callback)
      {
         mCallback = callback;
      }

      private void cloneDialogButton_Click(object sender, EventArgs e)
      {
         if (mCallback != null)
         {
            bool isSuccess = mCallback.OnAccept(replacementText1.Text);
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
   }
}
