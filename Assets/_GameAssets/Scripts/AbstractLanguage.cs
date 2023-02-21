using Animancer;
using System.Collections;
using System.Collections.Generic;
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

        protected IEnumerator _AnimationSequence(NamedAnimancerComponent[] animancers, List<string> komponenKata, bool sendToUI = false)
        {
            float fadeDuration = 0.25f;
            float noGestureAnimationWait = 1.0f;
            int idx = 0;

            AnimancerState state = animancers[0].States.Current;

            foreach (string kata in komponenKata)
            {
                if (sendToUI)
                {
                    UITextProcessing.Instance.SendTextResultToUI(idx, komponenKata);
                    idx += 1;
                }

                _PlayAllAnimancer(animancers, kata, fadeDuration, out state);
                if (state != null)
                {
                    while (state.Time < state.Length)
                    {
                        yield return null;
                    }
                }
                else
                {
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
                state = animancer.TryPlay(key, fadeDuration);
            }
        }
    }
}