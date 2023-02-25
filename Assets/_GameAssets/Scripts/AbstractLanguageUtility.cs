using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FasilkomUI
{
    public static class AbstractLanguageUtility
    {
        /**
         * <summary>
         * Buat lookup JSON khusus untuk Slang, Kata & Gesture
         * SKG Singkatan dari Slang, Kata, Gesture
         * Ada urutan SKG karena Raw text -> Slang -> Kata (formal) -> Gesture (Animasi)
         * T : GKS Dictionary contohnya Gesture Dictionary
         * U : GKS contohnya Gesture
         * </summary>
         */
        public static Dictionary<string, U> LoadSKGLookup<T, U>(string jsonData) where T : AbstractSKGDictionary<U> where U : AbstractSKG
        {
            T tempList = JsonUtility.FromJson<T>(jsonData);
            Dictionary<string, U> tableLookup = new Dictionary<string, U>();
            foreach (U skg in tempList.listSKG)
            {
                if(skg.GetType() == typeof(Kata))
                {
                    (skg as Kata).awalans = (skg as Kata).awalan.Split(';');
                    (skg as Kata).akhirans = (skg as Kata).akhiran.Split(';');
                    (skg as Kata).sukus = (skg as Kata).suku.Split(';');
                }

                tableLookup.Add(skg.id, skg);
            }

            return tableLookup;
        }

        /**
         * <summary>
         * Tokenization is essentially splitting a phrase, sentence, paragraph, or an entire text document into smaller units
         * , such as individual words or terms. Each of these smaller units are called tokens.
         * </summary>
         */
        public static string[] TokenizeText(string input)
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
         * Cek apakah ini kata majemuk atau bukan
         * Ex. Majemuk : orang-orang
         * </summary>
         */
        public static bool IsMajemuk(string word)
        {
            return Regex.IsMatch(word, @"[-]");
        }

        /**
         * <summary>
         * Kalau kata ini ga ada di database, cek ini kata adalah nomor, time format, atau desimal?
         * Kalo bukan semuanya, jadi pecahan kata kayak s u d i r m a n
         * </summary>
         */
        public static string[] AbjadChecker(string word)
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
                        return SplitString(word);
                    }
                    else
                    {
                        return NumberToDigit(word);
                    }
                }
                else
                {
                    return SplitString(word);
                }
            }
            else if (isTimeFormat)
            {
                Match match = Regex.Match(word, @"(?:[01][0-9]|2[0-3]|[1-9])[.][0-5][0-9]");
                if (match.Success)
                {
                    if (match.Value == word)
                    {
                        return NumberToTime(word);
                    }
                    else
                    {
                        string newNumber = word.Replace(".", "");
                        return NumberToDigit(newNumber);
                    }
                }
                else
                {
                    return NumberToTime(word);
                }
            }
            else if (isOtherNum)
            {
                string newNumber = word.Replace(".", "");
                return NumberToDigit(newNumber);
            }
            else
            {
                return SplitString(word);
            }
        }

        // bawah ini private aja?
        public static string[] NumberToDigit(string number) 
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
                str = Terbilang(Int32.Parse(number));
            }

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

        public static string[] NumberToTime(string number)
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
                str1 = Terbilang(Int32.Parse(tnToken[0]));
            }

            if (Int32.Parse(tnToken[2]) == 0)
            {
                str2 = "0";
            }
            else
            {
                str2 = Terbilang(Int32.Parse(tnToken[2]));
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

        /**
         * <summary>
         * Ubah jadi per karakter
         * Ex: sudirman --> s u d i r m a n
         * --> kalo angka & huruf jadinya ngaco (ex 5cl jadinya 5 c l, seharusnya lima c l)
         * </summary>
         */
        public static string[] SplitString(string word)
        {
            string[] words = Regex.Split(word, string.Empty);
            words = words.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return words;
        }

        public static string Terbilang(int num)
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
                strDigit = Terbilang(num / 10) + " puluh " + Terbilang(num % 10);
            }
            else if (num < 200)
            {
                strDigit = " ratus " + Terbilang(num - 100);
            }
            else if (num < 1000)
            {
                strDigit = Terbilang(num / 100) + " ratus " + Terbilang(num % 100);
            }
            else if (num < 2000) // kalo diatas seribu pakenya fungsi splitstring???
            {
                strDigit = " ribu   " + Terbilang(num - 1000);
            }
            else if (num < 1000000)
            {
                strDigit = Terbilang(num / 1000) + " ribu " + Terbilang(num % 1000);
            }
            else if (num < 1000000000)
            {
                strDigit = Terbilang(num / 1000000) + " juta " + Terbilang(num % 1000000);
            }

            strDigit = Regex.Replace(strDigit, @"^\s+|\s+$", " ");

            return strDigit;
        }
    }
}