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
    public partial class RecipesView : UserControl, IReloadable
    {
        public const string kAllCol = "All Columns";

        private bool mBaseModsOnly = true;
        private bool mIsLoading = false;
        private bool mIsApplyingChanges = false;

        private RecipeTable mDataTable;
        private HashSet<DataCell> mModifiedCells = new HashSet<DataCell>();
        private HashSet<JsonFileData> mModifiedFiles = new HashSet<JsonFileData>();
        private HashSet<int> mComboBoxColumns = new HashSet<int>();
        private HashSet<ComboBox> mComboBoxes = new HashSet<ComboBox>();

        private Stack<MultipleCellChange> mUndoStack = new Stack<MultipleCellChange>();
        private Stack<MultipleCellChange> mRedoStack = new Stack<MultipleCellChange>();
        private int currentStateUndoIndex = 0;

        // Cached material image paths
        private Dictionary<string, string> mMaterialImages = new Dictionary<string, string>();

        public RecipesView()
        {
            mDataTable = new RecipeTable(this);
            AddDataTableEventHandlers(mDataTable);

            InitializeComponent();

            // Add column names to the filter combobox
            filterCbx.Items.Add(kAllCol);
            filterCbx.Items.Add(RecipeTable.kAlias);
            filterCbx.Items.Add(RecipeTable.kDisplayName);
            filterCbx.Items.Add(RecipeTable.kCategory);
            filterCbx.Items.Add(RecipeTable.kMaterialTags);
            filterCbx.Items.Add(RecipeTable.kCrafter);
            filterCbx.Items.Add(RecipeTable.kLvlReq);
            filterCbx.Items.Add(RecipeTable.kNetWorth);
            filterCbx.Items.Add(RecipeTable.kAppeal);
            filterCbx.Items.Add(RecipeTable.kEffort);
            filterCbx.Items.Add(IngredientColumnGroup.kIngr + " " + IngredientColumnGroup.kName);
            filterCbx.Items.Add(IngredientColumnGroup.kIngr + " " + IngredientColumnGroup.kAmount);

            // Paint deprecated rows in gray.
            recipesGridView.RowPrePaint += (object sender, DataGridViewRowPrePaintEventArgs e) =>
            {
                if (((recipesGridView.Rows[e.RowIndex].DataBoundItem as DataRowView).Row as RecipeRow).IsDeprecated)
                {
                    recipesGridView.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Gray;
                }
            };
        }

        public void SaveModifiedFiles()
        {
            foreach (DataCell cell in mModifiedCells)
            {
                ColumnBehavior behavior = mDataTable.GetColumnBehavior(cell.Column);
                behavior.SaveCell(mModifiedFiles, cell.Row, cell.Row[cell.Column]);
            }

            // Save ingredient rows all at once because unlike other cells, an ingredient
            // does not map to a unique key in json. We rewrite the entire json array all at once
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
                modified.TrySetFlatFileData(modified.Json.ToString());
                modified.TrySaveFile();
            }

            mModifiedFiles.Clear();
            mModifiedCells.Clear();
            unsavedFilesLabel.Visible = false;
            currentStateUndoIndex = mUndoStack.Count();
        }

        public void Initialize()
        {
            mIsLoading = true;
            SelectContextMenuItems();
            filterCbx.SelectedIndex = 0;
            itemsTypeComboBox.SelectedIndex = int.Parse(Properties.Settings.Default.LastSelectedItemsTypeIndex);
            MakeDoubleBuffered();
            LoadMaterialImages();
            LoadColumnsData();
            mIsLoading = false;
        }

        public void Reload()
        {
            // Disable render while adding rows
            mIsLoading = true;
            SelectContextMenuItems();
            recipesGridView.DataSource = null;
            mDataTable.Dispose();
            mDataTable = new RecipeTable(this);
            AddDataTableEventHandlers(mDataTable);
            mModifiedCells.Clear();
            mComboBoxColumns.Clear();
            mUndoStack.Clear();
            mRedoStack.Clear();
            LoadColumnsData();
            FilterRows();
            mIsLoading = false;
        }

        private void SelectContextMenuItems()
        {
            var showRecipeContextMenuButtons = itemsTypeComboBox.Text == "Recipes";
            removeIngredientToolStripMenuItem.Visible = showRecipeContextMenuButtons;
            addNewIngredientToolStripMenuItem.Visible = showRecipeContextMenuButtons;
            openRecipeJSONToolStripMenuItem.Visible = showRecipeContextMenuButtons;
        }

        // Helps address DataGridView's slow repaint time. See https://www.codeproject.com/Tips/654101/Double-Buffering-a-DataGridview
        private void MakeDoubleBuffered()
        {
            BindingFlags bindings = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty;
            typeof(DataGridView).InvokeMember("DoubleBuffered", bindings, null, recipesGridView, new object[] { true });
        }

        private void AddDataTableEventHandlers(DataTable dataTable)
        {
            dataTable.ColumnChanging +=
                (object sender, DataColumnChangeEventArgs e) =>
                {
                    if (!mIsLoading && !mIsApplyingChanges)
                    {
                        DataCell cell = new DataCell(e.Column, (RecipeRow)e.Row);
                        MultipleCellChange change = new MultipleCellChange(cell, e.Row[e.Column], e.ProposedValue);
                        mUndoStack.Push(change);
                    }
                };

            dataTable.ColumnChanged +=
                (object sender, DataColumnChangeEventArgs e) =>
                {
                    RecipeRow row = (RecipeRow)e.Row;
                    DataColumn column = e.Column;
                    mDataTable.GetColumnBehavior(column).OnDataCellChanged(e);

                    if (!mIsLoading)
                    {
                        unsavedFilesLabel.Visible = true;
                        mModifiedCells.Add(new DataCell(column, row));
                    }
                };
        }

        private void LoadColumnsData()
        {
            string itemsType = itemsTypeComboBox.Text;
            switch(itemsType)
            {
                case "Recipes":
                    LoadAllRecipes();
                    break;
                case "Entities":
                    LoadEntities();
                    break;
                case "Iconics":
                    LoadIconics();
                    break;
                default:
                    throw new Exception("no editable option available for " + itemsType);
            }

            recipesGridView.DataSource = mDataTable;

            ConfigureColumns();
        }

        private void ConfigureColumns()
        {
            for (int i = 0; i < recipesGridView.Columns.Count; i++)
            {
                DataGridViewColumn column = recipesGridView.Columns[i];
                DataColumn dataColumn = mDataTable.Columns[i];
                ColumnBehavior colBehavior = mDataTable.GetColumnBehavior(dataColumn);

                // Add combo boxes for columns that have a predetermined set of valid values
                DataGridViewColumn replacementColumn = null;
                if (colBehavior is IngrNameColumnBehavior)
                {
                    DataGridViewComboBoxColumn newColumn = new DataGridViewComboBoxColumn();
                    newColumn.Name = column.Name;
                    newColumn.DataPropertyName = column.DataPropertyName;
                    newColumn.AutoComplete = false;
                    newColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                    newColumn.DataSource = GetAutoCompleteStrings(newColumn.Name);
                    newColumn.SortMode = DataGridViewColumnSortMode.Automatic;
                    replacementColumn = newColumn;
                } else if (colBehavior is BooleanColumnBehavior) {
                    var newColumn = new DataGridViewCheckBoxColumn();
                    newColumn.Name = column.Name;
                    newColumn.DataPropertyName = column.DataPropertyName;
                    newColumn.SortMode = DataGridViewColumnSortMode.Automatic;
                    replacementColumn = newColumn;
                }
                if (replacementColumn != null) {
                    recipesGridView.Columns.RemoveAt(i);
                    recipesGridView.Columns.Insert(i, replacementColumn);
                    mComboBoxColumns.Add(i);
                    column = replacementColumn;
                }

                // Configure columns based on column behavior
                colBehavior.ConfigureColumn(column);
            }
        }

        private void ConfigureColumnsAfterRender()
        {
            for (int i = 0; i < recipesGridView.Columns.Count; i++)
            {
                DataGridViewColumn column = recipesGridView.Columns[i];
                DataColumn dataColumn = mDataTable.Columns[i];
                ColumnBehavior colBehavior = mDataTable.GetColumnBehavior(dataColumn);
                colBehavior.ConfigureColumnAfterRender(column, recipesGridView);
            }
        }

        private void LoadEntities()
        {
            Dictionary<string, JsonFileData> entityJsonFiles = ModuleDataManager.GetInstance().GetJsonsOfType(JSONTYPE.ENTITY, mBaseModsOnly)
                .Where(kvp => kvp.Value.Json.SelectToken("components.stonehearth:ai") == null)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            foreach (KeyValuePair<string, JsonFileData> entry in entityJsonFiles)
            {
                RecipeRow row = mDataTable.NewRecipeRow();
                SetRowDataForItem(row, entry.Key, entry.Value);
                mDataTable.Rows.Add(row);
            }
        }

        private void LoadIconics()
        {
            Dictionary<string, JsonFileData> iconicJsonFiles = ModuleDataManager.GetInstance().GetIconicJsons(mBaseModsOnly);

            foreach (KeyValuePair<string, JsonFileData> entry in iconicJsonFiles)
            {
                RecipeRow row = mDataTable.NewRecipeRow();
                SetRowDataForItem(row, entry.Key, entry.Value);
                mDataTable.Rows.Add(row);
            }
        }

        private void LoadAllRecipes()
        {
            foreach (Module module in ModuleDataManager.GetInstance().GetAllModules())
            {
                bool shouldIncludeMod = mBaseModsOnly ? ModuleDataManager.IsBaseMod(module.Name) : true;
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
                    JToken effort = recipeJson["effort"];
                    JToken workUnits = recipeJson["work_units"];
                    JToken appeal = recipeJson.SelectToken("entity_data.stonehearth:appeal.appeal");

                    row.SetLevelRequired(lvlReq == null ? null : lvlReq.ToObject<int?>());
                    row.SetEffort(effort == null ? null : effort.ToObject<int?>());
                    row.SetWorkUnits(workUnits == null ? null : workUnits.ToObject<int?>());
                    row.SetCrafter(jobAlias);

                    JToken item = product["item"];
                    if (item != null)
                    {
                        string alias = item.ToString();
                        // Check aliases linked by recipe file
                        JsonFileData itemFileData = FindLinkedJsonMatchingAlias(jsonFileData, alias) ?? jsonFileData;
                        SetRowDataForItem(row, alias, itemFileData);
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
                            if (ingrJsonFileData == null)
                            {
                                MessageBox.Show("Could not find ingredient \"" + uri + "\" in the manifest for recipe \"" + jsonFileData.FileName + "\"");
                                continue;
                            }

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

        private JsonFileData FindLinkedJsonMatchingAlias(JsonFileData jsonFileData, string alias)
        {
            foreach (ModuleFile linkedModule in jsonFileData.LinkedAliases)
            {
                if (linkedModule.FullAlias.Equals(alias))
                {
                    return (JsonFileData)linkedModule.FileData;
                }
            }

            return null;
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

                object oldValue = mDataTable.GetRow(rowIndex)[colIndex];
                if (string.IsNullOrEmpty(value))
                {
                    mDataTable.GetRow(rowIndex)[colIndex] = DBNull.Value;
                }
                else
                {
                    mDataTable.GetRow(rowIndex)[colIndex] = value;
                }
                DataCell cell = new DataCell(mDataTable.Columns[colIndex], mDataTable.GetRow(rowIndex));
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
            DataColumn dataColumn = mDataTable.GetColumn(cell.ColumnIndex);
            RecipeRow recipeRow = mDataTable.GetRow(cell.RowIndex);
            mDataTable.GetColumnBehavior(dataColumn).TryDeleteCell(recipeRow);
        }

        private void ApplyCellChanges(Stack<MultipleCellChange> fromStack, Stack<MultipleCellChange> toStack, Action<MultipleCellChange> doChange)
        {
            if (fromStack.Any())
            {
                MultipleCellChange changes = fromStack.Pop();
                mIsApplyingChanges = true;
                doChange(changes);
                mIsApplyingChanges = false;
                toStack.Push(changes);

                // Hide unsaved indicator if we have undone/redone to the current state of the files
                if (currentStateUndoIndex == mUndoStack.Count())
                {
                    unsavedFilesLabel.Visible = false;
                    mModifiedCells.Clear();
                }
            }
        }

        public void ApplyStyle(DataGridViewCell cell, DataGridViewCellStyle style)
        {
            if (cell.Style != null)
            {
                cell.Style.ApplyStyle(style);
            }
            else
            {
                cell.Style = style;
            }
        }

        private void SetRowDataForItem(RecipeRow row, string alias, JsonFileData jsonFileData)
        {
            JObject json = jsonFileData.Json;

            JToken appeal = json.SelectToken("entity_data.stonehearth:appeal.appeal");
            row.SetAppeal(appeal == null ? null : appeal.ToObject<int?>());

            JToken hasVariableItemQuality = json.SelectToken("entity_data.stonehearth:item_quality.variable_quality");
            row.SetIsVariableQuality(hasVariableItemQuality == null ? null : hasVariableItemQuality.ToObject<bool?>());

            JToken shopLevel = json.SelectToken("entity_data.stonehearth:net_worth.shop_info.shopkeeper_level");
            row.SetShopLvl(shopLevel == null ? null : shopLevel.ToObject<int?>());
            JToken isBuyable = json.SelectToken("entity_data.stonehearth:net_worth.shop_info.buyable");
            row.SetIsBuyable(isBuyable == null ? null : isBuyable.ToObject<bool?>());
            JToken isSellable = json.SelectToken("entity_data.stonehearth:net_worth.shop_info.sellable");
            row.SetIsSellable(isSellable == null ? null : isSellable.ToObject<bool?>());

            row.SetAlias(alias);

            row.Item = jsonFileData;
            row.SetNetWorth(jsonFileData.NetWorth == -1 ? (int?)null : (int?)jsonFileData.NetWorth);
            row.SetDisplayName(GetTranslatedName(GetDisplayName(jsonFileData)));
            row.SetCategory(GetCategoryName(jsonFileData));
            row.SetMaterialTags(GetMaterialTags(jsonFileData));
            row.SetIcon(GetIcon(jsonFileData));

            if (jsonFileData.GetModuleFile() != null && jsonFileData.GetModuleFile().IsDeprecated)
            {
                row.IsDeprecated = true;
            }
        }

        private void FilterRows()
        {
            string searchTerm = searchBox.Text.ToString();

            if (searchTerm == "")
            {
                searchBox.BackColor = Color.White;
                mDataTable.DefaultView.RowFilter = "";
                return;
            }

            searchBox.BackColor = Color.Gold;

            string colFilter = filterCbx.Text;
            StringBuilder sb = new StringBuilder();
            // Filter by all columns or all ingredient columns
            if (colFilter == kAllCol || colFilter.Contains(IngredientColumnGroup.kIngr))
            {
                Regex matchIng = new Regex(IngredientColumnGroup.kIngr + "\\d");
                var isFirst = true;
                for (int i = 0; i < recipesGridView.Columns.Count; i++)
                {
                    DataGridViewColumn column = recipesGridView.Columns[i];
                    bool isText = column.ValueType == typeof(string) || column.ValueType == typeof(int);
                    bool match = colFilter.Contains(IngredientColumnGroup.kIngr) ? matchIng.IsMatch(column.Name) : isText;
                    if (match) {
                        if (!isFirst) {
                            sb.Append(" OR ");
                        }
                        sb.Append(GetColFilterString(column.Name, searchTerm));
                        isFirst = false;
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

        private string GetTranslatedName(string locKey)
        {
            if (locKey.Contains("i18n"))
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
                    bool shouldIncludeMod = mBaseModsOnly ? (modName == "stonehearth" || modName == "rayyas_children" || modName == "northern_alliance") : true;
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
            return GetCatalogField(jsonFileData, "display_name");
        }

        private string GetCategoryName(JsonFileData jsonFileData)
        {
            return GetCatalogField(jsonFileData, "category");
        }

        private string GetMaterialTags(JsonFileData jsonFileData)
        {
            return GetCatalogField(jsonFileData, "material_tags");
        }

        private string GetCatalogField(JsonFileData jsonFileData, String fieldName)
        {
            JObject entityJson = jsonFileData.Json;
            JToken result = entityJson.SelectToken("entity_data.stonehearth:catalog." + fieldName);
            try {
                JsonFileData mixinsFile = jsonFileData.CreateFileWithMixinsApplied();
                result = mixinsFile.Json.SelectToken("entity_data.stonehearth:catalog." + fieldName);
            } catch (Exception e) {
                // If could not apply mixin (e.g. if it is a mod json that uses mixintypes),
                // manually check the ghost for the display name.
                if (result == null) {
                    JToken entityFormsComponent = entityJson.SelectToken("components.stonehearth:entity_forms");
                    if (entityFormsComponent != null) {
                        JToken ghostForm = entityFormsComponent["ghost_form"];
                        if (ghostForm != null) {
                            string ghostFilePath = JsonHelper.GetFileFromFileJson(ghostForm.ToString(), jsonFileData.Directory);
                            ghostFilePath = JsonHelper.NormalizeSystemPath(ghostFilePath);

                            foreach (JsonFileData file in jsonFileData.OpenedFiles) {
                                JObject json = file.Json;
                                if (file.Path == ghostFilePath) {
                                    result = json.SelectToken("entity_data.stonehearth:catalog." + fieldName);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return result == null ? "" : result.ToString();
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
            FilterRows();
        }

        private void recipesGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!recipesGridView.IsCurrentCellInEditMode)
                {
                    recipesGridView.BeginEdit(true);
                    e.Handled = true;
                }
            }
            else if (e.Control && e.KeyCode == Keys.S)
            {
                // Save on ctrl+s
                SaveModifiedFiles();
            }
            else if ((e.Control && e.Shift && e.KeyCode == Keys.Z) || (e.Control && e.KeyCode == Keys.Y))
            {
                // Redo on ctrl+shift+z or ctrl+y
                ApplyCellChanges(mRedoStack, mUndoStack, changes => changes.Redo());
            }
            else if (e.Control && e.KeyCode == Keys.Z)
            {
                // Undo on ctrl+z
                ApplyCellChanges(mUndoStack, mRedoStack, changes => changes.Undo());
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if (recipesGridView.SelectedCells.Count == 0)
                {
                    return;
                }
                else if (recipesGridView.SelectedCells.Count == 1)
                {
                    // Delete current cell on del if only one selected
                    DeleteCurrentCell();
                }
                else
                {
                    // Delete multiple cells
                    // Add cell changes to one transaction so we can roll back if needed
                    mIsApplyingChanges = true;
                    MultipleCellChange changes = new MultipleCellChange();

                    foreach (DataGridViewCell cell in recipesGridView.SelectedCells)
                    {
                        DataColumn dataColumn = mDataTable.GetColumn(cell.ColumnIndex);
                        RecipeRow recipeRow = mDataTable.GetRow(cell.RowIndex);

                        object oldValue = recipeRow[dataColumn];
                        object newValue = DBNull.Value;

                        bool success = mDataTable.GetColumnBehavior(dataColumn).TryDeleteCell(recipeRow);

                        if (!success)
                        {
                            changes.Undo();
                            return;
                        }

                        DataCell dataCell = new DataCell(dataColumn, recipeRow);
                        changes.Add(new CellChange(dataCell, oldValue, newValue));
                    }

                    // Save cell changes made in this delete operation
                    mUndoStack.Push(changes);
                    mIsApplyingChanges = false;
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
                mIsApplyingChanges = true;
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
                }

                // Save cell changes made in this paste operation
                mUndoStack.Push(changes);
                mIsApplyingChanges = false;
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
                    // Set the cell value manually from suggestion list on click. Need to do this
                    // due to timing. We can't suppress mouse clicks from the companion, so by the time 
                    // the click goes through, the combobox/listbox will be hidden so setting the combobox
                    // at that time will not resulting in the grid view accepting the combobox value.
                    companion.OnClick =
                        (object s, EventArgs ev, ListBox suggList) =>
                        {
                            DataGridViewCell cell = recipesGridView.CurrentCell;
                            mDataTable.GetRow(cell.RowIndex)[cell.ColumnIndex] = suggList.Text;
                            return null;
                        };
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
            ApplyStyle(cell, style);
        }

        private void recipesGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = recipesGridView[e.ColumnIndex, e.RowIndex];
            if (cell.Style != null)
            {
                DataGridViewColumn column = recipesGridView.Columns[e.ColumnIndex];
                cell.Style.BackColor = column.DefaultCellStyle.BackColor;
            }
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
            RecipeRow recipeRow = mDataTable.GetRow(cell.RowIndex);
            recipeRow.AddNewIngredient();
            ConfigureColumns();
        }

        private void openJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewCell cell = recipesGridView.CurrentCell;
            RecipeRow recipeRow = mDataTable.GetRow(cell.RowIndex);
            System.Diagnostics.Process.Start(recipeRow.Item.Path);
        }

        private void openRecipeJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewCell cell = recipesGridView.CurrentCell;
            RecipeRow recipeRow = mDataTable.GetRow(cell.RowIndex);
            if (recipeRow.Recipe != null)
            {
                System.Diagnostics.Process.Start(recipeRow.Recipe.Path);
            }
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
                            "* Press `ctrl+s` to save the modified files \n" + 
                            "* Press `ctrl+c, ctrl+v` and select a single or multiple cells to paste a value \n" +
                            "* Press `ctrl+z` to undo \n" +
                            "* Press `ctrl+shift+z` or `ctrl+y` to redo \n" + 
                            "* Right click and press add ingredient to add ingredient columns to a recipe that does not have enough columns to fit a new ingredient \n" + 
                            "\n");
        }

        private void recipesGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataColumn column = mDataTable.Columns[e.ColumnIndex];
            mDataTable.GetColumnBehavior(column).OnGridCellChanged(recipesGridView, e);
        }

        // Some configure (like cell style updates) can only be applied after the grid view is done rendering
        private void recipesGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ConfigureColumnsAfterRender();
        }

        private void itemsTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mIsLoading)
            {
                return;
            }

            Reload();
            Properties.Settings.Default.LastSelectedItemsTypeIndex = itemsTypeComboBox.SelectedIndex.ToString();
            Properties.Settings.Default.Save();
        }

        private void baseModsButton_CheckedChanged(object sender, EventArgs e)
        {
            bool prev = mBaseModsOnly;
            if (prev != baseModsButton.Checked)
            {
                mBaseModsOnly = baseModsButton.Checked;
                Reload();
            }
        }
    }
}
