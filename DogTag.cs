using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

[System.Serializable]

public class DogTag : MonoBehaviour {
	
	public float my_time;
	
	public void Init(){
		my_time = 0f;
	}
	
	public float getID(){
		return my_time;
	}

	void Update(){
		my_time += Time.deltaTime;
	}
}