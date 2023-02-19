using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using Animancer;

namespace FasilkomUI
{
    public enum CharacterNames
    {
        Andi,
        Aini
    }

    public class TextProcessing : MonoBehaviour
    {
        public static TextProcessing Instance { get; private set; }

        public const string PREF_KEY_CURRENT_CHARACTER = "currentCharacter";

        [SerializeField] AbstractLanguage m_language;

        [Header("Character")]
        [SerializeField] GameObject _AndiModel;
        [SerializeField] GameObject _AiniModel;

        [Header("Current Variables")]
        public float currentSliderSpeedValue = 1.0f;

        #region Android Callbacks
        public void setSliderSpeedValue(string value)
        {
            currentSliderSpeedValue = float.Parse(value);
        }

        public void triggerModel(string model)
        {
            PlayerPrefs.SetString(PREF_KEY_CURRENT_CHARACTER, model);

            bool isAndi = model == CharacterNames.Andi.ToString();
            
            _AndiModel.SetActive(isAndi);
            _AiniModel.SetActive(!isAndi);
            m_language.ChangeModel(isAndi);
        }

        public void getInputFromAndroid(string text)
        {
            string rawText = text;

            string[] rawToken = m_language.TokenizeText(rawText);
            m_language.ConvertToAnimationFromToken(rawToken);
        }
        #endregion

        #region Unity Callbacks
        private void Awake()
        {
            Instance = this;

            triggerModel(PlayerPrefs.GetString(PREF_KEY_CURRENT_CHARACTER, CharacterNames.Andi.ToString()));
            StartCoroutine(_SpeedHandler());
        }
        #endregion

        private IEnumerator _SpeedHandler()
        {
            while (true)
            {
                Time.timeScale = currentSliderSpeedValue;
                yield return null;
            }
        }
    }

}
