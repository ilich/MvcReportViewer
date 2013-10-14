using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MvcReportViewer
{
    /// <summary>
    /// HTML iframe rengering engine for MvcReportViewer HTML extension.
    /// </summary>
    public class MvcReportViewerIframe : IMvcReportViewerOptions, IHtmlString
    {
        private readonly string _reportPath;

        private readonly string _reportServerUrl;

        private readonly string _username;

        private readonly string _password;

        private readonly IDictionary<string, object> _reportParameters;

        private readonly bool? _showParameterPrompts;

        private readonly IDictionary<string, object> _htmlAttributes;

        private readonly string _aspxViewer;

        /// <summary>
        /// Creates an instance of MvcReportViewerIframe class.
        /// </summary>
        /// <param name="reportPath">The path to the report on the server.</param>
        public MvcReportViewerIframe(string reportPath)
            : this(reportPath, null, null, null, null, null, null)
        {
        }

        /// <summary>
        /// Creates an instance of MvcReportViewerIframe class.
        /// </summary>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public MvcReportViewerIframe(
            string reportPath,
            IDictionary<string, object> htmlAttributes)
            : this(reportPath, null, null, null, null, null, htmlAttributes)
        {
        }

        /// <summary>
        /// Creates an instance of MvcReportViewerIframe class.
        /// </summary>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <param name="reportParameters">The report parameter properties for the report.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public MvcReportViewerIframe(
            string reportPath,
            IDictionary<string, object> reportParameters,
            IDictionary<string, object> htmlAttributes)
            : this(reportPath, null, null, null, reportParameters, null, htmlAttributes)
        {
        }

        /// <summary>
        /// Creates an instance of MvcReportViewerIframe class.
        /// </summary>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <param name="reportServerUrl">The URL for the report server.</param>
        /// <param name="username">The report server username.</param>
        /// <param name="password">The report server password.</param>
        /// <param name="reportParameters">The report parameter properties for the report.</param>
        /// <param name="showParameterPrompts">The value that indicates wether parameter prompts are dispalyed.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
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

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return RenderIframe();
        }

        /// <summary>
        /// Returns an HTML-encoded string.
        /// </summary>
        /// <returns>An HTML-encoded string.</returns>
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

        /// <summary>
        /// Sets the path to the report on the server.
        /// </summary>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions ReportPath(string reportPath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the URL for the report server.
        /// </summary>
        /// <param name="reportServerUrl">The URL for the report server.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions ReportServerUrl(string reportServerUrl)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the report server username.
        /// </summary>
        /// <param name="username">The report server username.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions Username(string username)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the report server password.
        /// </summary>
        /// <param name="password">The report server password.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions Password(string password)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the report parameter properties for the report.
        /// </summary>
        /// <param name="reportParameters">The report parameter properties for the report.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions ReportParameters(object reportParameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the value that indicates wether parameter prompts are dispalyed.
        /// </summary>
        /// <param name="showParameterPrompts">The value that indicates wether parameter prompts are dispalyed.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions ShowParameterPrompts(bool showParameterPrompts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets an object that contains the HTML attributes to set for the element.
        /// </summary>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions Attributes(object htmlAttributes)
        {
            throw new NotImplementedException();
        }
    }
}
