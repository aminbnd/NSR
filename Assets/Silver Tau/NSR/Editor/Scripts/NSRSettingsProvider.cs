using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEngine;

namespace SilverTau.iOS 
{
#if UNITY_EDITOR
	
    using UnityEditor;
    using UnityEditor.Callbacks;
    using System.IO;

    #if UNITY_IOS
    using UnityEditor.iOS.Xcode;
    #endif

    public class NSRSettingsProvider : SettingsProvider
    {
#if UNITY_IOS
	    private static string PhotoLibraryAddUsageKey = @"NSPhotoLibraryAddUsageDescription";
	    private static string PhotoLibraryAddUsageDescription = @"Allow this app to save videos to your photo library.";
	    private static string PhotoLibraryUsageKey = @"NSPhotoLibraryUsageDescription";
	    private static string PhotoLibraryUsageDescription = @"Allow this app to save videos to your photo library.";
#endif
	    private static string kCustomInterpolatorHelpBox = "If the input field is empty, the parameter is not added to Info.plist.";
	    private static string kCustomInterpolatorDocumentationURL = "https://silvertau.com";
	    
	    protected static Texture2D Icon;
	    protected static Texture2D banner;
	    protected static Texture2D Background;
	    protected GUIStyle BackgroundStyle;
	    protected GUIStyle SeparationLineStyle;
	    protected GUIStyle LogoStyle;
	    
	    private class Styles
	    {
		    public static readonly GUIContent CustomInterpLabel = L10n.TextContent("Permissions: ", "");
		    public static readonly GUIContent CustomInterpPhotoLibraryAddUsageDescriptionLabel = L10n.TextContent("Photo Library Add Usage Description", $"A message that tells the user why the app is requesting add-only access to the user’s photo library.");
		    public static readonly GUIContent CustomInterpPhotoLibraryUsageDescriptionLabel = L10n.TextContent("Photo Library Usage Description", $"A message that tells the user why the app is requesting access to the user’s photo library. \n\nIf your app only adds assets to the photo library and does not read assets, use the PhotoLibraryAddUsageDescription key instead.");
		    public static readonly GUIContent ReadMore = L10n.IconContent(banner, "Visit the developer's website.");
	    }
	    
	    public NSRSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
	    {
		    Icon = EditorGUIUtility.Load("Packages/com.silvertau.nativescreenrecorder/Editor/Images/icon_settings.png") as Texture2D;
		    if (Icon == null)
		    {
			    Icon = EditorGUIUtility.Load("Assets/Silver Tau/NSR/Editor/Images/icon_settings.png") as Texture2D;
		    }
		    
		    banner = EditorGUIUtility.Load("Packages/com.silvertau.nativescreenrecorder/Editor/Images/banner.png") as Texture2D;
		    if (banner == null)
		    {
			    banner = EditorGUIUtility.Load("Assets/Silver Tau/NSR/Editor/Images/banner.png") as Texture2D;
		    }
		    
		    Background = CreateTexture2D(2, 2, new Color(0.0f, 0.0f, 0.0f, 0.5f));
            
		    BackgroundStyle = new GUIStyle
		    {
			    fixedHeight = 64.0f,
			    stretchWidth = true,
			    normal = new GUIStyleState
			    {
				    background = Background
			    }
		    };
		    
		    SeparationLineStyle = new GUIStyle
		    {
			    fixedHeight = 2.5f,
			    stretchWidth = true,
			    normal = new GUIStyleState
			    {
				    background = Background
			    }
		    };
		    
		    LogoStyle = new GUIStyle
		    {
			    alignment = TextAnchor.MiddleLeft,
			    stretchWidth = true,
			    stretchHeight = true,
			    fixedHeight = 64.0f,
			    fontSize = 21,
			    richText = true
		    };
		    
		    guiHandler = OnGUIHandler;
	    }

	    private Texture2D CreateTexture2D(int width, int height, Color col)
	    {
		    var pix = new Color[width * height];
		    for(var i = 0; i < pix.Length; ++i)
		    {
			    pix[i] = col;
		    }
		    var result = new Texture2D(width, height);
		    result.SetPixels(pix);
		    result.Apply();
		    return result;
	    }

	    void OnGUIHandler(string searchContext)
	    {
		    EditorGUI.BeginChangeCheck();
		    GUILayout.BeginHorizontal(BackgroundStyle);
		    GUILayout.Label(Icon, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true));
		    GUILayout.Label("<b><color=#1b7fe3>Settings</color></b>", LogoStyle);
		    
		    GUILayout.EndHorizontal();
		    
		    EditorGUILayout.LabelField(Styles.CustomInterpLabel, EditorStyles.boldLabel);
		    EditorGUI.indentLevel++;
#if UNITY_IOS
		    var photoLibraryAddUsageDescription = EditorGUILayout.TextField(Styles.CustomInterpPhotoLibraryAddUsageDescriptionLabel, PhotoLibraryAddUsageDescription);
		    var photoLibraryUsageDescription = EditorGUILayout.TextField(Styles.CustomInterpPhotoLibraryUsageDescriptionLabel, PhotoLibraryUsageDescription);
#else
		    GUILayout.Label("<b><color=#ffffff>To enable the feature, switch the platform to iOS.</color></b>", LogoStyle);
#endif
		    EditorGUILayout.Space();
		    
		    GUILayout.BeginHorizontal(EditorStyles.helpBox);
		    
		    GUILayout.Label(EditorGUIUtility.IconContent("console.infoicon"), GUILayout.ExpandWidth(false));
		    GUILayout.Box(kCustomInterpolatorHelpBox, EditorStyles.wordWrappedLabel);
		    
		    GUILayout.EndHorizontal();
		    
		    EditorGUILayout.Space();
		    
		    GUILayout.BeginHorizontal(SeparationLineStyle);
		    GUILayout.EndHorizontal();
		    
		    EditorGUILayout.Space(10);
		    
		    GUILayout.BeginHorizontal();
		    
		    if (GUILayout.Button(Styles.ReadMore, GUIStyle.none, GUILayout.ExpandWidth(false)))
		    {
			    System.Diagnostics.Process.Start(kCustomInterpolatorDocumentationURL);
		    }
		    
		    GUILayout.EndHorizontal();
		    
		    EditorGUI.indentLevel--;
		    
		    if (EditorGUI.EndChangeCheck())
		    {
#if UNITY_IOS
			    ApplyChanges(photoLibraryAddUsageDescription, photoLibraryUsageDescription);
#endif
		    }
	    }

	    private void ApplyChanges(string photoLibraryAddUsageDescription, string photoLibraryUsageDescription)
	    {
#if UNITY_IOS
		    PhotoLibraryAddUsageDescription = photoLibraryAddUsageDescription;
		    PhotoLibraryUsageDescription = photoLibraryUsageDescription;
#endif
	    }
	    
        #if UNITY_IOS

		[PostProcessBuild]
		static void SetPermissions (BuildTarget buildTarget, string path) {
			if (buildTarget != BuildTarget.iOS) return;
			if(string.IsNullOrEmpty(PhotoLibraryAddUsageDescription) && string.IsNullOrEmpty(PhotoLibraryUsageDescription)) return;
			
			var plistPath = path + "/Info.plist";
			var plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));
			var rootDictionary = plist.root;
			
			if(!string.IsNullOrEmpty(PhotoLibraryAddUsageDescription)) rootDictionary.SetString(PhotoLibraryAddUsageKey, PhotoLibraryAddUsageDescription);
			if(!string.IsNullOrEmpty(PhotoLibraryUsageDescription)) rootDictionary.SetString(PhotoLibraryUsageKey, PhotoLibraryUsageDescription);
			
			File.WriteAllText(plistPath, plist.WriteToString());
		}
		#endif
	    
	    
	    [SettingsProvider]
	    public static SettingsProvider NSRSettingsProviderProjectSettingsProvider()
	    {
		    var provider = new NSRSettingsProvider("Project/Silver Tau/NSR - Screen Recorder", SettingsScope.Project);
		    return provider;
	    }
    }

#endif
}
#pragma warning restore 0162, 0429