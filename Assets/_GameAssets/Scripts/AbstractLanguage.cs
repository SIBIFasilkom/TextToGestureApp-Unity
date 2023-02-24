using Animancer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FasilkomUI
{
    #region Slang, Kata, Gesture classes
    [Serializable]
    public abstract class AbstractSKG
    {
        public string id;
    }

    [Serializable]
    public class Slang : AbstractSKG
    {
        public string formal;
    }

    [Serializable]
    public class Kata : AbstractSKG
    {
        public string pokok;
        public string awalan;
        public string akhiran;
        public string suku;

        public string[] awalans;
        public string[] akhirans;
        public string[] sukus;
    }

    [Serializable]
    public class Gesture : AbstractSKG
    {
        public string anim;
    }

    [Serializable]
    public abstract class AbstractSKGDictionary<T> where T : AbstractSKG
    {
        public List<T> listSKG;
    }

    [Serializable] public class SlangDictionary : AbstractSKGDictionary<Slang> { }
    [Serializable] public class GestureDictionary : AbstractSKGDictionary<Gesture> { }
    [Serializable] public class KataDictionary : AbstractSKGDictionary<Kata> { }
    #endregion

    public abstract class AbstractLanguage : MonoBehaviour
    {
        [Header("Database")]
        [SerializeField] protected TextAsset Data_SlangLookup;
        [SerializeField] protected TextAsset Data_TableLookup;
        [SerializeField] protected TextAsset Data_GestureLookup;

        public abstract void ConvertToAnimationFromToken(string[] rawToken);

        public abstract void ChangeModel(bool isAndi);

        protected IEnumerator _AnimationSequence(NamedAnimancerComponent[] animancers, List<Gesture> gestures, bool sendToUI = false)
        {
            float fadeDuration = 0.25f;
            float noGestureAnimationWait = 1.0f;
            int idx = 0;

            AnimancerState state = animancers[0].States.Current;

            foreach (Gesture gesture in gestures)
            {
                if (sendToUI)
                {
                    UITextProcessing.Instance.SendTextResultToUI(idx, gestures);
                    idx += 1;
                }

                _PlayAllAnimancer(animancers, gesture.anim, fadeDuration, out state);
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
                    Debug.Log("Animation not found for " + animancers[0].name + " : " + gesture.id + " - " + gesture.anim);
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

        /**
         * <summary>
         * Untuk melakukan dekonstruksi kata ke tabel lookup kata berimbuhan
         * Deconstruct word yang ini untuk badan
         * Ex. Berfungsi --> Ber Fungsi
         * </summary>
         */
        protected List<Gesture> _DeconstructWordForBody(string[] token)
        {
            Dictionary<string, Kata> tableLookup = AbstractLanguageUtility.LoadSKGLookup<KataDictionary, Kata>(Data_TableLookup.ToString());
            List<string> komponenKata = new List<string>();

            foreach (string t in token)
            {
                if (tableLookup.ContainsKey(t))
                {
                    // Cek apakah kata merupakan kata majemuk
                    if (AbstractLanguageUtility.IsMajemuk(t))
                    {
                        // 1. Tambah kata dasar 
                        komponenKata.Add(tableLookup[t].pokok);
                        // 2. Tambah awalan
                        foreach (string awalan in tableLookup[t].awalans)
                        {
                            if (awalan != "")
                            {
                                #region Sumpah gua ga ngerti ini fungsinya ngepain lollololol
                                Match match = Regex.Match(awalan, @"[0-9]");
                                string matchVal = "0";
                                if (match.Success)
                                {
                                    matchVal = match.Value;
                                }
                                int pos = int.Parse(matchVal);
                                #endregion

                                string cAwalan = Regex.Replace(awalan, @"[^a-zA-Z]", "");

                                if (pos == 1)
                                {
                                    komponenKata.Insert(komponenKata.IndexOf(tableLookup[t].pokok), cAwalan);
                                }
                                else
                                {
                                    komponenKata.Insert(komponenKata.IndexOf(tableLookup[t].pokok) + 1, cAwalan);
                                }
                            }
                        }

                        foreach (string akhiran in tableLookup[t].akhirans)
                        {
                            if (akhiran != "")
                            {
                                Match match = Regex.Match(akhiran, @"[0-9]");
                                string matchVal = "0";
                                if (match.Success)
                                {
                                    matchVal = match.Value;
                                }
                                int pos = int.Parse(matchVal);

                                string cAkhiran = Regex.Replace(akhiran, @"[^a-zA-Z]", "");

                                if (akhiran == "i")
                                {
                                    cAkhiran = "-" + cAkhiran;
                                }

                                if (pos == 1)
                                {
                                    komponenKata.Insert(komponenKata.LastIndexOf(tableLookup[t].pokok), cAkhiran);
                                }
                                else
                                {
                                    komponenKata.Insert(komponenKata.LastIndexOf(tableLookup[t].pokok) + 1, cAkhiran);
                                }
                            }
                        }

                    }
                    else
                    {
                        foreach (string awalan in tableLookup[t].awalans)
                        {
                            if (awalan != "")
                            {
                                komponenKata.Add(awalan + "-");
                            }
                        }


                        komponenKata.Add(tableLookup[t].pokok);

                        foreach (string akhiran in tableLookup[t].akhirans)
                        {
                            if (akhiran != "")
                            {
                                if (akhiran == "i")
                                {
                                    komponenKata.Add("-" + akhiran);
                                }
                                else
                                {
                                    komponenKata.Add("-" + akhiran);
                                }
                            }
                        }
                    }
                }
                else
                {
                    komponenKata.Add(t);
                }
            }

            List<Gesture> finalKomponen = _WordToGesture(komponenKata);
            return finalKomponen;
        }

        /**
         *  <summary>
         *  Persiapan lookup gerakan, mencari nama file animasi untuk kata formal yang diinput
         *  jika tidak ditemukan di database gesture lookup, pecah jadi alfabet
         *  jika merupakan angka, pecah sesuai digit nya
         *  </summary>
         */
        protected List<Gesture> _WordToGesture(List<string> komponenKata)
        {
            Dictionary<string, Gesture> gestureLookup = AbstractLanguageUtility.LoadSKGLookup<GestureDictionary, Gesture>(Data_GestureLookup.ToString());
            List<Gesture> finalKomponen = new List<Gesture>();

            foreach (string kata in komponenKata)
            {
                if (gestureLookup.ContainsKey(kata))
                {
                    finalKomponen.Add(gestureLookup[kata]);
                }
                else
                {
                    string[] split = AbstractLanguageUtility.AbjadChecker(kata);
                    foreach (string s in split)
                    {
                        if (gestureLookup.ContainsKey(s))
                            finalKomponen.Add(gestureLookup[s]);
                        else
                            finalKomponen.Add(_GestureNotFound(s));
                    }
                }
            }

            return finalKomponen;
        }

        /**
         * <summary>
         * Kalau gesture ga ada, bikin gesture kosong & throw warning
         * </summary>
         */
        protected Gesture _GestureNotFound(string splitKata)
        {
            Gesture gesture = new Gesture();
            gesture.id = splitKata;
            gesture.anim = "(GESTURE NOT FOUND, FIX GESTURELOOKUP DATABASE)";
            return gesture;
        }
    }
}