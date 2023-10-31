using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SilverTau.NSR.Recorders.Clocks;
using SilverTau.NSR.Recorders.Inputs;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

namespace SilverTau.NSR.Recorders.Video
{
    /// <summary>
    /// Video encoding format.
    /// </summary>
    [Serializable]
    public enum EncodeTo
    {
        MP4,
        HEVC
    }
    
    /// <summary>
    /// Video encoding format.
    /// </summary>
    [Serializable]
    public enum RecordStatus
    {
        Stopped = 0,
        Recording = 1,
        Paused = 2
    }

    [AddComponentMenu("Silver Tau/NSR/Universal Video Recorder")]
    public class UniversalVideoRecorder : MonoBehaviour
    {
        public static UniversalVideoRecorder Instance { get; set; }
        
        //Camera
        [Tooltip("The main rendering camera.")]
        public Camera mainCamera;
        [Tooltip("List of rendering cameras.")]
        public List<Camera> cameras = new List<Camera>();
        
        //Video
        [Tooltip("The actual name of the output video file name.")]
        public string videoFileName = "video file name";
        [Tooltip("Layers that will be displayed during recording.")]
        public LayerMask layerMasks;
        [Tooltip("Android: Divider for the size of the screen recording.")]
        [Range(1, 10)] public float screenDivideAndroid = 3;
        [Tooltip("iOS: Divider for the size of the screen recording.")]
        [Range(1, 10)] public float screenDivideIOS = 2;
        [Tooltip("Standalone: Divider for the size of the screen recording.")]
        [Range(1, 10)] public float screenDivideStandalone = 2;
        [Tooltip("Set the custom frame rate. Default - 30 fps.")]
        public int frameRate = 30;
        [Tooltip("Video output encoding format.")]
        public EncodeTo encodeTo = EncodeTo.MP4;
        
        [Tooltip("Video recorder manager (preview).")]
        public UniversalVideoRecorderManager universalVideoRecorderManager;
        
        [Tooltip("Action that is called after a video is successfully created.")]
        public UnityAction onCompleteCapture;
        
        //Audio
        [Tooltip("Recording sound from a microphone? A parameter that indicates whether the sound from the microphone will be recorded.")]
        public bool recordMicrophone;
        public bool recordAllAudioSources;
        public AudioListener audioListener;
        public bool recordOnlyOneAudioSource;
        public AudioSource targetAudioSource;
        
        //Time
        [Tooltip("Do we use realtime clock? A parameter that indicates whether the timer will be counted when recording video.")]
        public bool useRealtimeClock;
        [Tooltip("A text component that displays the recording time.")]
        public Text textTimer;
        
        //Advanced
        [Tooltip("Automatically pause/resume video recording during program focus/pause.")]
        [SerializeField] private bool autoPauseResumeRecorder = true;
        
        [Tooltip("Video output path.")]
        public string VideoOutputPath { get; private set; }

        [Tooltip("Is it recording? A setting that indicates whether video is being recorded.")]
        public bool IsRecording { get; private set; }
        
        [Tooltip("A parameter that allows you to find out the status of a record.")]
        public RecordStatus GetRecordStatus { get; private set; }

        /// <summary>
        /// A parameter that allows you to take preview textures for videos.
        /// </summary>
        public Texture2D PreviewImage
        {
            get => _tempPreviewImage;
            private set => _tempPreviewImage = value;
        }
        
        private UniversalVideoRecorderManager _currentUniversalVideoRecorderManager;
        private int _videoWidth;
        private int _videoHeight;
        private IMRecorder _recorder;
        private CameraInput _cameraInput; 
        private AudioInput _audioInput; 
        private AudioSource _microphoneSource;
        private RealtimeClock _recordingClock;
        private float _seconds = 0.0f;
        private float _minutes = 0.0f;
        private float _hours = 0.0f;
        private Texture2D _tempPreviewImage;
        private bool _isRecordingPaused = false;
        private bool _recordMic;
        private bool _appPaused;


        #region Editor

        [HideInInspector] public bool _cameraSettingExpand = true;
        [HideInInspector] public bool _videoSettingExpand = true;
        [HideInInspector] public bool _audioSettingExpand;
        [HideInInspector] public bool _timeSettingExpand;
        [HideInInspector] public bool _advancedSettingExpand;
        [HideInInspector] public bool _infoResolution;

        #endregion
        
        #region MonoBehaviour

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            
        }

        #region Advanced features

        private void OnApplicationPause(bool pauseStatus)
        {
            if(_appPaused) return;
            _appPaused = pauseStatus;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if(!_appPaused) return;

            if (autoPauseResumeRecorder)
            {
                if (!hasFocus)
                {
                    if (GetRecordStatus == RecordStatus.Recording) PauseVideoRecorder();
                }
                else
                {
                    if (GetRecordStatus == RecordStatus.Paused) ResumeVideoRecorder();
                }
            }
            
            _appPaused = !hasFocus;
        }

        #endregion
        
        private void Update()
        {
            if (!useRealtimeClock) return;
            if (!IsRecording) return;
            if (_recordingClock != null)
            {
                if(_recordingClock.paused) return;
            }
            
            _seconds += Time.deltaTime;
            
            if (_seconds >= 59)
            {
                if (_minutes >= 59)
                {
                    _hours += 1.0f;
                    _minutes = 0.0f;
                }

                _minutes += 1.0f;
                _seconds = 0.0f;
            }
            
            textTimer.text = $"{_hours:00}:{_minutes:00}:{_seconds:00}";
        }

        #endregion
        
        #region Video Recorder
        
        /// <summary>
        /// A method that start recording.
        /// </summary>
        public void StartVideoRecorder()
        {
            GetRecordStatus = RecordStatus.Stopped;
            
            FindActiveTargetCamera();
            
            _recordMic = recordMicrophone;
            
            if(mainCamera == null) return;

            StartCoroutine(StartVideoRecording());
        }
        
        /// <summary>
        /// Coroutine that start recording.
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartVideoRecording()
        {
            ScreenDivide();
            
            if (useRealtimeClock)
            {
                _seconds = 0.0f;
                _minutes = 0.0f;
                _hours = 0.0f;
                
                textTimer.text = $"{_hours:00}:{_minutes:00}:{_seconds:00}";
                textTimer.enabled = true;
                textTimer.gameObject.SetActive(true);
            }

#if !UNITY_EDITOR
            _microphoneSource = gameObject.GetComponent<AudioSource>();

            if (_microphoneSource == null)
            {
                _microphoneSource = gameObject.AddComponent<AudioSource>();
            }
        
            _microphoneSource.mute = _microphoneSource.loop = true;
            _microphoneSource.bypassEffects = _microphoneSource.bypassListenerEffects = false;
            
            // Start microphone
            if (_recordMic)
            {
#if PLATFORM_ANDROID
                if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
                {
                    Permission.RequestUserPermission(Permission.Microphone);
                    yield return new WaitForEndOfFrame();
                    yield return new WaitForSeconds(0.2f);
                }
#endif

                try
                {
                    _microphoneSource.clip = Microphone.Start(null, true, 10, AudioSettings.outputSampleRate);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
                
                //_microphoneSource.clip = Microphone.Start(null, true, 10, AudioSettings.outputSampleRate);
                if (Microphone.GetPosition(null) == 0)
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }

            if(_microphoneSource.clip != null) _microphoneSource.Play();
            
            // Start recording
            var sampleRate = AudioSettings.outputSampleRate;
            var channelCount = (int)AudioSettings.speakerMode;
            _recordingClock = new RealtimeClock();
            
            switch (encodeTo)
            {
                case EncodeTo.MP4:
                    _recorder = new MP4FormatRecorder(
                        _videoWidth,
                        _videoHeight,
                        frameRate,
                        sampleRate,
                        channelCount,
                        audioBitRate: 96_000
                    );
                    break;
                case EncodeTo.HEVC:
                    _recorder = new HEVCFormatRecorder(
                        _videoWidth,
                        _videoHeight,
                        frameRate,
                        sampleRate,
                        channelCount,
                        audioBitRate: 96_000
                    );
                    break;
                default:
                    break;
            }
            
            ConfigAudioInput(_recorder);
            ConfigCameraInput(_recorder);
            
            // Unmute microphone
            _microphoneSource.mute = _audioInput == null;
#endif
            GetRecordStatus = RecordStatus.Recording;
            IsRecording = true;
            yield break;
        }
        
        /// <summary>
        /// A method that stops recording.
        /// </summary>
        public async void StopVideoRecorder()
        {
            if (useRealtimeClock)
            {
                _seconds = 0.0f;
                _minutes = 0.0f;
                _hours = 0.0f;
                
                textTimer.text = $"{_hours:00}:{_minutes:00}:{_seconds:00}";
                textTimer.enabled = false;
                textTimer.gameObject.SetActive(false);
            }

            if (universalVideoRecorderManager != null)
            {
                StartCoroutine(GeneratePreviewImage(null, () =>
                {
                    _currentUniversalVideoRecorderManager = Instantiate(universalVideoRecorderManager, transform);
                    _currentUniversalVideoRecorderManager.gameObject.SetActive(true);
                }));
            }

#if !UNITY_EDITOR
            if (GetRecordStatus == RecordStatus.Recording)
            {
                // Stop recording
                _audioInput?.Dispose();
                _cameraInput?.Dispose();
            }

            _microphoneSource.mute = true;

            if (_microphoneSource == null)
            {
                Destroy(_microphoneSource);
            }

            VideoOutputPath = await _recorder.FinishWriting();
            Debug.Log("Test 1 : " + VideoOutputPath);
#else
            await Task.Delay(1000);
#endif
            GetRecordStatus = RecordStatus.Stopped;
            IsRecording = false;
            onCompleteCapture?.Invoke();
#if UNITY_ANDROID || UNITY_IOS
            NativeGallery.SaveVideoToGallery(VideoOutputPath, "Taher", "test1.mp4", null);
            //NativeGalleryManager.Instance.SaveVideoToGallery(VideoOutputPath, "Taher", "test1.mp4", null);
#endif
            Debug.Log("Test 2 : " + VideoOutputPath);
        }

        public void PauseVideoRecorder()
        {
            if(!IsRecording) return;
            if(_recordingClock == null) return;
            if(_recordingClock.paused) return;
            _recordingClock.paused = true;
            _audioInput?.Dispose();
            _cameraInput?.Dispose();
            
            _microphoneSource.mute = true;
            GetRecordStatus = RecordStatus.Paused;
        }

        public void ResumeVideoRecorder()
        {
            if(!IsRecording) return;
            if(_recordingClock == null) return;
            if(!_recordingClock.paused) return;
            
            _recordingClock.paused = false;
            
            ConfigAudioInput(_recorder);
            ConfigCameraInput(_recorder);
            
            _microphoneSource.mute = _audioInput == null;
            GetRecordStatus = RecordStatus.Recording;
        }
        
        /// <summary>
        /// A task that returns the original path of a video file asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetVideoOutputPath()
        {
            if (_recorder == null)
            {
                Debug.Log("Recorder is empty.");
                return VideoOutputPath;
            }
            VideoOutputPath = await _recorder.FinishWriting();
            return VideoOutputPath;
        }

        private void ConfigAudioInput(IMRecorder recorder)
        {
            if (recorder == null)
            {
                _audioInput = null;
                return;
            }
            
            if (recordAllAudioSources)
            {
                _audioInput = audioListener != null ? new AudioInput(recorder, _recordingClock, audioListener) : new AudioInput(recorder, _recordingClock, _microphoneSource, true);
                return;
            }
            
            if (recordOnlyOneAudioSource)
            {
                _audioInput = targetAudioSource != null ? new AudioInput(recorder, _recordingClock, targetAudioSource) : new AudioInput(recorder, _recordingClock, _microphoneSource, true);
                return;
            }
            
            if (_recordMic)
            {
                _audioInput = new AudioInput(recorder, _recordingClock, _microphoneSource, true);
                return;
            }
        }
        
        private void ConfigCameraInput(IMRecorder recorder)
        {
            if (recorder == null)
            {
                _cameraInput = null;
                return;
            }
            
            if (mainCamera == null && cameras.Count == 0)
            {
                _cameraInput = null;
                return;
            }
            
            _cameraInput = cameras.Count > 0 ? new CameraInput(recorder, _recordingClock, cameras.ToArray()) : new CameraInput(recorder, _recordingClock, mainCamera);
        }

        #endregion
        
        #region Help Methods

        /// <summary>
        /// Coroutine that generates an image (preview).
        /// </summary>
        /// <param name="tempImg">Raw Image which will be used to set a preview of the image.</param>
        private IEnumerator GeneratePreviewImage(Texture2D tempImg = null, Action callback = null)
        {
            if(PreviewImage) Destroy(PreviewImage);
            PreviewImage = null;
            
            yield return new WaitForEndOfFrame();

            GetTexture2DFromRenderTexture(mainCamera, Screen.width, Screen.height, () =>
            {
                byte[] jpgData = _tempPreviewImage.EncodeToJPG();
                var currentFileFormat = ".jpg";
                
                var filePath = Path.Combine(Application.temporaryCachePath, "universal_video_recorder_preview" + currentFileFormat);
                File.WriteAllBytes(filePath, jpgData);
        
                if(tempImg) tempImg = PreviewImage;
                callback?.Invoke();
            });
            
            yield break;
        }
        
        /// <summary>
        /// A method that converts RenderTexture and returns Texture2D.
        /// </summary>
        /// <param name="mCamera">Target camera.</param>
        /// <param name="width">Target width.</param>
        /// <param name="height">Target height.</param>
        /// <param name="callback">Callback action.</param>
        private void GetTexture2DFromRenderTexture(Camera mCamera, int width, int height, Action callback)
        {
            var rect = new Rect(0, 0, width, height);
            var renderTexture = new RenderTexture(width, height, 24);
            PreviewImage = new Texture2D(width, height, TextureFormat.RGBA32, false);
 
            var defaultCullingMask = mainCamera.cullingMask;
            mainCamera.cullingMask = 0;
            mainCamera.cullingMask = layerMasks;

            mCamera.targetTexture = renderTexture;
            mCamera.Render();
 
            RenderTexture.active = renderTexture;
            PreviewImage.ReadPixels(rect, 0, 0);
            PreviewImage.Apply();
 
            mCamera.targetTexture = null;
            RenderTexture.active = null;
 
            mainCamera.cullingMask = defaultCullingMask;
            
            Destroy(renderTexture);
            renderTexture = null;
            
            callback?.Invoke();
        }
        
        /// <summary>
        /// An additional method that searches for the active main camera.
        /// </summary>
        private void FindActiveTargetCamera()
        {
            if(mainCamera != null) return;
            if(cameras.Count == 0) return;

            foreach (var cam in cameras)
            {
                if(cam == null) continue;
                if(!cam.gameObject.activeSelf) continue;
                mainCamera = cam;
                break;
            }
        }

        /// <summary>
        /// A method that helps you express the size of a multiple of a number.
        /// </summary>
        /// <param name="a">Value input.</param>
        /// <param name="b">A multiple of a number.</param>
        /// <returns></returns>
        private int ClosestInteger(int a, int b)
        {
            int c1 = a - (a % b);
            int c2 = (a + b) - (a % b);
            if (a - c1 > c2 - a)
            {
                return c2;
            }
            else
            {
                return c1;
            }
        }
        
        /// <summary>
        /// A method that helps to express the target screen size for video recording.
        /// </summary>
        private void ScreenDivide()
        {
            var divide = 3.0f;
#if PLATFORM_ANDROID
            divide = screenDivideAndroid;
#elif PLATFORM_IOS
            divide = screenDivideIOS;
#elif PLATFORM_STANDALONE
            divide = screenDivideStandalone;
#endif
            var width = Screen.width / divide;
            _videoWidth = ClosestInteger((int)width, 2);
            var height = Screen.height / divide;
            _videoHeight = ClosestInteger((int)height, 2);
        }
        
        #endregion

        #region Additional Mathods

        /// <summary>
        /// A method that performs the function of storing video in the device's gallery.
        /// </summary>
        public void SaveVideo()
        {
            if (string.IsNullOrEmpty(VideoOutputPath))
            {
                return;
            }
            
            //Save video
            //Use your own methods to save the video. The final recording path corresponds to the VideoOutputPath parameter.
        }

        public void Dispose()
        {
            if (_currentUniversalVideoRecorderManager)
            {
                Destroy(_currentUniversalVideoRecorderManager.gameObject);
            }
        }
        
        /// <summary>
        /// Method that performs the preview function.
        /// </summary>
        public void Preview()
        {
            StartCoroutine(PlayStreamingVideo());
        }
    
        /// <summary>
        /// Coroutine that performs the preview function.
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayStreamingVideo()
        {
#if !UNITY_STANDALONE
            Handheld.PlayFullScreenMovie($"file://{VideoOutputPath}");
#endif
            
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.2f);
        }
        
        #endregion

        #region Share
        
        /// <summary>
        /// Method that performs the share function.
        /// </summary>
        /// <returns></returns>
        public void ShareVideo()
        {
            //Share video
            //Use your own methods to share the video. The final recording path corresponds to the VideoOutputPath parameter.
        }

        #endregion
    }
}