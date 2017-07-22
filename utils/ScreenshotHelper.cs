using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using System;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ScreenshotHelper))]
public class ScreenshotHelperEditor : Editor
{
    

    public override void OnInspectorGUI()
    {
        ScreenshotHelper helper = (ScreenshotHelper)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Take Screenshot"))
        {
            
            Rect lRect = new Rect(0f, 0f, 2f*Screen.width, Screen.height);
            if (helper.capturedImage) Destroy(helper.capturedImage);


            helper.capturedImage = zzTransparencyCapture.capture(lRect);

            Sprite image = Sprite.Create(helper.capturedImage, lRect, new Vector2(0.5f, 0.5f), 100f);
            helper.preview.sprite = image;

            zzTransparencyCapture.saveScreenshot(helper.capturedImage, helper.dir + "\\" + DateTime.Now.ToString("hmmss_ddmmyy") + ".png");
            
            helper.count++;
        }

    }

}
#endif


public class ScreenshotHelper: MonoBehaviour
{
    public Texture2D capturedImage;
    public SpriteRenderer preview;
    public Transform cameraTransform;
    public string dir = "C:\\Users\\VS\\Desktop\\screenshots";
    public int count = 0;
}