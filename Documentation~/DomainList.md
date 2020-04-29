# Domain List

Domain List is a binary file that stores a list of strings, such as a list of acceptable web host domains.

### Pre-requisites

While technically optional, it's highly recommended to create and edit a Domain List with an associating String Cryptographer.  In doing so, all the stored strings will be enrypted, making it difficult to edit the Domain List from external tools: details on how to create one is [documented here](/Documentation~/StringCryptographer.md).  For security reasons, Domain List does *not* store a copy of the String Cryptographer.

### Opening Domain List window

Creating and editing Domain List is rather unusual. Since it's intentionally designed to be difficult to read by any other application, this package provides a dialog box to step through the process of creating one, than using the "Asset -> Create" context menu. To open this window, in the file menu bar, select "Windows -> Omiya Games -> Domain List."

![Context Menu](/Documentation~/images/domainList/contextMenu.png)

This should make the following window pop-up.

![Default Window](/Documentation~/images/domainList/defaultWindow.png)

### Creating the asset

TODO: Fill in process of creating an asset below!

Note: the process of making this simpler is being investigated.  See: [Github Issue #5](https://github.com/OmiyaGames/omiya-games-cryptography/issues/5)

### Editing the asset

TODO: Fill in process of editing an asset below!

Note: the process of making this simpler is being investigated.  See: [Github Issue #5](https://github.com/OmiyaGames/omiya-games-cryptography/issues/5)

## Additional Resources

- [Doxygen-generated doc](/Documentation~/html/class_omiya_games_1_1_cryptography_1_1_domain_list.html)
- [`DomainList.cs` source code](/Runtime/DomainList.cs)
- [`DomainListAssetBundleGenerator.cs` source code (the dialog box)](/Editor/DomainListAssetBundleGenerator.cs)
