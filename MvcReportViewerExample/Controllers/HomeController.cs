using System;
using System.Web.Mvc;
using System.Collections.Generic;

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
    }
}
