# Resources

This package provides the following cyrptography tools:

### [String Cryptographer](/Documentation~/StringCryptographer.md)

An asset that encrypts and decrypts texts.

![Inspector](/Documentation~/images/stringCryptographer/inspector.png)

Using the asset in script is super-easy:

```csharp
// Create a new cryptographer with random password, hash key, etc.
// StringCryptographer can also be an inspector variable.
StringCryptographer encrypter = new StringCryptographer();

// Encrypt the text
string encryptedText = encrypter.Encrypt(text);

// Print on the console
Debug.Log(text + " encrypted is: " + encryptedText);
```

For more details, check out the references below:
- [Documentation](/Documentation~/StringCryptographer.md)
- [Doxygen-generated doc](/Documentation~/html/class_omiya_games_1_1_cryptography_1_1_string_cryptographer.html)
- [Source code](/Runtime/StringCryptographer.cs)

### [Domain List](/Documentation~/DomainList.md)

Domain List is a binary file that stores a list of strings, such as a list of acceptable web host domains. This package provides a dialog to create, read, and edit these files.

![Default Window](/Documentation~/images/domainList/defaultWindow.png)

For security reasons, typical read operation of a Domain List is a bit more involved. For more details, check out the references below:
- [Documentation](/Documentation~/DomainList.md)
- [Doxygen-generated doc](/Documentation~/html/class_omiya_games_1_1_cryptography_1_1_domain_list.html)
- [Source code](/Runtime/DomainList.cs)

## Doxygen-Generated Source Code Documentation

A Doxygen-generated documentation based on the source code for each tool is available at:
[Documentation~/html/annotated.html](/Documentation~/html/annotated.html)
