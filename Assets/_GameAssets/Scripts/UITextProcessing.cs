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

        [Header("Bottom UI")]
        [SerializeField] Slider m_sliderSpeed;

        CharacterNames m_currentChar = CharacterNames.Andi;

        #region Unity Callbacks
        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            TextProcessing.Instance.currentSliderSpeedValue = m_sliderSpeed.value;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                BackButton();
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

        public void BackButton()
        {
            // kalo nganu jangan balik ke menuscreen
            SceneManager.LoadScene("Menuscreen");
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
