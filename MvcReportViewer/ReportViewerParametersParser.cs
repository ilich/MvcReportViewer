using System;
using System.Collections.Specialized;
using System.Configuration;
using Microsoft.Reporting.WebForms;

namespace MvcReportViewer
{
    internal class ReportViewerParametersParser
    {
        public ReportViewerParameters Parse(NameValueCollection queryString)
        {
            if (queryString == null)
            {
                throw new ArgumentNullException("QueryString cannot be null.");
            }

            var parameters = InitializeDefaults();
            ResetDefaultCredentials(queryString, parameters);

            foreach (var key in queryString.AllKeys)
            {
                if (key.EqualsIgnoreCase(UriParameters.ReportPath))
                {
                    parameters.ReportPath = queryString[key];
                }
                else if (key.EqualsIgnoreCase(UriParameters.ReportServerUrl))
                {
                    parameters.ReportServerUrl = queryString[key];
                }
                else if (key.EqualsIgnoreCase(UriParameters.Username))
                {
                    parameters.Username = queryString[key];
                }
                else if (key.EqualsIgnoreCase(UriParameters.Password))
                {
                    parameters.Password = queryString[key];
                }
                else if (key.EqualsIgnoreCase(UriParameters.ShowParameterPrompts))
                {
                    bool value;
                    if (bool.TryParse(queryString[key], out value))
                    {
                        parameters.ShowParameterPrompts = value;
                    }
                }
                else
                {
                    if (parameters.ReportParameters.ContainsKey(key))
                    {
                        parameters.ReportParameters[key].Values.Add(queryString[key]);
                    }
                    else
                    {
                        var reportParameter = new ReportParameter(key);
                        reportParameter.Values.Add(queryString[key]);
                        parameters.ReportParameters.Add(key, reportParameter);
                    }
                }
            }

            if (string.IsNullOrEmpty(parameters.ReportServerUrl))
            {
                throw new MvcReportViewerException("Report Server is not specified.");
            }

            if (string.IsNullOrEmpty(parameters.ReportPath))
            {
                throw new MvcReportViewerException("Report is not specified.");
            }

            return parameters;
        }

        private static void ResetDefaultCredentials(NameValueCollection queryString, ReportViewerParameters parameters)
        {
            if (queryString.ContainsKeyIgnoreCase(UriParameters.Username) ||
                queryString.ContainsKeyIgnoreCase(UriParameters.Password))
            {
                parameters.Username = string.Empty;
                parameters.Password = string.Empty;
            }
        }

        private ReportViewerParameters InitializeDefaults()
        {
            var parameters = new ReportViewerParameters();
            parameters.ReportServerUrl = ConfigurationManager.AppSettings[WebConfigSettings.Server];
            parameters.Username = ConfigurationManager.AppSettings[WebConfigSettings.Username];
            parameters.Password = ConfigurationManager.AppSettings[WebConfigSettings.Password];

            bool showPrompts;
            if (!bool.TryParse(ConfigurationManager.AppSettings[WebConfigSettings.ShowParameterPrompts], out showPrompts))
            {
                showPrompts = false;
            }

            parameters.ShowParameterPrompts =  showPrompts;
            return parameters;
        }
    }
}
