using System;
using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.IO;
using System.Web;
using MvcReportViewer.Configuration;
using System.Collections;

namespace MvcReportViewer
{
    internal class ReportRunner
    {
        private readonly ReportViewerConfiguration _config = new ReportViewerConfiguration();

        private readonly ReportViewerParameters _viewerParameters;

        private readonly string _filename;

        public ReportRunner(
            ReportFormat reportFormat,
            string reportPath,
            ProcessingMode mode = ProcessingMode.Remote,
            IDictionary<string, DataTable> localReportDataSources = null,
            string filename = null)
            : this(reportFormat, reportPath, null, null, null, null, mode, localReportDataSources, filename)
        {
        }

        public ReportRunner(
            ReportFormat reportFormat,
            string reportPath,
            IDictionary<string, object> reportParameters,
            ProcessingMode mode = ProcessingMode.Remote,
            IDictionary<string, DataTable> localReportDataSources = null,
            string filename = null)
            : this(
                reportFormat, 
                reportPath, 
                reportParameters?.ToList(),
                mode,
                localReportDataSources,
                filename)
        {
        }

        public ReportRunner(
            ReportFormat reportFormat,
            string reportPath,
            IEnumerable<KeyValuePair<string, object>> reportParameters,
            ProcessingMode mode = ProcessingMode.Remote,
            IDictionary<string, DataTable> localReportDataSources = null,
            string filename = null)
            : this(reportFormat, reportPath, null, null, null, reportParameters, mode, localReportDataSources, filename)
        {
        }

        public ReportRunner(
            ReportFormat reportFormat,
            string reportPath,
            string reportServerUrl,
            string username,
            string password,
            IDictionary<string, object> reportParameters,
            ProcessingMode mode = ProcessingMode.Remote,
            IDictionary<string, DataTable> localReportDataSources = null,
            string filename = null)
            : this(
                reportFormat, 
                reportPath, 
                reportServerUrl, 
                username, 
                password, 
                reportParameters?.ToList(),
                mode,
                localReportDataSources,
                filename)
        {
        }

        public ReportRunner(
            ReportFormat reportFormat,
            string reportPath,
            string reportServerUrl,
            string username,
            string password,
            IEnumerable<KeyValuePair<string, object>> reportParameters,
            ProcessingMode mode = ProcessingMode.Remote,
            IDictionary<string, DataTable> localReportDataSources = null,
            string filename = null)
        {
            _viewerParameters = new ReportViewerParameters
            {
                ReportServerUrl = _config.ReportServerUrl,
                Username = _config.Username,
                Password = _config.Password,
                IsReportRunnerExecution = true
            };

            ReportFormat = reportFormat;
            _filename = filename;

            _viewerParameters.ProcessingMode = mode;
            if (mode == ProcessingMode.Local && localReportDataSources != null)
            {
                _viewerParameters.LocalReportDataSources = localReportDataSources;
            }

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
        internal ReportViewerParameters ViewerParameters => _viewerParameters;

        // The property is only used for unit-testing
        internal ReportFormat ReportFormat { get; }

        public FileStreamResult Run()
        {
            Validate();

            var reportViewer = new ReportViewer();
            reportViewer.Initialize(_viewerParameters);
            ValidateReportFormat(reportViewer);

            string mimeType;
            Stream output;

            if (_viewerParameters.ProcessingMode == ProcessingMode.Remote)
            {
                string extension;
                var format = ReportFormat2String(ReportFormat);

                output = reportViewer.ServerReport.Render(
                    format,
                    "<DeviceInfo></DeviceInfo>",
                    null,
                    out mimeType,
                    out extension);
            }
            else
            {
                var localReport = reportViewer.LocalReport;
                if (_viewerParameters.LocalReportDataSources != null)
                {
                    foreach(var dataSource in _viewerParameters.LocalReportDataSources)
                    {
                        var reportDataSource = new ReportDataSource(dataSource.Key, dataSource.Value);
                        localReport.DataSources.Add(reportDataSource);
                    }
                }

                Warning[] warnings;
                string[] streamids;
                string encoding;
                string extension;

                var format = ReportFormat2String(ReportFormat);

                var report = localReport.Render(
                    format, 
                    null,
                    out mimeType,
                    out encoding,
                    out extension,
                    out streamids,
                    out warnings);

                output = new MemoryStream(report);
            }

            if (!string.IsNullOrEmpty(_filename))
            {
                var response = HttpContext.Current.Response;
                response.ContentType = mimeType;
                response.AddHeader("Content-Disposition", $"attachment; filename={_filename}");
            }

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
                var parameterValue = reportParameter.Value;
                var parameterList = parameterValue as IEnumerable;
                if (parameterList != null && !(parameterValue is string))
                {
                    // I can loop through the values. User is using an array or a list.

                    foreach(var value in parameterList)
                    {
                        if (_viewerParameters.ReportParameters.ContainsKey(parameterName))
                        {
                            _viewerParameters.ReportParameters[parameterName].Values.Add(value.ToString());
                        }
                        else
                        {
                            var parameter = new ReportParameter(parameterName);
                            parameter.Values.Add(value.ToString());
                            _viewerParameters.ReportParameters.Add(parameterName, parameter);
                        }
                    }
                }
                else
                {
                    // Parameter is a literal object. Just add it to the list.

                    if (_viewerParameters.ReportParameters.ContainsKey(parameterName))
                    {
                        _viewerParameters.ReportParameters[parameterName].Values.Add(reportParameter.Value?.ToString());
                    }
                    else
                    {
                        var parameter = new ReportParameter(parameterName);
                        parameter.Values.Add(reportParameter.Value?.ToString());
                        _viewerParameters.ReportParameters.Add(parameterName, parameter);
                    }
                }
                
            }
        }

        private string ReportFormat2String(ReportFormat format)
        {
            return format == ReportFormat.Html ? "HTML4.0" : format.ToString();
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

        private void ValidateReportFormat(ReportViewer reportViewer)
        {
            var format = ReportFormat2String(ReportFormat);
            var formats = _viewerParameters.ProcessingMode == ProcessingMode.Remote
                ? reportViewer.ServerReport.ListRenderingExtensions()
                : reportViewer.LocalReport.ListRenderingExtensions();

            if (formats.All(f => string.Compare(f.Name, format, StringComparison.InvariantCultureIgnoreCase) != 0))
            {
                throw new MvcReportViewerException($"{ReportFormat} is not supported");
            }
        }
    }
}
