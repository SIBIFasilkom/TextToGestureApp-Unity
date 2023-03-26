using FasilkomUI.Tutorial;
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
        [SerializeField] InputField m_inputField;
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

            if(TouchScreenKeyboard.visible || m_inputField.isFocused)
            {
                var keyboardSize = _GetKeyboardHeightRatio() * m_wrapper.rect.height;
                m_wrapper.anchoredPosition = new Vector2(0.0f, keyboardSize);
            } else
            {
                m_wrapper.anchoredPosition = Vector2.zero;
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
