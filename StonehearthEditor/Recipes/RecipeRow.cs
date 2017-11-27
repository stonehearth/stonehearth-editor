using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Recipes
{
    // Data row that holds additional info like the JsonFileData for each json file represented in the row,
    // all ingredients for a single recipe, and cell data for recipe item columns
    class RecipeRow : DataRow
    {
        private List<Ingredient> mIngredients = new List<Ingredient>();

        public ReadOnlyCollection<Ingredient> Ingredients { get; private set; }

        public JsonFileData RecipeList { get; set; }

        public JsonFileData Recipe { get; set; }

        public JsonFileData Item { get; set; }

        internal RecipeRow(DataRowBuilder builder)
            : base(builder)
        {
            this.Ingredients = new ReadOnlyCollection<Ingredient>(mIngredients);
        }

        public void SetLevelRequired(int? value)
        {
            SetGridCell(RecipeTable.kLvlReq, value);
        }

        public void SetWorkUnits(int? value)
        {
            SetGridCell(RecipeTable.kWorkUnits, value);
        }

        public void SetAppeal(int? value)
        {
            SetGridCell(RecipeTable.kAppeal, value);
        }

        public void SetIsVariableQuality(bool? value)
        {
            SetGridCell(RecipeTable.kIsVariableQuality, value);
        }

        public void SetEffort(int? value)
        {
            SetGridCell(RecipeTable.kEffort, value);
        }

        public void SetCrafter(string value)
        {
            SetGridCell(RecipeTable.kCrafter, value);
        }

        public void SetAlias(string value)
        {
            SetGridCell(RecipeTable.kAlias, value);
        }

        public void SetNetWorth(int value)
        {
            SetGridCell(RecipeTable.kNetWorth, value);
        }

        public void SetDisplayName(string value)
        {
            SetGridCell(RecipeTable.kDisplayName, value);
        }

        public void SetIcon(Image value)
        {
            SetGridCell(RecipeTable.kIcon, value);
        }

        public Ingredient AddNewIngredient()
        {
            Ingredient ingr = new Ingredient(this, ((RecipeTable)Table).GetOrAddIngredientColumnGroup(mIngredients.Count));
            mIngredients.Add(ingr);
            return ingr;
        }

        public Ingredient GetOrAddIngredient(IngredientColumnGroup group)
        {
            while (!mIngredients.Any(i => i.Group == group))
            {
                Ingredient ingredientData = AddNewIngredient();
            }

            return mIngredients.Single(i => i.Group == group);
        }

        private void SetGridCell(string key, object value)
        {
            if (value != null)
            {
                this[key] = value;
            }
            else
            {
                this[key] = DBNull.Value;
            }
        }
    }
}
