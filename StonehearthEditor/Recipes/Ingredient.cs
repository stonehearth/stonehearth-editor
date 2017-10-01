using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Recipes
{
    class Ingredient
    {
        private IngredientColumnGroup columnGroup;
        private RecipeRow row;

        public IngredientColumnGroup Group => columnGroup;

        public Ingredient(RecipeRow row, IngredientColumnGroup columnGroup)
        {
            this.row = row;
            this.columnGroup = columnGroup;
        }

        public void SetName(string value)
        {
            this.row[columnGroup.NameColumnKey] = value;
        }

        public void SetIcon(Image value)
        {
            this.row[columnGroup.IconColumnKey] = value;
        }

        public void SetAmount(int value)
        {
            this.row[columnGroup.AmountColumnKey] = value;
        }
    }
}
