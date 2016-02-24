using System;
using System.Collections.Specialized;
using Microsoft.Reporting.WebForms;
using System.Web;
using MvcReportViewer.Configuration;
using Newtonsoft.Json;

namespace MvcReportViewer
{
    internal class ReportViewerParametersParser
    {
        private static readonly ReportViewerConfiguration Config = new ReportViewerConfiguration();

        public ReportViewerParameters Parse(NameValueCollection queryString)
        {
            if (queryString == null)
            {
                throw new ArgumentNullException(nameof(queryString));
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
                else if (key.EqualsIgnoreCase(UriParameters.ControlId))
                {
                    var parameter = isEncrypted ? SecurityUtil.Decrypt(urlParam) : urlParam;
                    parameters.ControlId = Guid.Parse(parameter);
                }
                else if (key.EqualsIgnoreCase(UriParameters.ProcessingMode))
                {
                    var parameter = isEncrypted ? SecurityUtil.Decrypt(urlParam) : urlParam;
                    parameters.ProcessingMode = (ProcessingMode)Enum.Parse(typeof(ProcessingMode), parameter);
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
                else if (key.EqualsIgnoreCase(UriParameters.EventsHandlerType))
                {
                    parameters.EventsHandlerType = isEncrypted ? SecurityUtil.Decrypt(urlParam) : urlParam;
                }
                else if (key.EqualsIgnoreCase(UriParameters.DataSourceCredentials))
                {
                    var json = isEncrypted ? SecurityUtil.Decrypt(urlParam) : urlParam;
                    parameters.DataSourceCredentials = JsonConvert.DeserializeObject<DataSourceCredentials[]>(json);
                }
                else if (!settinsManager.IsControlSetting(key))
                {
                    var values = queryString.GetValues(key);
                    if (values != null)
                    {
                        foreach (var value in values)
                        {
                            var realValue = isEncrypted ? SecurityUtil.Decrypt(value) : value;
                            var parsedKey = ParseKey(key);
                            var realKey = parsedKey.Item1;
                            var isVisible = parsedKey.Item2;

                            if (parameters.ReportParameters.ContainsKey(realKey))
                            {
                                parameters.ReportParameters[realKey].Values.Add(realValue);
                            }
                            else
                            {
                                var reportParameter = new ReportParameter(realKey) {Visible = isVisible};
                                reportParameter.Values.Add(realValue);
                                parameters.ReportParameters.Add(realKey, reportParameter);
                            }
                        }
                    }
                }
            }

            if (parameters.ProcessingMode == ProcessingMode.Remote 
                && string.IsNullOrEmpty(parameters.ReportServerUrl))
            {
                throw new MvcReportViewerException("Report Server is not specified.");
            }

            if (string.IsNullOrEmpty(parameters.ReportPath))
            {
                throw new MvcReportViewerException("Report is not specified.");
            }

            return parameters;
        }

        private static Tuple<string, bool> ParseKey(string key)
        {
            if (!key.Contains(MvcReportViewerIframe.VisibilitySeparator))
            {
                return new Tuple<string, bool>(key, true);
            }

            var parts = key.Split(new[] { MvcReportViewerIframe.VisibilitySeparator }, StringSplitOptions.RemoveEmptyEntries);
            bool isVisible;
            if (parts.Length != 2 || !bool.TryParse(parts[1], out isVisible))
            {
                return new Tuple<string, bool>(key, true);
            }

            return new Tuple<string, bool>(parts[0], isVisible);
        }

        private static bool CheckEncryption(ref NameValueCollection source)
        {
            var isEncrypted = Config.EncryptParameters;

            // each parameter is encrypted when POST method is used
            if (string.Compare(HttpContext.Current.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return isEncrypted;
            }

            if (!isEncrypted)
            {
                return false;
            }

            var encrypted = source[UriParameters.Encrypted];
            var decrypted = SecurityUtil.Decrypt(encrypted);
            source = HttpUtility.ParseQueryString(decrypted);
            return false;                                       // Return false here because we have already decrypted query string
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
            var parameters = new ReportViewerParameters
                {
                    ReportServerUrl = Config.ReportServerUrl,
                    Username = Config.Username,
                    Password = Config.Password,
                    IsAzureSsrs = Config.IsAzureSSRS
                };

            return parameters;
        }
    }
}
