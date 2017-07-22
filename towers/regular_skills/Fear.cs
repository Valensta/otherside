using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;

public class Fear : Modifier {
	public AI my_ai;	
	Transform finish;
	Transform start;
	
	float my_time;
	float lifetime;
    public string lava_name = "Wishes/fear_lava";
    Lava my_lava;

    public float Init(HitMe _hitme, float[] stats)
    {
        float _lifetime = stats[0];
        float little_more = 0.1f;//otherwise it looks like it ends too soon because some of them take time to turn around
        if (is_active)
        {
            lifetime = _lifetime;
            my_time = 0f;
            _hitme.EnableVisuals(MonsterType.Fear, lifetime + little_more);
            if (my_lava != null) my_lava.lifespan = lifetime;
            return 0.15f;
        }

        my_ai = _hitme.my_ai;
        finish = my_ai.player.transform;
        start = WaypointMultiPathfinder.Instance.paths[my_ai.path].start.transform;

        my_time = 0;
        is_active = true;

        lifetime = _lifetime;
        my_ai.player = start;
        my_ai.getNewPath();
        my_ai.forward_only = false;
        _hitme.EnableVisuals(MonsterType.Fear, lifetime + lifetime);

        float finisher_percent = (stats.Length == StaticStat.StatLength(EffectType.Fear,true)) ? stats[3] : 0;
        if (finisher_percent > 0 && UnityEngine.Random.RandomRange(0, 1) < finisher_percent)
        {
         
            CauseMassPanic(stats);
        //    Debug.Log("Fin\n");
        }

        return 0.15f;
    }

    void CauseMassPanic(float[] stats)
    {
        my_lava = Zoo.Instance.getObject(lava_name, false).GetComponent<Lava>();
        
        StatBit[] lava_statbits = new StatBit[1];
        lava_statbits[0] = new StatBit(EffectType.Fear, stats[0], 0, true);
        StatSum lava_stats = new StatSum(1, 0, lava_statbits, RuneType.Vexing);
        my_lava.SetLocation(my_ai.transform, my_ai.transform.position, stats[1], Quaternion.identity);
        my_lava.lifespan = stats[0];
        //not ideal, level is whaaaat
        my_lava.Init(EffectType.Fear, 2, lava_stats, lifetime, true, null);
        my_lava.gameObject.SetActive(true);
    }

    protected override void SafeDisable()
    {
        if (my_lava != null) {
            my_lava.KillMe();
            my_lava = null;
        }

        if (my_ai.player != finish)
        {
            my_ai.player = finish;
            my_ai.forward_only = true;
            is_active = false;
            if (!my_ai.dead) my_ai.getNewPath();

        }
    }

    protected override void YesUpdate () {
		
		my_time += Time.deltaTime;
		
		
		if (my_time >= lifetime)
        {
            SafeDisable();
		}				
	}


    
}
