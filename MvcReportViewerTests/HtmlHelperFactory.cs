using Moq;
using System.Web.Mvc;

namespace MvcReportViewer.Tests
{
    internal static class HtmlHelperFactory
    {
        public static HtmlHelper Create()
        {
            var viewContext = new ViewContext();
            var dataContainer = new Mock<IViewDataContainer>();
            var htmlHelper = new HtmlHelper(viewContext, dataContainer.Object);
            return htmlHelper;
        }
    }
}
