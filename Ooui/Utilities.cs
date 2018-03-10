using System;
using System.Text;

namespace Ooui
{
    public static class Utilities
    {
#if PCL

        static readonly uint[] crcTable;

        static Utilities ()
        {
            uint p = 0x04C11DB7;
            crcTable = new uint[256];
            for (uint c = 0; c <= 0xFF; c++) {
                crcTable[c] = CrcReflect (c, 8) << 24;
                for (uint i = 0; i < 8; i++) {
                    crcTable[c] = (crcTable[c] << 1) ^ (((crcTable[c] & (1u << 31)) != 0) ? p : 0);
                }
                crcTable[c] = CrcReflect (crcTable[c], 32);
            }
        }

        static uint CrcReflect (uint r, byte c)
        {
            uint v = 0;
            for (int i = 1; i < (c + 1); i++) {
                if ((r & 1) != 0) {
                    v |= (1u << (c - i));
                }
                r >>= 1;
            }
            return v;
        }

        public static string Hash (byte[] bytes)
        {
            uint crc = 0xffffffffu;
            for (var i = 0; i < bytes.Length; i++) {
                crc = (crc >> 8) ^ crcTable[(crc & 0xff) ^ bytes[i]];
            }
            crc ^= 0xffffffffu;
            return crc.ToString ("x8");
        }

#else

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

#endif
    }
}
