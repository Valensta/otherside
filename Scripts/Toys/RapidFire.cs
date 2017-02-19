using UnityEngine;
using System.Collections;

public class RapidFire : MonoBehaviour {	
	int arrows;  //fire how many in a row
	float speed_multiplier = 0.05f;
    float mass_multiplier = 0.85f;
    float mass;     //arrow stats
	float speed; //arrow stats
	float period; // time between groups of shots   |- - -           |- - -      
	float delta = 0.3f; //time between individual shots, constant
                        //	float TIME = 0;
    float aff;
	float current_arrow;
	
	//public void Init(float _aff, float _period, float _speed, float _mass){
    public void Init(float[] stats, float _speed, float _mass)
    {
        //multiplier = aff;

        aff = stats[0];
		arrows = (int)Mathf.Floor(stats[0]);
        speed = stats[2]*_speed;
        mass = stats[1]*_mass;
        period = stats[3];
	//	Debug.Log("Rapid fire arrow speed " + _speed + " -> " + speed + "\n");
			
	}
	
	public float GetSpeed(){
        //Debug.Log(" aff is " + aff + " speed is " + speed + " to " + (0.5f + 0.1f * aff) + "\n");
        return speed;
    }
	


	public float GetMass(){		//affects damage
		return mass;
	}
		
	public float GetTimeToNextArrow(){
		if (current_arrow < arrows){ 
	//	Debug.Log("current_arrow FIRE " + current_arrow + "\n");
			current_arrow++;
			return delta;
		}else{	
			current_arrow = 0;
			return period - delta*(arrows);
		}
	}
	
}
