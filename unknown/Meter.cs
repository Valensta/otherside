﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Meter : MonoBehaviour {
	public GameObject meter;
	public GameObject is_full;
	public enum Mode {
		Scale, OneDirectional, Text // Just broadcast the action on to the target


	}
	public Text text;
	public Mode type;
	bool full;
	bool charged;
	float steps = 10;
	public float capacity;
	public float charge;
	private Vector3 scale;

	// Use this for initialization
	void Start () {
		scale = meter.transform.localScale;
		/*
		if(is_full)
			is_full.GetComponentInChildren<UITexture>().material.name = "is_full";
			*/
		SetFull (false);
	//	steps = 0;
	//	Debug.Log ("Setting scale to " + scale);


	}


	public void SetCapacity(float c){
		capacity = c;
	}
	// Update is called once per frame
    /*
	public void SetCharged(bool c){

		charged = c;
	//	Debug.Log("Setting meter " + meter.name + " as charged " + charged + "\n");
		if (charged) {
						float tween_time = 0.3f;
						iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (1, 1, 1),
			                                "time", tween_time,
			                                "looptype", "loop",
			                                "name", "is_charged"
						));
				} else {
		//	Debug.Log("Stopping charged itween\n");
						iTween.Stop(this.gameObject);
				}
		//if(is_full)is_full.GetComponent<UITexture> ().MarkAsChanged ();
	}*/

	public bool GetFull(){
		return full;
	}

	public bool GetCharged(){
		return charged;
	}

	void SetFull(bool f){
		full = f;
	//	Debug.Log("Setting meter " + meter.name + " as full " + full + "\n");
		if (is_full) {
			/*
						if (full) {
								float tween_time = 0.3f;
								iTween.FadeTo (is_full, iTween.Hash ("alpha", 0.5f,
		     					                 "time", tween_time,
			                                     "looptype", "loop",			                                    
		                         		         "NamedColorValue", "_TintColor",
		                                		 "target_material", "is_full",
			                            		 "name", "is_full"			                                  
								));
						} else {
								iTween.Stop (is_full, "is_full", true);
						}
						
						is_full.SetActive (full);
						is_full.GetComponent<UITexture> ().MarkAsChanged ();
						*/
				}
	}


	public void UpdateMeter(float charge){
	//	Debug.Log ("CHARGE " + charge + " capacity " + capacity);
		float percent = charge / capacity;
		float update = percent*steps;
		//percent is actually 0 to 1
	//	Debug.Log ("update is " + update + " charge " + charge);
		if (percent >= 1) {
			percent = 1;
			SetFull (true);
		//	Debug.Log("Setting full for " + this.name);
		} else {
			SetFull(false);
		}
		switch (type){
			case (Mode.Scale):
				meter.transform.localScale = Vector3.one*percent;		
				break;
			case (Mode.OneDirectional):
				Vector3 pos = meter.transform.localPosition;
				pos.y = -50f + update/2f;
				meter.transform.localPosition = pos;		
				meter.transform.localScale = new Vector3(100, update,1);
				break;
			case (Mode.Text):
				text.text = Mathf.RoundToInt(charge).ToString();				
				break;
		}
	}
}
