using NUnit.Framework;
using System.Web.Mvc;
using System;
using Microsoft.Reporting.WebForms;

namespace MvcReportViewer.Tests
{
    [TestFixture]
    public class MvcReportViewerIframeFluentTests : IframeTests
    {
        private readonly HtmlHelper _htmlHelper = HtmlHelperFactory.Create();

        [Test]
        public void IframeFluent_SrcOnlyLocalProcessing()
        {
            var iframe = _htmlHelper.MvcReportViewerFluent(TestData.ReportName, Guid.Empty)
                                    .ProcessingMode(ProcessingMode.Local);

            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl = string.Format(
                "{0}&amp;{1}={2}",
                TestData.ViewerUriLocal,
                UriParameters.ReportPath,
                TestData.ReportName);
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void IframeFluent_SrcOnly()
        {
            var iframe = _htmlHelper.MvcReportViewerFluent(TestData.ReportName, Guid.Empty);

            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl = string.Format(
                "{0}&amp;{1}={2}",
                TestData.ViewerUri,
                UriParameters.ReportPath,
                TestData.ReportName);
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void IframeFluent_HeightWidthSrc()
        {
            var iframe = _htmlHelper.MvcReportViewerFluent(TestData.ReportName, Guid.Empty)
                                    .Attributes(new { style = TestData.Style });
            
            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(2, html.Attributes.Count);
            Assert.AreEqual(TestData.Style, html.Attributes["style"].Value);
            var expectedUrl = string.Format(
                "{0}&amp;{1}={2}",
                TestData.ViewerUri,
                UriParameters.ReportPath,
                TestData.ReportName);
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void IframeFluent_SrcReportServer()
        {
            var iframe = _htmlHelper.MvcReportViewerFluent(TestData.ReportName, Guid.Empty)
                                    .ReportServerUrl(TestData.Server);
            
            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl = string.Format(
                "{0}&amp;{1}={2}&amp;{3}={4}",
                TestData.ViewerUri,
                UriParameters.ReportPath,
                TestData.ReportName,
                UriParameters.ReportServerUrl,
                TestData.Server);
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void IframeFluent_SrcReportServerCredentials()
        {
            var iframe = _htmlHelper.MvcReportViewerFluent(TestData.ReportName, Guid.Empty)
                                    .ReportServerUrl(TestData.Server)
                                    .Username(TestData.Username)
                                    .Password(TestData.Password);
            
            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl = string.Format(
                "{0}&amp;{1}={2}&amp;{3}={4}&amp;{5}={6}&amp;{7}={8}",
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
        public void IframeFluent_SrcReportParameters()
        {
            var iframe = _htmlHelper.MvcReportViewerFluent(TestData.ReportName, Guid.Empty)
                                    .ReportParameters(
                                        new
                                            {
                                                Param1 = TestData.ExprectedParameters["Param1"],
                                                Param2 = TestData.ExprectedParameters["Param2"],
                                                Param3 = TestData.ExprectedParameters["Param3"]
                                            });
            
            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(1, html.Attributes.Count);
            var expectedUrl = string.Format(
                "{0}&amp;{1}={2}&amp;{3}",
                TestData.ViewerUri,
                UriParameters.ReportPath,
                TestData.ReportName,
                TestData.ReportParametes);
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }

        [Test]
        public void IframeFluent_SrcReportServerCredentialsPromptsParametersId()
        {
            var iframe = _htmlHelper.MvcReportViewerFluent(TestData.ReportName, Guid.Empty)
                                    .ReportServerUrl(TestData.Server)
                                    .Username(TestData.Username)
                                    .Password(TestData.Password)
                                    .ReportParameters(
                                        new
                                            {
                                                Param1 = TestData.ExprectedParameters["Param1"],
                                                Param2 = TestData.ExprectedParameters["Param2"],
                                                Param3 = TestData.ExprectedParameters["Param3"]
                                            })
                                    .ControlSettings(new ControlSettings
                                        {
                                            ShowParameterPrompts = TestData.ShowParameterPromptsValue
                                        })
                                    .Attributes(new { id = TestData.Id });
            
            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(2, html.Attributes.Count);
            Assert.AreEqual(TestData.Id, html.Attributes["id"].Value);
            var expectedUrl = string.Format(
                "{0}&amp;{1}={2}&amp;{3}={4}&amp;{5}={6}&amp;{7}={8}&amp;{9}={10}&amp;{11}",
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
        public void IframeFluent_SrcClassId()
        {
            var iframe = _htmlHelper.MvcReportViewerFluent(TestData.ReportName, Guid.Empty)
                                    .Attributes(new { @class = TestData.CssClass, id = TestData.Id });
            
            var html = ToIframeHtml(iframe);
            Assert.AreEqual("iframe", html.Name);
            Assert.AreEqual(3, html.Attributes.Count);
            Assert.AreEqual(TestData.CssClass, html.Attributes["class"].Value);
            Assert.AreEqual(TestData.Id, html.Attributes["id"].Value);
            var expectedUrl = string.Format(
                "{0}&amp;{1}={2}",
                TestData.ViewerUri,
                UriParameters.ReportPath,
                TestData.ReportName);
            Assert.AreEqual(expectedUrl, html.Attributes["src"].Value);
        }
    }
}
