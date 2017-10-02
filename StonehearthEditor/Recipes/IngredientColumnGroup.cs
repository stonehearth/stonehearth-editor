using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Recipes
{
    class IngredientColumnGroup
    {
        public const string kIcon = "Icon";
        public const string kName = "Name";
        public const string kIngr = "Ingr";
        public const string kAmount = "Amount";
        private int index = 0;
        private RecipesView recipesView = null;

        private static string GetIngredientPrefix(int index)
        {
            return kIngr + (index + 1) + " ";
        }

        private string prefix => GetIngredientPrefix(index);

        public string AmountColumnKey => prefix + kAmount;

        public string NameColumnKey => prefix + kName;

        public string IconColumnKey => prefix + kIcon;

        public IngredientColumnGroup(RecipeTable table, int index, RecipesView recipesView)
        {
            this.index = index;
            this.recipesView = recipesView;

            table.AddDataColumn(prefix + kIcon, typeof(Image), new IngrIconColumnBehavior());
            table.AddDataColumn(prefix + kName, typeof(string), new IngrNameColumnBehavior(this, recipesView));
            table.AddDataColumn(prefix + kAmount, typeof(int), new IngrAmountColumnBehavior(this));
        }
    }
}
