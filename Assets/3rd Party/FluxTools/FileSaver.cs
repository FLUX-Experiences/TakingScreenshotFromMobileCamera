using System;
using System.Collections;
using UnityEngine;

public class FileSaver : MonoBehaviour
{

    public UnityEngine.UI.Text logger;

    void Start()
    {
    }

    


    public void SetTexture(Texture2D _texture)
    {
        lastSavedScreenshot = _texture;
    }

    private Texture2D lastSavedScreenshot;

    public UnityEngine.Events.UnityEvent OnFileSavedSuccessfully;


    private string GetFileName(string app_name = "")
    {
        string filename = app_name;
        filename += DateTime.Now.ToString() + "_" + (UnityEngine.Random.value * 1000).ToString("0:000");

        filename += ".png";
        return filename;
    }



    
    public void StartSaveProcessWithGrabber()
    {
        logger.text = "Permission 111: " + NativeGallery.RequestPermission();

        StartCoroutine(TakeScreenshotAndSave());
    }

    private IEnumerator TakeScreenshotAndSave()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        // Save the screenshot to Gallery/Photos
        logger.text = "Permission 222: " + NativeGallery.SaveImageToGallery(ss, "GalleryTest", "Image.png", NativeGalleryError);

        // To avoid memory leaks
        Destroy(ss);

        OnFileSavedSuccessfully.Invoke();
    }
    private void NativeGalleryError(string result_error)
    {
        Debug.Log(result_error);
        logger.text = result_error;
    }


}
