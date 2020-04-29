# Documentation

This package provides the following cyrptography tools:

### [String Cryptographer](/Documentation~/StringCryptographer.md)

An asset that encrypts and decrypts texts. Using the asset in script is super-easy:

```csharp
// Encrypt the text
string encryptedText = encrypter.Encrypt(text);

// Print on the console
Debug.Log(text + " encrypted is: " + encryptedText);
```

As a ScriptableObject, it can be constructed within a script as well.

For more details, check out the references below:
- [Documentation](/Documentation~/StringCryptographer.md)
- [Doxygen-generated doc](/Documentation~/html/class_omiya_games_1_1_cryptography_1_1_string_cryptographer.html)
- [Source code](/Runtime/StringCryptographer.cs)

### [Domain List](/Documentation~/DomainList.md)

An AssetBundle that stores a list of encrypted strings, such as a list of acceptable web host domains. This package provides a dialog to create, read, and edit these files.

For more details, check out the references below:
- [Documentation](/Documentation~/DomainList.md)
- [Source code](/Runtime/DomainList.cs)

## Doxygen-Generated Source Code Documentation

A Doxygen-generated documentation based on the source code for each tool is available at:
[Documentation~/html/annotated.html](/Documentation~/html/annotated.html)
