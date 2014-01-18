using NUnit.Framework;
using System;
using System.Collections.Specialized;

namespace MvcReportViewer.Tests
{
    [TestFixture]
    public class ReportViewerParametersParserTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Parse_NullQueryString_RaisedArgumentNullException()
        {
            var parser = new ReportViewerParametersParser();
            parser.Parse(null);
        }

        [Test]
        [ExpectedException(typeof(MvcReportViewerException))]
        public void Parse_ParseWithoutQueryString_RaiseMvcReportViewerException()
        {
            var parser = new ReportViewerParametersParser();
            parser.Parse(new NameValueCollection());
        }

        [Test]
        [ExpectedException(typeof(MvcReportViewerException))]
        public void Parse_EmptyServerInQueryString_RaiseMvcReportViewerException()
        {
            var parser = new ReportViewerParametersParser();
            var queryString = GetQueryString();
            queryString.Add(UriParameters.ReportServerUrl, string.Empty);
            parser.Parse(queryString);
        }

        [Test]
        public void Parse_ParseWithoutQueryString_DefaultValuesAndReport()
        {
            var parser = new ReportViewerParametersParser();
            var queryString = GetQueryString();
            var parameters = parser.Parse(queryString);
            Assert.AreEqual(TestData.ReportName, parameters.ReportPath);
            Assert.AreEqual(TestData.DefaultServer, parameters.ReportServerUrl);
            Assert.AreEqual(TestData.DefaultUsername, parameters.Username);
            Assert.AreEqual(TestData.DefaultPassword, parameters.Password);
            TestHelpers.PropertiesAreNull(parameters.ControlSettings);
            Assert.AreEqual(0, parameters.ReportParameters.Count);
        }

        [Test]
        public void Parse_HasServer_ServerAndRestFromAppConfig()
        {
            var parser = new ReportViewerParametersParser();
            var queryString = GetQueryString();
            queryString.Add(UriParameters.ReportServerUrl, TestData.Server);
            var parameters = parser.Parse(queryString);
            Assert.AreEqual(TestData.ReportName, parameters.ReportPath);
            Assert.AreEqual(TestData.Server, parameters.ReportServerUrl);
            Assert.AreEqual(TestData.DefaultUsername, parameters.Username);
            Assert.AreEqual(TestData.DefaultPassword, parameters.Password);
            TestHelpers.PropertiesAreNull(parameters.ControlSettings);
            Assert.AreEqual(0, parameters.ReportParameters.Count);
        }

        [Test]
        public void Parse_HasServerUsername_ServerUsernameEmptyPasswordAndRestFromAppConfig()
        {
            var parser = new ReportViewerParametersParser();
            var queryString = GetQueryString();
            queryString.Add(UriParameters.ReportServerUrl, TestData.Server);
            queryString.Add(UriParameters.Username, TestData.Username);
            var parameters = parser.Parse(queryString);
            Assert.AreEqual(TestData.ReportName, parameters.ReportPath);
            Assert.AreEqual(TestData.Server, parameters.ReportServerUrl);
            Assert.AreEqual(TestData.Username, parameters.Username);
            Assert.AreEqual(string.Empty, parameters.Password);
            TestHelpers.PropertiesAreNull(parameters.ControlSettings);
            Assert.AreEqual(0, parameters.ReportParameters.Count);
        }

        [Test]
        public void Parse_HasServerUsernamePassword_ServerUsernamePasswordAndRestFromAppConfig()
        {
            var parser = new ReportViewerParametersParser();
            var queryString = GetQueryString();
            queryString.Add(UriParameters.ReportServerUrl, TestData.Server);
            queryString.Add(UriParameters.Username, TestData.Username);
            queryString.Add(UriParameters.Password, TestData.Password);
            var parameters = parser.Parse(queryString);
            Assert.AreEqual(TestData.ReportName, parameters.ReportPath);
            Assert.AreEqual(TestData.Server, parameters.ReportServerUrl);
            Assert.AreEqual(TestData.Username, parameters.Username);
            Assert.AreEqual(TestData.Password, parameters.Password);
            TestHelpers.PropertiesAreNull(parameters.ControlSettings);
            Assert.AreEqual(0, parameters.ReportParameters.Count);
        }

        [Test]
        public void Parse_HasServerUsernamePasswordPromptsFalse_AllFromQueryString()
        {
            var parser = new ReportViewerParametersParser();
            var queryString = GetQueryString();
            queryString.Add(UriParameters.ReportServerUrl, TestData.Server);
            queryString.Add(UriParameters.Username, TestData.Username);
            queryString.Add(UriParameters.Password, TestData.Password);

            // Set ReportViewer UI options
            
            queryString.Add(TestData.BackColor, TestData.BackColorArgbValue.ToString());
            queryString.Add(TestData.ShowParameterPrompts, TestData.ShowParameterPromptsValue.ToString());
            queryString.Add(TestData.ToolBarItemBorderStyle, TestData.ToolBarItemBorderStyleValue.ToString());
            queryString.Add(TestData.ToolBarItemBorderWidth, TestData.ToolBarItemBorderWidthValue.ToString());
            queryString.Add(TestData.ZoomMode, TestData.ZoomModeValue.ToString());
            queryString.Add(TestData.ZoomPercent, TestData.ZoomPercentValue.ToString());
            queryString.Add(TestData.HyperlinkTarget, TestData.HyperlinkTargetValue);
            
            var parameters = parser.Parse(queryString);
            Assert.AreEqual(TestData.ReportName, parameters.ReportPath);
            Assert.AreEqual(TestData.Server, parameters.ReportServerUrl);
            Assert.AreEqual(TestData.Username, parameters.Username);
            Assert.AreEqual(TestData.Password, parameters.Password);
            
            // Validate ReportViewer UI options

            TestHelpers.PropertiesAreNull(
                parameters.ControlSettings,
                "BackColor",
                "ShowParameterPrompts",
                "ToolBarItemBorderStyle",
                "ToolBarItemBorderWidth",
                "ZoomMode",
                "ZoomPercent",
                "HyperlinkTarget");

            Assert.AreEqual(
                TestData.BackColorValue.ToArgb(), 
                parameters.ControlSettings.BackColor.Value.ToArgb());

            Assert.AreEqual(
                TestData.ShowParameterPromptsValue, 
                parameters.ControlSettings.ShowParameterPrompts.Value);

            Assert.AreEqual(
                TestData.ToolBarItemBorderStyleValue, 
                parameters.ControlSettings.ToolBarItemBorderStyle.Value);

            Assert.AreEqual(
                TestData.ToolBarItemBorderWidthValue,
                parameters.ControlSettings.ToolBarItemBorderWidth.Value);

            Assert.AreEqual(
                TestData.ZoomModeValue,
                parameters.ControlSettings.ZoomMode.Value);

            Assert.AreEqual(
                TestData.ZoomPercentValue,
                parameters.ControlSettings.ZoomPercent.Value);

            Assert.AreEqual(
                TestData.HyperlinkTargetValue,
                parameters.ControlSettings.HyperlinkTarget);

            Assert.AreEqual(0, parameters.ReportParameters.Count);
        }

        [Test]
        public void Parse_EachTypeParameter_AllParametersHaveRightValuesAndDataTypes()
        {
            var parser = new ReportViewerParametersParser();
            var queryString = GetQueryString();
            PrepareTestReportParameters(queryString);
            var parameters = parser.Parse(queryString);
            Assert.AreEqual(TestData.ReportName, parameters.ReportPath);
            var errors = TestHelpers.ValidateReportParameters(parameters);
            if (!string.IsNullOrEmpty(errors))
            {
                Assert.Fail(errors);
            }
        }

        private NameValueCollection GetQueryString()
        {
            var queryString = new NameValueCollection { { UriParameters.ReportPath, TestData.ReportName } };
            return queryString;
        }

        private void PrepareTestReportParameters(NameValueCollection queryString)
        {
            foreach (var parameter in TestData.ExprectedParameters)
            {
                queryString.Add(parameter.Key, parameter.Value);
            }
        }
    }
}
