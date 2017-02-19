using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[System.Serializable]
public class StatSum{
	public int level;
	public float xp;
    public int towerID;
    public StatBit[] stats;
    public RuneType runetype;
	public float factor = 1f;

    public StatSum() { }

	public StatSum(int level, float xp, StatBit[] stats, RuneType runetype){
		this.level = level;
		this.xp = xp;
		this.stats = stats;
		this.runetype = runetype;
	}

    public float getRange()
    {
        StatBit sb = GetStatBit(EffectType.Range);
        if (sb == null) return -2;

        float range = sb.getStats()[0];
    //    Debug.Log("Got range " + range + "\n");
        if (runetype == RuneType.Vexing)
        {            
         
            StatBit extra = GetStatBit(EffectType.Focus);
            if (extra != null && extra.Level > 0) range += extra.getStats()[0];
          //  Debug.Log("Vexing modified range " + range + "\n");
        }

        return range;
    }

    public float getReloadTime()
    {
        return getReloadTime(true);
    }

    public float getReloadTime(bool basic)
    {
        float reload_time = GetStatBit(EffectType.ReloadTime).getStats()[0];
        
        if (!basic && runetype == RuneType.Vexing)
        {
       //     Debug.Log("reload time " + reload_time + "\n");
            StatBit extra = GetStatBit(EffectType.Focus);
            if (extra != null) reload_time += extra.getStats()[2];
            if (extra != null) Debug.Log("reload time " + reload_time + " increased by " + extra.getStats()[2] + "\n");
        }

        return reload_time;
    }

    public float getPrimary()
    {
        switch (runetype)
        {
            case RuneType.Sensible:
                return GetStatBit(EffectType.Force).getStats()[0];
            case RuneType.Airy:
                return GetStatBit(EffectType.Speed).getStats()[0];
            case RuneType.Vexing:
                float damage = GetStatBit(EffectType.VexingForce).getStats()[0];
                StatBit extra = GetStatBit(EffectType.Focus);
                if (extra != null && extra.Level > 0) damage += extra.getStats()[1];
                return damage;
        }
        return 0f;        
    }

    public float[] getModifiedPrimaryStats(float defense)
    {
        
        switch (runetype)
        {
            case RuneType.Sensible:
                return GetStatBit(EffectType.Force).getModifiedStats(factor, defense);
            case RuneType.Airy:
                return GetStatBit(EffectType.Speed).getModifiedStats(factor, defense);
            case RuneType.Vexing:
                
                float[] damage = GetStatBit(EffectType.VexingForce).getModifiedStats(factor, defense);
             //   Debug.Log("Getting primary for vexing lvl " + level + " got " + damage[0] + "\n");
                StatBit extra = GetStatBit(EffectType.Focus);                
                if (extra != null && extra.Level > 0) damage[0] += extra.getModifiedStats(factor, defense)[1];
           //     Debug.Log("Not it is " + damage[0] + " for lvl " + level + "\n");
                return damage;
        }
        return new float[0];
    }


    public StatSum getSubStatSum(EffectType type)
    {
        int actives = 0;
        for (int i = 0; i < stats.Length; i++)
        {
            if (stats[i].effect_type == type) actives++;
        }

        int my_i = 0;
        StatBit[] my_stats = new StatBit[actives];


        for (int i = 0; i < stats.Length; i++)
        {        
            if (stats[i].effect_type != type) continue;

            //my_stats[my_i] = new StatBit(stats[i].type, stats[i].stat, stats[i].recharge_time,stats[i].isGeneric);
            my_stats[my_i] = stats[i].clone();
            my_i++;
        }
        return new StatSum(level, xp, my_stats, runetype);
    }

    public StatSum cloneAndRemoveStat(EffectType t)
    {
        StatBit[] my_stats = new StatBit[stats.Length-1];
        int ix = 0;
        for (int i = 0; i < stats.Length; i++)
        {
            if (stats[i].effect_type == t) continue;
            my_stats[ix] = stats[i].clone();
            ix++;
        }
        return new StatSum(level, xp, my_stats, runetype);
    }

    public StatSum clone()
    {             
        StatBit[] my_stats = new StatBit[stats.Length];

        for (int i = 0; i < stats.Length; i++)
        {            
            my_stats[i] = stats[i].clone();            
        }
        return new StatSum(level, xp, my_stats, runetype);
    }

    public void UpdateStatBit(EffectType t, float f)
    {
        for (int i = 0; i < stats.Length; i++)
        {
            if (stats[i].effect_type == t) stats[i].updateStat(f);
        }

    }

    public StatBit GetStatBit(EffectType t)
    {
 //       if (t == EffectType.Range) Debug.Log("GetStatBit Range!!!\n");
        for (int i = 0; i < stats.Length; i++)
        {
            if (stats[i].effect_type == t) return stats[i];
        }
        return null;
    }
	
}