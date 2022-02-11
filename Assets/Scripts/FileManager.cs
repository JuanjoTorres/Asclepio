﻿using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FileManager : MonoBehaviour
{

    string inputPath;
    string outputPath;
    string modelPath;

    public Text text;
    bool sceneIsLoaded;

    Dropdown dropdown;
    InputField modelName;

    private void Start()
    {
        dropdown = GameObject.Find("Dropdown").GetComponent<Dropdown>();
        modelName = GameObject.Find("Model Name Field").GetComponent<InputField>();

        ConfigureListeners();
        UpdateDropdown();
    }

    private void OpenFileChooser(bool value)
    {
        FileBrowser.SetFilters(true,
            new FileBrowser.Filter("Images", ".jpg", ".png"),
            new FileBrowser.Filter("Text Files", ".txt", ".pdf"),
            new FileBrowser.Filter("Stereolitography Files", ".stl"));
        FileBrowser.SetDefaultFilter(".stl");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);

        // StartCoroutine(ShowLoadDialogCoroutine(value));
    }

    private IEnumerator ShowLoadDialogCoroutine(bool value)
    {
        yield return FileBrowser.WaitForLoadDialog(
            FileBrowser.PickMode.FilesAndFolders,   // Pick Mode
            false,                                  // Allow multiple selection
            null,                                   // Initial path: default (Documents)
            null,                                   // Initial filename: empty
            "Load Files and Folders",
            "Load Model");                          // Submit Button Title

        // Dialog is closed
        // Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
        // Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            // Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
            for (int i = 0; i < FileBrowser.Result.Length; i++)
            {
                if (value)
                {
                    inputPath = FileBrowser.Result[i];

                    // Show in screen
                    Text pathText = GameObject.Find("Path Text").GetComponent<Text>();
                    pathText.text = inputPath;
                }
                else
                {
                    outputPath = FileBrowser.Result[i];
                }
            }

            // Read the bytes of the first file via FileBrowserHelpers
            // Contrary to File.ReadAllBytes, this function works on Android 10+, as well
            byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);

            // Or, copy the first file to persistentDataPath
            string destinationPath = Path.Combine(Application.persistentDataPath, FileBrowserHelpers.GetFilename(FileBrowser.Result[0]));
            FileBrowserHelpers.CopyFile(FileBrowser.Result[0], destinationPath);
        }
    }

    private void ImportModel()
    {
        // Obtener datos de los elementos de la interfaz gráfica. 
        // float iso_value = GameObject.Find("iso_slider").GetComponent<Slider>().value;
        // float mesh_reduction = GameObject.Find("mesh_slider").GetComponent<Slider>().value;
        // float filter_value = GameObject.Find("filter_slider").GetComponent<Slider>().value;

        // bool smoothed = GameObject.Find("smoothed_toggle").GetComponent<Toggle>().isOn;

        // Calling STL Importer 
        // ProcessSTL("C:/Users/Juanjo Torres/Desktop/tfg_sources/bone1.stl");
        System.Threading.Thread.Sleep(15000);
        UpdateDropdown();
    }

    private void UpdateDropdown()
    {
        dropdown = GameObject.Find("Dropdown").GetComponent<Dropdown>();

        // Create a List of new Dropdown options
        List<string> dropdownOptions = new List<string>();

        DirectoryInfo directory = new DirectoryInfo(@"Assets\Imports\");
        FileInfo[] Files = directory.GetFiles("*.stl"); // Getting obj files

        foreach (FileInfo file in Files)
        {
            // Create list
            dropdownOptions.Add(file.Name);
        }

        // Clear dropdown
        for (int x = 0; x < dropdown.options.Count; x++)
        {
            dropdown.options.RemoveAt(x);
        }

        // Load into dropdown
        dropdown.AddOptions(dropdownOptions);
    }

    private void ConfigureListeners()
    {
        Button searchButton = GameObject.Find("Search Button").GetComponent<Button>();
        searchButton.onClick.AddListener(delegate { OpenFileChooser(true); });

        Button importButton = GameObject.Find("Import Button").GetComponent<Button>();
        importButton.onClick.AddListener(delegate { ImportModel(); });

        Button closeButton = GameObject.Find("Close Button").GetComponent<Button>();
        closeButton.onClick.AddListener(delegate { Application.Quit(); });

        Button loadButton = GameObject.Find("Load Model").GetComponent<Button>();
        loadButton.onClick.AddListener(delegate { LoadModel(); });
    }

    private void LoadModel()
    {
        modelPath = @"Assets\Imports\" + dropdown.options[dropdown.value].text;

    }

    private void OnDisable()
    {
        PlayerPrefs.SetString("ModelPath", modelPath);
    }
}