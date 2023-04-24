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

        [Header("UI Dictionary")]
        [SerializeField] RectTransform m_uiDictionary;
        public bool IsUIDictionaryActive => m_uiDictionary.gameObject.activeSelf;

        [SerializeField] RectTransform m_uiDictionary_search;
        [SerializeField] InputField m_uiDictioanry_search_inputField;
        [SerializeField] RectTransform m_uiDictionary_search_content;
        public RectTransform UIDictionary_Search_Content => m_uiDictionary_search_content;
        [SerializeField] Button m_uiDictionary_search_wordButtonPrefab;
        public Button UIDictionary_Search_WordButtonPrefab => m_uiDictionary_search_wordButtonPrefab;
        [SerializeField] int m_uiDictionary_search_perPageCount;
        public int UIDictionary_Search_PerPageCount => m_uiDictionary_search_perPageCount;

        [SerializeField] RectTransform m_uiDictionary_detail;
        [SerializeField] Text m_uiDictionary_detail_titleText;
        [SerializeField] Text m_uiDictionary_detail_contentText;

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

        [Header("UI Text Result Buttons")]
        [SerializeField] RectTransform m_content;
        public RectTransform TextResultContent => m_content;
        [SerializeField] Button m_uiTextResultButton_prefab;
        public Button TextResultButton => m_uiTextResultButton_prefab;
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

        #region Main Wrapper
        public void GenerateButton()
        {
            TextProcessing.Instance.Generate(m_inputField.text);
        }

        public void ToggleCharacterButton()
        {
            m_currentChar = (m_currentChar == CharacterNames.Andi) ? CharacterNames.Aini : CharacterNames.Andi;
            TextProcessing.Instance.TriggerModel(m_currentChar.ToString());
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

            if(IsUIDictionaryActive)
            {
                CloseDictionary();
                return;
            }

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

        public void SendTextResultToUI(int idx, List<AbstractDatabase> komponenKata2)
        {
            if(m_content.childCount < komponenKata2.Count)
            {
                Debug.LogError("UI Text Result Buttons is less than komponen kata, need at least : " + komponenKata2.Count);
                return;
            }

            for(int i=0; i<m_content.childCount; i++)
            {
                var textResultButton = m_content.GetChild(i);
                bool useButton = i < komponenKata2.Count;
                string str = (useButton) ? komponenKata2[i].id : "";
                bool isSelected = i == idx;
                textResultButton.GetComponent<UITextResultButton>().InitializeButton(useButton, str, isSelected);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(m_content);
        }
        #endregion

        #region UI Dictionary
        public void SearchDictionary()
        {
            m_uiDictionary.gameObject.SetActive(true);
            m_uiDictionary_detail.gameObject.SetActive(false);
            m_uiDictionary_search.gameObject.SetActive(true);
            // aktifin semua ui dictionary word button
            // pake uidictionarywordbutton.initialize
        }

        public void OpenDictionary(string sibi_id)
        {
            m_uiDictionary.gameObject.SetActive(true);
            m_uiDictionary_detail.gameObject.SetActive(true);
            m_uiDictionary_search.gameObject.SetActive(false);
            m_uiDictionary_detail_titleText.text = sibi_id;
            m_uiDictionary_detail_contentText.text = "Cara melakukan " + sibi_id + " disini"; // setup contenttext
        }

        public void CloseDictionary()
        {
            m_uiDictionary.gameObject.SetActive(false);
        }

        public void SearchButton()
        {
            // cek input field search
            // filter???
            // pake uidictionarywordbutton.initialize
        }

        public void GenerateFromDictionaryButton()
        {
            TextProcessing.Instance.Generate(m_uiDictionary_detail_titleText.text);
            CloseDictionary();
        }
        #endregion

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
