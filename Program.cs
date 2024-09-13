using Microsoft.Win32;

namespace Fallout76ConfigEditor
{
    class Program
    {
        static string iniFilePath;
        static Dictionary<string, string> messages;
        static string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fallout76Custom");
        static string languageFilePath = Path.Combine(basePath, "language.ini");
        static string exetype;

        static void Main(string[] args)
        {
            // Detect language from the operating system and load messages
            string language = DetectLanguage();
            LoadMessages(language);

            Console.WriteLine(GetLocalizedString("WelcomeMessage"));

            string falloutFolderPath = FindFalloutFolderPath();
            if (string.IsNullOrEmpty(falloutFolderPath))
            {
                Console.WriteLine(GetLocalizedString("FolderNotFoundMessage"));
                return;
            }
            else
            {
                Console.WriteLine(GetLocalizedString("Fallout76Found ") + falloutFolderPath);
            }

            // Define iniFilePath based on the executable found
            string exeFileName = Path.Combine(falloutFolderPath, "Fallout76.exe");
            if (File.Exists(exeFileName))
            {
                iniFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Fallout 76", "Fallout76Custom.ini");
                Console.WriteLine(GetLocalizedString("Fallout76VersionDetected"));
                exetype = exeFileName;
            }
            else
            {
                exeFileName = Path.Combine(falloutFolderPath, "Project76_GamePass.exe");
                if (File.Exists(exeFileName))
                {
                    iniFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Fallout 76", "Project76Custom.ini");
                    Console.WriteLine(GetLocalizedString("GamePassVersionDetected"));
                    exetype = exeFileName;
                }
                else
                {
                    Console.WriteLine(GetLocalizedString("NeitherExeFound"));
                    return;
                }
            }

            List<string> customBA2Files = FindCustomBA2Files(Path.Combine(falloutFolderPath, "Data"));
            if (customBA2Files.Count == 0)
            {
                Console.WriteLine(GetLocalizedString("NoCustomBA2FilesFound"));
                return;
            }

            // Ask the user if they want to select all BA2 files (Y)es or (N)o
            Console.Write(GetLocalizedString("SelectAllBA2FilesPrompt") + " ");
            var key = Console.ReadKey(true).KeyChar; // Reads a single key without requiring 'Enter'
            Console.WriteLine(); // Go to the next line after reading the key

            List<string> selectedFiles = new List<string>();

            if (key == 'Y' || key == 'y')
            {
                // Select all BA2 files automatically
                selectedFiles = new List<string>(customBA2Files);
                Console.WriteLine(GetLocalizedString("AllBA2FilesSelected"));
            }
            else if (key == 'N' || key == 'n')
            {
                // Ask user to manually select files
                selectedFiles = ManualFileSelection(customBA2Files);
            }
            else
            {
                Console.WriteLine(GetLocalizedString("InvalidInputMessage"));
                return;
            }

            if (selectedFiles.Count > 0)
            {
                SaveCheckedItems(selectedFiles);
            }
            else
            {
                Console.WriteLine(GetLocalizedString("NoFilesSelectedMessage"));
                return;
            }

            // Prompt the user to select whether to apply Load Reduction and disable VSync
            Console.Write(GetLocalizedString("EnableLoadTimeReductionPrompt") + " ");
            var loadReduceKey = Console.ReadKey(true).KeyChar;
            Console.WriteLine();

            Console.Write(GetLocalizedString("DisableVSyncPrompt") + " ");
            var vSyncKey = Console.ReadKey(true).KeyChar;
            Console.WriteLine();

            if (loadReduceKey == 'Y' || loadReduceKey == 'y')
            {
                SaveLoadReduceTime();
                Console.WriteLine(GetLocalizedString("LoadTimeReductionEnabled"));
            }

            if (vSyncKey == 'Y' || vSyncKey == 'y')
            {
                SaveDisableVSync();
                Console.WriteLine(GetLocalizedString("VSyncDisabled"));
            }

            // New section: Check if Fallout 76 is already set to High Priority in the Registry
            bool isHighPrioritySet = IsHighPrioritySet();

            // Prompt the user if they want to install or uninstall the high priority setting
            if (isHighPrioritySet)
            {
                Console.WriteLine(GetLocalizedString("HighPriorityAlreadyEnabledPrompt"));
                Console.Write(GetLocalizedString("RemoveHighPriorityPrompt") + " ");
            }
            else
            {
                Console.Write(GetLocalizedString("SetHighPriorityPrompt") + " ");
            }

            var priorityKey = Console.ReadKey(true).KeyChar;
            Console.WriteLine();

            if (priorityKey == 'Y' || priorityKey == 'y')
            {
                if (isHighPrioritySet)
                {
                    RemoveHighPriority();
                    Console.WriteLine(GetLocalizedString("HighPriorityDisabled"));
                }
                else
                {
                    SetHighPriority();
                    Console.WriteLine(GetLocalizedString("HighPriorityEnabled"));
                }
            }
            else if (priorityKey == 'N' || priorityKey == 'n')
            {
                Console.WriteLine(GetLocalizedString("NoChangesMadeMessage"));
            }
            else
            {
                Console.WriteLine(GetLocalizedString("InvalidInputMessage"));
                return;
            }

            Console.WriteLine(GetLocalizedString("ChangesSavedSuccessfully"));
            Thread.Sleep(2000);
            Environment.Exit(0);

            // Function to check if High Priority is already set
            static bool IsHighPrioritySet()
            {
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@$"Software\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\{exetype}\PerfOptions"))
                    {
                        if (key != null)
                        {
                            object value = key.GetValue("CpuPriorityClass");
                            return value != null && (int)value == 3;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(GetLocalizedString("RegistryReadError") + ex.Message);
                }
                return false;
            }

            // Function to set the Fallout 76 process to high priority in the registry
            static void SetHighPriority()
            {
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@$"Software\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\{exetype}\PerfOptions"))
                    {
                        key.SetValue("CpuPriorityClass", 3, RegistryValueKind.DWord); // 3 sets it to high priority
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(GetLocalizedString("RegistryWriteError") + ex.Message);
                }
            }

            // Function to remove high priority from the Fallout 76 process in the registry
            static void RemoveHighPriority()
            {
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@$"Software\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\{exetype}\PerfOptions", true))
                    {
                        if (key != null)
                        {
                            key.DeleteValue("CpuPriorityClass", false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(GetLocalizedString("RegistryDeleteError") + ex.Message);
                }
            }


            static string DetectLanguage()
            {
                // Detect the current culture of the operating system
                CultureInfo currentCulture = CultureInfo.CurrentCulture;
                string cultureName = currentCulture.TwoLetterISOLanguageName;

                // Default to English if the detected language is not supported
                if (cultureName != "en" && cultureName != $"{cultureName}")
                {
                    cultureName = "en";
                }

                return cultureName;
            }

            static void LoadMessages(string language)
            {
                string languageFilePath = Path.Combine(basePath, $"{language}.ini");

                if (!File.Exists(languageFilePath))
                {
                    // Fallback to English if the language file is not found
                    languageFilePath = Path.Combine(basePath, "en.ini");
                }

                messages = new Dictionary<string, string>();
                foreach (var line in File.ReadAllLines(languageFilePath))
                {
                    var parts = line.Split(new[] { '=' }, 2);
                    if (parts.Length == 2)
                    {
                        messages[parts[0].Trim()] = parts[1].Trim();
                    }
                }
            }

            static string GetLocalizedString(string key)
            {
                return messages.ContainsKey(key) ? messages[key] : $"[Missing translation for {key}]";
            }

            static string FindFalloutFolderPath()
            {
                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

                for (int i = 0; i < 2; i++)
                {
                    string parentDirectory = Directory.GetParent(currentDirectory).FullName;

                    if (File.Exists(Path.Combine(parentDirectory, "Fallout76.exe")) ||
                        File.Exists(Path.Combine(parentDirectory, "Project76_GamePass.exe")))
                    {
                        return parentDirectory;
                    }

                    currentDirectory = parentDirectory;
                }

                Console.WriteLine(GetLocalizedString("FolderNotFoundMessage"));
                Console.WriteLine(GetLocalizedString("PleaseEnterPathMessage"));
                string selectedFalloutPath = Console.ReadLine();

                if (Directory.Exists(selectedFalloutPath) &&
                    (File.Exists(Path.Combine(selectedFalloutPath, "Fallout76.exe")) ||
                     File.Exists(Path.Combine(selectedFalloutPath, "Project76_GamePass.exe"))))
                {
                    return selectedFalloutPath;
                }
                else
                {
                    Console.WriteLine(GetLocalizedString("InvalidFolderPathMessage"));
                    return null;
                }
            }

            static List<string> FindCustomBA2Files(string dataFolderPath)
            {
                List<string> customBA2Files = new List<string>();

                if (!Directory.Exists(dataFolderPath))
                {
                    Console.WriteLine(GetLocalizedString("DataFolderNotFoundMessage"));
                    return customBA2Files;
                }

                foreach (string file in Directory.GetFiles(dataFolderPath, "*.ba2"))
                {
                    if (!IsFallout76File(file))
                    {
                        customBA2Files.Add(file);
                    }
                }

                return customBA2Files;
            }

            static bool IsFallout76File(string fileName)
            {
                return fileName.EndsWith(".ba2", StringComparison.OrdinalIgnoreCase) &&
                       fileName.Contains("SeventySix - ");
            }

            static void SaveCheckedItems(List<string> checkedItems)
            {
                StringBuilder sb = new StringBuilder();

                foreach (string item in checkedItems)
                {
                    sb.Append(Path.GetFileName(item));
                    sb.Append(",");
                }

                string checkedItemsString = sb.ToString().TrimEnd(',');

                string[] iniLines = File.ReadAllLines(iniFilePath);
                bool foundSection = false;

                for (int i = 0; i < iniLines.Length; i++)
                {
                    if (iniLines[i].Trim().ToLower() == "[archive]")
                    {
                        foundSection = true;
                        int j = i + 1;

                        // Look for "sResourceArchive2List="
                        while (j < iniLines.Length && !iniLines[j].StartsWith("sResourceArchive2List="))
                        {
                            j++;
                        }

                        if (j < iniLines.Length)
                        {
                            // Replace the existing line with the selected BA2 files
                            iniLines[j] = "sResourceArchive2List=" + checkedItemsString;
                        }
                        else
                        {
                            // Add the line if it doesn't exist
                            Array.Resize(ref iniLines, iniLines.Length + 1);
                            iniLines[j] = "sResourceArchive2List=" + checkedItemsString;
                        }

                        // Ensure the other two lines are added below
                        Array.Resize(ref iniLines, iniLines.Length + 2);
                        iniLines[j + 1] = "sResourceDataDirsFinal=";   // Add sResourceDataDirsFinal
                        iniLines[j + 2] = "bInvalidateOlderFiles=1";        // Add iPresentInterval

                        break;
                    }
                }

                if (!foundSection)
                {
                    // If the [Archive] section doesn't exist, add it along with the new lines
                    Array.Resize(ref iniLines, iniLines.Length + 4);
                    iniLines[iniLines.Length - 4] = "[Archive]";
                    iniLines[iniLines.Length - 3] = "sResourceArchive2List=" + checkedItemsString;
                    iniLines[iniLines.Length - 2] = "sResourceDataDirsFinal=";
                    iniLines[iniLines.Length - 1] = "bInvalidateOlderFiles=1";
                }

                File.WriteAllLines(iniFilePath, iniLines);
            }


            static List<string> ManualFileSelection(List<string> customBA2Files)
            {
                List<string> selectedFiles = new List<string>();

                Console.WriteLine(GetLocalizedString("SelectBA2FilesPrompt"));
                for (int i = 0; i < customBA2Files.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {Path.GetFileName(customBA2Files[i])}");
                }

                Console.Write(GetLocalizedString("EnterFileNumbersPrompt") + " ");
                string[] fileSelections = Console.ReadLine().Split(',');

                foreach (string selection in fileSelections)
                {
                    if (int.TryParse(selection.Trim(), out int index) && index > 0 && index <= customBA2Files.Count)
                    {
                        selectedFiles.Add(customBA2Files[index - 1]);
                    }
                }

                return selectedFiles;
            }

            static void SaveLoadReduceTime()
            {
                string[] iniLines = File.ReadAllLines(iniFilePath);
                bool interfaceSectionExists = false;

                for (int i = 0; i < iniLines.Length; i++)
                {
                    if (iniLines[i].Trim().ToLower() == "[interface]")
                    {
                        interfaceSectionExists = true;
                        iniLines[i + 1] = "fFadeToBlackFadeSeconds=0.2000";
                        iniLines[i + 2] = "fMinSecondsForLoadFadeIn=0.3000";
                        break;
                    }
                }

                if (!interfaceSectionExists)
                {
                    List<string> tempList = new List<string>(iniLines);
                    tempList.Add("[Interface]");
                    tempList.Add("fFadeToBlackFadeSeconds=0.2000");
                    tempList.Add("fMinSecondsForLoadFadeIn=0.3000");
                    iniLines = tempList.ToArray();
                }

                File.WriteAllLines(iniFilePath, iniLines);
            }

            static void SaveDisableVSync()
            {
                string[] iniLines = File.ReadAllLines(iniFilePath);
                bool displaySectionExists = false;

                for (int i = 0; i < iniLines.Length; i++)
                {
                    if (iniLines[i].Trim().ToLower() == "[display]")
                    {
                        displaySectionExists = true;
                        iniLines[i + 1] = "iPresentInterval=0";
                        break;
                    }
                }

                if (!displaySectionExists)
                {
                    List<string> tempList = new List<string>(iniLines);
                    tempList.Add("[Display]");
                    tempList.Add("iPresentInterval=0");
                    iniLines = tempList.ToArray();
                }

                File.WriteAllLines(iniFilePath, iniLines);
            }
        }
    }
}
