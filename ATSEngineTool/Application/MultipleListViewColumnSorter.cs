using System.Collections.Generic;
using System.Windows.Forms;

namespace ATSEngineTool
{
    /// <summary>
    /// Creates a container of <see cref="ListViewColumnSorter"/>'s for
    /// <see cref="ListView"/> objects
    /// </summary>
    public class MultipleListViewColumnSorter
    {
        private List<ListViewColumnSorter> sorters;

        /// <summary>
        /// Creates a new instance of <see cref="MultipleListViewColumnSorter"/>
        /// </summary>
        public MultipleListViewColumnSorter()
        {
            sorters = new List<ListViewColumnSorter>();
        }

        /// <summary>
        /// Creates a <see cref="ListViewColumnSorter"/> and attaches it to the specified 
        /// <see cref="ListView"/> for sorting.
        /// </summary>
        /// <param name="lv">The <see cref="ListView"/></param> intended for sorting
        /// <param name="initialSortOrder">The initial sorting order of the list</param>
        public void AddListView(ListView lv, SortOrder initialSortOrder = SortOrder.None)
        {
            sorters.Add(new ListViewColumnSorter(lv) { Order = initialSortOrder });
        }
    }
}
