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
    class RecipeRow : DataRow
    {
        private List<Ingredient> mIngredients = new List<Ingredient>();

        public ReadOnlyCollection<Ingredient> Ingredients { get; private set; }

        internal RecipeRow(DataRowBuilder builder)
            : base(builder)
        {
            this.Ingredients = new ReadOnlyCollection<Ingredient>(mIngredients);
        }

        public JsonFileData RecipeList { get; set; }

        public JsonFileData Recipe { get; set; }

        public JsonFileData Item { get; set; }

        public void SetLevelRequired(int value)
        {
            this[RecipeTable.kLvlReq] = value;
        }

        public void SetEffort(int value)
        {
            this[RecipeTable.kEffort] = value;
        }

        public void SetCrafter(string value)
        {
            this[RecipeTable.kCrafter] = value;
        }

        public void SetAlias(string value)
        {
            this[RecipeTable.kAlias] = value;
        }

        public void SetNetWorth(int value)
        {
            this[RecipeTable.kNetWorth] = value;
        }

        public void SetDisplayName(string value)
        {
            this[RecipeTable.kDisplayName] = value;
        }

        public void SetIcon(Image value)
        {
            this[RecipeTable.kIcon] = value;
        }

        public Ingredient NewIngredient()
        {
            Ingredient ingr = new Ingredient(this, ((RecipeTable)Table).GetOrAddIngredientColumnGroup(mIngredients.Count));
            mIngredients.Add(ingr);
            return ingr;
        }

        public Ingredient GetOrAddIngredient(IngredientColumnGroup group)
        {
            while (!mIngredients.Any(i => i.Group == group))
            {
                Ingredient ingredientData = NewIngredient();
            }

            return mIngredients.Single(i => i.Group == group);
        }
    }
}
