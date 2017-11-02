using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Recipes
{
    public class RecipeTable : DataTable
    {
        public const string kIcon = "Icon";
        public const string kAlias = "Alias";
        public const string kDisplayName = "Display Name";
        public const string kNetWorth = "Net Worth";
        public const string kAppeal = "Appeal";
        public const string kCrafter = "Crafter";
        public const string kLvlReq = "Lvl Req";
        public const string kEffort = "Effort";
        public const string kWorkUnits = "Work Units";
        public const string kName = "Name";

        private readonly RecipesView mRecipesView;
        private readonly Dictionary<string, ColumnBehavior> mKeyToColumnBehavior = new Dictionary<string, ColumnBehavior>();
        private List<IngredientColumnGroup> mIngredientColumnGroups = new List<IngredientColumnGroup>();

        internal void AddDataColumn(string name, Type valueType, ColumnBehavior behavior)
        {
            Columns.Add(new DataColumn(name, valueType));
            mKeyToColumnBehavior.Add(name, behavior);
        }

        public RecipeTable(RecipesView recipesView)
        {
            this.mRecipesView = recipesView;
            AddRecipeColumns();
        }

        public void Reload()
        {
            this.Rows.Clear();
            this.Columns.Clear();
            mKeyToColumnBehavior.Clear();
            mIngredientColumnGroups.Clear();

            AddRecipeColumns();
        }

        protected override Type GetRowType()
        {
            return typeof(RecipeRow);
        }

        protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
        {
            return new RecipeRow(builder);
        }

        internal RecipeRow NewRecipeRow()
        {
            return (RecipeRow)NewRow();
        }

        private void AddIngredientColumns()
        {
            mIngredientColumnGroups.Add(new IngredientColumnGroup(this, mIngredientColumnGroups.Count, mRecipesView));
        }

        private void AddRecipeColumns()
        {
            AddDataColumn(kIcon, typeof(Image), new IconColumnBehavior());
            AddDataColumn(kAlias, typeof(string), new AliasColumnBehavior());
            AddDataColumn(kDisplayName, typeof(string), new DisplayNameColumnBehavior());
            AddDataColumn(kNetWorth, typeof(int), new NetWorthColumnBehavior());
            AddDataColumn(kAppeal, typeof(int), new AppealColumnBehavior());
            AddDataColumn(kCrafter, typeof(string), new CrafterColumnBehavior());
            AddDataColumn(kLvlReq, typeof(int), new LevelRequiredColumnBehavior());
            AddDataColumn(kEffort, typeof(int), new EffortColumnBehavior());
            AddDataColumn(kWorkUnits, typeof(int), new WorkUnitsColumnBehavior());
        }

        internal IngredientColumnGroup GetOrAddIngredientColumnGroup(int index)
        {
            while (mIngredientColumnGroups.Count <= index)
            {
                AddIngredientColumns();
            }

            return mIngredientColumnGroups[index];
        }

        internal ColumnBehavior GetColumnBehavior(DataColumn col)
        {
            return mKeyToColumnBehavior[col.ColumnName];
        }

        internal RecipeRow GetRow(int rowIndex)
        {
            return (RecipeRow)this.DefaultView[rowIndex].Row;
        }

        internal DataColumn GetColumn(int colIndex)
        {
            return this.Columns[colIndex];
        }
    }
}
