using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using MvcReportViewer.Configuration;
using Newtonsoft.Json;

namespace MvcReportViewer
{
    /// <summary>
    /// HTML iframe rengering engine for MvcReportViewer HTML extension.
    /// </summary>
    public class MvcReportViewerIframe : IMvcReportViewerOptions
    {
        internal static Func<string, string> ApplyAppPathModifier { get; set; }

        private ILocalReportDataSourceProvider DataSourceProvider => LocalReportDataSourceProviderFactory.Current.Create();

        internal static readonly string VisibilitySeparator = "~~~";

        private const string JsPostForm = @"
var formElement{0} = document.getElementById('{0}');
if (formElement{0}) {{
    formElement{0}.submit();
}}
";
        private readonly ControlSettingsManager _settingsManager = new ControlSettingsManager();

        private readonly ReportViewerConfiguration _config = new ReportViewerConfiguration();

        private string _reportPath;

        private string _reportServerUrl;

        private string _username;

        private string _password;

        private string _eventsHandlerType;

        private FormMethod _method;

        private ProcessingMode _processingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;

        private IList<KeyValuePair<string, object>> _reportParameters;

        private IDictionary<string, object> _htmlAttributes;

        private ControlSettings _controlSettings;

        private DataSourceCredentials[] _dataSourceCredentials;

        private readonly string _aspxViewer;

        private readonly bool _encryptParameters;

        static MvcReportViewerIframe()
        {
            var response = HttpContext.Current?.Response;
            if (response != null)
            {
                ApplyAppPathModifier = response.ApplyAppPathModifier;
            }
        }

        /// <summary>
        /// Creates an instance of the MvcReportViewerIframe class.
        /// </summary>
        /// <param name="configuration">The report configuration provider used to configure the new MvcReportViewerIframe.</param>
        public MvcReportViewerIframe(IProvideReportConfiguration configuration)
        {
            var controlId = configuration.ControlId;
            var controlSettings = configuration.ControlSettings;
            var dataSources = configuration.DataSources;
            var method = configuration.FormMethod;
            var htmlAttributes = ParameterHelpers.GetReportParameters(configuration.HtmlAttributes).ToDictionary(pair => pair.Key, pair => pair.Value);
            var password = configuration.Password;
            var reportParameters = ParameterHelpers.GetReportParameters(configuration.ReportParameters);
            var reportPath = configuration.ReportPath;
            var reportServerUrl = configuration.ReportServerUrl;
            var username = configuration.Username;

            if (string.IsNullOrEmpty(_config.AspxViewerJavaScript))
            {
                throw new MvcReportViewerException("MvcReportViewer.js location is not found. Make sure you have MvcReportViewer.AspxViewerJavaScript in your Web.config.");
            }

            _aspxViewer = GetAspxViewer();
            ControlId = controlId;
            _controlSettings = controlSettings;

            SetDataSources(dataSources);

            _encryptParameters = _config.EncryptParameters;
            _htmlAttributes = htmlAttributes;
            _method = method;
            _password = password;
            _reportParameters = reportParameters?.ToList();
            _reportPath = reportPath;
            _reportServerUrl = reportServerUrl;
            _username = username;
        }

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
                   reportParameters?.ToList(),
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
            if (String.IsNullOrEmpty(this._config.AspxViewerJavaScript))
            {
                throw new MvcReportViewerException("MvcReportViewer.js location is not found. Make sure you have MvcReportViewer.AspxViewerJavaScript in your Web.config.");
            }

            this._reportPath = reportPath;
            this._reportServerUrl = reportServerUrl;
            this._username = username;
            this._password = password;
            this._controlSettings = controlSettings;
            this._reportParameters = reportParameters?.ToList();
            this._htmlAttributes = htmlAttributes;
            this._method = method;



            this._aspxViewer = GetAspxViewer();

            this._encryptParameters = this._config.EncryptParameters;
            this.ControlId = Guid.NewGuid();
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
            return this.RenderIframe();
        }

        /// <summary>
        /// Returns an HTML-encoded string.
        /// </summary>
        /// <returns>An HTML-encoded string.</returns>
        public string ToHtmlString()
        {
            return this.ToString();
        }

        private string RenderIframe()
        {
            switch (this._method)
            {
                case FormMethod.Get:
                    return this.GetIframeUsingGetMethod();

                case FormMethod.Post:
                    return this.GetIframeUsingPostMethod();
            }

            throw new InvalidOperationException();
        }

        private string GetIframeUsingPostMethod()
        {
            var iframeId = this.GenerateId();
            var formId = this.GenerateId();

            // <form method="POST" action="/MvcReportViewer.aspx">...</form>
            var form = new TagBuilder("form");
            form.MergeAttribute("method", "POST");
            form.MergeAttribute("action", this._aspxViewer);
            form.MergeAttribute("target", iframeId);
            form.GenerateId(formId);
            form.InnerHtml = this.BuildIframeFormFields();

            // <iframe />
            var iframe = new TagBuilder("iframe");
            iframe.MergeAttributes(this._htmlAttributes);
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

            html.Append(this.CreateHiddenField(UriParameters.ControlId, this.ControlId));
            html.Append(this.CreateHiddenField(UriParameters.ProcessingMode, this._processingMode));

            if (!String.IsNullOrEmpty(this._reportPath))
            {
                html.Append(this.CreateHiddenField(UriParameters.ReportPath, this._reportPath));
            }

            if (!String.IsNullOrEmpty(this._reportServerUrl))
            {
                html.Append(this.CreateHiddenField(UriParameters.ReportServerUrl, this._reportServerUrl));
            }

            if (!String.IsNullOrEmpty(this._username) || !String.IsNullOrEmpty(this._password))
            {
                html.Append(this.CreateHiddenField(UriParameters.Username, this._username));
                html.Append(this.CreateHiddenField(UriParameters.Password, this._password));
            }

            if (!String.IsNullOrEmpty(this._eventsHandlerType))
            {
                html.Append(this.CreateHiddenField(UriParameters.EventsHandlerType, this._eventsHandlerType));
            }

            if (this._dataSourceCredentials?.Length > 0)
            {
                html.Append(this.CreateHiddenField(UriParameters.DataSourceCredentials, JsonConvert.SerializeObject(this._dataSourceCredentials)));
            }

            var frameHeight = this.GetFrameHeight();
            if (frameHeight != null)
            {
                if (this._controlSettings == null)
                {
                    this._controlSettings = new ControlSettings();
                }

                this._controlSettings.FrameHeight = new Unit(frameHeight.Item1, frameHeight.Item2);
            }

            var serializedSettings = this._settingsManager.Serialize(this._controlSettings);
            foreach (var setting in serializedSettings)
            {
                html.Append(this.CreateHiddenField(setting.Key, setting.Value));
            }

            if (this._reportParameters != null)
            {
                foreach (var parameter in this._reportParameters)
                {
                    if (parameter.Value == null)
                    {
                        continue;
                    }

                    var multiple = parameter.Value as IEnumerable;
                    if (parameter.Value is string || multiple == null)
                    {
                        var value = this.ConvertValueToString(parameter.Value);
                        html.Append(this.CreateHiddenField(parameter.Key, value));
                    }
                    else
                    {
                        foreach (var v in multiple)
                        {
                            var value = this.ConvertValueToString(v);
                            html.Append(this.CreateHiddenField(parameter.Key, value));
                        }
                    }
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
            if (this._encryptParameters)
            {
                strValue = SecurityUtil.Encrypt(strValue);
            }

            tag.MergeAttribute("value", strValue);

            return tag.ToString();
        }

        private string GetIframeUsingGetMethod()
        {
            var iframe = new TagBuilder("iframe");
            var uri = this.PrepareViewerUri();
            iframe.MergeAttribute("src", uri);
            iframe.MergeAttributes(this._htmlAttributes);
            return iframe.ToString();
        }

        private string PrepareViewerUri()
        {
            var query = HttpUtility.ParseQueryString(String.Empty);
            query[UriParameters.ControlId] = this.ControlId.ToString();
            query[UriParameters.ProcessingMode] = this._processingMode.ToString();
            if (!String.IsNullOrEmpty(this._reportPath))
            {
                query[UriParameters.ReportPath] = this._reportPath;
            }

            if (!String.IsNullOrEmpty(this._reportServerUrl))
            {
                query[UriParameters.ReportServerUrl] = this._reportServerUrl;
            }

            if (!String.IsNullOrEmpty(this._username) || !String.IsNullOrEmpty(this._password))
            {
                query[UriParameters.Username] = this._username;
                query[UriParameters.Password] = this._password;
            }

            if (!String.IsNullOrEmpty(this._eventsHandlerType))
            {
                query[UriParameters.EventsHandlerType] = this._eventsHandlerType;
            }

            if (this._dataSourceCredentials?.Length > 0)
            {
                query[UriParameters.DataSourceCredentials] = JsonConvert.SerializeObject(this._dataSourceCredentials);
            }

            var frameHeight = this.GetFrameHeight();
            if (frameHeight != null)
            {
                if (this._controlSettings == null)
                {
                    this._controlSettings = new ControlSettings();
                }

                this._controlSettings.FrameHeight = new Unit(frameHeight.Item1, frameHeight.Item2);
            }

            var serializedSettings = this._settingsManager.Serialize(this._controlSettings);
            foreach (var setting in serializedSettings)
            {
                query[setting.Key] = setting.Value;
            }

            if (this._reportParameters != null)
            {
                foreach (var parameter in this._reportParameters)
                {
                    if (parameter.Value == null)
                    {
                        continue;
                    }

                    var multiple = parameter.Value as IEnumerable;
                    if (parameter.Value is string || multiple == null)
                    {
                        var value = this.ConvertValueToString(parameter.Value);
                        query.Add(parameter.Key, value);
                    }
                    else
                    {
                        foreach (var v in multiple)
                        {
                            var value = this.ConvertValueToString(v);
                            query.Add(parameter.Key, value);
                        }
                    }
                }
            }

            string uri = this._aspxViewer;
            if (query.Count == 0)
            {
                return uri;
            }

            if (!this._encryptParameters)
            {
                return uri + "?" + query;
            }

            var encryptedQuery = UriParameters.Encrypted + "=" + SecurityUtil.Encrypt(query.ToString());
            return uri + "?" + encryptedQuery;
        }

        private Tuple<int, UnitType> GetFrameHeight()
        {
            if (this._htmlAttributes == null)
            {
                return null;
            }

            var iframeHeight = String.Empty;
            foreach (var key in this._htmlAttributes.Keys)
            {
                if (String.Compare(key, "height", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    iframeHeight = key;
                    break;
                }
            }

            if (String.IsNullOrEmpty(iframeHeight))
            {
                return null;
            }

            var raw = this._htmlAttributes[iframeHeight].ToString().Trim();
            if (!Regex.IsMatch(raw, @"^[\d]+[%|px]*$"))
            {
                return null;
            }

            var heightMatch = Regex.Match(raw, @"^[\d]+");
            if (!heightMatch.Success)
            {
                return null;
            }

            int value;
            if (!Int32.TryParse(heightMatch.Value, out value))
            {
                return null;
            }

            var unitType = raw.EndsWith("%") ? UnitType.Percentage : UnitType.Pixel;
            value -= unitType == UnitType.Pixel ? 20 : 1;
            return new Tuple<int, UnitType>(value, unitType);
        }

        private string ConvertValueToString(object value)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}", value);
        }

        /// <summary>
        /// Sets the path to the report on the server.
        /// </summary>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions ReportPath(string reportPath)
        {
            this._reportPath = reportPath;
            return this;
        }

        /// <summary>
        /// Sets the URL for the report server.
        /// </summary>
        /// <param name="reportServerUrl">The URL for the report server.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions ReportServerUrl(string reportServerUrl)
        {
            this._reportServerUrl = reportServerUrl;
            return this;
        }

        /// <summary>
        /// Sets the report server username.
        /// </summary>
        /// <param name="username">The report server username.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions Username(string username)
        {
            this._username = username;
            return this;
        }

        /// <summary>
        /// Sets the report server password.
        /// </summary>
        /// <param name="password">The report server password.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions Password(string password)
        {
            this._password = password;
            return this;
        }

        /// <summary>
        /// Sets the report parameter properties for the report.
        /// </summary>
        /// <param name="reportParameters">The report parameter properties for the report.</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions ReportParameters(object reportParameters)
        {
            this._reportParameters = reportParameters == null
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

            this._reportParameters = reportParameters.ToList();
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

            this._reportParameters = reportParameters.SelectMany(
                p => p.Values
                      .Cast<object>()
                      .Select(pv => new KeyValuePair<string, object>(
                          $"{p.Name}{VisibilitySeparator}{p.Visible}",
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
            this._htmlAttributes = attributes;
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
            this._method = method;
            return this;
        }

        /// <summary>
        /// Sets ReportViewer control UI parameters
        /// </summary>
        /// <param name="settings"></param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions ControlSettings(ControlSettings settings)
        {
            this._controlSettings = settings;
            return this;
        }

        /// <summary>
        /// Sets ReportViewer report processing mode.
        /// </summary>
        /// <param name="mode">Processing Mode (Local or Remote).</param>
        /// <returns>An instance of MvcViewerOptions class.</returns>
        public IMvcReportViewerOptions ProcessingMode(ProcessingMode mode)
        {
            this._processingMode = mode;
            return this;
        }

        /// <summary>
        /// Registers custom local data source, e.g. SQL query
        /// </summary>
        /// <param name="dataSourceName">Report data source name.</param>
        /// <param name="dataSource">The data.</param>
        /// <returns></returns>
        public IMvcReportViewerOptions LocalDataSource(string dataSourceName, object dataSource)
        {
            DataSourceProvider.Add(ControlId, dataSourceName, dataSource);

            return this;
        }

        /// <summary>
        /// Sets the type implementing IReportViewerEventsHandler interface. The instance of the type is responsible for
        /// processing Report Viewer Web Control's events, e.g. SubreportProcessing.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IMvcReportViewerOptions EventsHandlerType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.GetInterfaces().All(i => i != typeof(IReportViewerEventsHandler)))
            {
                throw new MvcReportViewerException($"{type.FullName} must implement IReportViewerEventsHandler interface.");
            }

            this._eventsHandlerType = type.AssemblyQualifiedName;
            return this;
        }

        /// <summary>
        /// Sets data source credentials for the report. 
        /// </summary>
        /// <param name="credentials">An array of Credentials objects.</param>
        /// <returns></returns>
        public IMvcReportViewerOptions SetDataSourceCredentials(DataSourceCredentials[] credentials)
        {
            this._dataSourceCredentials = credentials;
            return this;
        }

        private string GetAspxViewer()
        {
            if (string.IsNullOrEmpty(_config.AspxViewer))
            {
                throw new MvcReportViewerException("ASP.NET Web Forms viewer is not set. Make sure you have MvcReportViewer.AspxViewer in your Web.config.");
            }

            var aspxViewer = _config.AspxViewer.Trim();
            if (aspxViewer.StartsWith("~"))
            {
                aspxViewer = VirtualPathUtility.ToAbsolute(aspxViewer);
            }

            aspxViewer = ApplyAppPathModifier(aspxViewer);


            return aspxViewer;
        }

        private void SetDataSources(IEnumerable<KeyValuePair<string, object>> dataSources)
        {
            if (dataSources == null)
            {
                return;
            }

            foreach (var reportDataSource in dataSources)
            {
                DataSourceProvider.Add(ControlId, reportDataSource.Key, reportDataSource.Value);
            }
        }
    }
}
