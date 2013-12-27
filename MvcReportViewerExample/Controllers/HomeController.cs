using System;
using System.Web.Mvc;
using MvcReportViewer;

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

        public ActionResult DownloadExcel()
        {
            return DownloadReport(ReportFormat.Excel);
        }

        public ActionResult DownloadWord()
        {
            return DownloadReport(ReportFormat.Word);
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
    }
}
