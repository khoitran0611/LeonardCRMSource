using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Eli.Common
{
    /// <summary>
    /// This class helps to encrypt and decrypt a string
    /// </summary>
    public static class SecurityHelper
    {
        /// <summary>
        /// Encrypt a string to Base64 string
        /// </summary>
        /// <param name="textToBeEncrypted">The input string to encrypt</param>
        /// <returns></returns>
        public static string Encrypt(string textToBeEncrypted)
        {
            var rijndaelCipher = new RijndaelManaged();
            const string password = "abc!@#$%^&*!xyz";
            var plainText = System.Text.Encoding.Unicode.GetBytes(textToBeEncrypted);
            var salt = Encoding.ASCII.GetBytes(password.Length.ToString());
            var secretKey = new PasswordDeriveBytes(password, salt);
            //Creates a symmetric encryptor object. 
            var encryptor = rijndaelCipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));
            var memoryStream = new MemoryStream();
            //Defines a stream that links data streams to cryptographic transformations
            var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainText, 0, plainText.Length);
            //Writes the final state and clears the buffer
            cryptoStream.FlushFinalBlock();
            var cipherBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            var encryptedData = Convert.ToBase64String(cipherBytes);

            return encryptedData;
        }

        /// <summary>
        /// Decrypt an encrypted string back to plain text
        /// </summary>
        /// <param name="textToBeDecrypted">The encrypted string</param>
        /// <returns></returns>
        public static string Decrypt(string textToBeDecrypted)
        {
            var rijndaelCipher = new RijndaelManaged();

            const string password = "abc!@#$%^&*!xyz";
            string decryptedData;

            try
            {
                var encryptedData = Convert.FromBase64String(textToBeDecrypted);

                var salt = Encoding.ASCII.GetBytes(password.Length.ToString());
                //Making of the key for decryption
                var secretKey = new PasswordDeriveBytes(password, salt);
                //Creates a symmetric Rijndael decryptor object.
                var decryptor = rijndaelCipher.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));

                var memoryStream = new MemoryStream(encryptedData);
                //Defines the cryptographics stream for decryption.THe stream contains decrpted data
                var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                var plainText = new byte[encryptedData.Length];
                var decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);
                memoryStream.Close();
                cryptoStream.Close();

                //Converting to string
                decryptedData = Encoding.Unicode.GetString(plainText, 0, decryptedCount);
            }
            catch
            {
                decryptedData = textToBeDecrypted;
            }
            return decryptedData;
        }
    }
}
