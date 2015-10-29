using System.Linq;
using Microsoft.Reporting.WebForms;

namespace MvcReportViewer.Example.Models
{
    public class SubreportEventHandlers : IReportViewerEventsHandler
    {
        public void OnSubreportProcessing(ReportViewer reportViewer, SubreportProcessingEventArgs e)
        {
            var countryId = int.Parse(e.Parameters["CountryId"].Values.First());
            var cities = LocalData.GetCitiesByCountryId(countryId);
            e.DataSources.Add(new ReportDataSource("Cities", cities));
        }

        public void OnDrillthrough(ReportViewer reportViewer, DrillthroughEventArgs e)
        {
            var report = (LocalReport)e.Report;
            var parameters = report.GetParameters();
            var countryId = int.Parse(parameters["CountryId"].Values.First());
            var cities = LocalData.GetCitiesByCountryId(countryId);
            report.DataSources.Add(new ReportDataSource("Cities", cities));
            report.Refresh();
        }
    }
}