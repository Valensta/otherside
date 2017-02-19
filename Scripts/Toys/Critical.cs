using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Critical: MonoBehaviour {

    float min_multiplier;
    float max_multiplier;
    float percent_hit;
    float base_mass;
    float finisher_percent;
	
	public void Init(float[] stats, float mass){
        min_multiplier = stats[0];
        max_multiplier = stats[1];
        percent_hit = stats[2];
        base_mass = mass;

        finisher_percent = (stats.Length == 4) ? stats[3] : 0;
        

    }
		
	public float getCriticalForce()
    {
        float random = UnityEngine.Random.Range(0, 1f);
      //  Debug.Log("Random " + random + " < " + percent_hit + "?");
        if (random > percent_hit) return base_mass;


        float mult_random = UnityEngine.Random.Range(min_multiplier, max_multiplier);
        //   Debug.Log("Critical damage mult is " + mult_random + "\n");
        if (finisher_percent > 0 && UnityEngine.Random.RandomRange(0, 1) < finisher_percent)
        {
            mult_random = 999f;
        }

        return base_mass * (1 + mult_random);
    }
		

}
