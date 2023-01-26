using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum CharacterNames
{
    Andi,
    Aini
}

public class UITextProcessing : MonoBehaviour
{
    public static UITextProcessing Instance { get; private set; }

    [SerializeField] Text m_textResult;

    [Header("Debug Mode")]
    [SerializeField] bool m_editorDebugMode = false;
    [SerializeField] GameObject m_debugUI;
    [SerializeField] Slider m_sliderSpeed;
    [SerializeField] Toggle m_toggleTransition;
    [SerializeField] Toggle m_toggleLog;
    [SerializeField] Text m_textDebug;

    CharacterNames m_currentChar = CharacterNames.Andi;

    #region AndroidCallback
    private void _LoadHasDone()
    {
        try
        {
            using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    obj_Activity.Call("loadHasDone", "");
                }
            }
        } catch
        {
            m_editorDebugMode = true;
        }
    }
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        Instance = this;

        _LoadHasDone();
    }

    private void Update()
    {
        if(m_editorDebugMode)
        {
            m_debugUI.gameObject.SetActive(true);
            TextProcessing.Instance.currentSliderSpeedValue = m_sliderSpeed.value;
            TextProcessing.Instance.currentUseTransition = m_toggleTransition.isOn;
            TextProcessing.Instance.currentUseLog = m_toggleLog.isOn;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (SceneManager.GetActiveScene().buildIndex == 0)
                    Application.Quit();
                else
                    SceneManager.LoadScene(0);
            }
        }
    }
    #endregion

    public void GenerateButton(InputField inputField)
    {
        TextProcessing.Instance.getInputFromAndroid(inputField.text);
    }

    public void ToggleCharacterButton()
    {
        m_currentChar = (m_currentChar == CharacterNames.Andi) ? CharacterNames.Aini : CharacterNames.Andi;
        TextProcessing.Instance.triggerModel(m_currentChar.ToString());
    }

    public void DebugTextOutput(List<string> words)
    {
        if (m_editorDebugMode)
        {
            string output = "";

            foreach (string word in words)
            {
                output += word + ";";
            }

            m_textDebug.text = output;
        }
    }

    public void SendTextResultToUI(int idx, List<string> komponenKata2)
    {
        string text = "";
        for (int i = 0; i < komponenKata2.Count; i++)
        {
            string str = "";

            if (komponenKata2.Count > 1)
            {
                if (komponenKata2[i].Length > 1)
                {
                    if (i == 0)
                    {
                        str = char.ToUpper(komponenKata2[i][0]) + komponenKata2[i].Substring(1);
                    }
                    else
                    {
                        str = komponenKata2[i];
                    }
                }
                else
                {
                    str = komponenKata2[i];
                }
            }
            else
            {
                str = komponenKata2[i];
            }

            if (idx == i)
            {
                text += " <color=#955BA5>" + str + "</color>";
            }
            else
            {
                text += " " + str;
            }
        }

        m_textResult.text = text;
    }
}
