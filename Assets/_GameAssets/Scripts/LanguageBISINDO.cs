using Animancer;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FasilkomUI.BISINDO
{
    [System.Serializable]
    public class Kata
    {
        public string id;
        public string[] awalan;
        public string[] akhiran;
        public string[] suku;
        public string pokok;
    }

    [System.Serializable]
    public class KataBerimbuhan
    {
        public List<Kata> listKata;
    }

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
            //List<string> komponenKata = new List<string>(rawToken);
            //List<Gesture> komponenKata2 = _CorrectRawTokenToAnimation(rawToken);
            List<Gesture> komponenKata2 = _DeconstructWord2(rawToken);

            if (m_animancerHeadTongueCoroutine != null) StopCoroutine(m_animancerHeadTongueCoroutine);
            if (m_animancerBodyCoroutine != null) StopCoroutine(m_animancerBodyCoroutine);
            //m_animancerHeadTongueCoroutine = StartCoroutine(_AnimationSequence(new NamedAnimancerComponent[] { m_animancer, m_animancerTongue }, komponenKata));
            m_animancerBodyCoroutine = StartCoroutine(_AnimationSequence(new NamedAnimancerComponent[] { m_animancerBody }, komponenKata2, true));
        }

        private List<Gesture> _CorrectRawTokenToAnimation(string[] rawToken)
        {
            string jsonData = m_gestureLookup.ToString();
            GestureDictionary tempList = JsonUtility.FromJson<GestureDictionary>(jsonData);
            Dictionary<string, Gesture> tableLookup = new Dictionary<string, Gesture>();
            foreach (Gesture gesture in tempList.listGesture)
            {
                tableLookup.Add(gesture.id, gesture);
            }

            List<Gesture> komponenKata = new List<Gesture>();
            foreach (string kata in rawToken)
            {
                if (tableLookup.ContainsKey(kata))
                {
                    komponenKata.Add(tableLookup[kata]);
                }
                else
                {
                    komponenKata.Add(new Gesture(kata));
                }
            }

            return komponenKata;
        }

        private List<Gesture> _DeconstructWord2(string[] token)
        {
            Dictionary<string, Kata> tableLookup = _LoadTableLookup();
            List<string> komponenKata = new List<string>();

            foreach (string t in token)
            {
                if (tableLookup.ContainsKey(t))
                {
                    // Cek apakah kata merupakan kata majemuk
                    if (_IsMajemuk(t))
                    {
                        // 1. Tambah kata dasar 
                        komponenKata.Add(tableLookup[t].pokok);
                        // 2. Tambah awalan
                        // int indexDasar = komponenKata.IndexOf(tableLookup[t].pokok);
                        // Debug.Log(indexDasar);
                        foreach (string awalan in tableLookup[t].awalan)
                        {
                            if (awalan != "")
                            {
                                Match match = Regex.Match(awalan, @"[0-9]");
                                string matchVal = "0";
                                if (match.Success)
                                {
                                    matchVal = match.Value;
                                }
                                int pos = int.Parse(matchVal);

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

                        foreach (string akhiran in tableLookup[t].akhiran)
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
                        foreach (string awalan in tableLookup[t].awalan)
                        {
                            if (awalan != "")
                            {
                                komponenKata.Add(awalan + "-");
                            }
                        }


                        komponenKata.Add(tableLookup[t].pokok);

                        foreach (string akhiran in tableLookup[t].akhiran)
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

        private Dictionary<string, Kata> _LoadTableLookup()
        {
            string jsonData = Data_Location.ToString();

            KataBerimbuhan tempList = JsonUtility.FromJson<KataBerimbuhan>(jsonData);

            Dictionary<string, Kata> tableLookup = new Dictionary<string, Kata>();

            foreach (Kata kata in tempList.listKata)
            {
                tableLookup.Add(kata.id, kata);
            }

            return tableLookup;
        }

        private Dictionary<string, Gesture> _LoadGestureLookup()
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

        private List<Gesture> _WordToGesture(List<string> komponenKata)
        {
            Dictionary<string, Gesture> gestureLookup = _LoadGestureLookup();
            List<Gesture> finalKomponen = new List<Gesture>();

            foreach (string kata in komponenKata)
            {
                if (gestureLookup.ContainsKey(kata))
                {
                    finalKomponen.Add(gestureLookup[kata]);
                }
                else
                {
                    string[] split = _AbjadChecker(kata);
                    foreach (string s in split)
                    {
                        if (gestureLookup.ContainsKey(s))
                            finalKomponen.Add(gestureLookup[s]);
                        else
                            finalKomponen.Add(new Gesture(s));
                    }
                }
            }

            return finalKomponen;
        }
    }
}
