using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Recipes
{
    internal class CellChange
    {
        public object Before { get; private set; }

        public object After { get; private set; }

        public DataCell Cell { get; private set; }

        public CellChange(DataCell cell, object before, object after)
        {
            this.Cell = cell;
            this.Before = before;
            this.After = after;
        }

        public void Undo()
        {
            Cell.Row[Cell.Column] = this.Before;
        }

        public void Redo()
        {
            Cell.Row[Cell.Column] = this.After;
        }
    }

    internal class MultipleCellChange
    {
        private List<CellChange> mChanges = new List<CellChange>();

        public MultipleCellChange()
        {
        }

        public MultipleCellChange(DataCell cell, object before, object after)
        {
            mChanges.Add(new CellChange(cell, before, after));
        }

        public void Add(CellChange change)
        {
            mChanges.Add(change);
        }

        public void Undo()
        {
            foreach (CellChange change in mChanges)
            {
                change.Undo();
            }
        }

        public void Redo()
        {
            foreach (CellChange change in mChanges)
            {
                change.Redo();
            }
        }
    }
}
