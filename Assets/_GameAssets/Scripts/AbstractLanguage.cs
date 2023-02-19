using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FasilkomUI
{
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
    }
}