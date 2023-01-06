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

    public void GenerateButton(InputField inputField)
    {
        TextProcessing.Instance.getInputFromAndroid(inputField.text);
    }

    public void ToggleCharacterButton()
    {
        m_currentChar = (m_currentChar == CharacterNames.Andi) ? CharacterNames.Aini : CharacterNames.Andi;
        TextProcessing.Instance.triggerModel(m_currentChar.ToString());
    }

    public void SendTextResultToUI(string resultText)
    {
        m_textResult.text = resultText;
    }

    public void DebugTextOutput(List<string> words)
    {
        if(m_editorDebugMode)
        {
            string output = "";

            foreach (string word in words)
            {
                output += word + ";";
            }

            m_textDebug.text = output;
        }
    } 

    private void Awake()
    {
        Instance = this;
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
}
