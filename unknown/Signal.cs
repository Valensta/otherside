using UnityEngine;
using System.Collections;

public class SignalRetired : MonoBehaviour {
	public float duration;
	public bool temporary;

	// Use this for initialization
	void Start () {
//		Debug.Log ("Signal started\n");
		if (temporary) {
						StartCoroutine (WaitAndDie (duration));
				}
	//	Tweener tweenme = GetComponentInChildren<Tweener> ();

	//	if (tweenme != null)
	//					tweenme.TweenMe (Tweener.TweenType.ColorLoop, "select");

	}

	IEnumerator WaitAndDie(float seconds){
		yield return new WaitForSeconds(seconds);
		Peripheral.Instance.zoo.returnObject(this.gameObject);
		
	}







}