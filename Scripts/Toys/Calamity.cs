using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class Calamity : MonoBehaviour {
    public EffectType type;

    Transform my_target;
    Dictionary<string, HitMe> monsters = null;	
	bool am_enabled = false;	
    float lava_timer;
    public Lava my_lava;
    bool lava_active;
    float make_new_lava_timer = 0f;
    
	
		
	public delegate void OnLavaBurnHandler(EffectType type);
	public static event OnLavaBurnHandler OnLavaBurn; 

	
    void Update()
    {
        
        if (lava_timer > 0)lava_timer -= Time.deltaTime;
        if (make_new_lava_timer > 0) make_new_lava_timer -= Time.deltaTime;
        if (lava_active && lava_timer < 0)
        {
            lava_active = false;
            my_lava.gameObject.SetActive(false);
            lava_timer = 0;
        }

    }

    public void Init(StatBit bit, Transform target, Firearm firearm)
    {
        StatSum stats = firearm.rune.GetStats(false);
        float[] calamity_stats = bit.getStats();

        if (make_new_lava_timer > 0 || lava_active == true) return;

        my_lava.my_stats.UpdateStatBit(EffectType.Force, calamity_stats[0]);
        float range = stats.getRange();
        //float time = stats.getReloadTime();

        my_lava.transform.localScale = Vector3.one * range * calamity_stats[1];
       // make_new_lava_timer = time * calamity_stats[2];
       // lava_timer = time * calamity_stats[3];
        make_new_lava_timer = calamity_stats[2];
        lava_timer = calamity_stats[3];

        //      Debug.Log("Setting calamity (" + type + ") stats force " + stat.stat + " time " + time + "\n");
        my_lava.my_firearm = firearm;
        my_lava.transform.SetParent(target);
        my_lava.transform.localPosition = Vector3.zero;

        my_lava.gameObject.SetActive(true);
        lava_active = true;

    }
	
	
	
}
