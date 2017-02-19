using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TweenCounter: MonoBehaviour {
	float deltaTime = 0.0001f;
	public Text text;
    int count = 0;
	// Update is calle;d once per frame
	
	void Update () {
        int count = LeanTween.tweensRunning;
        text.text = string.Format("Tweens: {0:0.}", count);
        if (count < 50) return;

        Peripheral.Instance.Pause(true);
        string[] display = LeanTween.getTweenList(40);

        foreach (string hi in display) text.text += "\n" + hi;
            
        /*
        count += LeanTween.tweensRunning;
        deltaTime += Time.deltaTime;

        if (deltaTime < 1f) return;

		float tps = count / deltaTime;        
        if (tps > 999) tps = 999;
		text.text = string.Format("Tweens: {0:0.}", tps);
        count = 0;
        deltaTime = 0f;
		*/
    }
}
