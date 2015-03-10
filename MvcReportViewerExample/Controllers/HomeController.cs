using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Data;
using MvcReportViewer.Example.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace MvcReportViewer.Example.Controllers
{
    public class HomeController : Controller
    {
        private const string ReportName = "/TestReports/TestReport";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Fluent()
        {
            return View();
        }

        public ActionResult Post()
        {
            return View();
        }

        public ActionResult Multiple()
        {
            return View();
        }

        public ActionResult VisibilityCheck()
        {
            return View();
        }

        public ActionResult DownloadExcel()
        {
            return DownloadReport(ReportFormat.Excel);
        }

        public ActionResult DownloadWord()
        {
            return DownloadReportMultipleValues(ReportFormat.Word);
        }

        public ActionResult DownloadPdf()
        {
            return DownloadReport(ReportFormat.PDF);
        }

        public ActionResult DownloadImage()
        {
            return DownloadReport(ReportFormat.Image);
        }

        private ActionResult DownloadReport(ReportFormat format)
        {
            return this.Report(
                format,
                ReportName,
                new { Parameter1 = "Hello World!", Parameter2 = DateTime.Now, Parameter3 = 12345 });
        }

        private ActionResult DownloadReportMultipleValues(ReportFormat format)
        {
            return this.Report(
                format,
                ReportName,
                new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("Parameter1", "Value 1"),
                    new KeyValuePair<string, object>("Parameter1", "Value 2"),
                    new KeyValuePair<string, object>("Parameter2", DateTime.Now),
                    new KeyValuePair<string, object>("Parameter2", DateTime.Now.AddYears(10)),
                    new KeyValuePair<string, object>("Parameter3", 12345)
                });
        }

        public ActionResult LocalReports()
        {
            var model = new LocalReportsModel
            {
                Products = GetDataTable("select * from dbo.Products"),
                Cities = GetDataTable("select * from dbo.Cities"),
                FilteredCities = GetDataTable("select * from dbo.Cities where Id < 3")
            };

            return View(model);
        }

        private DataTable GetDataTable(string sql)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Products"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
        }
    }
}
