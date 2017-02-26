using UnityEngine;
using System.Collections;


public class FadeMe : MonoBehaviour{
	public float time;
	public bool auto_fade_on_enable = false;
	public CanvasGroup canvas_group;
	public float steps = 10.0f;
	public bool auto_kill_on_fadeout = false;//return to zoo
    public bool disable_on_fadeout = false; //just disable it
	public GameObject parent;
	float from = 0;
	float to = 1f;
    public bool auto_set_block_raycasts = false;
    public bool auto_pause = false;

	public void FadeIn(){
		_FadeIn ();
	}

	void _FadeIn(){
		from = 0f;
		to = 1f;
		StartCoroutine ("_Fade");
        if (auto_pause)
        {
            Peripheral.Instance.ResumeNormalSpeed();
            EagleEyes.Instance.SetFFButtons(false);
            Peripheral.Instance.Pause(true);
        }
	}

	void _FadeOut(){
		from = 1f;
		to = 0f;
		
        if (auto_pause)
        {            
            Peripheral.Instance.Pause(false);
            Peripheral.Instance.ResumeNormalSpeed();
            EagleEyes.Instance.SetFFButtons(true);
        }
        StartCoroutine("_Fade");
    }

	IEnumerator _Fade(){

		float current_time = 0f;
		float inc = (to - from)/steps;
		float time_inc = time/steps;
		float alpha = from;
	//	Debug.Log ("Fading " + this.name + "\n");
		while (current_time <= time) {

			canvas_group.alpha = alpha;
			alpha += inc;
            set_blocksRaycasts();
            current_time += time_inc;
		//	Debug.Log("alphaing " + current_time);
			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(time_inc));
		}
		if (auto_kill_on_fadeout && to == 0 && parent != null){
			Peripheral.Instance.zoo.returnObject(parent);
		}
        if (disable_on_fadeout && to == 0)
        {
            this.gameObject.SetActive(false);
        }
	}

	/*
	void OnClick(){
		_FadeOut();
	}
	
	
	public void OnFingerTap(Lean.LeanFinger finger){
		_FadeOut();
	}
*/
	public void OnClick(){
        Tracker.Log("FadeMe OnClick " + this.gameObject.name);

        _FadeOut();
	}

	void OnEnable (){	
		if (auto_fade_on_enable) {
			_FadeIn ();
		}

        set_blocksRaycasts();

    }

    void set_blocksRaycasts()
    {
        if (!auto_set_block_raycasts) return;

        if (canvas_group.alpha < 0.02) canvas_group.blocksRaycasts = false;
        else canvas_group.blocksRaycasts = true;
    }
}

public static class CoroutineUtil
{
	public static IEnumerator WaitForRealSeconds(float time)
	{
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + time)
		{
			yield return null;
		}
	}
}