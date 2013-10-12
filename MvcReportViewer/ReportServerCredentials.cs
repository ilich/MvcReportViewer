using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Net;

namespace MvcReportViewer
{
    internal class ReportServerCredentials : IReportServerCredentials
    {
        private readonly string _username;

        private readonly string _password;

        public ReportServerCredentials(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public WindowsIdentity ImpersonationUser
        {
            get { return null; }
        }

        public ICredentials NetworkCredentials
        {
            get
            {
                return new NetworkCredential(_username, _password);
            }
        }

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
