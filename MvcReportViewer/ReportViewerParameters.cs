using Microsoft.Reporting.WebForms;
using System.Collections.Generic;

namespace MvcReportViewer
{
    internal class ReportViewerParameters
    {
        public ReportViewerParameters()
        {
            ReportParameters = new Dictionary<string, ReportParameter>();
        }

        public string ReportServerUrl { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ReportPath { get; set; }

        public bool ShowParameterPrompts { get; set; }

        public Dictionary<string, ReportParameter> ReportParameters { get; set; }
    }
}
