using System;

namespace MvcReportViewer
{
    public abstract class BaseLocalDataSourceProvider
    {
        protected virtual string GetSessionValueKey(Guid reportControlId)
        {
            return string.Format("MvcReportViewer_Local_{0}", reportControlId);
        }
    }
}
