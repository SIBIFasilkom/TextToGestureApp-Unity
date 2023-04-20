using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FasilkomUI
{
    public class UIDictionaryWordButton : MonoBehaviour
    {
        [SerializeField] Text m_text;

        public void InitializeButton(bool isUseButton, string str)
        {
            gameObject.SetActive(isUseButton);
            m_text.text = str;
        }

        public void OpenDictionaryButton()
        {
            UITextProcessing.Instance.OpenDictionary(m_text.text);
        }
    }
}
