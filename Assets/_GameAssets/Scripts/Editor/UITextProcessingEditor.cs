using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FasilkomUI;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(UITextProcessing))]
public class UITextProcessingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UITextProcessing tp = (UITextProcessing)target;
        if (GUILayout.Button("Cache All Text Result Buttons"))
        {
            for(int i=0; i<tp.InstantiateButtonCount; i++)
            {
                Instantiate(tp.TextResultButton, tp.TextResultContent);
            }
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("Set All Text Result Buttons Inactive"))
        {
            for (int i = 0; i < tp.InstantiateButtonCount; i++)
            {
                tp.TextResultContent.GetChild(i).gameObject.SetActive(false);
            }
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("Destroy All Text Result Buttons"))
        {
            for (int i = 0; i < tp.InstantiateButtonCount; i++)
            {
                DestroyImmediate(tp.TextResultContent.GetChild(0).gameObject);
            }
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("Cache All UI Dictionary Word Buttons"))
        {
            for (int i = 0; i < tp.UIDictionary_Search_PerPageCount; i++)
            {
                Instantiate(tp.UIDictionary_Search_WordButtonPrefab, tp.UIDictionary_Search_Content);
            }
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("Destroy All UI Dictionary Word Buttons"))
        {
            for (int i = 0; i < tp.UIDictionary_Search_PerPageCount; i++)
            {
                DestroyImmediate(tp.UIDictionary_Search_Content.GetChild(0).gameObject);
            }
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif