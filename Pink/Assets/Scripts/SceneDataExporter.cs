using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using System.IO;

public class SceneDataExporter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();

        string path = Application.dataPath + "/SceneData.txt";

        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(" {\n     \"version\": 2,\n     \"gameObjects\": {");

        for (int i = 0; i < gameObjects.Length; i++)
        {
            //If object has mesh, print it
            MeshFilter meshFilter = gameObjects[i].GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                writer.WriteLine("          \"" + gameObjects[i].name + "\": {");
                writer.WriteLine("               \"Mesh Name\": " + "\"" + meshFilter.mesh.name + "\",");
                writer.WriteLine("               \"Position\": " + "\"" + gameObjects[i].transform.position + "\",");
                writer.WriteLine("               \"Rotation\": " + "\"" + gameObjects[i].transform.rotation + "\",");
                writer.Write("               \"Scale\": " + "\"" + gameObjects[i].transform.lossyScale + "\"}");

                if (i < gameObjects.Length - 1)
                    writer.WriteLine(",");
            }

            //writer.WriteLine(gameObjects[i].name + ": \n      MeshName=" + meshFilter.mesh.name + "\n      Position=" + gameObjects[i].transform.position + ", \n      Rotation=" + gameObjects[i].transform.rotation + ", \n      Scale=" + gameObjects[i].transform.lossyScale + "\n");
        }

        writer.WriteLine("\n     }\n}");

        writer.Close();
    }
}
