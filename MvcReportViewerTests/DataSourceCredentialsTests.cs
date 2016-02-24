using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using NUnit.Framework;

namespace MvcReportViewer.Tests
{
    [TestFixture]
    public class DataSourceCredentialsTests : IframeTests
    {
        private readonly HtmlHelper _htmlHelper = HtmlHelperFactory.Create();

        [TestFixtureSetUp]
        public void Setup()
        {
            MvcReportViewerIframe.ApplyAppPathModifier = p => p;
        }

        [SetUp]
        public void SetUp()
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest(null, "http://tempuri.org", null),
                new HttpResponse(null));
        }

        [TearDown]
        public void TearDown()
        {
            HttpContext.Current = null;
        }

        [Test]
        public void TestDataSourceCredentialsSerialization()
        {


            var iframe = _htmlHelper.MvcReportViewerFluent(TestData.ReportName, Guid.Empty)
                                    .ReportPath(TestData.Server)
                                    .SetDataSourceCredentials(new DataSourceCredentials[]
                                    {
                                       new DataSourceCredentials
                                       {
                                           Name = TestData.DataSource,
                                           UserId = TestData.DataSourceUsername,
                                           Password = TestData.DataSourcePassword
                                       }
                                    });

            var options = SerializeAndParse(iframe);
            Assert.AreEqual(1, options.DataSourceCredentials?.Length);
            Assert.AreEqual(TestData.DataSource, options.DataSourceCredentials?[0].Name);
            Assert.AreEqual(TestData.DataSourceUsername, options.DataSourceCredentials?[0].UserId);
            Assert.AreEqual(TestData.DataSourcePassword, options.DataSourceCredentials?[0].Password);
        }
    }
}
