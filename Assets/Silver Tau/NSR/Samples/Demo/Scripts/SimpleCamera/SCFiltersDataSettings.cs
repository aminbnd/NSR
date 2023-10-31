using System.Collections.Generic;
using UnityEngine;

namespace SilverTau.NSR.Samples
{
    [CreateAssetMenu(fileName = "Simple Camera - Filters Data Settings", menuName = "Silver Tau/Simple Camera/Filters Data Settings", order = 1)]
    public class SCFiltersDataSettings : ScriptableObject
    {
        public List<SCFilter> filters = new List<SCFilter>();
    }
}

