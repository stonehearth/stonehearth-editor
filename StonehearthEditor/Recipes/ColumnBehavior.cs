using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace StonehearthEditor.Recipes
{
    internal class ColumnBehavior
    {
        public virtual bool IsIngredientColumn => false;

        public virtual void OnCellChanged(DataColumnChangeEventArgs e)
        {
        }

        public virtual void SaveCell(RecipeRow row, object value)
        {

        }
    }

    internal class DisplayNameColumnBehavior : ColumnBehavior
    {
        public override void SaveCell(RecipeRow row, object value)
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
                        jsonFileData.TrySetFlatFileData(json.ToString());
                        jsonFileData.TrySaveFile();
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
        public override void SaveCell(RecipeRow row, object value)
        {
            JsonFileData jsonFileData = row.Item;
            JObject json = jsonFileData.Json;
            JValue token = json.SelectToken("entity_data.stonehearth:net_worth.value_in_gold") as JValue;
            token.Value = (int)value;
            jsonFileData.TrySetFlatFileData(json.ToString());
            jsonFileData.TrySaveFile();
        }
    }

    internal class EffortColumnBehavior : ColumnBehavior
    {
        public override void SaveCell(RecipeRow row, object value)
        {
            JsonFileData jsonFileData = row.Recipe;
            JObject json = jsonFileData.Json;
            JValue token = json["work_units"] as JValue;
            json["work_units"] = (int)value;
            jsonFileData.TrySetFlatFileData(json.ToString());
            jsonFileData.TrySaveFile();
        }
    }

    internal class LevelRequiredColumnBehavior : ColumnBehavior
    {
        public override void SaveCell(RecipeRow row, object value)
        {
            JsonFileData jsonFileData = row.Recipe;
            JObject json = jsonFileData.Json;
            JValue token = json["level_requirement"] as JValue;
            json["level_requirement"] = (int)value;
            jsonFileData.TrySetFlatFileData(json.ToString());
            jsonFileData.TrySaveFile();
        }
    }

    internal class IngrIconColumnBehavior : ColumnBehavior
    {
        public override bool IsIngredientColumn => true;
    }

    internal class IngrNameColumnBehavior : ColumnBehavior
    {
        private IngredientColumnGroup columnGroup = null;
        private RecipesView recipesView = null;

        public IngrNameColumnBehavior(IngredientColumnGroup columnGroup, RecipesView recipesView)
        {
            this.columnGroup = columnGroup;
            this.recipesView = recipesView;
        }

        public override bool IsIngredientColumn => true;

        public override void OnCellChanged(DataColumnChangeEventArgs e)
        {
            // grab col and change the image
            RecipeRow row = (RecipeRow)e.Row;
            string newName = (string)e.ProposedValue ?? "";
            Ingredient ingredient = row.GetIngredient(columnGroup);

            if (newName.Contains(":"))
            {
                JsonFileData jsonFileData = (JsonFileData)ModuleDataManager.GetInstance().GetModuleFile(newName).FileData;
                ingredient.Icon = recipesView.GetIcon(jsonFileData);
            }
            else
            {
                ingredient.Icon = recipesView.GetIcon(newName);
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
    }

    internal class IngrAmountColumnBehavior : ColumnBehavior
    {
        public override bool IsIngredientColumn => true;

        private IngredientColumnGroup columnGroup = null;

        public IngrAmountColumnBehavior(IngredientColumnGroup columnGroup)
        {
            this.columnGroup = columnGroup;
        }

        public override void OnCellChanged(DataColumnChangeEventArgs e)
        {
            RecipeRow row = (RecipeRow)e.Row;
            Ingredient ingredient = row.GetIngredient(columnGroup);
        }
    }

    internal class CrafterColumnBehavior : ColumnBehavior
    {
    }
}
