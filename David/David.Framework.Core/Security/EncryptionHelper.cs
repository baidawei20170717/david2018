using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace David.Framework.Core.Security
{
    public class EncryptionHelper
    {
        private static byte[] Keys = new byte[8]{
      (byte) 34,
      (byte) 226,
      (byte) 49,
      (byte) 117,
      (byte) 0,
      (byte) 193,
      (byte) 242,
      (byte) 34};
        public static string EncryptBalance(string encryptString, string key)
        {
            try
            {
                byte[] bytes1 = Encoding.UTF8.GetBytes(key);
                byte[] rgbIV = EncryptionHelper.Keys;
                byte[] bytes2 = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, cryptoServiceProvider.CreateEncryptor(bytes1, rgbIV), CryptoStreamMode.Write);
                cryptoStream.Write(bytes2, 0, bytes2.Length);
                cryptoStream.FlushFinalBlock();
                return Convert.ToBase64String(memoryStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }
        public static string DecryptBalance(string decryptString, string key)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(key);
                byte[] rgbIV = EncryptionHelper.Keys;
                byte[] buffer = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, cryptoServiceProvider.CreateDecryptor(bytes, rgbIV), CryptoStreamMode.Write);
                cryptoStream.Write(buffer, 0, buffer.Length);
                cryptoStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }
    }
}
