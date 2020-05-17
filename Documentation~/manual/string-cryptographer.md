# String Cryptographer

String Cryptographer is an asset that encrypts and decrypts texts.

### Creating the asset

To create one, simply click on the "Asset" file menu and select "Create -> Omiya Games -> String Cryptographer."

![Context Menu](https://omiyagames.github.io/omiya-games-cryptography/resources/string-cryptographer/context-menu.png)

Alternatiely, one can bring up the same context menu via clicking the plus button on the upper-left hand corner of the Project window, or right-click within the Project window.

In doing so, the Project window will prompt the user to enter a file name. Rename the file to your liking, and hit enter to confirm.

![Enter File Name](https://omiyagames.github.io/omiya-games-cryptography/resources/string-cryptographer/enter-file-name.png)

At this point, the asset is ready to use! All the fields are randomly generated, so each new String Cryptographer created should be unique.

### Editing the asset

The passwords held in the file can be customized by clicking on the asset in the Project window, and making the edits in the Inspector window. Alternatively, click on "Randomize all fields" button to replace all fields with a new random string.

![Inspector](https://omiyagames.github.io/omiya-games-cryptography/resources/string-cryptographer/inspector.png)

Don't forget to save the project after making edits to the file, so the changes actually gets written in the file.

![Save Project](https://omiyagames.github.io/omiya-games-cryptography/resources/string-cryptographer/save-project.png)

### Testing the asset

The inspector provides the user a chance to test how well the String Cryptographer encrypts and decrypts in the two groups of fields at the bottom foldouts. Simply click on the `Test Encryption` and/or `Test Decryption` to expand them.

![Asset Test Fields](https://omiyagames.github.io/omiya-games-cryptography/resources/string-cryptographer/test-asset.png)

For testing encryption, simply enter your text in the input field, and hit enter. The output field will contain the encrypted result. Similarly, for decryption, enter the encrypted text in the input field to get the decrypted text in the output field. Do note that text in the output fields can be copied onto the clipboard (e.g. by tapping ctrl+V on Windows and Linux OS).

## Using the asset in a script

Utilizing String Cryptographer in scripts is incredibly easy. Once simply needs to add a member variable exposed to the inspector to start utilizing the asset:

```csharp
using UnityEngine;

// Don't forget to add this "using" to support StringCryptographer
using OmiyaGames.Cryptography;

public class SampleStringCryptographer : MonoBehaviour
{
    // [SerializeField] Exposes private variables to the inspector
    [SerializeField]
    private StringCryptographer encrypter;
```

Remember to, after attaching this script to a GameObject, drag-and-drop a String Cryptographer asset to the inspector field.

![Script Inspector](https://omiyagames.github.io/omiya-games-cryptography/resources/string-cryptographer/script.png)

From there, to encrypt a string, simply use the `Encrypt(string)` method:

```csharp
// Encrypt the text
string encryptedText = encrypter.Encrypt(text);

// Print on the console
Debug.Log(text + " encrypted is: " + encryptedText);
```

And of course, to decrypt an encrypted string, simply use the `Decrypt(string)` method:

```csharp
// Decrypt the text
string decryptedText = encrypter.Decrypt(encryptedText);

// Print on the console
Debug.Log(encryptedText + " decrypted is: " + decryptedText);
```

Full example below:
```csharp
using UnityEngine;

// Don't forget to add this "using" to support StringCryptographer
using OmiyaGames.Cryptography;

public class SampleStringCryptographer : MonoBehaviour
{
    // [SerializeField] Exposes private variables to the inspector
    [SerializeField]
    private StringCryptographer encrypter;
    [SerializeField]
    private string text;

    // Start is called before the first frame update
    void Start()
    {
        // Encrypt the text
        string encryptedText = encrypter.Encrypt(text);

        // Print on the console
        Debug.Log(text + " encrypted is: " + encryptedText);

        // Decrypt the text
        string decryptedText = encrypter.Decrypt(encryptedText);

        // Print on the console
        Debug.Log(encryptedText + " decrypted is: " + decryptedText);
    }
}
```

As String Cryptographer is a [`ScriptableObject`](https://docs.unity3d.com/ScriptReference/ScriptableObject.html), it can be constructed within a script as well:

```csharp
readonly StringCryptographer encrypter = new StringCryptographer("Password123 - Also my briefcase code...", "Salt key", "IV Key");

// Encrypt the text
string encryptedText = encrypter.Encrypt("Text to encrypt and decrypt");

// Print on the console
Debug.Log(text + " encrypted is: " + encryptedText);

// Decrypt the text
string decryptedText = encrypter.Decrypt(encryptedText);

// Print on the console
Debug.Log(encryptedText + " decrypted is: " + decryptedText);
```

## Additional Resources

- [`StringCryptographer.cs` API documentation](https://omiyagames.github.io/omiya-games-cryptography/api/OmiyaGames.Cryptography.StringCryptographer.html)
- [`StringCryptographer.cs` source code](https://github.com/OmiyaGames/omiya-games-cryptography/blob/master/Runtime/StringCryptographer.cs)
- [`StringCryptographyEditor.cs` source code (generates the inspector editor)](https://github.com/OmiyaGames/omiya-games-cryptography/blob/master/Editor/StringCryptographyEditor.cs)
