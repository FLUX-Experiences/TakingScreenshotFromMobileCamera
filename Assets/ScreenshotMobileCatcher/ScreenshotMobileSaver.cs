using System.Collections;
using UnityEngine;

public class ScreenshotMobileSaver : MonoBehaviour
{
    [Header("required settings")]
    public string description1= "Remove the Plugins Folder outside to main Assets.";
    public string description2= "Android: remember to set the build player settings.";
    public string description3= "iOS: watch the required changes there ";

    
    public void TakePictureWithNativeCamera(int maxSize=512)
    {
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            Debug.Log("Image path: " + path);

        }, maxSize);

        Debug.Log("Permission result: " + permission);
    }

    public void TakePictureWithNativeCameraAndShowIt(int maxSize = 512)
    {
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            Debug.Log("Image path: " + path);


            PresentPhotoFromPathOnPhone(path, maxSize);

        }, maxSize);

        Debug.Log("Permission result: " + permission);
    }

    private void PresentPhotoFromPathOnPhone(string _path, int _maxsize)
    {
        // Create Texture from selected image
        Texture2D texture = NativeGallery.LoadImageAtPath(_path, _maxsize);
        if (texture == null)
        {
            Debug.Log("Couldn't load texture from " + _path);
            return;
        }

        // Assign texture to a temporary quad and destroy it after 5 seconds
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
        quad.transform.forward = Camera.main.transform.forward;
        quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

        Material material = quad.GetComponent<Renderer>().material;
        if (!material.shader.isSupported) // happens when Standard shader is not included in the build
            material.shader = Shader.Find("Legacy Shaders/Diffuse");

        material.mainTexture = texture;

        Destroy(quad, 5f);

        // If a procedural texture is not destroyed manually, 
        // it will only be freed after a scene change
        Destroy(texture, 5f);

    }

    public void SaveScreenshotToGallery()
    {
        // Take a screenshot and save it to Gallery/Photos
        StartCoroutine(TakeScreenshotAndSave());
    }

    private IEnumerator TakeScreenshotAndSave()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        // Save the screenshot to Gallery/Photos
        Debug.Log("Permission result: " + NativeGallery.SaveImageToGallery(ss, "GalleryTest", "Image.png"));

        // To avoid memory leaks
        Destroy(ss);
    }

    /// <summary>
    /// Pick a PNG image from Gallery/Photos
    /// If the selected image's width and/or height is greater than 512px, down-scale the image
    /// </summary>
    /// <param name="maxSize">512</param>
    public void PickImage(int maxSize=512)
    {
        if (NativeGallery.IsMediaPickerBusy())
            return;


        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                PresentPhotoFromPathOnPhone(path, maxSize);
            }
        }, "Select a PNG image", "image/png");

        Debug.Log("Permission result: " + permission);
    }

    /// <summary>
    /// Pick a video from Gallery/Photos
    /// </summary>
    public void PickVideo()
    {
        if (NativeGallery.IsMediaPickerBusy())
            return;

        NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((path) =>
        {
            Debug.Log("Video path: " + path);
            if (path != null)
            {
                // Play the selected video
                Handheld.PlayFullScreenMovie("file://" + path);
            }
        }, "Select a video");

        Debug.Log("Permission result: " + permission);
    }
}
