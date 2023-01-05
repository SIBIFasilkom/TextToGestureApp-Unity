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

    [SerializeField] GameObject m_debugUI;
    [SerializeField] Slider m_sliderSpeed;

    [SerializeField] Text m_textKalimat;
    public Text textKalimat { get { return m_textKalimat; } }

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

    public void SpeedSliderOnChange(Slider sliderSpeed)
    {
        TextProcessing.Instance.currentSliderSpeedValue = m_sliderSpeed.value;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
#if UNITY_EDITOR
        m_debugUI.gameObject.SetActive(true);
        m_sliderSpeed.value = TextProcessing.Instance.currentSliderSpeedValue;
#endif
    }

    //public void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        if (SceneManager.GetActiveScene().buildIndex == 0)
    //            Application.Quit();
    //        else
    //            SceneManager.LoadScene(0);
    //    }
    //}
}
