using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;

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
        /// <param name="mode">Report processing mode: remote or local.</param>
        /// <param name="localReportDataSources">Local report data sources</param>
        /// <param name="filename">Output filename</param>
        /// <returns>The file-content result object.</returns>
        public static FileStreamResult Report(
            this Controller controller, 
            ReportFormat reportFormat, 
            string reportPath,
            ProcessingMode mode = ProcessingMode.Remote,
            IDictionary<string, DataTable> localReportDataSources = null,
            string filename = null)
        {
            var reportRunner = new ReportRunner(reportFormat, reportPath, mode, localReportDataSources, filename);
            return reportRunner.Run();
        }

        /// <summary>
        /// Creates a FileContentResult object by using Report Viewer Web Control.
        /// </summary>
        /// <param name="controller">The Controller instance that this method extends.</param>
        /// <param name="reportFormat">Report Viewer Web Control supported format (Excel, Word, PDF or Image)</param>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <param name="reportParameters">The report parameter properties for the report.</param>
        /// <param name="mode">Report processing mode: remote or local.</param>
        /// <param name="localReportDataSources">Local report data sources</param>
        /// <param name="filename">Output filename</param>
        /// <returns>The file-content result object.</returns>
        public static FileStreamResult Report(
            this Controller controller,
            ReportFormat reportFormat,
            string reportPath,
            object reportParameters,
            ProcessingMode mode = ProcessingMode.Remote,
            IDictionary<string, DataTable> localReportDataSources = null,
            string filename = null)
        {
            var reportRunner = new ReportRunner(
                reportFormat, 
                reportPath, 
                HtmlHelper.AnonymousObjectToHtmlAttributes(reportParameters),
                mode,
                localReportDataSources,
                filename);

            return reportRunner.Run();
        }

        /// <summary>
        /// Creates a FileContentResult object by using Report Viewer Web Control.
        /// </summary>
        /// <param name="controller">The Controller instance that this method extends.</param>
        /// <param name="reportFormat">Report Viewer Web Control supported format (Excel, Word, PDF or Image)</param>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <param name="reportParameters">The report parameter properties for the report.</param>
        /// <param name="mode">Report processing mode: remote or local.</param>
        /// <param name="localReportDataSources">Local report data sources</param>
        /// <param name="filename">Output filename</param>
        /// <returns>The file-content result object.</returns>
        public static FileStreamResult Report(
            this Controller controller,
            ReportFormat reportFormat,
            string reportPath,
            IEnumerable<KeyValuePair<string, object>> reportParameters,
            ProcessingMode mode = ProcessingMode.Remote,
            IDictionary<string, DataTable> localReportDataSources = null,
            string filename = null)
        {
            var reportRunner = new ReportRunner(
                reportFormat,
                reportPath,
                reportParameters,
                mode,
                localReportDataSources,
                filename);

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
        /// <param name="mode">Report processing mode: remote or local.</param>
        /// <param name="localReportDataSources">Local report data sources</param>
        /// <param name="filename">Output filename</param>
        /// <returns>The file-content result object.</returns>
        public static FileStreamResult Report(
            this Controller controller,
            ReportFormat reportFormat,
            string reportPath,
            string reportServerUrl,
            string username = null,
            string password = null,
            object reportParameters = null,
            ProcessingMode mode = ProcessingMode.Remote,
            IDictionary<string, DataTable> localReportDataSources = null,
            string filename = null)
        {
            var reportRunner = new ReportRunner(
                reportFormat,
                reportPath,
                reportServerUrl,
                username,
                password,
                HtmlHelper.AnonymousObjectToHtmlAttributes(reportParameters),
                mode,
                localReportDataSources,
                filename);

            return reportRunner.Run();
        }

        /// <summary>
        /// Creates a FileContentResult object by using Report Viewer Web Control.
        /// </summary>
        /// <param name="controller">The Controller instance that this method extends.</param>
        /// <param name="reportFormat">Report Viewer Web Control supported format (Excel, Word, PDF or Image)</param>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <param name="reportServerUrl">The URL for the report server.</param>
        /// <param name="reportParameters">The report parameter properties for the report.</param>
        /// <param name="username">The report server username.</param>
        /// <param name="password">The report server password.</param>
        /// <param name="mode">Report processing mode: remote or local.</param>
        /// <param name="localReportDataSources">Local report data sources</param>
        /// <param name="filename">Output filename</param>
        /// <returns>The file-content result object.</returns>
        public static FileStreamResult Report(
            this Controller controller,
            ReportFormat reportFormat,
            string reportPath,
            string reportServerUrl,
            IEnumerable<KeyValuePair<string, object>> reportParameters,
            string username = null,
            string password = null,
            ProcessingMode mode = ProcessingMode.Remote,
            IDictionary<string, DataTable> localReportDataSources = null,
            string filename = null)
        {
            var reportRunner = new ReportRunner(
                reportFormat,
                reportPath,
                reportServerUrl,
                username,
                password,
                reportParameters,
                mode,
                localReportDataSources,
                filename);

            return reportRunner.Run();
        }
    }
}
