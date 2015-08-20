using System.Collections.Generic;
using System.Data;

namespace MvcReportViewer.Example.Models
{
    public class LocalReportsModel
    {
        public DataTable Products { get; set; }

        public IEnumerable<CityModel> Cities { get; set; }

        public DataTable FilteredCities { get; set; }
    }

    public class CityModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}