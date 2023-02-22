using Animancer;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FasilkomUI.BISINDO
{
    public class LanguageBISINDO : AbstractLanguage
    {
        [Header("Database")]
        [SerializeField] TextAsset Data_Location;
        [SerializeField] TextAsset m_gestureLookup;

        [Header("Animancer")]
        [SerializeField] NamedAnimancerComponent _Andi;
        [SerializeField] NamedAnimancerComponent _AndiTongue;
        [SerializeField] NamedAnimancerComponent _AndiBody;

        [SerializeField] NamedAnimancerComponent _Aini;
        [SerializeField] NamedAnimancerComponent _AiniTongue;
        [SerializeField] NamedAnimancerComponent _AiniBody;

        NamedAnimancerComponent m_animancer;
        NamedAnimancerComponent m_animancerTongue;
        NamedAnimancerComponent m_animancerBody;
        Coroutine m_animancerHeadTongueCoroutine;
        Coroutine m_animancerBodyCoroutine;

        public override void ChangeModel(bool isAndi)
        {
            m_animancer = (isAndi) ? _Andi : _Aini;
            m_animancerTongue = (isAndi) ? _AndiTongue : _AiniTongue;
            m_animancerBody = (isAndi) ? _AndiBody : _AiniBody;
        }

        public override void ConvertToAnimationFromToken(string[] rawToken)
        {
            List<Gesture> komponenKata2 = _DeconstructWord2(rawToken);

            if (m_animancerHeadTongueCoroutine != null) StopCoroutine(m_animancerHeadTongueCoroutine);
            if (m_animancerBodyCoroutine != null) StopCoroutine(m_animancerBodyCoroutine);
            m_animancerBodyCoroutine = StartCoroutine(_AnimationSequence(new NamedAnimancerComponent[] { m_animancerBody }, komponenKata2, true));
        }

        protected override Dictionary<string, Kata> _LoadTableLookup()
        {
            string jsonData = Data_Location.ToString();
            KataDictionary tempList = JsonUtility.FromJson<KataDictionary>(jsonData);
            Dictionary<string, Kata> tableLookup = new Dictionary<string, Kata>();
            foreach (Kata kata in tempList.listKata)
            {
                tableLookup.Add(kata.id, kata);
            }

            return tableLookup;
        }

        protected override Dictionary<string, Gesture> _LoadGestureLookup()
        {
            string jsonData = m_gestureLookup.ToString();
            GestureDictionary tempList = JsonUtility.FromJson<GestureDictionary>(jsonData);
            Dictionary<string, Gesture> tableLookup = new Dictionary<string, Gesture>();

            foreach (Gesture gesture in tempList.listGesture)
            {
                tableLookup.Add(gesture.id, gesture);
            }

            return tableLookup;
        }
    }
}
