﻿using System;
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
        private bool mListenOnCellChange = false;

        private RecipeTable mDataTable;
        private HashSet<DataCell> mModifiedCells = new HashSet<DataCell>();
        private HashSet<JsonFileData> mModifiedFiles = new HashSet<JsonFileData>();
        private HashSet<int> mComboBoxColumns = new HashSet<int>();
        private HashSet<ComboBox> mComboBoxes = new HashSet<ComboBox>();

        private Stack<MultipleCellChange> mUndoStack = new Stack<MultipleCellChange>();
        private Stack<MultipleCellChange> mRedoStack = new Stack<MultipleCellChange>();

        // Cached material image paths
        private Dictionary<string, string> mMaterialImages = new Dictionary<string, string>();

        public RecipesView()
        {
            mDataTable = new RecipeTable(this);
            mDataTable.ColumnChanging +=
                (object sender, DataColumnChangeEventArgs e) =>
                {
                    if (mListenOnCellChange)
                    {
                        DataCell cell = new DataCell((RecipeRow)e.Row, e.Column);
                        MultipleCellChange change = new MultipleCellChange(cell, e.Row[e.Column], e.ProposedValue);
                        mUndoStack.Push(change);
                    }
                };

            mDataTable.ColumnChanged +=
                (object sender, DataColumnChangeEventArgs e) =>
                {
                    mDataTable.GetColumnBehavior(e.Column).OnCellChanged(e);

                    if (mListenOnCellChange)
                    {
                        mModifiedCells.Add(new DataCell((RecipeRow)e.Row, e.Column));
                        unsavedFilesLabel.Visible = true;
                    }
                };

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
            foreach (DataCell cell in mModifiedCells)
            {
                ColumnBehavior behavior = mDataTable.GetColumnBehavior(cell.Column);
                behavior.SaveCell(mModifiedFiles, cell.Row, cell.Row[cell.Column]);
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
                SaveIngredients(mModifiedFiles, row);
            }

            foreach (JsonFileData modified in mModifiedFiles)
            {
                modified.TrySaveFile();
            }

            mModifiedFiles.Clear();
            mModifiedCells.Clear();
            unsavedFilesLabel.Visible = false;
        }

        public void Initialize()
        {
            DateTime mStart = DateTime.Now;
            MakeDoubleBuffered();
            LoadMaterialImages();
            LoadColumnsData();
            mListenOnCellChange = true;
        }

        public void Reload()
        {
            mListenOnCellChange = false;
            // Disable render while adding rows
            recipesGridView.DataSource = null;
            mDataTable.Reload();
            mModifiedCells.Clear();
            mComboBoxColumns.Clear();
            mUndoStack.Clear();
            mRedoStack.Clear();
            LoadColumnsData();
            mListenOnCellChange = false;
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
                    mComboBoxColumns.Add(i);
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
                else
                {
                    DataColumn dataColumn = mDataTable.Columns[i];
                    if (mDataTable.GetColumnBehavior(dataColumn).IsRecipeColumn)
                    {
                        column.DefaultCellStyle.BackColor = Color.Azure;
                    }
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
                        JsonFileData itemFileData = FindLinkedJsonMatchingAlias(jsonFileData, alias);
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
                        ingredientData.Amount = count.ToObject<int>();

                        if (material != null)
                        {
                            ingredientData.Name = material.ToString();
                        }
                        else if (uri != null)
                        {
                            JsonFileData ingrJsonFileData = FindLinkedJsonMatchingAlias(jsonFileData, uri.ToString());
                            ingredientData.Name = ingrJsonFileData.GetModuleFile().FullAlias;
                        }
                        else
                        {
                            throw new Exception("Recipe " + jsonFileData.GetModuleFile().FullAlias + " has invalid ingredient with no uri/material field");
                        }
                    }

                    mDataTable.Rows.Add(row);
                }
            }
        }

        private JsonFileData FindLinkedJsonMatchingAlias(JsonFileData jsonFileData, String alias)
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

        private void SaveIngredients(HashSet<JsonFileData> modifiedFiles, RecipeRow row)
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
            modifiedFiles.Add(jsonFileData);
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

        private bool SetGridValue(int colIndex, int rowIndex, string value, MultipleCellChange changes)
        {
            try
            {
                object oldValue = recipesGridView[colIndex, rowIndex].Value;

                DataGridViewColumn column = recipesGridView.Columns[colIndex];
                if (column is DataGridViewComboBoxColumn)
                {
                    List<string> autoCompleteStrings = (column as DataGridViewComboBoxColumn).DataSource as List<string>;
                    if (!autoCompleteStrings.Contains(value))
                    {
                        MessageBox.Show(string.Format("Value \"{0}\" not found in combobox for column \"{1}\"", value, column.Name));
                        return false;
                    }
                }
                else if (column.ReadOnly)
                {
                    MessageBox.Show(string.Format("Trying to write to read-only column {0}", column.Name));
                    return false;
                }

                recipesGridView[colIndex, rowIndex].Value = value;
                DataCell cell = new DataCell((RecipeRow)mDataTable.Rows[rowIndex], mDataTable.Columns[colIndex]);
                changes.Add(new CellChange(cell, oldValue, value));
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("Unable to set grid value for ({0}, {1}). Error: {2}", colIndex, rowIndex, exception.Message));
                return false;
            }

            return true;
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
                return ModuleDataManager.GetInstance().LocalizeString(locKey);
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

        private List<string> GetAutoCompleteStrings(string colName)
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
            if (e.KeyCode == Keys.Delete)
            {
                // Delete cell on del
                DeleteCurrentCell();
            }
            else if (e.Control && e.KeyCode == Keys.S)
            {
                // Save on ctrl+s
                SaveModifiedFiles();
            }
            else if (e.Control && e.KeyCode == Keys.Z)
            {
                if (mUndoStack.Any())
                {
                    MultipleCellChange changes = mUndoStack.Pop();
                    mListenOnCellChange = false;
                    changes.Undo();
                    mListenOnCellChange = true;
                    mRedoStack.Push(changes);
                }
            }
            else if (e.Control && e.KeyCode == Keys.Y)
            {
                if (mRedoStack.Any())
                {
                    MultipleCellChange changes = mRedoStack.Pop();
                    mListenOnCellChange = false;
                    changes.Redo();
                    mListenOnCellChange = true;
                    mUndoStack.Push(changes);
                }
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                // Paste multiple cells on ctrl+v
                if (recipesGridView.SelectedCells.Count == 0)
                {
                    return;
                }

                // Add cell changes to one transaction so we can roll back if needed
                mListenOnCellChange = false;
                MultipleCellChange changes = new MultipleCellChange();

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
                            bool success = SetGridValue(colIndex, rowIndex, cells[i], changes);
                            if (!success)
                            {
                                changes.Undo();
                                return;
                            }

                            colIndex++;
                        }

                        rowIndex++;
                    }

                    lineIndex++;

                    // Repeat last line in clipboard if we've copied one line and the selected rows exceeds one line
                    if (lineIndex == lines.Count() && lines.Count() == 1)
                    {
                        int selectedRowCount = Math.Abs(startCell.RowIndex - endCell.RowIndex) + 1;
                        int clipboardRowCount = Math.Abs((rowIndex - 1) - startRow) + 1;
                        if (selectedRowCount > clipboardRowCount)
                        {
                            lineIndex--;
                        }
                    }

                    // Save cell changes made in this paste operation
                    mUndoStack.Push(changes);
                    mListenOnCellChange = true;
                }
            }
        }

        private void recipesGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (mComboBoxColumns.Contains(recipesGridView.CurrentCell.ColumnIndex))
            {
                ComboBox cbx = (ComboBox)e.Control;
                cbx.DropDownStyle = ComboBoxStyle.DropDown;

                if (!mComboBoxes.Contains(cbx))
                {
                    mComboBoxes.Add(cbx);
                    var companion = new SuggestComboBoxCompanion(cbx);
                    companion.PropertySelector = collection => collection.Cast<string>();
                }
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