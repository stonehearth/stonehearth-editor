using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Recipes
{
    // Represents a single ingredient in a recipe row
    // Getters/setters for name, amount, icon that reflect the recipe grid
    class Ingredient
    {
        private IngredientColumnGroup mColumnGroup;
        private RecipeRow mRow;

        public IngredientColumnGroup Group => mColumnGroup;

        public Ingredient(RecipeRow row, IngredientColumnGroup columnGroup)
        {
            this.mRow = row;
            this.mColumnGroup = columnGroup;
        }

        public string Name
        {
            get
            {
                object value = this.mRow[mColumnGroup.NameColumnKey];
                return value == DBNull.Value ? "" : (string)value;
            }

            set
            {
                this.mRow[mColumnGroup.NameColumnKey] = value;
            }
        }

        public Image Icon
        {
            get
            {
                object value = this.mRow[mColumnGroup.IconColumnKey];
                return value == DBNull.Value ? null : (Image)value;
            }

            set
            {
                this.mRow[mColumnGroup.IconColumnKey] = value;
            }
        }

        public int? Amount
        {
            get
            {
                object value = this.mRow[mColumnGroup.AmountColumnKey];
                return value == DBNull.Value ? null : (int?)value;
            }

            set
            {
                this.mRow[mColumnGroup.AmountColumnKey] = value ?? (object)DBNull.Value;
            }
        }
    }
}
