using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace qRPC.Transport
{
    internal static class AesOperation
    {
        public static string Encrypt(this string data, string key)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.GenerateIV();
            
            ICryptoTransform transform = aes.CreateEncryptor();

            using MemoryStream ms = new MemoryStream();
            using CryptoStream cs = new(ms, transform, CryptoStreamMode.Write);
            using StreamWriter sw = new(cs);

            sw.Write(data);
            sw.Flush();
            cs.FlushFinalBlock();
            byte[] array = ms.ToArray();
            
            string siv = Convert.ToBase64String(aes.IV);
            string dataPart = Convert.ToBase64String(array);

            return $"{siv}.{dataPart}";
        }


        public static string Decrypt(this string input, string key)
        {
            string[] parts = input.Split('.');
            if (parts.Length != 2)
                throw new ArgumentException("Decryption input format incorrect.");

            byte[] iv = Convert.FromBase64String(parts[0]);
            byte[] data = Convert.FromBase64String(parts[1]);

            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;

            ICryptoTransform transform = aes.CreateDecryptor();

            using MemoryStream ms = new MemoryStream(data);
            using CryptoStream cs = new(ms, transform, CryptoStreamMode.Read);
            using StreamReader sr = new StreamReader(cs);
            
            return sr.ReadToEnd();
        }

        public static string EncryptIfKeyProvided(this string data, string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
                return data.Encrypt(key);
            return data;
        }

        public static string DecryptIfKeyProvided(this string data, string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
                return data.Decrypt(key);
            return data;
        }
    }
}
