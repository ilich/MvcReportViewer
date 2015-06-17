using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Reporting.WebForms;

namespace MvcReportViewer
{
    /// <summary>
    /// HTML iframe rengering engine for MvcReportViewer HTML extension.
    /// </summary>
    public class MvcReportViewerIframe : IMvcReportViewerOptions
    {
        internal static readonly string VisibilitySeparator = "~~~";

        private const string JsPostForm = @"
var formElement{0} = document.getElementById('{0}');
if (formElement{0}) {{
    formElement{0}.submit();
}}
";
        private readonly ControlSettingsManager _settingsManager = new ControlSettingsManager();

        private string _reportPath;

        private string _reportServerUrl;

        private string _username;

        private string _password;

        private FormMethod _method;

        private ProcessingMode _processingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;

        private IList<KeyValuePair<string, object>> _reportParameters;

        private IDictionary<string, object> _htmlAttributes;

        private ControlSettings _controlSettings;

        private readonly string _aspxViewer;

        private readonly bool _encryptParameters;

        /// <summary>
        /// Creates an instance of MvcReportViewerIframe class.
        /// </summary>
        /// <param name="reportPath">The path to the report on the server.</param>
        public MvcReportViewerIframe(string reportPath)
            : this(reportPath, null, null, null, null, null, null, FormMethod.Get)
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
            : this(reportPath, null, null, null, null, null, htmlAttributes, FormMethod.Get)
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
            : this(reportPath, null, null, null, reportParameters, null, htmlAttributes, FormMethod.Get)
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
            IEnumerable<KeyValuePair<string, object>> reportParameters,
            IDictionary<string, object> htmlAttributes)
            : this(reportPath, null, null, null, reportParameters, null, htmlAttributes, FormMethod.Get)
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
        /// <param name="controlSettings">The Report Viewer control's UI settings.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <param name="method">Method for sending parameters to the iframe, either GET or POST.</param>
        public MvcReportViewerIframe(
            string reportPath,
            string reportServerUrl,
            string username,
            string password,
            IDictionary<string, object> reportParameters,
            ControlSettings controlSettings,
            IDictionary<string, object> htmlAttributes,
            FormMethod method)
            : this(
                   reportPath, 
                   reportServerUrl, 
                   username, 
                   password, 
                   reportParameters != null ? reportParameters.ToList() : null, 
                   controlSettings, 
                   htmlAttributes, 
                   method)
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
        /// <param name="controlSettings">The Report Viewer control's UI settings.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <param name="method">Method for sending parameters to the iframe, either GET or POST.</param>
        public MvcReportViewerIframe(
            string reportPath,
            string reportServerUrl,
            string username,
            string password,
            IEnumerable<KeyValuePair<string, object>> reportParameters,
            ControlSettings controlSettings,
            IDictionary<string, object> htmlAttributes,
            FormMethod method)
        {
            var javaScriptApi = ConfigurationManager.AppSettings[WebConfigSettings.JavaScriptApi];
            if (string.IsNullOrEmpty(javaScriptApi))
            {
                throw new MvcReportViewerException("MvcReportViewer.js location is not found. Make sure you have MvcReportViewer.AspxViewerJavaScript in your Web.config.");
            }

            _reportPath = reportPath;
            _reportServerUrl = reportServerUrl;
            _username = username;
            _password = password;
            _controlSettings = controlSettings;
            _reportParameters = reportParameters != null ? reportParameters.ToList() : null;
            _htmlAttributes = htmlAttributes;
            _method = method;
            _aspxViewer = ConfigurationManager.AppSettings[WebConfigSettings.AspxViewer];
            if (string.IsNullOrEmpty(_aspxViewer))
            {
                throw new MvcReportViewerException("ASP.NET Web Forms viewer is not set. Make sure you have MvcReportViewer.AspxViewer in your Web.config.");
            }

            _aspxViewer = _aspxViewer.Trim();
            if (_aspxViewer.StartsWith("~"))
            {
                _aspxViewer = VirtualPathUtility.ToAbsolute(_aspxViewer);
            }

            var encryptParametesConfig = ConfigurationManager.AppSettings[WebConfigSettings.EncryptParameters];
            if (!bool.TryParse(encryptParametesConfig, out _encryptParameters))
            {
                _encryptParameters = false;
            }

            ControlId = Guid.NewGuid();
        }

        internal Guid ControlId
        {
            get;
            set;
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
            switch (_method)
            {
                case FormMethod.Get:
                    return GetIframeUsingGetMethod();
                    
                case FormMethod.Post:
                    return GetIframeUsingPostMethod();
            }
            
            throw new InvalidOperationException();
        }

        private string GetIframeUsingPostMethod()
        {
            var iframeId = GenerateId();
            var formId = GenerateId();

            // <form method="POST" action="/MvcReportViewer.aspx">...</form>
            var form = new TagBuilder("form");
            form.MergeAttribute("method", "POST");
            form.MergeAttribute("action", _aspxViewer);
            form.MergeAttribute("target", iframeId);
            form.GenerateId(formId);
            form.InnerHtml = BuildIframeFormFields();

            // <iframe />
            var iframe = new TagBuilder("iframe");
            iframe.MergeAttributes(_htmlAttributes);
            iframe.MergeAttribute("name", iframeId);
            iframe.GenerateId(iframeId);
            
            // <script>...</script>
            var script = new StringBuilder("<script>");
            script.AppendFormat(JsPostForm, formId);
            script.Append("</script>");

            var html = new StringBuilder();
            html.Append(form);
            html.Append(iframe);
            html.Append(script);
         
            return html.ToString();
        }

        private string GenerateId()
        {
            return "mvc_report_viewer_" + Guid.NewGuid().ToString("N");
        }

        private string BuildIframeFormFields()
        {
            var html = new StringBuilder();

            html.Append(CreateHiddenField(UriParameters.ControlId, ControlId));
            html.Append(CreateHiddenField(UriParameters.ProcessingMode, _processingMode));

            if (!string.IsNullOrEmpty(_reportPath))
            {
                html.Append(CreateHiddenField(UriParameters.ReportPath, _reportPath));
            }

            if (!string.IsNullOrEmpty(_reportServerUrl))
            {
                html.Append(CreateHiddenField(UriParameters.ReportServerUrl, _reportServerUrl));
            }

            if (!string.IsNullOrEmpty(_username) || !string.IsNullOrEmpty(_password))
            {
                html.Append(CreateHiddenField(UriParameters.Username, _username));
                html.Append(CreateHiddenField(UriParameters.Password, _password));
            }

            var serializedSettings = _settingsManager.Serialize(_controlSettings);
            foreach (var setting in serializedSettings)
            {
                html.Append(CreateHiddenField(setting.Key, setting.Value));
            }
        
            if (_reportParameters != null)
            {
                foreach (var parameter in _reportParameters)
                {
                    if (parameter.Value == null)
                    {
                        continue;
                    }

                    var value = ConvertValueToString(parameter.Value);
                    html.Append(CreateHiddenField(parameter.Key, value));
                }
            }

            return html.ToString();
        }

        private string CreateHiddenField<T>(string name, T value)
        {
            var tag = new TagBuilder("input");
            tag.MergeAttribute("type", "hidden");
            tag.MergeAttribute("name", name);

            var strValue = value.ToString();
            if (_encryptParameters)
            {
                strValue = SecurityUtil.Encrypt(strValue);
            }

            tag.MergeAttribute("value", strValue);

            return tag.ToString();
        }

        private string GetIframeUsingGetMethod()
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
            query[UriParameters.ControlId] = ControlId.ToString();
            query[UriParameters.ProcessingMode] = _processingMode.ToString();
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

            var serializedSettings = _settingsManager.Serialize(_controlSettings);
            foreach (var setting in serializedSettings)
            {
                query[setting.Key] = setting.Value;
            }

            if (_reportParameters != null)
            {
                foreach (var parameter in _reportParameters)
                {
                    if (parameter.Value == null)
                    {
                        continue;
                    }

                    var value = ConvertValueToString(parameter.Value);
                    query.Add(parameter.Key, value);
                }
            }

            string uri = _aspxViewer;
            if (query.Count == 0)
            {
                return uri;
            }

            if (!_encryptParameters)
            {
                return uri + "?" + query;
            }

            var encryptedQuery = UriParameters.Encrypted + "=" + SecurityUtil.Encrypt(query.ToString());
            return uri + "?" + encryptedQuery;
        }

        private string ConvertValueToString(object value)
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}", value);
        }

        /// <summary>
        /// Sets the path to the report on the server.
        /// </summary>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions ReportPath(string reportPath)
        {
            _reportPath = reportPath;
            return this;
        }

        /// <summary>
        /// Sets the URL for the report server.
        /// </summary>
        /// <param name="reportServerUrl">The URL for the report server.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions ReportServerUrl(string reportServerUrl)
        {
            _reportServerUrl = reportServerUrl;
            return this;
        }

        /// <summary>
        /// Sets the report server username.
        /// </summary>
        /// <param name="username">The report server username.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions Username(string username)
        {
            _username = username;
            return this;
        }

        /// <summary>
        /// Sets the report server password.
        /// </summary>
        /// <param name="password">The report server password.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions Password(string password)
        {
            _password = password;
            return this;
        }

        /// <summary>
        /// Sets the report parameter properties for the report.
        /// </summary>
        /// <param name="reportParameters">The report parameter properties for the report.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions ReportParameters(object reportParameters)
        {
            _reportParameters = reportParameters == null
                                    ? null
                                    : HtmlHelper.AnonymousObjectToHtmlAttributes(reportParameters).ToList();
            return this;
        }

        /// <summary>
        /// Sets the report parameter properties for the report.
        /// </summary>
        /// <param name="reportParameters">The report parameter properties for the report.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions ReportParameters(IEnumerable<KeyValuePair<string, object>> reportParameters)
        {
            if (reportParameters == null)
            {
                return this;
            }

            _reportParameters = reportParameters.ToList();
            return this;
        }

        /// <summary>
        ///  Sets the report parameter properties for the report.
        /// </summary>
        /// <param name="reportParameters">The report parameter properties for the report.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions ReportParameters(IEnumerable<ReportParameter> reportParameters)
        {
            if (reportParameters == null)
            {
                return this;
            }

            _reportParameters = reportParameters.SelectMany(
                p => p.Values
                      .Cast<object>()
                      .Select(pv => new KeyValuePair<string, object>(
                          string.Format("{0}{1}{2}", p.Name, VisibilitySeparator, p.Visible),
                          pv))).ToList();

            return this;
        }

        /// <summary>
        /// Sets an object that contains the HTML attributes to set for the element.
        /// </summary>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions Attributes(object htmlAttributes)
        {
            var attributes = htmlAttributes == null ?
                null :
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            _htmlAttributes = attributes;
            return this;
        }

        /// <summary>
        /// Sets the method for sending parametes to the iframe, either GET or POST.
        /// POST should be used to send long arguments, etc. Use GET otherwise.
        /// </summary>
        /// <param name="method">The HTTP method for sending parametes to the iframe, either GET or POST.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions Method(FormMethod method)
        {
            _method = method;
            return this;
        }

        /// <summary>
        /// Sets ReportViewer control UI parameters
        /// </summary>
        /// <param name="settings"></param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions ControlSettings(ControlSettings settings)
        {
            _controlSettings = settings;
            return this;
        }

        /// <summary>
        /// Sets ReportViewer report processing mode.
        /// </summary>
        /// <param name="mode">Processing Mode (Local or Remote).</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions ProcessingMode(ProcessingMode mode)
        {
            _processingMode = mode;
            return this;
        }

        /// <summary>
        /// Registers local data source.
        /// </summary>
        /// <param name="dataSourceName">Report data source name.</param>
        /// <param name="dataTable">The data.</param>
        /// <returns></returns>
        public IMvcReportViewerOptions LocalDataSource(string dataSourceName, DataTable dataTable)
        {
            var provider = LocalReportDataSourceProviderFactory.Current.Create();
            var dataSource = new ReportDataSource(dataSourceName, dataTable);
            provider.Add(ControlId, dataSource);

            return this;
        }

        /// <summary>
        /// Registers custom local data source, e.g. SQL query
        /// </summary>
        /// <param name="dataSourceName">Report data source name.</param>
        /// <param name="dataSource">The data.</param>
        /// <returns></returns>
        public IMvcReportViewerOptions LocalDataSource<T>(string dataSourceName, T dataSource)
        {
            var provider = LocalReportDataSourceProviderFactory.Current.Create();
            provider.Add(ControlId, dataSourceName, dataSource);

            return this;
        }
    }
}
