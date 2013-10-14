using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MvcReportViewer
{
    public class MvcReportViewerIframe : IHtmlString
    {
        private readonly string _reportPath;

        private readonly string _reportServerUrl;

        private readonly string _username;

        private readonly string _password;

        private readonly IDictionary<string, object> _reportParameters;

        private readonly bool? _showParameterPrompts;

        private readonly IDictionary<string, object> _htmlAttributes;

        private readonly string _aspxViewer;

        public MvcReportViewerIframe(
            string reportPath,
            IDictionary<string, object> htmlAttributes)
            : this(reportPath, null, null, null, null, null, htmlAttributes)
        {
        }

        public MvcReportViewerIframe(
            string reportPath,
            IDictionary<string, object> reportParameters,
            IDictionary<string, object> htmlAttributes)
            : this(reportPath, null, null, null, reportParameters, null, htmlAttributes)
        {
        }

        public MvcReportViewerIframe(
            string reportPath,
            string reportServerUrl,
            string username,
            string password,
            IDictionary<string, object> reportParameters,
            bool? showParameterPrompts,
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
            var iframe = new TagBuilder("iframe");
            var uri = PrepareViewerUri();
            iframe.MergeAttribute("src", uri);
            iframe.MergeAttributes(_htmlAttributes);
            return iframe.ToString();
        }

        private string PrepareViewerUri()
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrEmpty(_reportPath))
            {
                query[UriParameters.ReportPath] = _reportPath;
            }

            if (!string.IsNullOrEmpty(_reportServerUrl))
            {
                query[UriParameters.ReportServerUrl] = _reportServerUrl;
            }

            if (!string.IsNullOrEmpty(_username) || !string.IsNullOrEmpty(_password))
            {
                query[UriParameters.Username] = _username;
                query[UriParameters.Password] = _password;
            }

            if (_showParameterPrompts != null)
            {
                query[UriParameters.ShowParameterPrompts] = _showParameterPrompts.ToString();
            }

            if (_reportParameters != null)
            {
                foreach (var parameter in _reportParameters)
                {
                    var value = parameter.Value == null ? string.Empty : parameter.Value.ToString();
                    query[parameter.Key] = value;
                }
            }

            var uri = query.Count == 0 ? 
                _aspxViewer : 
                _aspxViewer + "?" + query.ToString();

            return uri;
        }
    }
}
