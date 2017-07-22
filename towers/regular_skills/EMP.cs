using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EMP: MonoBehaviour {

    
	
	public void Init(float[] stats, Modifier[] enemyTech, bool foil){
        float disable_time = (foil)? stats[0] : stats[2];
        
     	foreach(Modifier et in enemyTech)
        {
         //   Debug.Log("EMP is disabling " + et.name + " for " + disable_time + "\n");
            et.Disrupt(disable_time);
        }
		
	}
		
		

}
