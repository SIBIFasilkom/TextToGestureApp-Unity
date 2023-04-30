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

        NamedAnimancerComponent m_animancer;
        NamedAnimancerComponent m_animancerTongue;
        NamedAnimancerComponent m_animancerBody;
        Coroutine m_animancerHeadTongueCoroutine;
        Coroutine m_animancerBodyCoroutine;

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

        //protected IEnumerator _AnimationSequence(NamedAnimancerComponent[] animancers, List<Sibi> gestures, bool sendToUI = false)
        //{
        //    float fadeDuration = 0.25f;
        //    float noGestureAnimationWait = 1.0f;
        //    int idx = 0;

        //    AnimancerState state = animancers[0].States.Current;

        //    foreach (Sibi gesture in gestures)
        //    {
        //        if (sendToUI)
        //        {
        //            UITextProcessing.Instance.SendTextResultToUI(idx, gestures);
        //            idx += 1;
        //        }

        //        _PlayAllAnimancer(animancers, gesture.anim, fadeDuration, out state);
        //        if (state != null)
        //        {
        //            while (state.Time < state.Length)
        //            {
        //                yield return null;
        //            }

        //            yield return new WaitForSecondsRealtime(0.1f);
        //        }
        //        else
        //        {
        //            Debug.Log("Animation not found for " + animancers[0].name + " : " + gesture.id + " - " + gesture.anim);
        //            _PlayAllAnimancer(animancers, "idle", fadeDuration, out state);
        //            yield return new WaitForSecondsRealtime(noGestureAnimationWait);
        //        }
        //    }

        //    _PlayAllAnimancer(animancers, "idle", fadeDuration, out state);
        //}

        protected void _PlayAllAnimancer(NamedAnimancerComponent[] animancers, string key, float fadeDuration, out AnimancerState state)
        {
            state = null;
            foreach (NamedAnimancerComponent animancer in animancers)
            {
                state = animancer.TryPlay(key, fadeDuration, FadeMode.FromStart);
            }
        }

        /**
         * <summary>
         * Untuk melakukan dekonstruksi kata ke tabel lookup kata berimbuhan
         * Deconstruct word yang ini untuk badan
         * Ex. Berfungsi --> Ber Fungsi
         * </summary>
         */
        //protected List<Sibi> _DeconstructWordForBody(string[] token)
        //{
        //    Dictionary<string, Imbuhan> tableLookup = AbstractLanguageUtility.LoadSKGLookup<ImbuhanDictionary, Imbuhan>(Data_TableLookup.ToString());
        //    List<string> komponenKata = new List<string>();

        //    foreach (string t in token)
        //    {
        //        if (tableLookup.ContainsKey(t))
        //        {
        //            // Cek apakah kata merupakan kata majemuk
        //            if (AbstractLanguageUtility.IsMajemuk(t))
        //            {
        //                // 1. Tambah kata dasar 
        //                komponenKata.Add(tableLookup[t].pokok);
        //                // 2. Tambah awalan
        //                foreach (string awalan in tableLookup[t].awalans)
        //                {
        //                    if (awalan != "")
        //                    {
        //                        #region Sumpah gua ga ngerti ini fungsinya ngepain lollololol
        //                        Match match = Regex.Match(awalan, @"[0-9]");
        //                        string matchVal = "0";
        //                        if (match.Success)
        //                        {
        //                            matchVal = match.Value;
        //                        }
        //                        int pos = int.Parse(matchVal);
        //                        #endregion

        //                        string cAwalan = Regex.Replace(awalan, @"[^a-zA-Z]", "");

        //                        if (pos == 1)
        //                        {
        //                            komponenKata.Insert(komponenKata.IndexOf(tableLookup[t].pokok), cAwalan);
        //                        }
        //                        else
        //                        {
        //                            komponenKata.Insert(komponenKata.IndexOf(tableLookup[t].pokok) + 1, cAwalan);
        //                        }
        //                    }
        //                }

        //                foreach (string akhiran in tableLookup[t].akhirans)
        //                {
        //                    if (akhiran != "")
        //                    {
        //                        Match match = Regex.Match(akhiran, @"[0-9]");
        //                        string matchVal = "0";
        //                        if (match.Success)
        //                        {
        //                            matchVal = match.Value;
        //                        }
        //                        int pos = int.Parse(matchVal);

        //                        string cAkhiran = Regex.Replace(akhiran, @"[^a-zA-Z]", "");

        //                        if (akhiran == "i")
        //                        {
        //                            cAkhiran = "-" + cAkhiran;
        //                        }

        //                        if (pos == 1)
        //                        {
        //                            komponenKata.Insert(komponenKata.LastIndexOf(tableLookup[t].pokok), cAkhiran);
        //                        }
        //                        else
        //                        {
        //                            komponenKata.Insert(komponenKata.LastIndexOf(tableLookup[t].pokok) + 1, cAkhiran);
        //                        }
        //                    }
        //                }

        //            }
        //            else
        //            {
        //                foreach (string awalan in tableLookup[t].awalans)
        //                {
        //                    if (awalan != "")
        //                    {
        //                        komponenKata.Add(awalan + "-");
        //                    }
        //                }


        //                komponenKata.Add(tableLookup[t].pokok);

        //                foreach (string akhiran in tableLookup[t].akhirans)
        //                {
        //                    if (akhiran != "")
        //                    {
        //                        if (akhiran == "i")
        //                        {
        //                            komponenKata.Add("-" + akhiran);
        //                        }
        //                        else
        //                        {
        //                            komponenKata.Add("-" + akhiran);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            komponenKata.Add(t);
        //        }
        //    }

        //    List<Sibi> finalKomponen = _WordToGesture(komponenKata);
        //    return finalKomponen;
        //}

        /**
         *  <summary>
         *  Persiapan lookup gerakan, mencari nama file animasi untuk kata formal yang diinput
         *  jika tidak ditemukan di database gesture lookup, pecah jadi alfabet
         *  jika merupakan angka, pecah sesuai digit nya
         *  </summary>
         */
        //protected List<Sibi> _WordToGesture(List<string> komponenKata)
        //{
        //    Dictionary<string, Sibi> gestureLookup = AbstractLanguageUtility.LoadSKGLookup<GestureDictionary, Sibi>(Data_GestureLookup.ToString());
        //    List<Sibi> finalKomponen = new List<Sibi>();

        //    foreach (string kata in komponenKata)
        //    {
        //        if (gestureLookup.ContainsKey(kata))
        //        {
        //            finalKomponen.Add(gestureLookup[kata]);
        //        }
        //        else
        //        {
        //            string[] split = AbstractLanguageUtility.AbjadChecker(kata);
        //            foreach (string s in split)
        //            {
        //                if (gestureLookup.ContainsKey(s))
        //                    finalKomponen.Add(gestureLookup[s]);
        //                else
        //                    finalKomponen.Add(_GestureNotFound(s));
        //            }
        //        }
        //    }

        //    return finalKomponen;
        //}

        /**
         * <summary>
         * Kalau gesture ga ada, bikin gesture kosong & throw warning
         * </summary>
         */
        //protected Sibi _GestureNotFound(string splitKata)
        //{
        //    Sibi gesture = new Sibi();
        //    gesture.id = splitKata;
        //    gesture.anim = "(GESTURE NOT FOUND, FIX GESTURELOOKUP DATABASE)";
        //    return gesture;
        //}
    }
}