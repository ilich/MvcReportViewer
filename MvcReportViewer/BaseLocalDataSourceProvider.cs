using System;

namespace MvcReportViewer
{
    public abstract class BaseLocalDataSourceProvider
    {
        protected virtual string GetSessionValueKey(Guid reportControlId)
        {
            return $"MvcReportViewer_Local_{reportControlId}";
        }
    }
}
