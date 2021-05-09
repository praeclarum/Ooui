using System;
using System.Security.Cryptography;
using System.Text;

namespace Ooui
{
    public static class Utilities
    {
        [ThreadStatic]
        static SHA256 sha256;

        [ThreadStatic]
        static MD5 md5;

        public static string GetShaHash(byte[] bytes)
        {
            var sha = sha256;
            if (sha == null)
            {
                sha = SHA256.Create();
                sha256 = sha;
            }
            var data = sha.ComputeHash(bytes);

            return BytesToString(data);
        }

        public static string GetMd5Hash(string input)
        {
            var md = md5;
            if (md == null)
            {
                md = MD5.Create();
                md5 = md;
            }
            
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md.ComputeHash(Encoding.UTF8.GetBytes(input));

            return BytesToString(data);
        }

        private static string BytesToString(byte[] data)
        {
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public static string GetHash(string input)
        {
            return Crc64.GetHash(input);
        }
    }
}
