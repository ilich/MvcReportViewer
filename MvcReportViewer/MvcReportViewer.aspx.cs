using Microsoft.Reporting.WebForms;
using System;
using System.Web.UI;

namespace MvcReportViewer
{
    /// <summary>
    /// MvcReportViewer.aspx implementation. The page renders a report.
    /// </summary>
    public class MvcReportViewer : Page
    {
        protected ReportViewer ReportViewer;

        protected void Page_Load(object sender, EventArgs e)
        {
            ShopReport();
        }

        private void ShopReport()
        {
            if (IsPostBack)
            {
                return;
            }

            var parser = new ReportViewerParametersParser();
            var parameters = parser.Parse(Request.QueryString);

            ReportViewer.ProcessingMode = ProcessingMode.Remote;
            ReportViewer.ShowParameterPrompts = parameters.ShowParameterPrompts;

            var serverReport = ReportViewer.ServerReport;
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
