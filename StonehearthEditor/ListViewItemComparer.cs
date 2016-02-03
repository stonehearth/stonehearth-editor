using System;
using System.Collections;
using System.Windows.Forms;

namespace StonehearthEditor
{
   public class ListViewItemComparer : IComparer
   {
      private int column;
      private SortOrder order;

      public ListViewItemComparer(int column)
      {
         this.column = column;
      }

      public ListViewItemComparer()
      {
         this.column = 0;
         this.order = SortOrder.Ascending;
      }

      public ListViewItemComparer(int column, SortOrder order)
      {
         this.column = column;
         this.order = order;
      }
      public int Compare(object x, object y)
      {
         int returnVal = -1;
         string s1 = ((ListViewItem)x).SubItems[column].Text;
         string s2 = ((ListViewItem)y).SubItems[column].Text;
         int i1, i2;
         bool r1 = int.TryParse(s1, out i1);
         bool r2 = int.TryParse(s2, out i2);
         returnVal = r1 && r2 ? i1.CompareTo(i2) : string.Compare(s1, s2);
         if (order == SortOrder.Descending)
            returnVal *= -1;

         return returnVal;
      }
   }
}