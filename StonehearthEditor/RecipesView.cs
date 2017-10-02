﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
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
        private const string kNetWorth = "Net Worth";
        private const string kCrafter = "R Crafter";
        private const string kLvlReq = "R Lvl Req";
        private const string kEffort = "R Effort";
        private const string kIngr = "Ingr";
        private const string kName = "Name";
        private const string kAmount = "Amount";
        private const string kAllCol = "All Columns";

        private const int kRecipeColCount = 7;
        private const int kIngredientColCount = 3;

        private int mIngredientColumns = 0;
        private bool mBaseModsOnly = true;

        private DataTable mDataTable = new DataTable();
        private HashSet<DataGridViewRow> mModifiedRows = new HashSet<DataGridViewRow>();
        private HashSet<int> comboBoxColumns = new HashSet<int>();
        private new Dictionary<DataRow, RowMetadata> mRowFileDataIndex = new Dictionary<DataRow, RowMetadata>();

        // Cached data
        private Dictionary<string, Image> mMaterialImages = new Dictionary<string, Image>();
        private List<string> mJobAliases = new List<string>();

        public RecipesView()
        {
            InitializeComponent();
            filterByColumn.Items.Add(kAllCol);
            filterByColumn.Items.Add(kAlias);
            filterByColumn.Items.Add(kDisplayName);
            filterByColumn.Items.Add(kCrafter);
            filterByColumn.Items.Add(kLvlReq);
            filterByColumn.Items.Add(kNetWorth);
            filterByColumn.Items.Add(kEffort);
            filterByColumn.Items.Add(kIngr + " " + kName);
            filterByColumn.Items.Add(kIngr + " " + kAmount);
            filterByColumn.SelectedIndex = 0;
        }

        public void Initialize()
        {
            DateTime mStart = DateTime.Now;
            MakeDoubleBuffered();
            LoadMaterialImages();
            LoadColumnsData();
        }

        // Helps address DataGridView's slow repaint time. See https://www.codeproject.com/Tips/654101/Double-Buffering-a-DataGridview
        private void MakeDoubleBuffered()
        {
            BindingFlags bindings = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty;
            typeof(DataGridView).InvokeMember("DoubleBuffered", bindings, null, recipesGridView, new object[] { true });
        }

        private void LoadColumnsData()
        {
            // Add recipe columns
            mDataTable.Columns.Add(new DataColumn(kIcon, typeof(Image)));
            mDataTable.Columns.Add(new DataColumn(kAlias, typeof(string)));
            mDataTable.Columns.Add(new DataColumn(kDisplayName, typeof(string)));
            mDataTable.Columns.Add(new DataColumn(kNetWorth, typeof(int)));
            mDataTable.Columns.Add(new DataColumn(kCrafter, typeof(string)));
            mDataTable.Columns.Add(new DataColumn(kLvlReq, typeof(int)));
            mDataTable.Columns.Add(new DataColumn(kEffort, typeof(int)));

            LoadAllRecipes();

            recipesGridView.DataSource = mDataTable;

            ConfigureColumns();
        }

        private void ConfigureColumns()
        {
            for (int i = 0; i < recipesGridView.Columns.Count; i++)
            {
                var column = recipesGridView.Columns[i];

                // Add combo boxes for columns that have a predetermined set of valid values
                if ((column.Name.StartsWith(kIngr) && column.Name.EndsWith(kName)) ||
                    (column.Name == kCrafter))
                {
                    var newColumn = new DataGridViewComboBoxColumn();
                    newColumn.Name = column.Name;
                    newColumn.DataPropertyName = column.DataPropertyName;
                    newColumn.AutoComplete = false;
                    newColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                    newColumn.DataSource = GetAutoCompleteStrings(newColumn.Name);

                    recipesGridView.Columns.RemoveAt(i);
                    recipesGridView.Columns.Insert(i, newColumn);
                    comboBoxColumns.Add(i);
                    column = newColumn;
                }

                // Remove [x] broken image icons
                if (column is DataGridViewImageColumn)
                {
                    (column as DataGridViewImageColumn).DefaultCellStyle.NullValue = null;
                }

                // Color ingredient columns
                Regex matchOdd = new Regex(kIngr + "[13579]");
                Regex matchEven = new Regex(kIngr + "[02468]");
                if (matchOdd.IsMatch(column.Name))
                {
                    column.DefaultCellStyle.BackColor = Color.LemonChiffon;
                }
                else if (matchEven.IsMatch(column.Name))
                {
                    column.DefaultCellStyle.BackColor = Color.LightBlue;
                } else if (column.Name.StartsWith("R "))
                {
                    column.DefaultCellStyle.BackColor = Color.Azure;
                }
            }

            recipesGridView.Columns[kAlias].ReadOnly = true;
        }

        private void LoadAllRecipes()
        {
            foreach (Module module in ModuleDataManager.GetInstance().GetAllModules())
            {
                bool shouldIncludeMod = mBaseModsOnly ? (module.Name == "stonehearth" || module.Name == "rayyas_children") : true;
                if (shouldIncludeMod)
                {
                    ModuleFile jobsIndex = module.GetAliasFile("jobs:index");
                    if (jobsIndex != null)
                    {
                        JObject jobIndexJson = (jobsIndex.FileData as JsonFileData).Json;
                        JObject jobs = jobIndexJson["jobs"] as JObject;

                        foreach (JProperty job in jobs.Properties())
                        {
                            string jobAlias = job.Value["description"].ToString();
                            mJobAliases.Add(jobAlias);
                            LoadRecipesForJob(jobAlias);
                        }
                    }
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
                RowMetadata rmd = new RowMetadata();
                mRowFileDataIndex[row] = rmd;

                rmd.RecipeList = recipesIndex;

                JsonFileData jsonFileData = recipe.Value as JsonFileData;
                rmd.Recipe = jsonFileData;
                JObject recipeJson = jsonFileData.Json;
                JArray ingredientArray = recipeJson["ingredients"] as JArray;
                JArray productArray = recipeJson["produces"] as JArray;

                JToken lvlReq = recipeJson["level_requirement"];
                row[kLvlReq] = lvlReq == null ? 0 : lvlReq.ToObject<int>();
                row[kEffort] = recipeJson["work_units"].ToObject<int>();
                row[kCrafter] = jobAlias;

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
                                rmd.AddItem(linked);
                                foundLinked = true;
                            }
                        }

                        if (!foundLinked)
                        {
                            PopulationRecipeRow(row, jsonFileData);
                            rmd.AddItem(jsonFileData);
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

                    if (material != null)
                    {
                        Image icon;
                        mMaterialImages.TryGetValue(material.ToString(), out icon);
                        if (icon != null)
                        {
                            row[prefix + kIcon] = icon;
                        }
                    }
                    else if (uri != null)
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
            row[prefix + kName] = jsonFileData.GetModuleFile().FullAlias;
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
                    path = (fileData as JsonFileData).GetImageForFile();
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

        // Load the material constants from the stonehearth mod. This needs to be done before the data columns
        // are populated,since they use this map to fill in data grid for material ingredients
        private void LoadMaterialImages()
        {
            Module sh = ModuleDataManager.GetInstance().GetMod("stonehearth");
            ModuleFile resourceConstants = sh.GetAliasFile("data:resource_constants");
            if (resourceConstants != null)
            {
                JsonFileData jsonFileData = resourceConstants.FileData as JsonFileData;
                JObject resourceConstantsJson = jsonFileData.Json;
                JObject resourceJson = resourceConstantsJson["resources"] as JObject;

                foreach (JProperty resource in resourceJson.Properties())
                {
                    string name = resource.Name;
                    string iconLocation = resource.Value["icon"].ToString();
                    string path = JsonHelper.GetFileFromFileJson(iconLocation, jsonFileData.Directory);
                    if (path != "" && System.IO.File.Exists(path))
                    {
                        mMaterialImages.Add(name, ThumbnailCache.GetThumbnail(path));
                    }
                }
            }
        }

        private void SetGridValue(int col, int row, string value)
        {
            try
            {
                if (recipesGridView.Columns[col].ValueType == typeof(int))
                {
                    recipesGridView[col, row].Value = int.Parse(value);
                }
                else
                {
                    bool valid = true;
                    if (recipesGridView.Columns[col] is DataGridViewComboBoxColumn)
                    {
                        List<string> autoCompleteStrings = (recipesGridView.Columns[col] as DataGridViewComboBoxColumn).DataSource as List<string>;
                        if (!autoCompleteStrings.Contains(value))
                            valid = false;
                    }

                    if (valid)
                        recipesGridView[col, row].Value = value;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("Unable to set grid value for (%d, %d). Error: %s", col, row, exception.Message));
                return;
            }
        }

        private FileData GetFileDataForCell(int rowIndex, int colIndex)
        {
            DataGridViewRow row = recipesGridView.Rows[rowIndex];
            DataRow dataRow = (row.DataBoundItem as DataRowView).Row;
            // TODO: implement

            throw new NotImplementedException();
        }

        private string GetIngredientPrefix(int columnNum)
        {
            return kIngr + columnNum + " ";
        }

        private string GetTranslatedName(string locKey)
        {
            return ModuleDataManager.GetInstance().LocalizeString(locKey, true);
        }

        private Tuple<DataGridViewCell, DataGridViewCell> GetSelectedCellsBoundaries()
        {
            if (recipesGridView.SelectedCells.Count == 0)
                return null;

            int startRow = recipesGridView.Rows.Count - 1; ;
            int startCol = recipesGridView.Columns.Count - 1; ;
            int endRow = 0;
            int endCol = 0;

            foreach (DataGridViewCell cell in recipesGridView.SelectedCells)
            {
                // Get smallest row/col
                if (cell.RowIndex < startRow)
                    startRow = cell.RowIndex;
                if (cell.ColumnIndex < startCol)
                    startCol = cell.ColumnIndex;

                // Get largest row/col
                if (cell.RowIndex > endRow)
                    endRow = cell.RowIndex;
                if (cell.ColumnIndex > endCol)
                    endCol = cell.ColumnIndex;
            }

            return Tuple.Create<DataGridViewCell, DataGridViewCell>(recipesGridView[startCol, startRow], recipesGridView[endCol, endRow]);
        }

        private string GetColFilterString(string colName, string searchTerm)
        {
            return string.Format("Convert([{0}], 'System.String') LIKE '%{1}%'", colName, searchTerm);
        }

        private List<string> GetAutoCompleteStrings(string colName)
        {
            List<string> strings = new List<string>();
            if (colName.StartsWith(kIngr) && colName.EndsWith(kName))
            {
                // Add materials
                foreach (KeyValuePair<string, Image> kv in mMaterialImages)
                {
                    strings.Add(kv.Key);
                }

                ModuleDataManager mdm = ModuleDataManager.GetInstance();
                Dictionary<string, JsonFileData> itemJsons = mdm.GetJsonsByTerm("entity_data.stonehearth:net_worth");
                foreach (KeyValuePair<string, JsonFileData> kv in itemJsons)
                {
                    string modName = mdm.GetModNameFromAlias(kv.Key);
                    bool shouldIncludeMod = mBaseModsOnly ? (modName == "stonehearth" || modName == "rayyas_children") : true;
                    if (shouldIncludeMod)
                    {
                        strings.Add(kv.Key);
                    }
                }
            }
            else if (colName == kCrafter)
            {
                strings.AddRange(mJobAliases.ToArray());
            }

            return strings;
        }

        private void searchBox_Filter(object sender, EventArgs e)
        {
            string searchTerm = searchBox.Text.ToString();
            string colFilter = filterByColumn.Text;
            if (searchTerm != "")
            {
                searchBox.BackColor = Color.Gold;
            }
            else
            {
                searchBox.BackColor = Color.White;
            }

            StringBuilder sb = new StringBuilder();
            // Filter by all columns or all ingredient columns
            if (colFilter == kAllCol || colFilter.Contains(kIngr))
            {
                Regex matchIng = new Regex(kIngr + "\\d");
                for (int i = 0; i < recipesGridView.Columns.Count; i++)
                {
                    DataGridViewColumn column = recipesGridView.Columns[i];
                    bool isText = column.ValueType == typeof(string) || column.ValueType == typeof(int);
                    bool match = colFilter.Contains(kIngr) ? matchIng.IsMatch(column.Name) : isText;
                    if (match)
                    {
                        sb.Append(GetColFilterString(column.Name, searchTerm));
                        if (i < recipesGridView.Columns.Count - 1)
                        {
                            sb.Append(" OR ");
                        }
                    }
                }
            }
            else
            {
                // Filter by specific columns
                sb.Append(GetColFilterString(colFilter, searchTerm));
            }

            mDataTable.DefaultView.RowFilter = sb.ToString();
        }

        private void recipesGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                recipesCellContextMenu.Show(recipesGridView, new Point(e.X, e.Y));
            }
        }

        private void recipesGridView_KeyDown(object sender, KeyEventArgs e)
        {
            // Paste multiple cells
            // TODO: repeat last clipboard value row if columns selected exceeds clipboard columns
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (recipesGridView.SelectedCells.Count == 0)
                {
                    return;
                }

                Tuple<DataGridViewCell, DataGridViewCell> boundaryCells = GetSelectedCellsBoundaries();
                DataGridViewCell startCell = boundaryCells.Item1;
                DataGridViewCell endCell = boundaryCells.Item2;
                int startRow = startCell.RowIndex;
                int startCol = startCell.ColumnIndex;

                int rowIndex = startRow;
                string s = Clipboard.GetText();
                string[] lines = s.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.None);
                int lineIndex = 0;
                while (lineIndex < lines.Count())
                {
                    string line = lines[lineIndex];
                    if (rowIndex < recipesGridView.Rows.Count)
                    {
                        string[] cells = line.Split('\t');
                        int colIndex = startCol;
                        for (int i = 0; i < cells.Length && colIndex < recipesGridView.Columns.Count; i++)
                        {
                            SetGridValue(colIndex, rowIndex, cells[i]);
                            colIndex++;
                        }

                        rowIndex++;
                    }

                    lineIndex++;

                    // Repeat last line in clipboard if selected rows exceeds rows in clipboard
                    if (lineIndex == lines.Count())
                    {
                        int selectedRowCount = Math.Abs(startCell.RowIndex - endCell.RowIndex) + 1;
                        int clipboardRowCount = Math.Abs((rowIndex - 1) - startRow) + 1;
                        if (selectedRowCount > clipboardRowCount)
                        {
                            lineIndex--;
                        }
                    }
                }
            }
        }

        private void recipesGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (comboBoxColumns.Contains(recipesGridView.CurrentCell.ColumnIndex))
                            {
                ComboBox cbx = (ComboBox)e.Control;
                cbx.DropDownStyle = ComboBoxStyle.DropDown;
                cbx.AutoCompleteSource = AutoCompleteSource.ListItems;
                cbx.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                if ((string)cbx.Tag != "seen")
                {
                    cbx.Tag = "seen";
                    cbx.KeyDown += (s, a) =>
                    {
                        if (cbx.DroppedDown)
                        {
                            cbx.DroppedDown = false;
                        }
                    };
                }
            }
        }

        private void recipesGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = recipesGridView.CurrentCell.RowIndex;
            mModifiedRows.Add(recipesGridView.Rows[rowIndex]);
            // TODO: update modified rows on save
        }

        private void recipesGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = true;
        }
    }

    internal class RowMetadata
    {
        public JsonFileData RecipeList { get; set; }

        public JsonFileData Recipe { get; set; }

        public List<JsonFileData> Items { get; } = new List<JsonFileData>();

        public void AddItem(JsonFileData item)
        {
            Items.Add(item);
        }

    }
}
