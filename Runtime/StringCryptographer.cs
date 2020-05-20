using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace OmiyaGames.Cryptography
{
    ///-----------------------------------------------------------------------
    /// <remarks>
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
    /// <list type="table">
    /// <listheader>
    /// <term>Revision</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>
    /// <strong>Version:</strong> 0.0.0-preview.1<br/>
    /// <strong>Date:</strong> 2/11/2019<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Initial verison.</description>
    /// </item>
    /// <item>
    /// <term>
    /// <strong>Version:</strong> 0.1.0-preview.1<br/>
    /// <strong>Date:</strong> 4/3/2020<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Converted the class to a package.</description>
    /// </item>
    /// <item>
    /// <term>
    /// <strong>Version:</strong> 0.2.2-preview.1<br/>
    /// <strong>Date:</strong> 5/19/2020<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Fixing license documentation to be more DocFX friendly.</description>
    /// </item>
    /// </list>
    /// </remarks>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// <see cref="ScriptableObject"/> that can encrypt or decrypt strings.
    /// Taken directly from
    /// <a href="https://social.msdn.microsoft.com/Forums/vstudio/en-US/d6a2836a-d587-4068-8630-94f4fb2a2aeb/encrypt-and-decrypt-a-string-in-c?forum=csharpgeneral">
    /// this forum post by Kris444.
    /// </a>
    /// <seealso cref="RijndaelManaged"/>
    /// <seealso cref="RNGCryptoServiceProvider"/>
    /// </summary>
    public class StringCryptographer : ScriptableObject
    {
        public const int DefaultPasswordLength = 32;
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

        /// <summary>
        /// Generates a <see cref="StringCryptographer"/> with
        /// all fields set.
        /// </summary>
        /// <param name="passwordHash">Sets <see cref="PasswordHash"/></param>
        /// <param name="saltKey">Sets <see cref="SaltKey"/></param>
        /// <param name="ivKey">Sets <see cref="IvKey"/></param>
        public StringCryptographer(string passwordHash, string saltKey, string ivKey)
        {
            PasswordHash = passwordHash;
            SaltKey = saltKey;
            IvKey = ivKey;
        }

        /// <summary>
        /// Generates a <see cref="StringCryptographer"/> with <see cref="PasswordHash"/> and <see cref="SaltKey"/> set.
        /// All other fields are randomized using <see cref="GetRandomPassword(int, string)"/>.
        /// </summary>
        /// <param name="passwordHash">Sets <see cref="PasswordHash"/></param>
        /// <param name="saltKey">Sets <see cref="SaltKey"/></param>
        public StringCryptographer(string passwordHash, string saltKey) : this(passwordHash , saltKey , GetRandomPassword(IvKeyBlockSize))
        { }

        /// <summary>
        /// Generates a <see cref="StringCryptographer"/> with <see cref="PasswordHash"/> set.
        /// All other fields are randomized using <see cref="GetRandomPassword(int, string)"/>.
        /// </summary>
        /// <param name="passwordHash">Sets <see cref="PasswordHash"/></param>
        /// <param name="saltKeyLength">Length of <see cref="SaltKey"/></param>
        public StringCryptographer(string passwordHash, int saltKeyLength) : this(passwordHash, GetRandomPassword(saltKeyLength))
        { }

        /// <summary>
        /// Generates a <see cref="StringCryptographer"/> with <see cref="PasswordHash"/> set.
        /// All other fields are randomized using <see cref="GetRandomPassword(int, string)"/>.
        /// </summary>
        /// <param name="passwordHash">Sets <see cref="PasswordHash"/></param>
        public StringCryptographer(string passwordHash) : this(passwordHash, DefaultPasswordLength)
        { }

        /// <summary>
        /// Generates a <see cref="StringCryptographer"/> with
        /// a randomized value for every field, using <see cref="GetRandomPassword(int, string)"/>.
        /// </summary>
        /// <param name="passwordLength">The string length of <see cref="PasswordHash"/> and <see cref="SaltKey"/></param>
        public StringCryptographer(int passwordLength) : this(GetRandomPassword(passwordLength))
        { }

        /// <summary>
        /// Generates a <see cref="StringCryptographer"/> with
        /// a randomized value for every field, using <see cref="GetRandomPassword(int, string)"/>.
        /// </summary>
        public StringCryptographer() : this(DefaultPasswordLength)
        {  }

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
