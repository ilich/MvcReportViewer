using System;
using System.Collections.Generic;
using Microsoft.Reporting.WebForms;

namespace MvcReportViewer
{
    /// <summary>
    /// Interface which stores data sources for Local Report (.rdlc)
    /// </summary>
    public interface ILocalReportDataSourceProvider
    {
        /// <summary>
        /// Saves report data source.
        /// </summary>
        /// <param name="reportControlId">Internal MvcReportViewer instance ID.</param>
        /// <param name="dataSource">Report Data Source.</param>
        void Add(Guid reportControlId, ReportDataSource dataSource);

        /// <summary>
        /// Gets data sources used for the report.
        /// </summary>
        /// <param name="reportControlId">Internal MvcReportViewer instance ID.</param>
        /// <returns>The list of data sources.</returns>
        IEnumerable<ReportDataSource> Get(Guid reportControlId);
    }
}
