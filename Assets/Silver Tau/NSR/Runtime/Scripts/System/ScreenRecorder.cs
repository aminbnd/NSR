using UnityEngine;
using System;
using AOT;
using UnityEngine.Events;

#if PLATFORM_IOS
using System.Runtime.InteropServices;
#endif

namespace SilverTau.NSR
{
    
    /// <summary>
    /// Enum which is used to find out the current state of the recorder.
    /// </summary>
    [Serializable]
    public enum RecorderStatus
    {
        None = 0,
        Init = 1,
        Recording = 2,
        Stopped = 4
    }
    
    public class ScreenRecorder : MonoBehaviour
    {
        public static ScreenRecorder Instance { get; set; }

        #region MonoBehaviour

        private void Awake()
        {
            Instance = this;
        }
        
        private void Start()
        {
            ChangeRecorderStatus(RecorderStatus.None);
            if(useDelayInitialization) return;
            
#if !UNITY_EDITOR && PLATFORM_IOS
            UpdateSettings(useMicrophone, usePopoverPresentation, useSaveVideoToPhotosAfterRecord);
            Initialize();
#endif
        }

        #endregion

#if PLATFORM_IOS

        #region NativeScreenRecorder main

        [DllImport("__Internal")]
        private static extern void initialize();
        
        [DllImport("__Internal")]
        private static extern void dispose();
        
        #endregion

        #region Video Recorder

        [DllImport("__Internal")]
        private static extern void startScreenRecorder();
        
        [DllImport("__Internal")]
        private static extern void stopScreenRecorder();
        
        [DllImport("__Internal")]
        private static extern bool isRecording();
        
        [DllImport("__Internal")]
        private static extern bool isAvailable();
        
        [DllImport("__Internal")]
        private static extern void isMicrophoneEnabled(bool value);
        
        [DllImport("__Internal")]
        private static extern void isPopoverPresentationEnabled(bool value);
        
        [DllImport("__Internal")]
        private static extern void isSaveVideoToPhotosAfterRecord(bool value);

        [DllImport("__Internal")]
        private static extern string getVideoOutputPath();
        
        #endregion

        #region Get

        [DllImport("__Internal")]
        private static extern string getVersion();
        
        #endregion
        
        #region Share

        [DllImport("__Internal")]
        private static extern bool share(String text);
        
        #endregion
        
        #region Save

        [DllImport("__Internal")]
        private static extern bool saveVideoToPhotosAlbum(String text);
        
        #endregion
        
        #region Delegates

        public delegate void DidScreenRecorderStartDelegate();
        [DllImport("__Internal")]
        private static extern void didScreenRecorderStart(DidScreenRecorderStartDelegate callBack);
        
        public delegate void DidScreenRecorderStopDelegate();
        [DllImport("__Internal")]
        private static extern void didScreenRecorderStop(DidScreenRecorderStopDelegate callBack);
        
        public delegate void DidShareDelegate();
        [DllImport("__Internal")]
        private static extern void didShare(DidShareDelegate callBack);
        
        public delegate void DidScreenRecorderErrorDelegate(String value);
        [DllImport("__Internal")]
        private static extern void didScreenRecorderError(DidScreenRecorderErrorDelegate callBack);
        
        public delegate void DidShareErrorDelegate(String value);
        [DllImport("__Internal")]
        private static extern void didShareError(DidShareErrorDelegate callBack);
        
        #endregion

#endif
        /// <summary>
        /// A parameter that returns the path of the output video file.
        /// </summary>
        public static string videoOutputPath => CheckVideoOutputPath();
        
        private static string CheckVideoOutputPath()
        {
#if !UNITY_EDITOR && PLATFORM_IOS
            var result = getVideoOutputPath();
            Debug.Log($"NSR Output Path = {result}.");
            return result;
#else
            Debug.Log("NSR Output Path -> Empty. Unsupported platform.");
            return string.Empty;
#endif
        }
        
        /// <summary>
        /// A parameter that checks whether the video recording is in the recording state.
        /// </summary>
        public static bool IsRecording => CheckIsRecording();
        
        private static bool CheckIsRecording()
        {
#if !UNITY_EDITOR && PLATFORM_IOS
            var result = isRecording();
            Debug.Log($"NSR is recording ->" + result);
            return result;
#else
            Debug.Log("NSR is recording -> false. Unsupported platform.");
            return false;
#endif
        }
        
        /// <summary>
        /// A parameter that checks whether it is possible to record video on this device.
        /// </summary>
        public static bool IsAvailable => CheckIsAvailable();
        
        private static bool CheckIsAvailable()
        {
#if !UNITY_EDITOR && PLATFORM_IOS
            var result = isAvailable();
            Debug.Log($"NSR is available ->" + result);
            return result;
#else
            Debug.Log("NSR is available -> false. Unsupported platform.");
            return false;
#endif
        }
        
        /// <summary>
        /// A parameter that returns the package version value.
        /// </summary>
        public static string GetVersion => CheckGetVersion();
        
        private static string CheckGetVersion()
        {
#if !UNITY_EDITOR && PLATFORM_IOS
            var result = getVersion();
            Debug.Log(result);
            return result;
#else
            Debug.Log("NSR -> Unsupported platform.");
            return string.Empty;
#endif
        }
        
        /// <summary>
        /// A parameter that indicates whether the package is initialized.
        /// </summary>
        public static bool NSRInit { get; private set; }
        
        /// <summary>
        /// A parameter indicating the current state of screen recording.
        /// </summary>
        public static RecorderStatus CurrentRecorderStatus { get; private set; }
        
        [Header("Settings")]
        [Space(10)]
        [Tooltip("Initialize the framework at any time. If the parameter is enabled, initialization will occur only when you initialize the framework manually.")]
        public bool useDelayInitialization = false;
        [Space(10)]
        [Tooltip("This option allows you to turn the microphone on/off before recording a video.")]
        public bool useMicrophone = true;
        [Tooltip("This option allows you to enable/disable the presentation pop-up window after recording a video. \nThis window also has its own built-in video editor.")]
        public bool usePopoverPresentation = true;
        [Tooltip("This option allows you to automatically save videos to the device's gallery after recording. \n\nIf the usePopoverPresentation option is enabled, auto-saving will not work! \n\nBecause the save window is called, which has the finish editing function - save or share the video file.")]
        public bool useSaveVideoToPhotosAfterRecord = false;

        /// <summary>
        /// An action that is called when the screen recording status changes.
        /// </summary>
        public static UnityAction<RecorderStatus> onRecorderStatus;
        
        /// <summary>
        /// An action that signals the start of screen recording.
        /// </summary>
        public static UnityAction recorderStart;
        
        /// <summary>
        /// An action that signals when the screen recording stops.
        /// </summary>
        public static UnityAction recorderStop;
        
        /// <summary>
        /// An action that signals that you have shared a recording.
        /// </summary>
        public static UnityAction recorderShare;
        
        /// <summary>
        /// An action that signals an error when recording a screen.
        /// </summary>
        public static UnityAction<String> recorderError;
        
        /// <summary>
        /// An action that signals an error when you share a recording.
        /// </summary>
        public static UnityAction<String> recorderShareError;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recStatus">Screen recording status.</param>
        private static void ChangeRecorderStatus(RecorderStatus recStatus)
        {
            CurrentRecorderStatus = recStatus;
            onRecorderStatus?.Invoke(recStatus);
        }
        
        /// <summary>
        /// A method that initializes the framework.
        /// </summary>
        public static void Initialize()
        {
            if (NSRInit)
            {
                return;
            }
            
#if !UNITY_EDITOR && PLATFORM_IOS
            initialize();

            didScreenRecorderStart(DelegateDidScreenRecorderStart);
            didScreenRecorderStop(DelegateDidScreenRecorderStop);
            didScreenRecorderError(DelegateDidScreenRecorderError);
            didShare(DelegateDidShare);
            didShareError(DelegateDidShareError);

            ChangeRecorderStatus(RecorderStatus.Init);
            NSRInit = true;
#else
            NSRInit = false;
#endif
        }
        
        /// <summary>
        /// A method that disposes the framework.
        /// </summary>
        public static void Dispose()
        {
            if (!NSRInit)
            {
                return;
            }
            
#if !UNITY_EDITOR && PLATFORM_IOS
            dispose();
#endif
            ChangeRecorderStatus(RecorderStatus.None);
            NSRInit = false;
        }
        
        /// <summary>
        /// A method that helps you update the settings for recording live video.
        /// </summary>
        /// <param name="microphone">Microphone status.</param>
        /// <param name="popoverPresentation">State of the popover presentation.</param>
        /// <param name="saveVideoToPhotosAfterRecord">Save video to a photo after recording?</param>
        public static void UpdateSettings(bool microphone, bool popoverPresentation, bool saveVideoToPhotosAfterRecord)
        {
#if !UNITY_EDITOR && PLATFORM_IOS
            isMicrophoneEnabled(microphone);
            isPopoverPresentationEnabled(popoverPresentation);
            isSaveVideoToPhotosAfterRecord(saveVideoToPhotosAfterRecord);
#endif
        }
        
        /// <summary>
        /// A method that starts recording the screen.
        /// </summary>
        public static void StartScreenRecorder()
        {
            if (!NSRInit) return;
            
#if !UNITY_EDITOR && PLATFORM_IOS
            startScreenRecorder();
#endif
        }
        
        /// <summary>
        /// A method that stops recording the screen.
        /// </summary>
        public static void StopScreenRecorder()
        {
            if (!NSRInit) return;
            
#if !UNITY_EDITOR && PLATFORM_IOS
            stopScreenRecorder();
#endif
        }

        /// <summary>
        /// A method that allows you to share.
        /// </summary>
        /// <param name="path">Path to video. If the value is null, the value of the last recorded video is taken.</param>
        public static void Share(String path = null)
        {
            if (!NSRInit) return;
            
#if !UNITY_EDITOR && PLATFORM_IOS
            share(path);
#endif
        }

        /// <summary>
        /// A method that allows you to save video file to Photos Album.
        /// </summary>
        /// <param name="path">Path to video. If the value is null, the value of the last recorded video is taken.</param>
        public static void SaveVideoToPhotosAlbum(String path = null)
        {
            if (!NSRInit) return;
            
#if !UNITY_EDITOR && PLATFORM_IOS
            saveVideoToPhotosAlbum(path);
#endif
        }
        
#if !UNITY_EDITOR && PLATFORM_IOS
        [MonoPInvokeCallback(typeof(DidScreenRecorderStartDelegate))]
        private static void DelegateDidScreenRecorderStart()
        {
            ChangeRecorderStatus(RecorderStatus.Recording);
            recorderStart?.Invoke();
        }
        
        [MonoPInvokeCallback(typeof(DidScreenRecorderStopDelegate))]
        private static void DelegateDidScreenRecorderStop()
        {
            ChangeRecorderStatus(RecorderStatus.Stopped);
            recorderStop?.Invoke();
        }
        
        [MonoPInvokeCallback(typeof(DidShareDelegate))]
        private static void DelegateDidShare()
        {
            recorderShare?.Invoke();
        }
        
        [MonoPInvokeCallback(typeof(DidScreenRecorderErrorDelegate))]
        private static void DelegateDidScreenRecorderError(String value)
        {
            recorderError?.Invoke(value);
        }
        
        [MonoPInvokeCallback(typeof(DidShareErrorDelegate))]
        private static void DelegateDidShareError(String value)
        {
            recorderShareError?.Invoke(value);
        }
#endif
        
    }
}