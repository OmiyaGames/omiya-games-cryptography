using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace OmiyaGames.Cryptography
{
    ///-----------------------------------------------------------------------
    /// <copyright file="StringCryptographer.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2019-2020 Omiya Games
    /// 
    /// Permission is hereby granted, free of charge, to any person obtaining a copy
    /// of this software and associated documentation files (the "Software"), to deal
    /// in the Software without restriction, including without limitation the rights
    /// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    /// copies of the Software, and to permit persons to whom the Software is
    /// furnished to do so, subject to the following conditions:
    /// 
    /// The above copyright notice and this permission notice shall be included in
    /// all copies or substantial portions of the Software.
    /// 
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    /// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    /// THE SOFTWARE.
    /// </copyright>
    /// <date>2/11/2019</date>
    /// <author>Taro Omiya</author>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// <see cref="ScriptableObject"/> that can encrypt or decrypt strings.
    /// Taken directly from
    /// <a href="https://social.msdn.microsoft.com/Forums/vstudio/en-US/d6a2836a-d587-4068-8630-94f4fb2a2aeb/encrypt-and-decrypt-a-string-in-c?forum=csharpgeneral">
    /// this forum post by Kris444.
    /// </a>
    /// </summary>
    /// <seealso cref="RijndaelManaged"/>
    /// <seealso cref="RNGCryptoServiceProvider"/>
    /// <remarks>
    /// Revision History:
    /// <list type="table">
    ///   <listheader>
    ///     <description>Date</description>
    ///     <description>Name</description>
    ///     <description>Description</description>
    ///   </listheader>
    ///   <item>
    ///     <description>2/11/2019</description>
    ///     <description>Taro Omiya</description>
    ///     <description>Initial version</description>
    ///   </item>
    ///   <item>
    ///     <description>4/3/2020</description>
    ///     <description>Taro Omiya</description>
    ///     <description>Converted the class to a package. Fixing a typo in a property.</description>
    ///   </item>
    /// </list>
    /// </remarks>
    public class StringCryptographer : ScriptableObject
    {
        public const int IvKeyBlockSize = 18;
        public const string AlphaNumericChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        public const string AlphaNumericSymbolsChars = AlphaNumericChars + "!@#$%^&*-_+=?,.`~|(){}[]'\"\\/<>";

        [Header("Fields")]
        [SerializeField]
        private string passwordHash;
        [SerializeField]
        private string saltKey;
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("viKey")]
        private string ivKey;

        #region Properties
        /// <summary>
        /// A random hash used for  used for encryption.
        /// </summary>
        public string PasswordHash
        {
            private get => passwordHash;
            set => passwordHash = value;
        }

        /// <summary>
        /// A salt to make <see cref="PasswordHash"/> less predictable.
        /// </summary>
        public string SaltKey
        {
            private get => saltKey;
            set => saltKey = value;
        }

        /// <summary>
        /// Initialization Vector key, used for encryption.
        /// </summary>
        public string IvKey
        {
            private get => ivKey;
            set => ivKey = value;
        }
        #endregion

        /// <summary>
        /// Generates a random password using alphanumeric characters.
        /// Taken directly from
        /// <a href="https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings">
        /// this StackOverflow post by Eric J.</a>
        /// </summary>
        /// <param name="length">Length of the returned password</param>
        /// <returns>An alphnumeric password</returns>
        public static string GetRandomPassword(int length, string acceptableChars = AlphaNumericSymbolsChars)
        {
            byte[] data = new byte[length];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(length);
            foreach (byte b in data)
            {
                result.Append(acceptableChars[b % acceptableChars.Length]);
            }
            return result.ToString();
        }

        /// <summary>
        /// Encrypts a string.
        /// </summary>
        /// <param name="plainText">The string to encrypt.</param>
        /// <returns>plainText, encrypted.</returns>
        public string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.Zeros
            };
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(IvKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        /// <summary>
        /// Decrypts an encrypted string.
        /// </summary>
        /// <param name="encryptedText">String to decrypt.</param>
        /// <returns>Decrypted string.</returns>
        public string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None
            };

            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(IvKey));
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
    }
}
