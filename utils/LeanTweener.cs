using UnityEngine;
using System.Collections;

public class LeanTweener : MonoBehaviour {
	public GameObject target;

	public float delay = 0f;
	public LeanTweenType leantweentype = LeanTweenType.notUsed;
	public int pingpong = -99;
	public TweenType type = TweenType.Null;
	public float time = 2f;
	public Vector3 vector;
	public Vector3 init_vector;
	public Color my_color;	
	public float value = 0f;
    public bool auto_tween_on_enable = false;
    public LeanTweenerPreset preset = LeanTweenerPreset.Null;
    public bool ignoreTimeScale = false;
    bool tweening = false;

	public float duration = -99f;
	
	
	public enum TweenColor{Force, Scale, Speed, Collided, Select, Hint, Disabled};
	public enum TweenType{Scale, Rotate, ColorChange, Null};

	public void Awake(){
		//Init ();
	}


	public void Init () {
     //   Debug.Log("Initializing " + target.gameObject.name + "\n");
        if (target == null){Debug.Log("Missing target for " + this.gameObject.name + " LeanTweener\n"); return;}
		LTDescr l = null;
        LeanTween.cancel(target);
        switch (type)
        {
            case TweenType.Scale:
                target.transform.localScale = init_vector;
                l = LeanTween.scale(target, vector, time).setIgnoreTimeScale(ignoreTimeScale);
                break;
            case TweenType.Rotate:
                l = LeanTween.rotateZ(target, value, time).setIgnoreTimeScale(ignoreTimeScale);
                break;
            case TweenType.ColorChange:
                l = LeanTween.color(target, my_color, time).setIgnoreTimeScale(ignoreTimeScale);
                break;
            default:
                return;
        }

        tweening = true;
		if (delay > 0){l.setDelay(delay);}
		if (leantweentype != LeanTweenType.notUsed){l.setEase(leantweentype);}
		if (pingpong > -99){l.setLoopPingPong(pingpong);}
		
		
		
		
		if (duration != -99){
			StartCoroutine(StopMeSoon());
		}
	}

	public void StopMeNow(){
        if (!tweening) return;
        
    //    Debug.Log("Stopping " + target.gameObject.name + "\n");
    
		duration = -99f;
        LeanTween.cancel(target);
        tweening = false;
	}
		
	IEnumerator StopMeSoon(){
        if (!tweening) yield return null;
        if (duration > 0.05f) {
			duration = - 0.05f;
			
			yield return new WaitForSecondsRealtime(0.1f);
		}else{
            StopMeNow();
		}
		yield return null;
		
	}

    void OnEnable()
    {
        switch (preset)
        {
            case LeanTweenerPreset.Null:
                break;
            case LeanTweenerPreset.DefaultFastButton:
                duration = -99;
                time = 0.2f;
                leantweentype = LeanTweenType.easeInCirc;
                delay = 0f;
                type = TweenType.Scale;
                pingpong = -1;
                break;
            case LeanTweenerPreset.DefaultSlowButton:
                duration = -99;
                time = 0.35f;
                leantweentype = LeanTweenType.easeInCubic;
                delay = 0f;
                type = TweenType.Scale;
                pingpong = -1;
                break;
	        case LeanTweenerPreset.DefaultSlowButtonAlpha:
		        duration = -99;
		        time = 0.35f;
		        leantweentype = LeanTweenType.easeInCubic;
		        delay = 0f;
		        type = TweenType.ColorChange;
		        pingpong = -1;
		        break;
            case LeanTweenerPreset.GentleSlow:
                duration = -99;
                time = 0.35f;
                leantweentype = LeanTweenType.easeInOutSine;
                delay = 0f;
                type = TweenType.Scale;
                pingpong = -1;
                break;
	            
	        case LeanTweenerPreset.GentleSlowAlpha:
		        duration = -99;
		        time = 0.7f;
		        leantweentype = LeanTweenType.easeInCubic;
		        delay = 0f;
		        type = TweenType.ColorChange;
		        pingpong = -1;
		        break;
            case LeanTweenerPreset.UIBlip: //quick blip for a ui item like health or dream status
                duration = 0.3f;
                time = 0.3f;
                leantweentype = LeanTweenType.easeInQuad;
                delay = 0f;
                type = TweenType.Scale;
                pingpong = 1;
                break;
            case LeanTweenerPreset.UI3Blips: //quick blip for a ui item like health or dream status   // does not actually work
                duration = 0.9f;
                time = 0.3f;
                leantweentype = LeanTweenType.easeInQuad;
                delay = 0.1f;
                type = TweenType.Scale;
                pingpong = -1;
                break;
        }

        if (!auto_tween_on_enable) return;
        Init();
    }

    void OnDisable()
    {
        StopMeNow();

    }
}
