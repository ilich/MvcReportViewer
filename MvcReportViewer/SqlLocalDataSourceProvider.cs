using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.SessionState;
using Microsoft.Reporting.WebForms;

namespace MvcReportViewer
{
    public class SqlLocalDataSourceProvider : BaseLocalDataSourceProvider, ILocalReportDataSourceProvider
    {
        private static readonly string Config = "SqlLocalDataSourceProvider.ConnectionString";

        private readonly HttpSessionState _session = HttpContext.Current.Session;

        public void Add<T>(Guid reportControlId, string dataSourceName, T dataSource)
        {
            // This local report data source provider works only with string
            // parameters which are SQL queries
            if (!(dataSource is string))
            {
                throw new InvalidOperationException("SqlLocalDataSourceProvider supports only SQL queries (string)");
            }

            var key = GetSessionValueKey(reportControlId);
            var dataSources = _session[key] as List<SqlDataSource>;
            dataSources = dataSources ?? new List<SqlDataSource>();

            dataSources.Add(
                new SqlDataSource
                {
                    Name = dataSourceName,
                    Query = dataSource.ToString()
                });

            _session[key] = dataSources;
        }

        public IEnumerable<ReportDataSource> Get(Guid reportControlId)
        {
            var key = GetSessionValueKey(reportControlId);
            var queries = _session[key] as List<SqlDataSource>;

            var dataSources = new List<ReportDataSource>();
            if (queries == null || queries.Count == 0)
            {
                return dataSources;
            }

            var connectionString = GetConnectionString();
            var dbProviderFactory = DbProviderFactories.GetFactory(connectionString.ProviderName);
            using (var connection = dbProviderFactory.CreateConnection())
            {
                if (connection == null)
                {
                    throw new InvalidOperationException(connectionString.ProviderName);
                }

                connection.ConnectionString = connectionString.ConnectionString;

                foreach (var query in queries)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query.Query;

                        using (var adapter = dbProviderFactory.CreateDataAdapter())
                        {
                            if (adapter == null)
                            {
                                throw new InvalidOperationException("Cannot create Data Adapter for " + connectionString.ProviderName);
                            }

                            adapter.SelectCommand = command;

                            var table = new DataTable();
                            adapter.Fill(table);
                            dataSources.Add(new ReportDataSource(query.Name, table));
                        }
                    }
                }
            }

            return dataSources;
        }

        private ConnectionStringSettings GetConnectionString()
        {
            var connectionStringName = ConfigurationManager.AppSettings[Config];
            if (string.IsNullOrEmpty(connectionStringName))
            {
                throw new InvalidOperationException(Config + " is not set in application configuration file");
            }

            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName];
            return connectionString;
        }

        [Serializable]
        private class SqlDataSource
        {
            public string Name { get; set; }

            public string Query { get; set; }
        }
    }
}
