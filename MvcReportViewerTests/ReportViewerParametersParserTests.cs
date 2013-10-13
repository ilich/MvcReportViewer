using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace MvcReportViewer.Tests
{
    [TestFixture]
    public class ReportViewerParametersParserTests
    {
        private const string ReportName = "TestReport";

        private const string Server = "DummyServer";

        private const string Username = "root";

        private const string Password = "secret";

        private static readonly Dictionary<string, string> ExprectedParameters = new Dictionary<string, string>
            {
                { "Param1", "Test" },
                { "Param2", "22" },
                { "Param3", "25.5" }
            };

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
            Assert.AreEqual(ReportName, parameters.ReportPath);
            Assert.AreEqual("http://localhost/ReportServer_SQLEXPRESS", parameters.ReportServerUrl);
            Assert.AreEqual("admin", parameters.Username);
            Assert.AreEqual("password", parameters.Password);
            Assert.IsTrue(parameters.ShowParameterPrompts);
            Assert.AreEqual(0, parameters.ReportParameters.Count);
        }

        [Test]
        public void Parse_HasServer_ServerAndRestFromAppConfig()
        {
            var parser = new ReportViewerParametersParser();
            var queryString = GetQueryString();
            queryString.Add(UriParameters.ReportServerUrl, Server);
            var parameters = parser.Parse(queryString);
            Assert.AreEqual(ReportName, parameters.ReportPath);
            Assert.AreEqual(Server, parameters.ReportServerUrl);
            Assert.AreEqual("admin", parameters.Username);
            Assert.AreEqual("password", parameters.Password);
            Assert.IsTrue(parameters.ShowParameterPrompts);
            Assert.AreEqual(0, parameters.ReportParameters.Count);
        }

        [Test]
        public void Parse_HasServerUsername_ServerUsernameEmptyPasswordAndRestFromAppConfig()
        {
            var parser = new ReportViewerParametersParser();
            var queryString = GetQueryString();
            queryString.Add(UriParameters.ReportServerUrl, Server);
            queryString.Add(UriParameters.Username, Username);
            var parameters = parser.Parse(queryString);
            Assert.AreEqual(ReportName, parameters.ReportPath);
            Assert.AreEqual(Server, parameters.ReportServerUrl);
            Assert.AreEqual(Username, parameters.Username);
            Assert.AreEqual(string.Empty, parameters.Password);
            Assert.IsTrue(parameters.ShowParameterPrompts);
            Assert.AreEqual(0, parameters.ReportParameters.Count);
        }

        [Test]
        public void Parse_HasServerUsernamePassword_ServerUsernamePasswordAndRestFromAppConfig()
        {
            var parser = new ReportViewerParametersParser();
            var queryString = GetQueryString();
            queryString.Add(UriParameters.ReportServerUrl, Server);
            queryString.Add(UriParameters.Username, Username);
            queryString.Add(UriParameters.Password, Password);
            var parameters = parser.Parse(queryString);
            Assert.AreEqual(ReportName, parameters.ReportPath);
            Assert.AreEqual(Server, parameters.ReportServerUrl);
            Assert.AreEqual(Username, parameters.Username);
            Assert.AreEqual(Password, parameters.Password);
            Assert.IsTrue(parameters.ShowParameterPrompts);
            Assert.AreEqual(0, parameters.ReportParameters.Count);
        }

        [Test]
        public void Parse_HasServerUsernamePasswordPromptsFalse_AllFromQueryString()
        {
            var parser = new ReportViewerParametersParser();
            var queryString = GetQueryString();
            queryString.Add(UriParameters.ReportServerUrl, Server);
            queryString.Add(UriParameters.Username, Username);
            queryString.Add(UriParameters.Password, Password);
            queryString.Add(UriParameters.ShowParameterPrompts, bool.FalseString);
            var parameters = parser.Parse(queryString);
            Assert.AreEqual(ReportName, parameters.ReportPath);
            Assert.AreEqual(Server, parameters.ReportServerUrl);
            Assert.AreEqual(Username, parameters.Username);
            Assert.AreEqual(Password, parameters.Password);
            Assert.IsFalse(parameters.ShowParameterPrompts);
            Assert.AreEqual(0, parameters.ReportParameters.Count);
        }

        [Test]
        public void Parse_HasServerUsernamePasswordPromptsTrue_AllFromQueryString()
        {
            var parser = new ReportViewerParametersParser();
            var queryString = GetQueryString();
            queryString.Add(UriParameters.ReportServerUrl, Server);
            queryString.Add(UriParameters.Username, Username);
            queryString.Add(UriParameters.Password, Password);
            queryString.Add(UriParameters.ShowParameterPrompts, "1");
            var parameters = parser.Parse(queryString);
            Assert.AreEqual(ReportName, parameters.ReportPath);
            Assert.AreEqual(Server, parameters.ReportServerUrl);
            Assert.AreEqual(Username, parameters.Username);
            Assert.AreEqual(Password, parameters.Password);
            Assert.IsTrue(parameters.ShowParameterPrompts);
            Assert.AreEqual(0, parameters.ReportParameters.Count);
        }

        [Test]
        public void Parse_EachTypeParameter_AllParametersHaveRightValuesAndDataTypes()
        {
            var parser = new ReportViewerParametersParser();
            var queryString = GetQueryString();
            PrepareTestReportParameters(queryString);
            var parameters = parser.Parse(queryString);
            Assert.AreEqual(ReportName, parameters.ReportPath);
            var errors = ValidateReportParameters(parameters);
            if (!string.IsNullOrEmpty(errors))
            {
                Assert.Fail(errors);
            }
        }

        private NameValueCollection GetQueryString()
        {
            var queryString = new NameValueCollection { { UriParameters.ReportPath, ReportName } };
            return queryString;
        }

        private void PrepareTestReportParameters(NameValueCollection queryString)
        {
            foreach (var parameter in ExprectedParameters)
            {
                queryString.Add(parameter.Key, parameter.Value);
            }
        }

        private string ValidateReportParameters(ReportViewerParameters parameters)
        {
            var reportParameters = parameters.ReportParameters;
            if (reportParameters.Count != ExprectedParameters.Count)
            {
                return string.Format(
                    "There are {0} report parameters, but should be {1}.", 
                    reportParameters.Count,
                    ExprectedParameters.Count);
            }

            var errors = new StringBuilder();
            foreach (var expected in ExprectedParameters)
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
