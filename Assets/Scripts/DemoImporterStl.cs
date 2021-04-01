﻿using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace Parabox.Stl.Editor
{
    public class DemoImporterStl : MonoBehaviour
    {
        private const string MeshPath = "C:/Users/Juanjo Torres/Desktop/pelvis-ascii.stl";
        // private GameObject Workspace;
        // Start is called before the first frame update

        private void Awake()
        {

            Debug.Log("Hello!!");
            var name = Path.GetFileNameWithoutExtension(MeshPath);
            print(name);
            var meshes = Importer.Import(MeshPath);

            if (meshes.Length < 1)
                return;

            if (meshes.Length < 2)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);

                Object.DestroyImmediate(go.GetComponent<BoxCollider>());
                go.name = name;
                meshes[0].name = "Mesh-" + name;
                go.GetComponent<MeshFilter>().sharedMesh = meshes[0];

                AssetDatabase.CreateAsset(go, go.name);
                AssetDatabase.AddObjectToAsset(meshes[0], meshes[0].name);
                AssetDatabase.SetMainObject(go, go.name);

            }
            else
            {
                var parent = new GameObject();
                parent.name = name;

                for (int i = 0, c = meshes.Length; i < c; i++)
                {
                    var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Object.DestroyImmediate(go.GetComponent<BoxCollider>());
                    go.transform.SetParent(parent.transform, false);
                    go.name = name + "(" + i + ")";

                    var mesh = meshes[i];
                    mesh.name = "Mesh-" + name + "(" + i + ")";
                    go.GetComponent<MeshFilter>().sharedMesh = mesh;

                    AssetDatabase.AddObjectToAsset(mesh, mesh.name);
                }

                AssetDatabase.AddObjectToAsset(parent, parent.name);
                AssetDatabase.SetMainObject(parent, parent.name);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

    }

}
