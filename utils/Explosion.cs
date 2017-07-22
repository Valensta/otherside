using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
	//public GameObject explosion;
	float maxlife;
	// Use this for initialization
	void OnEnable () {
		maxlife = this.GetComponentInChildren<ParticleSystem> ().duration;
	//	Debug.Log("Explosion got maxlife " + maxlife + "\n");
	}
	
	// Update is called once per frame
	void Update () {
		maxlife -= Time.deltaTime;
		if (maxlife <= 0) {
		//	Debug.Log("destroying " + this.name);
			Peripheral.Instance.zoo.returnObject(this.gameObject);
		}
	}
}
