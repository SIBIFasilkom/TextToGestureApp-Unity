﻿using FasilkomUI.Tutorial;
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

        [SerializeField] RectTransform m_wrapper;

        [SerializeField] Text m_textResult;

        [Header("Bottom UI")]
        [SerializeField] Slider m_sliderSpeed;
        [SerializeField] Text m_inputCounter;
        [SerializeField] Color m_inputCounter_defaultColor;
        [SerializeField] Color m_inputCounter_maxColor;

        CharacterNames m_currentChar = CharacterNames.Andi;

        #region Unity Callbacks
        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            TextProcessing.Instance.currentSliderSpeedValue = m_sliderSpeed.value;

            m_wrapper.anchoredPosition = (TouchScreenKeyboard.visible) ? new Vector2(0.0f, TouchScreenKeyboard.area.height) : Vector2.zero;
            if(TouchScreenKeyboard.visible)
            {
                Debug.Log(m_wrapper.anchoredPosition.ToString() + " - " + TouchScreenKeyboard.area.height);
            }

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
            if(UITutorial.Instance && UITutorial.Instance.gameObject.activeSelf)
            {
                UITutorial.Instance.CloseButton();
                return;
            }

            // kamus kebuka ==> tutup kamus

            SceneManager.LoadScene("Menuscreen");
        }

        public void HelpButton()
        {
            UITutorial.Instance?.gameObject.SetActive(true);
        }

        public void UpdateInputCounter(InputField inputField)
        {
            m_inputCounter.text = inputField.text.Length + " / " + inputField.characterLimit;
            m_inputCounter.color = (inputField.text.Length < inputField.characterLimit) ? m_inputCounter_defaultColor : m_inputCounter_maxColor;
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
