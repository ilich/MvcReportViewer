using System;
using System.Text;
using System.Web.Security;

namespace MvcReportViewer
{
    static class SecurityUtil
    {
        public static string Encrypt(string value)
        {
            var bytes = Encoding.Unicode.GetBytes(value);
#if NET45
            var encryptedBytes = MachineKey.Protect(bytes);
            var encrypted = BitConverter.ToString(encryptedBytes).Replace("-", "");
#else
            var encrypted = MachineKey.Encode(bytes, MachineKeyProtection.Encryption);
#endif
            return encrypted;
        }

        public static string Decrypt(string encrypted)
        {
#if NET45
            var encryptedBytes = Hex2Bytes(encrypted);
            var bytes = MachineKey.Unprotect(encryptedBytes);
#else
            var bytes = MachineKey.Decode(encrypted, MachineKeyProtection.Encryption);
#endif
            if (bytes == null)
            {
                throw new InvalidOperationException("Data cannot be decrypted using Machine Key");    
            }

            var value = Encoding.Unicode.GetString(bytes);
            return value;
        }

        private static byte[] Hex2Bytes(string hex)
        {
            if (hex.Length % 2 != 0)
            {
                throw new ArgumentException(hex);
            }

            var bytes = new byte[hex.Length / 2];
            for(var i = 0; i < hex.Length; i += 2)
            {
                var encByte = hex.Substring(i, 2);
                bytes[i / 2] = Convert.ToByte(encByte, 16);
            }

            return bytes;
        }
    }
}
