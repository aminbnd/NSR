using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SilverTau.NSR.Samples
{
    /// <summary>
    /// Option type.
    /// </summary>
    [Serializable]
    public enum OptionType
    {
        None = 0,
        AudioSources = 1,
        Effects = 2,
        Filters = 4,
        Settings = 8
    }

    /// <summary>
    /// Method of type audio.
    /// </summary>
    [Serializable]
    public class SCAudioSource
    {
        public string id;
        public string title;
        public Sprite sprite;
        public AudioClip audioClip;
    }
    
    /// <summary>
    /// Method of type effect.
    /// </summary>
    [Serializable]
    public class SCEffect
    {
        public string id;
        public string title;
        public Sprite sprite;
        public GameObject prefabEffect;
    }
    
    /// <summary>
    /// Method of type filter.
    /// </summary>
    [Serializable]
    public class SCFilter
    {
        public string id;
        public string title;
        public Sprite sprite;
        public Shader filterShader;
        public Shader additionalFilterShader;
    }
}