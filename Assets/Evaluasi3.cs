using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Animancer;
using UnityEngine.SceneManagement;

public class Evaluasi3 : MonoBehaviour
{
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
                Application.Quit();
            else
                SceneManager.LoadScene(0);
        }
    }

    [SerializeField] private NamedAnimancerComponent _Animancer;
    [SerializeField] private NamedAnimancerComponent _Animancertongue;
    [SerializeField] private NamedAnimancerComponent _Animancerbody;

    public Toggle toggleTransisi;
    public Slider sliderSpeed;
    public Dropdown dropdownKuisioner;

    public Text txtQuestion;
    public Text txtNomor;

    private List<Kuisioner> currentKuisioner = new List<Kuisioner>();

    #region [Objek Kata Berimbuhan]
    [System.Serializable]
    public class Kata {
        public string id;
        public string[] awalan;
        public string[] akhiran;
        public string[] suku;
        public string pokok;
    }

    [System.Serializable]
    public class KataBerimbuhan {
        public List<Kata> listKata;
    }
    #endregion

    #region [Objek Kata Gesture]
    [System.Serializable]
    public class Gesture {
        public string id;
        public string jenis1;
        public string jenis2;
        public string gerakan;
    }

    [System.Serializable]
    public class GestureDictionary {
        public List<Gesture> listGesture;
    }
    #endregion

    #region [Objek Kata Slang]
    [System.Serializable]
    public class Slang {
        public string slang;
        public string formal;
    }

    [System.Serializable]
    public class SlangDictionary {
        public List<Slang> listSlang;
    }
    #endregion

    #region [Objek Kata Kuisioner]
    [System.Serializable]
    public class Kuisioner {
        public string id;
        public string jenis;
        public string kalimat;
    }

    [System.Serializable]
    public class KuisionerDictionary {
        public List<Kuisioner> listKuisioner;
    }
    #endregion

    private static string DATA_LOCATION_SAMPLE = "/Database/Sample.json";
    private static string DATA_LOCATION = "Database/TableLookup";
    private static string DATA_LOCATION_GESTURES = "Database/GestureLookup";
    private static string DATA_LOCATION_SLANG = "Database/SlangLookup";

    private static string DATA_KUISIONER = "Database/Kuisioner2";

    #region [JSON Load Handler]
    public void singleLoadTableLookup() {
        string jsonData = File.ReadAllText(Application.dataPath + DATA_LOCATION_SAMPLE);
        Kata kata = JsonUtility.FromJson<Kata>(jsonData);
    }

    public Dictionary<string, Kata> loadTableLookup() {
        //string jsonData = File.ReadAllText(Application.dataPath + DATA_LOCATION);
        //string jsonData = File.ReadAllText(Application.persistentDataPath + DATA_LOCATION);
        
        TextAsset file = Resources.Load(DATA_LOCATION) as TextAsset;
        string jsonData = file.ToString();

        KataBerimbuhan tempList = JsonUtility.FromJson<KataBerimbuhan>(jsonData);

        Dictionary<string, Kata> tableLookup = new Dictionary<string, Kata>();

        foreach( Kata kata in tempList.listKata ) {
            tableLookup.Add(kata.id, kata);
        }

        return tableLookup;
    }

    public Dictionary<string, Gesture> loadGestureLookup() {
        //string jsonData = File.ReadAllText(Application.dataPath + DATA_LOCATION_GESTURES);
        //string jsonData = File.ReadAllText(Application.persistentDataPath + DATA_LOCATION_GESTURES);
        
        TextAsset file = Resources.Load(DATA_LOCATION_GESTURES) as TextAsset;
        string jsonData = file.ToString();
        
        GestureDictionary tempList = JsonUtility.FromJson<GestureDictionary>(jsonData);

        Dictionary<string, Gesture> tableLookup = new Dictionary<string, Gesture>();

        foreach( Gesture gesture in tempList.listGesture ) {
            tableLookup.Add(gesture.id, gesture);
        }

        return tableLookup;
    }

    public Dictionary<string, Slang> loadSlangLookup() {
        //string jsonData = File.ReadAllText(Application.dataPath + DATA_LOCATION_SLANG);
        //string jsonData = File.ReadAllText(Application.persistentDataPath + DATA_LOCATION_SLANG);
        
        TextAsset file = Resources.Load(DATA_LOCATION_SLANG) as TextAsset;
        string jsonData = file.ToString();
        
        SlangDictionary  tempList = JsonUtility.FromJson<SlangDictionary>(jsonData);

        Dictionary<string, Slang> tableLookup = new Dictionary<string, Slang>();

        foreach( Slang slang in tempList.listSlang ) {
            tableLookup.Add(slang.slang, slang);
        }

        return tableLookup; 
    }

    public Dictionary<string, Kuisioner> loadKuisioner() {
        TextAsset file = Resources.Load(DATA_KUISIONER) as TextAsset;
        string jsonData = file.ToString();

        KuisionerDictionary tempList = JsonUtility.FromJson<KuisionerDictionary>(jsonData);

        Dictionary<string, Kuisioner> tableLookup = new Dictionary<string, Kuisioner>();

        foreach( Kuisioner kuisioner in tempList.listKuisioner ) {
            tableLookup.Add(kuisioner.id, kuisioner);
        }

        return tableLookup;
    }
    #endregion

    public string currentKalimat;
    public string currentId;
    public int currentOrder = 0;
    public int currentTotal;

    public void Start() {
        getKuisioner();   
        //getInputText(kuisioner.kalimat);
        // initialize random
    }

    public void getKuisioner() {
        currentKuisioner.Clear();
        Dictionary<string, Kuisioner> tableKuisioner = loadKuisioner();
        
        //Kuisioner kuisioner = tableKuisioner["K001"];
        int counter = 0;
        string jenisKuisioner = dropdownKuisioner.options[dropdownKuisioner.value].text;
        foreach(KeyValuePair<string, Kuisioner> k in tableKuisioner) {
            if (k.Value.jenis == jenisKuisioner) {
                Debug.LogWarning(k.Value.kalimat);
                currentKuisioner.Add(k.Value);
                counter++;
            }
        }

        this.currentOrder = 0;
        this.currentKalimat = currentKuisioner.ElementAt(currentOrder).kalimat;
        this.currentId = currentKuisioner.ElementAt(currentOrder).id;
        this.currentTotal = counter;

        txtQuestion.text = this.currentId;
        txtNomor.text = (currentOrder+1) + " dari " + counter;
    }

    public void nextKuisioner() {
        if (this.currentOrder == this.currentTotal-1) {
            this.currentOrder = 0;
        } else {
            this.currentOrder++;
        }
        this.currentKalimat = currentKuisioner.ElementAt(this.currentOrder).kalimat;
        this.currentId = currentKuisioner.ElementAt(this.currentOrder).id;
        txtQuestion.text = this.currentId;
        txtNomor.text = (this.currentOrder+1) + " dari " + this.currentTotal;
    }

    public void backKuisioner() {
        if (this.currentOrder == 0) {
            this.currentOrder = this.currentTotal-1;
        } else {
            this.currentOrder--;
        }
        this.currentKalimat = currentKuisioner.ElementAt(this.currentOrder).kalimat;
        this.currentId = currentKuisioner.ElementAt(this.currentOrder).id;
        txtQuestion.text = this.currentId;
        txtNomor.text = (this.currentOrder+1) + " dari " + this.currentTotal;
    }

    public void getInputText() {
        //get raw text from random sentences 28 sentences
        string rawText = currentKalimat;
        //Debug.Log(rawText);

        string[] rawToken = tokenizeText(rawText);
        string[] correctedToken = spellingChecker(rawToken);
        List<string> komponenKata = deconstructWord(correctedToken);
        List<string> komponenKata2 = deconstructWord2(correctedToken);


        StopAllCoroutines();
        // Production Mode
        StartCoroutine(SibiAnimationSequence(komponenKata));
        StartCoroutine(SibiAnimationSequence2(komponenKata2));
        
        
    }

    private IEnumerator SibiAnimationSequence(List<string> komponenKata) {
        AnimancerState previousState = _Animancer.CurrentState;
        AnimancerState state;

        AnimancerState previousStatet = _Animancertongue.CurrentState;
        AnimancerState statet;

        // Fade duration increases as the state speed increases
        // Default is 0.25
        float fadeDuration = 0.2f / sliderSpeed.value;
        float stateSpeed = sliderSpeed.value;
        
        foreach (string kata in komponenKata) {
            if(toggleTransisi.isOn) {
                //Dengan transisi
                state = _Animancer.CrossFadeFromStart(kata, fadeDuration);
                state.Speed = stateSpeed;

                statet = _Animancertongue.CrossFadeFromStart(kata, fadeDuration);
                statet.Speed = stateSpeed;
                
            } else {
                //Tanpa transisi
                state = _Animancer.Play(kata);
                state.Time = 0;

                statet = _Animancertongue.Play(kata);
                statet.Time = 0;
            }

            while(state.Time < state.Length) {
                yield return null;  
            }

            previousState = state;
            previousStatet = statet;
        }

        if (toggleTransisi.isOn) {
            // Important, also add the state speed and fade duration
            // for transition consistency 
            state = _Animancer.CrossFadeFromStart("idle", fadeDuration);
            state.Speed = stateSpeed;

            statet = _Animancertongue.CrossFadeFromStart("idle", fadeDuration);
            statet.Speed = stateSpeed;

        } else {
            _Animancer.Play("idle");
            _Animancertongue.Play("idle");
        }
    }



    private IEnumerator SibiAnimationSequence2(List<string> komponenKata2) {
        AnimancerState previousStateing = _Animancerbody.CurrentState;
        AnimancerState stateing;

        // Fade duration increases as the state speed increases
        // Default is 0.25
        float fadeDuration = 0.2f / sliderSpeed.value;
        float stateSpeed = sliderSpeed.value;
        
        foreach (string kata in komponenKata2) {
            if(toggleTransisi.isOn) {
                //Dengan transisi
                stateing = _Animancerbody.CrossFadeFromStart(kata, fadeDuration);
                stateing.Speed = stateSpeed;
                
            } else {
                //Tanpa transisi
                stateing = _Animancerbody.Play(kata);
                stateing.Time = 0;
            }

            while(stateing.Time < stateing.Length) {
                yield return null;  
            }

            previousStateing = stateing;
        }

        if (toggleTransisi.isOn) {
            // Important, also add the state speed and fade duration
            // for transition consistency 
            stateing = _Animancerbody.CrossFadeFromStart("idle", fadeDuration);
            stateing.Speed = stateSpeed;

        } else {
            _Animancerbody.Play("idle");
        }
    }

    public void PlaySequence(List<string> komponenKata)
    {
        StopAllCoroutines();
        // Production Mode
        //if(logMode.isOn) {
            // Enable Logging
            // StartCoroutine(SibiAnimationSequenceLogRotation(komponenKata));
        //} else {
            StartCoroutine(SibiAnimationSequence(komponenKata));
        //}
    }

    public string[] tokenizeText(string input) {
        string[] rawToken = Regex.Split(input, @"[\s\\?]");
        rawToken = rawToken
                    .Select(x => x.ToLowerInvariant())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToArray();
        return rawToken;
    }

    public string[] spellingChecker(string[] token) {
        Dictionary<string, Slang> tableLookup = loadSlangLookup();
        List<string> correctedToken = new List<string>();

        foreach (string t in token) {
            if (tableLookup.ContainsKey(t)) {
                Debug.Log("correction occur : " + t + "->" + tableLookup[t].formal);
                correctedToken.Add(tableLookup[t].formal);
            } else {
                correctedToken.Add(t);
            }
        }

        return correctedToken.ToArray();
    }


    /* 
        Untuk melakukan dekonstruksi kata ke tabel lookup kata berimbuhan
    */
    public List<string> deconstructWord(string[] token) {
        Dictionary<string, Kata> tableLookup = loadTableLookup();
        List<string> komponenKata =  new List<string>();

        foreach (string t in token) {
            if (tableLookup.ContainsKey(t)) {
                // Cek apakah kata merupakan kata majemuk
                if (isMajemuk(t)) {
                    // 1. Tambah kata dasar 
                    komponenKata.Add(tableLookup[t].pokok);
                    komponenKata.Add(tableLookup[t].pokok);

                    // 2. Tambah awalan
                    // int indexDasar = komponenKata.IndexOf(tableLookup[t].pokok);
                    // Debug.Log(indexDasar);
                    

                    
                    foreach(string suku in tableLookup[t].suku) {
                        if (suku != "") {
                            Match match = Regex.Match(suku, @"[0-9]");
                            string matchVal = "0";
                            if (match.Success) {
                                matchVal = match.Value;
                            }
                            int pos = int.Parse(matchVal);

                            string csuku = Regex.Replace(suku, @"[^a-zA-Z]", "");
                            
                            if (pos == 1) {
                                komponenKata.Insert(komponenKata.LastIndexOf(tableLookup[t].pokok), csuku);
                            } else {
                                komponenKata.Insert(komponenKata.LastIndexOf(tableLookup[t].pokok) + 1, csuku);
                            }
                        }
                    }
                } else {
                    


                    foreach(string suku in tableLookup[t].suku) {
                        if (suku != "") {
                            komponenKata.Add(suku);
                        }
                    }

                    
                }
            } else {
                komponenKata.Add(t);
            }
        }

        // foreach (string kata in komponenKata) {
        //     print(kata);
        // }

        List<string> finalKomponen = wordToGesture(komponenKata);
        return finalKomponen;
    }

    public List<string> deconstructWord2(string[] token) {
        Dictionary<string, Kata> tableLookup = loadTableLookup();
        List<string> komponenKata =  new List<string>();

        foreach (string t in token) {
            if (tableLookup.ContainsKey(t)) {
                // Cek apakah kata merupakan kata majemuk
                if (isMajemuk(t)) {
                    // 1. Tambah kata dasar 
                    komponenKata.Add(tableLookup[t].pokok);
                    komponenKata.Add(tableLookup[t].pokok);

                    // 2. Tambah awalan
                    // int indexDasar = komponenKata.IndexOf(tableLookup[t].pokok);
                    // Debug.Log(indexDasar);
                    foreach(string awalan in tableLookup[t].awalan) {
                        if (awalan != "") {
                            Match match = Regex.Match(awalan, @"[0-9]");
                            string matchVal = "0";
                            if (match.Success) {
                                matchVal = match.Value;
                            }
                            int pos = int.Parse(matchVal);

                            string cAwalan = Regex.Replace(awalan, @"[^a-zA-Z]", "");
                            
                            if (pos == 1) {
                                komponenKata.Insert(komponenKata.IndexOf(tableLookup[t].pokok), cAwalan);
                            } else {
                                komponenKata.Insert(komponenKata.IndexOf(tableLookup[t].pokok) + 1, cAwalan);
                            }
                        }
                    }

                    foreach(string akhiran in tableLookup[t].akhiran) {
                        if (akhiran != "") {
                            Match match = Regex.Match(akhiran, @"[0-9]");
                            string matchVal = "0";
                            if (match.Success) {
                                matchVal = match.Value;
                            }
                            int pos = int.Parse(matchVal);

                            string cAkhiran = Regex.Replace(akhiran, @"[^a-zA-Z]", "");

                            if (akhiran == "i") {
                                cAkhiran = "-" + cAkhiran;
                            }
                            
                            if (pos == 1) {
                                komponenKata.Insert(komponenKata.LastIndexOf(tableLookup[t].pokok), cAkhiran);
                            } else {
                                komponenKata.Insert(komponenKata.LastIndexOf(tableLookup[t].pokok) + 1, cAkhiran);
                            }
                        }
                    }
                    
                } else {
                    foreach(string awalan in tableLookup[t].awalan) {
                        if (awalan != "") {
                            komponenKata.Add(awalan + "-");
                        }
                    }


                    komponenKata.Add(tableLookup[t].pokok);

                    foreach (string akhiran in tableLookup[t].akhiran) {
                        if (akhiran != "") {
                            if (akhiran == "i") {
                                komponenKata.Add("-" + akhiran);
                            } else {
                                komponenKata.Add("-" + akhiran);
                            }
                        }
                    }
                }
            } else {
                komponenKata.Add(t);
            }
        }

        // foreach (string kata in komponenKata) {
        //     print(kata);
        // }

        List<string> finalKomponen = wordToGesture(komponenKata);
        return finalKomponen;
    }

    /* 
        Persiapan lookup gerakan, jika tidak ditemukan pecah jadi alfabet
        jika merupakan angka, pecah sesuai digit nya
    */
    public List<string> wordToGesture(List<string> komponenKata) {
        Dictionary<string, Gesture> gestureLookup = loadGestureLookup();
        List<string> finalKomponen =  new List<string>();

        foreach (string kata in komponenKata) {
            if (gestureLookup.ContainsKey(kata)) {
                finalKomponen.Add(kata);
            } else {
                string[] split = abjadChecker(kata);
                foreach (string s in split) {
                    finalKomponen.Add(s);
                }
            }
        }

        return finalKomponen;
    }

    public bool isMajemuk(string word) {
        return Regex.IsMatch(word, @"[-]");
    }

    public string[] abjadChecker(string word) {
        bool isNumeric = Regex.IsMatch(word, @"^[0-9]+$");
        bool isTimeFormat = Regex.IsMatch(word, @"(?:[01][0-9]|2[0-3]|[1-9])[.][0-5][0-9]");
        bool isOtherNum = Regex.IsMatch(word, @"(?:[0-9][0-9]|2[0-3]|[1-9])[.][0-5][0-9]");

        if (isNumeric) {
            int num;
            bool parsed = Int32.TryParse(word, out num);
            if (parsed) {
                if (num >= 1000) {
                    return splitString(word);
                } else {
                    return numberToDigit(word);
                }
            } else {
                return splitString(word);
            }
        } else if (isTimeFormat) { 
            // Additional checker
            Match match = Regex.Match(word, @"(?:[01][0-9]|2[0-3]|[1-9])[.][0-5][0-9]");
            if (match.Success){
                if(match.Value == word) {
                    return numberToTime(word);
                } else {
                    string newNumber = word.Replace(".", "");
                    return numberToDigit(newNumber);
                }
            } else {
                return numberToTime(word);
            }
        } else if (isOtherNum) {
            string newNumber = word.Replace(".", "");
            return numberToDigit(newNumber);
        } else {
            return splitString(word);
        }
    }

    public string terbilang(int num) {
        string strDigit = "";
        string[] baseNumber = {"","1","2","3","4","5","6","7","8","9","10",
                                "11","12","13","14","15","16","17","18","19"};

        if (num < 20) {
            strDigit = baseNumber[num];
        } else if (num < 100) {
            strDigit = this.terbilang(num / 10) + " puluh " + this.terbilang(num % 10);
        } else if (num < 200) {
            strDigit = " ratus " + this.terbilang(num - 100);
        } else if (num < 1000) {
            strDigit = this.terbilang(num / 100) + " ratus " + this.terbilang(num % 100);
        } else if (num < 2000) {
            strDigit = " ribu   " + this.terbilang(num - 1000);
        } else if (num < 1000000) {
            strDigit = this.terbilang(num / 1000) + " ribu " + this.terbilang(num % 1000);
        } else if (num < 1000000000) {
            strDigit = this.terbilang(num / 1000000) + " juta " + this.terbilang(num % 1000000);
        }

        strDigit = System.Text.RegularExpressions.Regex.Replace(strDigit, @"^\s+|\s+$", " ");

        return strDigit;
    }
    public string[] numberToDigit(string number) {
        // Kondisi jika dia adalah angka biasa
        string str = "";
        List<string> digits = new List<string>();
        if (Int32.Parse(number) == 0) {
            str = "0";
        } else {
            str = terbilang(Int32.Parse(number));
        }
        
        string[] finalDigit = Regex.Split(str, @"[\s\\?]");
        
        for(int i=0; i<finalDigit.Length; i++) {
            if (!string.IsNullOrEmpty(finalDigit[i])) {
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

    public string[] numberToTime(string number) {
        string tnumber = number.Replace(".", " lebih ");
        //split : 19 lebih 30
        string[] tnToken = Regex.Split(tnumber, @"[\s\\?]");

        string str = "";
        string str1 = "";
        string str2 = "";
        List<string> digits = new List<string>();

        if (Int32.Parse(tnToken[0]) == 0) {
            str1 = "0";
        } else {
            str1 = terbilang(Int32.Parse(tnToken[0]));
        }

        if (Int32.Parse(tnToken[2]) == 0) {
            str2 = "0";
        } else {
            str2 = terbilang(Int32.Parse(tnToken[2]));
        }

        str = str1 + " " + tnToken[1] + " " + str2;
        Debug.LogWarning(str);
        
        string[] finalDigit = Regex.Split(str, @"[\s\\?]");
        
        for(int i=0; i<finalDigit.Length; i++) {
            if (!string.IsNullOrEmpty(finalDigit[i])) {
                digits.Add(finalDigit[i]);
            }
        }

        return digits.ToArray();
    }

    public string[] splitString(string word) {
        string[] words = Regex.Split(word, string.Empty);
        words = words.Where(x => !string.IsNullOrEmpty(x)).ToArray();
        return words;
    }
}
