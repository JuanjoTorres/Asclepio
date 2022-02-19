using Parabox.Stl;
using System.IO;
using UnityEditor;
using UnityEngine;

class ImporterManager : MonoBehaviour
{
    private GameObject[] respawns;

    [Header("Spawn Settings")]
    [SerializeField] Transform spawnPointer;

    private void Awake()
    {
        if (respawns == null)
            respawns = GameObject.FindGameObjectsWithTag("Respawn");

        string meshPath = "C:/Users/Juanjo Torres/Desktop/tfg_sources/bone1.stl";
        // ProcessSTL(meshPath);
    }

    void ProcessSTL(string meshPath)
    {
        var meshes = Importer.Import(meshPath);

        if (meshes.Length < 1)
            return;
        else
        {
            string modelName = Path.GetFileNameWithoutExtension(meshPath);
            string localPath = AssetDatabase.GenerateUniqueAssetPath(@"Assets/Imports/Prefabs/" + modelName + ".prefab");
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

            asset.AddComponent<BoxCollider>();
            asset.AddComponent<Rigidbody>();
            asset.AddComponent<XROffsetGrabInteractable>();

            asset.layer = 8;

            Debug.Log($"New prefab \"{prefabName}\" imported in \"{localPath}\"");
            PrefabUtility.SaveAsPrefabAsset(asset, localPath);

            Instantiate(asset, spawnPointer.position, transform.rotation);
            DestroyImmediate(asset);
            // asset.transform.position = spawnPointer.transform.position;
        }
    }

}
