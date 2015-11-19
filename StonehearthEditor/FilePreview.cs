using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace StonehearthEditor
{
   public partial class FilePreview : UserControl
   {
      private FileData mFileData;
      private int mI18nTooltipLine = -1;
      public FilePreview(FileData fileData)
      {
         mFileData = fileData;
         InitializeComponent();
         textBox.Text = mFileData.FlatFileData;
      }

      private void textBox_Leave(object sender, EventArgs e)
      {
         mFileData.TrySetFlatFileData(textBox.Text);
      }

      private void textBox_MouseMove(object sender, MouseEventArgs e)
      {
         int charIndex = textBox.GetCharIndexFromPosition(e.Location);
         int line = textBox.GetLineFromCharIndex(charIndex);

         if (textBox.Lines.Length <= line)
         {
            return;
         }
         if (mI18nTooltipLine == line)
         {
            return;
         }
         i18nTooltip.Hide(textBox);

         mI18nTooltipLine = line;
         string lineString = textBox.Lines[line];
         Regex matcher = new Regex(@"i18n\(([^)]+)\)");
         Match locMatch = matcher.Match(lineString);
         if (locMatch.Success)
         {
            string translated = ModuleDataManager.GetInstance().LocalizeString(locMatch.Groups[1].Value);
            translated = JsonHelper.WordWrap(translated, 100);
            i18nTooltip.Show(translated, textBox, e.Location);
         }
         else
         {
            i18nTooltip.Hide(textBox);
         }
      }
      private void saveToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Save();
      }

      private void Save()
      {
         mFileData.TrySetFlatFileData(textBox.Text);
         mFileData.TrySaveFile();
         TabPage parentControl = Parent as TabPage;
         if (parentControl != null)
         {
            textBox.Text = mFileData.FlatFileData;
            parentControl.Text = mFileData.FileName;
         }
      }

      private void textBox_KeyDown(object sender, KeyEventArgs e)
      {
         TabPage parentControl = Parent as TabPage;
         if (parentControl != null)
         {
            if (!mFileData.FlatFileData.Equals(textBox.Text))
            {
               parentControl.Text = mFileData.FileName + "*";
            }
         }
      }

      private void openFile_Click(object sender, EventArgs e)
      {
         System.Diagnostics.Process.Start(mFileData.Path);
      }

      private void openFolder_Click(object sender, EventArgs e)
      {
         System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(mFileData.Path));
      }

      private void saveFile_Click(object sender, EventArgs e)
      {
         Save();
      }

      private void localizeFile_Click(object sender, EventArgs e)
      {
         /*
         ProcessStartInfo start = new ProcessStartInfo();
         start.FileName = "my/full/path/to/python.exe";
         start.Arguments = string.Format("{0} {1}", cmd, args);
         start.UseShellExecute = false;
         start.RedirectStandardOutput = true;
         using (Process process = Process.Start(start))
         {
            using (StreamReader reader = process.StandardOutput)
            {
               string result = reader.ReadToEnd();
               Console.Write(result);
            }
         }*/
      }
   }
}
