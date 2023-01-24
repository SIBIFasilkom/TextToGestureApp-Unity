using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoadingScreen : MonoBehaviour
{
    [SerializeField] TMP_Text m_loadingText;

    #region Adroid Callbacks
    public void LoadScene(string sceneName)
    {
        StartCoroutine(_LoadSceneHandler(sceneName));
    }

    private void _LoadHasDone()
    {
        using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                obj_Activity.Call("loadHasDone", "");
            }
        }
    }
    #endregion

    private void Start()
    {
        // kedepannya ga pake start, dipanggil dari android studio.
        LoadScene("MainSIBI");
    }

    private IEnumerator _LoadSceneHandler(string sceneName)
    {
        m_loadingText.text = "Tunggu Sebentar";
        yield return new WaitForSeconds(0.25f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            int progress = Mathf.RoundToInt(Mathf.Clamp01(operation.progress / 0.9f) * 100);
            m_loadingText.text = "Tunggu Sebentar\n<size=12>(" + progress + " %)</size>";

            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        //_LoadHasDone();
    }
}
