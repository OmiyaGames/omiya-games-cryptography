# [Omiya Games](https://www.omiyagames.com/) - Cryptography

[![openupm](https://img.shields.io/npm/v/com.omiyagames.cryptography?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.omiyagames.cryptography/) [![Documentation](https://github.com/OmiyaGames/omiya-games-cryptography/workflows/Host%20DocFX%20Documentation/badge.svg)](https://omiyagames.github.io/omiya-games-cryptography/) [![Mirroring](https://github.com/OmiyaGames/omiya-games-cryptography/workflows/Mirroring/badge.svg)](https://bitbucket.org/OmiyaGames/omiya-games-cryptography) [![ko-fi](https://www.ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/I3I51KS8F)

A collection of tools to encrypt and decrypt various things. This includes:

### [String Cryptographer](https://omiyagames.github.io/omiya-games-cryptography/manual/string-cryptographer.html)

An asset that encrypts and decrypts texts.

![Inspector](https://omiyagames.github.io/omiya-games-cryptography/resources/string-cryptographer/inspector.png)

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
- [Documentation](https://omiyagames.github.io/omiya-games-cryptography/manual/string-cryptographer.html)
- [Source code](/Runtime/StringCryptographer.cs)

### [Domain List](https://omiyagames.github.io/omiya-games-cryptography/manual/domain-list.html)

Domain List is a binary file that stores a list of strings, such as a list of acceptable web host domains. This package provides a dialog to create, read, and edit these files.

![Default Window](https://omiyagames.github.io/omiya-games-cryptography/resources/domain-list/default-window.png)

For security reasons, typical read operation of a Domain List is a bit more involved. For more details, check out the references below:
- [Documentation](https://omiyagames.github.io/omiya-games-cryptography/manual/domain-list.html)
- [Source code](/Runtime/DomainList.cs)

## Install

Can be installed via [OpenUPM](https://openupm.com/) or Unity's own Package Manager with the Github URL.  Install the former with `npm install -g openupm-cli`, then run.

```
openupm add com.omiyagames.cryptography
```

## Documentation

Full documentation is available at the [`Documentation~`](https://omiyagames.github.io/omiya-games-cryptography/) directory. For changes made between versions, check out the [`CHANGELOG.md`](https://omiyagames.github.io/omiya-games-cryptography/manual/changelog.html).

## LICENSE

Overall package is licensed under [MIT](/LICENSE.md), unless otherwise noted in the [3rd party licenses](/THIRD%20PARTY%20NOTICES.md) file and/or source code.

Copyright (c) 2016-2020 Omiya Games
