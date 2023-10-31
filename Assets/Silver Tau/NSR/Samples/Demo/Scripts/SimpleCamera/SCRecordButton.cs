using System.Collections;
using SilverTau.NSR.Recorders.Video;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SilverTau.NSR.Samples
{
    [RequireComponent(typeof(Button))]
    public class SCRecordButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Scripts")]
        [SerializeField] private SCAudioWorker audioWorker;
        
        [Header("Settings")]
        [Tooltip("Maximum duration that button can be pressed. If the value is 0, the recording will continue indefinitely as long as you hold down the record button.")]
        [Range(0f, 60f)] public float maxDuration = 0f;
        
        [Header("Events")]
        public UnityEvent onTouchDown;
        public UnityEvent onTouchUp;
        
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
        }

        private void OnDisable()
        {
            if (_isRecording)
            {
                UniversalVideoRecorder.Instance.StopVideoRecorder();
            }
            
            _isRecording = false;
        }

        public void OnPointerDown (PointerEventData eventData) => StartCoroutine(HoldRecord());

        public void OnPointerUp (PointerEventData eventData) => _isRecording = false;

        #endregion

        #region Buttons

        private void StartScreenRecorder()
        {
            UniversalVideoRecorder.Instance.StartVideoRecorder();
            onTouchDown?.Invoke();
        }

        private void StopScreenRecorder()
        {
            UniversalVideoRecorder.Instance.StopVideoRecorder();
            audioWorker.ResetProcess();
            _isRecording = false;
            onTouchUp?.Invoke();
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
    }
}