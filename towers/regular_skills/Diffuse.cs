using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Diffuse : MonoBehaviour {
    public Lava lava;	
	public Arrow arrow;		
    StatSum stats;    

    public float Init(StatSum stats)
    {
        this.stats = stats;
        return stats.GetStatBit(EffectType.Diffuse).getStats()[3];
    }


	public void MakeDiffuse(Vector3 pos) {
        //    Debug.Log("Diffuse doing diffusion\n");
        //force
        StatBit diffuse_statbit = stats.GetStatBit(EffectType.Diffuse);
        //StatSum lava_statsum_old = stats.cloneAndRemoveStat(EffectType.Diffuse);
        float[] stat_floats = diffuse_statbit.getStats();
        StatSum lava_statsum = new StatSum();
        lava_statsum.runetype = RuneType.Sensible;
        lava_statsum.stats = new StatBit[1];

        StatBit lava_statbit = new StatBit();
        lava_statbit.effect_type = EffectType.Force;
        lava_statbit.base_stat = stat_floats[5];
        lava_statbit.rune_type = RuneType.Sensible;
        lava_statbit.dumb = true;
        lava_statbit.very_dumb = true;

        lava_statsum.stats[0] = lava_statbit;
        
        float range = stat_floats[0];
        float lifespan = stat_floats[1];
        float factor = stat_floats[2];

        //this statbit is not dumb. Diffuse just diffuses whatever Vexing force the arrow has.

        //get your own lava since lavas live for much longer than arrows, arrows get reused much faster.
        //each arrow does not have its own lava
        lava = Zoo.Instance.getObject("Wishes/diffuse_lava", false).GetComponent<Lava>();

        lava.SetLocation(null, pos, range, Quaternion.identity);
        lava.gameObject.SetActive(true);
     //   Debug.Log("Diffuse lava lifetime " + lifespan + "\n");
        lava.Init(EffectType.Diffuse, diffuse_statbit.level, lava_statsum, lifespan, true, arrow.myFirearm);
        lava.SetFactor(factor);
        

		arrow.MakeMeDie(false);
        
	}
	
	

}
