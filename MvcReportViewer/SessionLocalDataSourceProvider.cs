using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using Microsoft.Reporting.WebForms;

namespace MvcReportViewer
{
    public class SessionLocalDataSourceProvider : ILocalReportDataSourceProvider
    {
        private readonly HttpSessionState _session = HttpContext.Current.Session;

        public void Add(Guid reportControlId, ReportDataSource dataSource)
        {
            if (dataSource == null)
            {
                return;
            }

            var key = GetSessionValueKey(reportControlId);
            var dataSources = _session[key] as List<ReportDataSource>;
            if (dataSources == null)
            {
                dataSources = new List<ReportDataSource>();
            }

            dataSources.Add(dataSource);
            _session[key] = dataSources;
        }

        public IEnumerable<ReportDataSource> Get(Guid reportControlId)
        {
            var key = GetSessionValueKey(reportControlId);
            var dataSources = _session[key] as List<ReportDataSource>;
            return dataSources ?? new List<ReportDataSource>();
        }

        private static string GetSessionValueKey(Guid reportControlId)
        {
            return string.Format("MvcReportViewer_Local_{0}", reportControlId);
        }
    }
}
