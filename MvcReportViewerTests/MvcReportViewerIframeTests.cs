using NUnit.Framework;
using System.Collections.Generic;
using System.Web.Mvc;
using System;

namespace MvcReportViewer.Tests
{
    [TestFixture]
    public class MvcReportViewerIframeTests : IframeTests
    {
        private readonly HtmlHelper _htmlHelper = HtmlHelperFactory.Create();

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
                new { id = TestData.Id });
            iframe.ControlId = Guid.Empty;

            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(2, html.Attributes.Count);
            Assert.AreEqual(TestData.Id, html.Attributes["id"].Value);
            var expectedUrl =
                $"{TestData.ViewerUri}&amp;{UriParameters.ReportPath}={TestData.ReportName}&amp;{UriParameters.ReportServerUrl}={TestData.Server}&amp;{UriParameters.Username}={TestData.Username}&amp;{UriParameters.Password}={TestData.Password}&amp;{TestData.ShowParameterPrompts}={TestData.ShowParameterPromptsValue}&amp;{TestData.ReportParametes}";
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
        public void Iframe_TestPostMethod_HasIframe()
        {
            CheckPostGeneratedIframe("iframe");
        }

        [Test]
        public void Iframe_TestPostMethod_HasForm()
        {
            CheckPostGeneratedIframe("form");
        }

        [Test]
        public void Iframe_TestPostMethod_HasScript()
        {
            CheckPostGeneratedIframe("script");
        }

        private void CheckPostGeneratedIframe(string expectedTag)
        {
            var html = _htmlHelper.MvcReportViewer(TestData.ReportName, method: FormMethod.Post);
            var hasTag = HasTag(html, expectedTag);
            Assert.IsTrue(hasTag);
        }
    }
}
