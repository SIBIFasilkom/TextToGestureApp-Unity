using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FasilkomUI
{
    public class UITextResultButton : MonoBehaviour
    {
        [SerializeField] Text m_text;
        [SerializeField] Image m_image;
        [SerializeField] Color m_selectedColor;
        [SerializeField] Color m_selectedColor_text;
        [SerializeField] Color m_normalColor;
        [SerializeField] Color m_normalColor_text;

        public void InitializeButton(bool isUseButton, string str, bool isSelected)
        {
            gameObject.SetActive(isUseButton);
            m_text.text = str;
            m_image.color = (isSelected) ? m_selectedColor : m_normalColor;
            m_text.color = (isSelected) ? m_selectedColor_text : m_normalColor_text;
        }

        public void OpenDictionaryButton()
        {
            UITextProcessing.Instance.OpenDictionary(m_text.text);
        }
    }
}