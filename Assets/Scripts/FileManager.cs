using Dummiesman;
using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FileManager : MonoBehaviour
{
    private const float LAMP_INTENSITY_MAX = 300f;

    string outputPath;
    string sourcePath;
    string destinationPath;
    GameObject loadedObject;
    GameObject childLoadedObject;

    [Header("Lists and Dropdowns")]
    public Dropdown ModelsCollection;

    [Header("Buttons")]
    public Button QuitButton;
    public Button SearchButton;
    public Button LoadButton;
    public Button CloseButton;

    [Header("Another Inputs")]
    public Toggle ExplorationModeToggle;
    public Toggle LightsToggle;
    public Toggle LampsToggle;
    public Slider LampLightSlider;

    // InputField modelName;

    private void Awake()
    {
        // modelName = GameObject.Find("Model Name Field").GetComponent<InputField>();

        ConfigureListeners();
        UpdateDropdown();
    }

    private void OpenFileChooser(bool value)
    {
        FileBrowser.SetFilters(true,
            new FileBrowser.Filter("Wavefront Files", ".obj"));
        FileBrowser.SetDefaultFilter(".obj");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);

        StartCoroutine(ShowLoadDialogCoroutine(value));
        // ImportModel(); LoadModel

    }

    private IEnumerator ShowLoadDialogCoroutine(bool value)
    {
        yield return FileBrowser.WaitForLoadDialog(
            FileBrowser.PickMode.FilesAndFolders,   // Pick Mode
            false,                                  // Allow multiple selection
            null,                                   // Initial path: default (Documents)
            null,                                   // Initial filename: empty
            "Load Files and Folders",
            "Import Model");                        // Submit Button Title

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
                    // Show in screen
                    Debug.Log(FileBrowser.Result[i]);
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
            sourcePath = Path.Combine(Application.persistentDataPath, FileBrowserHelpers.GetFilename(FileBrowser.Result[0]));
            Debug.Log("Source path: " + sourcePath);
            FileBrowserHelpers.CopyFile(FileBrowser.Result[0], sourcePath);

            StartCoroutine(ImportModel());
        }
    }

    private IEnumerator ImportModel()
    {
        if (File.Exists(sourcePath))
        {
            if (loadedObject != null)
                Destroy(loadedObject);
            loadedObject = new OBJLoader().Load(sourcePath);

            if (childLoadedObject != null)
            {
                Destroy(childLoadedObject);
            }

            childLoadedObject = loadedObject.transform.GetChild(0).gameObject;

            var collider = childLoadedObject.AddComponent<BoxCollider>();

            childLoadedObject.GetComponent<Renderer>().material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            childLoadedObject.AddComponent<XROffsetGrabInteractable>();
            childLoadedObject.tag = "Model";
            childLoadedObject.layer = LayerMask.NameToLayer("Grab");
            childLoadedObject.transform.position = new Vector3(0, 4, 0);
        }

        yield return null;
    }

    private void UpdateDropdown()
    {
        // Create a List of new Dropdown options
        List<string> dropdownOptions = new List<string>();

        DirectoryInfo directory = new DirectoryInfo(@"Assets\Resources");
        FileInfo[] files = directory.GetFiles("*.prefab");

        foreach (FileInfo file in files)
        {
            // Create list
            dropdownOptions.Add(file.Name);
            Debug.Log(file.Name);
        }

        // Clear dropdown
        int modelsCount = ModelsCollection.options.Count;

        for (int x = 0; x < modelsCount; x++)
        {
            ModelsCollection.options.RemoveAt(0);
        }

        Debug.Log(ModelsCollection.options.Count);

        // Load into dropdown
        ModelsCollection.AddOptions(dropdownOptions);
        Debug.Log(ModelsCollection.options.Count);
    }

    private void ConfigureListeners()
    {
        // Object Model Processing Buttons
        SearchButton.onClick.AddListener(delegate
        {
            OpenFileChooser(true);
        });

        // Control App Flow Buttons
        QuitButton.onClick.AddListener(delegate
        {
            UnityEditor.EditorApplication.isPlaying = false;
        });

        // Environment Settings Option Buttons
        CloseButton.onClick.AddListener(delegate
        {
            gameObject.SetActive(false);
        });
        ExplorationModeToggle.onValueChanged.AddListener(delegate
        {
            EnableExplorationMode(ExplorationModeToggle.isOn);
        });
        LightsToggle.onValueChanged.AddListener(delegate
        {
            EnableLightsRoom(LightsToggle.isOn);
        });
        LampsToggle.onValueChanged.AddListener(delegate
        {
            EnableLampsRoom(LampsToggle.isOn);
        });
        LampLightSlider.onValueChanged.AddListener(delegate
        {
            ChangeSpotlightsIntesity(LampLightSlider.value);
        });



    }

    private void ChangeSpotlightsIntesity(float value)
    {
        var spotLights = GameObject.FindGameObjectsWithTag("Spot");

        foreach (var spot in spotLights)
        {
            spot.GetComponent<Light>().intensity = value * LAMP_INTENSITY_MAX;
        }
    }

    private void EnableExplorationMode(bool enabled)
    {
        var targets = GameObject.FindGameObjectsWithTag("Model");
        Debug.Log(enabled);

        if (targets != null)
        {
            foreach (var target in targets)
            {
                Debug.Log(target.name);
                var physics = target.GetComponent<Rigidbody>();
                physics.isKinematic = enabled;
                physics.useGravity = !enabled;
            }
        }
    }

    private void EnableLightsRoom(bool enabled)
    {
        GameObject[] lights = GameObject.FindGameObjectsWithTag("Lights");
        foreach (var light in lights)
        {
            light.GetComponent<Light>().enabled = enabled;
        }
    }

    private void EnableLampsRoom(bool enabled)
    {
        GameObject[] spotLights = GameObject.FindGameObjectsWithTag("Spot");
        foreach (var spot in spotLights)
        {
            spot.GetComponent<Light>().enabled = enabled;
        }
    }


}
