using NUnit.Framework;
using System.Collections.Generic;
using System.Web.Mvc;

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
            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl = string.Format(
                "{0}?{1}={2}",
                TestData.ViewerUri,
                UriParameters.ReportPath,
                TestData.ReportName);
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void Iframe_HeightWidthSrc()
        {
            var iframe = _htmlHelper.MvcReportViewer(TestData.ReportName, new { style = TestData.Style });
            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(2, html.Attributes.Count);
            Assert.AreEqual(TestData.Style, html.Attributes["style"].Value);
            var expectedUrl = string.Format(
                "{0}?{1}={2}",
                TestData.ViewerUri,
                UriParameters.ReportPath,
                TestData.ReportName);
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void Iframe_SrcReportServer()
        {
            var iframe = _htmlHelper.MvcReportViewer(TestData.ReportName, TestData.Server);
            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl = string.Format(
                "{0}?{1}={2}&amp;{3}={4}",
                TestData.ViewerUri,
                UriParameters.ReportPath,
                TestData.ReportName,
                UriParameters.ReportServerUrl,
                TestData.Server);
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
            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl = string.Format(
                "{0}?{1}={2}&amp;{3}={4}&amp;{5}={6}&amp;{7}={8}",
                TestData.ViewerUri,
                UriParameters.ReportPath,
                TestData.ReportName,
                UriParameters.ReportServerUrl,
                TestData.Server,
                UriParameters.Username,
                TestData.Username,
                UriParameters.Password,
                TestData.Password);
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
            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl = string.Format(
                "{0}?{1}={2}&amp;{3}",
                TestData.ViewerUri,
                UriParameters.ReportPath,
                TestData.ReportName,
                TestData.ReportParametes);
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
            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(2, html.Attributes.Count);
            Assert.AreEqual(TestData.Id, html.Attributes["id"].Value);
            var expectedUrl = string.Format(
                "{0}?{1}={2}&amp;{3}={4}&amp;{5}={6}&amp;{7}={8}&amp;{9}={10}&amp;{11}",
                TestData.ViewerUri,
                UriParameters.ReportPath,
                TestData.ReportName,
                UriParameters.ReportServerUrl,
                TestData.Server,
                UriParameters.Username,
                TestData.Username,
                UriParameters.Password,
                TestData.Password,
                TestData.ShowParameterPrompts,
                TestData.ShowParameterPromptsValue,
                TestData.ReportParametes);
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void Iframe_SrcClassId()
        {
            var iframe = _htmlHelper.MvcReportViewer(TestData.ReportName, new { @class = TestData.CssClass, id = TestData.Id });
            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(3, html.Attributes.Count);
            Assert.AreEqual(TestData.CssClass, html.Attributes["class"].Value);
            Assert.AreEqual(TestData.Id, html.Attributes["id"].Value);
            var expectedUrl = string.Format(
                "{0}?{1}={2}",
                TestData.ViewerUri,
                UriParameters.ReportPath,
                TestData.ReportName);
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
