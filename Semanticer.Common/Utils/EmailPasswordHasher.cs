using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Semanticer.Common.Utils
{
    /// <summary>
    /// Klasa do hashowania podanych przez użytkownika haseł
    /// </summary>
    public static class EmailPasswordHasher
    {
        private static readonly byte[] key = new byte[16] { 164, 134, 36, 246, 235, 15, 9, 218, 169, 78, 155, 78, 23, 147, 201, 3 };

        /// <summary>
        /// Podstawowa metoda hashująca
        /// </summary>
        /// <param name="password"> Hasło do zhashowania </param>
        /// <returns> Tablica bajtów hasha </returns>
        public static byte[] HashPassword(string password)
        {
            byte[] passwordData = Encoding.ASCII.GetBytes(password);
            passwordData = new SHA256Managed().ComputeHash(passwordData);
            return passwordData;
        }

        /// <summary>
        /// Metoda hashująca z dodatkiem wybranej soli
        /// </summary>
        /// <param name="password"> Hasło do zhashowania </param>
        /// <param name="salt"> Sól </param>
        /// <returns> Tablica bajtów hasha </returns>
        public static byte[] HashPasswordWithSalt(string password, string salt)
        {
            byte[] saltData = Encoding.ASCII.GetBytes(salt);
            byte[] passwordData = Encoding.ASCII.GetBytes(password);
            byte[] combinedData = new byte[passwordData.Length + saltData.Length];
            Buffer.BlockCopy(passwordData, 0, combinedData, 0, passwordData.Length);
            Buffer.BlockCopy(saltData, 0, combinedData, passwordData.Length, saltData.Length);
            combinedData = new SHA256Managed().ComputeHash(combinedData);
            byte[] hashedPassword = new byte[combinedData.Length + saltData.Length];
            Buffer.BlockCopy(combinedData, 0, hashedPassword, 0, combinedData.Length);
            Buffer.BlockCopy(saltData, 0, hashedPassword, combinedData.Length, saltData.Length);
            return hashedPassword;
        }

        /// <summary>
        /// Metoda szyfrująca hasło do przechowania w bazie, używając algorytmu AES i podanego klucza
        /// </summary>
        /// <param name="password"> Hasło do zaszyfrowania </param>
        /// <param name="key"> Klucz do zaszyfrowania hasła </param>
        /// <returns> Ciąg zaszyfrowanych bajtów </returns>
        public static byte[] EncryptPassword(string password)
        {
            AesManaged aesEncryptor = new AesManaged();
            aesEncryptor.Key = key;
            aesEncryptor.IV = key;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] passwordData = UTF8Encoding.UTF8.GetBytes(password);
                    cryptoStream.Write(passwordData, 0, passwordData.Length);
                    cryptoStream.FlushFinalBlock();
                    cryptoStream.Close();
                    return memoryStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Metoda deszyfrująca hasło wyciągnięte z bazy, za pomocą algorytmu AES i podanego klucza
        /// </summary>
        /// <param name="encryptedPassword"> Ciąg zaszyfrowanych bajtów hasła </param>
        /// <param name="key"> Klucz do deszyfracji hasła </param>
        /// <returns> Odszyfrowane hasło </returns>
        public static string DecryptPassword(byte[] encryptedPassword)
        {
            AesManaged aesDecryptor = new AesManaged();
            aesDecryptor.Key = key;
            aesDecryptor.IV = key;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(encryptedPassword, 0, encryptedPassword.Length);
                    cryptoStream.Flush();
                    cryptoStream.Close();
                    byte[] passwordData = memoryStream.ToArray();
                    return UTF8Encoding.UTF8.GetString(passwordData, 0, passwordData.Length);
                }
            }
        }
    }
}
