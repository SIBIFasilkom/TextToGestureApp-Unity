using Animancer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FasilkomUI
{
    [System.Serializable]
    public class Gesture
    {
        public string id;
        public string anim;

        public Gesture(string id, string anim = null)
        {
            this.id = id;
            this.anim = anim;
        }
    }

    [System.Serializable]
    public class GestureDictionary
    {
        public List<Gesture> listGesture;
    }

    public abstract class AbstractLanguage : MonoBehaviour
    {
        public abstract void ConvertToAnimationFromToken(string[] rawToken);

        public abstract void ChangeModel(bool isAndi);

        /**
         * <summary>
         * Tokenization is essentially splitting a phrase, sentence, paragraph, or an entire text document into smaller units
         * , such as individual words or terms. Each of these smaller units are called tokens.
         * </summary>
         */
        public string[] TokenizeText(string input)
        {
            string[] rawToken = Regex.Split(input, @"[\s\\?]");
            rawToken = rawToken
                        .Select(x => x.ToLowerInvariant())
                        .Where(x => !string.IsNullOrEmpty(x))
                        .ToArray();
            return rawToken;
        }

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

        protected bool _IsMajemuk(string word)
        {
            return Regex.IsMatch(word, @"[-]");
        }

        protected string[] _AbjadChecker(string word)
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
    }
}