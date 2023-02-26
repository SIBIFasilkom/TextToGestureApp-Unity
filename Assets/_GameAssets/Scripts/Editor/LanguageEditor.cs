using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace FasilkomUI
{
    [CreateAssetMenu(fileName = "Language Scriptable", menuName = "Fasilkom-UI/Language Scriptable", order = 1)]
    public class LanguageScriptable : ScriptableObject
    {
        public Dictionary<string, Animation> m_animationKeys;

        [MenuItem("Fasilkom-UI/WriteFilenamesList")]
        static void WriteFilenamesAsJSON()
        {
            string path = Application.dataPath + "/_GameAssets/Animations/_Raw (Unused For Now)/BISINDO raw/";
            string outputPath = Application.dataPath + "/_GameAssets/Filenames_BISINDO raw.txt";
            string fileType = "fbx";
            if (string.IsNullOrWhiteSpace(path))
            {
                Debug.LogError("Path is null or whitespace, please check Animation Key Scriptable");
                return;
            }

            Debug.Log("Looking for files in " + path);

            string[] entries = Directory.GetFileSystemEntries(path, "*." + fileType, SearchOption.AllDirectories);
            string content = "";
            foreach (string entry in entries)
            {
                content += Path.GetFileName(entry).Replace("." + fileType, "") + Environment.NewLine;
            }

            if (File.Exists(outputPath))
                File.Delete(outputPath);

            File.WriteAllText(outputPath, content);
            AssetDatabase.Refresh();
        }

        [MenuItem("Fasilkom-UI/SplitStringTest")]
        static void SplitStringTest()
        {
            string word = "ini test split string";
            string[] ayy = AbstractLanguageUtility.SplitString(word);
            foreach (string ay in ayy)
            {
                Debug.Log(ay);
            }
        }
    }
}
