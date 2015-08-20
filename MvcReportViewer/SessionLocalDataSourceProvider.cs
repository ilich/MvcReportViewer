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
            var source = dataSource as ReportDataSource ?? new ReportDataSource(dataSourceName, dataSource);

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
                    Name = source.Name,
                    Value = source.Value
                });

            _session[key] = dataSources;
        }

        public IEnumerable<ReportDataSource> Get(Guid reportControlId)
        {
            var key = GetSessionValueKey(reportControlId);
            var dataSources = _session[key] as List<ReportDataSourceWrapper>;
            return dataSources?.Select(s => new ReportDataSource(s.Name, s.Value)) ?? new List<ReportDataSource>();
        }

        [Serializable]
        private class ReportDataSourceWrapper
        {
            public string Name { get; set; }

            public object Value { get; set; }
        }
    }
}
