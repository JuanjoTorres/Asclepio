using Parabox.Stl;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ImporterUtils
{
    class ImporterManager : MonoBehaviour
    {
        private GameObject[] respawns;

        [Header("Model Settings")]
        [SerializeField] Transform spawnPointer;
        [SerializeField] Material ModelMaterial;

        private void Awake()
        {
            if (respawns == null)
                respawns = GameObject.FindGameObjectsWithTag("Respawn");
        }

        public static void ProcessSTL(string meshPath)
        {
            Debug.Log("Fichero que se va a importar: " + meshPath);
            var meshes = Importer.Import(meshPath);

            if (meshes.Length < 1)
                return;
            else
            {
                string modelName = Path.GetFileNameWithoutExtension(meshPath);
                string localPath = AssetDatabase.GenerateUniqueAssetPath(@"Assets/Resources/" + modelName + ".prefab");
                string prefabName = Path.GetFileNameWithoutExtension(localPath);

                GameObject asset = new GameObject();
                asset.name = prefabName;

                int meshCount = 0;

                foreach (Mesh mesh in meshes)
                {
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    DestroyImmediate(go.GetComponent<BoxCollider>());
                    go.transform.SetParent(asset.transform, false);

                    string childName = string.Format("{0} ({1})", prefabName, meshCount);
                    go.name = childName;

                    mesh.name = "Mesh-" + childName;
                    go.GetComponent<MeshFilter>().sharedMesh = mesh;

                    AssetDatabase.AddObjectToAsset(mesh, localPath);
                    meshCount++;
                }

                asset.AddComponent<MeshRenderer>();
                asset.AddComponent<MeshFilter>();
                asset.AddComponent<MeshCollider>();
                asset.AddComponent<Rigidbody>();
                asset.AddComponent<XROffsetGrabInteractable>();
                asset.AddComponent<Paintable>();

                asset.layer = 8; // Layer Grab
                asset.tag = "Model";
                asset.transform.position = new Vector3(0, 4, 0);

                // Debug.Log($"New prefab \"{prefabName}\" imported in \"{localPath}\"");
                // PrefabUtility.SaveAsPrefabAsset(asset, localPath);

            }
        }

    }
}
