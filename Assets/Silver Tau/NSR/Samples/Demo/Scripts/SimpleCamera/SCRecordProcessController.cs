using SilverTau.NSR.Recorders.Video;
using UnityEngine;
using UnityEngine.UI;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#elif PLATFORM_IOS
using UnityEngine.iOS;
#endif

namespace SilverTau.NSR.Samples
{
    public class SCRecordProcessController : MonoBehaviour
    {
        [Header("Scripts")]
        [Space(10)]
        [SerializeField] private UniversalVideoRecorder universalVideoRecorder;
        
        [Header("Microphone")]
        [Space(10)]
        [SerializeField] private Button buttonMicrophone;
        [SerializeField] private Image imageMicrophone;
        [SerializeField] private Sprite spriteMicrophoneOn;
        [SerializeField] private Sprite spriteMicrophoneOff;

        [Header("Web Camera")]
        [Space(10)]
        [SerializeField] private WebCamUITexture webCamUITexture;
        [SerializeField] private Button buttonSwitchCamera;
        
        [Header("Settings")]
        [Space(10)]
        [SerializeField] private Button buttonSettingsOptionFrameRate;
        [SerializeField] private Text textSettingsOptionFrameRate;
        [SerializeField] private Button buttonSettingsOptionQuality;
        [SerializeField] private Text textSettingsOptionQuality;
        
        private bool _microphoneStatus = false;
        private bool _switchCamera = false;
        private int _frameRate = 30;
        private int _quality = 2;
        
        private void Start()
        {
            Init();
        }
        
        /// <summary>
        /// Initialization of components.
        /// </summary>
        private void Init()
        {
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
            buttonMicrophone.onClick.AddListener(MicrophoneStatus);
            buttonSwitchCamera.onClick.AddListener(SwitchCamera);
            buttonSettingsOptionFrameRate.onClick.AddListener(ChangeFrameRate);
            buttonSettingsOptionQuality.onClick.AddListener(ChangeQuality);
        }
        
        /// <summary>
        /// Function for changing the microphone status.
        /// </summary>
        private void MicrophoneStatus()
        {
            _microphoneStatus = !_microphoneStatus;
            universalVideoRecorder.recordMicrophone = _microphoneStatus;
            universalVideoRecorder.recordAllAudioSources = !_microphoneStatus;
            imageMicrophone.sprite = _microphoneStatus ? spriteMicrophoneOn : spriteMicrophoneOff;
        }
        
        /// <summary>
        /// Function for changing the camera status.
        /// </summary>
        private void SwitchCamera()
        {
            _switchCamera = !_switchCamera;
            webCamUITexture.OnChangeCamera();
            webCamUITexture.flipHorizontal = _switchCamera;
        }
        
        /// <summary>
        /// Function for changing the frame rate.
        /// </summary>
        private void ChangeFrameRate()
        {
            _frameRate = _frameRate switch
            {
                60 => 30,
                30 => 25,
                25 => 24,
                24 => 60,
                _ => 60
            };

            universalVideoRecorder.frameRate = _frameRate;
            textSettingsOptionFrameRate.text = _frameRate.ToString();
        }
        
        /// <summary>
        /// A function to change the quality of the output video.
        /// </summary>
        private void ChangeQuality()
        {
            string text;

            switch (_quality)
            {
                case 1:
                    _quality = 2;
                    text = "MEDIUM";
                    break;
                case 2:
                    _quality = 3;
                    text = "LOW";
                    break;
                case 3:
                    _quality = 1;
                    text = "HIGH";
                    break;
                default:
                    _quality = 1;
                    text = "HIGH";
                    break;
            }
            
            universalVideoRecorder.screenDivideAndroid = _quality;
            universalVideoRecorder.screenDivideIOS = _quality;
            universalVideoRecorder.screenDivideStandalone = _quality;
            textSettingsOptionQuality.text = text;
        }
    }
}