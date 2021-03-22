
using Assimp;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RuntimeMeshImporter
{
    class MeshMaterial
    {
        public MeshMaterial(string meshName, UnityEngine.Mesh mesh, UnityEngine.Material material)
        {
            MeshName = meshName;
            Mesh = mesh;
            Material = material;
        }

        public string MeshName { get; }
        public UnityEngine.Mesh Mesh { get; }
        public UnityEngine.Material Material { get; }
    }

    public class MeshImporter
    {
        public static GameObject Load(string meshPath, float scaleX = 1,  float scaleY = 1, float scaleZ = 1)
        {
            if (!File.Exists(meshPath))
            {
                Debug.Log("ERROR DE FICHERO: No se ha encontrado ningun archivo con ese nombre.");
                return null;
            }
                

            AssimpContext importer = new AssimpContext();
            Scene scene = importer.ImportFile(meshPath,
                //PostProcessSteps.MakeLeftHanded |
                //PostProcessSteps.FlipWindingOrder |
                PostProcessSteps.Triangulate |
                PostProcessSteps.GenerateUVCoords
            );

            if (scene == null)
            {
                Debug.Log("ERROR ASSIMP: El content no se ha procesado correctamente");
                return null;
            }
                

            string parentDirectory = Directory.GetParent(meshPath).FullName;

            // Materials
            List<UnityEngine.Material> uMaterials = new List<UnityEngine.Material>();
            Debug.Log("Assimp - Escena tiene materiales? " + scene.HasMaterials);

            if (scene.HasMaterials)
            {
                List<Assimp.Material> aMaterials = scene.Materials;
                Debug.Log("Assimp - Numero de materiales: " + aMaterials.Count);
                // Debug Variable
                var countMaterial = 0;

                foreach (var aMaterial in aMaterials)
                {
                    UnityEngine.Material uMaterial = new UnityEngine.Material(Shader.Find("Standard"));
                    
                    // Albedo
                    Debug.Log("Assimp - Material " + countMaterial + " tiene diffuse? " + aMaterial.HasColorDiffuse);
                    if (aMaterial.HasColorDiffuse)
                    {
                        Color color = new Color (
                            aMaterial.ColorDiffuse.R,
                            aMaterial.ColorDiffuse.G,
                            aMaterial.ColorDiffuse.B,
                            aMaterial.ColorDiffuse.A
                        );

                        uMaterial.SetColor("_Color", color);
                    }

                    // Emission
                    Debug.Log("Assimp - Material " + countMaterial + " tiene emission? " + aMaterial.HasColorEmissive);
                    if (aMaterial.HasColorEmissive)
                    {
                        Color color = new Color(
                            aMaterial.ColorEmissive.R,
                            aMaterial.ColorEmissive.G,
                            aMaterial.ColorEmissive.B,
                            aMaterial.ColorEmissive.A
                        );

                        uMaterial.SetColor("_EmissionColor", color);
                        uMaterial.EnableKeyword("_EMISSION");
                    }

                    // Reflectivity
                    Debug.Log("Assimp - Material " + countMaterial + " tiene reflectivity? " + aMaterial.HasColorReflective);
                    if (aMaterial.HasReflectivity)
                    {
                        uMaterial.SetFloat("_Glossiness", aMaterial.Reflectivity);
                    }

                    // Texture
                    Debug.Log("Assimp - Material " + countMaterial + " tiene texture diffuse? " + aMaterial.HasTextureDiffuse);
                    if (aMaterial.HasTextureDiffuse)
                    {
                        Texture2D uTexture = new Texture2D(2,2);
                        string texturePath = Path.Combine(parentDirectory, aMaterial.TextureDiffuse.FilePath);
                        byte[] byteArray = File.ReadAllBytes(texturePath);
                        bool isLoaded = uTexture.LoadImage(byteArray);

                        if (!isLoaded)
                        {
                            throw new Exception ("Cannot find texture file: " + texturePath);
                        }

                        uMaterial.SetTexture("_MainTex", uTexture);
                    }

                    countMaterial++; // TODO delete variable
                    uMaterials.Add(uMaterial);
                }

                Debug.Log("Unity - Numero de materiales: " + uMaterials.Count);
            }

            // Meshes
            List<MeshMaterial> uMeshes = new List<MeshMaterial>();
            // Debug Variable
            var countMeshes = 0;

            Debug.Log("Assimp - Escena tiene meshes? " + scene.HasMeshes);
            if (scene.HasMeshes)
            {
                List<Assimp.Mesh> aMeshes = scene.Meshes;
                Debug.Log("Assimp - Numero de meshes: " + aMeshes.Count);

                foreach (var aMesh in aMeshes)
                {
                    List<Vector3> uVertices = new List<Vector3>();
                    List<Vector3> uNormals = new List<Vector3>();
                    List<Vector2> uUV = new List<Vector2>();
                    List<int> uIndices = new List<int>();

                    // Vertices
                    // Variable debug
                    int countVertexs = 0;
                    Debug.Log("Assimp - Mesh " + countMeshes + " tiene vertices? " + aMesh.HasVertices);
                    if (aMesh.HasVertices)
                    {
                        foreach (var vertice in aMesh.Vertices)
                        {
                            Debug.Log("Vertice " + countVertexs + " : X " + vertice.X + " - Y " + vertice.Y + " - Z " + vertice.Z);
                            uVertices.Add(new Vector3(vertice.X, vertice.Y, vertice.Z));
                            countVertexs++;
                        }

                    }

                    // Normals
                    // Variable debug
                    var countNormals = 0;
                    Debug.Log("Assimp - Mesh " + countMeshes + " tiene normals? " + aMesh.HasNormals);
                    if (aMesh.HasNormals)
                    {
                        foreach (var aNormal in aMesh.Normals)
                        {
                            Debug.Log("Normal " + countNormals + " : X " + aNormal.X + " - Y " + aNormal.Y + " - Z " + aNormal.Z);
                            uNormals.Add(new Vector3(aNormal.X, aNormal.Y, aNormal.Z));
                            countNormals++;
                        }
                    }

                    // Triangles
                    Debug.Log("Assimp - Mesh " + countMeshes + " tiene triangles? " + aMesh.HasFaces);
                    if (aMesh.HasFaces)
                    {
                        foreach (var face in aMesh.Faces)
                        {
                            // TODO try to rewrite this code
                            // Ignore degenerated faces
                            if (face.IndexCount == 1 || face.IndexCount == 2)
                                continue;

                            // Variable Debug
                            var countIndices = 0;
                            for (int i = 0; i < (face.IndexCount - 2); i++)
                            {
                                uIndices.Add(face.Indices[i + 2]);
                                Debug.Log("Indice " + countIndices++ + ": " + face.Indices[i + 2].ToString());
                                uIndices.Add(face.Indices[i + 1]);
                                Debug.Log("Indice " + countIndices++ + ": " + face.Indices[i + 1].ToString());
                                uIndices.Add(face.Indices[0]);
                                Debug.Log("Indice " + countIndices++ + ": " + face.Indices[i].ToString());
                            }
                        }
                    }

                    // UV (Texture Coordinates)
                    Debug.Log("Assimp - Mesh " + countMeshes + " tiene uvs? " + aMesh.HasTextureCoords(0));
                    if (aMesh.HasTextureCoords(0))
                    {
                        foreach (var aUV in aMesh.TextureCoordinateChannels[0])
                            uUV.Add(new Vector2(aUV.X, aUV.Y));
                    }

                    UnityEngine.Mesh uMesh = new UnityEngine.Mesh();
                    uMesh.vertices = uVertices.ToArray();
                    uMesh.normals = uNormals.ToArray();
                    uMesh.triangles = uIndices.ToArray();
                    uMesh.uv = uUV.ToArray();

                    uMeshes.Add(new MeshMaterial(aMesh.Name, uMesh, uMaterials[aMesh.MaterialIndex]));
                    countMeshes++; // TODO delete variable
                }

                Debug.Log("Unity - Numero de materiales: " + uMeshes.Count);
            }

            // Create GameObjects from Nodes
            GameObject NodeToGameObject (Node node)
            {
                GameObject uObject = new GameObject(node.Name);
                Debug.Log("Unity - Nombre objeto: " + node.Name);

                // Set Mesh
                if (node.HasMeshes)
                {
                    foreach (var aMeshIndices in node.MeshIndices)
                    {
                        var uMesh = uMeshes[aMeshIndices];

                        GameObject uSubObject = new GameObject(uMesh.MeshName);
                        uSubObject.AddComponent<MeshFilter>();
                        uSubObject.AddComponent<MeshRenderer>();
                        uSubObject.AddComponent<MeshCollider>();

                        uSubObject.GetComponent<MeshFilter>().mesh = uMesh.Mesh;
                        uSubObject.GetComponent<MeshRenderer>().material = uMesh.Material;
                        uSubObject.transform.SetParent(uObject.transform, true);
                        uSubObject.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
                    }
                }

                // Transform
                // Decompose Assimp transform into scale, rotation and translation
                Assimp.Vector3D aScale = new Assimp.Vector3D();
                Assimp.Quaternion aQuaternion = new Assimp.Quaternion();
                Assimp.Vector3D aTranslation = new Assimp.Vector3D();
                node.Transform.Decompose(out aScale, out aQuaternion, out aTranslation);

                // Convert Assimp transform into Unity transform and set transformation of game object
                UnityEngine.Quaternion uQuaternion = new UnityEngine.Quaternion(aQuaternion.X, aQuaternion.Y, aQuaternion.Z, aQuaternion.W);
                var euler = uQuaternion.eulerAngles;

                uObject.transform.localScale = new UnityEngine.Vector3(aScale.X, aScale.Y, aScale.Z);
                uObject.transform.localPosition = new UnityEngine.Vector3(aTranslation.X, aTranslation.Y, aTranslation.Z);
                uObject.transform.localRotation = UnityEngine.Quaternion.Euler(aQuaternion.X, aQuaternion.Y, aQuaternion.Z);

                if (node.HasChildren)
                {
                    var countChild = 0;
                    foreach (var child in node.Children)
                    {
                        var uObjectChild = NodeToGameObject(child);
                        uObjectChild.transform.SetParent(uObject.transform, false);
                        Debug.Log("Unity - Parent del objeto child " + countChild + ": " + uObjectChild.transform.parent);
                        countChild++;
                    }
                }

                return uObject;
            }

            return NodeToGameObject(scene.RootNode);
        }
    }
}