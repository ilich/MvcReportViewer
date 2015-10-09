using System.Configuration;

namespace MvcReportViewer.Configuration
{
    class ReportViewerConfiguration
    {
        private readonly MvcReportViewerSettings _settings;

        public ReportViewerConfiguration(string configSection = "MvcReportViewer")
        {
            _settings = ConfigurationManager.GetSection(configSection) as MvcReportViewerSettings;    
        }

        public string ReportServerUrl => _settings != null
            ? _settings.ReportServerUrl
            : ConfigurationManager.AppSettings["MvcReportViewer.ReportServerUrl"];

        public string Username => _settings != null
            ? _settings.Username
            : ConfigurationManager.AppSettings["MvcReportViewer.Username"];

        public string Password => _settings != null
            ? _settings.Password
            : ConfigurationManager.AppSettings["MvcReportViewer.Password"];

        public string AspxViewer => _settings != null
            ? _settings.AspxViewer
            : ConfigurationManager.AppSettings["MvcReportViewer.AspxViewer"];

        public string AspxViewerJavaScript => _settings != null
            ? _settings.AspxViewer
            : ConfigurationManager.AppSettings["MvcReportViewer.AspxViewerJavaScript"];

        public string ErrorPage => _settings != null
            ? _settings.ErrorPage
            : ConfigurationManager.AppSettings["MvcReportViewer.ErrorPage"];

        public bool ShowErrorPage => _settings?.ShowErrorPage ?? ReadBoolConfig("MvcReportViewer.ShowErrorPage");

        // ReSharper disable once InconsistentNaming
        public bool IsAzureSSRS => _settings?.IsAzureSSRS ?? ReadBoolConfig("MvcReportViewer.IsAzureSSRS");

        public bool EncryptParameters => _settings?.EncryptParameters ?? ReadBoolConfig("MvcReportViewer.EncryptParameters");

        public string LocalDataSourceProvider => _settings != null
            ? _settings.LocalDataSourceProvider
            : ConfigurationManager.AppSettings["MvcReportViewer.LocalDataSourceProvider"];

        private static bool ReadBoolConfig(string config)
        {
            var strValue = ConfigurationManager.AppSettings[config];

            bool value;
            return bool.TryParse(strValue, out value) && value;
        }
    }
}
