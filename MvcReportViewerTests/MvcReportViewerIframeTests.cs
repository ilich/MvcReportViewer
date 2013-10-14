using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Mvc;
using HtmlAgilityPack;
using System.Web;

namespace MvcReportViewer.Tests
{
    [TestFixture]
    public class MvcReportViewerIframeTests
    {
        private const string Style = "height: 100px; width: 100px;";

        private const string CssClass = "dummy-class";

        private const string Id = "dummy-id";

        private const string ViewerUri = "/MvcReportViewer.aspx";

        private readonly string ReportParametes = string.Join("&", TestData.ExprectedParameters
                                                                           .Select(p => HttpUtility.UrlEncode(p.Key) + "=" + HttpUtility.UrlEncode(p.Value)));

        private HtmlHelper _htmlHelper = HtmlHelperFactory.Create();

        [Test]
        public void Iframe_SrcOnly()
        {
            var iframe = _htmlHelper.MvcReportViewer(TestData.ReportName);
            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl = string.Format(
                "{0}?{1}={2}", 
                ViewerUri,
                UriParameters.ReportPath, 
                TestData.ReportName);
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void Iframe_HeightWidthSrc()
        {
            var iframe = _htmlHelper.MvcReportViewer(TestData.ReportName, new { style = Style });
            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(2, html.Attributes.Count);
            Assert.AreEqual(Style, html.Attributes["style"].Value);
            var expectedUrl = string.Format(
                "{0}?{1}={2}",
                ViewerUri,
                UriParameters.ReportPath,
                TestData.ReportName);
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void Iframe_SrcReportServer()
        {
            var iframe = _htmlHelper.MvcReportViewer(TestData.ReportName, reportServerUrl: TestData.Server);
            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl = string.Format(
                "{0}?{1}={2}&{3}={4}",
                ViewerUri,
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
                reportServerUrl: TestData.Server,
                username: TestData.Username,
                password: TestData.Password);
            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl = string.Format(
                "{0}?{1}={2}&{3}={4}&{5}={6}&{7}={8}",
                ViewerUri,
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
                "{0}?{1}={2}&{3}",
                ViewerUri,
                UriParameters.ReportPath,
                TestData.ReportName,
                ReportParametes);
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
                new
                    {
                        Param1 = TestData.ExprectedParameters["Param1"],
                        Param2 = TestData.ExprectedParameters["Param2"],
                        Param3 = TestData.ExprectedParameters["Param3"]
                    },
                TestData.ShowParameterPrompts,
                new { id = Id });
            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(2, html.Attributes.Count);
            Assert.AreEqual(Id, html.Attributes["id"].Value);
            var expectedUrl = string.Format(
                "{0}?{1}={2}&{3}={4}&{5}={6}&{7}={8}&{9}={10}&{11}",
                ViewerUri,
                UriParameters.ReportPath,
                TestData.ReportName,
                UriParameters.ReportServerUrl,
                TestData.Server,
                UriParameters.Username,
                TestData.Username,
                UriParameters.Password,
                TestData.Password,
                UriParameters.ShowParameterPrompts,
                TestData.ShowParameterPrompts.ToString(),
                ReportParametes);
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void Iframe_SrcClassId()
        {
            var iframe = _htmlHelper.MvcReportViewer(TestData.ReportName, new { @class = CssClass, id = Id });
            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(3, html.Attributes.Count);
            Assert.AreEqual(CssClass, html.Attributes["class"].Value);
            Assert.AreEqual(Id, html.Attributes["id"].Value);
            var expectedUrl = string.Format(
                "{0}?{1}={2}",
                ViewerUri,
                UriParameters.ReportPath,
                TestData.ReportName);
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        private HtmlNode ToIframeHtml(IHtmlString html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html.ToHtmlString());
            return doc.DocumentNode.SelectSingleNode("//iframe");
        }
    }
}
