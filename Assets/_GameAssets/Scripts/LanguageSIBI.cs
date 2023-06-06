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
    public class SIBI : AbstractDatabaseLanguage
    {
        public string detail;
        public string category;
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
        Dictionary<string, Imbuhan_SIBI>[] m_table_imbuhan_sibi;

        [SerializeField] bool m_debugAnimationList = false;

        #region Unity Callbacks
        protected override void Awake()
        {
            base.Awake();

            m_table_sibi = AbstractLanguageUtility.LoadDatabaseLookup<SIBIDictionary, SIBI>(m_data_languageLookup.ToString(), m_debugDuplicate);
            m_table_alt_sibi = AbstractLanguageUtility.LoadDatabaseLookup<AltSIBIDictionary, Alt_SIBI>(m_data_alt_languageLookup.ToString(), m_debugDuplicate);

            m_table_imbuhan_sibi = new Dictionary<string, Imbuhan_SIBI>[m_data_imbuhan_languageLookup.Length];
            for (int i = 0; i < m_data_imbuhan_languageLookup.Length; i++)
            {
                m_table_imbuhan_sibi[i] = AbstractLanguageUtility.LoadDatabaseLookup<ImbuhanSIBIDictionary, Imbuhan_SIBI>(m_data_imbuhan_languageLookup[i].ToString(), m_debugDuplicate);
            }

#if UNITY_EDITOR
            if (m_debugAnimationList)
            {
                string outputPath = Application.dataPath + "/_GameAssets/Animation List.txt";
                if (m_animancerBody == null)
                    return;

                string content = "";
                foreach (var state in m_animancerBody.States)
                {
                    content += state.Key.ToString() + Environment.NewLine;
                }

                Debug.Log("Animation list copied to clipboard :\n" + content);
                GUIUtility.systemCopyBuffer = content;
            }
#endif

            UITextProcessing.Instance.InitializeUIDictionaryDatabase(m_table_sibi);
        }
        #endregion

        public override void ConvertToAnimationFromToken(string[] rawTokens)
        {
            List<SIBI> sibiList = new List<SIBI>();
            foreach (string rawToken in rawTokens)
            {
                _SearchKeyFromTable(sibiList, rawToken);
            }

            if (m_animancerCoroutine != null) StopCoroutine(m_animancerCoroutine);
            m_animancerCoroutine = StartCoroutine(_AnimationSequence(sibiList));
        }

        public override string GetHowToLanguage(string key)
        {
            bool isExist = CheckAnimationExist(key);
            string existMsg = (!isExist) ? "\n\n<color=red>*Mohon maaf, saat ini animasi untuk kata ini tidak tersedia.</color>" : "";

            return m_table_sibi[key].detail + existMsg;
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

            for (int i = 0; i < m_table_imbuhan_sibi.Length; i++)
            {
                if (m_table_imbuhan_sibi[i].ContainsKey(rawToken))
                {
                    var awalans = m_table_imbuhan_sibi[i][rawToken].sibi_id_awalan.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var awalan in awalans)
                        _SearchKeyFromTable(sibiList, awalan);

                    _SearchKeyFromTable(sibiList, m_table_imbuhan_sibi[i][rawToken].sibi_id);

                    var akhirans = m_table_imbuhan_sibi[i][rawToken].sibi_id_akhiran.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var akhiran in akhirans)
                        _SearchKeyFromTable(sibiList, akhiran);

                    return;
                }
            }

            if (m_table_slang.ContainsKey(rawToken))
            {
                _SearchKeyFromTable(sibiList, m_table_slang[rawToken].formal);
                return;
            }

            if (AbstractLanguageUtility.CheckContainStrip(rawToken))
            {
                var majemukTokens = rawToken.Split('-', StringSplitOptions.RemoveEmptyEntries);
                foreach (var majemukToken in majemukTokens)
                    _SearchKeyFromTable(sibiList, majemukToken);

                return;
            }

            // p.s : baru di handle sampe juta doang, belum milyar
            if (AbstractLanguageUtility.CheckNeedToSplitNumeric(rawToken))
            {
                int parsedToken = Mathf.Abs(int.Parse(rawToken));

                if (parsedToken >= 1000000)
                {
                    _SearchKeyFromTable(sibiList, "" + (parsedToken / 1000000));
                    _SearchKeyFromTable(sibiList, "-juta");
                    if (parsedToken % 1000000 > 0)
                        _SearchKeyFromTable(sibiList, "" + (parsedToken % 1000000));
                }

                if (parsedToken >= 1000 && parsedToken < 1000000)
                {
                    _SearchKeyFromTable(sibiList, "" + (parsedToken / 1000));
                    _SearchKeyFromTable(sibiList, "-ribu");
                    if (parsedToken % 1000 > 0)
                        _SearchKeyFromTable(sibiList, "" + (parsedToken % 1000));
                }

                if (parsedToken >= 100 && parsedToken < 1000)
                {
                    _SearchKeyFromTable(sibiList, "" + (parsedToken / 100));
                    _SearchKeyFromTable(sibiList, "-ratus");
                    if (parsedToken % 100 > 0)
                        _SearchKeyFromTable(sibiList, "" + (parsedToken % 100));
                }

                if (parsedToken >= 20 && parsedToken < 100)
                {
                    _SearchKeyFromTable(sibiList, "" + (parsedToken / 10));
                    _SearchKeyFromTable(sibiList, "-puluh");
                    if (parsedToken % 10 > 0)
                        _SearchKeyFromTable(sibiList, "" + (parsedToken % 10));
                }
                return;
            }

            if (AbstractLanguageUtility.CheckIsTimeFormat(rawToken))
            {
                var timeTokens = rawToken.Split('.', StringSplitOptions.RemoveEmptyEntries);
                if (timeTokens.Length == 2)
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
                foreach (string tokenSplit in tokenSplits)
                    _SearchKeyFromTable(sibiList, tokenSplit);

                return;
            }

            Debug.LogWarning("Unable to process this token : " + rawToken);
        }
    }
}