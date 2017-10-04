using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Recipes
{
    class Cell
    {
        private object mBeforeValue = null;
        private object mAfterValue = null;

        public object OldValue { get; set; }

        public object NewValue { get; set; }

        public RecipeRow Row { get; private set; }

        public DataColumn Column { get; private set; }

        public Cell(RecipeRow row, DataColumn column)
        {
            this.Row = row;
            this.Column = column;
        }
    }
}
