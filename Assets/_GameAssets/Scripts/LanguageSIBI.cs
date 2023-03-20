using Animancer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FasilkomUI.SIBI
{
    public class LanguageSIBI : AbstractLanguage
    {
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
            string[] correctedToken = _SlangChecker(rawToken);
            List<Gesture> komponenKata = _DeconstructWordForMouth(correctedToken);
            List<Gesture> komponenKata2 = _DeconstructWordForBody(correctedToken);

            if (m_animancerHeadTongueCoroutine != null) StopCoroutine(m_animancerHeadTongueCoroutine);
            if (m_animancerBodyCoroutine != null) StopCoroutine(m_animancerBodyCoroutine);
            m_animancerHeadTongueCoroutine = StartCoroutine(_AnimationSequence(new NamedAnimancerComponent[] { m_animancer, m_animancerTongue }, komponenKata));
            m_animancerBodyCoroutine = StartCoroutine(_AnimationSequence(new NamedAnimancerComponent[] { m_animancerBody }, komponenKata2, true));
        }

        /**
         * <summary>
         * To correct any slang in a word
         * Ex. trmksih --> terima kasih
         * </summary>
         */
        private string[] _SlangChecker(string[] token)
        {
            Dictionary<string, Slang> tableLookup = AbstractLanguageUtility.LoadSKGLookup<SlangDictionary, Slang>(Data_SlangLookup.ToString());
            List<string> correctedToken = new List<string>();

            foreach (string t in token)
            {
                if (tableLookup.ContainsKey(t))
                {
                    correctedToken.Add(tableLookup[t].formal);
                }
                else
                {
                    correctedToken.Add(t);
                }
            }

            return correctedToken.ToArray();
        }

        /**
         * <summary>
         * Untuk melakukan dekonstruksi kata ke tabel lookup kata berimbuhan
         * Deconstruct word yang ini untuk kepala dan lidah, jadinya cuma a i u e o aja
         * Ex. Berfungsi --> Ber Fungsi
         * </summary>
         */
        private List<Gesture> _DeconstructWordForMouth(string[] token)
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

                        // 2. Tambah awalan
                        // int indexDasar = komponenKata.IndexOf(tableLookup[t].pokok);
                        // Debug.Log(indexDasar);



                        foreach (string suku in tableLookup[t].sukus)
                        {
                            if (suku != "")
                            {
                                Match match = Regex.Match(suku, @"[0-9]");
                                string matchVal = "0";
                                if (match.Success)
                                {
                                    matchVal = match.Value;
                                }
                                int pos = int.Parse(matchVal);

                                string csuku = Regex.Replace(suku, @"[^a-zA-Z]", "");

                                if (pos == 1)
                                {
                                    komponenKata.Insert(komponenKata.LastIndexOf(tableLookup[t].pokok), csuku);
                                }
                                else
                                {
                                    komponenKata.Insert(komponenKata.LastIndexOf(tableLookup[t].pokok) + 1, csuku);
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
                                komponenKata.Add(awalan);
                            }
                        }

                        foreach (string suku in tableLookup[t].sukus)
                        {
                            if (suku != "")
                            {
                                komponenKata.Add(suku);
                            }
                        }

                        foreach (string akhiran in tableLookup[t].akhirans)
                        {
                            if (akhiran != "")
                            {
                                if (akhiran == "i")
                                {
                                    komponenKata.Add(akhiran);
                                }
                                else
                                {
                                    komponenKata.Add(akhiran);
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
    }
}