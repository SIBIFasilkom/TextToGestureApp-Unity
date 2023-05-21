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
        [SerializeField] InputField m_uiDictionary_search_inputField;
        [SerializeField] RectTransform m_uiDictionary_search_content;
        public RectTransform UIDictionary_Search_Content => m_uiDictionary_search_content;
        [SerializeField] ToggleGroup m_uiDictionary_search_pageContent;
        public ToggleGroup UIDictionary_Search_PageContent => m_uiDictionary_search_pageContent;
        [SerializeField] Button m_uiDictionary_search_wordButtonPrefab;
        public Button UIDictionary_Search_WordButtonPrefab => m_uiDictionary_search_wordButtonPrefab;
        [SerializeField] Toggle m_uiDictionary_search_pageButtonPrefab;
        public Toggle UIDictionary_Search_PageButtonPrefab => m_uiDictionary_search_pageButtonPrefab;
        [SerializeField] int m_uiDictionary_search_perPageCount;
        public int UIDictionary_Search_PerPageCount => m_uiDictionary_search_perPageCount;
        [SerializeField] int m_uiDictionary_search_pageCount;
        public int UIDictionary_Search_PageCount => m_uiDictionary_search_pageCount;

        [SerializeField] RectTransform m_uiDictionary_detail;
        [SerializeField] Text m_uiDictionary_detail_titleText;
        [SerializeField] Text m_uiDictionary_detail_contentText;

        List<string> m_languageKeys = new List<string>();

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
            m_sliderSpeed.value = TextProcessing.Instance.currentSliderSpeedValue;

            m_wrapper.anchoredPosition = new Vector2(0.0f, m_keyboardSize * m_keyboardOpenPercentage);
            if (m_inputField.isFocused)
            {
                m_keyboardSize = _GetKeyboardHeightRatio() * m_wrapper.rect.height;
                m_keyboardOpenPercentage = Mathf.Min(m_keyboardOpenPercentage + Time.deltaTime * m_keyboardOpenSpeed, 1.0f);

                UITutorial.Instance?.UpdateTutorial(TutorialType.GenerateTutorial_Typing);
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

            UITutorial.Instance?.UpdateTutorial(TutorialType.GenerateTutorial_Button);
        }

        public void ToggleCharacterButton()
        {
            m_currentChar = (m_currentChar == CharacterNames.Andi) ? CharacterNames.Aini : CharacterNames.Andi;
            TextProcessing.Instance.TriggerModel(m_currentChar.ToString());

            UITutorial.Instance?.UpdateTutorial(TutorialType.SwitchCharacterTutorial);
        }

        public void OnSliderZoomChange()
        {
            var zoomClamp = TouchCameraControl.Instance.zoomClamp;
            float zoom = zoomClamp.min + (zoomClamp.max - zoomClamp.min) * (1 - m_sliderZoom.value);
            TouchCameraControl.Instance.UpdateAllCamerasZoom(zoom);

            if(m_sliderZoom.value > 0.25)
                UITutorial.Instance?.UpdateTutorial(TutorialType.ZoomCameraTutorial);
        }

        public void OnSliderSpeedChange()
        {
            TextProcessing.Instance.currentSliderSpeedValue = m_sliderSpeed.value;

            if (m_sliderSpeed.value < 0.75 || m_sliderSpeed.value > 1.25)
                UITutorial.Instance?.UpdateTutorial(TutorialType.AnimationSpeedTutorial);
        }

        public void BackButton()
        {
            if(UITutorial.Instance?.gameObject.activeSelf == true)
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
            if (UITutorial.Instance?.gameObject.activeSelf == true)
                return;

            UITutorial.Instance?.gameObject.SetActive(true);
        }

        public void UpdateInputCounter(InputField inputField)
        {
            m_inputCounter.text = inputField.text.Length + " / " + inputField.characterLimit;
            m_inputCounter.color = (inputField.text.Length < inputField.characterLimit) ? m_inputCounter_defaultColor : m_inputCounter_maxColor;
        }

        public void SendTextResultToUI<T>(int idx, List<T> komponenKata2) where T : AbstractDatabaseLanguage
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
        public void InitializeUIDictionaryDatabase<T>(Dictionary<string, T> languageDatabase) where T : AbstractDatabase
        {
            m_languageKeys = new List<string>(languageDatabase.Keys);
            _HandleSearchContentChild();
        }

        public void SearchDictionary()
        {
            m_uiDictionary.gameObject.SetActive(true);
            m_uiDictionary_detail.gameObject.SetActive(false);
            m_uiDictionary_search.gameObject.SetActive(true);

            UITutorial.Instance?.UpdateTutorial(TutorialType.DictionaryTutorial);
        }

        public void OpenDictionary(string language_id)
        {
            m_uiDictionary.gameObject.SetActive(true);
            m_uiDictionary_detail.gameObject.SetActive(true);
            m_uiDictionary_search.gameObject.SetActive(false);

            var contentLanguage = TextProcessing.Instance.Language.GetHowToLanguage(language_id);
            m_uiDictionary_detail_titleText.text = language_id;
            m_uiDictionary_detail_contentText.text = contentLanguage;

            UITutorial.Instance?.UpdateTutorial(TutorialType.DictionaryTutorial);
        }

        public void CloseDictionary()
        {
            m_uiDictionary.gameObject.SetActive(false);
        }

        public void SearchButton()
        {
            var searchText = m_uiDictionary_search_inputField.text.ToLower();
            _HandleSearchContentChild(searchText);
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

        private void _HandleSearchContentChild(string searchText = "")
        {
            // bikin coroutine, sama di up ke atas scrollbarnya tolong
            for (int i = 0; i < m_uiDictionary_search_content.childCount; i++)
            {
                var wordButtonChild = m_uiDictionary_search_content.GetChild(i);
                var wordButton = wordButtonChild.GetComponent<UIDictionaryWordButton>();
                var isActive = i < m_languageKeys.Count && m_languageKeys[i].Contains(searchText);
                wordButton.InitializeButton(isActive, (isActive) ? m_languageKeys[i] : "");
            }
        }
    }
}
