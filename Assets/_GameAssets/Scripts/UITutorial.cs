using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FasilkomUI.Tutorial
{
    [System.Serializable]
    public class Tutorial
    {
        [SerializeField] Sprite m_sprite;
        public Sprite Sprite => m_sprite;
        [SerializeField] string m_contentString;
        public string Content => m_contentString;
    }

    public class UITutorial : MonoBehaviour
    {
        public static UITutorial Instance { get; private set; }

        public const string PREF_KEY_TUTORIAL_IS_DONE = "tutorialIsDone";

        [SerializeField] Tutorial[] m_tutorials;

        [SerializeField] Image m_tutorialImage;
        [SerializeField] Text m_contentText;
        [SerializeField] Text m_countText;
        [SerializeField] Button m_prevButton;
        [SerializeField] Button m_nextButton;

        int m_currentCount = 0;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            bool tutorialIsDone = PlayerPrefs.GetString(PREF_KEY_TUTORIAL_IS_DONE, false.ToString()) == true.ToString();
            gameObject.SetActive(!tutorialIsDone);
            if (tutorialIsDone)
                return;
        }

        private void OnEnable()
        {
            m_currentCount = 0;
            _UpdateTutorial();
        }

        public void IncreaseCurrentCountButton(int inc)
        {
            m_currentCount = Mathf.Clamp(m_currentCount + inc, 0, m_tutorials.Length - 1);
            _UpdateTutorial();
        }

        public void CloseButton()
        {
            gameObject.SetActive(false);
            PlayerPrefs.SetString(PREF_KEY_TUTORIAL_IS_DONE, true.ToString());
        }

        private void _UpdateTutorial()
        {
            m_tutorialImage.sprite = m_tutorials[m_currentCount].Sprite;
            m_contentText.text = m_tutorials[m_currentCount].Content;
            m_countText.text = (m_currentCount + 1) + "/" + m_tutorials.Length;
            m_prevButton.interactable = m_currentCount > 0;
            m_nextButton.interactable = m_currentCount + 1 < m_tutorials.Length;
        }
    }
}
