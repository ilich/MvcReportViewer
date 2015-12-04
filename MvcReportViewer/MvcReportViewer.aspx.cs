using Microsoft.Reporting.WebForms;
using System;
using System.Web;
using System.Web.UI;
using MvcReportViewer.Configuration;

namespace MvcReportViewer
{
    /// <summary>
    /// MvcReportViewer.aspx implementation. The page renders a report.
    /// </summary>
    public class MvcReportViewer : Page
    {
        private const string IsHeightChangedJs = "<script type='text/javascript'>window.hasUserSetHeight = {0};</script>";
        private const string ShowPrintButtonJs = "<script type='text/javascript'>window.showPrintButton = {0};</script>";

        private const string EventHandlers = "MvcReportViewer.EventHandlers";

        private readonly ReportViewerConfiguration _config = new ReportViewerConfiguration();

        protected ReportViewer ReportViewer;

        protected ScriptManager ScriptManager;

        protected void Page_Load(object sender, EventArgs e)
        {
            ShowReport();
        }

        private void ShowReport()
        {
            try
            {
                if (!IsPostBack)
                {
                    var parser = new ReportViewerParametersParser();
                    var parameters = Request.Form.Count > 0 ? parser.Parse(Request.Form) : parser.Parse(Request.QueryString);

                    var hasHeightChangedScript = string.Format(
                        IsHeightChangedJs,
                        parameters.ControlSettings.Height == null ? "false" : "true");
                    ClientScript.RegisterStartupScript(GetType(), "IsHeightChangedJS", hasHeightChangedScript);

                    var showPrintButtonScript = string.Format(ShowPrintButtonJs,
                        parameters.ControlSettings.ShowPrintButton == false ? "false" : "true");
                    ClientScript.RegisterStartupScript(GetType(), "ShowPrintButtonJs", showPrintButtonScript);

                    ReportViewer.ReportError += OnReportError;
                    ReportViewer.Initialize(parameters);

                    RegisterJavaScriptApi(parameters);

                    // Save Drillthrough information for future use
                    if (!string.IsNullOrEmpty(parameters.EventsHandlerType))
                    {
                        ViewState[EventHandlers] = parameters.EventsHandlerType;
                    }
                }
                else
                {
                    // Drillthrough event has to be configured no matter what.
                    var eventHandlers = ViewState[EventHandlers] as string;
                    if (!string.IsNullOrEmpty(eventHandlers))
                    {
                        ReportViewer.SetupDrillthrough(eventHandlers);
                    }
                }
            }
            catch (Exception e)
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

        private void RegisterJavaScriptApi(ReportViewerParameters parameters)
        {
            if (parameters.ControlSettings.AsyncPostBackTimeout != null)
            {
                ScriptManager.AsyncPostBackTimeout = (int)parameters.ControlSettings.AsyncPostBackTimeout;
            }

            var javaScriptApi = _config.AspxViewerJavaScript;
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

            var errorPage = _config.ErrorPage;
            if (!_config.ShowErrorPage || string.IsNullOrEmpty(errorPage))
            {
                return false;
            }

            var errorUrl = $"{errorPage}?error={Server.UrlEncode(exception.Message)}&errorType={Server.UrlEncode(exception.GetType().FullName)}";

            Response.Redirect(errorUrl);
            return true;
        }
    }
}
