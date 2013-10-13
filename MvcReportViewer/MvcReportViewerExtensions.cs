using System.Web.Mvc;

namespace MvcReportViewer
{
    /// <summary>
    /// HTML helpers for MvcReportViewer.
    /// </summary>
    public static class MvcReportViewerExtensions
    {
        /// <summary>
        /// Returns an HTML <b>iframe</b> rendering ASP.NET ReportViewer control with Remote Processing Mode.
        /// </summary>
        /// <param name="helper">The HTML helper instance that this method extends.</param>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <returns>An HTML <b>iframe</b> element.</returns>
        public static MvcReportViewerIframe MvcReportViewer(
            this HtmlHelper helper,
            string reportPath,
            object htmlAttributes)
        {
            return new MvcReportViewerIframe(
                reportPath, 
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns an HTML <b>iframe</b> rendering ASP.NET ReportViewer control with Remote Processing Mode.
        /// </summary>
        /// <param name="helper">The HTML helper instance that this method extends.</param>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <param name="reportParameters">The report parameter properties for the report.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <returns>An HTML <b>iframe</b> element.</returns>
        public static MvcReportViewerIframe MvcReportViewer(
            this HtmlHelper helper, 
            string reportPath, 
            object reportParameters, 
            object htmlAttributes)
        {
            return new MvcReportViewerIframe(
                reportPath,
                HtmlHelper.AnonymousObjectToHtmlAttributes(reportParameters),
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns an HTML <b>iframe</b> rendering ASP.NET ReportViewer control with Remote Processing Mode.
        /// </summary>
        /// <param name="helper">The HTML helper instance that this method extends.</param>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <param name="reportServerUrl">The URL for the report server.</param>
        /// <param name="username">The report server username.</param>
        /// <param name="password">The report server password.</param>
        /// <param name="reportParameters">The report parameter properties for the report.</param>
        /// <param name="showParameterPrompts">The value that indicates wether parameter prompts are dispalyed.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <returns>An HTML <b>iframe</b> element.</returns>
        public static MvcReportViewerIframe MvcReportViewer(
            this HtmlHelper helper,
            string reportPath,
            string reportServerUrl = null,
            string username = null,
            string password = null,
            object reportParameters = null,
            bool showParameterPrompts = false,
            object htmlAttributes = null)
        {
            return new MvcReportViewerIframe(
                reportPath,
                reportServerUrl,
                username,
                password,
                HtmlHelper.AnonymousObjectToHtmlAttributes(reportParameters),
                showParameterPrompts,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }
    }
}
