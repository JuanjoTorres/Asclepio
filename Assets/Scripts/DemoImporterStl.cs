using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityMeshImporter;

public class DemoImporterStl : MonoBehaviour
{
    private const string MeshPath = "C:/Users/Juanjo Torres/Documents/repos/unity/Asclepio/Assets/Resources/smoothPelvis.stl";
    private GameObject Workspace;
    // Start is called before the first frame update

    private void Awake()
    {
        Workspace = GameObject.Find("Workspace");
        // string fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "smoothPelvis.stl");
        string fileName = Path.Combine(Application.dataPath, "cube2.stl");
        print(fileName);

        var newAsset = MeshImporter.Load(MeshPath);
        print(Workspace.name);
        newAsset.transform.SetParent(Workspace.transform, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
