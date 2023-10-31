#if UNITY_EDITOR

using UnityEditor;

namespace SilverTau.NSR.Recorders.Video
{
    [CustomEditor(typeof(UniversalVideoRecorderManager))]
    public class UniversalVideoRecorderManagerEditor : NSRPackageEditor
    {
        private UniversalVideoRecorderManager _target;
        
        private void OnEnable()
        {
            if (target) _target = (UniversalVideoRecorderManager)target;
        }
        
        public override void OnInspectorGUI()
        {
            BoxLogo(_target, " <b><color=#ffffff>Universal Video Recorder Manager</color></b>");
            base.OnInspectorGUI();
        }
    }
}

#endif
