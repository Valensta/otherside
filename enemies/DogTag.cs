using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

[System.Serializable]

public class DogTag : MonoBehaviour {
	
	public float my_time;
    public string my_name;
    public int wave;
        
	public void Init(){
		my_time = 0f;
	}
	
    public string getLabel()
    {
        return my_name;
    }

	public float getID(){
		return my_time;
	}

	void Update(){
        if (Time.timeScale == 0) return;
        my_time += Time.deltaTime;
	}
}