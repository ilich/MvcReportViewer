using System;
using Microsoft.Reporting.WebForms;

namespace MvcReportViewer
{
    internal static class ReportViewerExtensions
    {
        public static void Initialize(this ReportViewer reportViewer, ReportViewerParameters parameters)
        {
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.ShowParameterPrompts = parameters.ShowParameterPrompts;

            var serverReport = reportViewer.ServerReport;
            serverReport.ReportServerUrl = new Uri(parameters.ReportServerUrl);
            serverReport.ReportPath = parameters.ReportPath;
            if (!string.IsNullOrEmpty(parameters.Username))
            {
                serverReport.ReportServerCredentials = new ReportServerCredentials(
                    parameters.Username,
                    parameters.Password);
            }

            if (parameters.ReportParameters.Count > 0)
            {
                serverReport.SetParameters(parameters.ReportParameters.Values);
            }
        }
    }
}
