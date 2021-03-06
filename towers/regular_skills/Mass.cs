﻿using UnityEngine;
using System.Collections;

public class Mass : Modifier {
	public Rigidbody2D my_rigidbody;
	public float lifetime;
	float my_time;
	float init_mass;
	bool start;
    bool am_dead = false;
    Transform scale_me;
	
	public HitMeStatusBar my_status_bar;
    Peripheral my_peripheral;
	float mass_factor;
    float cumulative_damage;
    
    //float cumulative_xp = 0f;
	
    public bool amIDead()
    {
        return am_dead;
    }

	public bool IsHurt(){
	//	Debug.Log("hurt? " + my_rigidbody.mass/init_mass + "\n");
		if (my_rigidbody.mass/init_mass < 0.99f) return true;
		return false;
	}
	
	public void Define(Rigidbody2D _rigidbody, Transform parent, float factor, Vector3 _status_bar_location, Transform scale_me){
		my_rigidbody = _rigidbody;
		init_mass = my_rigidbody.mass;
		mass_factor = factor;

        my_status_bar.Init(init_mass / mass_factor);
        SetPosition(_status_bar_location);
        UpdateMass(init_mass);
        this.scale_me = scale_me;
	}
	
	public void SetPosition(Vector3 _status_bar_location){
	//Debug.Log("Setting position for " + my_status_bar.transform.parent.transform.parent.name + "\n");
	
		my_status_bar.transform.localPosition = _status_bar_location;
		my_status_bar.transform.localRotation = Quaternion.identity;
	}
	
	public float Init(float[] stats){
        
        float aff = stats[0];
        float _lifetime = stats[1]; 
        if (my_peripheral == null) my_peripheral = Peripheral.Instance;

        //aff /= 4f; //for balancing purposes
        aff /= Get.bullshit_damage_factor; //for balancing purposes but now make it harder

        lifetime = _lifetime;
		my_time = 0;		
		start = false;
        is_active = false;
		if (_lifetime > 0){
            is_active = true;
		}
        if (aff > 0) aff *= my_peripheral.damage_factor.getStat();

      //  Debug.Log("hurt for " + aff + "\n");
        float new_mass =  my_rigidbody.mass - aff;

		UpdateMass(new_mass);
                
        float actual_damage = (new_mass < init_mass / 2f ) ? aff + (new_mass - init_mass / 2f) : aff;

   //     Debug.Log(Duration.realtimeSinceStartup + "," + aff + "\n");

        float xp = actual_damage / (init_mass / 2f);
       if (xp <= 0 && aff > 0)
        {
        //    Debug.Log("WHAT aff " + aff + " act dmg " + actual_damage + " / " + (init_mass/2f)  + " \n");
            return 0;
        }
          //  Debug.Log("Mass hurt for " + aff + " xp is " + xp + " " + Sun.Instance.current_time_of_day + "\n");
        return xp;//because they die once mass = init_mass/2f
	}

 
    protected override void YesUpdate()
    {
        

        my_time += Time.deltaTime;
        if (!start)
        {
            if (my_time > lifetime)
            { start = true; }
            else { return; }
        }


        float delta = 2 * Time.deltaTime / lifetime;
        if (init_mass - my_rigidbody.mass <= delta)
        {
            UpdateMass(init_mass);          
            is_active = false;
            return;
        }

        //		Debug.Log("delta " + delta);		
        UpdateMass(my_rigidbody.mass + delta);

    }

    protected override void SafeDisable()
    {
    }

    public void setPercentMass(float percent)
    {
        float new_mass = init_mass * percent;
        UpdateMass(new_mass);
    }


    public float getPercentMass()
    {
        return my_rigidbody.mass/init_mass;
        
    }



    void UpdateMass(float m){
		if (m > init_mass)return;
        cumulative_damage += m - my_rigidbody.mass;
        //Debug.Log("Update mass " + m + " " + Time.realtimeSinceStartup + "\n");


        my_rigidbody.mass = m;



        am_dead = m / init_mass < 0.5f;
      
        my_status_bar.UpdateStatus(m-init_mass/mass_factor);
        if (scale_me != null) scale_me.localScale = (m/init_mass)*Vector3.one;


    }

    
}
