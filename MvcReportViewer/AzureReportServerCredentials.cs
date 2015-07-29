using Microsoft.Reporting.WebForms;
using System.Net;
using System.Security.Principal;

namespace MvcReportViewer
{
    // Please check http://msdn.microsoft.com/en-us/library/gg552871.aspx#Authentication for
    // additional details or if you have found a bug in this code.

    internal class AzureReportServerCredentials : IReportServerCredentials
    {
        private readonly string _username;

        private readonly string _password;

        private readonly string _server;

        public AzureReportServerCredentials(string username, string password, string server)
        {
            _username = username;
            _password = password;
            _server = server;
        }

        public bool GetFormsCredentials(out Cookie authCookie, out string userName, out string password, out string authority)
        {
            authCookie = null;
            userName = _username;
            password = _password;
            authority = _server;
            return true;
        }

        public WindowsIdentity ImpersonationUser => null;

        public ICredentials NetworkCredentials => null;
    }
}
