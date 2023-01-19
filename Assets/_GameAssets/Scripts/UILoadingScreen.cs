using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoadingScreen : MonoBehaviour
{
    [SerializeField] TMP_Text m_loadingText;

    private void Start()
    {
        StartCoroutine(_LoadSceneHandler("MainSIBI"));
    }

    private IEnumerator _LoadSceneHandler(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            int progress = Mathf.RoundToInt(Mathf.Clamp01(operation.progress / 0.9f) * 100);
            m_loadingText.text = "Tunggu Sebentar\n(" + progress + " %)";

            if (operation.progress >= 0.9f)
            {
                print("coy");
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        // panggil show ui
    }
}
