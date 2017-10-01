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
                JObject fileJson = file.Json;
                JValue nameToken = fileJson.SelectToken("entity_data.stonehearth:catalog.display_name") as JValue;
                if (nameToken != null)
                {
                    string locKey = nameToken.Value.ToString();
                    int i18nLength = "i18n(".Length;
                    locKey = locKey.Substring(i18nLength, locKey.Length - i18nLength - 1);
                    ModuleDataManager.GetInstance().ChangeEnglishLocValue(locKey, (string)value);
                }
            }
            jsonFileData.TrySaveFile();
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

        public override void OnCellChanged(DataColumnChangeEventArgs e)
        {
            // grab col and change the image
            RecipeRow row = (RecipeRow)e.Row;
            Ingredient ingredient = row.GetIngredient(columnGroup);
            string newName = (string)e.ProposedValue;
            if (newName.Contains(":"))
            {
                JsonFileData jsonFileData = (JsonFileData)ModuleDataManager.GetInstance().GetModuleFile(newName).FileData;
                ingredient.SetIcon(recipesView.GetIcon(jsonFileData));
            }
            else
            {
                ingredient.SetIcon(recipesView.GetIcon(newName));
            }
        }
    }

    internal class IngrAmountColumnBehavior : ColumnBehavior
    {
    }

    internal class CrafterColumnBehavior : ColumnBehavior
    {
    }
}
