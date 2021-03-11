using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Assimp;

public class DemoImporterStl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AssimpContext importer = new AssimpContext();

        Stream stream = File.Open("mymodel.fbx", FileMode.Open);
        Scene scene = importer.ImportFileFromStream(stream);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
