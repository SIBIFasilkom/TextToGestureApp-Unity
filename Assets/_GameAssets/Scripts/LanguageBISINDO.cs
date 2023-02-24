using Animancer;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FasilkomUI.BISINDO
{
    public class LanguageBISINDO : AbstractLanguage
    {
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
            List<Gesture> komponenKata2 = _DeconstructWordForBody(rawToken);

            if (m_animancerHeadTongueCoroutine != null) StopCoroutine(m_animancerHeadTongueCoroutine);
            if (m_animancerBodyCoroutine != null) StopCoroutine(m_animancerBodyCoroutine);
            m_animancerBodyCoroutine = StartCoroutine(_AnimationSequence(new NamedAnimancerComponent[] { m_animancerBody }, komponenKata2, true));
        }
    }
}
