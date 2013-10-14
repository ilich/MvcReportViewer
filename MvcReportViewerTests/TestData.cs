using System.Collections.Generic;

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
    }
}
