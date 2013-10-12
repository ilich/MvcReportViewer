using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace MvcReportViewer
{
    /// <summary>
    /// MvcReportViewer.aspx implementation. The page renders a report.
    /// </summary>
    public class MvcReportViewer : Page
    {
        protected ReportViewer _reportViewer;

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

            _reportViewer.ProcessingMode = ProcessingMode.Remote;
            _reportViewer.ShowParameterPrompts = parameters.ShowParameterPrompts;

            var serverReport = _reportViewer.ServerReport;
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
