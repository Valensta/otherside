using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class RotateMe : MonoBehaviour {



	public GameObject me;
	
	public float speed;
	
	
	void Start(){
		//this.transform.localPosition = Vector3.zero;
		
	}

	void Update(){
		this.transform.Rotate(0, 0, Time.deltaTime*speed);
		
	}
	

}
