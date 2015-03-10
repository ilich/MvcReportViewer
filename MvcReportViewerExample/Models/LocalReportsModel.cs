using System.Data;

namespace MvcReportViewer.Example.Models
{
    public class LocalReportsModel
    {
        public DataTable Products { get; set; }

        public DataTable Cities { get; set; }

        public DataTable FilteredCities { get; set; }
    }
}