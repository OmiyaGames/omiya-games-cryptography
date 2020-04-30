# Domain List

Domain List is a binary file that stores a list of strings, such as a list of acceptable web host domains.

### Pre-requisites

While technically optional, it's highly recommended to create and edit a Domain List with an associating String Cryptographer.  In doing so, all the stored strings will be enrypted, making it difficult to edit the Domain List from external tools: details on how to create one is [documented here](/Documentation~/StringCryptographer.md).  For security reasons, Domain List does *not* store a copy of the String Cryptographer.

### Opening Domain List window

Creating and editing Domain List is rather unusual. Since it's intentionally designed to be difficult to read by any other application, this package provides a dialog box to step through the process of creating one, than using the "Asset -> Create" context menu. To open this window, in the file menu bar, select "Windows -> Omiya Games -> Domain List."

![Context Menu](/Documentation~/images/domainList/contextMenu.png)

This should make the following window pop-up.

![Default Window](/Documentation~/images/domainList/defaultWindow.png)

### Creating an asset bundle

Once the window is open, to start creating a Domain List, simply fill out the following fields:

1. (Optional) Set the Encrypter field to a String Encrypter in the Project window.
2. Expand "Generate Domain List Asset" foldout if it isn't already by clicking on it. Under "All Accepted Domains" list, add the plain-text strings to store in the asset bundle.
3. Fill out the the two fields to indicate where the file should be created:
    1. "Name of folder" field should contain a path relative to the root of the project e.g. `Assets/FolderName`. Note Unity expects '/' as the folder divider, regardless of operating system.
    2. "Name of asset to generate" will set the name of the new file.

![Asset Setup](/Documentation~/images/domainList/generateAsset.png)

Once all the fields are filled in, click "Generate Domain List Asset" to create the asset. This can take a few minutes.

![Asset Created!](/Documentation~/images/domainList/createdAsset.png)

Note: the process of making this simpler is being investigated.  See: [Github Issue #5](https://github.com/OmiyaGames/omiya-games-cryptography/issues/5)

### Reading and editing an asset bundle

In the same window, fill out the following fields:
1. (Optional) Set the Encrypter field to a String Encrypter in the Project window.
2. Expand "REad Domain List Asset" foldout if it isn't already by clicking on it. Drag-and-drop from the Project window the Domain List you want to edit or read.

![Loading Domain List](/Documentation~/images/domainList/loadAsset.png)

If you click on "Read Domain List Asset," a message bubble will show up at the bottom of the foldout, providing a list of strings the Domain List holds (if it was able to be read successfully).

![REading Domain List](/Documentation~/images/domainList/readAsset.png)

If you click on "Edit Domain List Asset," the fields in the "Generate Domain List Asset" foldout will now be populated with the information contained in the domain list. Simply expand that foldout, make the edits needed, then click "Generate Domain List Asset."

![Editing Domain List](/Documentation~/images/domainList/editAsset.png)

Note #1: if you try to overwrite an existing file, the dialog will warn you so, and ask if you want to confirm this action. Obviously, click "Yes" will prompt the dialog to overwrite the file.

![Overwrite Confirmation](/Documentation~/images/domainList/overwrite.png)

Note #2: the process of making this simpler is being investigated.  See: [Github Issue #5](https://github.com/OmiyaGames/omiya-games-cryptography/issues/5)

## Using the asset in a script

Loading a Domain List in a scripts does require a little work. The example below indicates how to load an AssetBundle asynchronously using a user-defined file name, followed by retrieving a DomainList from that AssetBundle via a helper function:

```csharp
using UnityEngine;
using System.Collections;
using System.IO;

// This using is necessary to support Domain List
using OmiyaGames.Cryptography;

public class SampleDomainList : MonoBehaviour
{
    // Inspector variables necessary to load a Domain List
    [SerializeField]
    private string assetName;
    [SerializeField]
    private StringCryptographer decrypter;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        // Asyncrhonously load the asset bundle
        AssetBundleCreateRequest bundleLoadRequest = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, assetName));
        yield return bundleLoadRequest;

        // Confirm an asset bundle was successfully loaded
        AssetBundle myAssetBundle = bundleLoadRequest.assetBundle;
        if (myAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");

            // Halt Start
            yield break;
        }

        // Attempt to load the Domain List from the asset bundle
        DomainList list = DomainList.Get(myAssetBundle);
        if (list == null)
        {
            Debug.Log("Failed to load DomainList!");

            // Unload the entire asset bundle
            myAssetBundle.Unload(false);

            // Halt Start
            yield break;
        }

        // If successfully loaded, decrypt all the strings stored in the asset
        foreach(string encryptedString in list)
        {
            Debug.Log(decrypter.Decrypt(encryptedString));
        }

        // Note: this loop below does the same thing as the one above;
        // DomainList is just a read-only IList<string>, after all.
        for (int index = 0; index < list.Count; ++index)
        {
            Debug.Log(decrypter.Decrypt(list[index]));
        }

        // Unload the entire asset bundle
        myAssetBundle.Unload(false);
    }
}
```

## Additional Resources

- [Doxygen-generated doc](/Documentation~/html/class_omiya_games_1_1_cryptography_1_1_domain_list.html)
- [`DomainList.cs` source code](/Runtime/DomainList.cs)
- [`DomainListAssetBundleGenerator.cs` source code (the dialog box)](/Editor/DomainListAssetBundleGenerator.cs)
