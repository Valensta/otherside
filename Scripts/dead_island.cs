using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class dead_island : MonoBehaviour {
	
    float interval = 0.25f;
    public float my_time;
    Island_Button my_button;

	public void EnableMe(float timer, Island_Button island) 
	{
        my_button = island;
        my_time = timer;
		StartCoroutine ("DisableMe");
        this.transform.parent = island.transform;
	}

	IEnumerator DisableMe(){
        while (my_time > 0)
        {
            yield return new WaitForSeconds(interval);
            my_time -= interval;
        }
        my_button.blocked = false;
        my_button = null;
    
		Peripheral.Instance.zoo.returnObject (this.gameObject);
		
		yield return null;
	}	
}