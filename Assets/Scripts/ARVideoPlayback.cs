using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARVideoPlayback : MonoBehaviour
{
    [SerializeField]
    ARTrackedImageManager m_TrackedImageManager;

    [System.Serializable]
    public class ARVideoImageTarget
    {
        public string referenceImageName;
        public GameObject videoPrefab;
    }

    public ARVideoImageTarget[] videoTargets;

    private Dictionary<string, GameObject> instantiatedVideos;

    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnChanged;
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnChanged;
    }

    void Start()
    {
        instantiatedVideos = new Dictionary<string, GameObject>();
    }

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            HandleImage(newImage);
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            HandleImage(updatedImage);
        }

        foreach (var removedImage in eventArgs.removed)
        {
            HandleRemovedImage(removedImage);
        }
    }

    void HandleImage(ARTrackedImage trackedImage)
    {
        string imageTargetName = trackedImage.referenceImage.name;

        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            if (instantiatedVideos.TryGetValue(imageTargetName, out GameObject videoObject))
            {
                // The video prefab is already instantiated and the target is tracked
                // Update the position and rotation of the video object to match the tracked image
                UpdateVideoPositionAndRotation(videoObject, trackedImage.transform);
                videoObject.SetActive(true);

                // Get the VideoPlayer component and start playing if not already playing
                VideoPlayer videoPlayer = videoObject.GetComponentInChildren<VideoPlayer>();
                if (videoPlayer != null && !videoPlayer.isPlaying)
                {
                    videoPlayer.Play();
                }
            }
            else
            {
                // Instantiate the video prefab and add it to the dictionary when the target is tracked
                foreach (var videoTarget in videoTargets)
                {
                    if (imageTargetName == videoTarget.referenceImageName)
                    {
                        GameObject newVideoObject = Instantiate(videoTarget.videoPrefab, trackedImage.transform.position, trackedImage.transform.rotation);
                        instantiatedVideos.Add(imageTargetName, newVideoObject);

                        // Get the VideoPlayer component and start playing
                        VideoPlayer videoPlayer = newVideoObject.GetComponentInChildren<VideoPlayer>();
                        if (videoPlayer != null)
                        {
                            videoPlayer.Play();
                        }
                    }
                }
            }
        }
        else
        {
            // The target is not tracked; remove the video object
            if (instantiatedVideos.TryGetValue(imageTargetName, out GameObject videoObject))
            {
                instantiatedVideos.Remove(imageTargetName);
                Destroy(videoObject);
            }
        }
    }

    void HandleRemovedImage(ARTrackedImage trackedImage)
    {
        string imageTargetName = trackedImage.referenceImage.name;

        if (instantiatedVideos.TryGetValue(imageTargetName, out GameObject videoObject))
        {
            instantiatedVideos.Remove(imageTargetName);
            Destroy(videoObject);
        }
    }

    void UpdateVideoPositionAndRotation(GameObject videoObject, Transform imageTransform)
    {
        // Update the position of the video object to match the tracked image
        videoObject.transform.position = imageTransform.position;

        // Use the tracked image's rotation without modifying it
        videoObject.transform.rotation = imageTransform.rotation;
    }
}