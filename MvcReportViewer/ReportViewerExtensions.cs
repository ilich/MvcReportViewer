using System;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;

namespace MvcReportViewer
{
    internal static class ReportViewerExtensions
    {
        public static void Initialize(this ReportViewer reportViewer, ReportViewerParameters parameters)
        {
            reportViewer.ProcessingMode = ProcessingMode.Remote;

            var serverReport = reportViewer.ServerReport;
            serverReport.ReportServerUrl = new Uri(parameters.ReportServerUrl);
            serverReport.ReportPath = parameters.ReportPath;
            if (!string.IsNullOrEmpty(parameters.Username))
            {
                if (parameters.IsAzureSSRS)
                {
                    var server = serverReport.ReportServerUrl.Host;
                    serverReport.ReportServerCredentials = new AzureReportServerCredentials(
                        parameters.Username,
                        parameters.Password,
                        server);
                }
                else
                {
                    serverReport.ReportServerCredentials = new ReportServerCredentials(
                        parameters.Username,
                        parameters.Password);
                }
            }

            if (parameters.ReportParameters.Count > 0)
            {
                serverReport.SetParameters(parameters.ReportParameters.Values);
            }

            SetReportViewerSettings(reportViewer, parameters.ControlSettings);
        }

        private static void SetReportViewerSettings(ReportViewer reportViewer, ControlSettings parameters)
        {
            // Hide parameters prompt by default
            if (parameters == null)
            {
                reportViewer.ShowParameterPrompts = false;
                return;
            }

            if (parameters.BackColor != null)
            {
                reportViewer.BackColor = parameters.BackColor.Value;
            }

            if (parameters.DocumentMapCollapsed != null)
            {
                reportViewer.DocumentMapCollapsed = parameters.DocumentMapCollapsed.Value;
            }

            if (parameters.DocumentMapWidth != null)
            {
                reportViewer.DocumentMapWidth = parameters.DocumentMapWidth.Value;
            }

            if (parameters.HyperlinkTarget != null)
            {
                reportViewer.HyperlinkTarget = parameters.HyperlinkTarget;
            }

            if (parameters.InternalBorderColor != null)
            {
                reportViewer.InternalBorderColor = parameters.InternalBorderColor.Value;
            }

            if (parameters.InternalBorderStyle != null)
            {
                reportViewer.InternalBorderStyle = parameters.InternalBorderStyle.Value;
            }

            if (parameters.InternalBorderWidth != null)
            {
                reportViewer.InternalBorderWidth = parameters.InternalBorderWidth.Value;
            }

            if (parameters.LinkActiveColor != null)
            {
                reportViewer.LinkActiveColor = parameters.LinkActiveColor.Value;
            }

            if (parameters.LinkActiveHoverColor != null)
            {
                reportViewer.LinkActiveHoverColor = parameters.LinkActiveHoverColor.Value;
            }

            if (parameters.LinkDisabledColor != null)
            {
                reportViewer.LinkDisabledColor = parameters.LinkDisabledColor.Value;
            }

            if (parameters.PromptAreaCollapsed != null)
            {
                reportViewer.PromptAreaCollapsed = parameters.PromptAreaCollapsed.Value;
            }

            if (parameters.ShowBackButton != null)
            {
                reportViewer.ShowBackButton = parameters.ShowBackButton.Value;
            }

            if (parameters.ShowCredentialPrompts != null)
            {
                reportViewer.ShowCredentialPrompts = parameters.ShowCredentialPrompts.Value;
            }

            if (parameters.ShowDocumentMapButton != null)
            {
                reportViewer.ShowDocumentMapButton = parameters.ShowDocumentMapButton.Value;
            }

            if (parameters.ShowExportControls != null)
            {
                reportViewer.ShowExportControls = parameters.ShowExportControls.Value;
            }

            if (parameters.ShowFindControls != null)
            {
                reportViewer.ShowFindControls = parameters.ShowFindControls.Value;
            }

            if (parameters.ShowPageNavigationControls != null)
            {
                reportViewer.ShowPageNavigationControls = parameters.ShowPageNavigationControls.Value;
            }

            if (parameters.ShowParameterPrompts != null)
            {
                reportViewer.ShowParameterPrompts = parameters.ShowParameterPrompts.Value;
            }

            if (parameters.ShowPrintButton != null)
            {
                reportViewer.ShowPrintButton = parameters.ShowPrintButton.Value;
            }

            if (parameters.ShowPromptAreaButton != null)
            {
                reportViewer.ShowPromptAreaButton = parameters.ShowPromptAreaButton.Value;
            }

            if (parameters.ShowRefreshButton != null)
            {
                reportViewer.ShowRefreshButton = parameters.ShowRefreshButton.Value;
            }

            if (parameters.ShowReportBody != null)
            {
                reportViewer.ShowReportBody = parameters.ShowReportBody.Value;
            }

            if (parameters.ShowToolBar != null)
            {
                reportViewer.ShowToolBar = parameters.ShowToolBar.Value;
            }

            if (parameters.ShowWaitControlCancelLink != null)
            {
                reportViewer.ShowWaitControlCancelLink = parameters.ShowWaitControlCancelLink.Value;
            }

            if (parameters.ShowZoomControl != null)
            {
                reportViewer.ShowZoomControl = parameters.ShowZoomControl.Value;
            }

            if (parameters.SizeToReportContent != null)
            {
                reportViewer.SizeToReportContent = parameters.SizeToReportContent.Value;
            }

            if (parameters.SplitterBackColor != null)
            {
                reportViewer.SplitterBackColor = parameters.SplitterBackColor.Value;
            }

            if (parameters.ToolBarItemBorderColor != null)
            {
                reportViewer.ToolBarItemBorderColor = parameters.ToolBarItemBorderColor.Value;
            }

            if (parameters.ToolBarItemBorderStyle != null)
            {
                reportViewer.ToolBarItemBorderStyle = parameters.ToolBarItemBorderStyle.Value;
            }

            if (parameters.ToolBarItemBorderWidth != null)
            {
                reportViewer.ToolBarItemBorderWidth = parameters.ToolBarItemBorderWidth.Value;
            }

            if (parameters.ToolBarItemHoverBackColor != null)
            {
                reportViewer.ToolBarItemHoverBackColor = parameters.ToolBarItemHoverBackColor.Value;
            }

            if (parameters.ZoomMode != null)
            {
                reportViewer.ZoomMode = parameters.ZoomMode.Value;
            }

            if (parameters.ZoomPercent != null)
            {
                reportViewer.ZoomPercent = parameters.ZoomPercent.Value;
            }

            if (parameters.AsyncRendering != null)
            {
                reportViewer.AsyncRendering = (bool)parameters.AsyncRendering;
            }

            reportViewer.Width = parameters.Width ?? new Unit("100%");
            reportViewer.Height = parameters.Height ?? new Unit("100%");
            reportViewer.KeepSessionAlive = parameters.KeepSessionAlive ?? false;
        }
    }
}
