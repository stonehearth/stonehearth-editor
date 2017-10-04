using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Recipes
{
    // Mostly a data holder for each group of columns that represent an ingredient
    class IngredientColumnGroup
    {
        public const string kIcon = "Icon";
        public const string kName = "Name";
        public const string kIngr = "Ingr";
        public const string kAmount = "Amount";

        private int mIndex = 0;
        private RecipesView mRecipesView = null;

        private static string GetIngredientPrefix(int index)
        {
            return kIngr + (index + 1) + " ";
        }

        private string prefix => GetIngredientPrefix(mIndex);

        public string AmountColumnKey => prefix + kAmount;

        public string NameColumnKey => prefix + kName;

        public string IconColumnKey => prefix + kIcon;

        public IngredientColumnGroup(RecipeTable table, int index, RecipesView recipesView)
        {
            this.mIndex = index;
            this.mRecipesView = recipesView;

            table.AddDataColumn(IconColumnKey, typeof(Image), new IngrIconColumnBehavior(this));
            table.AddDataColumn(NameColumnKey, typeof(string), new IngrNameColumnBehavior(this, recipesView));
            table.AddDataColumn(AmountColumnKey, typeof(int), new IngrAmountColumnBehavior(this));
        }
    }
}
