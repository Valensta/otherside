using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class ReturnInvisible : MonoBehaviour {
	public GameObject parent;
  

	void OnBecameInvisible(){
 
		if (parent != null){
			Peripheral.Instance.zoo.returnObject(parent);
		}else{
			Peripheral.Instance.zoo.returnObject(this.transform.parent.gameObject);
		}
	}
	
}
