using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;


public class Persistent : MonoBehaviour {

	void Awake(){
		DontDestroyOnLoad (transform.gameObject);
	}

}