using Animancer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FasilkomUI.SIBI
{
    #region Database Classes
    [Serializable]
    public class SIBI : AbstractDatabase
    {
        public string detail;
        public string category;
        // suku
    }

    [Serializable]
    public class Alt_SIBI : AbstractDatabase
    {
        public string sibi_id;
    }

    [Serializable]
    public class Imbuhan_SIBI : AbstractDatabase
    {
        public string sibi_id;
        public string sibi_id_awalan;
        public string sibi_id_akhiran;

        public string[] awalans;
        public string[] akhirans;
    }

    [Serializable] public class SIBIDictionary : AbstractDatabaseDictionary<SIBI> { }
    [Serializable] public class AltSIBIDictionary : AbstractDatabaseDictionary<Alt_SIBI> { }
    [Serializable] public class ImbuhanSIBIDictionary : AbstractDatabaseDictionary<Imbuhan_SIBI> { }
    #endregion

    public class LanguageSIBI : AbstractLanguage
    {
        Dictionary<string, SIBI> m_table_sibi;
        Dictionary<string, Alt_SIBI> m_table_alt_sibi;
        Dictionary<string, Imbuhan_SIBI> m_table_imbuhan_sibi;

        #region Unity Callbacks
        protected override void Awake()
        {
            base.Awake();

            m_table_sibi = AbstractLanguageUtility.LoadDatabaseLookup<SIBIDictionary, SIBI>(m_data_languageLookup.ToString());
            m_table_alt_sibi = AbstractLanguageUtility.LoadDatabaseLookup<AltSIBIDictionary, Alt_SIBI>(m_data_alt_languageLookup.ToString());
            // ganti ke foreach
            m_table_imbuhan_sibi = AbstractLanguageUtility.LoadDatabaseLookup<ImbuhanSIBIDictionary, Imbuhan_SIBI>(m_data_imbuhan_languageLookup[0].ToString());
        }
        #endregion

        public override void ConvertToAnimationFromToken(string[] rawTokens)
        {
            //string[] correctedToken = _SlangChecker(rawToken);
            //List<Gesture> komponenKata = _DeconstructWordForMouth(correctedToken);
            //List<Gesture> komponenKata2 = _DeconstructWordForBody(correctedToken);

            //if (m_animancerHeadTongueCoroutine != null) StopCoroutine(m_animancerHeadTongueCoroutine);
            //if (m_animancerBodyCoroutine != null) StopCoroutine(m_animancerBodyCoroutine);
            //m_animancerHeadTongueCoroutine = StartCoroutine(_AnimationSequence(new NamedAnimancerComponent[] { m_animancer, m_animancerTongue }, komponenKata));
            //m_animancerBodyCoroutine = StartCoroutine(_AnimationSequence(new NamedAnimancerComponent[] { m_animancerBody }, komponenKata2, true));

            // cek setiap rawtoken apakah token tersebut ada di database
            List<SIBI> sibiList = new List<SIBI>();
            foreach(string rawToken in rawTokens)
            {
                _SearchKeyFromTable(sibiList, rawToken);
            }

            UITextProcessing.Instance.SendTextResultToUI(0, sibiList);
        }

        private void _SearchKeyFromTable(List<SIBI> sibiList, string rawToken)
        {
            if (m_table_sibi.ContainsKey(rawToken))
            {
                sibiList.Add(m_table_sibi[rawToken]);
                return;
            }

            if (m_table_alt_sibi.ContainsKey(rawToken))
            {
                _SearchKeyFromTable(sibiList, m_table_alt_sibi[rawToken].sibi_id);
                return;
            }

            // ganti ke foreach
            // tambah akhiran awalan juga
            if (m_table_imbuhan_sibi.ContainsKey(rawToken))
            {
                _SearchKeyFromTable(sibiList, m_table_imbuhan_sibi[rawToken].sibi_id);
                return;
            }

            // kbbi
            // imbuhan kbbi
            // slang
            // majemuk strip, split -
            // cek nomor, modulo setiap anu
            // cek time, liat pake .
            if(rawToken.Length > 1)
            {
                var tokenSplits = AbstractLanguageUtility.SplitString(rawToken);
                foreach(string tokenSplit in tokenSplits)
                {
                    _SearchKeyFromTable(sibiList, tokenSplit);
                }

                return;
            }

            Debug.LogWarning("Somehow, unable to process this token : " + rawToken);
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

        /**
         * <summary>
         * To correct any slang in a word
         * Ex. trmksih --> terima kasih
         * </summary>
         */
        //private string[] _SlangChecker(string[] token)
        //{
        //    Dictionary<string, Slang> tableLookup = AbstractLanguageUtility.LoadSKGLookup<SlangDictionary, Slang>(Data_SlangLookup.ToString());
        //    List<string> correctedToken = new List<string>();

        //    foreach (string t in token)
        //    {
        //        if (tableLookup.ContainsKey(t))
        //        {
        //            correctedToken.Add(tableLookup[t].formal);
        //        }
        //        else
        //        {
        //            correctedToken.Add(t);
        //        }
        //    }

        //    return correctedToken.ToArray();
        //}

        /**
         * <summary>
         * Untuk melakukan dekonstruksi kata ke tabel lookup kata berimbuhan
         * Deconstruct word yang ini untuk kepala dan lidah, jadinya cuma a i u e o aja
         * Ex. Berfungsi --> Ber Fungsi
         * </summary>
         */
        //private List<SIBI> _DeconstructWordForMouth(string[] token)
        //{
        //    Dictionary<string, Imbuhan_SIBI> tableLookup = AbstractLanguageUtility.LoadSKGLookup<ImbuhanSIBIDictionary, Imbuhan_SIBI>(Data_TableLookup.ToString());
        //    List<string> komponenKata = new List<string>();

        //    foreach (string t in token)
        //    {
        //        if (tableLookup.ContainsKey(t))
        //        {
        //            // Cek apakah kata merupakan kata majemuk
        //            if (AbstractLanguageUtility.IsMajemuk(t))
        //            {
        //                // 1. Tambah kata dasar 

        //                // 2. Tambah awalan
        //                // int indexDasar = komponenKata.IndexOf(tableLookup[t].pokok);
        //                // Debug.Log(indexDasar);



        //                foreach (string suku in tableLookup[t].sukus)
        //                {
        //                    if (suku != "")
        //                    {
        //                        Match match = Regex.Match(suku, @"[0-9]");
        //                        string matchVal = "0";
        //                        if (match.Success)
        //                        {
        //                            matchVal = match.Value;
        //                        }
        //                        int pos = int.Parse(matchVal);

        //                        string csuku = Regex.Replace(suku, @"[^a-zA-Z]", "");

        //                        if (pos == 1)
        //                        {
        //                            komponenKata.Insert(komponenKata.LastIndexOf(tableLookup[t].pokok), csuku);
        //                        }
        //                        else
        //                        {
        //                            komponenKata.Insert(komponenKata.LastIndexOf(tableLookup[t].pokok) + 1, csuku);
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
        //                        komponenKata.Add(awalan);
        //                    }
        //                }

        //                foreach (string suku in tableLookup[t].sukus)
        //                {
        //                    if (suku != "")
        //                    {
        //                        komponenKata.Add(suku);
        //                    }
        //                }

        //                foreach (string akhiran in tableLookup[t].akhirans)
        //                {
        //                    if (akhiran != "")
        //                    {
        //                        if (akhiran == "i")
        //                        {
        //                            komponenKata.Add(akhiran);
        //                        }
        //                        else
        //                        {
        //                            komponenKata.Add(akhiran);
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

        //    List<SIBI> finalKomponen = _WordToGesture(komponenKata);
        //    return finalKomponen;
        //}
    }
}