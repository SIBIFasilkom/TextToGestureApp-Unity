using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace FasilkomUI.Tutorial
{
    public enum TutorialType
    {
        None,
        RotateCameraTutorial,
        ZoomCameraTutorial,
        SwitchCharacterTutorial,
        GenerateTutorial_Typing,
        GenerateTutorial_Button,
        AnimationSpeedTutorial,
        DictionaryTutorial
    }

    [System.Serializable]
    public class Tutorial
    {
        [SerializeField] TutorialType m_tutorialType;
        public TutorialType TutorialType => m_tutorialType;
        [SerializeField] GameObject m_panel;
        public GameObject Panel => m_panel;
    }

    public class UITutorial : MonoBehaviour
    {
        public static UITutorial Instance { get; private set; }

        public const string PREF_KEY_TUTORIAL_IS_DONE = "tutorialIsDone";

        [SerializeField] Tutorial[] m_tutorials;
        public TutorialType currentTutorial { get; private set; } = TutorialType.None;

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

        private void OnEnable()
        {
            _SetTutorialPanel(TutorialType.RotateCameraTutorial);
        }

        public void CloseButton()
        {
            _SetTutorialPanel(TutorialType.None);
            gameObject.SetActive(false);
            PlayerPrefs.SetString(PREF_KEY_TUTORIAL_IS_DONE, true.ToString());
        }

        public void UpdateTutorial(TutorialType tutorialType)
        {
            if (!gameObject.activeSelf)
                return;

            if (tutorialType != currentTutorial)
                return;

            if ((int)tutorialType + 1 > m_tutorials.Length)
                CloseButton();
            else 
                _SetTutorialPanel(tutorialType + 1);
        }

        private void _SetTutorialPanel(TutorialType tutorialType)
        {
            currentTutorial = tutorialType;
            foreach (var tutorial in m_tutorials)
                tutorial.Panel.SetActive(tutorialType == tutorial.TutorialType);
        }
    }
}
