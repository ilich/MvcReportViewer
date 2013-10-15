using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcReportViewer.Tests
{
    internal static class TestData
    {
        public static readonly string ReportName = "TestReport";

        public static readonly string Server = "DummyServer";

        public static readonly string Username = "root";

        public static readonly string Password = "secret";

        public static readonly bool ShowParameterPrompts = true;

        public static readonly Dictionary<string, string> ExprectedParameters = new Dictionary<string, string>
            {
                { "Param1", "Test Value 1" },
                { "Param2", "22" },
                { "Param3", "25.5" }
            };

        public static readonly string Style = "height: 100px; width: 100px;";

        public static readonly string CssClass = "dummy-class";

        public static readonly string Id = "dummy-id";

        public static readonly string ViewerUri = "/MvcReportViewer.aspx";

        public static readonly string ReportParametes = string.Join(
            "&amp;", 
            ExprectedParameters.Select(p => HttpUtility.UrlEncode(p.Key) + "=" + HttpUtility.UrlEncode(p.Value)));
    }
}
