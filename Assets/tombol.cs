using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tombol : MonoBehaviour
{
    // Start is called before the first frame update
    public void changescene(string scenename)
    {
        Application.LoadLevel(scenename);
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
                Application.Quit();
            else
                SceneManager.LoadScene(0);
        }
    }
}
