using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PhoneCameraUnit : MonoBehaviour
{
    [Header("camera feed")]
    public PhoneCameraRenderer CameraFeed;
    
    [Header("camera record")]
    public Animator CameraFlickerScreen;
    private string flickerAnimationTrigger="FlickerNow";
    public float timeOfFlickerEffect = 0.5f;
    public string AlbumName = "your app name";
    public string FileNameOnGallery = "app_camera"; //without ending file type

    private Texture2D lastShotTaken;

    /// <summary>
    /// user just asked to make a screenshot (save the screen). we are doing a flicker animation
    /// </summary>
    public UnityEvent ShotStarted;

    /// <summary>
    /// system waits for the end of the rendering frame. we are a moment before actually taking the snapshot
    /// use this place to remove all ui elements you dont want to appear in the shot
    /// </summary>
    public UnityEvent RightBeforeTakingShot;

    /// <summary>
    /// finished making the screenshot. it takes a few moments. 
    /// use this to return the ui elements removed for the shot. and to send the result
    /// </summary>
    [System.Serializable]
    public class ValueTexture2DChanged : UnityEvent<Texture2D> { };
    public ValueTexture2DChanged CameraShotTaken;

    void Start()
    {
        CameraFeed.OnCameraInitSuccessfully.AddListener(OnCameraFeedInitSuccessfully);
    }

    private void OnCameraFeedInitSuccessfully()
    {
        //match size of flicker plane to size of camera 
        CameraFlickerScreen.transform.localScale = new Vector3(CameraFeed.transform.localScale.x, CameraFeed.transform.localScale.y, 1);
    }

    public void TakeShot()
    {
        ShotStarted.Invoke();

        CameraFlickerScreen.SetTrigger(flickerAnimationTrigger);

        // Take a screenshot and save it to Gallery/Photos
        StartCoroutine(TakeScreenshotAndSave());
    }

    private IEnumerator TakeScreenshotAndSave()
    {
        //finish flicker animation
        yield return new WaitForSeconds(timeOfFlickerEffect);

        RightBeforeTakingShot.Invoke();

        //finish render
        yield return new WaitForEndOfFrame();

        lastShotTaken = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        lastShotTaken.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        
        if (lastShotTaken != null) lastShotTaken.Apply();

        CameraShotTaken.Invoke(lastShotTaken);

        SaveTexture(lastShotTaken);
    }

    private void NativeGalleryError(string result_error)
    {
        Debug.Log(result_error);
    }

    public void SaveTexture(Texture2D file)
    {
        if (file == null) return;

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            // Save the screenshot to Gallery/Photos
            var result = NativeGallery.SaveImageToGallery(file, AlbumName, FileNameOnGallery+".png", NativeGalleryError);
            Debug.Log("Permission result: " + result);

            Destroy(file);  
#endif
    }

}
