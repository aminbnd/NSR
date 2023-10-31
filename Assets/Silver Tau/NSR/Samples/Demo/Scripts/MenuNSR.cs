using System;
using UnityEngine;
using UnityEngine.UI;

namespace SilverTau.NSR.Samples
{
    public class MenuNSR : MonoBehaviour
    {
        [Header("Settings")]
        [Space(10)]
        [Tooltip("This option allows you to turn the microphone on/off before recording a video.")]
        public bool useMicrophone = true;
        [Tooltip("This option allows you to enable/disable the presentation pop-up window after recording a video. \nThis window also has its own built-in video editor.")]
        public bool usePopoverPresentation = true;
        [Tooltip("This option allows you to automatically save videos to the device's gallery after recording. \n\nIf the usePopoverPresentation option is enabled, auto-saving will not work! \n\nBecause the save window is called, which has the finish editing function - save or share the video file.")]
        public bool useSaveVideoToPhotosAfterRecord = false;
        
        [Header("UI")]
        [Space(10)]
        [SerializeField] private Button btnInit;
        [SerializeField] private Button btnDispose;
        [SerializeField] private Button btnIsInit;
        [SerializeField] private Button btnIsAvailable;
        [SerializeField] private Button btnStartScreenRecorder;
        [SerializeField] private Button btnStopScreenRecorder;
        [SerializeField] private Button btnIsRecording;
        [SerializeField] private Button btnGetVersion;
        [SerializeField] private Button btnGetVideoOutputPath;
        [SerializeField] private Button btnGetCurrentRecorderStatus;
        [SerializeField] private Button btnShare;
        [SerializeField] private Button btnSaveVideoToPhotosAlbum;
        [SerializeField] private Toggle tgUseMicrophone;
        [SerializeField] private Toggle tgUsePopoverPresentation;
        [SerializeField] private Toggle tgUseSaveVideoToPhotosAfterRecord;
        
        [Header("Debug")]
        [Space(10)]
        [SerializeField] private ScrollRect debugScrollRect;
        [SerializeField] private Text debug;
        [SerializeField] private bool debugConsole;

        private string _debugLog;

        #region MonoBehaviour

        private void Start()
        {
            if(btnInit) btnInit.onClick.AddListener(Init);
            if(btnDispose) btnDispose.onClick.AddListener(Dispose);
            if(btnIsAvailable) btnIsAvailable.onClick.AddListener(IsAvailable);
            if(btnIsInit) btnIsInit.onClick.AddListener(IsInit);
            if(btnStartScreenRecorder) btnStartScreenRecorder.onClick.AddListener(StartScreenRecorder);
            if(btnStopScreenRecorder) btnStopScreenRecorder.onClick.AddListener(StopScreenRecorder);
            if(btnIsRecording) btnIsRecording.onClick.AddListener(IsRecording);
            if(btnGetVersion) btnGetVersion.onClick.AddListener(GetVersion);
            if(btnGetVideoOutputPath) btnGetVideoOutputPath.onClick.AddListener(GetVideoOutputPath);
            if(btnGetCurrentRecorderStatus) btnGetCurrentRecorderStatus.onClick.AddListener(GetCurrentRecorderStatus);
            if(btnShare) btnShare.onClick.AddListener(() =>
            {
                Share(ScreenRecorder.videoOutputPath);
            });
            
            if(btnSaveVideoToPhotosAlbum) btnSaveVideoToPhotosAlbum.onClick.AddListener(() =>
            {
                SaveVideoToPhotosAlbum(ScreenRecorder.videoOutputPath);
            });
            
            if(tgUseMicrophone) tgUseMicrophone.onValueChanged.AddListener(OnValueChangedMicrophone);
            if(tgUsePopoverPresentation) tgUsePopoverPresentation.onValueChanged.AddListener(OnValueChangedPopoverPresentation);
            if(tgUseSaveVideoToPhotosAfterRecord) tgUseSaveVideoToPhotosAfterRecord.onValueChanged.AddListener(OnValueChangedSaveVideoToPhotosAfterRecord);
        }

        private void OnEnable()
        {
            ScreenRecorder.recorderStart += RecorderStart;
            ScreenRecorder.recorderStop += RecorderStop;
            ScreenRecorder.recorderError += RecorderError;
            ScreenRecorder.recorderShare += RecorderShare;
            ScreenRecorder.recorderShareError += RecorderShareError;
        }

        private void OnDisable()
        {
            ScreenRecorder.recorderStart -= RecorderStart;
            ScreenRecorder.recorderStop -= RecorderStop;
            ScreenRecorder.recorderError -= RecorderError;
            ScreenRecorder.recorderShare -= RecorderShare;
            ScreenRecorder.recorderShareError -= RecorderShareError;
        }

        #endregion

        #region UnityActions

        private void RecorderStart()
        {
            SendMassageToDebug("Screen recording has started.");
        }
        
        private void RecorderStop()
        {
            SendMassageToDebug("Screen recording is stopped.");
        }
        
        private void RecorderError(String value)
        {
            SendMassageToDebug(value);
        }
        
        private void RecorderShare()
        {
            SendMassageToDebug("Share.");
        }
        
        private void RecorderShareError(String value)
        {
            SendMassageToDebug(value);
        }

        #endregion

        #region Toggles

        private void OnValueChangedMicrophone(bool status)
        {
            useMicrophone = status;
            SendMassageToDebug("The changes will be applied only when you record the screen again.");
        }

        private void OnValueChangedPopoverPresentation(bool status)
        {
            usePopoverPresentation = status;
            
            if (status && useSaveVideoToPhotosAfterRecord)
            {
                useSaveVideoToPhotosAfterRecord = false;
                tgUseSaveVideoToPhotosAfterRecord.isOn = false;
                SendMassageToDebug("This option allows you to automatically save videos to the device's gallery after recording. \n\nIf the usePopoverPresentation option is enabled, auto-saving will not work! \n\nBecause the save window is called, which has the finish editing function - save or share the video file.");
            }
            
            SendMassageToDebug("The changes will be applied only when you record the screen again.");
        }

        private void OnValueChangedSaveVideoToPhotosAfterRecord(bool status)
        {
            useSaveVideoToPhotosAfterRecord = status;
            
            if (status && usePopoverPresentation)
            {
                usePopoverPresentation = false;
                tgUsePopoverPresentation.isOn = false;
                SendMassageToDebug("This option allows you to automatically save videos to the device's gallery after recording. \n\nIf the usePopoverPresentation option is enabled, auto-saving will not work! \n\nBecause the save window is called, which has the finish editing function - save or share the video file.");
            }
            
            SendMassageToDebug("The changes will be applied only when you record the screen again.");
        }

        #endregion
        
        #region Buttons

        private void Init()
        {
            if (ScreenRecorder.NSRInit)
            {
                SendMassageToDebug("The framework has already been initialized.");
                return;
            }
            
            ScreenRecorder.Initialize();
            SendMassageToDebug("The framework is initialized.");
        }
        
        private void Dispose()
        {
            if (!ScreenRecorder.NSRInit)
            {
                SendMassageToDebug("The framework has already been disposed.");
                return;
            }
            
            ScreenRecorder.Dispose();
            SendMassageToDebug("The framework has been disposed of.");
        }
        
        private void StartScreenRecorder()
        {
            if(!ScreenRecorder.IsAvailable) return;
            if (!ScreenRecorder.IsRecording)
            {
                ScreenRecorder.UpdateSettings(useMicrophone, usePopoverPresentation, useSaveVideoToPhotosAfterRecord);
            }

            ScreenRecorder.StartScreenRecorder();
        }

        private void StopScreenRecorder()
        {
            ScreenRecorder.StopScreenRecorder();
        }

        private void IsAvailable()
        {
            if (ScreenRecorder.IsAvailable)
            {
                SendMassageToDebug("Screen recording is available.");
                return;
            }
            
            SendMassageToDebug("Screen recording is not available.");
        }

        private void IsRecording()
        {
            if (ScreenRecorder.IsRecording)
            {
                SendMassageToDebug("Screen recording in progress.");
                return;
            }
            
            SendMassageToDebug("The screen is not recorded.");
        }

        private void IsInit()
        {
            if (ScreenRecorder.NSRInit)
            {
                SendMassageToDebug("NSR is initialized.");
                return;
            }
            
            SendMassageToDebug("NSR error not initialized.");
        }

        private void GetVersion()
        {
            var version = ScreenRecorder.GetVersion;
            SendMassageToDebug(version);
        }

        private void GetVideoOutputPath()
        {
            var videoOutputPath = ScreenRecorder.videoOutputPath;
            SendMassageToDebug(videoOutputPath);
        }

        private void GetCurrentRecorderStatus()
        {
            var recorderStatus = ScreenRecorder.CurrentRecorderStatus;
            SendMassageToDebug(recorderStatus.ToString());
        }

        private void Share(string path)
        {
            if(!ScreenRecorder.NSRInit) return;
            if(!ScreenRecorder.IsAvailable) return;
            if (string.IsNullOrEmpty(path)) return;
            ScreenRecorder.Share(path);
        }

        private void SaveVideoToPhotosAlbum(string path)
        {
            if(!ScreenRecorder.NSRInit) return;
            if(!ScreenRecorder.IsAvailable) return;
            if (string.IsNullOrEmpty(path)) return;
            ScreenRecorder.SaveVideoToPhotosAlbum(path);
        }

        #endregion

        #region Debug

        private void SendMassageToDebug(string text)
        {
            _debugLog += DateTime.Now.ToLongTimeString() + ": " + text + "\n";
            if (debug) debug.text = _debugLog;
            if (debugScrollRect) debugScrollRect.verticalNormalizedPosition = 0;
            if (debugConsole) Debug.Log(text);
        }

        #endregion
    }
}
