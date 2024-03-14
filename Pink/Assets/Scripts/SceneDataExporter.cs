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

        foreach (GameObject go in gameObjects)
        {
            //If object has mesh, print it
            MeshFilter meshFilter = go.GetComponent<MeshFilter>();

            if (meshFilter != null)
                writer.WriteLine(go.name + ": \n      MeshName=" + meshFilter.mesh.name + "\n      Position=" + go.transform.position + ", \n      Rotation=" + go.transform.rotation + ", \n      Scale=" + go.transform.lossyScale + "\n");
        }

        writer.Close();
    }
}
