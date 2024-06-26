#if (UNITY_EDITOR) 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using System.IO;
using UnityEditor;

public class SceneDataExporter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();

        string path = Application.dataPath + "/SceneData.txt";
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(" {\n     \"version\": 4,\n     \"gameObjects\": {");

        string collidersPath = Application.dataPath + "/CollidersData.txt";
        StreamWriter collisionWriter = new StreamWriter(collidersPath, false);
        collisionWriter.WriteLine(" {\n     \"version\": 4,\n     \"colliders\": {");

        bool skipComma = true;

        for (int i = 0; i < gameObjects.Length; i++)
        {
            //If object has mesh, print it
            MeshFilter meshFilter = gameObjects[i].GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                //Attempts to find a mesh filename in Unity Assets
                string modelDataPath = AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
                //Debug.Log("filepath: " + modelDataPath + ", objectname: " + gameObjects[i].name, gameObjects[i].transform);

                string name = gameObjects[i].name;
                if (gameObjects[i].name == "default")
                {
                    name = gameObjects[i].transform.parent.name;
                }

                writer.WriteLine("          \"" + name + "\": {");
                writer.WriteLine("               \"Object Name\": " + "\"" + name.Split(" ")[0] + "\",");

                //Model is default resource, ie Cube or Capsule
                if (modelDataPath == "Library/unity default resources")
                {
                    writer.WriteLine("               \"Mesh Name\": " + "\"" + meshFilter.mesh.name + "\",");
                    writer.WriteLine("               \"Position\": " + "\"" + gameObjects[i].transform.position + "\",");
                    writer.WriteLine("               \"Rotation\": " + "\"" + gameObjects[i].transform.rotation.eulerAngles + "\",");
                    writer.Write("               \"Scale\": " + "\"" + gameObjects[i].transform.lossyScale + "\"}");
                }
                //Model is ProBuilderMesh
                //ProBuilderMeshes assume objects are only Cubes/Rectangles have rotations of 90 degrees only
                //Exporter only gets external bounds of mesh, which is why cubes are assumed
                //Therefore exporter ignores rotation
                else if (modelDataPath == "")
                {
                    Vector3 probuilderScale = gameObjects[i].GetComponent<Renderer>().bounds.extents;

                    writer.WriteLine("               \"Mesh Name\": " + "\"Unknown ProBuilderMesh\",");
                    writer.WriteLine("               \"Position\": " + "\"" + gameObjects[i].transform.position + "\",");
                    writer.WriteLine("               \"Rotation\": " + "\"" + new Vector3(0,0,0) + "\",");
                    writer.Write("               \"Scale\": " + "\"" + probuilderScale + "\"}");
                }
                else
                {
                    string[] segments = modelDataPath.Split('/');
                    string filename = segments[segments.Length-1];

                    writer.WriteLine("               \"Mesh Name\": " + "\"" + filename + "\",");
                    writer.WriteLine("               \"Position\": " + "\"" + gameObjects[i].transform.position + "\",");
                    writer.WriteLine("               \"Rotation\": " + "\"" + gameObjects[i].transform.rotation.eulerAngles + "\",");
                    writer.Write("               \"Scale\": " + "\"" + gameObjects[i].transform.lossyScale + "\"}");
                }

                if (i < gameObjects.Length - 1)
                    writer.WriteLine(",");
            }
            else
            {
                if (gameObjects[i].GetComponent<BoxCollider>() != null)
                {
                    if (skipComma)
                    {
                        skipComma = false;
                    }
                    else
                    {
                        collisionWriter.WriteLine(",");
                    }

                    Debug.Log("objectname: " + gameObjects[i].name, gameObjects[i].transform);

                    string name = gameObjects[i].name;
                    if (gameObjects[i].name == "default")
                    {
                        name = gameObjects[i].transform.parent.name;
                    }

                    collisionWriter.WriteLine("          \"" + name + "\": {");
                    collisionWriter.WriteLine("               \"Object Name\": " + "\"" + name.Split(" ")[0] + "\",");

                    collisionWriter.WriteLine("               \"Position\": " + "\"" + gameObjects[i].transform.position + "\",");
                    collisionWriter.WriteLine("               \"Rotation\": " + "\"" + gameObjects[i].transform.rotation.eulerAngles + "\",");
                    collisionWriter.Write("               \"Scale\": " + "\"" + gameObjects[i].transform.lossyScale + "\"}");
                }
            }
        }

        writer.WriteLine("\n     }\n}");
        collisionWriter.WriteLine("\n     }\n}");

        writer.Close();
        collisionWriter.Close();
    }
}
#endif