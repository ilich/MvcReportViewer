using System.Configuration;

namespace MvcReportViewer.Configuration
{
    public class MvcReportViewerSettings : ConfigurationSection
    {
        [ConfigurationProperty("reportServerUrl")]
        public string ReportServerUrl
        {
            get { return this["reportServerUrl"] as string; }
            set { this["reportServerUrl"] = value; }
        }

        [ConfigurationProperty("username")]
        public string Username
        {
            get { return this["username"] as string; }
            set { this["username"] = value; }
        }

        [ConfigurationProperty("password")]
        public string Password
        {
            get { return this["password"] as string; }
            set { this["password"] = value; }
        }

        [ConfigurationProperty("aspxViewer", DefaultValue = "~/MvcReportViewer.aspx")]
        public string AspxViewer
        {
            get { return this["aspxViewer"] as string; }
            set { this["aspxViewer"] = value; }
        }

        [ConfigurationProperty("aspxViewerJavaScript", DefaultValue = "~/Scripts/MvcReportViewer.js")]
        public string AspxViewerJavaScript
        {
            get { return this["aspxViewerJavaScript"] as string; }
            set { this["aspxViewerJavaScript"] = value; }
        }

        [ConfigurationProperty("errorPage", DefaultValue = "~/MvcReportViewerErrorPage.html")]
        public string ErrorPage
        {
            get { return this["errorPage"] as string; }
            set { this["errorPage"] = value; }
        }

        [ConfigurationProperty("showErrorPage", DefaultValue = false)]
        public bool ShowErrorPage
        {
            get { return (bool)this["showErrorPage"]; }
            set { this["showErrorPage"] = value; }
        }

        [ConfigurationProperty("isAzureSSRS", DefaultValue = false)]
        // ReSharper disable once InconsistentNaming
        public bool IsAzureSSRS
        {
            get { return (bool)this["isAzureSSRS"]; }
            set { this["isAzureSSRS"] = value; }
        }

        [ConfigurationProperty("encryptParameters", DefaultValue = false)]
        public bool EncryptParameters
        {
            get { return (bool)this["encryptParameters"]; }
            set { this["encryptParameters"] = value; }
        }

        [ConfigurationProperty("localDataSourceProvider")]
        public string LocalDataSourceProvider
        {
            get { return this["localDataSourceProvider"] as string; }
            set { this["localDataSourceProvider"] = value; }
        }
    }
}