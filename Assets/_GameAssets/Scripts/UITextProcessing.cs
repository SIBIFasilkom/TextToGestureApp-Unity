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
        [SerializeField] Slider m_sliderZoom;
        [SerializeField] Slider m_sliderSpeed;
        [SerializeField] InputField m_inputField;
        [SerializeField] Text m_inputCounter;
        [SerializeField] Color m_inputCounter_defaultColor;
        [SerializeField] Color m_inputCounter_maxColor;

        CharacterNames m_currentChar = CharacterNames.Andi;

        [SerializeField] float m_keyboardOpenSpeed = 10.0f;
        float m_keyboardSize = 0.0f;
        float m_keyboardOpenPercentage = 0.0f;

        [Header("Temp")]
        [SerializeField] RectTransform m_content;
        public RectTransform DictionaryContent => m_content;
        [SerializeField] Button m_uiDictionaryButton_prefab;
        public Button DictionaryButtonPrefab => m_uiDictionaryButton_prefab;
        [SerializeField] int m_instantiateButtonCount;
        public int InstantiateButtonCount => m_instantiateButtonCount;

        #region Unity Callbacks
        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            m_sliderZoom.value = 1 - TouchCameraControl.Instance.GenerateCameraZoomPercentage();

            TextProcessing.Instance.currentSliderSpeedValue = m_sliderSpeed.value;

            m_wrapper.anchoredPosition = new Vector2(0.0f, m_keyboardSize * m_keyboardOpenPercentage);
            if (m_inputField.isFocused)
            {
                m_keyboardSize = _GetKeyboardHeightRatio() * m_wrapper.rect.height;
                m_keyboardOpenPercentage = Mathf.Min(m_keyboardOpenPercentage + Time.deltaTime * m_keyboardOpenSpeed, 1.0f);
            } else
            {
                m_keyboardOpenPercentage = Mathf.Max(m_keyboardOpenPercentage - Time.deltaTime * m_keyboardOpenSpeed, 0.0f);
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

        public void OnSliderZoomChange()
        {
            var zoomClamp = TouchCameraControl.Instance.zoomClamp;
            float zoom = zoomClamp.min + (zoomClamp.max - zoomClamp.min) * (1 - m_sliderZoom.value);
            TouchCameraControl.Instance.UpdateAllCamerasZoom(zoom);
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

        private float _GetKeyboardHeightRatio()
        {
            if (Application.isEditor)
            {
                return 0.4f;       
            }

#if UNITY_ANDROID
            using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");
                using (AndroidJavaObject rect = new AndroidJavaObject("android.graphics.Rect"))
                {
                    View.Call("getWindowVisibleDisplayFrame", rect);
                    return (float)(Screen.height - rect.Call<int>("height")) / Screen.height;
                }
            }
#else
        return (float)TouchScreenKeyboard.area.height / Screen.height;
#endif
        }
    }
}
