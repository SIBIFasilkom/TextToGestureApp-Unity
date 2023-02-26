using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FasilkomUI
{
    [CreateAssetMenu(fileName = "Language Scriptable", menuName = "Fasilkom-UI/Language Scriptable", order = 1)]
    public class LanguageScriptable : ScriptableObject
    {
        public Dictionary<string, Animation> m_animationKeys;
    }
}
