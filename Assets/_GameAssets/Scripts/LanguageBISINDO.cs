using Animancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FasilkomUI.BISINDO
{
    [System.Serializable]
    public class Gesture
    {
        public string id;
        public string anim;
    }

    [System.Serializable]
    public class GestureDictionary
    {
        public List<Gesture> listGesture;
    }

    public class LanguageBISINDO : AbstractLanguage
    {
        [Header("Database")]
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
            List<string> komponenKata = new List<string>(rawToken);
            List<string> komponenKata2 = _CorrectRawTokenToAnimation(rawToken);

            if (m_animancerHeadTongueCoroutine != null) StopCoroutine(m_animancerHeadTongueCoroutine);
            if (m_animancerBodyCoroutine != null) StopCoroutine(m_animancerBodyCoroutine);
            m_animancerHeadTongueCoroutine = StartCoroutine(_AnimationSequence(new NamedAnimancerComponent[] { m_animancer, m_animancerTongue }, komponenKata));
            m_animancerBodyCoroutine = StartCoroutine(_AnimationSequence(new NamedAnimancerComponent[] { m_animancerBody }, komponenKata2, true));
        }

        private List<string> _CorrectRawTokenToAnimation(string[] rawToken)
        {
            string jsonData = m_gestureLookup.ToString();

            GestureDictionary tempList = JsonUtility.FromJson<GestureDictionary>(jsonData);

            Dictionary<string, Gesture> tableLookup = new Dictionary<string, Gesture>();

            foreach (Gesture gesture in tempList.listGesture)
            {
                tableLookup.Add(gesture.id, gesture);
            }

            List<string> komponenKata = new List<string>();

            foreach (string kata in rawToken)
            {
                if (tableLookup.ContainsKey(kata))
                {
                    komponenKata.Add(tableLookup[kata].anim);
                }
                else
                {
                    komponenKata.Add(kata);
                }
            }

            return komponenKata;
        }
    }
}
