using System.Collections.Generic;
using System.Web.Mvc;

namespace MvcReportViewer
{
    internal static class ParameterHelpers
    {
        public static IEnumerable<KeyValuePair<string, object>> GetReportParameters(object reportParameters)
        {
            return reportParameters is IEnumerable<KeyValuePair<string, object>>
                       ? (IEnumerable<KeyValuePair<string, object>>)reportParameters
                       : HtmlHelper.AnonymousObjectToHtmlAttributes(reportParameters);
        }
    }
}