using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace StonehearthEditor.Recipes
{
    public partial class RecipesView : UserControl
    {
        public const string kAllCol = "All Columns";

        private bool mBaseModsOnly = true;

        // Cell with extra getters for the DataRow/Column
        private struct Cell
        {
            public RecipeRow Row { get; private set; }

            public DataColumn Column { get; private set; }

            public Cell(RecipeRow row, DataColumn column)
            {
                this.Row = row;
                this.Column = column;
            }
        }

        private RecipeTable mDataTable;
        private HashSet<Cell> mModifiedCells = new HashSet<Cell>();
        private HashSet<int> mCmbxColumns = new HashSet<int>();

        // Cached material image paths
        private Dictionary<string, string> mMaterialImages = new Dictionary<string, string>();

        public RecipesView()
        {
            mDataTable = new RecipeTable(this);

            InitializeComponent();

            // Add column names to the filter combobox
            filterCmbx.Items.Add(kAllCol);
            filterCmbx.Items.Add(RecipeTable.kAlias);
            filterCmbx.Items.Add(RecipeTable.kDisplayName);
            filterCmbx.Items.Add(RecipeTable.kCrafter);
            filterCmbx.Items.Add(RecipeTable.kLvlReq);
            filterCmbx.Items.Add(RecipeTable.kNetWorth);
            filterCmbx.Items.Add(RecipeTable.kEffort);
            filterCmbx.Items.Add(IngredientColumnGroup.kIngr + " " + IngredientColumnGroup.kName);
            filterCmbx.Items.Add(IngredientColumnGroup.kIngr + " " + IngredientColumnGroup.kAmount);
            filterCmbx.SelectedIndex = 0;
        }

        public void SaveModifiedFiles()
        {
            foreach (Cell cell in mModifiedCells)
            {
                ColumnBehavior behavior = mDataTable.GetColumnBehavior(cell.Column);
                behavior.SaveCell(cell.Row, cell.Row[cell.Column]);
            }

            // Save ingredient rows all at once instead of once per cell to avoid unnecessary calculation
            var ingredientRows =
                mModifiedCells
                .Where(e => mDataTable.GetColumnBehavior(e.Column).IsIngredientColumn)
                .Select(e => e.Row)
                .Distinct()
                .ToList();

            foreach (var row in ingredientRows)
            {
                SaveIngredients(row);
            }

            mModifiedCells.Clear();
            unsavedFilesLabel.Visible = false;
        }

        public void Initialize()
        {
            DateTime mStart = DateTime.Now;
            MakeDoubleBuffered();
            LoadMaterialImages();
            LoadColumnsData();

            // Attach event listener after all data has been populated
            mDataTable.ColumnChanged +=
                (sender, e) =>
                {
                    RecipeRow row = (RecipeRow)e.Row;
                    mModifiedCells.Add(new Cell(row, e.Column));
                    unsavedFilesLabel.Visible = true;

                    mDataTable.GetColumnBehavior(e.Column).OnCellChanged(e);
                };
        }

        public void Reload()
        {
            // Disable render while adding rows
            recipesGridView.DataSource = null;
            mDataTable.Reload();
            mModifiedCells.Clear();
            mCmbxColumns.Clear();
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
            LoadAllRecipes();

            recipesGridView.DataSource = mDataTable;

            ConfigureColumns();
        }

        private DataColumn AddDataColumn(string name, Type valueType)
        {
            DataColumn dataColumn = new DataColumn(name, valueType);
            mDataTable.Columns.Add(dataColumn);
            return dataColumn;
        }

        private void ConfigureColumns()
        {
            for (int i = 0; i < recipesGridView.Columns.Count; i++)
            {
                var column = recipesGridView.Columns[i];

                // Add combo boxes for columns that have a predetermined set of valid values
                if (column.Name.StartsWith(IngredientColumnGroup.kIngr) && column.Name.EndsWith(IngredientColumnGroup.kName))
                {
                    DataGridViewComboBoxColumn newColumn = new DataGridViewComboBoxColumn();
                    newColumn.Name = column.Name;
                    newColumn.DataPropertyName = column.DataPropertyName;
                    newColumn.AutoComplete = false;
                    newColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                    newColumn.DataSource = GetAutoCompleteStrings(newColumn.Name);
                    newColumn.SortMode = DataGridViewColumnSortMode.Automatic;

                    recipesGridView.Columns.RemoveAt(i);
                    recipesGridView.Columns.Insert(i, newColumn);
                    mCmbxColumns.Add(i);
                    column = newColumn;
                }

                // Remove [x] broken image icons
                if (column is DataGridViewImageColumn)
                {
                    (column as DataGridViewImageColumn).DefaultCellStyle.NullValue = null;
                }

                // Color ingredient columns
                Regex matchOdd = new Regex(IngredientColumnGroup.kIngr + "[13579]");
                Regex matchEven = new Regex(IngredientColumnGroup.kIngr + "[02468]");
                if (matchOdd.IsMatch(column.Name))
                {
                    column.DefaultCellStyle.BackColor = Color.LemonChiffon;
                }
                else if (matchEven.IsMatch(column.Name))
                {
                    column.DefaultCellStyle.BackColor = Color.LightBlue;
                }
                else if (column.Name.StartsWith("R "))
                {
                    column.DefaultCellStyle.BackColor = Color.Azure;
                }
            }

            recipesGridView.Columns[RecipeTable.kAlias].ReadOnly = true;
            recipesGridView.Columns[RecipeTable.kAlias].Frozen = true;
            recipesGridView.Columns[RecipeTable.kCrafter].ReadOnly = true;
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
                JsonFileData jsonFileData = recipe.Value as JsonFileData;
                JObject recipeJson = jsonFileData.Json;
                JArray ingredientArray = recipeJson["ingredients"] as JArray;
                JArray productArray = recipeJson["produces"] as JArray;

                foreach (JToken product in productArray)
                {
                    RecipeRow row = mDataTable.NewRecipeRow();

                    row.RecipeList = recipesIndex;
                    row.Recipe = jsonFileData;

                    JToken lvlReq = recipeJson["level_requirement"];
                    row.SetLevelRequired(lvlReq == null ? 0 : lvlReq.ToObject<int>());
                    row.SetEffort(recipeJson["work_units"].ToObject<int>());
                    row.SetCrafter(jobAlias);

                    // TODO: make a row for each fine item?
                    JToken item = product["item"];
                    if (item != null)
                    {
                        string alias = item.ToString();
                        row.SetAlias(alias);

                        // Check aliases linked by recipe file
                        JsonFileData itemFileData = FindJsonFileMatchingAlias(jsonFileData, alias);
                        row.Item = itemFileData;
                        row.SetNetWorth(itemFileData.NetWorth);
                        row.SetDisplayName(GetTranslatedName(GetDisplayName(itemFileData)));
                        row.SetIcon(GetIcon(itemFileData));
                    }

                    foreach (JToken ingredient in ingredientArray)
                    {
                        JToken uri = ingredient["uri"];
                        JToken material = ingredient["material"];
                        JToken count = ingredient["count"];

                        Ingredient ingredientData = row.AddNewIngredient();

                        string alias = uri != null ? uri.ToString() : material.ToString();

                        ingredientData.Amount = count.ToObject<int>();

                        if (material != null)
                        {
                            ingredientData.Name = alias;
                            ingredientData.Icon = GetIcon(material.ToString());
                        }
                        else if (uri != null)
                        {
                            JsonFileData ingrJsonFileData = FindJsonFileMatchingAlias(jsonFileData, alias);
                            ingredientData.Name = ingrJsonFileData.GetModuleFile().FullAlias;
                            ingredientData.Icon = GetIcon(ingrJsonFileData);
                        }
                    }

                    mDataTable.Rows.Add(row);
                }
            }
        }

        private JsonFileData FindJsonFileMatchingAlias(JsonFileData jsonFileData, String alias)
        {
            foreach (ModuleFile linkedModule in jsonFileData.LinkedAliases)
            {
                if (linkedModule.FullAlias.Equals(alias))
                {
                    return (JsonFileData)linkedModule.FileData;
                }
            }

            return jsonFileData;
        }

        private void SaveIngredients(RecipeRow row)
        {
            JsonFileData jsonFileData = row.Recipe;
            JObject json = jsonFileData.Json;

            JArray newIngrArray = new JArray();
            foreach (Ingredient ingredient in row.Ingredients)
            {
                if (!string.IsNullOrEmpty(ingredient.Name))
                {
                    string ingrKey = ingredient.Name.Contains(':') ? "uri" : "material";
                    JObject newIngr = new JObject(
                        new JProperty(ingrKey, ingredient.Name),
                        new JProperty("count", ingredient.Amount));

                    newIngrArray.Add(newIngr);
                }
            }

            json["ingredients"] = newIngrArray;
            jsonFileData.TrySetFlatFileData(json.ToString());
            jsonFileData.TrySaveFile();
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
                        mMaterialImages.Add(name, path);
                    }
                }
            }
        }

        private void SetGridValue(int colIndex, int rowIndex, string value)
        {
            try
            {
                if (recipesGridView.Columns[colIndex].ValueType == typeof(int))
                {
                    recipesGridView[colIndex, rowIndex].Value = int.Parse(value);
                }
                else
                {
                    bool valid = true;
                    if (recipesGridView.Columns[colIndex] is DataGridViewComboBoxColumn)
                    {
                        List<string> autoCompleteStrings = (recipesGridView.Columns[colIndex] as DataGridViewComboBoxColumn).DataSource as List<string>;
                        if (!autoCompleteStrings.Contains(value))
                            valid = false;
                    }

                    if (valid)
                        recipesGridView[colIndex, rowIndex].Value = value;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("Unable to set grid value for (%d, %d). Error: %s", colIndex, rowIndex, exception.Message));
                return;
            }
        }

        private void DeleteCurrentCell()
        {
            DataGridViewCell cell = recipesGridView.CurrentCell;
            DataColumn dataColumn = mDataTable.Columns[cell.ColumnIndex];
            RecipeRow recipeRow = (RecipeRow)mDataTable.Rows[cell.RowIndex];
            mDataTable.GetColumnBehavior(dataColumn).TryDeleteCell(recipeRow);
        }

        private string GetTranslatedName(string locKey)
        {
            if (locKey.Contains(':'))
            {
                return ModuleDataManager.GetInstance().LocalizeString(locKey, true);
            }
            else
            {
                return locKey;
            }
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

        private List<string> GetAutoCompleteStrings(string colName) // TODO: put in behavior
        {
            List<string> strings = new List<string>();
            if (colName.StartsWith(IngredientColumnGroup.kIngr) && colName.EndsWith(IngredientColumnGroup.kName))
            {
                strings.Add("");

                // Add materials
                strings.AddRange(mMaterialImages.Keys);

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

            return strings;
        }

        private string GetDisplayName(JsonFileData jsonFileData)
        {
            JsonFileData mixinsFile = jsonFileData.CreateFileWithMixinsApplied();
            JToken displayName = mixinsFile.Json.SelectToken("entity_data.stonehearth:catalog.display_name");
            return displayName.ToString();
        }

        public Image GetIcon(JsonFileData jsonFileData)
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

        public Image GetIcon(string material)
        {
            string path;
            mMaterialImages.TryGetValue(material, out path);
            if (path != null)
            {
                return ThumbnailCache.GetThumbnail(path);
            }

            return null;
        }

        private void searchBox_Filter(object sender, EventArgs e)
        {
            string searchTerm = searchBox.Text.ToString();
            string colFilter = filterCmbx.Text;
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
            if (colFilter == kAllCol || colFilter.Contains(IngredientColumnGroup.kIngr))
            {
                Regex matchIng = new Regex(IngredientColumnGroup.kIngr + "\\d");
                for (int i = 0; i < recipesGridView.Columns.Count; i++)
                {
                    DataGridViewColumn column = recipesGridView.Columns[i];
                    bool isText = column.ValueType == typeof(string) || column.ValueType == typeof(int);
                    bool match = colFilter.Contains(IngredientColumnGroup.kIngr) ? matchIng.IsMatch(column.Name) : isText;
                    if (match)
                    {
                        sb.Append(GetColFilterString(column.Name, searchTerm));
                        if (i < recipesGridView.Columns.Count - 1)
                        {
                            sb.Append(" OR "); // to aggregate multiple columns into the search filter query
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

        private void recipesGridView_KeyDown(object sender, KeyEventArgs e)
        {
            // Delete cell on del
            if (e.KeyCode == Keys.Delete)
            {
                DeleteCurrentCell();
            }

            // Save on ctrl+s
            else if (e.Control && e.KeyCode == Keys.S)
            {
                SaveModifiedFiles();
            }

            // Paste multiple cells on ctrl+v
            else if (e.Control && e.KeyCode == Keys.V)
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
            if (mCmbxColumns.Contains(recipesGridView.CurrentCell.ColumnIndex))
            {
                ComboBox cbx = (ComboBox)e.Control;
                cbx.DropDownStyle = ComboBoxStyle.DropDown;
                cbx.AutoCompleteSource = AutoCompleteSource.ListItems;
                cbx.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                // Otherwise two dropdowns appear. Known bug where the KeyDown event of a combobox is raised twice
                cbx.KeyDown -= ComboBoxKeyDown;
                cbx.KeyDown += ComboBoxKeyDown;
            }
        }

        private void ComboBoxKeyDown(object sender, EventArgs args)
        {
            ComboBox cbx = (ComboBox)sender;
            if (cbx.DroppedDown)
            {
                cbx.DroppedDown = false;
            }
        }

        private void recipesGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = true;
        }

        private void recipesGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewCell cell = recipesGridView[e.ColumnIndex, e.RowIndex];
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.BackColor = Color.Gold;
            cell.Style = style;
        }

        private void recipesGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = recipesGridView[e.ColumnIndex, e.RowIndex];
            cell.Style = null;
        }

        private void recipesGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                recipesCellContextMenu.Show(recipesGridView, new Point(e.X, e.Y));
            }
        }

        private void recipesGridView_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.ColumnIndex > 0 && e.RowIndex > 0)
                {
                    recipesGridView.CurrentCell = this.recipesGridView[e.ColumnIndex, e.RowIndex];
                }
            }
        }

        private void removeIngredientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteCurrentCell();
        }

        private void addNewIngredientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewCell cell = recipesGridView.CurrentCell;
            RecipeRow recipeRow = (RecipeRow)mDataTable.Rows[cell.RowIndex];
            recipeRow.AddNewIngredient();
            ConfigureColumns();
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("From this view, you can: \n" + 
                            "* Change the Display Name and Net Worth of an item \n" +
                            "* Change the Effort and Lvl Req of a recipe \n" + 
                            "* Add and remove ingredients from a recipe \n" + 
                            "* Change ingredient amounts and ingredients in a recipe \n" + 
                            "Hotkeys: \n" + 
                            "* Press `delete` to delete an ingredient \n" +
                            "* Press ctrl+s to save the modified files \n" + 
                            "* Press ctrl+c ctrl+v and select a single or multiple cells to paste a value \n" +
                            "* Right click and press add ingredient to add ingredient columns to a recipe that does not have enough columns to fit a new ingredient \n" + 
                            "\n");
        }
    }
}
