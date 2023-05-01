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
        public static Dictionary<string, U> LoadDatabaseLookup<T, U>(string jsonData) where T : AbstractDatabaseDictionary<U> where U : AbstractDatabase
        {
            T tempList = JsonUtility.FromJson<T>(jsonData);
            Dictionary<string, U> tableLookup = new Dictionary<string, U>();
            foreach (U skg in tempList.list)
            {
                if(tableLookup.ContainsKey(skg.id))
                {
                    Debug.LogWarning("Loading " + skg.GetType() + " : " + skg.id + " is duplicated & has already been added, skipping... ");
                    continue;
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
        public static bool CheckContainStrip(string word)
        {
            return Regex.IsMatch(word, @"[-]");
        }

        /**
         * <summary>
         * Cek apakah ini kata majemuk atau bukan
         * Ex. Majemuk : orang-orang
         * </summary>
         */
        public static bool CheckNeedToSplitNumeric(string word)
        {
            return Regex.IsMatch(word, @"^[0-9]+$");
        }

        /**
         * <summary>
         * Cek apakah ini kata majemuk atau bukan
         * Ex. Majemuk : orang-orang
         * </summary>
         */
        public static bool CheckIsTimeFormat(string word)
        {
            return Regex.IsMatch(word, @"(?:[01][0-9]|2[0-3]|[1-9])[.][0-5][0-9]");
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
    }
}