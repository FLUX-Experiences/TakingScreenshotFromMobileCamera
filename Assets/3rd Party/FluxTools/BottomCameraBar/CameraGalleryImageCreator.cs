using UnityEngine;
using UnityEngine.Events;

public class CameraGalleryImageCreator : MonoBehaviour
{   
    [System.Serializable]
    public class ValueTexture2DChanged : UnityEvent<UnityEngine.Texture2D> { };
    public ValueTexture2DChanged OnImageReady;

    void Start()
    {
        
    }

    /// <summary>
    /// uses NativeCamera lib
    /// </summary>
    /// <param name="maxSize"></param>
    public void TakePictureWithNativeCameraAndShowIt(int maxSize = 512)
    {
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            Debug.Log("Image path: " + path);

            CreateTextureFromImagePath(path, maxSize);
            //PresentPhotoFromPathOnPhone(path, maxSize);

        }, maxSize);

        Debug.Log("Permission result: " + permission);
    }

    /// <summary>
    /// shows an image from the phone gallery on a canvas
    /// uses NativeGallery
    /// </summary>
    /// <param name="_path"></param>
    /// <param name="_maxsize"></param>
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

        // If a procedural texture is not destroyed manually, it will only be freed after a scene change
        Destroy(texture, 5f);
    }

    private void CreateTextureFromImagePath(string _path, int _maxsize)
    {
        // Create Texture from selected image
        Texture2D texture = NativeGallery.LoadImageAtPath(_path, _maxsize);
        if (texture == null)
        {
            Debug.Log("Couldn't load texture from " + _path);
            return;
        }

        OnImageReady.Invoke(texture);
    }
    /// <summary>
    /// Pick a PNG image from Gallery/Photos
    /// If the selected image's width and/or height is greater than 512px, down-scale the image
    /// uses NativeGallery
    /// </summary>
    /// <param name="maxSize">512</param>
    public void PickImage(int maxSize = 512)
    {
        if (NativeGallery.IsMediaPickerBusy())
            return;

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                CreateTextureFromImagePath(path, maxSize);
                //PresentPhotoFromPathOnPhone(path, maxSize);
            }
        }, "Select a PNG image", "image/png");

        Debug.Log("Permission result: " + permission);
    }

}
