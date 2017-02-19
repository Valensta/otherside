using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaitNDestroy : MonoBehaviour {

	public float waitTime = 10f;
    float interval = 1f;
    public float my_time;

	void OnEnable () 
	{
        my_time = waitTime;
		StartCoroutine ("DisableMe");	
	}

	IEnumerator DisableMe(){
        while (my_time > 0)
        {
            yield return new WaitForSeconds(interval);
            my_time -= interval;
        }
		Peripheral.Instance.zoo.returnObject (this.gameObject);
		//this.gameObject.SetActive(false);
		yield return null;
	}	
}