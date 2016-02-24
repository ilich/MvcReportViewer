using System;
using System.Web;
using HtmlAgilityPack;

namespace MvcReportViewer.Tests
{
    public abstract class IframeTests
    {
        protected HtmlNode ToIframeHtml(IHtmlString html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html.ToHtmlString());
            return doc.DocumentNode.SelectSingleNode("//iframe");
        }

        protected bool HasTag(IHtmlString html, string tag)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html.ToHtmlString());
            var xpath = "//" + tag;
            return doc.DocumentNode.SelectSingleNode(xpath) != null;
        }

        internal ReportViewerParameters SerializeAndParse(IHtmlString html)
        {
            var iframe = ToIframeHtml(html);
            var src = iframe.Attributes["src"].Value;
            src = src.Replace("&amp;", "&");
            src = $"http://tempuri.org{src}";
            var iframeUri = new Uri(src);
            var queryString = HttpUtility.ParseQueryString(iframeUri.Query);

            var parser = new ReportViewerParametersParser();
            return parser.Parse(queryString);
        }
    }
}
