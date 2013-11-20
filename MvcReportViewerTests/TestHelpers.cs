using System.Text;

namespace MvcReportViewer.Tests
{
    internal static class TestHelpers
    {
        public static string ValidateReportParameters(ReportViewerParameters parameters)
        {
            var reportParameters = parameters.ReportParameters;
            if (reportParameters.Count != TestData.ExprectedParameters.Count)
            {
                return string.Format(
                    "There are {0} report parameters, but should be {1}.",
                    reportParameters.Count,
                    TestData.ExprectedParameters.Count);
            }

            var errors = new StringBuilder();
            foreach (var expected in TestData.ExprectedParameters)
            {
                var key = expected.Key;
                if (!reportParameters.ContainsKey(key))
                {
                    errors.AppendFormat("{0} is not found. ", key);
                    continue;
                }

                var reportParameter = reportParameters[key].Values[0];
                if (expected.Value != reportParameter)
                {
                    errors.AppendFormat(
                        "{0}: expected {1}, but have {2}. ",
                        key,
                        expected.Value,
                        reportParameters[key]);
                }
            }

            return errors.ToString().Trim();
        }
    }
}
