using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace StonehearthEditor
{
    public partial class NetWorthVisualizer : Form
    {
        private const float kMinZoom = 0.1f;
        private const float kMaxZoom = 1.0f;

        private Dictionary<int, List<JsonFileData>> mNetWorthValues;
        private int mMaxNetWorth = 0;
        private int mItemCount = 0;
        private ManifestView mManifestView;
        private JsonFileData mHoveredFileData = null;
        private float mZoom = 1.0f;

        public NetWorthVisualizer()
        {
            InitializeComponent();
            UpdateNetWorthData();
        }

        public void SetManifestView(ManifestView view)
        {
            mManifestView = view;
        }

        public void UpdateNetWorthData()
        {
            mItemCount = 0;
            mMaxNetWorth = 0;
            mNetWorthValues = new Dictionary<int, List<JsonFileData>>();
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

                    string imageFile = data.GetImageForFile();
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

            canvas.Refresh();
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

        private static int kBottomOffset = 10;
        private static int kStringOffset = 15;
        private static int kCellSize = 40;
        private static int kMaxRows = 1000;
        private static float kMaxRecommendedMultiplier = 1.6f;
        private static float kMinRecommendedMultiplier = 0.9f;

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            int cellSizeZoomed = (int)Math.Round(kCellSize * mZoom);

            int maxCols = Math.Min(mMaxNetWorth, kMaxRows);
            int maxRows = Math.Min(mItemCount, kMaxRows);
            int canvasWidth = maxCols * (cellSizeZoomed + 1);
            int canvasHeightLimit = maxRows * (cellSizeZoomed + 1);
            int canvasHeight = (maxRows * (cellSizeZoomed + 1)) + kBottomOffset;

            canvas.Width = canvasWidth;
            canvas.Height = canvasHeight;

            for (int i = 0; i < maxCols; ++i)
            {
                if (mZoom > 0.25f || (mZoom == 0.25f && ((i + 1) % 10) == 0))
                {
                    string colName = "" + (i + 1);
                    Point position = new Point(i * cellSizeZoomed, canvasHeight - kStringOffset);
                    graphics.DrawString(colName, SystemFonts.DefaultFont, Brushes.Black, position);
                }
                else
                {
                }

                List<JsonFileData> list;
                if (mNetWorthValues.TryGetValue(i + 1, out list))
                {
                    int count = Math.Min(maxRows, list.Count);
                    for (int j = 0; j < count; j++)
                    {
                        JsonFileData data = list[j];
                        string imageFile = data.GetImageForFile();
                        if (string.IsNullOrEmpty(imageFile))
                        {
                            Console.WriteLine("file " + data.FileName + " has no icon!");
                        }
                        else
                        {
                            Image thumbnail = ThumbnailCache.GetThumbnail(imageFile);

                            int ylocation = canvasHeight - ((j + 1) * cellSizeZoomed) - maxRows - kBottomOffset - 1;
                            Rectangle location = new Rectangle(i * cellSizeZoomed, ylocation, cellSizeZoomed, cellSizeZoomed);
                            graphics.DrawImage(thumbnail, location);

                            if (data.RecommendedMaxNetWorth > 0)
                            {
                                int cost = i + 1;
                                bool shouldWarn = false;
                                JToken sellable = data.Json.SelectToken("entity_data.stonehearth:net_worth.shop_info.sellable");
                                if (sellable != null && sellable.ToString() == "False")
                                {
                                    shouldWarn = true;
                                }

                                if (cost < data.RecommendedMinNetWorth * kMinRecommendedMultiplier)
                                {
                                    shouldWarn = true;
                                }

                                if (cost > ((data.RecommendedMaxNetWorth * kMaxRecommendedMultiplier) + 1))
                                {
                                    shouldWarn = true;
                                }

                                if (shouldWarn)
                                {
                                    Pen semiRed = new Pen(Color.FromArgb(100, Color.Red));
                                    graphics.FillRectangle(semiRed.Brush, location);
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < canvasWidth; i += cellSizeZoomed)
            {
                graphics.DrawLine(System.Drawing.Pens.Black, new Point(i, 0), new Point(i, canvasHeightLimit));
            }

            for (int j = 0; j < canvasHeightLimit; j += cellSizeZoomed)
            {
                graphics.DrawLine(System.Drawing.Pens.Black, new Point(0, j), new Point(canvasWidth, j));
            }
        }

        private void canvas_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int maxRows = Math.Min(mItemCount, kMaxRows);
            int cellSizeZoomed = (int)Math.Round(kCellSize * mZoom);
            int x = e.X / cellSizeZoomed;
            int y = (canvas.Height - e.Y - maxRows - kBottomOffset) / cellSizeZoomed;
            List<JsonFileData> list;
            if (mNetWorthValues.TryGetValue(x + 1, out list))
            {
                if (y < list.Count)
                {
                    mManifestView.SetSelectedFileData(list[y]);
                }
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = canvas.PointToClient(Cursor.Position);

            int cellSizeZoomed = (int)Math.Round(kCellSize * mZoom);
            int maxRows = Math.Min(mItemCount, kMaxRows);
            int x = pos.X / cellSizeZoomed;
            int y = (canvas.Height - pos.Y - maxRows - kBottomOffset) / cellSizeZoomed;
            List<JsonFileData> list;
            if (mNetWorthValues.TryGetValue(x + 1, out list))
            {
                if (y < list.Count && y >= 0)
                {
                    if (list[y] != mHoveredFileData)
                    {
                        pos.X = pos.X + 2;
                        pos.Y = pos.Y + 2;
                        mHoveredFileData = list[y];
                        string tooltip = mHoveredFileData.FileName;
                        if (mHoveredFileData.RecommendedMinNetWorth >= 0)
                        {
                            tooltip = tooltip + "\n Recommended Net Worth: " + mHoveredFileData.RecommendedMinNetWorth + " - " + (mHoveredFileData.RecommendedMaxNetWorth * kMaxRecommendedMultiplier);
                            tooltip = tooltip + "\n Average: " + mHoveredFileData.RecommendedMaxNetWorth;
                        }

                        imageTooltip.Show(tooltip, canvas, pos);
                    }

                    return;
                }
            }

            mHoveredFileData = null;
            imageTooltip.Hide(canvas);
        }

        private void zoomInButton_Click(object sender, EventArgs e)
        {
            float newZoom = mZoom + 0.25f;
            if (mZoom == kMinZoom)
            {
                newZoom = 0.25f;
            }

            newZoom = Math.Min(newZoom, kMaxZoom);
            if (newZoom != mZoom)
            {
                mZoom = newZoom;
                canvas.Refresh();
            }
        }

        private void zoomOutButton_Click(object sender, EventArgs e)
        {
            float newZoom = mZoom - 0.25f;
            newZoom = Math.Max(newZoom, kMinZoom);
            if (newZoom != mZoom)
            {
                mZoom = newZoom;
                canvas.Refresh();
            }
        }
    }
}
