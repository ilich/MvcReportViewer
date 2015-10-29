using Microsoft.Reporting.WebForms;

namespace MvcReportViewer
{
    public interface IReportViewerEventsHandler
    {
        void OnSubreportProcessing(ReportViewer reportViewer, SubreportProcessingEventArgs e);

        void OnDrillthrough(ReportViewer reportViewer, DrillthroughEventArgs e);
    }
}
