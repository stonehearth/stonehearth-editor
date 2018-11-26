using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StonehearthEditor.Dialogs
{
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void SHEDHomePageLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SHEDHomePageLinkLabel.LinkVisited = true;
            System.Diagnostics.Process.Start("https://github.com/stonehearth/stonehearth-editor");
        }

        private void moddingGuideLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            moddingGuideLinkLabel.LinkVisited = true;
            System.Diagnostics.Process.Start("https://stonehearth.github.io/modding_guide/index.html");
        }

        private void OKbutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
