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
        writer.WriteLine(" {\n     \"version\": 3,\n     \"gameObjects\": {");

        for (int i = 0; i < gameObjects.Length; i++)
        {
            //If object has mesh, print it
            MeshFilter meshFilter = gameObjects[i].GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                string modelDataPath = AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
                Debug.Log("filepath: " + modelDataPath + ", objectname: " + gameObjects[i].name);
                //GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(modelDataPath);

                //if (!meshFilter.sharedMesh.name.Equals(model.name))
                //{
                //    Debug.Log(model.name);
                //}

                if(modelDataPath == "Library/unity default resources")
                {
                    writer.WriteLine("          \"" + gameObjects[i].name + "\": {");
                    writer.WriteLine("               \"Mesh Name\": " + "\"" + meshFilter.mesh.name + "\",");
                    writer.WriteLine("               \"Position\": " + "\"" + gameObjects[i].transform.position + "\",");
                    writer.WriteLine("               \"Rotation\": " + "\"" + gameObjects[i].transform.rotation.eulerAngles + "\",");
                    writer.Write("               \"Scale\": " + "\"" + gameObjects[i].transform.lossyScale + "\"}");
                }
                else if (modelDataPath == "")
                {
                    writer.WriteLine("          \"" + gameObjects[i].name + "\": {");
                    writer.WriteLine("               \"Mesh Name\": " + "\"Unknown ProBuilderMesh\",");
                    writer.WriteLine("               \"Position\": " + "\"" + gameObjects[i].transform.position + "\",");
                    writer.WriteLine("               \"Rotation\": " + "\"" + gameObjects[i].transform.rotation.eulerAngles + "\",");
                    writer.Write("               \"Scale\": " + "\"" + gameObjects[i].transform.lossyScale + "\"}");
                }
                else
                {
                    string[] segments = modelDataPath.Split('/');
                    string filename = segments[segments.Length-1];

                    writer.WriteLine("          \"" + gameObjects[i].name + "\": {");
                    writer.WriteLine("               \"Mesh Name\": " + "\"" + filename + "\",");
                    writer.WriteLine("               \"Position\": " + "\"" + gameObjects[i].transform.position + "\",");
                    writer.WriteLine("               \"Rotation\": " + "\"" + gameObjects[i].transform.rotation.eulerAngles + "\",");
                    writer.Write("               \"Scale\": " + "\"" + gameObjects[i].transform.lossyScale + "\"}");
                }

                if (i < gameObjects.Length - 1)
                    writer.WriteLine(",");
            }
        }

        writer.WriteLine("\n     }\n}");

        writer.Close();
    }
}
