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

            //convert "none" to 0 when appropriate (when one side is a number and the other is "none")
            if (r1 && !r2 && s2 == "none")
            {
                i2 = 0;
                r2 = true;
            }
            else if (r2 && !r1 && s1 == "none")
            {
                i1 = 0;
                r1 = true;
            }
            
            returnVal = r1 && r2 ? i1.CompareTo(i2) : string.Compare(s1, s2);
            if (order == SortOrder.Descending)
                returnVal *= -1;

            return returnVal;
        }
    }
}