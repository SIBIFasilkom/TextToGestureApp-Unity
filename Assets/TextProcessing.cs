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

public class TextProcessing : MonoBehaviour
{
    
    [SerializeField] 
    public NamedAnimancerComponent _Animancer;
    public NamedAnimancerComponent _Animancertongue;
    public NamedAnimancerComponent _Animancerbody;

    public NamedAnimancerComponent _Andi;
    public NamedAnimancerComponent _AndiTongue;
    public NamedAnimancerComponent _AndiBody;

    public NamedAnimancerComponent _Aini;
    public NamedAnimancerComponent _AiniTongue;
    public NamedAnimancerComponent _AiniBody;
    public GameObject _AndiModel;
    public GameObject _AiniModel;

    public InputField txtInput;

    public Text txtOutput;

    public Toggle toggleTransisi;
    public Toggle logMode;
    public Slider sliderSpeed;
    
    public Text txtKalimat;

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

    [System.Serializable]
    public class Frame {

    }
    #endregion

    private static string DATA_LOCATION_SAMPLE = "/Database/Sample.json";
    private static string DATA_LOCATION = "Database/TableLookup4";
    private static string DATA_LOCATION_GESTURES = "Database/GestureLookup";
    private static string DATA_LOCATION_GESTURES_ALFABET = "Database/GestureLookupAlfabet";
    private static string DATA_LOCATION_SLANG = "Database/SlangLookup";

    public Dictionary<string,AnimationClip> animations = new Dictionary<string,AnimationClip>();

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

    public List<string> loadAllKata() {
        TextAsset file = Resources.Load(DATA_LOCATION) as TextAsset;
        string jsonData = file.ToString();

        KataBerimbuhan tempList = JsonUtility.FromJson<KataBerimbuhan>(jsonData);

        List<string> result = new List<string>();

        foreach( Kata kata in tempList.listKata ) {
            if (!result.Contains(kata.pokok)){
                result.Add(kata.pokok);
            }
        }
        return result;
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

    public Dictionary<string, Gesture> loadGestureLookupAlfabet() {
        //string jsonData = File.ReadAllText(Application.dataPath + DATA_LOCATION_GESTURES);
        //string jsonData = File.ReadAllText(Application.persistentDataPath + DATA_LOCATION_GESTURES);
        
        TextAsset file = Resources.Load(DATA_LOCATION_GESTURES_ALFABET) as TextAsset;
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

    
    #endregion

    public void Start() {
        // ApplicationChrome.statusBarState = ApplicationChrome.navigationBarState = ApplicationChrome.States.Visible;
        _Animancer = _Andi;
        _Animancertongue = _AndiTongue;
        _Animancerbody = _AndiBody;
        Screen.orientation = ScreenOrientation.Portrait;

        // UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/SIBI/", typeof(Motion[]));
        AnimationClip[] foundObjects = Resources.LoadAll<AnimationClip>("");
        var clips = Resources.FindObjectsOfTypeAll<AnimationClip>();


        // Prepare the animations dictionary
        foreach (var c in clips) {
            print(c.name);
            animations[c.name.ToLowerInvariant()] =  c;
        }

    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Escape)) {
        //     using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")){
        //         using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity")){
        //             obj_Activity.Call("customBackPressed");
        //         }
        //     }
        // }
    }

    public float sliderSpeedValue = 0;

    public void setSliderSpeedValue(string value){
        sliderSpeedValue = float.Parse(value);
    }

    // public void triggerModel(string model)
    // {
        
    //     if (model == "andi"){
    //         _Andi.SetActive(true);
    //         _Aini.SetActive(false);
    //         // _Animancer = _andi;
    //     }
    //     else{   
    //         _Aini.SetActive(false);
    //         _Andi.SetActive(true);
    //         // _Animancer = _aini;
    //     }
    // }

    // public void changeAndi()
    // {
    //     triggerModel("andi");
    // }
    // public void changeAini()
    // {
    //     triggerModel("aini");
    // }

    public void triggerModel(string model)
    {
        
        if (model == "Andi"){
            _AndiModel.SetActive(true);
            _AiniModel.SetActive(false);
            _Animancer = _Andi;
            _Animancertongue = _AndiTongue;
            _Animancerbody = _AndiBody;
        }
        else{   
            _AndiModel.SetActive(false);
            _AiniModel.SetActive(true);
            _Animancer = _Aini;
            _Animancertongue = _AiniTongue;
            _Animancerbody = _AiniBody;
        }
    }

    public void changeAndi()
    {
        triggerModel("Andi");
    }
    public void changeAini()
    {
        triggerModel("Aini");
    }

    public void getInputText() {
        string rawText = txtInput.text;
        //Debug.Log(rawText);

        string[] rawToken = tokenizeText(rawText);
        string[] correctedToken = spellingChecker(rawToken);
        List<string> komponenKata = deconstructWord(correctedToken);
        List<string> komponenKata2 = deconstructWord2(correctedToken);

        string output = "";
        
        foreach (string kata in komponenKata2) {
            output += kata + ";";
            //_Animancer.CrossFade(kata);   
        }
        
        txtOutput.text = output;

        StopAllCoroutines();
        // Production Mode
        if(logMode.isOn) {
            // Enable Logging
            StartCoroutine(SibiAnimationSequenceLogRotation(komponenKata));
            StartCoroutine(SibiAnimationSequenceLogRotation2(komponenKata2));
        } else {
            StartCoroutine(SibiAnimationSequence(komponenKata));
            StartCoroutine(SibiAnimationSequence2(komponenKata2));
            printHeader();
        }
    }

    // To be called by Android
    public void getInputFromAndroid(string text) {
        string rawText = text;

        string[] rawToken = tokenizeText(rawText);
        string[] correctedToken = spellingChecker(rawToken);
        List<string> komponenKata = deconstructWord(correctedToken);
        List<string> komponenKata2 = deconstructWord2(correctedToken);

        string output = "";
        
        foreach (string kata in komponenKata2) {
            output += kata + ";";
            //_Animancer.CrossFade(kata);   
        }
        
        txtOutput.text = output;

        StopAllCoroutines();
        // Production Mode
        if(logMode.isOn) {
            // Enable Logging
            StartCoroutine(SibiAnimationSequenceLogRotation(komponenKata));
            StartCoroutine(SibiAnimationSequenceLogRotation2(komponenKata2));
        } else {
            StartCoroutine(SibiAnimationSequence(komponenKata));
            StartCoroutine(SibiAnimationSequence2(komponenKata2));
            printHeader();
        }
    }

    //TODO: Find out a way to play animation sequence with named animations
    private IEnumerator CoroutineAnimationSequence(List<string> komponenKata)
    {
        // Start at the 2nd animation (index [1]) since we are already playing the 1st.
        foreach (string kata in komponenKata) {
            // if (kata == "adalah") {
            //     _Animancer.CrossFade("adalah_2");
            //     yield return new WaitForSeconds(_Animancer.GetState("adalah_2").Length);
            // } else {
                _Animancer.CrossFade(kata);
                _Animancertongue.CrossFade(kata);
                // Transform[] ats = _Animancer.GetComponentsInChildren<Transform>();
                // foreach ( var a in ats) {
                //     Debug.Log(a);
                // }
                yield return new WaitForSeconds(_Animancer.GetState(kata).Length);
                yield return new WaitForSeconds(_Animancertongue.GetState(kata).Length);
            //}    
        }
        _Animancer.CrossFade("idle");
        _Animancertongue.CrossFade("idle");
    }

     private IEnumerator CoroutineAnimationSequence2(List<string> komponenKata2)
    {
        // Start at the 2nd animation (index [1]) since we are already playing the 1st.
        foreach (string kata in komponenKata2) {
            // if (kata == "adalah") {
            //     _Animancer.CrossFade("adalah_2");
            //     yield return new WaitForSeconds(_Animancer.GetState("adalah_2").Length);
            // } else {
                _Animancerbody.CrossFade(kata);
                // Transform[] ats = _Animancer.GetComponentsInChildren<Transform>();
                // foreach ( var a in ats) {
                //     Debug.Log(a);
                // }
                yield return new WaitForSeconds(_Animancerbody.GetState(kata).Length);
            //}    
        }
        _Animancerbody.CrossFade("idle");
    }

    // Logging 
    private IEnumerator SibiAnimationSequenceLog(List<string> komponenKata) {
        var bone = _Animancer.Animator.GetBoneTransform(HumanBodyBones.Spine);
        var transforms = bone.GetComponentsInChildren<Transform>();

        var bonet = _Animancertongue.Animator.GetBoneTransform(HumanBodyBones.Spine);
        var transformst = bonet.GetComponentsInChildren<Transform>();

        AnimancerState previousState = _Animancer.CurrentState;
        AnimancerState previousStatet = _Animancertongue.CurrentState;

        string transformsData = "";
        string transformsDatat = "";

        foreach (string kata in komponenKata) {
            var state = _Animancer.CrossFade(kata);
            var statet = _Animancertongue.CrossFade(kata);

            while(state.Time < state.Length) {
                yield return null;  
 
                // Since there are probably quite a few transforms, use some LogWarnings to clearly separate each frame.
    
                if (previousState == null) {
                    //Debug.LogWarning("Previous State: null");
                    transformsData += "Previous State: null\n";
                } else {
                    //Debug.LogWarning("Previous State: " + previousState.Clip.name + ", Time=" + previousState.Time + ", Weight=" + previousState.Weight);
                    transformsData += "Previous State: " + previousState.Clip.name + ", Time=" + previousState.Time + ", Weight=" + previousState.Weight + "\n";
                }
                
                //Debug.LogWarning("Current State: " + state.Clip.name + ", Time=" + state.Time + ", Weight=" + state.Weight);
                transformsData += "Current State: " + state.Clip.name + ", Time=" + state.Time + ", Weight=" + state.Weight + "\n\n";

                foreach (var transform in transforms) {
                    //Debug.Log(transform + " " + transform.rotation);
                    
                    transformsData += transform.name.Replace("mixamorig:", "") + " " + transform.localEulerAngles + "\n";
                    transformsData += transform.name.Replace("mixamorig:", "") + " " + transform.position + "\n";
                }

                transformsData += "===================\n\n";
            }

             while(statet.Time < statet.Length) {
                yield return null;  
 
                // Since there are probably quite a few transforms, use some LogWarnings to clearly separate each frame.
    
                if (previousStatet == null) {
                    //Debug.LogWarning("Previous State: null");
                    transformsDatat += "Previous State: null\n";
                } else {
                    //Debug.LogWarning("Previous State: " + previousState.Clip.name + ", Time=" + previousState.Time + ", Weight=" + previousState.Weight);
                    transformsDatat += "Previous State: " + previousStatet.Clip.name + ", Time=" + previousStatet.Time + ", Weight=" + previousStatet.Weight + "\n";
                }
                
                //Debug.LogWarning("Current State: " + state.Clip.name + ", Time=" + state.Time + ", Weight=" + state.Weight);
                transformsDatat += "Current State: " + statet.Clip.name + ", Time=" + statet.Time + ", Weight=" + statet.Weight + "\n\n";

                foreach (var transform in transformst) {
                    //Debug.Log(transform + " " + transform.rotation);
                    
                    transformsDatat += transform.name.Replace("mixamorig:", "") + " " + transform.localEulerAngles + "\n";
                    transformsDatat += transform.name.Replace("mixamorig:", "") + " " + transform.position + "\n";
                }

                transformsDatat += "===================\n\n";
            }            

            previousState = state;
            previousStatet = statet;
        }

        string path = Application.dataPath + "/Log.txt";
        if (!File.Exists(path)) {
            File.WriteAllText(path, "Transformations Data Log\n");
        }
        string date = "Log Date : " + System.DateTime.Now + "\n\n";
        File.AppendAllText(path, date + transformsData);

        _Animancer.CrossFade("idle");
        _Animancertongue.CrossFade("idle");
    }

    private IEnumerator SibiAnimationSequenceLog2(List<string> komponenKata2) {
        var boneing = _Animancerbody.Animator.GetBoneTransform(HumanBodyBones.Spine);
        var transformsing = boneing.GetComponentsInChildren<Transform>();

        AnimancerState previousStateing = _Animancerbody.CurrentState;

        string transformsDataing = "";

        foreach (string kata in komponenKata2) {
            var stateing = _Animancerbody.CrossFade(kata);
            
            while(stateing.Time < stateing.Length) {
                yield return null;  
 
                // Since there are probably quite a few transforms, use some LogWarnings to clearly separate each frame.
    
                if (previousStateing == null) {
                    //Debug.LogWarning("Previous State: null");
                    transformsDataing += "Previous State: null\n";
                } else {
                    //Debug.LogWarning("Previous State: " + previousState.Clip.name + ", Time=" + previousState.Time + ", Weight=" + previousState.Weight);
                    transformsDataing += "Previous State: " + previousStateing.Clip.name + ", Time=" + previousStateing.Time + ", Weight=" + previousStateing.Weight + "\n";
                }
                
                //Debug.LogWarning("Current State: " + state.Clip.name + ", Time=" + state.Time + ", Weight=" + state.Weight);
                transformsDataing += "Current State: " + stateing.Clip.name + ", Time=" + stateing.Time + ", Weight=" + stateing.Weight + "\n\n";

                foreach (var transforming in transformsing) {
                    //Debug.Log(transform + " " + transform.rotation);
                    
                    transformsDataing += transforming.name.Replace("mixamorig:", "") + " " + transforming.localEulerAngles + "\n";
                    transformsDataing += transforming.name.Replace("mixamorig:", "") + " " + transforming.position + "\n";
                }

                transformsDataing += "===================\n\n";
            }

            previousStateing = stateing;
        }

        string path = Application.dataPath + "/Log.txt";
        if (!File.Exists(path)) {
            File.WriteAllText(path, "Transformations Data Log\n");
        }
        string date = "Log Date : " + System.DateTime.Now + "\n\n";
        File.AppendAllText(path, date + transformsDataing);

        _Animancerbody.CrossFade("idle");
    }

    private void printHeader() {
        var bone = _Animancer.Animator.GetBoneTransform(HumanBodyBones.Spine);
        var transforms = bone.GetComponentsInChildren<Transform>();

        // Get Header for CSV
        string header = "kata,time,weight,";
        foreach (var transform in transforms) {
            string bonename = transform.name.Replace("mixamorig:", ""); 
            header += bonename + "_x," + bonename + "_y," + bonename + "_z";
            if (Array.IndexOf(transforms, transform) != transforms.Count() - 1) {
                header += ",";
            }
        }
        header += "\n";

        Debug.Log(header);
    }

    private IEnumerator SibiAnimationSequenceLogRotation(List<string> komponenKata) {
        var bone = _Animancer.Animator.GetBoneTransform(HumanBodyBones.Spine);
        var transforms = bone.GetComponentsInChildren<Transform>();

        AnimancerState previousState = _Animancer.CurrentState;
        AnimancerState state;

        float fadeDuration = 0.25f;

        
        string transformsData = ""; //rotation
        string transformsData2 = ""; //position


        var bonet = _Animancertongue.Animator.GetBoneTransform(HumanBodyBones.Spine);
        var transformst = bonet.GetComponentsInChildren<Transform>();

        AnimancerState previousStatet = _Animancertongue.CurrentState;
        AnimancerState statet;

        
        string transformsDatat = ""; //rotation
        string transformsData2t = ""; //position

        // Get Header for CSV
        string header = "kata,time,weight,";
        foreach (var transform in transforms) {
            string bonename = transform.name.Replace("mixamorig:", ""); 
            header += bonename + "_x," + bonename + "_y," + bonename + "_z";
            if (Array.IndexOf(transforms, transform) != transforms.Count() - 1) {
                header += ",";
            }
        }
        header += "\n";

        // Get Data for Every Frame
        foreach (string kata in komponenKata) {
            if(toggleTransisi.isOn) {
                //Dengan transisi
                state = _Animancer.CrossFadeFromStart(kata, fadeDuration);
            } else {
                //Tanpa transisi
                state = _Animancer.Play(kata);
                state.Time = 0;
            }

            while(state.Time < state.Length) {
                yield return null;

                transformsData += kata + "," + state.Time + "," + state.Weight + ",";
                transformsData2 += kata + "," + state.Time + "," + state.Weight + ",";
                
                foreach (var transform in transforms) {
                    
                    transformsData += transform.localEulerAngles.x + "," + transform.localEulerAngles.y + "," + transform.localEulerAngles.z;
                    transformsData2 += transform.localPosition.x + "," + transform.localPosition.y + "," + transform.localPosition.z;
                    
                    if (Array.IndexOf(transforms, transform) != transforms.Count() - 1) {
                        transformsData += ",";
                        transformsData2 += ",";
                    }
                }
                transformsData += "\n";
                transformsData2 += "\n";
            }

            previousState = state;


             if(toggleTransisi.isOn) {
                //Dengan transisi
                statet = _Animancertongue.CrossFadeFromStart(kata, fadeDuration);
            } else {
                //Tanpa transisi
                statet = _Animancertongue.Play(kata);
                statet.Time = 0;
            }

            while(statet.Time < statet.Length) {
                yield return null;

                transformsDatat += kata + "," + statet.Time + "," + statet.Weight + ",";
                transformsData2t += kata + "," + statet.Time + "," + statet.Weight + ",";
                
                foreach (var transform in transformst) {
                    
                    transformsDatat += transform.localEulerAngles.x + "," + transform.localEulerAngles.y + "," + transform.localEulerAngles.z;
                    transformsData2t += transform.localPosition.x + "," + transform.localPosition.y + "," + transform.localPosition.z;
                    
                    if (Array.IndexOf(transforms, transform) != transformst.Count() - 1) {
                        transformsDatat += ",";
                        transformsData2t += ",";
                    }
                }
                transformsDatat += "\n";
                transformsData2t += "\n";
            }

            previousStatet = statet;


        }

        string path = "";
        string path2 = "";

        if (toggleTransisi.isOn) {
            path = Application.dataPath + "/LogRotation.csv";
            path2 = Application.dataPath + "/LogPosition.csv";
        } else {
            path = Application.dataPath + "/LogRotation_NoTransisi.csv";
            path2 = Application.dataPath + "/LogPosition_NoTransisi.csv";
        }

        if (!File.Exists(path)) {
            File.WriteAllText(path, header + transformsData);
        } else {
            File.AppendAllText(path, transformsData);
        }

        if (!File.Exists(path2)) {
            File.WriteAllText(path2, header + transformsData2);
        } else {
            File.AppendAllText(path2, transformsData2);
        }

        _Animancer.CrossFade("idle");
        _Animancertongue.CrossFade("idle");
    }










    private IEnumerator SibiAnimationSequenceLogRotation2(List<string> komponenKata2) {
        var boneing = _Animancerbody.Animator.GetBoneTransform(HumanBodyBones.Spine);
        var transformsing = boneing.GetComponentsInChildren<Transform>();

        AnimancerState previousStateing = _Animancerbody.CurrentState;
        AnimancerState stateing;

        float fadeDuration = 0.25f;
        
        string transformsDataing = ""; //rotation
        string transformsData2ing = ""; //position

        // Get Header for CSV
        string header = "kata,time,weight,";
        foreach (var transform in transformsing) {
            string bonename = transform.name.Replace("mixamorig:", ""); 
            header += bonename + "_x," + bonename + "_y," + bonename + "_z";
            if (Array.IndexOf(transformsing, transform) != transformsing.Count() - 1) {
                header += ",";
            }
        }
        header += "\n";

        // Get Data for Every Frame
        foreach (string kata in komponenKata2) {            
            if(toggleTransisi.isOn) {
                //Dengan transisi
                stateing = _Animancerbody.CrossFadeFromStart(kata, fadeDuration);
            } else {
                //Tanpa transisi
                stateing = _Animancerbody.Play(kata);
                stateing.Time = 0;
            }

            while(stateing.Time < stateing.Length) {
                yield return null;

                transformsDataing += kata + "," + stateing.Time + "," + stateing.Weight + ",";
                transformsData2ing += kata + "," + stateing.Time + "," + stateing.Weight + ",";
                
                foreach (var transforming in transformsing) {
                    
                    transformsDataing += transforming.localEulerAngles.x + "," + transforming.localEulerAngles.y + "," + transforming.localEulerAngles.z;
                    transformsData2ing += transforming.localPosition.x + "," + transforming.localPosition.y + "," + transforming.localPosition.z;
                    
                    if (Array.IndexOf(transformsing, transforming) != transformsing.Count() - 1) {
                        transformsDataing += ",";
                        transformsData2ing += ",";
                    }
                }
                transformsDataing += "\n";
                transformsData2ing += "\n";
            }

            previousStateing = stateing;
        }

        string path = "";
        string path2 = "";

        if (toggleTransisi.isOn) {
            path = Application.dataPath + "/LogRotation.csv";
            path2 = Application.dataPath + "/LogPosition.csv";
        } else {
            path = Application.dataPath + "/LogRotation_NoTransisi.csv";
            path2 = Application.dataPath + "/LogPosition_NoTransisi.csv";
        }

        if (!File.Exists(path)) {
            File.WriteAllText(path, header + transformsDataing);
        } else {
            File.AppendAllText(path, transformsDataing);
        }

        if (!File.Exists(path2)) {
            File.WriteAllText(path2, header + transformsData2ing);
        } else {
            File.AppendAllText(path2, transformsData2ing);
        }

        _Animancerbody.CrossFade("idle");
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

        string text = "";


        // Fade duration increases as the state speed increases
        // Default is 0.25
        float fadeDuration = 0.25f / sliderSpeed.value;
        float stateSpeed = sliderSpeed.value;

        int idx = 0;
        
        foreach (string kata in komponenKata2) {
            for (int i = 0; i < komponenKata2.Count(); i++) {
                string str = "";
                
                if (komponenKata2.Count() > 1){
                    if (komponenKata2[i].Length > 1){
                        if (i==0){
                            str = char.ToUpper(komponenKata2[i][0]) + komponenKata2[i].Substring(1);
                        }
                        else{
                            str = komponenKata2[i];
                        }
                    }
                    else{
                        str = komponenKata2[i];
                    }
                }
                else{
                    str = komponenKata2[i];
                }

                if (idx == i ){
                    text+=" <color=#955BA5>"+str+"</color>";
                }
                else{
                    text+=" "+str;
                }
            }
            txtKalimat.text = text;

            text = "";

            if(idx==0){
                try
                {
                    using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")){
                        using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity")){
                            obj_Activity.Call("startAnimating", "");
                        }
                    }
                }
                catch (System.Exception)
                {
                    
                    print("no android");
                }
            }

            idx +=1;

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

            txtKalimat.text = "";

            using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")){
                using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity")){
                    obj_Activity.Call("showInput", "");
                }
            }

        } else {
            _Animancerbody.Play("idle");
        }
    }

    public void PlaySequence(List<string> komponenKata)
    {
        StopAllCoroutines();
        // Production Mode
        if(logMode.isOn) {
            // Enable Logging
            StartCoroutine(SibiAnimationSequenceLogRotation(komponenKata));
        } else {
            StartCoroutine(SibiAnimationSequence(komponenKata));
            printHeader();
        }
    }

    public void PlaySequence2(List<string> komponenKata2)
    {
        StopAllCoroutines();
        // Production Mode
        if(logMode.isOn) {
            // Enable Logging
            StartCoroutine(SibiAnimationSequenceLogRotation2(komponenKata2));
        } else {
            StartCoroutine(SibiAnimationSequence2(komponenKata2));
            printHeader();
        }
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

                    foreach(string awalan in tableLookup[t].awalan) {
                        if (awalan != "") {
                            komponenKata.Add(awalan);
                        }
                    }                   

                    foreach(string suku in tableLookup[t].suku) {
                        if (suku != "") {
                            komponenKata.Add(suku);
                        }
                    }

                    foreach (string akhiran in tableLookup[t].akhiran) {
                        if (akhiran != "") {
                            if (akhiran == "i") {
                                komponenKata.Add(akhiran);
                            } else {
                                komponenKata.Add(akhiran);
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

    public List<string> deconstructWord2(string[] token) {
        Dictionary<string, Kata> tableLookup = loadTableLookup();
        List<string> komponenKata =  new List<string>();

        foreach (string t in token) {
            if (tableLookup.ContainsKey(t)) {
                // Cek apakah kata merupakan kata majemuk
                if (isMajemuk(t)) {
                    // 1. Tambah kata dasar 
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

    public List<string> wordToGestureAlfabet(List<string> komponenKata) {
        Dictionary<string, Gesture> gestureLookup = loadGestureLookupAlfabet();
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
