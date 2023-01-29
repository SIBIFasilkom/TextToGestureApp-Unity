using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILoadingScreen : MonoBehaviour
{
    [SerializeField] Text m_loadingText;

    #region Adroid Callbacks
    public void LoadScene(string sceneName)
    {
        StartCoroutine(_LoadSceneHandler(sceneName));
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
    }
}
