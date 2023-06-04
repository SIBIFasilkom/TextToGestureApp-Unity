using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FasilkomUI
{
    public class UIDictionaryWordButton : MonoBehaviour
    {
        [SerializeField] Text m_text;

        string m_key;

        public void InitializeButton(bool isUseButton, string str = "")
        {
            gameObject.SetActive(isUseButton);

            bool isExist = TextProcessing.Instance.Language.CheckAnimationExist(str);
            m_key = str;
            m_text.text = isExist ? str : "<color=red>" + str + "</color>";
        }

        public void OpenDictionaryButton()
        {
            UITextProcessing.Instance.OpenDictionary(m_key);
        }
    }
}
