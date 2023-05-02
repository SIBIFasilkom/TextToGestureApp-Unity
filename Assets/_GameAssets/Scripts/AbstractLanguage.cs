using Animancer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FasilkomUI
{
    #region Database classes
    [Serializable]
    public abstract class AbstractDatabase
    {
        public string id;
    }

    public abstract class AbstractDatabaseLanguage : AbstractDatabase
    {
        public string suku;
    }

    [Serializable]
    public abstract class AbstractDatabaseDictionary<T> where T : AbstractDatabase
    {
        public List<T> list;
    }

    [Serializable]
    public class KBBI : AbstractDatabase
    {
        public string sibi_id;
        //public string bisindo_id;
    }

    [Serializable]
    public class Imbuhan_KBBI : AbstractDatabase
    {
        public string sibi_id;
        public string bisindo_id;
    }

    [Serializable]
    public class Slang : AbstractDatabase
    {
        public string formal;
    }
    [Serializable] public class KBBIDictionary : AbstractDatabaseDictionary<KBBI> { }
    [Serializable] public class ImbuhanKBBIDictionary : AbstractDatabaseDictionary<Imbuhan_KBBI> { }
    [Serializable] public class SlangDictionary : AbstractDatabaseDictionary<Slang> { }
    #endregion

    public abstract class AbstractLanguage : MonoBehaviour
    {
        [Header("Database")]
        [SerializeField] protected TextAsset m_data_languageLookup;
        [SerializeField] protected TextAsset m_data_alt_languageLookup;
        [SerializeField] protected TextAsset[] m_data_imbuhan_languageLookup;
        [SerializeField] protected TextAsset m_data_kBBILookup;
        [SerializeField] protected TextAsset[] m_data_imbuhan_kBBILookup;
        [SerializeField] protected TextAsset m_data_slangLookup;

        [Header("Animancer")]
        [SerializeField] NamedAnimancerComponent _Andi;
        [SerializeField] NamedAnimancerComponent _AndiTongue;
        [SerializeField] NamedAnimancerComponent _AndiBody;

        [SerializeField] NamedAnimancerComponent _Aini;
        [SerializeField] NamedAnimancerComponent _AiniTongue;
        [SerializeField] NamedAnimancerComponent _AiniBody;

        protected NamedAnimancerComponent m_animancer;
        protected NamedAnimancerComponent m_animancerTongue;
        protected NamedAnimancerComponent m_animancerBody;
        //protected Coroutine m_animancerHeadTongueCoroutine;
        //protected Coroutine m_animancerBodyCoroutine;
        protected Coroutine m_animancerCoroutine;

        #region Unity Callbacks
        protected virtual void Awake()
        {
            // cache databases general
            print("Jangan lupa cache Slang & KBBI");
        }
        #endregion

        public abstract void ConvertToAnimationFromToken(string[] rawToken);
        public abstract string GetHowToLanguage(string key);

        public void ChangeModel(bool isAndi)
        {
            m_animancer = (isAndi) ? _Andi : _Aini;
            m_animancerTongue = (isAndi) ? _AndiTongue : _AiniTongue;
            m_animancerBody = (isAndi) ? _AndiBody : _AiniBody;
        }

        protected IEnumerator _AnimationSequence<T>(List<T> language_id) where T : AbstractDatabaseLanguage
        {
            float fadeDuration = 0.25f;
            float noGestureAnimationWait = 1.0f;
            
            AnimancerState state = m_animancerBody.States.Current;
            Coroutine headTongueAnimancerCoroutine = null;

            for(int i=0; i<language_id.Count; i++)
            {
                UITextProcessing.Instance.SendTextResultToUI(i, language_id);

                state = m_animancerBody.TryPlay(language_id[i].id, fadeDuration, FadeMode.FromStart);
                if (state != null)
                {
                    if (headTongueAnimancerCoroutine != null) StopCoroutine(headTongueAnimancerCoroutine);
                    headTongueAnimancerCoroutine = StartCoroutine(_PlayHeadTongueAnimancers(language_id[i].suku, fadeDuration));

                    while (state.Time < state.Length) // next : nungguin suku juga
                    {
                        yield return null;
                    }

                    yield return new WaitForSecondsRealtime(0.1f);
                }
                else
                {
                    Debug.LogWarning("Animation body not found for : " + language_id[i].id);
                    _PlayAllAnimancer("idle", fadeDuration, out state);
                    yield return new WaitForSecondsRealtime(noGestureAnimationWait);
                }
            }

            if (headTongueAnimancerCoroutine != null) StopCoroutine(headTongueAnimancerCoroutine);
            _PlayAllAnimancer("idle", fadeDuration, out state);
        }

        protected void _PlayAllAnimancer(string key, float fadeDuration, out AnimancerState state)
        {
            state = m_animancerBody.TryPlay(key, fadeDuration, FadeMode.FromStart);
            m_animancer.TryPlay(key, fadeDuration, FadeMode.FromStart);
            m_animancerTongue.TryPlay(key, fadeDuration, FadeMode.FromStart);
        }

        protected IEnumerator _PlayHeadTongueAnimancers(string suku, float fadeDuration)
        {
            var sukuSplit = suku.Split(';');

            for(int i=0; i<sukuSplit.Length; i++)
            {
                var headState = m_animancer.TryPlay(sukuSplit[i], fadeDuration, FadeMode.FromStart);
                var tongueState = m_animancerTongue.TryPlay(sukuSplit[i], fadeDuration, FadeMode.FromStart);
                if (headState != null && tongueState != null)
                {
                    while (headState.Time < headState.Length && tongueState.Time < tongueState.Length)
                    {
                        yield return null;
                    }

                    yield return new WaitForSeconds(0.1f);
                } else
                {
                    if (headState == null)
                        Debug.LogWarning("Animation head not found for : " + sukuSplit[i]);

                    if (tongueState == null)
                        Debug.LogWarning("Animation tongue not found for : " + sukuSplit[i]);

                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }
}