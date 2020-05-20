using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OmiyaGames.Common.Editor;

namespace OmiyaGames.Cryptography.Editor
{
    ///-----------------------------------------------------------------------
    /// <remarks>
    /// <copyright file="DomainListAssetBundleGenerator.cs" company="Omiya Games">
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
    /// <list type="table">
    /// <listheader>
    /// <term>Revision</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>
    /// <strong>Version:</strong> 0.0.0-preview.1<br/>
    /// <strong>Date:</strong> 5/14/2016<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Initial verison.</description>
    /// </item>
    /// <item>
    /// <term>
    /// <strong>Version:</strong> 0.2.0-preview.1<br/>
    /// <strong>Date:</strong> 4/27/2020<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Converting to package.</description>
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
    /// Creates a window that, after pushing the "Generate Domain List Asset," creates
    /// an Asset Bundle containing an instance of <see cref="DomainList"/>.
    /// <seealso cref="DomainList"/>
    /// </summary>
    public class DomainListAssetBundleGenerator : EditorWindow
    {
        const string BundleId = "domains";
        const string HelpMessage = "What is a Domain List?\n\n" +
            "It's an Asset Bundle containing a ScriptableObject, containing a list of" +
            " strings, optionally encrypted with String Cryptographer. Since Asset Bundles" +
            " are serialized, and thus, harder to edit, it's a more secure method of" +
            " storing text, especially if encrypted. Intended to be used in conjunction" +
            " with security features, such as a script verifying the web domain a" +
            " Unity WebGL game is hosted on by downloading and reading a list from a file.";
        const string TestErrorInvalidFileMessage =
            "Unable to read file. Is it really an Asset Bundle?";
        const string TestErrorInvalidAssetMessage = "Was able to read the Asset Bundle," +
            " but the asset contained in it was not a DomainList ScriptableObject.";
        const string TestEmptyWarningMessage = "Was able to read the Asset Bundle, but" +
            " the asset contained in it had no content.";
        const string TestInfoMessage = "Asset Bundle contains the following domains:";
        const string EditMessage = "Updated information in the section below. " +
            "Just edit the details, and click \"Generate\"!";

        string nameOfFile = BundleId, nameOfFolder = "Assets", testResult = null;
        MessageType testResultType = MessageType.None;
        bool toggleGenerateArea = true, toggleTestArea = false;
        Object testAsset = null, lastTestAsset = null;
        List<string> allDomains = new List<string> { "localhost", "build.cloud.unity3d.com" };
        ReorderableList allDomainsField = null;
        StringCryptographer encrypter = null;
        readonly StringBuilder builder = new StringBuilder();

        GUIStyle FoldOutstyle
        {
            get
            {
                return EditorStyles.foldout;
            }
        }

        [MenuItem("Window/Omiya Games/Domain List")]
        static void Initialize()
        {
            DomainListAssetBundleGenerator window = GetWindow<DomainListAssetBundleGenerator>(title: "Domain List");
            window.Show();
        }

        /// <summary>
        /// Generates a new <see cref="AssetBundle"/> file containing a single <see cref="DomainList"/>.
        /// </summary>
        /// <param name="nameOfFolder">Folder to generate the new file.</param>
        /// <param name="nameOfFile">Name of the file to generate.</param>
        /// <param name="allDomains">The list of strings to store in the <see cref="DomainList"/>.</param>
        /// <param name="encrypter">Encrypts the content of the generated <see cref="DomainList"/>.</param>
        /// <param name="relativeToProject">If true, nameOfFolder will be relative to the Unity project root folder.</param>
        /// <param name="overwriteFile">If true, allows overwriting the file without user prompt.</param>
        /// <returns>A newly generated <see cref="DomainList"/> the new file contains.</returns>
        public static DomainList GenerateDomainList(string nameOfFolder, string nameOfFile, IList<string> allDomains, StringCryptographer encrypter, bool relativeToProject = true, bool overwriteFile = false)
        {
            DomainList newAsset = DomainList.Generate(nameOfFile, allDomains, encrypter);

            // Create a temporary file name
            int tempFileId = Random.Range(0, 99999999);
            string tempFileName = BundleId + tempFileId.ToString("00000000");

            // Generate the asset bundle
            AssetHelpers.SaveAsAssetBundle(newAsset, nameOfFolder, nameOfFile, tempFileName, new StringBuilder(), relativeToProject, overwriteFile);
            return newAsset;
        }

        #region Unity Events
        void OnGUI()
        {
            // Explain what this dialog does
            EditorGUILayout.HelpBox(HelpMessage, MessageType.None);

            // Draw the encrypter
            EditorGUILayout.Space();
            encrypter = (StringCryptographer)EditorGUILayout.ObjectField("Encrypter", encrypter, typeof(StringCryptographer), false);
            EditorGUILayout.Space();

            // Draw the area for testing an asset bundle
            DrawTestAssetArea();

            // Put a space between generated and test area
            EditorGUILayout.Space();

            // Draw the area for generating Domain List Assets
            DrawGenerateAssetArea();
        }

        void OnEnable()
        {
            // Setup toggle
            toggleGenerateArea = true;
            toggleTestArea = false;

            // Setup Reordable list
            allDomainsField = new ReorderableList(allDomains, typeof(string), true, true, true, true);
            allDomainsField.drawHeaderCallback = DrawLevelListHeader;
            allDomainsField.drawElementCallback = DrawLevelListElement;
            allDomainsField.onAddCallback = OnAddDomain;
            allDomainsField.elementHeight = EditorHelpers.SingleLineHeight(EditorHelpers.VerticalMargin);
        }
        #endregion

        #region Reordable List Events
        void OnAddDomain(ReorderableList list)
        {
            string toAdd = string.Empty;
            if (allDomains.Count > 0)
            {
                if ((list.index >= 0) && (list.index < allDomains.Count))
                {
                    toAdd = allDomains[list.index];
                }
                else
                {
                    toAdd = allDomains[allDomains.Count - 1];
                }
            }
            allDomains.Add(toAdd);
        }

        void DrawLevelListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "All Accepted Domains");
        }

        void DrawLevelListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y += EditorHelpers.VerticalMargin;
            rect.height = EditorGUIUtility.singleLineHeight;
            allDomains[index] = EditorGUI.TextField(rect, GUIContent.none, allDomains[index]);
        }
        #endregion

        #region Helper Methods
        void DrawGenerateAssetArea()
        {
            // Draw foldout area
            EditorGUILayout.BeginVertical();
            toggleGenerateArea = EditorGUILayout.Foldout(toggleGenerateArea, "Generate Domain List Asset", FoldOutstyle);
            if (toggleGenerateArea == true)
            {
                // Ask the list of layouts
                allDomainsField.DoLayoutList();

                // Ask the path this will generate
                GUILayout.Label("Name of folder to generate this asset\n(relative to this project's folder):");
                nameOfFolder = EditorGUILayout.TextField(nameOfFolder);

                // Ask the name of the file this will generate
                GUILayout.Label("Name of asset to generate (no file extensions):");
                nameOfFile = EditorGUILayout.TextField(nameOfFile);

                // Create generate buttons
                if (GUILayout.Button("Generate Domain List Asset") == true)
                {
                    // Check if file already exists
                    string pathOfAsset = Path.Combine(nameOfFolder, nameOfFile);
                    if (AssetHelpers.ConfirmFileIsWriteable(pathOfAsset, nameOfFile) == true)
                    {
                        GenerateDomainList(nameOfFolder, nameOfFile, allDomains, encrypter);
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        void DrawTestAssetArea()
        {
            // Draw foldout area
            EditorGUILayout.BeginVertical();
            toggleTestArea = EditorGUILayout.Foldout(toggleTestArea, "Read Domain List Asset", FoldOutstyle);
            if (toggleTestArea == true)
            {
                // Create a field to set an object
                testAsset = EditorGUILayout.ObjectField(lastTestAsset, typeof(Object), false);
                if ((testAsset == null) || (testAsset != lastTestAsset))
                {
                    // Clear results
                    testResult = null;
                    testResultType = MessageType.None;
                }
                lastTestAsset = testAsset;

                // Create a generate button
                if (GUILayout.Button("Read Domain List Asset") == true)
                {
                    TestDomainAsset(testAsset, builder, encrypter, out testResult, out testResultType);
                }

                // Create a generate button
                if (GUILayout.Button("Edit Domain List Asset") == true)
                {
                    EditDomainAsset(testAsset, encrypter, out testResult, out testResultType);
                }

                // Print out results, if there are any
                if (string.IsNullOrEmpty(testResult) == false)
                {
                    EditorGUILayout.HelpBox(testResult, testResultType);
                }
            }
            EditorGUILayout.EndVertical();
        }

        static void TestDomainAsset(Object testAsset, StringBuilder builder, StringCryptographer decrypter, out string testResult, out MessageType testResultType)
        {
            // Setup variables
            AssetBundle bundle = null;
            testResult = TestErrorInvalidFileMessage;
            testResultType = MessageType.Error;

            // Null check
            if (testAsset == null)
            {
                // Don't do anything if asset is null
                return;
            }

            try
            {
                // Load the bundle, and convert it to a domain list
                bundle = AssetBundle.LoadFromFile(AssetDatabase.GetAssetPath(testAsset));
                DomainList domainList = DomainList.Get(bundle);

                // By default, indicate the bundle doesn't contain DomainList
                testResult = TestErrorInvalidAssetMessage;
                testResultType = MessageType.Error;

                // Check if the bundle contains an AcceptedDomainList
                if (domainList != null)
                {
                    // By default, indicate the domain list is empty
                    testResult = TestEmptyWarningMessage;
                    testResultType = MessageType.Warning;
                    if ((domainList != null) && (domainList.Count > 0))
                    {
                        // list out all the domains in the list
                        builder.Clear();
                        builder.Append(TestInfoMessage);
                        for (int index = 0; index < domainList.Count; ++index)
                        {
                            builder.AppendLine();
                            builder.Append("* \"");
                            if (decrypter != null)
                            {
                                builder.Append(decrypter.Decrypt(domainList[index]));
                            }
                            else
                            {
                                builder.Append(domainList[index]);
                            }
                            builder.Append("\"");
                        }
                        testResult = builder.ToString();
                        testResultType = MessageType.Info;
                    }
                }
            }
            finally
            {
                if (bundle != null)
                {
                    // Clean-up
                    bundle.Unload(true);
                }
            }
        }

        void EditDomainAsset(Object testAsset, StringCryptographer decrypter, out string testResult, out MessageType testResultType)
        {
            // Setup variables
            AssetBundle bundle = null;
            testResult = TestErrorInvalidFileMessage;
            testResultType = MessageType.Error;

            // Null check
            if (testAsset == null)
            {
                // Don't do anything if asset is null
                return;
            }

            try
            {
                // Check if we can get the path of the asset
                string localAssetPath = AssetDatabase.GetAssetPath(testAsset);

                // Load the bundle, and convert it to a domain list
                bundle = AssetBundle.LoadFromFile(localAssetPath);
                DomainList domainList = DomainList.Get(bundle);

                // By default, indicate the bundle doesn't contain DomainList
                testResult = TestErrorInvalidAssetMessage;
                testResultType = MessageType.Error;

                // Check if the bundle contains an AcceptedDomainList
                if (domainList != null)
                {
                    // By default, indicate the domain list is empty
                    testResult = TestEmptyWarningMessage;
                    testResultType = MessageType.Warning;
                    if ((domainList != null) && (domainList.Count > 0))
                    {
                        // Replace the name of file
                        nameOfFile = Path.GetFileName(localAssetPath);

                        // Replace the name of folder
                        int stringLengthOfFolder = (localAssetPath.Length - (nameOfFile.Length + 1));
                        nameOfFolder = localAssetPath.Substring(0, stringLengthOfFolder);

                        // Replace the domain list
                        DomainList.Decrypt(domainList, decrypter, ref allDomains);

                        testResult = EditMessage;
                        testResultType = MessageType.Info;
                    }
                }
            }
            finally
            {
                if (bundle != null)
                {
                    // Clean-up
                    bundle.Unload(true);
                }
            }
        }
        #endregion
    }
}
