using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MvcReportViewer
{
    public interface IProvideReportConfiguration
    {
        Guid ControlId { get; set; }

        ControlSettings ControlSettings { get; set; }

        IEnumerable<KeyValuePair<string, object>> DataSources { get; set; }

        FormMethod FormMethod { get; set; }

        object HtmlAttributes { get; set; }

        string Password { get; set; }

        object ReportParameters { get; set; }

        string ReportPath { get; set; }

        string ReportServerUrl { get; set; }

        string Username { get; set; }
    }
}