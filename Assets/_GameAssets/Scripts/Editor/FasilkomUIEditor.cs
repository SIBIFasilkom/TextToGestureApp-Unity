using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;

namespace FasilkomUI.Editor
{
    public class FasilkomUIEditor
    {
        [MenuItem("Fasilkom-UI/WriteFilenamesList")]
        static void WriteFilenamesAsJSON()
        {
            string path = Application.dataPath + "/_GameAssets/Animations/v1/_Raw/SIBI raw/SIBI ga tau ada di editor ini entah kenapa/";
            string outputPath = Application.dataPath + "/_GameAssets/Filenames_SIBI raw 2.txt";
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
            Debug.Log("Toggle from menuscreen (need ro reenable everytime you close the project) : " + togglePlayFromMenuscreen + " - " + sceneAsset);
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

        [MenuItem("Fasilkom-UI/FixJSONFilesArgumentError")]
        public static void FixJSONFiles()
        {
            Debug.Log("Trying to fix all json files...");
            string path = "Assets/_GameAssets/Database/v2/";
            string[] fileNames = { "1-sibi.json", "2-alt_sibi.json", "3-imbuhan_sibi_generated_1.json" };
            foreach(string fileName in fileNames)
            {
                Debug.Log("Adding {\"list\": to "+ path + fileName);
                string renamedFile = path + fileName + ".orig";
                File.Move(path + fileName, renamedFile); //rename the original file
                char[] buffer = new char[1000000]; //create a buffer for copying data
                using (StreamReader sr = new StreamReader(renamedFile)) //open the renamed file for reading
                using (StreamWriter sw = new StreamWriter(path + fileName, false)) //open the original file for writing
                {
                    sw.Write("{\"list\":"); //write the text at the start
                    int read;
                    while ((read = sr.Read(buffer, 0, buffer.Length)) > 0) //copy data from reader to writer
                        sw.Write(buffer, 0, read);
                    sw.Write("}"); //write the text at the end
                }
                File.Delete(renamedFile); //delete the renamed file
            }
            Debug.Log("Done!");
        }
    }

    //[CustomEditor(typeof(AbstractLanguage))]
    //public class TextProcessingEditor : UnityEditor.Editor
    //{
    //    public override void OnInspectorGUI()
    //    {
    //        DrawDefaultInspector();

    //        AbstractLanguage tp = (AbstractLanguage)target;
    //        if (GUILayout.Button("Fix JSON files argument error"))
    //        {


    //            EditorUtility.SetDirty(target);
    //            AssetDatabase.SaveAssets();
    //            AssetDatabase.Refresh();
    //        }
    //    }
    //}
}
#endif