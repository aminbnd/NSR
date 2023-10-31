using System.Collections.Generic;
using UnityEngine;

namespace SilverTau.NSR.Samples
{
    [CreateAssetMenu(fileName = "Simple Camera - Effects Data Settings", menuName = "Silver Tau/Simple Camera/Effects Data Settings", order = 1)]
    public class SCEffectsDataSettings : ScriptableObject
    {
        public List<SCEffect> effects = new List<SCEffect>();
    }
}

