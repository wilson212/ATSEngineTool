using System.Collections.Generic;
using System.Windows.Forms;

namespace ATSEngineTool
{
    public class MultipleListViewColumnSorter
    {
        private List<ListViewColumnSorter> sorters;

        public MultipleListViewColumnSorter()
        {
            sorters = new List<ListViewColumnSorter>();
        }

        public void AddListView(ListView lv)
        {
            sorters.Add(new ListViewColumnSorter(lv));
        }
    }
}
