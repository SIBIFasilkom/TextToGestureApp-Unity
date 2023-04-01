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
        if (GUILayout.Button("Cache All Dictionary Buttons"))
        {
            for(int i=0; i<tp.InstantiateButtonCount; i++)
            {
                Instantiate(tp.DictionaryButtonPrefab, tp.DictionaryContent);
            }
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("Destroy All Dictionary Buttons"))
        {
            for (int i = 0; i < tp.InstantiateButtonCount; i++)
            {
                DestroyImmediate(tp.DictionaryContent.GetChild(0).gameObject);
            }
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif