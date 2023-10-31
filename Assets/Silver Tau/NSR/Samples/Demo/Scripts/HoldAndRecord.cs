using System;
using System.Collections;
using SilverTau.NSR.Recorders.Video;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

namespace SilverTau.NSR.Samples
{
    [RequireComponent(typeof(Button))]
    public class HoldAndRecord : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Settings")]
        [Space(10)]
        [Tooltip("This option allows you to turn the microphone on/off before recording a video.")]
        public bool useMicrophone = true;

        [Header("Settings")]
        [Tooltip("Maximum duration that button can be pressed. If the value is 0, the recording will continue indefinitely as long as you hold down the record button.")]
        [Range(0f, 60f)] public float maxDuration = 0f;
        
        [Header("Events")]
        public UnityEvent onTouchDown;
        public UnityEvent onTouchUp;
        
        [Header("UI")]
        [Space(10)]
        [SerializeField] private Text timer;
        
        [Header("Debug")]
        [Space(10)]
        [SerializeField] private ScrollRect debugScrollRect;
        [SerializeField] private Text debug;
        [SerializeField] private bool debugConsole;

        private Button _btnStartStopScreenRecorder;
        private string _debugLog;
        private bool _isRecording;

        #region MonoBehaviour

        private void Start()
        {
            if (_btnStartStopScreenRecorder == null)
            {
                _btnStartStopScreenRecorder = GetComponent<Button>();
            }
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

        public void OnPointerDown (PointerEventData eventData) => StartCoroutine(HoldRecord());

        public void OnPointerUp (PointerEventData eventData) => _isRecording = false;

        #endregion

        #region Buttons

        private void StartScreenRecorder()
        {
            UniversalVideoRecorder.Instance.recordMicrophone = useMicrophone;
            UniversalVideoRecorder.Instance.StartVideoRecorder();
            if(timer) timer.gameObject.SetActive(true);
            onTouchDown?.Invoke();
            SendMassageToDebug("Screen recording has started.");
        }

        private void StopScreenRecorder()
        {
            UniversalVideoRecorder.Instance.StopVideoRecorder();
            if(timer) timer.gameObject.SetActive(false);
            _isRecording = false;
            onTouchUp?.Invoke();
            SendMassageToDebug("Screen recording has stopped.");
        }

        private IEnumerator HoldRecord () {
            _isRecording = true;
            
            yield return new WaitForSeconds(0.2f);
            if (!_isRecording) yield break;
            
            StartScreenRecorder();
            
            var recordTimer = Time.time;
            while (_isRecording) {
                if (maxDuration > 2f)
                {
                    var ratio = (Time.time - recordTimer) / maxDuration;
                    _isRecording = ratio <= 1f;
                }
                yield return null;
            }
            
            StopScreenRecorder();
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
