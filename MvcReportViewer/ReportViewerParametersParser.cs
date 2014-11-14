using System;
using System.Collections.Specialized;
using System.Configuration;
using Microsoft.Reporting.WebForms;
using System.Web;

namespace MvcReportViewer
{
    internal class ReportViewerParametersParser
    {
        public ReportViewerParameters Parse(NameValueCollection queryString)
        {
            if (queryString == null)
            {
                throw new ArgumentNullException("queryString");
            }

            var isEncrypted = CheckEncryption(ref queryString);

            var settinsManager = new ControlSettingsManager(isEncrypted);

            var parameters = InitializeDefaults();
            ResetDefaultCredentials(queryString, parameters);
            parameters.ControlSettings = settinsManager.Deserialize(queryString);

            foreach (var key in queryString.AllKeys)
            {
                var urlParam = queryString[key];
                if (key.EqualsIgnoreCase(UriParameters.ReportPath))
                {
                    parameters.ReportPath = isEncrypted ? SecurityUtil.Decrypt(urlParam) : urlParam;
                }
                else if (key.EqualsIgnoreCase(UriParameters.ReportServerUrl))
                {
                    parameters.ReportServerUrl = isEncrypted ? SecurityUtil.Decrypt(urlParam) : urlParam;
                }
                else if (key.EqualsIgnoreCase(UriParameters.Username))
                {
                    parameters.Username = isEncrypted ? SecurityUtil.Decrypt(urlParam) : urlParam;
                }
                else if (key.EqualsIgnoreCase(UriParameters.Password))
                {
                    parameters.Password = isEncrypted ? SecurityUtil.Decrypt(urlParam) : urlParam;
                }
                else if (!settinsManager.IsControlSetting(key))
                {
                    var values = queryString.GetValues(key);
                    if (values != null)
                    {
                        foreach (var value in values)
                        {
                            var realValue = isEncrypted ? SecurityUtil.Decrypt(value) : value;

                            if (parameters.ReportParameters.ContainsKey(key))
                            {
                                parameters.ReportParameters[key].Values.Add(realValue);
                            }
                            else
                            {
                                var reportParameter = new ReportParameter(key);
                                reportParameter.Values.Add(realValue);
                                parameters.ReportParameters.Add(key, reportParameter);
                            }
                        }
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

        private static bool CheckEncryption(ref NameValueCollection source)
        {
            bool isEncrypted;
            var encryptParametesConfig = ConfigurationManager.AppSettings[WebConfigSettings.EncryptParameters];
            if (!bool.TryParse(encryptParametesConfig, out isEncrypted))
            {
                isEncrypted = false;
            }

            // each parameter is encrypted when POST method is used
            if (string.Compare(HttpContext.Current.Request.HttpMethod, "POST", true) == 0)
            {
                return isEncrypted;
            }

            if (!isEncrypted)
            {
                return isEncrypted;
            }

            var encrypted = source[UriParameters.Encrypted];
            var decrypted = SecurityUtil.Decrypt(encrypted);
            isEncrypted = false;
            source = HttpUtility.ParseQueryString(decrypted);
            return isEncrypted;
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
            var isAzureSSRS = ConfigurationManager.AppSettings[WebConfigSettings.IsAzureSSRS];
            bool isAzureSSRSValue;
            if (string.IsNullOrEmpty(isAzureSSRS) || !bool.TryParse(isAzureSSRS, out isAzureSSRSValue))
            {
                isAzureSSRSValue = false;
            }
            
            var parameters = new ReportViewerParameters
                {
                    ReportServerUrl = ConfigurationManager.AppSettings[WebConfigSettings.Server],
                    Username = ConfigurationManager.AppSettings[WebConfigSettings.Username],
                    Password = ConfigurationManager.AppSettings[WebConfigSettings.Password],
                    IsAzureSSRS = isAzureSSRSValue
                };

            return parameters;
        }
    }
}
