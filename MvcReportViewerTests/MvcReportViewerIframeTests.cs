using NUnit.Framework;
using System.Collections.Generic;
using System.Web.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;

using Microsoft.Reporting.WebForms;

using Moq;

namespace MvcReportViewer.Tests
{
    [TestFixture]
    public class MvcReportViewerIframeTests : IframeTests
    {
        private readonly HtmlHelper _htmlHelper = HtmlHelperFactory.Create();

        [TestFixtureSetUp]
        public void Setup()
        {
            MvcReportViewerIframe.ApplyAppPathModifier = p => p;
        }

        [Test]
        public void Iframe_FromConfiguration_SrcOnly()
        {
            IProvideReportConfiguration configuration = new ReportConfigurationProvider
            {
                ControlId = Guid.Empty,
                ReportPath = TestData.ReportName
            };

            var iframe = _htmlHelper.MvcReportViewer(configuration);

            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl = $"{TestData.ViewerUri}&amp;{UriParameters.ReportPath}={TestData.ReportName}";
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void Iframe_FromConfiguration_SrcDataSources()
        {
            var httpContext = GetHttpContext();
            HttpContext.Current = httpContext;

            var mockDependencyResolver = new Mock<IDependencyResolver>();
            var sessionLocalDataSourceProvider = new SessionLocalDataSourceProvider();
            mockDependencyResolver.Setup(resolver => resolver.GetService(typeof(ILocalReportDataSourceProvider))).Returns(sessionLocalDataSourceProvider);
            var dependencyResolver = mockDependencyResolver.Object;

            DependencyResolver.SetResolver(dependencyResolver);

            var firstDataSource = new KeyValuePair<string, object>("First", new List<string>());
            var secondDataSource = new KeyValuePair<string, object>("Second", new List<string>());

            IProvideReportConfiguration configuration = new ReportConfigurationProvider
            {
                ControlId = Guid.Empty,
                DataSources = new List<KeyValuePair<string, object>>
                {
                    firstDataSource,
                    secondDataSource
                },
                ReportPath = TestData.ReportName
            };

            var iframe = _htmlHelper.MvcReportViewer(configuration);

            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl = $"{TestData.ViewerUri}&amp;{UriParameters.ReportPath}={TestData.ReportName}";
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);

            var dataSourceProvider = LocalReportDataSourceProviderFactory.Current.Create();
            var dataSources = dataSourceProvider.Get(iframe.ControlId).ToList();

            Assert.AreEqual(2, dataSources.Count);
            Assert.AreEqual(dataSources[0].Name, firstDataSource.Key);
            Assert.AreEqual(dataSources[1].Name, secondDataSource.Key);
        }

        [Test]
        public void Iframe_SrcOnly()
        {
            var iframe = _htmlHelper.MvcReportViewer(TestData.ReportName);
            iframe.ControlId = Guid.Empty;

            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl = $"{TestData.ViewerUri}&amp;{UriParameters.ReportPath}={TestData.ReportName}";
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void Iframe_FromConfiguration_HeightWidthSrc()
        {
            IProvideReportConfiguration configuration = new ReportConfigurationProvider
            {
                ControlId = Guid.Empty,
                HtmlAttributes = new { style = TestData.Style },
                ReportPath = TestData.ReportName,
            };

            var iframe = _htmlHelper.MvcReportViewer(configuration);

            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(2, html.Attributes.Count);
            Assert.AreEqual(TestData.Style, html.Attributes["style"].Value);
            var expectedUrl = $"{TestData.ViewerUri}&amp;{UriParameters.ReportPath}={TestData.ReportName}";
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void Iframe_HeightWidthSrc()
        {
            var iframe = _htmlHelper.MvcReportViewer(TestData.ReportName, new { style = TestData.Style });
            iframe.ControlId = Guid.Empty;

            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(2, html.Attributes.Count);
            Assert.AreEqual(TestData.Style, html.Attributes["style"].Value);
            var expectedUrl = $"{TestData.ViewerUri}&amp;{UriParameters.ReportPath}={TestData.ReportName}";
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void Iframe_FromConfiguration_SrcReportServer()
        {
            IProvideReportConfiguration configuration = new ReportConfigurationProvider
            {
                ControlId = Guid.Empty,
                ReportPath = TestData.ReportName,
                ReportServerUrl = TestData.Server
            };

            var iframe = _htmlHelper.MvcReportViewer(configuration);

            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl =
                $"{TestData.ViewerUri}&amp;{UriParameters.ReportPath}={TestData.ReportName}&amp;{UriParameters.ReportServerUrl}={TestData.Server}";
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void Iframe_SrcReportServer()
        {
            var iframe = _htmlHelper.MvcReportViewer(TestData.ReportName, TestData.Server);
            iframe.ControlId = Guid.Empty;

            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl =
                $"{TestData.ViewerUri}&amp;{UriParameters.ReportPath}={TestData.ReportName}&amp;{UriParameters.ReportServerUrl}={TestData.Server}";
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void Iframe_FromConfiguration_SrcReportServerCredentials()
        {
            IProvideReportConfiguration configuration = new ReportConfigurationProvider
            {
                ControlId = Guid.Empty,
                ReportPath = TestData.ReportName,
                ReportServerUrl = TestData.Server,
                Username = TestData.Username,
                Password = TestData.Password
            };

            var iframe = _htmlHelper.MvcReportViewer(configuration);

            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl =
                $"{TestData.ViewerUri}&amp;{UriParameters.ReportPath}={TestData.ReportName}&amp;{UriParameters.ReportServerUrl}={TestData.Server}&amp;{UriParameters.Username}={TestData.Username}&amp;{UriParameters.Password}={TestData.Password}";
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void Iframe_SrcReportServerCredentials()
        {
            var iframe = _htmlHelper.MvcReportViewer(
                TestData.ReportName,
                TestData.Server,
                TestData.Username,
                TestData.Password);
            iframe.ControlId = Guid.Empty;

            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl =
                $"{TestData.ViewerUri}&amp;{UriParameters.ReportPath}={TestData.ReportName}&amp;{UriParameters.ReportServerUrl}={TestData.Server}&amp;{UriParameters.Username}={TestData.Username}&amp;{UriParameters.Password}={TestData.Password}";
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void Iframe_FromConfiguration_SrcReportParameters()
        {
            IProvideReportConfiguration configuration = new ReportConfigurationProvider
            {
                ControlId = Guid.Empty,
                ReportParameters = new
                {
                    Param1 = TestData.ExprectedParameters["Param1"],
                    Param2 = TestData.ExprectedParameters["Param2"],
                    Param3 = TestData.ExprectedParameters["Param3"]
                },
                ReportPath = TestData.ReportName
            };

            var iframe = _htmlHelper.MvcReportViewer(configuration);

            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl =
                $"{TestData.ViewerUri}&amp;{UriParameters.ReportPath}={TestData.ReportName}&amp;{TestData.ReportParametes}";
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void Iframe_SrcReportParameters()
        {
            var iframe = _htmlHelper.MvcReportViewer(
                TestData.ReportName,
                new
                    {
                        Param1 = TestData.ExprectedParameters["Param1"],
                        Param2 = TestData.ExprectedParameters["Param2"],
                        Param3 = TestData.ExprectedParameters["Param3"]
                    },
                null);
            iframe.ControlId = Guid.Empty;

            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl =
                $"{TestData.ViewerUri}&amp;{UriParameters.ReportPath}={TestData.ReportName}&amp;{TestData.ReportParametes}";
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void Iframe_FromConfiguration_SrcReportServerCredentialsPromptsParametersId()
        {
            IProvideReportConfiguration configuration = new ReportConfigurationProvider
            {
                ControlId = Guid.Empty,
                ReportPath = TestData.ReportName,
                ReportServerUrl = TestData.Server,
                Username = TestData.Username,
                Password = TestData.Password,
                ReportParameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>("Param1", TestData.ExprectedParameters["Param1"]),
                        new KeyValuePair<string, object>("Param2", TestData.ExprectedParameters["Param2"]),
                        new KeyValuePair<string, object>("Param3", TestData.ExprectedParameters["Param3"]),
                    },
                ControlSettings = new ControlSettings
                {
                    ShowParameterPrompts = TestData.ShowParameterPromptsValue
                },
                HtmlAttributes = new
                {
                    id = TestData.Id,
                    height = TestData.FrameHeightValue
                }
            };

            var iframe = _htmlHelper.MvcReportViewer(configuration);

            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(3, html.Attributes.Count);
            Assert.AreEqual(TestData.Id, html.Attributes["id"].Value);
            Assert.AreEqual(TestData.FrameHeightValue.ToString(), html.Attributes["height"].Value);

            int expectedViewerHeight = TestData.FrameHeightValue - 20;
            var expectedUrl =
                $"{TestData.ViewerUri}&amp;{UriParameters.ReportPath}={TestData.ReportName}&amp;{UriParameters.ReportServerUrl}={TestData.Server}&amp;{UriParameters.Username}={TestData.Username}&amp;{UriParameters.Password}={TestData.Password}&amp;{TestData.ShowParameterPrompts}={TestData.ShowParameterPromptsValue}&amp;{TestData.FrameHeightParameter}={expectedViewerHeight}px&amp;{TestData.ReportParametes}";
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void Iframe_SrcReportServerCredentialsPromptsParametersId()
        {
            var iframe = _htmlHelper.MvcReportViewer(
                TestData.ReportName,
                TestData.Server,
                TestData.Username,
                TestData.Password,
                new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>("Param1", TestData.ExprectedParameters["Param1"]),
                        new KeyValuePair<string, object>("Param2", TestData.ExprectedParameters["Param2"]),
                        new KeyValuePair<string, object>("Param3", TestData.ExprectedParameters["Param3"]),
                    },
                new ControlSettings
                    {
                        ShowParameterPrompts = TestData.ShowParameterPromptsValue
                    },
                new
                {
                    id = TestData.Id,
                    height = TestData.FrameHeightValue
                });
            iframe.ControlId = Guid.Empty;

            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(3, html.Attributes.Count);
            Assert.AreEqual(TestData.Id, html.Attributes["id"].Value);
            Assert.AreEqual(TestData.FrameHeightValue.ToString(), html.Attributes["height"].Value);

            int expectedViewerHeight = TestData.FrameHeightValue - 20;
            var expectedUrl =
                $"{TestData.ViewerUri}&amp;{UriParameters.ReportPath}={TestData.ReportName}&amp;{UriParameters.ReportServerUrl}={TestData.Server}&amp;{UriParameters.Username}={TestData.Username}&amp;{UriParameters.Password}={TestData.Password}&amp;{TestData.ShowParameterPrompts}={TestData.ShowParameterPromptsValue}&amp;{TestData.FrameHeightParameter}={expectedViewerHeight}px&amp;{TestData.ReportParametes}";
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void Iframe_FromConfiguration_SrcClassId()
        {
            IProvideReportConfiguration configuration = new ReportConfigurationProvider
            {
                ControlId = Guid.Empty,
                ReportPath = TestData.ReportName,
                HtmlAttributes = new { @class = TestData.CssClass, id = TestData.Id }
            };

            var iframe = _htmlHelper.MvcReportViewer(configuration);

            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(3, html.Attributes.Count);
            Assert.AreEqual(TestData.CssClass, html.Attributes["class"].Value);
            Assert.AreEqual(TestData.Id, html.Attributes["id"].Value);
            var expectedUrl = $"{TestData.ViewerUri}&amp;{UriParameters.ReportPath}={TestData.ReportName}";
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void Iframe_SrcClassId()
        {
            var iframe = _htmlHelper.MvcReportViewer(TestData.ReportName, new { @class = TestData.CssClass, id = TestData.Id });
            iframe.ControlId = Guid.Empty;

            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(3, html.Attributes.Count);
            Assert.AreEqual(TestData.CssClass, html.Attributes["class"].Value);
            Assert.AreEqual(TestData.Id, html.Attributes["id"].Value);
            var expectedUrl = $"{TestData.ViewerUri}&amp;{UriParameters.ReportPath}={TestData.ReportName}";
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }
        
        [Test]
        public void Iframe_FromConfiguration_TestPostMethod_HasIframe()
        {
            CheckPostGeneratedFromConfigurationIframe("iframe");
        }

        [Test]
        public void Iframe_TestPostMethod_HasIframe()
        {
            CheckPostGeneratedIframe("iframe");
        }

        [Test]
        public void Iframe_FromConfiguration_TestPostMethod_HasForm()
        {
            CheckPostGeneratedFromConfigurationIframe("form");
        }

        [Test]
        public void Iframe_TestPostMethod_HasForm()
        {
            CheckPostGeneratedIframe("form");
        }

        [Test]
        public void Iframe_FromConfiguration_TestPostMethod_HasScript()
        {
            CheckPostGeneratedFromConfigurationIframe("script");
        }

        [Test]
        public void Iframe_TestPostMethod_HasScript()
        {
            CheckPostGeneratedIframe("script");
        }

        private void CheckPostGeneratedFromConfigurationIframe(string expectedTag)
        {
            IProvideReportConfiguration configuration = new ReportConfigurationProvider
            {
                ReportPath = TestData.ReportName,
                FormMethod = FormMethod.Post
            };

            var html = _htmlHelper.MvcReportViewer(configuration);
            var hasTag = HasTag(html, expectedTag);
            Assert.IsTrue(hasTag);
        }

        private HttpContext GetHttpContext()
        {
            var httpRequest = new HttpRequest("", "http://localhost/", "");
            var stringWriter = new StringWriter();
            var httpResponce = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponce);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                    new HttpStaticObjectsCollection(), 10, true,
                                                    HttpCookieMode.AutoDetect,
                                                    SessionStateMode.InProc, false);

            SessionStateUtility.AddHttpSessionStateToContext(httpContext, sessionContainer);

            return httpContext;
        }

        private void CheckPostGeneratedIframe(string expectedTag)
        {
            var html = _htmlHelper.MvcReportViewer(TestData.ReportName, method: FormMethod.Post);
            var hasTag = HasTag(html, expectedTag);
            Assert.IsTrue(hasTag);
        }
    }
}
