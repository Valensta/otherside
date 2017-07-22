using UnityEngine;
using System.Collections;

public class Stun : MonoBehaviour {
	public AI my_ai;
	public float lifetime;
	float my_time;
	float init_speed;
	bool start;
	bool am_enabled;
	
	
	
	public void Init(AI _ai, float aff, float _lifetime){
		my_ai = _ai;
		lifetime = _lifetime;
		my_time = 0;		
		start = false;
		am_enabled = true;
		init_speed = my_ai.speed;
		
		
		my_ai.speed = init_speed*(1f- 0.5f/Get.MaxLvl(EffectType.Speed));
	//	Debug.Log("Mass changed from " + init_mass + " to " + my_rigidbody.mass + "\n");
	
		//old logic
		//float max = Get.MaxLvl (EffectType.Speed);
		//float new_speed = my_ai.speed - init_speed*aff/max;
		
	
	}
	
	
	void Update () {
		if (!am_enabled){return;}
		my_time += Time.deltaTime;
		if (!start){
			if (my_time > lifetime)
			{start = true;}
			else {return;}
		}
		
		if (init_speed - my_ai.speed <= 2*Time.deltaTime/lifetime){
			my_ai.speed = init_speed;
			am_enabled = false;
			return;
		}		
		my_ai.speed += 2*Time.deltaTime/lifetime;		
	//	Debug.Log("Mass " + my_rigidbody.mass + "\n");
		
		
		
	}
}
