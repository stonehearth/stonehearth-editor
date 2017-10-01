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
        internal RecipeRow(DataRowBuilder builder) : base(builder)
        {
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

        private List<Ingredient> ingredients = new List<Ingredient>();

        public Ingredient NewIngredient()
        {
            Ingredient ingr = new Ingredient(this, ((RecipeTable)Table).GetIngredientColumnGroup(ingredients.Count));
            ingredients.Add(ingr);
            return ingr;
        }

        public Ingredient GetIngredient(IngredientColumnGroup group)
        {
            return ingredients.Single(i => i.Group == group);
        }
    }
}
