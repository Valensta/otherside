using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
/*
public class EndBody : Body {




	void Start()
	{
	
	}
	void OnTriggerEnter(Trigger c){

//	void OnTriggerEnter(Collider other){
	//	Debug.Log (this.name + " hit by " + other.name);
	if (!end) {				
		if ((other.tag == "PlayerArrow" && this.tag == "Enemy") || (other.tag == "EnemyArrow" && this.tag == "Player")) {
								//		Debug.Log (this.tag + "BODY SAW A " + other.tag);
							//	Destroy (other.gameObject);
				other.GetComponent<Arrow>().active = false;
				other.GetComponent<Arrow>().myTarget = null;
								actor.GetComponent<Actor> ().HurtMe (other.GetComponent<Arrow> ().type, other.GetComponent<Arrow> ().Mass);


			}


				} else {
				//Debug.Log("End reached by " + other.tag + " and it is a body ( " + other.GetComponentInChildren<Body> () + " )" );
				if (other.tag == "Enemy" ){//&& other.GetComponentInChildren<Body> ()){ //why was this added??
					//actor.GetComponent<Peripheral>().max_dreams -= (int)Mathf.Ceil(other.GetComponent<Actor> ()./3);
						other.tag = "EnemyWon";
				Debug.Log("EMENY reached the castle!\n");
						other.attachedRigidbody.gameObject.GetComponent<Actor>().DieSpecial();
						Peripheral.Instance.health --;
					}
				}


	}
	
}
*/