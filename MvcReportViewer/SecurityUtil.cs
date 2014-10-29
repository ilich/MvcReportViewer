using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace MvcReportViewer
{
    static class SecurityUtil
    {
        public static string Encrypt(string value)
        {
            var bytes = Encoding.Unicode.GetBytes(value);
            var encrypted = MachineKey.Encode(bytes, MachineKeyProtection.Encryption);
            return encrypted;
        }

        public static string Decrypt(string encrypted)
        {
            var bytes = MachineKey.Decode(encrypted, MachineKeyProtection.Encryption);
            var value = Encoding.Unicode.GetString(bytes);
            return value;
        }
    }
}
