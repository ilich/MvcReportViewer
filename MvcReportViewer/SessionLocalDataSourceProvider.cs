using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Microsoft.Reporting.WebForms;

namespace MvcReportViewer
{
    public class SessionLocalDataSourceProvider : BaseLocalDataSourceProvider, ILocalReportDataSourceProvider
    {
        private readonly HttpSessionState _session = HttpContext.Current.Session;

        public void Add<T>(Guid reportControlId, string dataSourceName, T dataSource)
        {
            throw new NotSupportedException();
        }

        public void Add(Guid reportControlId, ReportDataSource dataSource)
        {
            if (dataSource == null)
            {
                return;
            }

            var key = GetSessionValueKey(reportControlId);
            var dataSources = _session[key] as List<ReportDataSourceWrapper>;
            dataSources = dataSources ?? new List<ReportDataSourceWrapper>();

            dataSources.Add(
                new ReportDataSourceWrapper
                {
                    Name = dataSource.Name,
                    Value = (DataTable)dataSource.Value
                });

            _session[key] = dataSources;
        }

        public IEnumerable<ReportDataSource> Get(Guid reportControlId)
        {
            var key = GetSessionValueKey(reportControlId);
            var dataSources = _session[key] as List<ReportDataSourceWrapper>;
            return dataSources == null
                ? new List<ReportDataSource>()
                : dataSources.Select(s => new ReportDataSource(s.Name, s.Value));
        }

        [Serializable]
        private class ReportDataSourceWrapper
        {
            public string Name { get; set; }

            public DataTable Value { get; set; }
        }
    }
}
