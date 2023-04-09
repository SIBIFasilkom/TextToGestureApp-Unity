using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

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

        [SerializeField] HorizontalScrollSnap m_tutorialImages;
        [SerializeField] Text m_contentText;
        [SerializeField] Text m_countText;
        [SerializeField] Button m_prevButton;
        [SerializeField] Button m_nextButton;

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
        }

        public void CloseButton()
        {
            gameObject.SetActive(false);
            PlayerPrefs.SetString(PREF_KEY_TUTORIAL_IS_DONE, true.ToString());
        }

        public void UpdateTutorial()
        {
            int currentCount = m_tutorialImages.CurrentPage;
            m_contentText.text = m_tutorials[currentCount].Content;
            m_countText.text = (currentCount + 1) + "/" + m_tutorials.Length;
        }
    }
}
