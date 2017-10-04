using System.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace StonehearthEditor.Recipes
{
    // Defines behavior for each column in a RecipeTable/RecipeView
    internal class ColumnBehavior
    {
        public virtual bool IsIngredientColumn => false;
        public virtual bool IsRecipeColumn => false;

        public virtual void OnCellChanged(DataColumnChangeEventArgs e)
        {
        }

        public virtual void SaveCell(HashSet<JsonFileData> modifiedFiles, RecipeRow row, object value)
        {
        }

        public virtual void TryDeleteCell(RecipeRow row)
        {
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
                        jsonFileData.TrySetFlatFileData(json.ToString());
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
            jsonFileData.TrySetFlatFileData(json.ToString());
            modifiedFiles.Add(jsonFileData);
        }
    }

    internal class CrafterColumnBehavior : ColumnBehavior
    {
        public override bool IsRecipeColumn => true;

    }

    internal class EffortColumnBehavior : ColumnBehavior
    {
        public override bool IsRecipeColumn => true;

        public override void SaveCell(HashSet<JsonFileData> modifiedFiles, RecipeRow row, object value)
        {
            JsonFileData jsonFileData = row.Recipe;
            JObject json = jsonFileData.Json;
            JValue token = json["work_units"] as JValue;
            json["work_units"] = (int)value;
            jsonFileData.TrySetFlatFileData(json.ToString());
            modifiedFiles.Add(jsonFileData);
        }
    }

    internal class LevelRequiredColumnBehavior : ColumnBehavior
    {
        public override bool IsRecipeColumn => true;

        public override void SaveCell(HashSet<JsonFileData> modifiedFiles, RecipeRow row, object value)
        {
            JsonFileData jsonFileData = row.Recipe;
            JObject json = jsonFileData.Json;
            JValue token = json["level_requirement"] as JValue;
            json["level_requirement"] = (int)value;
            jsonFileData.TrySetFlatFileData(json.ToString());
            modifiedFiles.Add(jsonFileData);
        }
    }

    internal class IngrIconColumnBehavior : ColumnBehavior
    {
        private IngredientColumnGroup columnGroup = null;

        public override bool IsIngredientColumn => true;

        public IngrIconColumnBehavior(IngredientColumnGroup columnGroup)
        {
            this.columnGroup = columnGroup;
        }

        public override void TryDeleteCell(RecipeRow row)
        {
            Ingredient ingredient = row.GetOrAddIngredient(columnGroup);
            ingredient.Name = "";
        }
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
            Ingredient ingredient = row.GetOrAddIngredient(columnGroup);

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

        public override void TryDeleteCell(RecipeRow row)
        {
            Ingredient ingredient = row.GetOrAddIngredient(columnGroup);
            ingredient.Name = "";
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
            Ingredient ingredient = row.GetOrAddIngredient(columnGroup);
        }

        public override void TryDeleteCell(RecipeRow row)
        {
            Ingredient ingredient = row.GetOrAddIngredient(columnGroup);
            ingredient.Name = "";
        }
    }
}
