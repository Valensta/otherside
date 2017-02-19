using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[System.Serializable]
public class Pool{

	public string name;
	public int count;
    public bool non_refundable; //returnAll will not return these guys, they are not meant to be reused
	
	[System.NonSerialized]
	public List<GameObject> pool = new List<GameObject>();
	public string obj_name;

	public Pool(string n, int c){
		name = n;
		count = c;
	}

	public Pool(){
		name = "blah";
		count = 0;
	}
	
	public void SetName(string n){
		obj_name = n;
	}
	
}


[System.Serializable]//why the fuck do I have this class
public class Poolette{
	public string name;
	public int count;
	public int order;
    public bool non_refundable;
	public List<GameObject> objects = new List<GameObject>();

}

