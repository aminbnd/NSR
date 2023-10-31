using System;
using System.Collections.Generic;
using SilverTau.NSR.Recorders.Video;
using UnityEditor;
using UnityEngine;

namespace SilverTau.NSR
{
    [CustomEditor(typeof(ScreenRecorder))]
    public class NSREditor : NSRPackageEditor
    {
        private ScreenRecorder _target;
        
        private void OnEnable()
        {
            if (target) _target = (ScreenRecorder)target;
        }
        
        public override void OnInspectorGUI()
        {
            BoxLogo(_target, " <b><color=#ffffff>Screen Recorder</color></b>");
            
            base.OnInspectorGUI();
        }
        private static GameObject CreateElementRoot(string name)
        {
	        var child = new GameObject(name);
	        Undo.RegisterCreatedObjectUndo(child, "Create " + name);
	        Selection.activeGameObject = child;
	        return child;
        }

        static GameObject CreateObject(string name, GameObject parent)
        {
	        var go = new GameObject(name);
	        GameObjectUtility.SetParentAndAlign(go, parent);
	        return go;
        }
        
        [MenuItem("GameObject/Silver Tau/NSR/Screen Recorder (iOS)", false)]
		static public void AddNScreenRecorder()
		{
			var nsr = CreateElementRoot("NSR - Screen Recorder (iOS)");
			nsr.AddComponent<ScreenRecorder>();
		}
        
		[MenuItem("GameObject/Silver Tau/NSR/Universal Screen Recorder", false)]
		static public void AddNUniversalScreenRecorder()
		{
			var nsr = CreateElementRoot("NSR - Screen Recorder (Universal)");
			nsr.AddComponent<UniversalVideoRecorder>();
		}
		
		[MenuItem("Window/Silver Tau/NSR - Screen Recorder/Prepare and check the Screen Recorder (iOS)")]
		public static void PrepareAndCheckNSRScreenRecorderiOS()
		{
			var screenRecorder = UnityEngine.Object.FindObjectOfType<ScreenRecorder>();
			if (screenRecorder == null)
			{
				AddNScreenRecorder();
				screenRecorder = UnityEngine.Object.FindObjectOfType<ScreenRecorder>();
			}
			
			if (screenRecorder != null) return;
			Debug.Log("<color=cyan>NSR - Screen Recorder - Screen Recorder (iOS) is ready to use.</color>");
		}

		[MenuItem("Window/Silver Tau/NSR - Screen Recorder/Prepare and check the Universal Video Recorder")]
		public static void PrepareAndCheckNSRScreenRecorder()
		{
			var universalVideoRecorder = UnityEngine.Object.FindObjectOfType<UniversalVideoRecorder>();
			if (universalVideoRecorder == null)
			{
				AddNUniversalScreenRecorder();
				universalVideoRecorder = UnityEngine.Object.FindObjectOfType<UniversalVideoRecorder>();
			}
			
			if (universalVideoRecorder != null) return;
			Debug.Log("<color=cyan>NSR - Screen Recorder - Universal Video Recorder is ready to use.</color>");
		}
        
		[MenuItem("Window/Silver Tau/NSR - Screen Recorder/Validate plugin", false, 1)]
		public static void ValidateNSRScreenRecorder()
		{
			var hasWarning = false;
			
			var universalVideoRecorder = UnityEngine.Object.FindObjectsOfType<UniversalVideoRecorder>();

			if (universalVideoRecorder.Length == 0)
			{
				Debug.LogWarning("There is no Universal Video Recorder component on the scene opened in the editor.");
				
				var screenRecorder = UnityEngine.Object.FindObjectsOfType<ScreenRecorder>();
				
				if (screenRecorder.Length == 0)
				{
					Debug.LogWarning("There is no Screen Recorder (iOS) component on the scene opened in the editor.");
					hasWarning = true;
				}
				else if (screenRecorder.Length > 1)
				{
					Debug.LogWarning("There is more than one Screen Recorder (iOS) component in the scene open in the editor. There should be only one Screen Recorder (iOS) component on the scene.");
					hasWarning = true;
				}
				else
				{
					hasWarning = true;
				}
			}
			else if (universalVideoRecorder.Length > 1)
			{
				Debug.LogWarning("There is more than one Universal Video Recorder component in the scene open in the editor. There should be only one Universal Video Recorder component on the scene.");
				hasWarning = true;
			}
			
			var rpuAssets = AssetDatabase.FindAssets("StcCorder");

			var files = new List<string> {"StcCorder.framework", "StcCorder.dll", "StcCorder.bundle", "StcCorder.aar", "NSR.framework"};
			
			foreach (var asset in rpuAssets)
			{
				var path = AssetDatabase.GUIDToAssetPath(asset);
				
				CheckAssetInProject(path, files, "NSR.framework", ref hasWarning);
				CheckAssetInProject(path, files, "StcCorder.framework", ref hasWarning);
				CheckAssetInProject(path, files, "StcCorder.dll", ref hasWarning);
				CheckAssetInProject(path, files, "StcCorder.bundle", ref hasWarning);
				CheckAssetInProject(path, files, "StcCorder.aar", ref hasWarning);
			}
			
#if PLATFORM_IOS
			if (!ValidateTargetOsVersion())
			{
				Debug.LogWarning("The minimum recommended OS version should be 12+.");
				hasWarning = true;
			}
#endif
			if(!hasWarning) Debug.Log("<color=cyan>NSR - Screen Recorder is ready to use.</color>");
		}
		
#if PLATFORM_IOS
	    private static bool ValidateTargetOsVersion()
	    {
		    Version ver;
            
		    try
		    {
			    ver = Version.Parse(PlayerSettings.iOS.targetOSVersionString);
		    }
		    catch (Exception e)
		    {
			    Debug.Log(e);
			    return false;
		    }
            
		    return ver.Major >= 12;
	    }
#endif

	    private static void CheckAssetInProject(string path, ICollection<string> files, string assetName, ref bool hasWarning)
	    {
		    if (!path.Contains(assetName)) return;
		    
		    if (files.Contains(assetName))
		    {
			    files.Remove(assetName);
		    }
		    else
		    {
			    Debug.LogWarning("More than one " +assetName +" was found in the project. There should be only one " + assetName + ".\nPath: " + path);
			    hasWarning = true;
		    }
	    }
	    
		[MenuItem("Window/Silver Tau/NSR - Screen Recorder/Documentation (Web)", false, 3)]
		public static void DocumentationWebNSRScreenRecorder()
		{
			Application.OpenURL("https://silvertau.s3.eu-central-1.amazonaws.com/NSR-ScreenRecorder/Documentation/index.html");
		}
	    
		[MenuItem("Window/Silver Tau/NSR - Screen Recorder/Product Page", false, 2)]
		public static void ProductPageNSRScreenRecorder()
		{
			Application.OpenURL("https://www.silvertau.com/products/nsr-screen-recorder");
		}

		[MenuItem("Window/Silver Tau/NSR - Screen Recorder/Unity Asset Store Page", false, 3)]
		public static void UnityAssetStorePageWebNSRScreenRecorder()
		{
			Application.OpenURL("https://prf.hn/l/q5yDAXe");
		}
    }
}