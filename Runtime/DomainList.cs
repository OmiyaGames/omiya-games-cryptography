using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using OmiyaGames.Cryptography;

namespace OmiyaGames.Domain
{
    ///-----------------------------------------------------------------------
    /// <copyright file="DomainList.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2016-2020 Omiya Games
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
    /// <date>5/14/2016</date>
    /// <author>Taro Omiya</author>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// <see cref="ScriptableObject"/> that contains a list of strings, optionally encrypted by <see cref="StringCryptographer"/>.
    /// </summary>
    /// <remarks>
    /// Revision History:
    /// <list type="table">
    ///   <listheader>
    ///     <description>Date</description>
    ///     <description>Name</description>
    ///     <description>Description</description>
    ///   </listheader>
    ///   <item>
    ///     <description>5/14/2016</description>
    ///     <description>Taro</description>
    ///     <description>Initial version</description>
    ///   </item>
    ///   <item>
    ///     <description>4/27/2020</description>
    ///     <description>Taro</description>
    ///     <description>Converting to package.</description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="StringCryptographer"/>
    public class DomainList : ScriptableObject, ICollection<string>
    {
        /// <summary>
        /// List of domains.  Each string can contain '*' as a wildcard character.
        /// </summary>
        [SerializeField]
        string[] domains = null;

        #region Static Functions
        /// <summary>
        /// Constructs a new <see cref="DomainList"/>.
        /// </summary>
        /// <param name="name">Name of the asset.</param>
        /// <param name="allDomains">List of unencrypted strings to store in the asset.</param>
        /// <param name="encrypter">An optional cryptographer that encrypts allDomains.</param>
        /// <returns>A mew <see cref="DomainList"/>.</returns>
        public static DomainList Generate(string name, IList<string> allDomains, StringCryptographer encrypter = null)
        {
            // Verify all arguments
            if (string.IsNullOrWhiteSpace(name) == true)
            {
                throw new ArgumentNullException("name");
            }
            else if (allDomains == null)
            {
                throw new ArgumentNullException("allDomains");
            }

            // Copy over all the domain names
            string[] domains = new string[allDomains.Count];
            if (encrypter != null)
            {
                // Encrypt all entries
                for (int index = 0; index < allDomains.Count; ++index)
                {
                    domains[index] = encrypter.Encrypt(allDomains[index]);
                }
            }
            else
            {
                // Copy directly to the array
                allDomains.CopyTo(domains, 0);
            }

            // Setup asset
            DomainList newAsset = CreateInstance<DomainList>();
            newAsset.name = name;
            newAsset.Domains = domains;
            return newAsset;
        }

        /// <summary>
        /// Loads an asset from <see cref="AssetBundle"/> to a <see cref="DomainList"/>.
        /// </summary>
        /// <param name="bundle">Reference to an asset bundle.</param>
        /// <param name="assetNameNoFileExtension">(Optional) A name to a file in an asset bundle. If none is provided, the first file is converted.</param>
        /// <returns>If successful, a loaded <see cref="DomainList"/> asset.  Otherwise, null.</returns>
        public static DomainList Get(AssetBundle bundle, string assetNameNoFileExtension = null)
        {
            DomainList returnDomain = null;

            // Search for an *.asset file
            string[] allAssets = bundle.GetAllAssetNames();
            string firstAsset = null;
            if (allAssets != null)
            {
                for (int index = 0; index < allAssets.Length; ++index)
                {
                    if ((string.IsNullOrEmpty(allAssets[index]) == false) &&
                        (Path.GetExtension(allAssets[index]) == OmiyaGames.Helpers.FileExtensionScriptableObject) &&
                        ((string.IsNullOrEmpty(assetNameNoFileExtension) == true) || (Path.GetFileNameWithoutExtension(allAssets[index]) == assetNameNoFileExtension)))
                    {
                        firstAsset = allAssets[index];
                        break;
                    }
                }
            }

            // Check if an asset is found
            if (string.IsNullOrEmpty(firstAsset) == false)
            {
                try
                {
                    // Convert it to an AcceptedDomainList
                    returnDomain = bundle.LoadAsset<DomainList>(firstAsset);
                }
                catch (Exception)
                {
                    returnDomain = null;
                }
            }
            return returnDomain;
        }

        /// <summary>
        /// Converts a string used to match a domain to a case-insensitive, single-line regular expression. Supports '*' (matches any string) and '?' (matches any single character) wildcards, e.g. "*.google.com" and "?.google.com"
        /// </summary>
        /// <param name="domainString">A one-line string to match a domain.</param>
        /// <param name="buf">A <see cref="StringBuilder"/> to use as cache; its content will be completely replaced. If set to null, this function creates a new <see cref="StringBuilder"/>.</param>
        /// <returns>New <see cref="Regex"/> that matches a domain.</returns>
        public static Regex ConvertToRegex(string domainString, StringBuilder buf = null)
        {
            // Check if we need to create a new string builder
            if (buf == null)
            {
                buf = new StringBuilder(domainString.Length + 2);
            }
            // Reset StringBuilder
            buf.Clear();

            // Add forced start characters
            buf.Append('^');

            // Escape all Regex Expression, and
            // replace ? and * with equivalent symbols
            buf.Append(Regex.Escape(domainString).Replace("\\?", ".").Replace("\\*", ".*"));

            // Add forced end characters
            buf.Append('$');

            // Create a new Regex
            return new Regex(buf.ToString(), RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        /// <summary>
        /// Decrypts a <see cref="DomainList"/> into a string array.
        /// </summary>
        /// <param name="domainList">The <see cref="DomainList"/> to decrypt.</param>
        /// <param name="decrypter">The cryptographer to decrypt the <see cref="DomainList"/></param>
        /// <returns>Decrypted list of domains.</returns>
        public static string[] Decrypt(DomainList domainList, StringCryptographer decrypter = null)
        {
            string[] allDomains = new string[domainList.Count];
            if (decrypter != null)
            {
                for (int index = 0; index < allDomains.Length; ++index)
                {
                    allDomains[index] = decrypter.Decrypt(domainList[index]);
                }
            }
            else
            {
                for (int index = 0; index < allDomains.Length; ++index)
                {
                    allDomains[index] = domainList[index];
                }
            }
            return allDomains;
        }

        /// <summary>
        /// Clears a <see cref="List{string}"/>, then populates it with decrypted strings from a <see cref="DomainList"/>.
        /// </summary>
        /// <param name="domainList">The <see cref="DomainList"/> to decrypt.</param>
        /// <param name="decrypter">The cryptographer to decrypt the <see cref="DomainList"/>. Can be set to null, in which case the content of domainList is copied over directly.</param>
        /// <param name="decryptedDomains"><see cref="List{string}"/> that gets populated with decrypted strings.</param>
        public static void Decrypt(DomainList domainList, StringCryptographer decrypter, ref List<string> decryptedDomains)
        {
            decryptedDomains.Clear();
            if (decrypter != null)
            {
                foreach (string encryptedDomain in domainList)
                {
                    decryptedDomains.Add(decrypter.Decrypt(encryptedDomain));
                }
            }
            else
            {
                foreach (string domain in domainList)
                {
                    decryptedDomains.Add(domain);
                }
            }
        }

        /// <summary>
        /// Clears a <see cref="List{string}"/>, then populates it with decrypted <see cref="Regex"/> from a <see cref="DomainList"/>.
        /// </summary>
        /// <param name="domainList">The <see cref="DomainList"/> to decrypt.</param>
        /// <param name="decrypter">The cryptographer to decrypt the <see cref="DomainList"/> Can be set to null, in which case the content of domainList will be converted to <see cref="Regex"/> directly.</param>
        /// <param name="regularExpressions"><see cref="List{Regex}"/> that gets populated with domain-matching <see cref="Regex"/>.</param>
        /// <seealso cref="ConvertToRegex(string, StringBuilder)"/>
        public static void Decrypt(DomainList domainList, StringCryptographer decrypter, ref List<Regex> regularExpressions)
        {
            // Setup cache
            StringBuilder cacheBuffer = new StringBuilder();
            String domain;

            // Clear list
            regularExpressions.Clear();

            // Setup 
            foreach (string encryptedDomain in domainList)
            {
                domain = encryptedDomain;
                if (decrypter != null)
                {
                    domain = decrypter.Decrypt(encryptedDomain);
                }
                regularExpressions.Add(ConvertToRegex(domain, cacheBuffer));
            }
        }
        #endregion

        /// <summary>
        /// Property to access <see cref="domains"/>.
        /// For future-proofing purposes, use this property over the member variable.
        /// </summary>
        private string[] Domains
        {
            get => domains;
            set => domains = value;
        }

        /// <summary>
        /// Gets and sets a string in this list.
        /// </summary>
        public string this[int index]
        {
            get => Domains[index];
            set => Domains[index] = value;
        }

        /// <summary>
        /// The number of domains stored.
        /// </summary>
        public int Count
        {
            get => (Domains != null) ? Domains.Length : 0;
        }

        #region ICollection Implementation
        /// <summary>
        /// Always returns true.
        /// </summary>
        public bool IsReadOnly
        {
            get => true;
        }

        /// <summary>
        /// Copies the string domains into another array.
        /// </summary>
        /// <param name="array">Array to copy to.</param>
        /// <param name="arrayIndex">Index to start copying from in array.</param>
        public void CopyTo(string[] array, int arrayIndex)
        {
            Domains.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Provides an enumerator to go through each string entry in the list.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)Domains).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Domains.GetEnumerator();
        }

        /// <summary>
        /// Checks if a string is in the <see cref="DomainList"/>.
        /// </summary>
        /// <param name="item">String to search for.</param>
        /// <returns>True if item is in the <see cref="DomainList"/>.</returns>
        public bool Contains(string item)
        {
            bool returnFlag = false;
            for (int index = 0; index < Count; ++index)
            {
                if (Domains[index] == item)
                {
                    returnFlag = true;
                    break;
                }
            }
            return returnFlag;
        }
        #endregion

        #region Unimplemented methods
        /// <summary>
        /// *NOT* implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Always.
        /// </exception>
        public void Add(string item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// *NOT* implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Always.
        /// </exception>
        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// *NOT* implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Always.
        /// </exception>
        public bool Remove(string item)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
