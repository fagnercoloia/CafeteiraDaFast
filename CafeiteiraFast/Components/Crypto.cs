using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace CafeiteiraFast.Components
{
    public static class Crypto
    {
        static byte[] keyIVBytes = ASCIIEncoding.ASCII.GetBytes("FASTCAFE");

        public static string Encrypt(string originalString)
        {
            if (String.IsNullOrEmpty(originalString))
            {
                throw new ArgumentNullException
                       ("The string which needs to be encrypted can not be null.");
            }

            var cryptoProvider = new DESCryptoServiceProvider();
            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream,
                    cryptoProvider.CreateEncryptor(keyIVBytes, keyIVBytes), CryptoStreamMode.Write))
                {
                    using (var writer = new StreamWriter(cryptoStream))
                    {
                        writer.Write(originalString);
                        writer.Flush();
                        cryptoStream.FlushFinalBlock();
                        writer.Flush();

                        return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
                    }
                }
            }
        }

        public static string Decrypt(string cryptedString)
        {
            if (String.IsNullOrEmpty(cryptedString))
            {
                throw new ArgumentNullException
                   ("The string which needs to be decrypted can not be null.");
            }
            var cryptoProvider = new DESCryptoServiceProvider();
            using (var memoryStream = new MemoryStream(Convert.FromBase64String(cryptedString)))
            {
                using (var cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(keyIVBytes, keyIVBytes), CryptoStreamMode.Read))
                {
                    using (var reader = new StreamReader(cryptoStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}