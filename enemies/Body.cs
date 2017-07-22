using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;
public class Body : MonoBehaviour {

	public GameObject hitme;
	public HitMe my_hitme;
	bool is_active = true;

    public delegate void onXpAddedHandler(float xp, Vector3 pos);
    public static event onXpAddedHandler onXpAdded;

    public delegate void onCheckCastleDistanceHandler(Vector3 pos, string label);
    public static event onCheckCastleDistanceHandler onCheckCastleDistance;
    int defaultLayer = -1;
    bool am_hidden = false;
    
    public bool amHidden()
    {
        return am_hidden;
    }
        

    void Start(){
		is_active = true;
		if (my_hitme == null) my_hitme = hitme.GetComponent<HitMe> ();
        if (defaultLayer < 0)
        {
            defaultLayer = gameObject.layer;
        }
        else
        {
            gameObject.layer = defaultLayer;
        }
        am_hidden = false;
        
	}
	
	public void AmActive(bool a){
		is_active = a;
	}

    private void OnTriggerEnter2D(Collider2D c)
    {
        if (!Monitor.Instance.enable_overpasses || defaultLayer == Get.flyingMonsterLayer) return;    

        bool new_hidden = (c.gameObject.layer == Get.overpassLayer);
        if (am_hidden != new_hidden)
        {
            am_hidden = new_hidden;
            gameObject.layer = Get.hiddenMonsterLayer;
        }
    }

    private void OnTriggerExit2D(Collider2D c)
    {
        if (!Monitor.Instance.enable_overpasses || defaultLayer == Get.flyingMonsterLayer) return;
        
        bool unhide_me = (c.gameObject.layer == Get.overpassLayer);
        if (am_hidden && unhide_me)
        {
            am_hidden = false;
            gameObject.layer = defaultLayer;
        }
    }


    void OnCollisionEnter2D(Collision2D c){        
		DoTheThing(c.collider);
       
    }
	
	
	public void DoTheThing(Firearm firearm, StatSum stats){
		if (!is_active) return;
        if (am_hidden) return;
        //Laser
        if (my_hitme == null) { Debug.Log("No hitme\n"); return; }
        if (stats == null) Debug.Log("No stats\n");
	    if (firearm== null) Debug.Log("No firearm\n");
	    if (firearm.current_arrow_name == null) Debug.Log("No current arrow now\n");
        try
        {
            my_hitme.HurtMe(stats, firearm, EffectTypeOverride(firearm.current_arrow_name.type));
        }catch (NullReferenceException e)
        {
            Debug.LogError($"Something is null: stats {stats == null} firearm {firearm == null} my_hitme {my_hitme == null} firearm.current_arrow_name {firearm?.current_arrow_name == null}");
        }
               
	}
	
    public EffectType EffectTypeOverride(ArrowType arrow)
    {
        if (arrow == ArrowType.Sparkle) return EffectType.Sparkles;
        else if (arrow == ArrowType.Focus) return EffectType.Focus;
        else if (arrow == ArrowType.Critical) return EffectType.Critical;
        else if (arrow == ArrowType.RapidFire) return EffectType.RapidFire;
        
        return EffectType.Null;    
    }

	public void DoTheThing(Collider2D other){
		if (!is_active) return;
        if (am_hidden) return;
        
        if (!other.name.Contains ("End")             
		    && ((other.tag.Equals("PlayerArrow") && this.tag.Equals("Enemy")))
		    ) {		
			Arrow arrow = other.GetComponent<Arrow>();
            if (my_hitme.gameObject.GetInstanceID() == arrow.sourceID) return;
            arrow.myTarget = null;
            Vector3 pos = this.transform.position;

            //int primary_level = (arrow.arrow_type == ArrowType.Sparkle) ? arrow.type.level : -1;
            //for sparkles, use level of sparkes, works like lava.
            my_hitme.HurtMe(arrow.type, arrow.myFirearm, EffectTypeOverride(arrow.arrow_type));
            if (arrow.arrow_type == ArrowType.Fast)
            {
                if (onCheckCastleDistance != null) { onCheckCastleDistance(pos, my_hitme.my_ai.my_dogtag.getLabel()); }
             
            }
            if (Noisemaker.Instance != null)Noisemaker.Instance.Play("arrow_hit_monster");
            arrow.RegisterHit(pos);         

        }	
	}
    
}
