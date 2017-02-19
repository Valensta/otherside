using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FPSCounter : MonoBehaviour {
	float deltaTime = 0.0001f;
	public Text text;
    int frames = 0;
	// Update is calle;d once per frame
	
	void Update () {
        frames++;
        deltaTime += Time.deltaTime;

        if (deltaTime < 1f) return;

		float fps = frames / deltaTime;        
        if (fps > 999) fps = 999;
		text.text = string.Format("{0:0.}", fps);
        frames = 0;
        deltaTime = 0f;
		
	}
}
