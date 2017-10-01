using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace StonehearthEditor
{
    internal class ColumnBehavior
    {
        public virtual JsonFileData GetSourceFileData(RowMetadata rowMetadata)
        {
            return null;
        }

        public virtual void OnCellChanged(DataGridView dataGridView, DataGridViewCellEventArgs e)
        {
        }
    }

    internal class ItemColumnBehavior : ColumnBehavior
    {
        public override JsonFileData GetSourceFileData(RowMetadata rowMetadata)
        {
            return rowMetadata.Item;
        }
    }

    internal class IngrIconColumnBehavior : ColumnBehavior
    {
        public override JsonFileData GetSourceFileData(RowMetadata rowMetadata)
        {
            return rowMetadata.Recipe;
        }
    }

    internal class IngrNameColumnBehavior : ColumnBehavior
    {
        private int mIngrId = 0;
        private RecipesView mRecipesView = null;

        public IngrNameColumnBehavior(RecipesView recipesView, int ingrId)
        {
            mIngrId = ingrId;
            mRecipesView = recipesView;
        }

        public override void OnCellChanged(DataGridView dataGridView, DataGridViewCellEventArgs e)
        {
            // grab col and change the image
            DataRow row = (dataGridView.DataSource as DataTable).Rows[e.RowIndex];
            string ingrIconColName = RecipesView.GetIngredientPrefix(mIngrId) + RecipesView.kName;
            row[ingrIconColName] = dataGridView;
        }

        public override JsonFileData GetSourceFileData(RowMetadata rowMetadata)
        {
            return rowMetadata.Recipe;
        }
    }

    internal class IngrAmountColumnBehavior : ColumnBehavior
    {
        public override JsonFileData GetSourceFileData(RowMetadata rowMetadata)
        {
            return rowMetadata.Recipe;
        }
    }

    internal class CrafterColumnBehavior : ColumnBehavior
    {
        public override JsonFileData GetSourceFileData(RowMetadata rowMetadata)
        {
            return rowMetadata.RecipeList;
        }
    }

    internal class LvlReqColumnBehavior : ColumnBehavior
    {
        public override JsonFileData GetSourceFileData(RowMetadata rowMetadata)
        {
            return rowMetadata.Recipe;
        }
    }

    internal class EffortColumnBehavior : ColumnBehavior
    {
        public override JsonFileData GetSourceFileData(RowMetadata rowMetadata)
        {
            return rowMetadata.Recipe;
        }
    }
}
