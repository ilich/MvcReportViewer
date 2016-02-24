using System;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;

namespace MvcReportViewer
{
    internal static class ReportViewerExtensions
    {
        public static void Initialize(this ReportViewer reportViewer, ReportViewerParameters parameters)
        {
            var handlers = CreateEventHandlers(parameters);

            if (parameters.ProcessingMode == ProcessingMode.Remote)
            {
                SetupRemoteProcessing(reportViewer, parameters);
            }
            else
            {
                SetupLocalProcessing(reportViewer, parameters, handlers);
            }

            if (handlers != null)
            {
                reportViewer.Drillthrough += (sender, e) => handlers.OnDrillthrough(reportViewer, e);
            }

            SetReportViewerSettings(reportViewer, parameters.ControlSettings);
        }

        public static void SetupDrillthrough(this ReportViewer reportViewer, string eventsHandlerType)
        {
            var handlers = CreateEventHandlers(eventsHandlerType);
            if (handlers != null)
            {
                reportViewer.Drillthrough += (sender, e) => handlers.OnDrillthrough(reportViewer, e);
            }
        }

        private static void SetupLocalProcessing(ReportViewer reportViewer, ReportViewerParameters parameters, IReportViewerEventsHandler handlers)
        {
            reportViewer.ProcessingMode = ProcessingMode.Local;
            var localReport = reportViewer.LocalReport;
            localReport.ReportPath = parameters.ReportPath;

            if (parameters.ControlSettings?.UseCurrentAppDomainPermissionSet != null &&
                parameters.ControlSettings.UseCurrentAppDomainPermissionSet.Value)
            {
                localReport.SetBasePermissionsForSandboxAppDomain(AppDomain.CurrentDomain.PermissionSet.Copy());
            }

            if (parameters.ControlSettings?.EnableExternalImages != null &&
                parameters.ControlSettings.EnableExternalImages.Value)
            {
                localReport.EnableExternalImages = true;
            }

            if (parameters.ControlSettings?.EnableHyperlinks != null &&
                parameters.ControlSettings.EnableHyperlinks.Value)
            {
                localReport.EnableHyperlinks = true;
            }

            if (parameters.ReportParameters.Count > 0)
            {
                localReport.SetParameters(parameters.ReportParameters.Values);
            }

            if (handlers != null)
            {
                localReport.SubreportProcessing += (sender, e) => handlers.OnSubreportProcessing(reportViewer, e);
            }

            // If parameters.LocalReportDataSources then we should get report data source
            // from local data source provider (ignore it for Report Runner)
            if (parameters.LocalReportDataSources == null && !parameters.IsReportRunnerExecution)
            {
                if (parameters.ControlId == null)
                {
                    throw new MvcReportViewerException("MvcReportViewer control ID is missing");
                }

                var dataSourceProvider = LocalReportDataSourceProviderFactory.Current.Create();
                var dataSources = dataSourceProvider.Get((Guid)parameters.ControlId);
                localReport.DataSources.Clear();
                foreach (var dataSource in dataSources)
                {
                    localReport.DataSources.Add(dataSource);
                }
            }
            
        }

        private static void SetupRemoteProcessing(ReportViewer reportViewer, ReportViewerParameters parameters)
        {
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            var serverReport = reportViewer.ServerReport;
            serverReport.ReportServerUrl = new Uri(parameters.ReportServerUrl);
            serverReport.ReportPath = parameters.ReportPath;
            if (!string.IsNullOrEmpty(parameters.Username))
            {
                if (parameters.IsAzureSsrs)
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

            if (parameters.DataSourceCredentials?.Length > 0)
            {
                serverReport.SetDataSourceCredentials(parameters.DataSourceCredentials);
            }

            if (parameters.ReportParameters.Count > 0)
            {
                serverReport.SetParameters(parameters.ReportParameters.Values);
            }
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

            reportViewer.Width = parameters.Width ?? new Unit("100%");

            // Use AsyncRendering = false and hard-coded Height to fix problem with Google Chrome report rendering
            // http://stackoverflow.com/questions/778688/reportviewer-control-height-issue

            if (parameters.Height != null)
            {
                reportViewer.Height = parameters.Height.Value;
            }
            else if (parameters.FrameHeight != null)
            {
                reportViewer.Height = parameters.FrameHeight.Value;
            }
            else
            {
                reportViewer.Height = new Unit("100%");
            }

            reportViewer.AsyncRendering = parameters.AsyncRendering ?? false;

            reportViewer.KeepSessionAlive = parameters.KeepSessionAlive ?? false;
        }

        private static IReportViewerEventsHandler CreateEventHandlers(ReportViewerParameters parameters)
        {
            return CreateEventHandlers(parameters.EventsHandlerType);
        }

        private static IReportViewerEventsHandler CreateEventHandlers(string eventsHandlerType)
        {
            if (string.IsNullOrEmpty(eventsHandlerType))
            {
                return null;
            }

            var handlersType = Type.GetType(eventsHandlerType);
            if (handlersType == null)
            {
                throw new MvcReportViewerException($"Type {eventsHandlerType} is not found");
            }

            var handlers = Activator.CreateInstance(handlersType) as IReportViewerEventsHandler;
            if (handlers == null)
            {
                throw new MvcReportViewerException(
                    $"Type {eventsHandlerType} has not implemented IReportViewerEventsHandler interface or cannot be instantiated.");
            }

            return handlers;
        }
    }
}
