using System;
using System.IO;
using System.Security.Cryptography;

namespace SqlToGraphiteInterfaces
{
    public class Encryption : IEncryption
    {
        private static PasswordDeriveBytes passwordDeriveBytes;

        private const string key = "!KJSHFSDOUREW192837SM.CXMV";

        public string Encrypt(string clearText)
        {
            passwordDeriveBytes = new PasswordDeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            var clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);
            var encryptedData = Encrypt(clearBytes, passwordDeriveBytes.GetBytes(32), passwordDeriveBytes.GetBytes(16));
            return Convert.ToBase64String(encryptedData);
        }

        // Decrypt a byte array into a byte array using a key and an IV 
        private byte[] Decrypt(byte[] cipherData, byte[] key, byte[] iv)
        {
            var ms = new MemoryStream();
            var alg = Rijndael.Create();
            alg.Key = key;
            alg.IV = iv;
            var cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherData, 0, cipherData.Length);
            cs.Close();
            byte[] decryptedData = ms.ToArray();
            ms.Close();
            return decryptedData;
        }

        public string Decrypt(string cipherText)
        {
            passwordDeriveBytes = new PasswordDeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] decryptedData = Decrypt(cipherBytes, passwordDeriveBytes.GetBytes(32), passwordDeriveBytes.GetBytes(16));
            return System.Text.Encoding.Unicode.GetString(decryptedData);
        }


        private byte[] Encrypt(byte[] clearData, byte[] key, byte[] iv)
        {
            // Create a MemoryStream to accept the encrypted bytes 
            var ms = new MemoryStream();
            var alg = Rijndael.Create();
            alg.Key = key;
            alg.IV = iv;
            var cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(clearData, 0, clearData.Length);
            cs.Close();
            byte[] encryptedData = ms.ToArray();
            ms.Close();
            return encryptedData;
        }
    }
}