using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Parabox.Stl.Editor
{
    public class DemoImporterStl : MonoBehaviour
    {
        private List<string> listMeshPath =  new List<string>() { "C:/Users/Juanjo Torres/Desktop/tfg_sources/prove_foot2.stl", "C:/Users/Juanjo Torres/Desktop/tfg_sources/foot_model.stl" };
        private GameObject[] respawns;


        private void Start()
        {
            if (respawns == null)
                respawns = GameObject.FindGameObjectsWithTag("Respawn");
        }
        // Start is called before the first frame update
        private void Awake()
        {
            //foreach (string meshPath in ListMeshPath)
            //{
            //    var name = Path.GetFileNameWithoutExtension(meshPath);
            //    print("Importando STL " + name + "...");
            //    ProcessSTL(meshPath);
            //}
            string meshPath = "C:/Users/Juanjo Torres/Desktop/tfg_sources/prove_foot2.stl";
            var name = Path.GetFileNameWithoutExtension(meshPath);
            print("Importando STL " + name + "...");
            // ProcessSTL(meshPath);
        }

        void ProcessSTL(string meshPath)
        {
            var meshes = Importer.Import(meshPath);
            string name = Path.GetFileNameWithoutExtension(meshPath);

            if (meshes.Length < 1)
                return;

            if (meshes.Length < 2)
            {
                print("Enter because have one mesh");
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
                print("Enter because have two meshes");
                int meshCount = 0;
                GameObject asset = new GameObject();
                asset.name = name;

                foreach (Mesh mesh in meshes)
                {
                    GameObject objectChild = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Object.DestroyImmediate(objectChild.GetComponent<BoxCollider>());
                    objectChild.transform.SetParent(asset.transform, false);
                    objectChild.name = name + "(" + meshCount + ")";

                    mesh.name = "Mesh-" + name + "(" + meshCount + ")";
                    objectChild.GetComponent<MeshFilter>().sharedMesh = mesh;

                    AssetDatabase.AddObjectToAsset(mesh, asset.name);
                    meshCount++;
                }

                asset.AddComponent<BoxCollider>();
                asset.AddComponent<Rigidbody>();
                asset.transform.Translate(new Vector3(0, 10, 0), Space.World);
                string localPath = "Assets/Import/" + asset.name + ".prefab";
                localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                PrefabUtility.SaveAsPrefabAsset(asset, localPath);

                Instantiate(asset, respawns[0].transform.position, respawns[0].transform.rotation);
                //AssetDatabase.AddObjectToAsset(parent, parent.name);
                //AssetDatabase.SetMainObject(parent, parent.name);
            }
        }

    }

}
