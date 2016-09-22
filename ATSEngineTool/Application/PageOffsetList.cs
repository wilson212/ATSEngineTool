using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ATSEngineTool.Database;

namespace ATSEngineTool
{
    /// <summary>
    /// A class used to specify the total records in a <see cref="BindingNavigator"/>
    /// </summary>
    public class PageOffsetList : IListSource
    {
        /// <summary>
        /// Sets the number of records to display on the Engines Data Grid View
        /// </summary>
        public int PageSize { get; set; } = 50;

        public bool ContainsListCollection { get; protected set; }

        /// <summary>
        /// Gets the total number of records
        /// </summary>
        public int TotalRecords => Engines.Count;

        /// <summary>
        /// Gets the internal list
        /// </summary>
        public List<Engine> Engines { get; protected set; }

        /// <summary>
        /// Creates a new instance of PageOffsetList
        /// </summary>
        /// <param name="engines"></param>
        public PageOffsetList(List<Engine> engines)
        {
            Engines = engines;
        }

        public IList GetList()
        {
            // Return a list of page offsets based on "totalRecords" and "pageSize"
            var pageOffsets = new List<int>();
            for (int offset = 0; offset <= Engines.Count; offset += PageSize)
                pageOffsets.Add(offset);
            return pageOffsets;
        }
    }
}
