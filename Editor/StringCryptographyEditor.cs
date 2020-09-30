using UnityEditor;
using UnityEngine;
using System.IO;
using OmiyaGames.Common.Editor;
using UnityEngine.UIElements;

namespace OmiyaGames.Cryptography.Editor
{
    ///-----------------------------------------------------------------------
    /// <remarks>
    /// <copyright file="StringCryptographyEditor.cs" company="Omiya Games">
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
    /// <item>
    /// <term>
    /// <strong>Version:</strong> 0.3.0-preview.1<br/>
    /// <strong>Date:</strong> 6/16/2020<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Converted editor to utilize UIElements. Redesigned the UI a bit.</description>
    /// </item>
    /// </list>
    /// </remarks>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// An editor script for <see cref="StringCryptographer"/>.
    /// <seealso cref="StringCryptographer"/>
    /// </summary>
    [CustomEditor(typeof(StringCryptographer), true)]
    public class StringCryptographyEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Default file name when creating a new <see cref="StringCryptographer"/>.
        /// </summary>
        public const string DefaultFileName = "New Cryptographer" + Helpers.FileExtensionScriptableObject;

        // Member variables
        private SerializedProperty passwordHash;
        private SerializedProperty saltKey;
        private SerializedProperty ivKey;
        private TextField testInputTextField;
        private TextField testOutputTextField;
        private Button copyToClipboardButton;

        /// <summary>
        /// Creates a new StringCryptographer instance in the Unity Project window.
        /// </summary>
        [MenuItem("Tools/Omiya Games/Create/String Cryptographer")]
        [MenuItem("Assets/Create/Omiya Games/String Cryptographer", priority = 203)]
        private static void CreateStringCryptographer()
        {
            // Setup asset
            StringCryptographer newAsset = CreateInstance<StringCryptographer>();

            // Setup path to file
            string folderName = AssetHelpers.GetSelectedFolder();
            string pathOfAsset = Path.Combine(folderName, DefaultFileName);
            pathOfAsset = AssetDatabase.GenerateUniqueAssetPath(pathOfAsset);

            // Create the asset, and prompt the user to rename it
            ProjectWindowUtil.CreateAsset(newAsset, pathOfAsset);
        }

        /// <inheritdoc/>
        private void OnEnable()
        {
            // Grab all properties
            passwordHash = serializedObject.FindProperty("passwordHash");
            saltKey = serializedObject.FindProperty("saltKey");
            ivKey = serializedObject.FindProperty("ivKey");
        }

        /// <inheritdoc/>
        public override VisualElement CreateInspectorGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement container = new VisualElement();

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.omiyagames.cryptography/Editor/StringCryptographyUxml.uxml");
            VisualElement fullTree = visualTree.CloneTree();
            container.Add(fullTree);

            // Compress the foldout by default
            Foldout testFoldout = fullTree.Query<Foldout>("TestFoldout").First();
            testFoldout.value = false;

            // Retrieve the test text fields into variables
            testInputTextField = fullTree.Query<TextField>("TestInput").First();
            testOutputTextField = fullTree.Query<TextField>("TestOutput").First();

            // Clear out the two fields
            testInputTextField.value = null;
            testOutputTextField.value = null;

            // Grab the RandomizeAll button, and bind it to a method
            Button button = fullTree.Query<Button>("RandomizeAll").First();
            button.clicked += OnRandomizeAllClicked;

            // Grab the Encrypt button, and bind it to a method
            button = fullTree.Query<Button>("EncryptButton").First();
            button.clicked += OnEncryptClicked;

            // Grab the Decrypt button, and bind it to a method
            button = fullTree.Query<Button>("DecryptButton").First();
            button.clicked += OnDecryptClicked;

            // Grab the CopyToClipboard button, and bind it to a method
            copyToClipboardButton = fullTree.Query<Button>("CopyToClipboard").First();
            copyToClipboardButton.clicked += OnCopyToClipboardClicked;
            copyToClipboardButton.SetEnabled(false);

            return container;
        }

        #region Button Events
        /// <summary>
        /// Randomzes all serialized fields.
        /// </summary>
        private void OnRandomizeAllClicked()
        {
            // Make sure all serialized properties are set
            if ((passwordHash != null) && (saltKey != null) && (ivKey != null))
            {
                // Randomize all fields
                passwordHash.stringValue = StringCryptographer.GetRandomPassword(StringCryptographer.DefaultPasswordLength);
                saltKey.stringValue = StringCryptographer.GetRandomPassword(StringCryptographer.DefaultPasswordLength);
                ivKey.stringValue = StringCryptographer.GetRandomPassword(StringCryptographer.IvKeyBlockSize);

                // Apply all modifications
                serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// Encrypts text in Input text field, and puts it in Output text field.
        /// </summary>
        private void OnEncryptClicked()
        {
            // Confirm the text fields exists
            if ((testInputTextField != null) && (testOutputTextField != null))
            {
                // Check to see if the input field has a value set
                string output = null;
                if (string.IsNullOrEmpty(testInputTextField.value) == false)
                {
                    // Run the encrypt function
                    output = testInputTextField.value;
                    output = ((StringCryptographer)target).Encrypt(output);
                }

                // Update output text field
                testOutputTextField.value = output;

                // Enable if copy to clipboard button based on output
                bool enableCopyToClipboard = !string.IsNullOrEmpty(output);
                copyToClipboardButton.SetEnabled(enableCopyToClipboard);
            }
        }

        /// <summary>
        /// Decrypts text in Input text field, and puts it in Output text field.
        /// </summary>
        private void OnDecryptClicked()
        {
            // Confirm the text fields exists
            if ((testInputTextField != null) && (testOutputTextField != null))
            {
                // Check to see if the input field has a value set
                string output = null;
                if (string.IsNullOrEmpty(testInputTextField.value) == false)
                {
                    // Run the decrypt function
                    output = testInputTextField.value;
                    output = ((StringCryptographer)target).Decrypt(output);
                }

                // Update output text field
                testOutputTextField.value = output;

                // Enable if copy to clipboard button based on output
                bool enableCopyToClipboard = !string.IsNullOrEmpty(output);
                copyToClipboardButton.SetEnabled(enableCopyToClipboard);
            }
        }

        /// <summary>
        /// Copies text in Output text field to the OS' clipboard
        /// </summary>
        private void OnCopyToClipboardClicked()
        {
            // Confirm output field has actual text in it
            if ((testOutputTextField != null) && (string.IsNullOrEmpty(testOutputTextField.value) == false))
            {
                // Set copy buffer to the output text field's text.
                GUIUtility.systemCopyBuffer = testOutputTextField.value;
            }
        }
        #endregion
    }
}
