using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using Animancer;

public enum CharacterNames
{
    Andi,
    Aini
}

public class TextProcessing : MonoBehaviour
{
    public static TextProcessing Instance { get; private set; }

    public const string PREF_KEY_CURRENT_CHARACTER = "currentCharacter";

    [Header("Database")]
    [SerializeField] TextAsset Data_Location;
    [SerializeField] TextAsset Data_Location_Gestures;
    [SerializeField] TextAsset Data_Location_Slang;

    [Header("Character")]
    [SerializeField] GameObject _AndiModel;
    [SerializeField] NamedAnimancerComponent _Andi;
    [SerializeField] NamedAnimancerComponent _AndiTongue;
    [SerializeField] NamedAnimancerComponent _AndiBody;

    [SerializeField] GameObject _AiniModel;
    [SerializeField] NamedAnimancerComponent _Aini;
    [SerializeField] NamedAnimancerComponent _AiniTongue;
    [SerializeField] NamedAnimancerComponent _AiniBody;

    [Header("Current Variables")]
    public float currentSliderSpeedValue = 1.0f;
    public bool currentUseTransition = true;
    public bool currentUseLog = false;

    NamedAnimancerComponent m_animancer;
    NamedAnimancerComponent m_animancerTongue;
    NamedAnimancerComponent m_animancerBody;
    Coroutine m_animancerHeadTongueCoroutine;
    Coroutine m_animancerBodyCoroutine;

    #region Android Callbacks
    public void setSliderSpeedValue(string value)
    {
        currentSliderSpeedValue = float.Parse(value);
    }

    public void triggerModel(string model)
    {
        PlayerPrefs.SetString(PREF_KEY_CURRENT_CHARACTER, model);

        bool isAndi = model == CharacterNames.Andi.ToString();
        _AndiModel.SetActive(isAndi);
        _AiniModel.SetActive(!isAndi);
        m_animancer = (isAndi) ? _Andi : _Aini;
        m_animancerTongue = (isAndi) ? _AndiTongue : _AiniTongue;
        m_animancerBody = (isAndi) ? _AndiBody : _AiniBody;
    }

    public void getInputFromAndroid(string text)
    {
        string rawText = text;

        string[] rawToken = tokenizeText(rawText);
        string[] correctedToken = spellingChecker(rawToken);
        List<string> komponenKata = deconstructWord(correctedToken);
        List<string> komponenKata2 = deconstructWord2(correctedToken);

        UITextProcessing.Instance.DebugTextOutput(komponenKata2);

        if (m_animancerHeadTongueCoroutine != null) StopCoroutine(m_animancerHeadTongueCoroutine);
        if (m_animancerBodyCoroutine != null) StopCoroutine(m_animancerBodyCoroutine);
        m_animancerHeadTongueCoroutine = StartCoroutine(_SibiAnimationSequence(new NamedAnimancerComponent[] { m_animancer, m_animancerTongue }, komponenKata));
        m_animancerBodyCoroutine = StartCoroutine(_SibiAnimationSequence(new NamedAnimancerComponent[] { m_animancerBody }, komponenKata2, true));
    }
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        Instance = this;

        triggerModel(PlayerPrefs.GetString(PREF_KEY_CURRENT_CHARACTER, CharacterNames.Andi.ToString()));
        StartCoroutine(_SpeedHandler());
    }
    #endregion

    private IEnumerator _SibiAnimationSequence(NamedAnimancerComponent[] animancers, List<string> komponenKata,  bool sendToUI = false)
    {
        float fadeDuration = (currentUseTransition) ? 0.25f : 0.0f;
        float stateSpeed = 1.0f;
        int idx = 0;

        AnimancerState state = animancers[0].States.Current;

        foreach (string kata in komponenKata)
        {
            if(sendToUI)
            {
                UITextProcessing.Instance.SendTextResultToUI(idx, komponenKata);
                idx += 1;
            }

            foreach(NamedAnimancerComponent animancer in animancers)
            {
                state = animancer.TryPlay(kata, fadeDuration);
                if(state != null) state.Speed = stateSpeed;
            }

            if(state != null)
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

    private IEnumerator _SpeedHandler()
    {
        while(true)
        {
            Time.timeScale = currentSliderSpeedValue;
            yield return null;
        }
    }


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

    #region [JSON Load Handler]
    public Dictionary<string, Kata> loadTableLookup()
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

    public List<string> loadAllKata()
    {
        string jsonData = Data_Location.ToString();

        KataBerimbuhan tempList = JsonUtility.FromJson<KataBerimbuhan>(jsonData);

        List<string> result = new List<string>();

        foreach (Kata kata in tempList.listKata)
        {
            if (!result.Contains(kata.pokok))
            {
                result.Add(kata.pokok);
            }
        }
        return result;
    }

    public Dictionary<string, Gesture> loadGestureLookup()
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

    public Dictionary<string, Slang> loadSlangLookup()
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

    #region Text Processing
    /**
     * <summary>
     * Tokenization is essentially splitting a phrase, sentence, paragraph, or an entire text document into smaller units
     * , such as individual words or terms. Each of these smaller units are called tokens.
     * </summary>
     */
    public string[] tokenizeText(string input)
    {
        string[] rawToken = Regex.Split(input, @"[\s\\?]");
        rawToken = rawToken
                    .Select(x => x.ToLowerInvariant())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToArray();
        return rawToken;
    }

    /**
     * <summary>
     * To correct any slang in a word
     * Ex. trmksih --> terima kasih
     * </summary>
     */
    public string[] spellingChecker(string[] token)
    {
        Dictionary<string, Slang> tableLookup = loadSlangLookup();
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
     * Ex. Berfungsi --> Ber Fungsi
     * </summary>
     */
    public List<string> deconstructWord(string[] token)
    {
        Dictionary<string, Kata> tableLookup = loadTableLookup();
        List<string> komponenKata = new List<string>();

        foreach (string t in token)
        {
            if (tableLookup.ContainsKey(t))
            {
                // Cek apakah kata merupakan kata majemuk
                if (isMajemuk(t))
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

        //foreach (string kata in komponenKata)
        //{
        //    print(kata);
        //}

        List<string> finalKomponen = wordToGesture(komponenKata);
        return finalKomponen;
    }

    public List<string> deconstructWord2(string[] token)
    {
        Dictionary<string, Kata> tableLookup = loadTableLookup();
        List<string> komponenKata = new List<string>();

        foreach (string t in token)
        {
            if (tableLookup.ContainsKey(t))
            {
                // Cek apakah kata merupakan kata majemuk
                if (isMajemuk(t))
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

         /*foreach (string kata in komponenKata) {
             print(kata);
         }*/

        List<string> finalKomponen = wordToGesture(komponenKata);
        return finalKomponen;
    }

    /* 
        Persiapan lookup gerakan, jika tidak ditemukan pecah jadi alfabet
        jika merupakan angka, pecah sesuai digit nya
        todo --> cek langsung ke animancernya aja daripada lewat table
    */
    public List<string> wordToGesture(List<string> komponenKata)
    {
        Dictionary<string, Gesture> gestureLookup = loadGestureLookup();
        List<string> finalKomponen = new List<string>();

        foreach (string kata in komponenKata)
        {
            if (gestureLookup.ContainsKey(kata))
            {
                finalKomponen.Add(kata);
            }
            else
            {
                string[] split = abjadChecker(kata);
                foreach (string s in split)
                {
                    finalKomponen.Add(s);
                }
            }
        }

        return finalKomponen;
    }

    public bool isMajemuk(string word)
    {
        return Regex.IsMatch(word, @"[-]");
    }

    public string[] abjadChecker(string word)
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
                    return splitString(word);
                }
                else
                {
                    return numberToDigit(word);
                }
            }
            else
            {
                return splitString(word);
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
                    return numberToTime(word);
                }
                else
                {
                    string newNumber = word.Replace(".", "");
                    return numberToDigit(newNumber);
                }
            }
            else
            {
                return numberToTime(word);
            }
        }
        else if (isOtherNum)
        {
            string newNumber = word.Replace(".", "");
            return numberToDigit(newNumber);
        }
        else
        {
            return splitString(word);
        }
    }

    public string terbilang(int num)
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
            strDigit = this.terbilang(num / 10) + " puluh " + this.terbilang(num % 10);
        }
        else if (num < 200)
        {
            strDigit = " ratus " + this.terbilang(num - 100);
        }
        else if (num < 1000)
        {
            strDigit = this.terbilang(num / 100) + " ratus " + this.terbilang(num % 100);
        }
        else if (num < 2000)
        {
            strDigit = " ribu   " + this.terbilang(num - 1000);
        }
        else if (num < 1000000)
        {
            strDigit = this.terbilang(num / 1000) + " ribu " + this.terbilang(num % 1000);
        }
        else if (num < 1000000000)
        {
            strDigit = this.terbilang(num / 1000000) + " juta " + this.terbilang(num % 1000000);
        }

        strDigit = System.Text.RegularExpressions.Regex.Replace(strDigit, @"^\s+|\s+$", " ");

        return strDigit;
    }
    public string[] numberToDigit(string number)
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
            str = terbilang(Int32.Parse(number));
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

    public string[] numberToTime(string number)
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
            str1 = terbilang(Int32.Parse(tnToken[0]));
        }

        if (Int32.Parse(tnToken[2]) == 0)
        {
            str2 = "0";
        }
        else
        {
            str2 = terbilang(Int32.Parse(tnToken[2]));
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

    public string[] splitString(string word)
    {
        string[] words = Regex.Split(word, string.Empty);
        words = words.Where(x => !string.IsNullOrEmpty(x)).ToArray();
        return words;
    }
    #endregion
}
