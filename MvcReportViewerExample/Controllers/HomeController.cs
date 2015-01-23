using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Remoting.Messaging;
using MvcReportViewer.Example.localhost;


namespace MvcReportViewer.Example.Controllers
{
    public class HomeController : Controller
    {
        private readonly string ReportName = ConfigurationManager.AppSettings[WebConfigSettings.TestReportName];
        private readonly string ReportServiceUrl = ConfigurationManager.AppSettings[WebConfigSettings.ReportServiceUrl];
        private readonly string SsrsReportRoot = ConfigurationManager.AppSettings[WebConfigSettings.SsrsReportRoot];

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReportList()
        {
            BuildReportHierarchyModel();
            return View("ReportList", "_ReportLayout");
        }

        private void BuildReportHierarchyModel()
        {
            ReportingService2010 rs = new ReportingService2010();

            rs.Credentials = System.Net.CredentialCache.DefaultCredentials;
            rs.Url = ReportServiceUrl;

            ReportHierarchy reportHierarchy = new ReportHierarchy();
            
            IDictionary<string, string> catalogList = new Dictionary<string, string>();
            LoadSsrsCatalog(rs, ReportHierarchy.CatalogEntries, SsrsReportRoot, 0);

        }

        private void LoadSsrsCatalog(ReportingService2010 rs,  IList<CatalogEntry> catalogList , string dir, int parentId)
        {
            try
            {
                // SSRS needs path without ending / to search, we need it for separation
                string searchDir = dir;

                if (dir.Length > 1 && dir.EndsWith("/"))
                    searchDir = dir.Substring(0, dir.Length - 1);

                // Return a list of catalog items in the report server database
                CatalogItem[] items = rs.ListChildren(searchDir, false);

                // For each report, display the path of the report in a Listbox
                foreach (CatalogItem ci in items)
                {
                    CatalogEntry entry = new CatalogEntry();
                    
                    if (ci.TypeName == "Folder")
                    {
                        if (parentId == 0 && (ci.Name == "Data Sources" || ci.Name == "Datasets"))
                            continue;

                        entry.Id = catalogList.Count + 1;
                        entry.ParentId = parentId;
                        entry.ItemName = ci.Name;
                        entry.IsReport = false;
                        entry.FullPath = (dir + ci.Name);
                        entry.FullPath = entry.FullPath.Substring(1, entry.FullPath.Length - 1); // Need to omit leading / for id
                        catalogList.Add(entry);
                        
                        LoadSsrsCatalog(rs, catalogList, dir + ci.Name + "/", entry.Id);
                    }
                    else
                    {
                        entry.Id = catalogList.Count + 1;
                        entry.ParentId = parentId;
                        entry.ItemName = ci.Name;
                        entry.IsReport = true;
                        entry.FullPath = (dir + ci.Name);
                        entry.FullPath = entry.FullPath.Substring(1, entry.FullPath.Length - 1); // Need to omit leading / for id

                        catalogList.Add(entry);
                    }
                }
            }

            catch (Exception e)
            {
                Response.Write(e.Message);
            }
        }

        public MvcReportViewerIframe GetMvcReportViewerQueryString()
        {
            MvcReportViewerIframe result = MvcReportViewerExtensions.MvcReportViewer(null, Request.QueryString[0]);

            return result;
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
