using System;
using System.Text;
using NUnit.Framework;

namespace MvcReportViewer.Tests
{
    internal static class TestHelpers
    {
        public static void PropertiesAreNull(object obj, params string[] except)
        {
            Assert.IsNotNull(obj);

            var type = obj.GetType();
            foreach (var property in type.GetProperties())
            {
                var value = property.GetValue(obj, null);
                if (except.Length == 0)
                {
                    if (value != null)
                    {
                        Assert.Fail("Property {0} is not null", property.Name);    
                    }
                }
                else
                {
                    var shouldBeNull = Array.IndexOf(except, property.Name) == -1;
                    if (!shouldBeNull && value == null)
                    {
                        Assert.Fail("Property {0} is not null", property.Name);    
                    }
                    else if (shouldBeNull && value != null)
                    {
                        Assert.Fail("Property {0} is null", property.Name);    
                    }
                }
            }
        }

        public static string ValidateReportParameters(ReportViewerParameters parameters)
        {
            var reportParameters = parameters.ReportParameters;
            if (reportParameters.Count != TestData.ExprectedParameters.Count)
            {
                return $"There are {reportParameters.Count} report parameters, but should be {TestData.ExprectedParameters.Count}.";
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
