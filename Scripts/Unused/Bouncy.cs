using UnityEngine;
using System.Collections;

public class Bouncy : MonoBehaviour {
	public Collider my_collider;
	public float lifetime;
	float my_time;
	bool start;
	public bool _enabled;

	
	public void Init(Collider _collider, float aff, float _lifetime){
		my_collider = _collider;
		lifetime = _lifetime;
		my_time = 0;		
		start = false;
		_enabled = true;
		
		float new_bounciness = 0.6f + aff/10f;
		my_collider.material.bounceCombine = PhysicMaterialCombine.Maximum;
		my_collider.material.bounciness = new_bounciness;			
	}
	

	void Update () {
		if (!_enabled){return;}
		my_time += Time.deltaTime;
		if (!start){
			if (my_time > lifetime)
			{start = true;}
			else {return;}
		}
		
		if (my_collider.material.bounciness <= 2*Time.deltaTime/lifetime){
			my_collider.material.bounciness = 0;
			_enabled = false;
			return;
		}		
		my_collider.material.bounciness -= 2*Time.deltaTime/lifetime;		
		
		
		
		
	}
}
