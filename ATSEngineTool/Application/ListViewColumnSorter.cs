using System.Collections;
using System.Windows.Forms;

namespace ATSEngineTool
{
    /// <summary>
    /// This class is used to perform sorting behavior on a ListView when a 
    /// column header is clicked
    /// </summary>
    public class ListViewColumnSorter : IComparer
    {
        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        private CaseInsensitiveComparer ObjectCompare;

        /// <summary>
        /// Gets or sets the zero-based index of the column to apply the sorting operation on.
        /// </summary>
        public int SortColumn { get; set; } = 0;

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order { get; set; } = SortOrder.None;

        /// <summary>
        /// The listview we are sorting
        /// </summary>
        private ListView listView;

        /// <summary>
        /// Creates a new instance of <see cref="ListViewColumnSorter"/>
        /// </summary>
        public ListViewColumnSorter(ListView lv)
        {
            // Setup the listview to use this sorter instance
            listView = lv;
            listView.ListViewItemSorter = this;
            listView.ColumnClick += listView_ColumnClick;

            // Initialize the CaseInsensitiveComparer object
            ObjectCompare = new CaseInsensitiveComparer();
        }

        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ReverseSortOrderAndSort(e.Column, (ListView)sender);
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX, listviewY;

            // Cast the objects to be compared to ListViewItem objects
            listviewX = (ListViewItem)x;
            listviewY = (ListViewItem)y;

            // Compare the two items
            compareResult = ObjectCompare.Compare(listviewX.SubItems[SortColumn].Text, listviewY.SubItems[SortColumn].Text);

            // Calculate correct return value based on object comparison
            if (Order == SortOrder.Ascending)
            {
                // Ascending sort is selected, return normal result of compare operation
                return compareResult;
            }
            else if (Order == SortOrder.Descending)
            {
                // Descending sort is selected, return negative result of compare operation
                return (-compareResult);
            }
            else
            {
                // Return '0' to indicate they are equal
                return 0;
            }
        }

        private void ReverseSortOrderAndSort(int column, ListView lv)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (column == this.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (Order == SortOrder.Ascending)
                {
                    Order = SortOrder.Descending;
                }
                else
                {
                    Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                SortColumn = column;
                Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            lv.Sort();
        }
    }
}
