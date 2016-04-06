using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StonehearthEditor
{
   public partial class AliasSelectionDialog : Form
   {
      public interface IDialogCallback
      {
         // Returns true if we can close the dialog
         bool OnAccept(HashSet<string> aliases);

         void onCancelled();
      }

      private HashSet<string> mAllAliases = new HashSet<string>();
      private IDialogCallback mCallback;

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
            HashSet<string> selected = new HashSet<string>();
            foreach (object obj in listBox.SelectedItems)
            {
               string alias = (string)obj;
               selected.Add(alias);
            }

            bool isSuccess = mCallback.OnAccept(selected);
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
            mCallback.onCancelled();
            mCallback = null;
         }
      }

      private void listBox_MouseDoubleClick(object sender, MouseEventArgs e)
      {
         acceptButton.PerformClick();
      }
   }
}
