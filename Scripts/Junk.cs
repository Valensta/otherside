using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Junk : MonoBehaviour {
	

	 void OnTransformChildrenChanged(){
		int children = transform.childCount;
		
		for (int i = children - 1; i > 0; i--)
		{
			GameObject.Destroy(transform.GetChild(i).gameObject);
		}


	}







}