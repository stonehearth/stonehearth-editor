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
   public partial class PreviewCloneDialog : Form
   {
      public interface IDialogCallback
      {
         // Returns true if we can close the dialog
         bool OnAccept(HashSet<string> unwantedItems);
         void onCancelled();
      }

      private IDialogCallback mCallback;
      private HashSet<string> mSet;
      public PreviewCloneDialog(string title, HashSet<string> set, IDialogCallback callback)
      {
         InitializeComponent();
         Text = title;
         dependenciesListBox.Items.Clear();
         mSet = set;
         foreach (string item in mSet)
         {
            dependenciesListBox.Items.Add(item, true);
         }
         mCallback = callback;
      }

      private void acceptButton_Click(object sender, EventArgs e)
      {
         if (mCallback != null)
         {
            HashSet<string> unwantedItems = new HashSet<string>(mSet);
            foreach(object item in dependenciesListBox.CheckedItems)
            {
               unwantedItems.Remove(item.ToString());
            }
            bool isSuccess = mCallback.OnAccept(unwantedItems);
            if (isSuccess)
            {
               mCallback = null;
               Close();
            }
         }
      }

      private void cancelButton_Click(object sender, EventArgs e)
      {
         if (mCallback != null)
         {
            mCallback.onCancelled();
            mCallback = null;
         }
      }

      private void PreviewCloneDialog_FormClosed(object sender, FormClosedEventArgs e)
      {
         if (mCallback != null)
         {
            mCallback.onCancelled();
            mCallback = null;
         }
      }
   }
}
