using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Web;
using System.Web.UI;

namespace MvcReportViewer
{
    /// <summary>
    /// MvcReportViewer.aspx implementation. The page renders a report.
    /// </summary>
    public class MvcReportViewer : Page
    {
        private const string IsHeightChangedJS = "<script type='text/javascript'>window.hasUserSetHeight = {0};</script>";

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
            
            var hasHeightChangedScript = string.Format(
                IsHeightChangedJS,
                parameters.ControlSettings.Height == null ? "false" : "true");
            ClientScript.RegisterStartupScript(GetType(), "IsHeightChangedJS", hasHeightChangedScript);

            ReportViewer.Initialize(parameters);

            RegisterJavaScriptApi();
        }

        private void RegisterJavaScriptApi()
        {
            var javaScriptApi = ConfigurationManager.AppSettings[WebConfigSettings.JavaScriptApi];
            if (string.IsNullOrEmpty(javaScriptApi))
            {
                throw new MvcReportViewerException("MvcReportViewer.js location is not found. Make sure you have MvcReportViewer.AspxViewerJavaScript in your Web.config.");
            }

            if (javaScriptApi.StartsWith("~"))
            {
                javaScriptApi = VirtualPathUtility.ToAbsolute(javaScriptApi);
            }

            ClientScript.RegisterClientScriptInclude("JavaScriptAPI", javaScriptApi);
        }
    }
}
