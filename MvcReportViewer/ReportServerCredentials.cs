using System;
using Microsoft.Reporting.WebForms;
using System.Net;
using System.Security.Principal;

namespace MvcReportViewer
{
    [Serializable]
    internal class ReportServerCredentials : IReportServerCredentials
    {
        private readonly string _username;

        private readonly string _password;

        public ReportServerCredentials(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public WindowsIdentity ImpersonationUser => null;

        public ICredentials NetworkCredentials => new NetworkCredential(_username, _password);

        public bool GetFormsCredentials(out Cookie authCookie, out string userName, out string password, out string authority)
        {
            authCookie = null;
            userName = null;
            password = null;
            authority = null;
            return false;
        }
    }
}
