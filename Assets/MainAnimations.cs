using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

public sealed class MainAnimations : MonoBehaviour
{
    [SerializeField] private NamedAnimancerComponent _Animancer;

    public void PlayNama()
    {
        _Animancer.CrossFade("nama");
    }
}
