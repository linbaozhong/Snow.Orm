using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Snow.Orm
{
    public class MD5Encrypt
    {
        //MD5不可逆加密 
        //32位加密 
        public static string Get32(string s)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x2"));
            }

            return sb.ToString();
        }

        //16位加密 
        public static string Get16(string s)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(s)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }
    }
}
