using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;

namespace MvcReportViewer.Tests
{
    internal static class TestData
    {
        public static readonly string ReportName = "TestReport";

        public static readonly string DefaultServer = "http://localhost/ReportServer_SQLEXPRESS";

        public static readonly string DefaultUsername = "admin";

        public static readonly string DefaultPassword = "password";

        public static readonly string Server = "DummyServer";

        public static readonly string Username = "root";

        public static readonly string Password = "secret";

        #region ReportViewer control UI settings

        public const string BackColor = "_1";

        public static readonly Color BackColorValue = Color.Green;

        public static readonly int BackColorArgbValue = Color.Green.ToArgb();

        public const string ShowParameterPrompts = "_18";

        public const bool ShowParameterPromptsValue = true;

        public const string ToolBarItemBorderStyle = "_29";

        public const BorderStyle ToolBarItemBorderStyleValue = BorderStyle.Dashed;

        public const string ToolBarItemBorderWidth = "_30";

        public static readonly Unit ToolBarItemBorderWidthValue = new Unit(100);

        public const string ZoomMode = "_32";

        public static readonly ZoomMode ZoomModeValue = Microsoft.Reporting.WebForms.ZoomMode.PageWidth;

        public const string ZoomPercent = "_33";

        public const int ZoomPercentValue = 40;

        public const string HyperlinkTarget = "_4";

        public const string HyperlinkTargetValue = "http://google.com";

        #endregion

        public static readonly Dictionary<string, string> ExprectedParameters = new Dictionary<string, string>
            {
                { "Param1", "Test Value 1" },
                { "Param2", "22" },
                { "Param3", "255" }
            };

        public static readonly Dictionary<string, object> ActualParameters = new Dictionary<string, object>
            {
                { "Param1", "Test Value 1" },
                { "Param2", 22 },
                { "Param3", 255 }
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
