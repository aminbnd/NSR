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
#if UNITY_ANDROID && !UNITY_EDITOR
    var outputPath = UniversalVideoRecorder.Instance.VideoOutputPath;
    var fileName = UniversalVideoRecorder.Instance.videoFileName;

    if (string.IsNullOrEmpty(outputPath) || string.IsNullOrEmpty(fileName))
    {
        Debug.LogError("outputPath or fileName is null or empty");
        return;
    }

    // Ensure that outputPath is the correct file path, not a directory
    if (!outputPath.EndsWith("/" + fileName))
    {
        outputPath = Path.Combine(outputPath, fileName);
    }

    if (File.Exists(outputPath))
    {
        // Create an Android intent for sharing
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        // Set the action to ACTION_SEND
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));

        // Set the type to video/*
        intentObject.Call<AndroidJavaObject>("setType", "video/*");

        // Attach the video file
        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
        AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromFile", new AndroidJavaObject("java.io.File", outputPath));
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);

        // Set the title of the sharing dialog
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TITLE"), "Share Video");

        // Start the Android sharing activity
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share Video");
        currentActivity.Call("startActivity", chooser);
    }
    else
    {
        Debug.LogError("Video file does not exist: " + outputPath);
    }
#else
            Debug.LogWarning("Sharing is only supported on Android");
#endif
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
