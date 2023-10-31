using System;
using SilverTau.NSR.Recorders.Video;
using UnityEngine;
using UnityEngine.UI;

#if PLATFORM_ANDROID
    using UnityEngine.Android;
#endif

namespace SilverTau.NSR.Samples
{
    public class MenuNSRUniversal : MonoBehaviour
    {
        [Header("Settings")]
        [Space(10)]
        [Tooltip("This option allows you to turn the microphone on/off before recording a video.")]
        public bool useMicrophone = true;
        
        [Header("UI")]
        [Space(10)]
        [SerializeField] private Button btnStartScreenRecorder;
        [SerializeField] private Button btnStopScreenRecorder;
        [SerializeField] private Button btnIsRecording;
        [SerializeField] private Toggle tgUseMicrophone;
        [SerializeField] private Button btnPauseScreenRecorder;
        [SerializeField] private Button btnResumeScreenRecorder;
        [SerializeField] private Text timer;
        
        [Header("Debug")]
        [Space(10)]
        [SerializeField] private ScrollRect debugScrollRect;
        [SerializeField] private Text debug;
        [SerializeField] private bool debugConsole;

        private string _debugLog;
        private bool _isRecording;

        #region MonoBehaviour

        private void Start()
        {
            if(btnStartScreenRecorder) btnStartScreenRecorder.onClick.AddListener(StartScreenRecorder);
            if(btnStopScreenRecorder) btnStopScreenRecorder.onClick.AddListener(StopScreenRecorder);
            if(btnIsRecording) btnIsRecording.onClick.AddListener(IsRecording);
            if(tgUseMicrophone) tgUseMicrophone.onValueChanged.AddListener(OnValueChangedMicrophone);
            if(btnStartScreenRecorder) btnPauseScreenRecorder.onClick.AddListener(PauseScreenRecorder);
            if(btnStartScreenRecorder) btnResumeScreenRecorder.onClick.AddListener(ResumeScreenRecorder);
        }

        private void OnEnable()
        {
            _isRecording = false;
            if (timer)
            {
                timer.gameObject.SetActive(false);
                UniversalVideoRecorder.Instance.textTimer = timer;
            }
            
#if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
#elif PLATFORM_IOS
            if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                Application.RequestUserAuthorization(UserAuthorization.Microphone);
            }
#endif
        }

        private void OnDisable()
        {
            if (_isRecording)
            {
                UniversalVideoRecorder.Instance.StopVideoRecorder();
            }
            
            _isRecording = false;
            if(timer) timer.gameObject.SetActive(false);
        }

        #endregion

        #region Toggles

        private void OnValueChangedMicrophone(bool status)
        {
            useMicrophone = status;
            SendMassageToDebug("The changes will be applied only when you record the screen again.");
        }
        
        #endregion
        
        #region Buttons

        private void StartScreenRecorder()
        {
            if (_isRecording)
            {
                SendMassageToDebug("The screen is recorded.");
                return;
            }

            UniversalVideoRecorder.Instance.recordMicrophone = useMicrophone;
            UniversalVideoRecorder.Instance.StartVideoRecorder();
            if(timer) timer.gameObject.SetActive(true);
            _isRecording = true;
            SendMassageToDebug("Screen recording has started.");
        }

        private void StopScreenRecorder()
        {
            if (!_isRecording) 
            { 
                SendMassageToDebug("Screen recording does not occur.");
                return; 
            }
            
            UniversalVideoRecorder.Instance.StopVideoRecorder();
            if(timer) timer.gameObject.SetActive(false);
            _isRecording = false;
            SendMassageToDebug("Screen recording has stopped.");
        }

        private void PauseScreenRecorder()
        {
            if (!_isRecording) 
            { 
                SendMassageToDebug("Screen recording does not occur.");
                return; 
            }
            
            UniversalVideoRecorder.Instance.PauseVideoRecorder();
            SendMassageToDebug("Screen recording has paused.");
        }

        private void ResumeScreenRecorder()
        {
            if (!_isRecording) 
            { 
                SendMassageToDebug("Screen recording does not occur.");
                return; 
            }
            
            UniversalVideoRecorder.Instance.ResumeVideoRecorder();
            SendMassageToDebug("Screen recording has resumed.");
        }

        private void IsRecording()
        {
            if (_isRecording)
            {
                SendMassageToDebug("Screen recording in progress.");
                return;
            }
            
            SendMassageToDebug("The screen is not recorded.");
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
