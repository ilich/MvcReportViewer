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

            try
            {
                var parser = new ReportViewerParametersParser();
                var parameters = Request.Form.Count > 0 ? parser.Parse(Request.Form) : parser.Parse(Request.QueryString);

                var hasHeightChangedScript = string.Format(
                    IsHeightChangedJS,
                    parameters.ControlSettings.Height == null ? "false" : "true");
                ClientScript.RegisterStartupScript(GetType(), "IsHeightChangedJS", hasHeightChangedScript);

                ReportViewer.ReportError += OnReportError;
                ReportViewer.Initialize(parameters);

                RegisterJavaScriptApi();
            }
            catch(Exception e)
            {
                var result = RedirectToErrorPage(e);
                if (!result)
                {
                    throw;
                }
            }
        }

        private void OnReportError(object sender, ReportErrorEventArgs e)
        {
            RedirectToErrorPage(e.Exception);
            e.Handled = true;
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



        private bool RedirectToErrorPage(Exception exception)
        {
            Trace.Warn("MvcReportViewer", exception.Message);

            var errorPage = ConfigurationManager.AppSettings[WebConfigSettings.ErrorPage];
            var showErrorPage = ConfigurationManager.AppSettings[WebConfigSettings.ShowErrorPage];
            bool isErrorPageShown;

            if (!bool.TryParse(showErrorPage, out isErrorPageShown))
            {
                return false;
            }

            if (!isErrorPageShown || string.IsNullOrEmpty(errorPage))
            {
                return false;
            }

            Response.Redirect(errorPage);
            return true;
        }
    }
}
