using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FasilkomUI
{
    public class UITextProcessing : MonoBehaviour
    {
        public static UITextProcessing Instance { get; private set; }

        [SerializeField] Text m_textResult;

        [Header("Debug Mode")]
        [SerializeField] bool m_editorDebugMode = false;
        [SerializeField] GameObject m_debugUI;
        [SerializeField] Slider m_sliderSpeed;
        [SerializeField] Text m_textDebug;

        CharacterNames m_currentChar = CharacterNames.Andi;

        Quaternion m_originalCamRotation;

        #region Unity Callbacks
        private void Awake()
        {
#if UNITY_EDITOR
            m_editorDebugMode = true;
#endif

            Instance = this;
        }

        private void Start()
        {
            m_originalCamRotation = transform.rotation;
        }

        private void Update()
        {
            if (m_editorDebugMode)
            {
                m_debugUI.gameObject.SetActive(true);
                TextProcessing.Instance.currentSliderSpeedValue = m_sliderSpeed.value;

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (SceneManager.GetActiveScene().buildIndex == 0)
                        Application.Quit();
                    else
                        SceneManager.LoadScene(0);
                }
            }
        }

        private void LateUpdate()
        {
            transform.rotation = Camera.main.transform.rotation * m_originalCamRotation;
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

        public void DebugTextOutput(List<Gesture> words)
        {
            if (m_editorDebugMode)
            {
                string output = "";

                foreach (Gesture word in words)
                {
                    output += word.id + ";";
                }

                m_textDebug.text = output;
            }
        }

        public void SendTextResultToUI(int idx, List<Gesture> komponenKata2)
        {
            string text = "";
            for (int i = 0; i < komponenKata2.Count; i++)
            {
                string str = "";

                str = komponenKata2[i].id;

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

}
