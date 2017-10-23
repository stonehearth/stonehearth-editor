using System.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Drawing;

namespace StonehearthEditor.Recipes
{
    // Defines behavior for each column in a RecipeTable/RecipeView
    internal class ColumnBehavior
    {
        public virtual bool IsIngredientColumn => false;

        public virtual bool IsRecipeColumn => false;

        public virtual void OnDataCellChanged(DataColumnChangeEventArgs e)
        {
        }

        public virtual void OnGridCellChanged(DataGridView recipesGridView, DataGridViewCellEventArgs e)
        {
        }

        public virtual void SaveCell(HashSet<JsonFileData> modifiedFiles, RecipeRow row, object value)
        {
        }

        public virtual void TryDeleteCell(RecipeRow row)
        {
        }

        public virtual void ConfigureColumn(DataGridViewColumn gridCol)
        {
        }

        public virtual void ConfigureColumnAfterRender(DataGridViewColumn gridCol, DataGridView recipesGridView)
        {
        }
    }

    internal class IngrColumnBehavior : ColumnBehavior
    {
        protected IngredientColumnGroup columnGroup = null;

        public override bool IsIngredientColumn => true;

        public IngrColumnBehavior(IngredientColumnGroup columnGroup)
        {
            this.columnGroup = columnGroup;
        }

        public override void ConfigureColumn(DataGridViewColumn gridCol)
        {
            int index = columnGroup.Index;

            // Color ingredient columns
            if (index % 2 == 0)
            {
                gridCol.DefaultCellStyle.BackColor = Color.LemonChiffon;
            }
            else
            {
                gridCol.DefaultCellStyle.BackColor = Color.LightBlue;
            }
        }
    }

    internal class RecipeColumnBehavior : ColumnBehavior
    {
        public override bool IsRecipeColumn => true;

        public override void ConfigureColumn(DataGridViewColumn gridCol)
        {
            gridCol.DefaultCellStyle.BackColor = Color.Azure;
        }
    }

    internal class AliasColumnBehavior : ColumnBehavior
    {
        public override void ConfigureColumn(DataGridViewColumn gridCol)
        {
            gridCol.ReadOnly = true;
            gridCol.Frozen = true;
        }
    }

    internal class DisplayNameColumnBehavior : ColumnBehavior
    {
        public override void SaveCell(HashSet<JsonFileData> modifiedFiles, RecipeRow row, object value)
        {
            JsonFileData jsonFileData = row.Item;
            foreach (JsonFileData file in jsonFileData.OpenedFiles)
            {
                JObject json = file.Json;
                JValue nameToken = json.SelectToken("entity_data.stonehearth:catalog.display_name") as JValue;
                if (nameToken != null)
                {
                    string locKey = nameToken.Value.ToString();

                    if (!locKey.Contains(":"))
                    {
                        nameToken.Value = (string)value;
                        modifiedFiles.Add(jsonFileData);
                    }
                    else
                    {
                        int i18nLength = "i18n(".Length;
                        locKey = locKey.Substring(i18nLength, locKey.Length - i18nLength - 1);
                        ModuleDataManager.GetInstance().ChangeEnglishLocValue(locKey, (string)value);
                    }
                }
            }
        }
    }

    internal class NetWorthColumnBehavior : ColumnBehavior
    {
        public override void SaveCell(HashSet<JsonFileData> modifiedFiles, RecipeRow row, object value)
        {
            JsonFileData jsonFileData = row.Item;
            JObject json = jsonFileData.Json;
            JValue token = json.SelectToken("entity_data.stonehearth:net_worth.value_in_gold") as JValue;
            token.Value = (int)value;
            modifiedFiles.Add(jsonFileData);
        }
    }

    internal class CrafterColumnBehavior : RecipeColumnBehavior
    {
        public override void ConfigureColumn(DataGridViewColumn gridCol)
        {
            base.ConfigureColumn(gridCol);
            gridCol.ReadOnly = true;
        }
    }

    internal class EffortColumnBehavior : RecipeColumnBehavior
    {

        public override void SaveCell(HashSet<JsonFileData> modifiedFiles, RecipeRow row, object value)
        {
            JsonFileData jsonFileData = row.Recipe;
            JObject json = jsonFileData.Json;
            json["effort"] = (int)value;
            modifiedFiles.Add(jsonFileData);
        }
    }

    internal class WorkUnitsColumnBehavior : RecipeColumnBehavior
    {

        public override void SaveCell(HashSet<JsonFileData> modifiedFiles, RecipeRow row, object value)
        {
            JsonFileData jsonFileData = row.Recipe;
            JObject json = jsonFileData.Json;
            if (value == null || value == DBNull.Value)
            {
                if (json["work_units"] != null)
                {
                    json.Remove("work_units");
                }
            }
            else
            {
                json["work_units"] = (int)value;
            }

            modifiedFiles.Add(jsonFileData);
        }

        public override void TryDeleteCell(RecipeRow row)
        {
            row.SetWorkUnits(null);
        }
    }

    internal class LevelRequiredColumnBehavior : RecipeColumnBehavior
    {
        public override void SaveCell(HashSet<JsonFileData> modifiedFiles, RecipeRow row, object value)
        {
            JsonFileData jsonFileData = row.Recipe;
            JObject json = jsonFileData.Json;
            json["level_requirement"] = (int)value;
            modifiedFiles.Add(jsonFileData);
        }
    }

    internal class IngrIconColumnBehavior : IngrColumnBehavior
    {
        public IngrIconColumnBehavior(IngredientColumnGroup columnGroup)
            : base(columnGroup)
        {
        }

        public override void TryDeleteCell(RecipeRow row)
        {
            Ingredient ingredient = row.GetOrAddIngredient(this.columnGroup);
            ingredient.Name = "";
        }

        public override void ConfigureColumn(DataGridViewColumn gridCol)
        {
            base.ConfigureColumn(gridCol);

            // Remove [x] broken image icons
            if (gridCol is DataGridViewImageColumn)
            {
                gridCol.DefaultCellStyle.NullValue = null;
            }
        }
    }

    internal class IngrNameColumnBehavior : IngrColumnBehavior
    {
        private RecipesView recipesView = null;

        public IngrNameColumnBehavior(IngredientColumnGroup columnGroup, RecipesView recipesView)
            : base(columnGroup)
        {
            this.recipesView = recipesView;
        }

        public override void OnDataCellChanged(DataColumnChangeEventArgs e)
        {
            // grab col and change the image
            RecipeRow row = (RecipeRow)e.Row;
            string newName = e.ProposedValue == DBNull.Value || e.ProposedValue == null ? "" : (string)e.ProposedValue;
            Ingredient ingredient = row.GetOrAddIngredient(this.columnGroup);

            if (IsMaterial(newName))
            {
                ingredient.Icon = recipesView.GetIcon(newName);
            }
            else
            {
                JsonFileData jsonFileData = (JsonFileData)ModuleDataManager.GetInstance().GetModuleFile(newName).FileData;
                ingredient.Icon = recipesView.GetIcon(jsonFileData);
            }

            if (newName == "")
            {
                ingredient.Amount = null;
            }
            else if (ingredient.Amount == null)
            {
                ingredient.Amount = 1;
            }
        }

        public override void OnGridCellChanged(DataGridView recipesGridView, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = recipesGridView[e.ColumnIndex, e.RowIndex];
            if (cell.Value != DBNull.Value && !string.IsNullOrEmpty(cell.Value as string))
            {
                UpdateIngrNameColor(cell, (string)cell.Value);
            }
        }

        public override void TryDeleteCell(RecipeRow row)
        {
            Ingredient ingredient = row.GetOrAddIngredient(this.columnGroup);
            ingredient.Name = "";
        }

        public override void ConfigureColumnAfterRender(DataGridViewColumn gridCol, DataGridView recipesGridView)
        {
            for (int i = 0; i < recipesGridView.Rows.Count; i++)
            {
                DataGridViewCell cell = recipesGridView[gridCol.Name, i];
                UpdateIngrNameColor(cell, cell.Value);
            }
        }

        private void UpdateIngrNameColor(DataGridViewCell cell, object ingrName)
        {
            if (cell.Value != DBNull.Value && !string.IsNullOrEmpty(cell.Value as string))
            {
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                if (IsMaterial((string)cell.Value))
                {
                    style.ForeColor = Color.Navy;
                }
                else
                {
                    style.ForeColor = Color.Black;
                }

                recipesView.ApplyStyle(cell, style);
            }
        }

        private bool IsMaterial(string ingrName)
        {
            return !ingrName.Contains(":");
        }
    }

    internal class IngrAmountColumnBehavior : IngrColumnBehavior
    {

        public IngrAmountColumnBehavior(IngredientColumnGroup columnGroup)
            : base(columnGroup)
        {
        }

        public override void TryDeleteCell(RecipeRow row)
        {
            Ingredient ingredient = row.GetOrAddIngredient(this.columnGroup);
            ingredient.Name = "";
        }
    }
}
