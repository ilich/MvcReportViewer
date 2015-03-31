using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace MvcReportViewer
{
    internal class ReportRunner
    {
        private readonly ReportViewerParameters _viewerParameters = new ReportViewerParameters
            {
                ReportServerUrl = ConfigurationManager.AppSettings[WebConfigSettings.Server],
                Username = ConfigurationManager.AppSettings[WebConfigSettings.Username],
                Password = ConfigurationManager.AppSettings[WebConfigSettings.Password]
            };

        private readonly ReportFormat _reportFormat;

        public ReportRunner(
            ReportFormat reportFormat,
            string reportPath)
            : this(reportFormat, reportPath, null, null, null, null)
        {
        }

        public ReportRunner(
            ReportFormat reportFormat,
            string reportPath,
            IDictionary<string, object> reportParameters)
            : this(
                reportFormat, 
                reportPath, 
                reportParameters != null ? reportParameters.ToList() : null)
        {
        }

        public ReportRunner(
            ReportFormat reportFormat,
            string reportPath,
            IEnumerable<KeyValuePair<string, object>> reportParameters)
            : this(reportFormat, reportPath, null, null, null, reportParameters)
        {
        }

        public ReportRunner(
            ReportFormat reportFormat,
            string reportPath,
            string reportServerUrl,
            string username,
            string password,
            IDictionary<string, object> reportParameters)
            : this(
                reportFormat, 
                reportPath, 
                reportServerUrl, 
                username, 
                password, 
                reportParameters != null ? reportParameters.ToList() : null)
        {
        }

        public ReportRunner(
            ReportFormat reportFormat,
            string reportPath,
            string reportServerUrl,
            string username,
            string password,
            IEnumerable<KeyValuePair<string, object>> reportParameters)
        {
            _reportFormat = reportFormat;
            _viewerParameters.ProcessingMode = ProcessingMode.Remote;
            _viewerParameters.ReportPath = reportPath;
            _viewerParameters.ReportServerUrl = reportServerUrl ?? _viewerParameters.ReportServerUrl;
            if (username != null || password != null)
            {
                _viewerParameters.Username = username;
                _viewerParameters.Password = password;
            }

            ParseParameters(reportParameters);
        }

        // The property is only used for unit-testing
        internal ReportViewerParameters ViewerParameters
        {
            get { return _viewerParameters; }
        }

        // The property is only used for unit-testing
        internal ReportFormat ReportFormat
        {
            get { return _reportFormat; }
        }

        public FileStreamResult Run()
        {
            Validate();

            var reportViewer = new ReportViewer();
            reportViewer.Initialize(_viewerParameters);

            string mimeType, fileExt;
            var output = reportViewer.ServerReport.Render(
                ReportFormat.ToString(),
                "<DeviceInfo></DeviceInfo>",
                null,
                out mimeType,
                out fileExt);

            return new FileStreamResult(output, mimeType);
        }

        private void ParseParameters(IEnumerable<KeyValuePair<string, object>> reportParameters)
        {
            if (reportParameters == null)
            {
                return;
            }

            foreach (var reportParameter in reportParameters)
            {
                var parameterName = reportParameter.Key;
                if (_viewerParameters.ReportParameters.ContainsKey(parameterName))
                {
                    _viewerParameters.ReportParameters[parameterName].Values.Add(reportParameter.Value.ToString());
                }
                else
                {
                    var parameter = new ReportParameter(parameterName);
                    parameter.Values.Add(reportParameter.Value.ToString());
                    _viewerParameters.ReportParameters.Add(parameterName, parameter);
                }
            }
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(_viewerParameters.ReportServerUrl))
            {
                throw new MvcReportViewerException("Report Server is not specified.");
            }

            if (string.IsNullOrEmpty(_viewerParameters.ReportPath))
            {
                throw new MvcReportViewerException("Report is not specified.");
            }
        }
    }
}
