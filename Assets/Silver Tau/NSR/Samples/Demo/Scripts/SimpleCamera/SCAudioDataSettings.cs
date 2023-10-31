using System.Collections.Generic;
using UnityEngine;

namespace SilverTau.NSR.Samples
{
    [CreateAssetMenu(fileName = "Simple Camera - Audio Data Settings", menuName = "Silver Tau/Simple Camera/Audio Data Settings", order = 1)]
    public class SCAudioDataSettings : ScriptableObject
    {
        public List<SCAudioSource> audioSources = new List<SCAudioSource>();
    }
}

