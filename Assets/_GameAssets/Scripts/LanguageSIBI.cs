using Animancer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FasilkomUI.SIBI
{
    #region [Objek Kata Berimbuhan]
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
    #endregion

    #region [Objek Kata Gesture]
    [System.Serializable]
    public class Gesture
    {
        public string id;
        public string jenis1; // unused var
        public string jenis2; // unused var
        public string gerakan; // unused var
    }

    [System.Serializable]
    public class GestureDictionary
    {
        public List<Gesture> listGesture;
    }
    #endregion

    #region [Objek Kata Slang]
    [System.Serializable]
    public class Slang
    {
        public string slang;
        public string formal;
    }

    [System.Serializable]
    public class SlangDictionary
    {
        public List<Slang> listSlang;
    }

    [System.Serializable]
    public class Frame
    {

    }
    #endregion

    public class LanguageSIBI : AbstractLanguage
    {
        [Header("Database")]
        [SerializeField] TextAsset Data_Location;
        [SerializeField] TextAsset Data_Location_Gestures;
        [SerializeField] TextAsset Data_Location_Slang;

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
            string[] correctedToken = _SpellingChecker(rawToken);
            List<string> komponenKata = _DeconstructWord(correctedToken);
            List<string> komponenKata2 = _DeconstructWord2(correctedToken);

            UITextProcessing.Instance.DebugTextOutput(komponenKata2);

            if (m_animancerHeadTongueCoroutine != null) StopCoroutine(m_animancerHeadTongueCoroutine);
            if (m_animancerBodyCoroutine != null) StopCoroutine(m_animancerBodyCoroutine);
            m_animancerHeadTongueCoroutine = StartCoroutine(_SibiAnimationSequence(new NamedAnimancerComponent[] { m_animancer, m_animancerTongue }, komponenKata));
            m_animancerBodyCoroutine = StartCoroutine(_SibiAnimationSequence(new NamedAnimancerComponent[] { m_animancerBody }, komponenKata2, true));
        }

        private IEnumerator _SibiAnimationSequence(NamedAnimancerComponent[] animancers, List<string> komponenKata, bool sendToUI = false)
        {
            float fadeDuration = 0.25f;
            float stateSpeed = 1.0f;
            int idx = 0;

            AnimancerState state = animancers[0].States.Current;

            foreach (string kata in komponenKata)
            {
                if (sendToUI)
                {
                    UITextProcessing.Instance.SendTextResultToUI(idx, komponenKata);
                    idx += 1;
                }

                foreach (NamedAnimancerComponent animancer in animancers)
                {
                    state = animancer.TryPlay(kata, fadeDuration);
                    if (state != null) state.Speed = stateSpeed;
                }

                if (state != null)
                {
                    while (state.Time < state.Length)
                    {
                        yield return null;
                    }
                }
            }

            foreach (NamedAnimancerComponent animancer in animancers)
            {
                state = animancer.TryPlay("idle", fadeDuration);
                state.Speed = stateSpeed;
            }
        }

        #region [JSON Load Handler]
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
            string jsonData = Data_Location_Gestures.ToString();

            GestureDictionary tempList = JsonUtility.FromJson<GestureDictionary>(jsonData);

            Dictionary<string, Gesture> tableLookup = new Dictionary<string, Gesture>();

            foreach (Gesture gesture in tempList.listGesture)
            {
                tableLookup.Add(gesture.id, gesture);
            }

            return tableLookup;
        }

        private Dictionary<string, Slang> _LoadSlangLookup()
        {
            string jsonData = Data_Location_Slang.ToString();

            SlangDictionary tempList = JsonUtility.FromJson<SlangDictionary>(jsonData);

            Dictionary<string, Slang> tableLookup = new Dictionary<string, Slang>();

            foreach (Slang slang in tempList.listSlang)
            {
                tableLookup.Add(slang.slang, slang);
            }

            return tableLookup;
        }
        #endregion

        /**
         * <summary>
         * To correct any slang in a word
         * Ex. trmksih --> terima kasih
         * </summary>
         */
        private string[] _SpellingChecker(string[] token)
        {
            Dictionary<string, Slang> tableLookup = _LoadSlangLookup();
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
        private List<string> _DeconstructWord(string[] token)
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

                        // 2. Tambah awalan
                        // int indexDasar = komponenKata.IndexOf(tableLookup[t].pokok);
                        // Debug.Log(indexDasar);



                        foreach (string suku in tableLookup[t].suku)
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

                        foreach (string awalan in tableLookup[t].awalan)
                        {
                            if (awalan != "")
                            {
                                komponenKata.Add(awalan);
                            }
                        }

                        foreach (string suku in tableLookup[t].suku)
                        {
                            if (suku != "")
                            {
                                komponenKata.Add(suku);
                            }
                        }

                        foreach (string akhiran in tableLookup[t].akhiran)
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

            List<string> finalKomponen = _WordToGesture(komponenKata);
            return finalKomponen;
        }

        /**
         * <summary>
         * Untuk melakukan dekonstruksi kata ke tabel lookup kata berimbuhan
         * Deconstruct word yang ini untuk badan
         * Ex. Berfungsi --> Ber Fungsi
         * </summary>
         */
        private List<string> _DeconstructWord2(string[] token)
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

            List<string> finalKomponen = _WordToGesture(komponenKata);
            return finalKomponen;
        }

        /* 
            Persiapan lookup gerakan, jika tidak ditemukan pecah jadi alfabet
            jika merupakan angka, pecah sesuai digit nya
            todo --> cek langsung ke animancernya aja daripada lewat table
        */
        private List<string> _WordToGesture(List<string> komponenKata)
        {
            Dictionary<string, Gesture> gestureLookup = _LoadGestureLookup();
            List<string> finalKomponen = new List<string>();

            foreach (string kata in komponenKata)
            {
                if (gestureLookup.ContainsKey(kata))
                {
                    finalKomponen.Add(kata);
                }
                else
                {
                    string[] split = _AbjadChecker(kata);
                    foreach (string s in split)
                    {
                        finalKomponen.Add(s);
                    }
                }
            }

            return finalKomponen;
        }

        private bool _IsMajemuk(string word)
        {
            return Regex.IsMatch(word, @"[-]");
        }

        private string[] _AbjadChecker(string word)
        {
            bool isNumeric = Regex.IsMatch(word, @"^[0-9]+$");
            bool isTimeFormat = Regex.IsMatch(word, @"(?:[01][0-9]|2[0-3]|[1-9])[.][0-5][0-9]");
            bool isOtherNum = Regex.IsMatch(word, @"(?:[0-9][0-9]|2[0-3]|[1-9])[.][0-5][0-9]");

            if (isNumeric)
            {
                int num;
                bool parsed = Int32.TryParse(word, out num);
                if (parsed)
                {
                    if (num >= 1000)
                    {
                        return _SplitString(word);
                    }
                    else
                    {
                        return _NumberToDigit(word);
                    }
                }
                else
                {
                    return _SplitString(word);
                }
            }
            else if (isTimeFormat)
            {
                // Additional checker
                Match match = Regex.Match(word, @"(?:[01][0-9]|2[0-3]|[1-9])[.][0-5][0-9]");
                if (match.Success)
                {
                    if (match.Value == word)
                    {
                        return _NumberToTime(word);
                    }
                    else
                    {
                        string newNumber = word.Replace(".", "");
                        return _NumberToDigit(newNumber);
                    }
                }
                else
                {
                    return _NumberToTime(word);
                }
            }
            else if (isOtherNum)
            {
                string newNumber = word.Replace(".", "");
                return _NumberToDigit(newNumber);
            }
            else
            {
                return _SplitString(word);
            }
        }

        private string _Terbilang(int num)
        {
            string strDigit = "";
            string[] baseNumber = {"","1","2","3","4","5","6","7","8","9","10",
                                "11","12","13","14","15","16","17","18","19"};

            if (num < 20)
            {
                strDigit = baseNumber[num];
            }
            else if (num < 100)
            {
                strDigit = this._Terbilang(num / 10) + " puluh " + this._Terbilang(num % 10);
            }
            else if (num < 200)
            {
                strDigit = " ratus " + this._Terbilang(num - 100);
            }
            else if (num < 1000)
            {
                strDigit = this._Terbilang(num / 100) + " ratus " + this._Terbilang(num % 100);
            }
            else if (num < 2000) // kalo diatas seribu pakenya fungsi splitstring???
            {
                strDigit = " ribu   " + this._Terbilang(num - 1000);
            }
            else if (num < 1000000)
            {
                strDigit = this._Terbilang(num / 1000) + " ribu " + this._Terbilang(num % 1000);
            }
            else if (num < 1000000000)
            {
                strDigit = this._Terbilang(num / 1000000) + " juta " + this._Terbilang(num % 1000000);
            }

            strDigit = Regex.Replace(strDigit, @"^\s+|\s+$", " ");

            return strDigit;
        }

        private string[] _NumberToDigit(string number)
        {
            // Kondisi jika dia adalah angka biasa
            string str = "";
            List<string> digits = new List<string>();
            if (Int32.Parse(number) == 0)
            {
                str = "0";
            }
            else
            {
                str = _Terbilang(Int32.Parse(number));
            }

            string[] finalDigit = Regex.Split(str, @"[\s\\?]");

            for (int i = 0; i < finalDigit.Length; i++)
            {
                if (!string.IsNullOrEmpty(finalDigit[i]))
                {
                    digits.Add(finalDigit[i]);
                }
            }

            // //TODO: Perbaiki
            // for(int i=0; i<len; i++) {
            //     string num = "";
            //     num += number[i];
            //     digits.Add(num);
            // }
            /*  
            for(int i=0; i<len; i++) {
                string num = "";
                if (number[i] != '0') {
                    num += number[i];
                    for (int j=0; j<(len-i-1); j++) {
                        num += "0";
                    }
                    digits.Add(num);
                }
            }
            */
            return digits.ToArray();
        }

        private string[] _NumberToTime(string number)
        {
            string tnumber = number.Replace(".", " lebih ");
            //split : 19 lebih 30
            string[] tnToken = Regex.Split(tnumber, @"[\s\\?]");

            string str = "";
            string str1 = "";
            string str2 = "";
            List<string> digits = new List<string>();

            if (Int32.Parse(tnToken[0]) == 0)
            {
                str1 = "0";
            }
            else
            {
                str1 = _Terbilang(Int32.Parse(tnToken[0]));
            }

            if (Int32.Parse(tnToken[2]) == 0)
            {
                str2 = "0";
            }
            else
            {
                str2 = _Terbilang(Int32.Parse(tnToken[2]));
            }

            str = str1 + " " + tnToken[1] + " " + str2;
            Debug.LogWarning(str);

            string[] finalDigit = Regex.Split(str, @"[\s\\?]");

            for (int i = 0; i < finalDigit.Length; i++)
            {
                if (!string.IsNullOrEmpty(finalDigit[i]))
                {
                    digits.Add(finalDigit[i]);
                }
            }

            return digits.ToArray();
        }

        private string[] _SplitString(string word)
        {
            string[] words = Regex.Split(word, string.Empty);
            words = words.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return words;
        }
    }
}