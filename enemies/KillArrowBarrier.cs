using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class KillArrowBarrier : MonoBehaviour {

	
	
	void OnTriggerEnter2D(Collider2D c){
     //   Debug.Log("Kill arrow barrier got a hit\n");
		DoTheThing(c);
		
	}

    void OnCollisionEnter2D(Collision2D c)
    {
        //   Debug.Log("Kill arrow barrier got a hit\n");
        DoTheThing(c.collider);

    }

    public void DoTheThing(Collider2D other){
		        
        if (other.gameObject.layer == Get.flyingProjectileLayer || other.gameObject.layer == Get.regularProjectileLayer) {		
			Arrow arrow = other.GetComponent<Arrow>();
            arrow.MakeMeDie(true);
        }
       
	}
    
}
