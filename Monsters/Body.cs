using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class Body : MonoBehaviour {

	public GameObject hitme;
	public HitMe my_hitme;
	bool is_active = true;

    public delegate void onXpAddedHandler(float xp, Vector3 pos);
    public static event onXpAddedHandler onXpAdded;

    void Start(){
		is_active = true;
		my_hitme = hitme.GetComponent<HitMe> ();
	}
	
	public void AmActive(bool a){
		is_active = a;
	}

	void OnTriggerEnter2D (Collider2D  other){
   //     Debug.Log("Body registered a trigger\n");
    }
	
	
	
	void OnCollisionEnter2D(Collision2D c){
        
		DoTheThing(c.collider);
		
	}
	
	
	public float DoTheThing(Firearm firearm, StatSum stats){
		if (!is_active) return 0;
		float xp = my_hitme.HurtMe (stats);
        float return_xp = 0f;//if tower is at max xp, return the xp
        return_xp = firearm.addXp(xp);
        if (onXpAdded != null) onXpAdded(xp - return_xp, this.transform.position);
        if (return_xp > 0) my_hitme.stats.returnXp(return_xp);
        return xp;
	}
	
	public void DoTheThing(Collider2D other){
		if (!is_active) return;
        
        if (!other.name.Contains ("End")             
		    && ((other.tag == "PlayerArrow" && this.tag == "Enemy") || (other.tag == "EnemyArrow" && this.tag == "Player"))
		    ) {		
			Arrow arrow = other.GetComponent<Arrow>();
            if (my_hitme.gameObject.GetInstanceID() == arrow.sourceID) return;
            arrow.myTarget = null;
            Vector3 pos = this.transform.position;
			float xp = my_hitme.HurtMe (arrow.type);
         //   if (xp > 0) Debug.Log("Xp " + xp + " from " + this.gameObject.name + "\n");
              
            float return_xp = 0f;//if tower is at max xp, return the xp
			if (arrow.myFirearm != null) return_xp = arrow.myFirearm.addXp(xp);            
            if (return_xp > 0) my_hitme.stats.returnXp(return_xp);
            if (onXpAdded != null) onXpAdded(xp - return_xp, pos);
            if (Noisemaker.Instance != null)Noisemaker.Instance.Play("arrow_hit_monster");
            arrow.RegisterHit(pos);         

        }	
	}
    
}
