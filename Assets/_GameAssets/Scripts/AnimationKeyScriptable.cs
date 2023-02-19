using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace FasilkomUI
{
    [CreateAssetMenu(fileName = "AnimationKey", menuName = "Fasilkom-UI/AnimationKey", order = 1)]
    public class AnimationKeyScriptable : ScriptableObject
    {
        public Dictionary<string, Animation> m_animationKeys;

        [MenuItem("Fasilkom-UI/WriteFilenamesList")]
        static void WriteFilenamesAsJSON()
        {
            string path = Application.dataPath + "/_GameAssets/Animations/_Raw (Unused For Now)/BISINDO Raw/";
            string outputPath = Application.dataPath + "/_GameAssets/Filenames_BISINDO Raw.txt";
            if (string.IsNullOrWhiteSpace(path))
            {
                Debug.LogError("Path is null or whitespace, please check Animation Key Scriptable");
                return;
            }

            Debug.Log("Looking for files in " + path);

            string[] entries = Directory.GetFileSystemEntries(path, "*.fbx", SearchOption.AllDirectories);
            string content = "";
            foreach (string entry in entries)
            {
                content += Path.GetFileName(entry).Replace(".fbx", "") + Environment.NewLine;
            }

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            File.WriteAllText(outputPath, content);
            AssetDatabase.Refresh();
        }
    }
}
