using System.Linq;
using Microsoft.Reporting.WebForms;

namespace MvcReportViewer.Example.Models
{
    public class SubreportEventHandlers : IReportViewerEventsHandler
    {
        public void OnSubreportProcessing(ReportViewer reportViewer, SubreportProcessingEventArgs e)
        {
            var countryId = int.Parse(e.Parameters.First().Values.First());
            var cities = LocalData.GetCitiesByCountryId(countryId);
            e.DataSources.Add(new ReportDataSource("Cities", cities));
        }
    }
}