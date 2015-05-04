using System;
using System.Configuration;
using System.Web.Mvc;

namespace MvcReportViewer
{
    public class LocalReportDataSourceProviderFactory
    {
        private static object syncRoot = new object();

        private static LocalReportDataSourceProviderFactory _factory;

        public static LocalReportDataSourceProviderFactory Current
        {
            get
            {
                if (_factory == null)
                {
                    var resolver = DependencyResolver.Current;
                    lock(syncRoot)
                    {
                        _factory = resolver.GetService<LocalReportDataSourceProviderFactory>();
                        if (_factory == null)
                        {
                            _factory = new LocalReportDataSourceProviderFactory();
                        }
                    }
                }

                return _factory;
            }
        }

        public virtual ILocalReportDataSourceProvider Create()
        {
            var resolver = DependencyResolver.Current;
            var provider = resolver.GetService<ILocalReportDataSourceProvider>();
            if (provider != null)
            {
                return provider;
            }

            // Try to get data source provider from database settings

            var providerTypeName = ConfigurationManager.AppSettings[WebConfigSettings.LocalDataSourceProvider];
            if (string.IsNullOrEmpty(providerTypeName))
            {
                throw new MvcReportViewerException(
                    string.Format("{0} configuration is not found in the Web.config", WebConfigSettings.LocalDataSourceProvider));
            }

            try
            {
                var providerType = Type.GetType(providerTypeName);
                provider = (ILocalReportDataSourceProvider)Activator.CreateInstance(providerType);

                return provider;
            }
            catch (Exception err)
            {
                throw new MvcReportViewerException(
                    string.Format("{0} configuration in the Web.config is not correct", WebConfigSettings.LocalDataSourceProvider),
                    err);
            }
        }
    }
}
