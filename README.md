# [Omiya Games](https://www.omiyagames.com/) - Cryptography

[![openupm](https://img.shields.io/npm/v/com.omiyagames.cryptography?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.omiyagames.cryptography/) [![ko-fi](https://www.ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/I3I51KS8F)

A collection of tools to encrypt and decrypt various things. This includes:

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
- [Doxygen-generated doc](/Documentation~/html/class_omiya_games_1_1_cryptography_1_1_domain_list.html)
- [Source code](/Runtime/DomainList.cs)

## Install

Can be installed via [OpenUPM](https://openupm.com/) or Unity's own Package Manager with the Github URL.  Install the former with `npm install -g openupm-cli`, then run.

```
openupm add com.omiyagames.cryptography
```

## Documentation

Full documentation is available at the [`Documentation~`](/Documentation~/Cryptography.md) directory. For changes made between versions, check out the [`CHANGELOG.md`](/CHANGELOG.md).

## LICENSE

Overall package is licensed under [MIT](/LICENSE.md), unless otherwise noted in the [3rd party licenses](/THIRD%20PARTY%20NOTICES.md) file and/or source code.

Copyright (c) 2016-2020 Omiya Games
