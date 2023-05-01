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

            UITextProcessing.Instance.InitializeUIDictionaryDatabase(m_table_sibi);
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

            // jalanin coroutin
            UITextProcessing.Instance.SendTextResultToUI(0, sibiList);
        }

        public override string GetHowToLanguage(string key)
        {
            return m_table_sibi[key].detail;
        }

        private void _SearchKeyFromTable(List<SIBI> sibiList, string rawToken)
        {
            if (string.IsNullOrEmpty(rawToken))
                return;

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
            if (m_table_imbuhan_sibi.ContainsKey(rawToken))
            {
                var awalans = m_table_imbuhan_sibi[rawToken].sibi_id_awalan.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var awalan in awalans)
                    _SearchKeyFromTable(sibiList, awalan);

                _SearchKeyFromTable(sibiList, m_table_imbuhan_sibi[rawToken].sibi_id);

                var akhirans = m_table_imbuhan_sibi[rawToken].sibi_id_akhiran.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var akhiran in akhirans)
                    _SearchKeyFromTable(sibiList, akhiran);

                return;
            }

            // kbbi
            // imbuhan kbbi
            // slang

            if(AbstractLanguageUtility.CheckContainStrip(rawToken))
            {
                var majemukTokens = rawToken.Split('-', StringSplitOptions.RemoveEmptyEntries);
                foreach (var majemukToken in majemukTokens)
                    _SearchKeyFromTable(sibiList, majemukToken);

                return;
            }

            // kalo minus??
            // p.s : baru di handle sampe juta doang, belum milyar
            // definately butuh di simplify lol
            if (AbstractLanguageUtility.CheckNeedToSplitNumeric(rawToken))
            {
                int parsedToken = Mathf.Abs(int.Parse(rawToken));

                if (parsedToken >= 2000000)
                {
                    _SearchKeyFromTable(sibiList, "" + (parsedToken / 1000000));
                    _SearchKeyFromTable(sibiList, "-juta");
                    if (parsedToken % 1000000 > 0)
                        _SearchKeyFromTable(sibiList, "" + (parsedToken % 1000000));
                }

                if (parsedToken >= 1000000 && parsedToken < 2000000)
                {
                    _SearchKeyFromTable(sibiList, "1000000");
                    if (parsedToken % 1000000 > 0)
                        _SearchKeyFromTable(sibiList, "" + (parsedToken % 1000000));
                }

                if (parsedToken >= 2000 && parsedToken < 1000000)
                {
                    _SearchKeyFromTable(sibiList, "" + (parsedToken / 1000));
                    _SearchKeyFromTable(sibiList, "-ribu");
                    if (parsedToken % 1000 > 0)
                        _SearchKeyFromTable(sibiList, "" + (parsedToken % 1000));
                }

                if (parsedToken >= 1000 && parsedToken < 2000)
                {
                    _SearchKeyFromTable(sibiList, "1000");
                    if (parsedToken % 1000 > 0)
                        _SearchKeyFromTable(sibiList, "" + (parsedToken % 1000));
                }

                if (parsedToken >= 200 && parsedToken < 1000)
                {
                    _SearchKeyFromTable(sibiList, "" + (parsedToken / 100));
                    _SearchKeyFromTable(sibiList, "-ratus");
                    if(parsedToken % 100 > 0)
                        _SearchKeyFromTable(sibiList, "" + (parsedToken % 100));
                }

                if (parsedToken >= 100 && parsedToken < 200)
                {
                    _SearchKeyFromTable(sibiList, "100");
                    if(parsedToken % 100 > 0)
                        _SearchKeyFromTable(sibiList, "" + (parsedToken % 100));
                }

                if (parsedToken >= 20 && parsedToken < 100)
                {
                    _SearchKeyFromTable(sibiList, "" + (parsedToken / 10));
                    _SearchKeyFromTable(sibiList, "-puluh");
                    if (parsedToken % 10 > 0)
                        _SearchKeyFromTable(sibiList, "" + (parsedToken % 10));
                }

                if(parsedToken >= 12 && parsedToken < 20)
                {
                    _SearchKeyFromTable(sibiList, "" + (parsedToken % 10));
                    _SearchKeyFromTable(sibiList, "-belas");
                }
                return;
            }

            if (AbstractLanguageUtility.CheckIsTimeFormat(rawToken))
            {
                var timeTokens = rawToken.Split('.', StringSplitOptions.RemoveEmptyEntries);
                if(timeTokens.Length == 2)
                {
                    _SearchKeyFromTable(sibiList, timeTokens[0]);
                    _SearchKeyFromTable(sibiList, "lebih");
                    _SearchKeyFromTable(sibiList, timeTokens[1]);

                    return;
                }
            }

            if (rawToken.Length > 1)
            {
                var tokenSplits = AbstractLanguageUtility.SplitString(rawToken);
                foreach(string tokenSplit in tokenSplits)
                    _SearchKeyFromTable(sibiList, tokenSplit);

                return;
            }

            Debug.LogWarning("Unable to process this token : " + rawToken);
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