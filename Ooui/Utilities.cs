using System;
using System.Text;

namespace Ooui
{
    public static class Utilities
    {
        [ThreadStatic]
        static System.Security.Cryptography.SHA256 sha256;

        public static string Hash (byte[] bytes)
        {
            var sha = sha256;
            if (sha == null) {
                sha = System.Security.Cryptography.SHA256.Create ();
                sha256 = sha;
            }
            var data = sha.ComputeHash (bytes);
            StringBuilder sBuilder = new StringBuilder ();
            for (int i = 0; i < data.Length; i++) {
                sBuilder.Append (data[i].ToString ("x2"));
            }
            return sBuilder.ToString ();
        }
    }
}
