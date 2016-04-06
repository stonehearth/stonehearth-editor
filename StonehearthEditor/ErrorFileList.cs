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
    public partial class ErrorFileList : Form
    {
        private ManifestView mOwner;
        public ErrorFileList(ManifestView owner)
        {
            mOwner = owner;
            InitializeComponent();
            foreach (FileData file in ModuleDataManager.GetInstance().GetErrorFiles())
            {
                listBox.Items.Add(file);
            }
        }

        private void listBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            FileData file = listBox.SelectedItem as FileData;
            if (file != null)
            {
                mOwner.SetSelectedFileData(file);
            }
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FileData file = listBox.SelectedItem as FileData;
            if (file != null)
            {
                errorsTextBox.Text = file.Errors;
            }
        }
    }
}
