using System.Collections.Generic;

namespace MvcReportViewer
{
    public class ReportHierarchy
    {
        /// <summary>
        /// Construct empty list
        /// </summary>
        public ReportHierarchy()
        {
            CatalogEntries = new List<CatalogEntry>();
        }

        /// <summary>
        /// Dictionary Name
        /// </summary>
        public static IList<CatalogEntry> CatalogEntries { get; set; }
    }

    public class CatalogEntry
    {
        /// <summary>
        /// Item Id 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Parent Id
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// IsReport
        /// </summary>
        public bool IsReport { get; set; }

        /// <summary>
        /// Item Name
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Full Path Name used for SSRS
        /// </summary>
        public string FullPath { get; set; }
    }
}