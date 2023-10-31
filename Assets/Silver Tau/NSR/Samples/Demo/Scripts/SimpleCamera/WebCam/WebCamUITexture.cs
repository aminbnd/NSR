using System;
using System.Collections;
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#elif PLATFORM_IOS
using UnityEngine.iOS;
#endif
using UnityEngine.UI;

namespace SilverTau.NSR.Samples
{
    public class WebCamUITexture : MonoBehaviour
    {
        [Space(10)]
        public RawImage phoneCamera;
        public AspectRatioFitter aspectRatioFitter;
        /// <summary>
        /// Set the name of the device to use.
        /// </summary>
        [SerializeField, TooltipAttribute("Set the name of the device to use.")]
        public string requestedDeviceName = null;

        /// <summary>
        /// Set the width of WebCamTexture.
        /// </summary>
        [SerializeField, TooltipAttribute("Set the width of WebCamTexture.")]
        public int requestedWidth = 320;

        /// <summary>
        /// Set the height of WebCamTexture.
        /// </summary>
        [SerializeField, TooltipAttribute("Set the height of WebCamTexture.")]
        public int requestedHeight = 240;

        /// <summary>
        /// Set FPS of WebCamTexture.
        /// </summary>
        [SerializeField, TooltipAttribute("Set FPS of WebCamTexture.")]
        public int requestedFPS = 30;

        /// <summary>
        /// Set whether to use the front facing camera.
        /// </summary>
        [SerializeField, TooltipAttribute("Set whether to use the front facing camera.")]
        public bool requestedIsFrontFacing = false;

        /// <summary>
        /// Determines if adjust pixels direction.
        /// </summary>
        [SerializeField, TooltipAttribute("Determines if adjust pixels direction.")]
        public bool adjustPixelsDirection = false;

        [SerializeField, TooltipAttribute("Enable debug.")]
        public bool debug;
        
        /// <summary>
        /// The webcam texture.
        /// </summary>
        WebCamTexture webCamTexture;

        /// <summary>
        /// The webcam device.
        /// </summary>
        WebCamDevice webCamDevice;

        /// <summary>
        /// The colors.
        /// </summary>
        Color32[] colors;

        /// <summary>
        /// The rotated colors.
        /// </summary>
        Color32[] rotatedColors;

        /// <summary>
        /// Determines if rotates 90 degree.
        /// </summary>
        bool rotate90Degree = false;

        public bool flipVertical = false;

        public bool flipHorizontal = false;

        /// <summary>
        /// Indicates whether this instance is waiting for initialization to complete.
        /// </summary>
        bool isInitWaiting = false;

        /// <summary>
        /// Indicates whether this instance has been initialized.
        /// </summary>
        bool hasInitDone = false;

        /// <summary>
        /// The screenOrientation.
        /// </summary>
        ScreenOrientation screenOrientation;

        /// <summary>
        /// The width of the screen.
        /// </summary>
        int screenWidth;

        /// <summary>
        /// The height of the screen.
        /// </summary>
        int screenHeight;

        /// <summary>
        /// The texture.
        /// </summary>
        Texture2D texture;

        // Use this for initialization
        void Start()
        {
            //Run();
        }

        private void OnEnable()
        {
            Run();
        }

        private void OnDisable()
        {
            Dispose();
        }

        private void Run()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes webcam texture.
        /// </summary>
        private void Initialize()
        {
            if (isInitWaiting)
                return;

    #if UNITY_ANDROID && !UNITY_EDITOR
                // Set the requestedFPS parameter to avoid the problem of the WebCamTexture image becoming low light on some Android devices. (Pixel, pixel 2)
                if (requestedIsFrontFacing) {
                    int rearCameraFPS = requestedFPS;
                    requestedFPS = 15;
                    StartCoroutine (_Initialize ());
                    requestedFPS = rearCameraFPS;
                } else {
                    StartCoroutine (_Initialize ());
                }
    #else
            StartCoroutine(_Initialize());
    #endif
        }

        /// <summary>
        /// Initializes webcam texture by coroutine.
        /// </summary>
        private IEnumerator _Initialize()
        {
            if (hasInitDone)
                Dispose();

            isInitWaiting = true;

#if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
                yield return new WaitForEndOfFrame();
            }
#elif PLATFORM_IOS
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
                yield return new WaitForEndOfFrame();
            }  
#endif
            yield return new WaitForSeconds(0.2f);
            
            // Creates the camera
            if (!String.IsNullOrEmpty(requestedDeviceName))
            {
                int requestedDeviceIndex = -1;
                if (Int32.TryParse(requestedDeviceName, out requestedDeviceIndex))
                {
                    if (requestedDeviceIndex >= 0 && requestedDeviceIndex < WebCamTexture.devices.Length)
                    {
                        webCamDevice = WebCamTexture.devices[requestedDeviceIndex];
                        webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight, requestedFPS);
                    }
                }
                else
                {
                    for (int cameraIndex = 0; cameraIndex < WebCamTexture.devices.Length; cameraIndex++)
                    {
                        if (WebCamTexture.devices[cameraIndex].name == requestedDeviceName)
                        {
                            webCamDevice = WebCamTexture.devices[cameraIndex];
                            webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight, requestedFPS);
                            break;
                        }
                    }
                }

                if (webCamTexture == null)
                {
                    if(debug) Debug.Log("Cannot find camera device " + requestedDeviceName + ".");
                }
            }

            if (webCamTexture == null)
            {
                // Checks how many and which cameras are available on the device
                for (int cameraIndex = 0; cameraIndex < WebCamTexture.devices.Length; cameraIndex++)
                {
                    if (WebCamTexture.devices[cameraIndex].isFrontFacing == requestedIsFrontFacing)
                    {
                        webCamDevice = WebCamTexture.devices[cameraIndex];
                        webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight, requestedFPS);
                        break;
                    }
                }
            }

            if (webCamTexture == null)
            {
                if (WebCamTexture.devices.Length > 0)
                {
                    webCamDevice = WebCamTexture.devices[0];
                    webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight, requestedFPS);
                }
                else
                {
                    //Debug.LogError("Camera device does not exist.");
                    isInitWaiting = false;
                    yield break;
                }
            }

            // Starts the camera
            webCamTexture.Play();

            while (true)
            {
                //If you want to use webcamTexture.width and webcamTexture.height on iOS, you have to wait until webcamTexture.didUpdateThisFrame == 1, otherwise these two values will be equal to 16.
    #if UNITY_IOS && !UNITY_EDITOR && (UNITY_4_6_3 || UNITY_4_6_4 || UNITY_5_0_0 || UNITY_5_0_1)
                    if (webCamTexture.width > 16 && webCamTexture.height > 16) {
    #else
                if (webCamTexture.didUpdateThisFrame)
                {
    #if UNITY_IOS && !UNITY_EDITOR && UNITY_5_2
                        while (webCamTexture.width <= 16) {
                            webCamTexture.GetPixels32 ();
                            yield return new WaitForEndOfFrame ();
                        } 
    #endif
    #endif

                    if (debug)
                    {
                        Debug.Log("name:" + webCamTexture.deviceName + " width:" + webCamTexture.width + " height:" + webCamTexture.height + " fps:" + webCamTexture.requestedFPS);
                        Debug.Log("videoRotationAngle:" + webCamTexture.videoRotationAngle + " videoVerticallyMirrored:" + webCamTexture.videoVerticallyMirrored + " isFrongFacing:" + webCamDevice.isFrontFacing);
                    }
                    
                    screenOrientation = Screen.orientation;
                    screenWidth = Screen.width;
                    screenHeight = Screen.height;
                    isInitWaiting = false;
                    hasInitDone = true;

                    OnInited();

                    break;
                }
                else
                {
                    yield return 0;
                }
            }
        }

        /// <summary>
        /// Releases all resource.
        /// </summary>
        private void Dispose()
        {
            rotate90Degree = false;
            isInitWaiting = false;
            hasInitDone = false;

            if (webCamTexture != null)
            {
                webCamTexture.Stop();
                WebCamTexture.Destroy(webCamTexture);
                webCamTexture = null;
            }
            if (texture != null)
            {
                Texture2D.Destroy(texture);
                texture = null;
            }
        }

        /// <summary>
        /// Raises the webcam texture initialized event.
        /// </summary>
        private void OnInited()
        {
            if (colors == null || colors.Length != webCamTexture.width * webCamTexture.height)
            {
                colors = new Color32[webCamTexture.width * webCamTexture.height];
                rotatedColors = new Color32[webCamTexture.width * webCamTexture.height];
            }

            if (adjustPixelsDirection)
            {
    #if !UNITY_EDITOR && !(UNITY_STANDALONE || UNITY_WEBGL)
                    if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown) {
                        rotate90Degree = true;
                    }else{
                        rotate90Degree = false;
                    }
    #endif
            }
            if (rotate90Degree)
            {
                texture = new Texture2D(webCamTexture.height, webCamTexture.width, TextureFormat.RGBA32, false);
                float ratio = (float)webCamTexture.height/ webCamTexture.width;
                aspectRatioFitter.aspectRatio = ratio;
            }
            else
            {
                texture = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);
                float ratio = webCamTexture.width / (float)webCamTexture.height;
                aspectRatioFitter.aspectRatio = ratio;
            }

            phoneCamera.texture = texture;

            if (debug) Debug.Log("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);

            float width = texture.width;
            float height = texture.height;

            float widthScale = (float)Screen.width / width;
            float heightScale = (float)Screen.height / height;
        }

        // Update is called once per frame
        void Update()
        {
            if (adjustPixelsDirection)
            {
                // Catch the orientation change of the screen.
                if (screenOrientation != Screen.orientation && (screenWidth != Screen.width || screenHeight != Screen.height))
                {
                    Initialize();
                }
                else
                {
                    screenWidth = Screen.width;
                    screenHeight = Screen.height;
                }
            }

            if (hasInitDone && webCamTexture.isPlaying && webCamTexture.didUpdateThisFrame)
            {
                Color32[] colors = GetColors();

                if (colors != null)
                {
                    if (texture != null)
                    {
                        texture.SetPixels32(colors);
                        texture.Apply(false);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the current WebCameraTexture frame that converted to the correct direction.
        /// </summary>
        private Color32[] GetColors()
        {
            try
            {
                if (webCamTexture != null)
                {
                    if (hasInitDone && webCamTexture.isPlaying)
                    {
                        webCamTexture.GetPixels32(colors);
                    }
                }
            }
            catch (Exception e)
            {
                if (debug) Debug.Log(e);
                gameObject.SetActive(false);
            }

            if (adjustPixelsDirection)
            {
                //Adjust an array of color pixels according to screen orientation and WebCamDevice parameter.
                if (rotate90Degree)
                {
                    try
                    {
                        Rotate90CW(colors, rotatedColors, webCamTexture.width, webCamTexture.height);
                        FlipColors(rotatedColors, webCamTexture.width, webCamTexture.height);
                    }
                    catch (Exception e)
                    {
                        if (debug) Debug.Log(e);
                        gameObject.SetActive(false);
                    }
                    return rotatedColors;
                }
                else
                {
                    try
                    {
                        FlipColors(colors, webCamTexture.width, webCamTexture.height);
                    }
                    catch (Exception e)
                    {
                        if (debug) Debug.Log(e);
                        gameObject.SetActive(false);
                    }
                    return colors;
                }
            }
            return colors;
        }

        /// <summary>
        /// Raises the destroy event.
        /// </summary>
        void OnDestroy()
        {
            Dispose();
        }

        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick()
        {
        }

        public void VericalFlip()
        {
            if (flipVertical)
            {
                flipVertical = false;
            }
            else
            {
                flipVertical = true;
            }
        }

        public void HorizontalFlip()
        {
            if (flipHorizontal)
            {
                flipHorizontal = false;
            }
            else
            {
                flipHorizontal = true;
            }
        }

        /// <summary>
        /// Raises the change camera button click event.
        /// </summary>
        public void OnChangeCamera()
        {
            if (hasInitDone)
            {
                requestedDeviceName = null;
                requestedIsFrontFacing = !requestedIsFrontFacing;
                Initialize();
            }
        }

        /// <summary>
        /// Flips the colors.
        /// </summary>
        /// <param name="colors">Colors.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        void FlipColors(Color32[] colors, int width, int height)
        {
            int flipCode = int.MinValue;

            if (webCamDevice.isFrontFacing)
            {
                if (webCamTexture.videoRotationAngle == 0)
                {
                    flipCode = 1;
                }
                else if (webCamTexture.videoRotationAngle == 90)
                {
                    flipCode = 1;
                }
                if (webCamTexture.videoRotationAngle == 180)
                {
                    flipCode = 0;
                }
                else if (webCamTexture.videoRotationAngle == 270)
                {
                    flipCode = 0;
                }
            }
            else
            {
                if (webCamTexture.videoRotationAngle == 0)
                {
                    flipCode = 1;
                }
                else if (webCamTexture.videoRotationAngle == 90)
                {
                    flipCode = 1;
                }
                if (webCamTexture.videoRotationAngle == 180)
                {
                    flipCode = 0;
                }
                else if (webCamTexture.videoRotationAngle == 270)
                {
                    flipCode = 0;
                }
            }

            if (flipCode > int.MinValue)
            {
                if (rotate90Degree)
                {
                    if (flipCode == 0)
                    {
                        FlipVertical(colors, colors, height, width);
                    }
                    else if (flipCode == 1)
                    {
                        FlipHorizontal(colors, colors, height, width);
                    }
                    else if (flipCode < 0)
                    {
                        Rotate180(colors, colors, height, width);
                    }
                }
                else
                {
                    if (flipCode == 0)
                    {
                        FlipVertical(colors, colors, width, height);
                    }
                    else if (flipCode == 1)
                    {
                        FlipHorizontal(colors, colors, width, height);
                    }
                    else if (flipCode < 0)
                    {
                        Rotate180(colors, colors, height, width);
                    }
                }

                if (flipVertical && rotate90Degree)
                {
                    FlipVertical(colors, colors, height, width);
                }
                else if(flipVertical && !rotate90Degree)
                {
                    FlipVertical(colors, colors, width, height);
                }

                if (flipHorizontal && rotate90Degree)
                {
                    FlipHorizontal(colors, colors, height, width);
                }
                else if (flipHorizontal && !rotate90Degree)
                {
                    FlipHorizontal(colors, colors, width, height);
                }
            }
        }

        /// <summary>
        /// Flips vertical.
        /// </summary>
        /// <param name="src">Src colors.</param>
        /// <param name="dst">Dst colors.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        void FlipVertical(Color32[] src, Color32[] dst, int width, int height)
        {
            for (var i = 0; i < height / 2; i++)
            {
                var y = i * width;
                var x = (height - i - 1) * width;
                for (var j = 0; j < width; j++)
                {
                    int s = y + j;
                    int t = x + j;
                    Color32 c = src[s];
                    dst[s] = src[t];
                    dst[t] = c;
                }
            }
        }

        /// <summary>
        /// Flips horizontal.
        /// </summary>
        /// <param name="src">Src colors.</param>
        /// <param name="dst">Dst colors.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        void FlipHorizontal(Color32[] src, Color32[] dst, int width, int height)
        {
            for (int i = 0; i < height; i++)
            {
                int y = i * width;
                int x = y + width - 1;
                for (var j = 0; j < width / 2; j++)
                {
                    int s = y + j;
                    int t = x - j;
                    Color32 c = src[s];
                    dst[s] = src[t];
                    dst[t] = c;
                }
            }
        }

        /// <summary>
        /// Rotates 180 degrees.
        /// </summary>
        /// <param name="src">Src colors.</param>
        /// <param name="dst">Dst colors.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        void Rotate180(Color32[] src, Color32[] dst, int height, int width)
        {
            int i = src.Length;
            for (int x = 0; x < i / 2; x++)
            {
                Color32 t = src[x];
                dst[x] = src[i - x - 1];
                dst[i - x - 1] = t;
            }
        }

        /// <summary>
        /// Rotates 90 degrees (CLOCKWISE).
        /// </summary>
        /// <param name="src">Src colors.</param>
        /// <param name="dst">Dst colors.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        void Rotate90CW(Color32[] src, Color32[] dst, int height, int width)
        {
            int i = 0;
            for (int x = height - 1; x >= 0; x--)
            {
                for (int y = 0; y < width; y++)
                {
                    dst[i] = src[x + y * height];
                    i++;
                }
            }
        }

        /// <summary>
        /// Rotates 90 degrees (COUNTERCLOCKWISE).
        /// </summary>
        /// <param name="src">Src colors.</param>
        /// <param name="dst">Dst colors.</param>
        /// <param name="height">Height.</param>
        /// <param name="width">Width.</param>
        void Rotate90CCW(Color32[] src, Color32[] dst, int width, int height)
        {
            int i = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = height - 1; y >= 0; y--)
                {
                    dst[i] = src[x + y * width];
                    i++;
                }
            }
        }
    }
}