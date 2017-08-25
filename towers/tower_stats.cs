using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class skill_stat : IDeepCloneable<skill_stat>
{   
    public float xp;
    public float wave_start;
    public int lvl;

    public skill_stat() { }


    object IDeepCloneable.DeepClone()
    {
        return this.DeepClone();
    }

    public skill_stat DeepClone()
    {
        skill_stat my_clone = new skill_stat();        
        my_clone.xp = this.xp;
        my_clone.lvl = this.lvl;
        my_clone.wave_start = this.wave_start;
        return my_clone;

    }
}

[System.Serializable]
public class skill_stat_group : IDeepCloneable<skill_stat_group>
{

    public EffectType effect_type;

    public skill_stat[] skill_stats;

    public skill_stat_group() { }    


    object IDeepCloneable.DeepClone()
    {
        return this.DeepClone();
    }


    public skill_stat_group TrimClone() //only use this when saving results to a file to save space
    {
        skill_stat_group my_clone = new skill_stat_group();
        my_clone.effect_type = this.effect_type;

        List<skill_stat> new_list = new List<skill_stat>();

        for (int i = 0; i < this.skill_stats.Length; i++)
        {
            if (this.skill_stats[i].xp > 0) new_list.Add(this.skill_stats[i].DeepClone());
        }


        if (new_list.Count > 0)
        {
            my_clone.skill_stats = new_list.ToArray();
            return my_clone;
        }
        else
            return null;

    }

    public skill_stat_group DeepClone()
    {
        skill_stat_group my_clone = new skill_stat_group();
        my_clone.effect_type = this.effect_type;
        my_clone.skill_stats = CloneUtil.copyArray<skill_stat>(this.skill_stats);
        return my_clone;

    }
}
  

[System.Serializable]
public class tower_stats : IDeepCloneable<tower_stats>
{
    private int id = 0;
    public string name = "";
    public skill_stat_group[] skill_stats;
    public int hits = 0;
    public int shots_fired = 0;
    private int lava_count = 0;
    //private float lava_xp = 0;
    private int special_skill_count = 0;
    //private float special_skill_xp = 0;
    public string island_name;
    public float wave_time;



    public void initSkillStats(RuneType runetype)
    {
        int i = 0;
        switch (runetype)
        {
            case RuneType.Sensible:
                skill_stats = new skill_stat_group[7];
                skill_stats[i++] = newSkillStatGroup(EffectType.Laser);
                skill_stats[i++] = newSkillStatGroup(EffectType.Diffuse);
                skill_stats[i++] = newSkillStatGroup(EffectType.Force);
                skill_stats[i++] = newSkillStatGroup(EffectType.Transform);
                skill_stats[i++] = newSkillStatGroup(EffectType.AirAttack);                
                skill_stats[i++] = newSkillStatGroup(EffectType.Sparkles);
                skill_stats[i++] = newSkillStatGroup(EffectType.Meteor);
                break;
            case RuneType.Airy:
                skill_stats = new skill_stat_group[8];
                skill_stats[i++] = newSkillStatGroup(EffectType.Speed);
                skill_stats[i++] = newSkillStatGroup(EffectType.Force);
                skill_stats[i++] = newSkillStatGroup(EffectType.Weaken);
                skill_stats[i++] = newSkillStatGroup(EffectType.Calamity);
                skill_stats[i++] = newSkillStatGroup(EffectType.Swarm);
                skill_stats[i++] = newSkillStatGroup(EffectType.Foil);
                skill_stats[i++] = newSkillStatGroup(EffectType.Frost);
                skill_stats[i++] = newSkillStatGroup(EffectType.Plague);
                break;
            case RuneType.Vexing:
                skill_stats = new skill_stat_group[9];
                skill_stats[i++] = newSkillStatGroup(EffectType.RapidFire);
                skill_stats[i++] = newSkillStatGroup(EffectType.DOT);                
                skill_stats[i++] = newSkillStatGroup(EffectType.Bees);                                
                skill_stats[i++] = newSkillStatGroup(EffectType.Fear);                
                skill_stats[i++] = newSkillStatGroup(EffectType.Teleport);
                skill_stats[i++] = newSkillStatGroup(EffectType.Critical);
                skill_stats[i++] = newSkillStatGroup(EffectType.Focus);
                skill_stats[i++] = newSkillStatGroup(EffectType.VexingForce);
                skill_stats[i++] = newSkillStatGroup(EffectType.Force);
                break;
            case RuneType.Fast:
                skill_stats = new skill_stat_group[1];
                skill_stats[i] = newSkillStatGroup(EffectType.Force);                
                break;
            case RuneType.Slow:
                skill_stats = new skill_stat_group[1];
                skill_stats[i] = newSkillStatGroup(EffectType.Force);
                break;
            case RuneType.Time:
                skill_stats = new skill_stat_group[1];
                skill_stats[i] = newSkillStatGroup(EffectType.Speed);
                break;
            default:
                skill_stats = new skill_stat_group[0];
                break;
        }

    }

  
        skill_stat_group newSkillStatGroup(EffectType type)
    {
        skill_stat_group st = new skill_stat_group();
        st.effect_type = type;
        st.skill_stats = new skill_stat[4];
        st.skill_stats[0] = new skill_stat();
        st.skill_stats[0].lvl = 0;
        st.skill_stats[1] = new skill_stat();
        st.skill_stats[1].lvl = 1;
        st.skill_stats[2] = new skill_stat();
        st.skill_stats[2].lvl = 2;
        st.skill_stats[3] = new skill_stat();
        st.skill_stats[3].lvl = 3;
        return st;
    }

    public skill_stat getSkillStat(EffectType type, int lvl)
    {
        foreach (skill_stat_group stat in skill_stats)
        {
            if (stat.effect_type == type) return stat.skill_stats[lvl];
        }
        return null;
    }

    public void setSkillStat(EffectType type, int lvl, float xp)
    {
        foreach (skill_stat_group stat in skill_stats)
        {
            if (stat.effect_type == type)
            {
                if (stat.skill_stats[lvl].xp == 0 && xp > 0) stat.skill_stats[lvl].wave_start = Moon.Instance.TIME;
                stat.skill_stats[lvl].xp = xp;
            }
        }
        
    }

    public void addXp(EffectType type, int lvl, float xp)
    {
        foreach (skill_stat_group stat in skill_stats)
        {
            if (stat.effect_type == type) {
                if (stat.skill_stats[lvl].xp == 0 && xp > 0) stat.skill_stats[lvl].wave_start = Moon.Instance.TIME;
                stat.skill_stats[lvl].xp += xp;
                return;
            }
        }
        Debug.LogError("Could not find type " + type + " lvl " + lvl + " on island " + island_name + "\n");
    }

    public int Hits
    {
        get
        {
            return hits;
        }

        set
        {
            hits = value;
        }
    }

    public int Lava_count
    {
        get
        {
            return lava_count;
        }

        set
        {
            lava_count = value;
        }
    }

   

    public int ID
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
        }
    }

    public int Shots_fired
    {
        get
        {
            return shots_fired;
        }

        set
        {
            shots_fired = value;
        }
    }

    public int Special_skill_count
    {
        get
        {
            return special_skill_count;
        }

        set
        {
            special_skill_count = value;
        }
    }

  

    public tower_stats()
    {
        id = 0;        
        Hits = 0;
    }


    object IDeepCloneable.DeepClone()
    {
        return this.DeepClone();
    }

    public tower_stats DeepClone()
    {
        tower_stats my_clone = new tower_stats();
        my_clone.id = this.id;
        my_clone.name = string.Copy(this.name);
        my_clone.skill_stats = CloneUtil.copyArray<skill_stat_group>(this.skill_stats);
        my_clone.hits = this.hits;
        my_clone.shots_fired = this.shots_fired;
        my_clone.lava_count = this.lava_count;        
        my_clone.island_name = string.Copy(this.island_name);
        
        return my_clone;
    }

    public string getConciseString()
    {
        string summary = name + "|" + island_name;

        string all = "";

        for (int i = 0; i < this.skill_stats.Length; i++)
        {
            skill_stat_group g = skill_stats[i];

            string stats = "";
            foreach (skill_stat ss in g.skill_stats)
            {
                if (ss.xp > 0) stats += "|" + ss.lvl + "_" + Get.Round(ss.xp,1) + "_" + Get.Round(ss.wave_start,1);
            }
            
            if (!stats.Equals(""))   all += ":" +  (int)g.effect_type + stats;
        }

        

        string hey = (all.Equals("")) ? "" : summary + all;
        
        return hey;
    }

    public tower_stats TrimClone() //only use this when saving results to a file to save space
    {
        tower_stats my_clone = new tower_stats();
        my_clone.id = this.id;
        my_clone.name = string.Copy(this.name);        
        my_clone.hits = this.hits;
        my_clone.shots_fired = this.shots_fired;
        my_clone.lava_count = this.lava_count;
        my_clone.island_name = string.Copy(this.island_name);

        List<skill_stat_group> new_list = new List<skill_stat_group>();
        foreach (skill_stat_group g in this.skill_stats)
        {
            skill_stat_group new_group = g.TrimClone();
            if (new_group != null) new_list.Add(g);
        }
        my_clone.skill_stats = new_list.ToArray();

        return my_clone;
    }

}