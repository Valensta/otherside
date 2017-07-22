using UnityEngine;
using System.Collections;

public class RapidFire : MonoBehaviour {	
	int arrows;  //fire how many in a row
	//float speed_multiplier = 0.05f;
    float mass_multiplier = 0.85f;
    float mass;     //arrow stats
	float speed; //arrow stats
	float period; // time between groups of shots   |- - -           |- - -      
	float delta = 0.2f; //time between individual shots, constant
                        //	float TIME = 0;
    int level;
    float aff;
	float current_arrow = 0;
    float damage_multiplier;
    
	//public void Init(float _aff, float _period, float _speed, float _mass){
    public void Init(float[] stats, float _mass, int level)
    {
        //multiplier = aff;
        //current_arrow = 0;
        aff = stats[0];
		arrows = (int)Mathf.Floor(stats[0]);
        speed = stats[2];
        mass = stats[1]*_mass;
        period = stats[3];
        damage_multiplier = stats[5];
        this.level = level;
        
	//	Debug.Log("Rapid fire arrow speed " + _speed + " -> " + speed + "\n");
			
	}
	
    
    public void modifyArrow(Arrow arrow)
    {
        arrow.speed = GetSpeed();
//        arrow.type.factor = damage_multiplier;

  
        StatBit vf = arrow.type.GetStatBit(EffectType.VexingForce);
        vf.dumb = true;
        vf.level = level;        
        vf.Base_stat *= damage_multiplier;
        vf.very_dumb = true;



    }

	public float GetSpeed(){
        //Debug.Log(" aff is " + aff + " speed is " + speed + " to " + (0.5f + 0.1f * aff) + "\n");
        return speed;
    }
	


	public float GetMass(){		//affects damage
		return mass;
	}
		
	public float GetTimeToNextArrow(){
        current_arrow++;
        if (current_arrow < arrows){ 
	//	Debug.Log("current_arrow FIRE " + current_arrow + "\n");

			return delta;
		}else{	
			current_arrow = 0;
			return period - delta*(arrows);
		}
	}
	
}
