using HtmlAgilityPack;
using System.Web;

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
    }
}
