using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace MvcReportViewer
{
    public class MvcReportViewerIframe : IHtmlString
    {
        private readonly string _reportPath;

        private readonly string _reportServerUrl;

        private readonly string _username;

        private readonly string _password;

        private readonly IDictionary<string, object> _reportParameters;

        private readonly bool _showParameterPrompts;

        private readonly IDictionary<string, object> _htmlAttributes;

        private readonly string _aspxViewer;

        public MvcReportViewerIframe(
            string reportPath,
            IDictionary<string, object> htmlAttributes)
            : this(reportPath, null, null, null, null, false, htmlAttributes)
        {
        }

        public MvcReportViewerIframe(
            string reportPath,
            IDictionary<string, object> reportParameters,
            IDictionary<string, object> htmlAttributes)
            : this(reportPath, null, null, null, reportParameters, false, htmlAttributes)
        {
        }

        public MvcReportViewerIframe(
            string reportPath,
            string reportServerUrl,
            string username,
            string password,
            IDictionary<string, object> reportParameters,
            bool showParameterPrompts,
            IDictionary<string, object> htmlAttributes)
        {
            _reportPath = reportPath;
            _reportServerUrl = reportServerUrl;
            _username = username;
            _password = password;
            _showParameterPrompts = showParameterPrompts;
            _reportParameters = reportParameters;
            _htmlAttributes = htmlAttributes;
            _aspxViewer = ConfigurationManager.AppSettings[WebConfigSettings.AspxViewer];
            if (string.IsNullOrEmpty(_aspxViewer))
            {
                throw new MvcReportViewerException("ASP.NET Web Forms viewer is not set. Make sure you have MvcReportViewer.AspxViewer in your Web.config.");
            }
        }

        public override string ToString()
        {
            return RenderIframe();
        }

        public string ToHtmlString()
        {
            return ToString();
        }

        private string RenderIframe()
        {
            throw new NotImplementedException();
        }
    }
}
