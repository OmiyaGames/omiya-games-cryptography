using UnityEditor;
using UnityEditor.AnimatedValues;
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
        private AnimBool encryptionGroup, decryptionGroup;
        private string testEncryption, testDecryption;

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
        //public override void OnInspectorGUI()
        //{
        //    // Update the serialized object
        //    serializedObject.Update();

        //    // Display all fields
        //    EditorGUILayout.PropertyField(passwordHash);
        //    EditorGUILayout.PropertyField(saltKey);
        //    EditorGUILayout.PropertyField(ivKey);

        //    // Display a button to randomize all fields
        //    EditorGUILayout.Space();
        //    if (GUILayout.Button("Randomize all fields") == true)
        //    {
        //        passwordHash.stringValue = StringCryptographer.GetRandomPassword(StringCryptographer.DefaultPasswordLength);
        //        saltKey.stringValue = StringCryptographer.GetRandomPassword(StringCryptographer.DefaultPasswordLength);
        //        ivKey.stringValue = StringCryptographer.GetRandomPassword(StringCryptographer.IvKeyBlockSize);
        //    }

        //    // Display test encryption
        //    EditorGUILayout.Space();
        //    EditorHelpers.DrawBoldFoldout(encryptionGroup, "Test Encryption");
        //    using (EditorGUILayout.FadeGroupScope scope = new EditorGUILayout.FadeGroupScope(encryptionGroup.faded))
        //    {
        //        if (scope.visible == true)
        //        {
        //            testEncryption = EditorGUILayout.DelayedTextField("Input", testEncryption);
        //            string output = null;
        //            if (string.IsNullOrEmpty(testEncryption) == false)
        //            {
        //                output = ((StringCryptographer)target).Encrypt(testEncryption);
        //            }
        //            EditorGUILayout.TextField("Output", output);
        //        }
        //    }

        //    // Display test decryption
        //    EditorGUILayout.Space();
        //    EditorHelpers.DrawBoldFoldout(decryptionGroup, "Test Decryption");
        //    using (EditorGUILayout.FadeGroupScope scope = new EditorGUILayout.FadeGroupScope(decryptionGroup.faded))
        //    {
        //        if (scope.visible == true)
        //        {
        //            testDecryption = EditorGUILayout.DelayedTextField("Input", testDecryption);
        //            string output = null;
        //            if (string.IsNullOrEmpty(testDecryption) == false)
        //            {
        //                output = ((StringCryptographer)target).Decrypt(testDecryption);
        //            }
        //            EditorGUILayout.TextField("Output", output);
        //        }
        //    }

        //    // Apply modifications
        //    serializedObject.ApplyModifiedProperties();
        //}

        private void OnEnable()
        {
            // Grab all properties
            passwordHash = serializedObject.FindProperty("passwordHash");
            saltKey = serializedObject.FindProperty("saltKey");
            ivKey = serializedObject.FindProperty("ivKey");

            // Setup the animations
            encryptionGroup = new AnimBool(false, Repaint);
            decryptionGroup = new AnimBool(false, Repaint);
        }

        public override VisualElement CreateInspectorGUI()
        {
            // Each editor window contains a root VisualElement object
            var container = new VisualElement();

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.omiyagames.cryptography/Editor/StringCryptographyUxml.uxml");
            VisualElement labelFromUXML = visualTree.CloneTree();
            container.Add(labelFromUXML);

            return container;
        }
    }
}
