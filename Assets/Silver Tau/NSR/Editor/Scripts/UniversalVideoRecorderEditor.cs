#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SilverTau.NSR.Recorders.Video
{
    [CustomEditor(typeof(UniversalVideoRecorder))]
    public class UniversalVideoRecorderEditor : NSRPackageEditor
    {
        private UniversalVideoRecorder _target;
        
        protected GUIStyle button;
        protected GUIStyle buttonChild;
        protected Color colorChild;
        protected Color colorChildHover;
        protected Color colorMain;
        protected Color colorMainHover;
        
        //private SerializedProperty id;
        //private SerializedProperty isDeselectLocked;
        //private SerializedProperty _proSettingsExpand;
        //private SerializedProperty _changeGameObjectExpand;
        //private SerializedProperty _changeSpriteExpand;
        //private SerializedProperty _changeTextExpand;
        //private SerializedProperty gameObjectComponents;
        //private SerializedProperty spriteComponents;
        //private SerializedProperty textComponents;

        //Common
        private SerializedProperty _cameraSettingExpand;
        private SerializedProperty _videoSettingExpand;
        private SerializedProperty _audioSettingExpand;
        private SerializedProperty _timeSettingExpand;
        private SerializedProperty _advancedSettingExpand;
        private SerializedProperty _infoResolution;
        
        //Camera
        private SerializedProperty _mainCamera;
        private SerializedProperty _cameras;
        
        //Video
        private SerializedProperty _videoFileName;
        private SerializedProperty _layerMasks;
        private SerializedProperty _screenDivideAndroid;
        private SerializedProperty _screenDivideIOS;
        private SerializedProperty _screenDivideStandalone;
        private SerializedProperty _frameRate;
        private SerializedProperty _encodeTo;
        private SerializedProperty _universalVideoRecorderManager;
        
        //Audio
        private SerializedProperty _recordMicrophone;
        private SerializedProperty _recordAllAudioSources;
        private SerializedProperty _audioListener;
        private SerializedProperty _recordOnlyOneAudioSource;
        private SerializedProperty _targetAudioSource;
        
        //Time
        private SerializedProperty _useRealtimeClock;
        private SerializedProperty _textTimer;
        
        //Advanced
        private SerializedProperty _autoPauseResumeRecorder;
        
        public override void Awake()
        {
            base.Awake();
            
            ColorUtility.TryParseHtmlString("#1b7fe3", out colorChild);
            ColorUtility.TryParseHtmlString("#1665b5", out colorChildHover);
            ColorUtility.TryParseHtmlString("#212121", out colorMain);
            ColorUtility.TryParseHtmlString("#1665b5", out colorMainHover);

            button = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fixedHeight = 26.0f,
                richText = true,
                normal = new GUIStyleState
                {
                    background = CreateTexture2D(2, 2, colorMain),
                    textColor = Color.white
                }, 
                hover = 
                {
                    background = CreateTexture2D(2, 2, colorMainHover),
                    textColor = Color.white
                }
            };
            
            buttonChild = new GUIStyle
            {
                padding = new RectOffset(15, 0, 0, 0),
                alignment = TextAnchor.MiddleLeft,
                fixedHeight = 21.0f,
                richText = true,
                normal = new GUIStyleState
                {
                    background = CreateTexture2D(2, 2, colorChild),
                    textColor = Color.white
                }, 
                hover = 
                {
                    background = CreateTexture2D(2, 2, colorChildHover),
                    textColor = Color.white
                }
            };
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
        
        private void OnEnable()
        {
            if (target) _target = (UniversalVideoRecorder)target;
            
            _infoResolution = serializedObject.FindProperty("_infoResolution");
            
            _cameraSettingExpand = serializedObject.FindProperty("_cameraSettingExpand");
            _mainCamera = serializedObject.FindProperty("mainCamera");
            _cameras = serializedObject.FindProperty("cameras");
            
            _videoSettingExpand = serializedObject.FindProperty("_videoSettingExpand");
            _videoFileName = serializedObject.FindProperty("videoFileName");
            _layerMasks = serializedObject.FindProperty("layerMasks");
            _screenDivideAndroid = serializedObject.FindProperty("screenDivideAndroid");
            _screenDivideIOS = serializedObject.FindProperty("screenDivideIOS");
            _screenDivideStandalone = serializedObject.FindProperty("screenDivideStandalone");
            _frameRate = serializedObject.FindProperty("frameRate");
            _encodeTo = serializedObject.FindProperty("encodeTo");
            _universalVideoRecorderManager = serializedObject.FindProperty("universalVideoRecorderManager");
            
            _audioSettingExpand = serializedObject.FindProperty("_audioSettingExpand");
            _recordMicrophone = serializedObject.FindProperty("recordMicrophone");
            _recordAllAudioSources = serializedObject.FindProperty("recordAllAudioSources");
            _audioListener = serializedObject.FindProperty("audioListener");
            _recordOnlyOneAudioSource = serializedObject.FindProperty("recordOnlyOneAudioSource");
            _targetAudioSource = serializedObject.FindProperty("targetAudioSource");
            
            _timeSettingExpand = serializedObject.FindProperty("_timeSettingExpand");
            _useRealtimeClock = serializedObject.FindProperty("useRealtimeClock");
            _textTimer = serializedObject.FindProperty("textTimer");
            
            _advancedSettingExpand = serializedObject.FindProperty("_advancedSettingExpand");
            _autoPauseResumeRecorder = serializedObject.FindProperty("autoPauseResumeRecorder");
            
            serializedObject.ApplyModifiedProperties();
        }
        
        public override void OnInspectorGUI()
        {
            BoxLogo(_target, " <b><color=#ffffff>Universal Video Recorder</color></b>");
            
            //base.OnInspectorGUI();
            
            serializedObject.Update();
            
            EditorGUI.BeginChangeCheck();

            GUILayout.Space(10);
            
            if (GUILayout.Button("<b>Camera Settings</b>", button))
            {
                _cameraSettingExpand.boolValue = !_cameraSettingExpand.boolValue;
            }

            if (_cameraSettingExpand.boolValue)
            {
                GUILayout.Space(10);
                
                EditorGUILayout.PropertyField(_mainCamera);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_cameras);

                GUILayout.Space(10);
            }
            
            GUILayout.Space(10);
            
            // Video
            if (GUILayout.Button("<b>Video Settings</b>", button))
            {
                _videoSettingExpand.boolValue = !_videoSettingExpand.boolValue;
            }

            if (_videoSettingExpand.boolValue)
            {
                GUILayout.Space(10);
                
                EditorGUILayout.PropertyField(_videoFileName);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_layerMasks);
                GUILayout.Space(10);
                
                EditorGUILayout.BeginHorizontal();
                
                GUILayout.Label("<b>Resolution</b>", new GUIStyle() {richText = true, alignment = TextAnchor.MiddleLeft, fontSize = 12, normal = { textColor = Color.white}});

                if(GUILayout.Button("Info", new GUIStyle(GUI.skin.button){ fixedWidth = 50 }))
                {
                    _infoResolution.boolValue = !_infoResolution.boolValue;
                }
                
                EditorGUILayout.EndHorizontal();
                
                if(_infoResolution.boolValue)
                {
                    GUILayout.Space(5);
                    
                    EditorGUILayout.HelpBox("\n" + "You can control the video record resolution depending on the device resolution and using power of 2 screen dividers. These options allow you to divide your current screen resolution by the value you set." 
                                                 + "\n\n" 
                                                 + "We did this so that you can change the video resolution safely and without deforming the video output file."
                                                 + "\n\n" 
                                                 + "For example, the iPhone 13 Pro has a resolution of 1170x2532, setting the \"screenDivideIOS\" parameter to 2, we will get a resolution of 585x1266 and make it a power of 2 - result is 584x1266." 
                                                 + "\n", MessageType.Info);
                }
                
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_screenDivideAndroid);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_screenDivideIOS);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_screenDivideStandalone);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_frameRate);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_encodeTo);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_universalVideoRecorderManager);

                GUILayout.Space(10);
            }
            
            GUILayout.Space(10);
            
            //Audio
            if (GUILayout.Button("<b>Audio Settings</b>", button))
            {
                _audioSettingExpand.boolValue = !_audioSettingExpand.boolValue;
            }

            if (_audioSettingExpand.boolValue)
            {
                GUILayout.Space(10);

                if (_recordMicrophone.boolValue)
                {
#if PLATFORM_IOS
                    GUILayout.Space(5);
                    
                    EditorGUILayout.HelpBox("\n" + "For correct microphone recording on iOS devices, you need to enable the following settings in PlayerSettings -> Player -> Other Settings:" 
                                                 + "\n\n" 
                                                 + "Prepare iOS for Recording - Enable this option to initialize the microphone recording APIs. This lowers recording latency, but it also re-routes iPhone audio output via earphones."
                                                 + "\n\n"
                                                 + "Force iOS Speakers when Recording - Enable this option to send the phone’s audio output through the internal speakers, even when headphones are plugged in and recording."
                                                 + "\n", MessageType.Info);
#endif
                }

                if (_recordMicrophone.boolValue && (_recordAllAudioSources.boolValue || _recordOnlyOneAudioSource.boolValue))
                {
                    GUILayout.Space(5);
                    
                    EditorGUILayout.HelpBox("\n" + "Remember, if you use microphone and audio source recording at the same time, you may get echo when recording the microphone." 
                                                 + "\n\n" 
                                                 + "To solve this, you can use headphones."
                                                 + "\n", MessageType.Info);
                }
                
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_recordMicrophone);
                
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_recordAllAudioSources);
                
                if (_recordAllAudioSources.boolValue)
                {
                    _recordOnlyOneAudioSource.boolValue = false;
                    GUILayout.Space(5);
                    EditorGUILayout.PropertyField(_audioListener);
                }
                
                GUILayout.Space(5);
                
                EditorGUILayout.PropertyField(_recordOnlyOneAudioSource);
                
                if (_recordOnlyOneAudioSource.boolValue)
                {
                    _recordAllAudioSources.boolValue = false;
                    GUILayout.Space(5);
                    EditorGUILayout.PropertyField(_targetAudioSource);
                }
                
                GUILayout.Space(10);
            }
            
            GUILayout.Space(10);
            
            //Time
            if (GUILayout.Button("<b>Time Settings</b>", button))
            {
                _timeSettingExpand.boolValue = !_timeSettingExpand.boolValue;
            }

            if (_timeSettingExpand.boolValue)
            {
                GUILayout.Space(10);
                
                EditorGUILayout.PropertyField(_useRealtimeClock);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_textTimer);
                
                GUILayout.Space(10);
            }
            
            GUILayout.Space(10);
            
            //Advanced
            if (GUILayout.Button("<b>Advanced Settings</b>", button))
            {
                _advancedSettingExpand.boolValue = !_advancedSettingExpand.boolValue;
            }

            if (_advancedSettingExpand.boolValue)
            {
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(_autoPauseResumeRecorder);
                GUILayout.Space(10);
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_target);
                EditorSceneManager.MarkSceneDirty(_target.gameObject.scene);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}

#endif
