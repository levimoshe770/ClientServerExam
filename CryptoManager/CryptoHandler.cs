using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CryptoManager
{
    public class CryptoHandler
    {
        static public string HashEncode (string pInputString)
        {
            string res;
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hashValue = sha.ComputeHash(Encoding.ASCII.GetBytes(pInputString));
                res = ConvertHashToString(hashValue);
            }
            return res;
        }

        private static string ConvertHashToString(byte[] buffer)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                sb.Append(string.Format("{0:X2}", buffer[i]));
                if (i % 4 == 3)
                    sb.Append(" ");
            }
            return sb.ToString();
        }

    }
}
