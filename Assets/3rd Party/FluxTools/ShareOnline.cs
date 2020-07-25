using UnityEngine;

public class ShareOnline : MonoBehaviour
{
	private PhoneCameraUnit cameraUnit;
	public void ShotAndShareFromCamera()
	{
		if (cameraUnit == null)
			cameraUnit = FindObjectOfType<PhoneCameraUnit>();

		cameraUnit.CameraShotTaken.AddListener(ShareTexture);
		cameraUnit.TakeShot();
	}

	private void ShareTexture(Texture2D fileToShare)
	{
		//save temporary pic
		string filePath = System.IO.Path.Combine(Application.temporaryCachePath, "shared img.png");
		System.IO.File.WriteAllBytes(filePath, fileToShare.EncodeToPNG());

		//share
		new NativeShare().AddFile(filePath).SetSubject("Subject goes here").SetText("Hello world!").Share();


		cameraUnit.CameraShotTaken.RemoveListener(ShareTexture);
	}

	/*
	//called from unity
	public void ScreenshotAndShareNow()
	{
		StartCoroutine(TakeSSAndShare());
	}

	private IEnumerator TakeSSAndShare()
	{
		//take screenshot
		yield return new WaitForEndOfFrame();
		Texture2D resultedPic = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		resultedPic.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		resultedPic.Apply();

		//share now
		ShareTexture(resultedPic);

		// To avoid memory leaks
		Destroy(resultedPic); 
	}
	*/

	public void PickImageFromGalleryAndShare(string shareText= "Subject goes here")
	{
		NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
		{
			Debug.Log("Image path: " + path);
			if (path != null)
			{
				new NativeShare().AddFile(path).SetSubject(shareText).SetText(shareText).Share();

				// Share on WhatsApp only, if installed (Android only)
				//if( NativeShare.TargetExists( "com.whatsapp" ) )
				//	new NativeShare().AddFile( path ).SetText( "Hello world!" ).SetTarget( "com.whatsapp" ).Share();
			}
		}, "Select a PNG image", "image/png");

		Debug.Log("Permission result: " + permission);
	}
}