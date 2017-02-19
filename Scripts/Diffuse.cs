using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Diffuse : MonoBehaviour {
    public Lava lava;	
	public Arrow arrow;		
    StatSum stats;

    public void Init(StatSum stats)
    {
        this.stats = stats;
    }


	public void MakeDiffuse(Vector3 pos) {
        //    Debug.Log("Diffuse doing diffusion\n");
        
        StatSum lava_statsum = stats.cloneAndRemoveStat(EffectType.Diffuse);
        float[] stat_floats = stats.GetStatBit(EffectType.Diffuse).getStats();
        float range = stat_floats[0];
        float lifespan = stat_floats[1];
        float factor = stat_floats[2];

        lava.SetLocation(null, pos, range, Quaternion.identity);
        lava.Init(lava_statsum, lifespan, true, arrow.myFirearm);
        lava.SetFactor(factor);
        

        //Debug.Log("initializing diffuse " + stats.GetStatBit(EffectType.Diffuse).Level + " stat " + stats.GetStatBit(EffectType.Diffuse).stat +  " with range " + range + " final factor " + lava.my_stats.factor + "\n");

        
        lava.gameObject.SetActive(true);

		arrow.MakeMeDie();
        
	}
	
	

}
