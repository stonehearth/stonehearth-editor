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

        private const int kRecipeColCount = 6;
        private const int kIngredientColCount = 3;

        private DataTable mDataTable = new DataTable();
        private int mIngredientColumns = 0;
        private bool mBaseModsOnly = true;

        public RecipesView()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            LoadColumnsData();
        }

        private void LoadColumnsData()
        {
            // Add recipe columns
            mDataTable.Columns.Add(new DataColumn(kIcon, typeof(Image)));
            mDataTable.Columns.Add(new DataColumn(kAlias, typeof(string)));
            mDataTable.Columns.Add(new DataColumn(kDisplayName, typeof(string)));
            mDataTable.Columns.Add(new DataColumn(kLvlReq, typeof(int)));
            mDataTable.Columns.Add(new DataColumn(kNetWorth, typeof(int)));
            mDataTable.Columns.Add(new DataColumn(kEffort, typeof(int)));

            foreach (Module module in ModuleDataManager.GetInstance().GetAllModules())
            {
                bool shouldIncludeMod = mBaseModsOnly ? (module.Name == "stonehearth" || module.Name == "rayyas_children") : true;
                ModuleFile jobsIndex = module.GetAliasFile("jobs:index");
                if (shouldIncludeMod && jobsIndex != null)
                {
                    JObject jobIndexJson = (jobsIndex.FileData as JsonFileData).Json;
                    JObject jobs = jobIndexJson["jobs"] as JObject;

                    foreach (JProperty job in jobs.Properties())
                    {
                        LoadRecipesForJob(job.Value["description"].ToString());
                    }
                }
            }

            //"stonehearth: data: resource_constants"

            this.recipesGridView.DataSource = mDataTable;
            ClearBrokenImages();
        }

        // Remove [x] broken image icons
        private void ClearBrokenImages()
        {

            foreach (var column in this.recipesGridView.Columns)
            {
                if (column is DataGridViewImageColumn)
                {
                    (column as DataGridViewImageColumn).DefaultCellStyle.NullValue = null;
                }
            }
        }

        private void LoadRecipesForJob(string jobAlias)
        {
            string modName = jobAlias.Split(':')[0];
            int index = jobAlias.IndexOf(':');
            string jobKey = jobAlias.Substring(index + 1);
            char sep = System.IO.Path.DirectorySeparatorChar;
            string recipeFileDataKey = modName + sep + "aliases" + sep + jobKey + sep + "recipes";

            JsonFileData recipesIndex = ModuleDataManager.GetInstance().GetSelectedFileData(recipeFileDataKey) as JsonFileData;

            // Non-crafter jobs will not have recipes
            if (recipesIndex == null)
                return;

            Dictionary<string, FileData> recipeFileData = recipesIndex.LinkedFileData;
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

                        var foundLinked = false;
                        // Check aliases linked by recipe file
                        foreach (ModuleFile linkedAlias in jsonFileData.LinkedAliases)
                        {
                            if (linkedAlias.FullAlias.Equals(alias))
                            {
                                JsonFileData linked = linkedAlias.FileData as JsonFileData;
                                PopulationRecipeRow(row, linked);
                                foundLinked = true;
                            }
                        }

                        if (!foundLinked)
                        {
                            PopulationRecipeRow(row, jsonFileData);
                        }
                    }
                }

                int ingredientCount = 1;
                foreach (JToken ingredient in ingredientArray)
                {
                    // If we don't have enough columns for this ingredient, add a new set of columns
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

                    if (uri != null)
                    {
                        var foundLinked = false;
                        foreach (ModuleFile linkedAlias in jsonFileData.LinkedAliases)
                        {
                            if (linkedAlias.FullAlias.Equals(alias))
                            {
                                JsonFileData linked = linkedAlias.FileData as JsonFileData;
                                PopulateIngredientRow(row, linked, prefix);
                                foundLinked = true;
                            }
                        }

                        if (!foundLinked)
                        {
                            PopulateIngredientRow(row, jsonFileData, prefix);
                        }

                        ingredientCount++;
                    }
                }

                mDataTable.Rows.Add(row);
            }
        }

        private void PopulateIngredientRow(DataRow row, JsonFileData jsonFileData, string prefix)
        {
            row[prefix + kName] = GetTranslatedName(GetDisplayName(jsonFileData));
            row[prefix + kIcon] = GetIcon(jsonFileData);
        }

        private void PopulationRecipeRow(DataRow row, JsonFileData jsonFileData)
        {
            row[kNetWorth] = jsonFileData.NetWorth;
            row[kDisplayName] = GetTranslatedName(GetDisplayName(jsonFileData));
            row[kIcon] = GetIcon(jsonFileData);
        }

        private string GetDisplayName(JsonFileData jsonFileData)
        {
            JsonFileData mixinsFile = jsonFileData.CreateFileWithMixinsApplied();
            JToken displayName = mixinsFile.Json.SelectToken("entity_data.stonehearth:catalog.display_name");
            return displayName.ToString();

        }

        private Image GetIcon(JsonFileData jsonFileData)
        {
            foreach (KeyValuePair<string, FileData> kv in jsonFileData.LinkedFileData)
            {
                FileData fileData = kv.Value;
                string path = "";
                if (fileData is JsonFileData)
                {
                    path = (fileData as JsonFileData).FindImageForFile();
                }
                else if (fileData is ImageFileData)
                {
                    path = fileData.Path;
                }

                if (path != "" && System.IO.File.Exists(path))
                {
                    return ThumbnailCache.GetThumbnail(path);
                }
            }

            return null;
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
            for (int i = mDataTable.Columns.Count; i > mDataTable.Columns.Count - kIngredientColCount; i--)
            {
                mDataTable.Columns.RemoveAt(i);
            }
        }

        // TODO: implement
        private FileData GetFileDataForCell(int row, int column)
        {
            throw new NotImplementedException();
        }

        private string GetIngredientPrefix(int columnNum)
        {
            return "C" + columnNum + " ";
        }

        private string GetTranslatedName(string locKey)
        {
            return ModuleDataManager.GetInstance().LocalizeString(locKey, true);
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
