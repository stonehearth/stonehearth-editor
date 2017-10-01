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

        public string Name
        {
            get
            {
                object value = this.row[columnGroup.NameColumnKey];
                return value == DBNull.Value ? "" : (string)value;
            }

            set
            {
                this.row[columnGroup.NameColumnKey] = value;
            }
        }

        public Image Icon
        {
            get
            {
                object value = this.row[columnGroup.IconColumnKey];
                return value == DBNull.Value ? null : (Image)value;
            }

            set
            {
                this.row[columnGroup.IconColumnKey] = value;
            }
        }

        public int? Amount
        {
            get
            {
                object value = this.row[columnGroup.AmountColumnKey];
                return value == DBNull.Value ? 0 : (int)value;
            }

            set
            {
                if (value == null)
                {
                    this.row[columnGroup.AmountColumnKey] = DBNull.Value;
                }
                else
                {
                    this.row[columnGroup.AmountColumnKey] = value;
                }
            }
        }
    }
}
