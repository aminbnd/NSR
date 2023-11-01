using NativeShareAndroid;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace SilverTau.NSR.Recorders.Video
{
    public class UniversalVideoRecorderManager : MonoBehaviour
    {
        public RawImage rawImage;
        [SerializeField] private Button buttonBack;
        [SerializeField] private Button buttonPreview;
        [SerializeField] private Button buttonOpenInFileBrowser;
        [SerializeField] private Button buttonShare; // Added

        private ScreenOrientation _screenOrientation;
        private bool _autorotateToPortrait;
        private bool _autorotateToPortraitUpsideDown;
        private bool _autorotateToLandscapeLeft;
        private bool _autorotateToLandscapeRight;

        private void Start()
        {
            if (buttonBack)
            {
                buttonBack.onClick.AddListener(Back);
#if PLATFORM_STANDALONE
                if(buttonBack) buttonBack.gameObject.SetActive(true);
#elif PLATFORM_IOS || PLATFORM_ANDROID
                if (buttonBack) buttonBack.gameObject.SetActive(true);
#endif
            }

            if (buttonPreview)
            {
                buttonPreview.onClick.AddListener(Preview);
#if PLATFORM_STANDALONE
                if(buttonPreview) buttonPreview.gameObject.SetActive(false);
#elif PLATFORM_IOS || PLATFORM_ANDROID
                if (buttonPreview) buttonPreview.gameObject.SetActive(true);
#endif
            }

            if (buttonOpenInFileBrowser)
            {
                buttonOpenInFileBrowser.onClick.AddListener(OpenInFileBrowser);
#if PLATFORM_STANDALONE
                if(buttonOpenInFileBrowser) buttonOpenInFileBrowser.gameObject.SetActive(true);
#elif PLATFORM_IOS || PLATFORM_ANDROID
                if (buttonOpenInFileBrowser) buttonOpenInFileBrowser.gameObject.SetActive(false);
#endif
            }
            if (buttonShare)
            {
                buttonShare.onClick.AddListener(Share);
#if PLATFORM_STANDALONE
                if(buttonShare) buttonShare.gameObject.SetActive(true);
#elif PLATFORM_IOS || PLATFORM_ANDROID
                if (buttonShare) buttonShare.gameObject.SetActive(true);
#endif
            }
        }

        private void OnEnable()
        {
            _screenOrientation = Screen.orientation;

            _autorotateToPortrait = Screen.autorotateToPortrait;
            _autorotateToPortraitUpsideDown = Screen.autorotateToPortraitUpsideDown;
            _autorotateToLandscapeLeft = Screen.autorotateToLandscapeLeft;
            _autorotateToLandscapeRight = Screen.autorotateToLandscapeRight;

            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;

            if (rawImage) rawImage.texture = UniversalVideoRecorder.Instance.PreviewImage;
        }

        private void OnDisable()
        {
            Screen.autorotateToPortrait = _autorotateToPortrait;
            Screen.autorotateToPortraitUpsideDown = _autorotateToPortraitUpsideDown;
            Screen.autorotateToLandscapeLeft = _autorotateToLandscapeLeft;
            Screen.autorotateToLandscapeRight = _autorotateToLandscapeRight;

        }

        private void OnDestroy()
        {
            Screen.autorotateToPortrait = _autorotateToPortrait;
            Screen.autorotateToPortraitUpsideDown = _autorotateToPortraitUpsideDown;
            Screen.autorotateToLandscapeLeft = _autorotateToLandscapeLeft;
            Screen.autorotateToLandscapeRight = _autorotateToLandscapeRight;
        }

        private void Back()
        {
            //SaveVideo();
            UniversalVideoRecorder.Instance.Dispose();
        }

        private void SaveVideo()
        {
            var outputPath = UniversalVideoRecorder.Instance.VideoOutputPath;
            if (string.IsNullOrEmpty(outputPath))
            {
                return;
            }

            //Save video
            //Use your own methods to save the video. The final recording path corresponds to the VideoOutputPath parameter.
            try
            {
                var resultVideoFileBytes = File.ReadAllBytes(outputPath);

                var format = string.Empty;

                switch (UniversalVideoRecorder.Instance.encodeTo)
                {
                    case EncodeTo.MP4:
                        format = ".mp4";
                        break;
                    default:
                        format = ".mp4";
                        break;
                }

                File.WriteAllBytes(Path.Combine(Application.persistentDataPath, UniversalVideoRecorder.Instance.videoFileName + format), resultVideoFileBytes);
            }
            catch (Exception e)
            {
                Debug.Log("Save error:" + e);
                throw;
            }
        }

        private void Preview()
        {
            UniversalVideoRecorder.Instance.Preview();
        }

        private void OpenInFileBrowser()
        {
            var outputPath = UniversalVideoRecorder.Instance.VideoOutputPath;

#if PLATFORM_STANDALONE
            SilverTau.NSR.OpenInFileBrowser.OpenFileBrowser(outputPath);
#endif
        }

        private void Share()
        {
            var outputPath = UniversalVideoRecorder.Instance.VideoOutputPath;
            var fileName = UniversalVideoRecorder.Instance.videoFileName;

            // Check if the file exists
            string filePath = Path.Combine(outputPath, fileName);
            if (File.Exists(filePath))
            {
                // Copy the video to the external storage directory
                string destinationPath = Path.Combine(Application.persistentDataPath, fileName);
                File.Copy(filePath, destinationPath, true);

                // Share the video from the external storage directory
                NativeShare.ShareOnAndroid(destinationPath, "Share Video");
            }
            else
            {
                Debug.LogError("Video file does not exist: " + filePath);
            }
        }




        //Display a basic share prompt for other platforms
        private void ShowSharePopup(string message)
        {
            // Implement a platform-specific share dialogue here (e.g., using native plugins).
            Debug.Log("Sharing on this platform is not supported.");
        }

        private void Test()
        {
            var outputPath = UniversalVideoRecorder.Instance.VideoOutputPath;
            var fileName = UniversalVideoRecorder.Instance.videoFileName;
            NativeShare.ShareOnAndroid(outputPath, fileName);
        }

    }
}
