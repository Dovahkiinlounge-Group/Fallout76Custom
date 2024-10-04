# Fallout 76 Customization Tool

## Features:
- **Automatic Detection of Fallout 76 Folder**: Automatically detects the installation folder of Fallout 76, including the Game Pass version.
- **Custom BA2 File Selection**: Choose to either automatically select all found BA2 files or manually select the ones you want to use.
- **Reduce Load Times**: Enable enhanced load time reduction for a faster gaming experience.
- **Disable V-Sync**: Turn off V-Sync to minimize input lag and improve responsiveness.
- **Multilingual Support**: Supports multiple languages and can be easily adapted for other languages.
- **Registry Settings**: The tool sets the Fallout76/Project76.exe file to high priority (Install/Uninstall) in the Windows Registry for better performance.

## Usage:
1. **Start the Tool**: Run the `Fallout76Custom.ini.exe`.
2. **Language Selection**: The tool detects your operating system's language and automatically downloads the corresponding language file. If no matching language file is found, the default language (English) will be used.
3. **Find Fallout 76 Folder**: The tool automatically searches for the Fallout 76 installation folder. If not found, you will be prompted to manually select the folder.
4. **Select BA2 Files**:
   - **Automatic Selection**: Press `Y` or `y` to automatically select all found BA2 files.
   - **Manual Selection**: Press `N` or `n` to view a list of BA2 files and select them manually.
5. **Adjust Settings**: Choose whether to enable load time reduction and disable V-Sync by pressing `Y` or `N` for Yes or No.
6. **Save Changes**: The tool automatically saves the changes to the Fallout 76 configuration file and exits upon completion.

## Creating Custom Translations:
To create a custom translation, follow these steps:

1. **Create a Template**:
   - Create a new INI file named `xx.ini`, where `xx` represents the language code (e.g., `fr` for French, `es` for Spanish).
   - Use the existing INI files (`en.ini`, `de.ini`) as a template and copy their contents into your new file.
   
2. **Translate Texts**:
   - Open the INI file with a text editor.
   - Translate the values behind the keys (e.g., `WelcomeMessage`, `FolderNotFoundMessage`) into the desired language, keeping the keys unchanged.
   
3. **Save the File**:
   - Save the translated INI file in the `Fallout76Custom` folder.
   
4. **Test**:
   - Run the tool and check if the translation loads correctly. Adjust the translation if needed.

## Frequently Asked Questions (FAQ):
### Q: What should I do if the tool cannot find the Fallout 76 folder?
- **A**: Check the installation path of Fallout 76 and ensure that the `.exe` files are present in the specified directory. If the problem persists, manually enter the path.

### Q: How can I add additional languages?
- **A**: Create a new INI file in the `Fallout76Custom` folder, named according to the language code (e.g., `it.ini` for Italian). Translate the key-value pairs and save the file.

### Q: What if no language file is available for my language?
- **A**: Create a new INI file based on the available language files and add your translations.

### Q: Why is the program not starting?
- **A**: Ensure that you have the latest .NET 8 Desktop Runtime installed on your system. You can download it from the following link: [Download .NET Desktop Runtime 8.0.8](https://dotnet.microsoft.com/download).

### Q: Where is the registry setting installed?
- **A**: The registry settings for high CPU priority are located in `HKEY_CURRENT_USER\Software\Microsoft\Windows NT\CurrentVersion\Image File Execution Options`. This registry key contains performance settings for the Fallout 76 executable.

---

