using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class End : MonoBehaviour {
//	public GameObject actor;
	void Start()
	{
	
	}

	void OnTriggerEnter2D(Collider2D c){
	//	Debug.Log ("END REACHED\n");
		Collider2D other = c;
		if (other.tag == "Enemy" ){//&& other.GetComponentInChildren<Body> ()){ //why was this added??
			//actor.GetComponent<Peripheral>().max_dreams -= (int)Mathf.Ceil(other.GetComponent<Actor> ()./3);
			other.tag = "EnemyWon";
			
		//	Debug.Log("EMENY reached the castle!\n");
			HitMe my_hitme = other.attachedRigidbody.gameObject.GetComponent<HitMe>();
			
			
			my_hitme.DieSpecial();
		//	Debug.Log ("DAMAGE " + damage + "\n");
			Peripheral.Instance.AdjustHealth(-1);
            GameStatCollector.Instance.CastleInvaded(my_hitme.gameObject.name);
		}else{
			
		}
	}

}