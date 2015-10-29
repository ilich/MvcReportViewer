using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MvcReportViewer.Example.Models
{
    static class LocalData
    {
        public static IEnumerable<CityModel> GetPocoCities()
        {
            return new List<CityModel>
            {
                new CityModel {Id = 1, Name = "New York"},
                new CityModel {Id = 2, Name = "London"},
                new CityModel {Id = 3, Name = "Paris"},
                new CityModel {Id = 4, Name = "Prague"},
                new CityModel {Id = 5, Name = "Amsterdam"},
            };
        }

        public static DataTable GetProducts()
        {
            return GetDataTable("select * from dbo.Products");
        }

        public static DataTable GetCities()
        {
            return GetDataTable("select * from dbo.Cities");
        }

        public static DataTable GetCitiesByCountryId(int countryId)
        {
            var parameter = new SqlParameter("countryId", SqlDbType.Int)
            {
                Value = countryId
            };

            return GetDataTable("select * from dbo.Cities where CountryId = @countryId", parameter);
        }

        public static DataTable GetCountries()
        {
            return GetDataTable("select * from dbo.Countries");
        }

        public static DataTable GetDataTable(string sql, params SqlParameter[] parameters)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Products"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.AddRange(parameters);    

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
        }
    }
}