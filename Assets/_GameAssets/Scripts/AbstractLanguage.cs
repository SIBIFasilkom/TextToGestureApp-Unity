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
        protected Coroutine m_animancerHeadTongueCoroutine;
        protected Coroutine m_animancerBodyCoroutine;

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

        protected IEnumerator _AnimationSequence<T>(NamedAnimancerComponent[] animancers, List<T> language_id, bool sendToUI = false) where T : AbstractDatabase
        {
            float fadeDuration = 0.25f;
            float noGestureAnimationWait = 1.0f;
            int idx = 0;

            AnimancerState state = animancers[0].States.Current;

            foreach (AbstractDatabase language in language_id)
            {
                if (sendToUI)
                {
                    UITextProcessing.Instance.SendTextResultToUI(idx, language_id);
                    idx += 1;
                }

                _PlayAllAnimancer(animancers, language.id, fadeDuration, out state);
                if (state != null)
                {
                    while (state.Time < state.Length)
                    {
                        yield return null;
                    }

                    yield return new WaitForSecondsRealtime(0.1f);
                }
                else
                {
                    Debug.LogWarning("Animation not found for " + animancers[0].name + " : " + language.id);
                    _PlayAllAnimancer(animancers, "idle", fadeDuration, out state);
                    yield return new WaitForSecondsRealtime(noGestureAnimationWait);
                }
            }

            _PlayAllAnimancer(animancers, "idle", fadeDuration, out state);
        }

        protected void _PlayAllAnimancer(NamedAnimancerComponent[] animancers, string key, float fadeDuration, out AnimancerState state)
        {
            state = null;
            foreach (NamedAnimancerComponent animancer in animancers)
            {
                state = animancer.TryPlay(key, fadeDuration, FadeMode.FromStart);
            }
        }
    }
}