using System;
using System.Collections.Specialized;
using System.Linq;

namespace MvcReportViewer
{
    internal static class IgnoreCaseExtensions
    {
        public static bool EqualsIgnoreCase(this string that, string str)
        {
            return string.Compare(that, str, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static bool ContainsKeyIgnoreCase(this NameValueCollection collection, string key)
        {
            return collection.AllKeys.Any(k => k.EqualsIgnoreCase(key));
        }
    }
}
