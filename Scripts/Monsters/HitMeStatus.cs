using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class HitMeStatus : MonoBehaviour {

	float max;
	float current;

	public void Init(float m)
	{
		max = m;
	}

	public void UpdateStatus(float c){
		current = c;
		
	}
	
}
