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
   public partial class FilePreview : UserControl
   {
      private FileData mFileData;
      public FilePreview(FileData fileData)
      {
         mFileData = fileData;
         InitializeComponent();
         textBox.Text = mFileData.FlatFileData;
      }
   }
}
