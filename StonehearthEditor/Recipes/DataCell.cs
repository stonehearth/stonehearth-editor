using System.Data;

namespace StonehearthEditor.Recipes
{
    // Cell with extra getters for the DataRow/Column
    internal class DataCell
    {
        public RecipeRow Row { get; private set; }

        public DataColumn Column { get; private set; }

        public DataCell(DataColumn column, RecipeRow row)
        {
            this.Column = column;
            this.Row = row;
        }
    }
}