using System.Web.Mvc;

namespace MvcReportViewer
{
    public static class ReportRunnerExtensions
    {
        /// <summary>
        /// Creates a FileContentResult object by using Report Viewer Web Control.
        /// </summary>
        /// <param name="controller">The Controller instance that this method extends.</param>
        /// <param name="reportFormat">Report Viewer Web Control supported format (Excel, Word, PDF or Image)</param>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <returns>The file-content result object.</returns>
        public static FileStreamResult Report(
            this Controller controller, 
            ReportFormat reportFormat, 
            string reportPath)
        {
            var reportRunner = new ReportRunner(reportFormat, reportPath);
            return reportRunner.Run();
        }

        /// <summary>
        /// Creates a FileContentResult object by using Report Viewer Web Control.
        /// </summary>
        /// <param name="controller">The Controller instance that this method extends.</param>
        /// <param name="reportFormat">Report Viewer Web Control supported format (Excel, Word, PDF or Image)</param>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <param name="reportParameters">The report parameter properties for the report.</param>
        /// <returns>The file-content result object.</returns>
        public static FileStreamResult Report(
            this Controller controller,
            ReportFormat reportFormat,
            string reportPath,
            object reportParameters)
        {
            var reportRunner = new ReportRunner(
                reportFormat, 
                reportPath, 
                HtmlHelper.AnonymousObjectToHtmlAttributes(reportParameters));

            return reportRunner.Run();
        }

        /// <summary>
        /// Creates a FileContentResult object by using Report Viewer Web Control.
        /// </summary>
        /// <param name="controller">The Controller instance that this method extends.</param>
        /// <param name="reportFormat">Report Viewer Web Control supported format (Excel, Word, PDF or Image)</param>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <param name="reportServerUrl">The URL for the report server.</param>
        /// <param name="username">The report server username.</param>
        /// <param name="password">The report server password.</param>
        /// <param name="reportParameters">The report parameter properties for the report.</param>
        /// <returns>The file-content result object.</returns>
        public static FileStreamResult Report(
            this Controller controller,
            ReportFormat reportFormat,
            string reportPath,
            string reportServerUrl,
            string username = null,
            string password = null,
            object reportParameters = null)
        {
            var reportRunner = new ReportRunner(
                reportFormat,
                reportPath,
                reportServerUrl,
                username,
                password,
                HtmlHelper.AnonymousObjectToHtmlAttributes(reportParameters));

            return reportRunner.Run();
        }
    }
}
