using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Parabox.Stl;

namespace Assets.Scripts
{
    class ImporterManager : MonoBehaviour
    {
        private List<string> listMeshPath = new List<string>() { "C:/Users/Juanjo Torres/Desktop/tfg_sources/prove_foot2.stl", "C:/Users/Juanjo Torres/Desktop/tfg_sources/foot_model.stl" };
        private GameObject[] respawns;

        [Header("Spawn Settings")]
        [SerializeField] Transform spwanPointer;

        private void Start()
        {
            
        }
        // Start is called before the first frame update
        private void Awake()
        {
            if (respawns == null)
                respawns = GameObject.FindGameObjectsWithTag("Respawn");
            //foreach (string meshPath in ListMeshPath)
            //{
            //    var name = Path.GetFileNameWithoutExtension(meshPath);
            //    print("Importando STL " + name + "...");
            //    ProcessSTL(meshPath);
            //}
            string meshPath = "C:/Users/Juanjo Torres/Desktop/tfg_sources/bone1.stl";
            var name = Path.GetFileNameWithoutExtension(meshPath);
            print("Importando STL " + name + "...");
            ProcessSTL(meshPath);
        }

        void ProcessSTL(string meshPath)
        {
            var meshes = Importer.Import(meshPath);

            if (meshes.Length < 1)
                return;
            else
            {
                string name = Path.GetFileNameWithoutExtension(meshPath);

                GameObject asset = new GameObject
                {
                    name = name
                };

                int meshCount = 0;
                string localPath = "Assets/Import/" + asset.name + ".prefab";
                localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

                foreach (Mesh mesh in meshes)
                {
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    DestroyImmediate(go.GetComponent<BoxCollider>());
                    go.transform.SetParent(asset.transform, false);

                    string childName = string.Format("{0} ({1})", name, meshCount);
                    go.name = childName;

                    mesh.name = "Mesh-" + childName;
                    go.GetComponent<MeshFilter>().sharedMesh = mesh;

                    AssetDatabase.AddObjectToAsset(mesh, localPath);
                    meshCount++;
                }

                asset.AddComponent<BoxCollider>();
                asset.AddComponent<Rigidbody>();

                PrefabUtility.SaveAsPrefabAsset(asset, localPath);
                asset.transform.position = spwanPointer.position;
            }
        }

    }
}
