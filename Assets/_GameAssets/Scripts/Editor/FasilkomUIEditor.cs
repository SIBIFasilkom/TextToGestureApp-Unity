using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using UnityEditor.SceneManagement;

namespace FasilkomUI.Editor
{
    public class FasilkomUIEditor
    {
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

        public static bool togglePlayFromMenuscreen = true;

        [MenuItem("Fasilkom-UI/TogglePlayFromMenuscreen")]
        public static void PlayFromMenuscreen()
        {
            togglePlayFromMenuscreen = !togglePlayFromMenuscreen;
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/_GameAssets/Scenes/Menuscreen.unity");
            Debug.Log("Toggle from menuscreen : " + togglePlayFromMenuscreen + " - " + sceneAsset);
            if (togglePlayFromMenuscreen)
                EditorSceneManager.playModeStartScene = sceneAsset;
            else
                EditorSceneManager.playModeStartScene = null;
        }

        [MenuItem("Fasilkom-UI/SimulateOpenTouchScreenKeyboard")]
        public static void SimulateOpenTouchScreenKeyboard()
        {
            Debug.Log("Simulating touch screen keyboard... Nah, jk. This function doesn't work lmao");
            TouchScreenKeyboard.Open("");
        }
    }
}
