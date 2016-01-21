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
   public partial class NetWorthVisualizer : Form
   {
      private Dictionary<int, List<JsonFileData>> mNetWorthValues = new Dictionary<int, List<JsonFileData>>();
      private int mMaxNetWorth = 0;
      private int mItemCount = 0;
      private ManifestView mManifestView;

      public NetWorthVisualizer()
      {
         UpdateNetWorthData();
         InitializeComponent();
      }

      public void SetManifestView(ManifestView view)
      {
         mManifestView = view;
      }

      private string FindImageForFile(JsonFileData data)
      {
         foreach (KeyValuePair<string, FileData> linkedFile in data.LinkedFileData)
         {
            if ((linkedFile.Value is ImageFileData) && System.IO.File.Exists(linkedFile.Value.Path))
            {
               return linkedFile.Value.Path;
            }
         }
         return string.Empty;
      }

      private static int kCellSize = 40;
      public void UpdateNetWorthData()
      {
         mItemCount = 0;
         foreach (Module mod in ModuleDataManager.GetInstance().GetAllModules())
         {
            foreach (ModuleFile file in mod.GetAliases())
            {
               JsonFileData data = file.FileData as JsonFileData;
               if (data == null)
               {
                  continue;
               }
               int netWorth = data.NetWorth;
               if (netWorth <= 0)
               {
                  continue;
               }
               string imageFile = FindImageForFile(data);
               if (string.IsNullOrEmpty(imageFile))
               {
                  Console.WriteLine("file " + data.FileName + " has no icon!");
                  continue;
               }

               if (netWorth > mMaxNetWorth)
               {
                  mMaxNetWorth = netWorth;
               }
               List<JsonFileData> list;
               if (!mNetWorthValues.TryGetValue(netWorth, out list))
               {
                  list = new List<JsonFileData>();
                  mNetWorthValues[netWorth] = list;
               }
               list.Add(data);
               if (list.Count > mItemCount)
               {
                  mItemCount = list.Count;
               }
            }
         }
         //canvas.Refresh();
      }

      private void openFileButton_Click(object sender, EventArgs e)
      {
         // open file
         PictureBox label = sender as PictureBox;
         if (label != null && label.Tag != null)
         {
            JsonFileData fileData = label.Tag as JsonFileData;
            mManifestView.SetSelectedFileData(fileData);
         }
      }

      private void canvas_Paint(object sender, PaintEventArgs e)
      {
         Graphics graphics = e.Graphics;

         int maxCols = Math.Min(mMaxNetWorth, 50);
         int maxRows = Math.Min(mItemCount, 50);
         int canvasWidth = maxCols * (kCellSize + 1);
         int canvasHeight = maxRows * (kCellSize + 1) + 10;

         canvas.Width = canvasWidth;
         canvas.Height = canvasHeight;

         for (int i = 0; i < maxCols; ++i)
         {
            string colName = "" + (i + 1);
            Point position = new Point(i * kCellSize, canvasHeight - 15);
            graphics.DrawString(colName, SystemFonts.DefaultFont, Brushes.Black, position);

            List<JsonFileData> list;
            if (mNetWorthValues.TryGetValue(i + 1, out list))
            {
               int count = Math.Min(maxRows, list.Count);
               for (int j = 0; j < count; j++)
               {
                  JsonFileData data = list[j];
                  string imageFile = FindImageForFile(data);
                  if (string.IsNullOrEmpty(imageFile))
                  {
                     Console.WriteLine("file " + data.FileName + " has no icon!");
                  }
                  else
                  {
                     Image image = Image.FromFile(imageFile);
                     Image resized = image.GetThumbnailImage(kCellSize, kCellSize, null, IntPtr.Zero);
                     Rectangle location = new Rectangle(i * kCellSize, j * kCellSize, kCellSize, kCellSize);
                     graphics.DrawImageUnscaledAndClipped(resized, location);
                  }
               }
            }
         }

         for (int i = 0; i < canvasWidth; i += kCellSize)
         {
            graphics.DrawLine(System.Drawing.Pens.Black, new Point(i, 0), new Point(i, canvasHeight));
         }
         for (int j = 0; j < canvasHeight; j += kCellSize)
         {
            graphics.DrawLine(System.Drawing.Pens.Black, new Point(0, j), new Point(canvasWidth, j));
         }
      }

      private void canvas_DoubleClick(object sender, EventArgs e)
      {

      }
   }
}
