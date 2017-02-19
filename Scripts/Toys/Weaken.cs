using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weaken : Modifier{
    public HitMe my_hitme;
    public float lifetime;
    float my_time;
    float init_speed = -1;
    bool start;
    float finisher_percent;

    bool stun;
    List<Defense> original_defenses;

    //weaken applies to defenses that are both in hitme.defenses and in hitme.init_defenses, won't apply to secondary defenses that may be added later on, like Fear.
    public float Init(HitMe _hitme, float[] stats) {//, float aff, float _lifetime){
		my_hitme = _hitme;
		
		my_time = 0;		
		start = false;
		is_active = true;

        float by = stats[0];
        lifetime = stats[1];

        _hitme.EnableVisuals(MonsterType.Weaken, lifetime);

        original_defenses = new List<Defense>();
    //    Debug.Log("Initialized weaken for " + by + " percent and " + lifetime + " time\n");

        finisher_percent = (stats.Length == 3) ? stats[2] : 0;
        if (finisher_percent > 0 && UnityEngine.Random.RandomRange(0, 1) < finisher_percent)
        {
            by = 0.05f;
        }

        foreach (Defense def in my_hitme.defenses) {
            Defense init = null;
            foreach (Defense init_def in my_hitme.init_defenses)
            {
                if (def.type == init_def.type)
                {
                    original_defenses.Add(new Defense(init_def.type, init_def.strength));
                    init = init_def;
                }
            }
         
			if (init != null) def.strength = init.strength*by; 
		}		
		return by;        
        

    }

    private void _ReturnToNormal()
    {
        foreach (Defense d in original_defenses)
        {
            my_hitme.SetDefense(d.type, d.strength);
        }
    }

    protected override void SafeDisable()
    {
        _ReturnToNormal();

    }
    protected override void YesUpdate () {
		
		my_time += Time.deltaTime;
		
		if (my_time > lifetime)
		{

            _ReturnToNormal();		
													
			is_active = false;
			return;
		}		
			
	}
}
