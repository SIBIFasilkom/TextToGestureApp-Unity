using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMenuscreen : MonoBehaviour
{
    #region Unity's Callback
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackButton();
        }
    }
    #endregion

    public void BackButton()
    {
        Application.Quit();
    }

    public void SIBIButton()
    {
        UILoadingScreen.loadingSceneName = "MainSIBI";
        SceneManager.LoadScene("Loading");
    }

    public void BISINDOButton()
    {
        UILoadingScreen.loadingSceneName = "MainBISINDO";
        SceneManager.LoadScene("Loading");
    }
}
