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
            var parameters = Request.Form.Count > 0 ? parser.Parse(Request.Form) : parser.Parse(Request.QueryString);
            ReportViewer.Initialize(parameters);
        }
    }
}
