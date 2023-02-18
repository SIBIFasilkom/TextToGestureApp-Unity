using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

[CreateAssetMenu(fileName = "AnimationKey", menuName = "Fasilkom-UI/AnimationKey", order = 1)]
public class AnimationKeyScriptable : ScriptableObject
{
    [MenuItem("Fasilkom-UI/WriteFilenamesAsJSON")]
    static void WriteFilenamesAsJSON()
    {
        string path = Application.dataPath + "/_GameAssets/Animations/_Raw (Unused For Now)/SIBI Raw/Data Gerakan";
        string outputPath = Application.dataPath + "/_GameAssets/SIBI Raw.json";
        if (string.IsNullOrWhiteSpace(path))
        {
            Debug.LogError("Path is null or whitespace, please check Animation Key Scriptable");
            return;
        }

        Debug.Log("Looking for files in " + path);

        //DirectoryInfo dir = new DirectoryInfo(path);
        //FileInfo[] info = dir.GetFiles("*.*");

        //foreach (FileInfo f in info)
        //{
        //    Debug.Log(f.ToString());
        //}

        string[] entries = Directory.GetFileSystemEntries(path, "*", SearchOption.AllDirectories);
        string content = "";
        foreach(string entry in entries)
        {
            content = JsonUtility.ToJson(entry) + Environment.NewLine;
            Debug.Log(content + " " + entry);
        }

        if(File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        File.WriteAllText(outputPath, content);
    }
}
