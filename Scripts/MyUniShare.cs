using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if NETFX_CORE
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Foundation;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime;
using Windows;
using System;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class MyUniShare: MonoBehaviour  {

	#if UNITY_EDITOR
	static MyUniShare()
	{
	//	PlayerSettings.Android.forceSDCardPermission = true; 
	}
	#endif 

	private Texture2D createdTexture;
	private bool instantShare = false; 
	private string screenshotPath;

	public Image SharePopupImage;
	public GameObject SharePopupWindow;
	public GameObject SharePopupWatermark;
	public string ScreenshotName="screenshot.png";
	public string ShareText="UniShare just works! #unishare"; 

	#if UNITY_IPHONE
	[DllImport ("__Internal")]	
		private static extern void presentActivitySheetWithImageAndString(string message,byte[] imgData,int _length);
	#endif

	void Start()
	{
		if(SharePopupWatermark)SharePopupWatermark.SetActive(false);

		#if UNITY_ANDROID
		AndroidJNIHelper.debug = true;
		#endif
	}

	public void TakeScreenshot()
	{
		instantShare = false;
		StartCoroutine(getScreenshot());
	}

	public void TakeScreenshotAndShare()
	{
		instantShare = true;
		StartCoroutine(getScreenshot());
	}

	public void ShareScreenshot()
	{
		ShareNativeImage ( ShareText );
	}
		
	IEnumerator getScreenshot()
	{
		
		if(SharePopupWatermark)SharePopupWatermark.SetActive(true);

		yield return new WaitForEndOfFrame();

		createdTexture = new Texture2D(Screen.width,Screen.height,TextureFormat.ARGB32,false);
		createdTexture.ReadPixels(new Rect(0,0, Screen.width,Screen.height), 0, 0,false);
		createdTexture.Apply();

		if(SharePopupWatermark)SharePopupWatermark.SetActive(false);
		if(SharePopupImage)
		{
			Sprite image = Sprite.Create(createdTexture,new Rect(0,0,Screen.width,Screen.height),new Vector2(0.5f,0.5f),100f);
			SharePopupImage.sprite = image;
		}

		if(SharePopupWindow)SharePopupWindow.SetActive(true);

		if(instantShare)
		{
			ShareScreenshot();
		}

		yield return null;
	}



	void ShareNativeImage(string shareText) {
        /*
		if(Application.isEditor)
		{
			#if UNITY_EDITOR
			UnityEditor.EditorUtility.DisplayDialog("Ooops!","Social sharing is not working in the editor!","Got it!");
			#endif
			Debug.LogError("UniShare: Social sharing is not working in the editor!");
			return;
		}
        */
#if NETFX_CORE

		screenshotPath = Path.Combine (Application.persistentDataPath, ScreenshotName).Replace("/","\\");

		byte[] imgData = createdTexture.EncodeToPNG();

		//write out all the bytes into a png
		File.WriteAllBytes (screenshotPath, imgData);

		_Call();

#endif

#if UNITY_IPHONE
		byte[] imgData = createdTexture.EncodeToPNG();
		presentActivitySheetWithImageAndString(shareText,imgData,imgData.Length);
#endif

#if UNITY_ANDROID

        Debug.Log("UNISHARE: " + "started");


        string snapshot_file = "blah";// Central.Instance.getSnapshotFile(false);
        if (System.IO.File.Exists(snapshot_file))
        {
            Debug.Log("Snapshot file exists " + snapshot_file + "\n");
            shareText += "\n\n\nP.S. your save games are below\n--------------------------\n" + snapshot_file + "\n";
            StreamReader theReader = new StreamReader(snapshot_file, Encoding.Default);
            shareText += theReader.ReadToEnd();
        }

        snapshot_file = "hey";// Central.Instance.getSnapshotFile(true);
            
        if (System.IO.File.Exists(snapshot_file))
        {
            Debug.Log("Snapshot file exists " + snapshot_file +  "\n");
            shareText += "\n--------------------------\n" + snapshot_file + "\n";
            StreamReader theReader = new StreamReader(snapshot_file, Encoding.Default);
            shareText += theReader.ReadToEnd();
        }

        Debug.Log(shareText);

        string screenShotPath = Application.persistentDataPath + "/" + ScreenshotName;
        System.IO.File.WriteAllBytes(screenShotPath, createdTexture.EncodeToPNG());

        Debug.Log("UNISHARE: " + System.IO.File.Exists(screenShotPath));

        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");

        Debug.Log("UNISHARE: " + "file://" + screenShotPath);

        AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + screenShotPath);
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
        intentObject.Call<AndroidJavaObject>("setType", "image/png");
        



        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareText);

        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share");
        currentActivity.Call("startActivity", jChooser);


#endif
    }

    void _Call()
	{
		#if NETFX_CORE

		//ui thread of course..
		UnityEngine.WSA.Application.InvokeOnUIThread(() =>
		{

		DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
		dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);

		Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();

		}, false);

		#endif
	}

	#if NETFX_CORE
	private async void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
	{
		try
		{
			DataRequest request = e.Request;
			request.Data.Properties.Title = "Share";
			request.Data.Properties.Description = ShareText;

			Windows.Storage.StorageFile sampleFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(screenshotPath);

			request.Data.SetBitmap(RandomAccessStreamReference.CreateFromFile(sampleFile));
		}
		catch (System.Exception ex)
		{
			Debug.Log(ex.Message);
		}
	}
	#endif

	#if UNITY_EDITOR
	static class UniShareGameObjectCreator {
		[MenuItem("GameObject/UniShare", false,1)]
		static void Create() {

			PlayerSettings.Android.forceSDCardPermission = true;

			GameObject go = new GameObject("UniShare", typeof(MyUniShare));

			Canvas canvas = Selection.activeGameObject ? Selection.activeGameObject.GetComponent<Canvas>() : null;

			Selection.activeGameObject = go;
		}
	}
	#endif
}

