using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace StonehearthEditor
{
    public partial class RecipesView : UserControl
    {
        private const string kIcon = "Icon";
        private const string kAlias = "Alias";
        private const string kDisplayName = "Display Name";
        private const string kLvlReq = "Lvl Req";
        private const string kNetWorth = "Net Worth";
        private const string kEffort = "Effort";

        private const string kName = "Name";
        private const string kAmount = "Amount";

        DataTable mDataTable = new DataTable();
        int mIngredientColumns = 0;

        public RecipesView()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            SetColumnsData();
        }

        private void SetColumnsData()
        {
            mDataTable.Columns.Add(new DataColumn(kIcon, typeof(Image)));
            mDataTable.Columns.Add(new DataColumn(kAlias, typeof(string)));
            mDataTable.Columns.Add(new DataColumn(kDisplayName, typeof(string)));
            mDataTable.Columns.Add(new DataColumn(kLvlReq, typeof(int)));
            mDataTable.Columns.Add(new DataColumn(kNetWorth, typeof(int)));
            mDataTable.Columns.Add(new DataColumn(kEffort, typeof(int)));


            AddIngredientColumn();
            // populateColums

            //dt.Columns["colStatus"].Expression = String.Format("IIF(colBestBefore < #{0}#, 'Ok','Not ok')", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //dt.Rows.Add(DateTime.Now.AddDays(-1));
            //dt.Rows.Add(DateTime.Now.AddDays(1));
            //dt.Rows.Add(DateTime.Now.AddDays(2));
            //dt.Rows.Add(DateTime.Now.AddDays(-2));


            ModuleFile m = ModuleDataManager.GetInstance().GetModuleFile("stonehearth:jobs:engineer");
            JsonFileData f = ModuleDataManager.GetInstance().GetSelectedFileData("stonehearth\\aliases\\jobs:engineer\\recipes") as JsonFileData;
            //"stonehearth: data: resource_constants"
            Dictionary<string, FileData> recipeFileData = f.LinkedFileData;
            foreach (KeyValuePair<string, FileData> recipe in recipeFileData)
            {
                DataRow row = mDataTable.NewRow();

                JsonFileData jsonFileData = recipe.Value as JsonFileData;
                JObject recipeJson = jsonFileData.Json;
                JArray ingredientArray = recipeJson["ingredients"] as JArray;
                JArray productArray = recipeJson["produces"] as JArray;

                JToken lvlReq = recipeJson["level_requirement"];
                row[kLvlReq] = lvlReq == null ? 0 : lvlReq.ToObject<int>();
                row[kEffort] = recipeJson["work_units"].ToObject<int>();

                foreach (JToken product in productArray)
                {
                    JToken item = product["item"];
                    if (item != null)
                    {
                        string alias = item.ToString();
                        row[kAlias] = alias;

                        // Check aliases linked by recipe file
                        foreach (ModuleFile linkedAlias in jsonFileData.LinkedAliases)
                        {
                            if (linkedAlias.FullAlias.Equals(alias))
                            {
                                JsonFileData linked = linkedAlias.FileData as JsonFileData;
                                row[kNetWorth] = linked.NetWorth;

                                // TODO refactor this
                                foreach (KeyValuePair<string, FileData> fd in linked.LinkedFileData)
                                {
                                    // Get image
                                    string path = "";
                                    if (fd.Value is JsonFileData)
                                    {
                                        path = (fd.Value as JsonFileData).FindImageForFile();
                                        JToken displayName = (fd.Value as JsonFileData).Json.SelectToken("entity_data.stonehearth:catalog.display_name");
                                        if (displayName != null)
                                        {
                                            row[kDisplayName] = displayName.ToString();
                                        }
                                    } else if (fd.Value is ImageFileData)
                                    {
                                        path = fd.Value.Path;
                                    }

                                    if (path != "" && System.IO.File.Exists(path))
                                    {
                                        row[kIcon] = ThumbnailCache.GetThumbnail(path);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                int ingredientCount = 1;
                foreach (JToken ingredient in ingredientArray)
                {
                    if (ingredientCount > mIngredientColumns)
                    {
                        AddIngredientColumn();
                    }

                    JToken uri = ingredient["uri"];
                    JToken material = ingredient["material"];
                    JToken count = ingredient["count"];

                    string prefix = GetIngredientPrefix(ingredientCount);
                    string alias = uri != null ? uri.ToString() : material.ToString();
                    row[prefix + kName] = alias;
                    row[prefix + kAmount] = count.ToObject<int>();

                    // TODO: look in a different place for icons if it is a material

                    foreach (ModuleFile linkedAlias in jsonFileData.LinkedAliases)
                    {
                        if (linkedAlias.FullAlias.Equals(alias))
                        {
                            JsonFileData linked = linkedAlias.FileData as JsonFileData;
                            JToken displayName = linked.Json.SelectToken("entity_data.stonehearth:catalog.display_name");
                            if (displayName != null)
                            {
                                row[prefix + kName] = displayName.ToString();
                            }

                            // TODO refactor this
                            foreach (KeyValuePair<string, FileData> fd in linked.LinkedFileData)
                            {
                                string path = "";
                                if (fd.Value is JsonFileData)
                                {
                                    path = (fd.Value as JsonFileData).FindImageForFile();
                                }
                                else if (fd.Value is ImageFileData)
                                {
                                    path = fd.Value.Path;
                                }

                                if (path != "" && System.IO.File.Exists(path))
                                {
                                    row[prefix + kIcon] = ThumbnailCache.GetThumbnail(path);
                                    break;
                                }
                            }
                        }
                    }

                    ingredientCount++;
                }

                mDataTable.Rows.Add(row);
            }

            this.recipesGridView.DataSource = mDataTable;
        }

        // Add a single ingredient column
        private void AddIngredientColumn()
        {
            mIngredientColumns++;
            string prefix = GetIngredientPrefix(mIngredientColumns);
            mDataTable.Columns.Add(new DataColumn(prefix + kIcon, typeof(Image)));
            mDataTable.Columns.Add(new DataColumn(prefix + kName, typeof(string)));
            mDataTable.Columns.Add(new DataColumn(prefix + kAmount, typeof(int)));
        }

        private void RemoveIngredientColumn()
        {
            int recipeColumnCount = 6; // TODO: put the column names in a table, and get the count from there
            int ingredientsColumnCount = 3;
            for (int i = mDataTable.Columns.Count; i > mDataTable.Columns.Count - ingredientsColumnCount; i--)
            {
                mDataTable.Columns.RemoveAt(i);
            }
        }

        private string GetIngredientPrefix(int columnNum)
        {
            return "C" + columnNum + " ";
        }

        private void recipeGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                recipesCellContextMenu.Show(recipesGridView, new Point(e.X, e.Y));
            }
        }
    }
}
