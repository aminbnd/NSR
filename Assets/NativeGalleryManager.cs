using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NativeGallery;

public class NativeGalleryManager : MonoBehaviour
{
    private static NativeGalleryManager instance = null;
    public static NativeGalleryManager Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        // Initialisation du Native Gallery Manager
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SaveVideoToGallery(string existingMediaPath, string album, string filename, MediaSaveCallback callback = null)
    {
        NativeGallery.SaveVideoToGallery(existingMediaPath, album, filename, callback);
    }
}
