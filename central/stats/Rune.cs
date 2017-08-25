using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

//AKA SKILL TREE
[System.Serializable]
public class TimeBonus{
	public TimeName time;
	public float percent = 1;
	
	
	public TimeBonus(TimeName _name, float _percent){
		time = _name;
		percent = _percent;
	}
	
	
	public TimeBonus(){}
}

[System.Serializable]
public class RuneSaver// : IDeepCloneable<RuneSaver>
{

    public RuneType runetype = RuneType.Null;
    public StatBitSaver[] stats;
    public int ID = -1;
    public int order = -1;
    public int level = 0;
    public int max_level;
    public float xp = 0;
    public float invested_cost;
    public float distance_bonus = 0f;
    public bool hero;
    public ToyType toy_type;
    
    
    public RuneSaver()
    {
    }  
}

//AKA SKILL TREE
[System.Serializable]
public class Rune 
{
    public RuneType runetype = RuneType.Null;
    public StatBit[] stats;
    public StatSum stat_sum;
    public StatSum special_stat_sum;
    public int ID = -1;    
    public int level = 0;
    public float[] xp_reqs;    
    public float xp = 0;
    int max_level;   //saved by unitStats    
    public float invested_cost;
    public int order = -1;
    public float distance_bonus = 0f;    
    //public bool hero;    
    public ToyType toy_type;

    private float dmg_xp = 0;
    public delegate void OnCanUpgradeHandler(RuneType type);
    public static event OnCanUpgradeHandler onCanUpgrade;

    public delegate void OnUpgradeHandler(EffectType type, int ID);
    public static event OnUpgradeHandler onUpgrade;


    public Rune()
    {
    }

    public RuneSaver getSnapshot()
    {
        RuneSaver saver = new RuneSaver();
        saver.runetype = runetype;
        saver.ID = ID;
        saver.level = level;
        saver.xp = xp;
        saver.invested_cost = invested_cost;
        saver.distance_bonus = distance_bonus;
        saver.order = order;
        saver.toy_type = toy_type;
        saver.max_level = max_level;
        StatBitSaver[] saver_stats = new StatBitSaver[stats.Length];

        for (int i = 0; i < stats.Length; i++)        
        {
            saver_stats[i] = stats[i].getSnapshot();
        }
        saver.stats = saver_stats;
        return saver;
    }
   
    public void loadSnapshot(RuneSaver saver)
    {              
        runetype = saver.runetype;
        toy_type = saver.toy_type;        
        invested_cost = saver.invested_cost;
        ID = saver.ID;
        level = saver.level;
        xp = saver.xp;
        distance_bonus = saver.distance_bonus;
        order = saver.order;
        
        if (saver.max_level < 0) saver.max_level = 0;
        int max_level = Mathf.Max(saver.max_level,
            LevelStore.getMaxLevel(Central.Instance.current_lvl, Peripheral.Instance.difficulty, runetype, toy_type));
        setMaxLevel(max_level);

        Sun.OnDayTimeChange += OnDayTimeChange;
        StaticRune.assignStatBits(ref stats, this);

        foreach (StatBitSaver s in saver.stats)
        {
            StatBit stat = getStat(s.effect_type);
            stat.loadSnapshot(s, this);
        }

        UpdateTimeOfDay();
        setXpReqs();        
        UpdateStats();
    }

    public void initStats(RuneType rtype, int _max_lvl, ToyType _toy_type)
    {        
        runetype = rtype;
        if (_max_lvl < 0) _max_lvl = 0;
        setMaxLevel(_max_lvl);
        
        toy_type = _toy_type;
        invested_cost = 0;
        Sun.OnDayTimeChange += OnDayTimeChange;
        StaticRune.assignStatBits(ref stats, this);
        dmg_xp = 0f;
        setXpReqs();
    
        UpdateStats();
    }

    void setXpReqs()
    {
        xp_reqs = TowerStore.getXpReqs(toy_type, runetype);
    }
    
    public void InitHero(int level)
    {
        if (xp > 0) return;
        if (runetype == RuneType.Sensible) level = level - 1;
        if (runetype == RuneType.Airy) level = level - 2;
        if (runetype == RuneType.Vexing) level = level - 3;        

    //    if (level > 0) Debug.Log("Initializing Hero " + runetype + " to toy level " + level + "\n");
        else return;
        float dreams = 15f;

        

        for (int i = 0; i < Mathf.Min(level, getMaxLevel()); i++)
        {
            addXp(xp_reqs[i] + 1, false);
        //    Debug.Log("Hero got " + dreams + " dreams\n");
            Peripheral.Instance.addDreams(dreams, Vector3.zero, false);                
            dreams += 5f;
        }

        EffectType me = EffectType.Null;
        if (runetype == RuneType.Sensible)        
            me = EffectType.AirAttack;        

        if (runetype == RuneType.Vexing)
            me = (Random.Range(0, 1) < 0.5f) ? EffectType.Teleport : EffectType.Bees;


        if (runetype == RuneType.Airy)
        {

            float hey = Random.Range(0f, 1f);
        //    Debug.Log("yes " + hey + "\n");
            if (level >= 4)
                me = (hey < 0.3333f) ? EffectType.EMP : (hey < 0.66666) ? EffectType.Plague : EffectType.Frost;
            else
                me = (hey < 0.5f) ? EffectType.Plague : EffectType.Frost;
        }        
        if (me == EffectType.Null) return;

        GiveSpecialSkill(me);
    }

    public void GiveSpecialSkill(EffectType type)
    {
        if (!Get.isSpecial(type)) { Debug.LogError("Trying to give a special skill of type " + type + ", INVALID.\n"); }
        Upgrade(type, false, true);
        Peripheral.Instance.my_skillmaster.SetSkill(getStatBit(type));
        Peripheral.Instance.my_skillmaster.setInventory(type, true);
    }

  


    public void OnDayTimeChange(TimeName name)
    {
     //   Debug.Log(ID + " registered a time change " + name + "\n");
        UpdateStats();
    }

    public StatBit getStatBit(EffectType type)
    {       
       StatBit b = stat_sum.GetStatBit(type);
        if (b == null) b = special_stat_sum.GetStatBit(type);
        return b;
    }

    public float getRange()
    {
        return stat_sum.getRange();        
    }

    public void UpdateTimeOfDay()
    {

        float current_time_bonus = StaticRune.GetTimeBonus(runetype, toy_type);
        if (stats == null) return;
        for (int i = 0; i < stats.Length; i++)
        {
            //if (!stats[i].active) continue;
            stats[i].setModifier(current_time_bonus, distance_bonus);
        }
    }
    
    public void UpdateStats()
    {
        //     Debug.Log("Updating stats " + stats.Length + "\n");
        int actives = 0;
        int special_actives = 0;
        if (stats == null) return;
        for (int i = 0; i < stats.Length; i++)
        {
            bool special = Get.isSpecial(stats[i].effect_type);
            stats[i].checkFinisher();
            if (stats[i].active && special) special_actives++;
            if (stats[i].active && !special) actives++;
        }
        int my_i = 0;
        int my_special_i = 0;
        StatBit[] my_stats = new StatBit[actives];
        StatBit[] my_special_stats = new StatBit[special_actives];

        float current_time_bonus = StaticRune.GetTimeBonus(runetype, toy_type);
        for (int i = 0; i < stats.Length; i++)
        {
            
            
            stats[i].setModifier(current_time_bonus, distance_bonus);
            if (!stats[i].active) continue;

            bool special = Get.isSpecial(stats[i].effect_type);
            if (special)
            {
                my_special_stats[my_special_i] = stats[i];
                my_special_i++;
            }
            else
            {
                my_stats[my_i] = stats[i];                    
                my_i++;
            }

        }

        stat_sum = new StatSum(level, xp, my_stats, runetype);
        special_stat_sum = new StatSum(level, xp, my_special_stats, runetype);
    }

    public StatSum GetStats(bool special)
    {
        return (special)? special_stat_sum : stat_sum;
    }

    public StatSum GetStats(float f, bool special)
    {
        if (stat_sum == null || special_stat_sum == null) { UpdateStats(); }
        return stat_sum;

    }



    public bool CheckStatReqs(EffectType type, RuneType runetype)
    {
        bool is_vocal = false;
        /*
        if (!Get.isSpecial(type) && isMaxLevel())
        {
            if (is_vocal) Debug.Log("Reached max level " + getMaxLevel() + "\n");
            return false;
        }*/
        int s = getStatID(type);
        StatReq[] statreqs = stats[s].stat_reqs;


        if (statreqs != null && statreqs.Length > 0)
        {
            for (int i = 0; i < statreqs.Length; i++)
            {
                StatReq statreq = statreqs[i];

                if (!Peripheral.Instance.canBuildToy(runetype, statreq.required_toy)) return false;
                //if (statreq.required_toy != "" && !Central.Instance.HaveActiveToy(statreq.required_toy)) return false;


                if (statreq.type == EffectType.Null)
                {
                    return !(level < statreq.min_level);
                }

                float stat_level = this.getLevel(statreq.type);
                if (statreq.min_level > 0 && stat_level < statreq.min_level)
                {
                    if (is_vocal) Debug.Log("Rune below required level to upgrade " + type + "\n");
                    return false;
                }
                if (statreq.min_level < 0 && stat_level > 0)
               {
                    if (is_vocal) Debug.Log("Rune below required level to upgrade " + type + "\n");
                    return false;
                }
                /*
                if (xp < statreq.xp)
                {
                    if (is_vocal) Debug.Log("Below required xp to upgrade " + type + "\n");
                    return false;
                }*/

            }
        }
        else {
            if (is_vocal) Debug.Log("Invalid statreq\n");
            return false;
        }

        return true;
    }

    public bool HasUpgrade(EffectType type)
    {
        //	Debug.Log("Has upbrade? " + type + " " + stats.ContainsKey(type) + "\n");
        int i = getStatID(type);
        return (i != -1);

    }

    
    public int getStatID(EffectType type)
    {
        int r = -1;
        for (int i = 0; i < stats.Length; i++)
        {
            if (stats[i].effect_type == type) { r = i; }
        }
        return r;
    }
    
    public StatBit getStat(EffectType type)
    {
        
               
        for (int i = 0; i < stats.Length; i++)
        {
            if (stats[i].effect_type == type) { return stats[i]; }
        }
        return null;
    }

    public float addXp(float _xp, bool damage_based)
    {
        int lvl_cap = getMaxLevel();
        if (xp_reqs.Length <= lvl_cap) { Debug.Log("WTF\n"); return _xp; }

        float xp_cap = (lvl_cap > 0) ? xp_reqs[lvl_cap - 1] : xp_reqs[getMaxLevel()];

        if (isMaxLevel() || xp >= xp_cap) return _xp;
        
        float add_me = Mathf.Min(xp_cap - xp, _xp);
        xp += add_me;
        if (damage_based) dmg_xp += add_me; //for testing purposes only
        
     //   Debug.Log("Added " + add_me + " xp\n");
        return _xp - add_me;

    }

    public float getXp()
    {
        return xp;
    }

    

    public float getCurrentLevelXp()
    {
        if (level >= 1) return xp - xp_reqs[level - 1]; else return xp;
    }


    public int getMaxLevel()
    {
        return (Peripheral.Instance != null && Peripheral.Instance.getLevelMod().remove_lvl_caps)? 10 : max_level;
    }

    public void setMaxLevel(int _max_level)
    {
        Debug.Log(runetype + " " + toy_type + " Setting MaxLevel " + _max_level + "\n");
        max_level = _max_level;
    }


    public bool isMaxLevel()
    {
        if (Peripheral.Instance != null && Peripheral.Instance.getLevelMod().remove_lvl_caps) return false;
        return level >= getMaxLevel();
    }

    public float getXpToNextLevel()
    {
        if (isMaxLevel()) return 99999f;
        if (level >= 1) return xp_reqs[level] - xp_reqs[level - 1]; else return xp_reqs[level];
    }
   

    public StateType CanUpgrade(EffectType type, RuneType _runetype)
    {
        return CanUpgrade(type, _runetype, false); 
    }

    public bool CanUpgradeASpecialSkill()
    {
        foreach (StatBit statbit in special_stat_sum.stats)
        {
            //StateType check = CanUpgrade(statbit.effect_type, runetype);
         //   Debug.Log("Can upgrade " + statbit.effect_type + "? " + check + "\n");
            if (CanUpgrade(statbit.effect_type, runetype, true) == StateType.Yes) return true;
        }
        return false;
    }


    public StateType CanUpgrade(EffectType type, RuneType runetype, bool vocal)
    {
        vocal = false;
        if (vocal) Debug.Log("Can upgrade " + type + " " + runetype + " " + level + "\n");
        int i = getStatID(type);

        if (i != -1 && CheckStatReqs(type, runetype) == false)
        {
            if (vocal) Debug.Log("Skill  " + type + " stat reqs not met for upgrade\n");
            return StateType.WrongType;
        }

        if (!Get.isSpecial(type) && isMaxLevel())
        {
            if (vocal) Debug.Log("Too high level to upgrade " + type + "\n");
            return StateType.No;
        }

        if (i != -1 && stats[i].Level >= Get.MaxLvl(type))
        {
            if (vocal) Debug.Log("Skill " + type + " is already at its maximum " + Get.MaxLvl(type) + "\n");
            return StateType.No;
        }

        if (!Get.isSpecial(type) && xp < xp_reqs[level])
        {
            if (vocal) Debug.Log("Not enough xp to upgrade " + type + "\n");
            return StateType.NoResources;
        }


        if (i != -1 && !Peripheral.Instance.HaveResource(stats[i].cost))
        {
            if (vocal) Debug.Log("Cannot afford upgrade " + type + " for " + stats[i].cost.type + " " + stats[i].cost.Amount + "\n");
            return StateType.NoResources;
        }

      //  Debug.Log("CAN UPGRADE " + runetype + " " + toy_type + " " + type + "\n");
        if (onCanUpgrade != null) { onCanUpgrade(runetype); }
        return StateType.Yes;
    }

 
    
    public Cost GetUpgradeCost(EffectType type)
    {

        int s = getStatID(type);
        return (s != -1) ? StaticStat.getCost(runetype, type, stats[s].Level) : null;

    }
    
    public int getLevel(EffectType type)
    {
        int i = getStatID(type);

        if (i != -1 && stats[i].active)
        {
            return stats[i].Level;
        }
        return -1;

    }

    public bool haveUpgrade(EffectType type)
    {
        if (Get.isBasic(type) || Get.isGeneric(type)) return true;

        int i = getStatID(type);

        if (i != -1 && stats[i].active)
        {
            return stats[i].Level > 0;
        }
        return false;

    }

    public float get(EffectType type)
    {
        int i = getStatID(type);


        if (i != -1 && stats[i].active)
        {            
            return StaticRune.time_bonus_aff(stats[i].get(), type, runetype, toy_type, distance_bonus);
            
        }
        return 0;

    }


  

    public void DoSpecialThing(EffectType type)
    {
        
        StatBit g = special_stat_sum.GetStatBit(type);
        if (g == null) return;
        if (g.level == 0) return;

     //   Debug.Log("Doing special thing " + type + "\n");

        switch (type)
        {                 
            case EffectType.Speed:
                Upgrade(EffectType.Force, false);
                break;            
            default:
                
            //    Debug.Log("Rune doing special thing " + type  + "\n");
                Peripheral.Instance.my_skillmaster.SetSkill(g);
                break;
        }
    }


    public void LoadSpecialSkills()//after loading level
    {
     //   Debug.Log("Loading special skills for " + runetype + "\n");
        UpdateStats();
        foreach (StatBit s in stats)
        {
            if (Get.isSpecial(s.effect_type))
            {
                DoSpecialThing(s.effect_type);
            }
        }
    }

    public void resetSkills(bool special)
    {
        resetSkills(EffectType.Null, special);
    }

    public void resetSkills(EffectType type, bool special)
    {
        if (special && !(toy_type == ToyType.Hero || toy_type == ToyType.Hero))
        {
            Debug.Log("Trying to reset special skills on a non-hero ToyType. Stop.\n");
            return;
        }
        if (!special && runetype == RuneType.Castle)
        {
            Debug.Log("Trying to reset a regular skills on the castle. Stop.\n");
            return;
            
        }

        if (special)
        {
            foreach (StatBit stat in stats)
            {
                if (!Get.isSpecial(stat.effect_type)) continue;
                if (type != EffectType.Null && type != stat.effect_type) continue;
                Upgrade(stat.effect_type, true, true, true);
            }

            
        }
        else
        {
            if (type != EffectType.Null) Debug.LogError("Resetting 1 regular skill is not supported!\n");
            
            foreach (StatBit stat in stats)
            {
                if (Get.isSpecial(stat.effect_type) || Get.isBasic(stat.effect_type)) continue; //Get.isGeneric(stat.effect_type) || 
                                                                                                //    Debug.Log("Resetting skill " + stat.effect_type + "\n");                
                Upgrade(stat.effect_type, true, true, true);
            }            
            
        }

        Tracker.Log(PlayerEvent.ResetSkills, !special,
           customAttributes: new Dictionary<string, string>() { { "attribute_2", Peripheral.Instance.difficulty.ToString() }, { "attribute_1", runetype.ToString() + " " + type.ToString() } },
           customMetrics: new Dictionary<string, double>() { { "metric_2", order } });


    }

    public float Upgrade(EffectType type, bool use_resource)
    {
        return Upgrade(type, use_resource, false, false);
    }

    public float Upgrade(EffectType type, bool use_resource, bool force)
    {
        return Upgrade(type, use_resource, force, false);
    }

    public float Upgrade(EffectType type, bool use_resource, bool force, bool reset)
    {//ghosts should be verifying with parent to see if they can upgrade
        if (!reset && !force && toy_type != ToyType.Temporary && !(CanUpgrade(type, runetype, true) == StateType.Yes))
         { Debug.Log("You shouldn't be trying to upgrade this!"); return 0; }
        int i = getStatID(type);
        // FALSE USE_RESOURCE IS ONLY FOR TESTING or parent tower to ghost tower upgrades

        //    Debug.Log("Upgrading (reset = " + reset + ")" + type + "\n");

        if (i != -1)
        {
            //if (reset && stats[i].level == 0) return 0;
            //	Debug.Log("Leveling up " + type + "\n");


            int old_level = stats[i].Level;
            Cost init_cost = stats[i].cost;
            if (reset)
            {
                if (!Get.isGeneric(type)) stats[i].active = false;
                if (!Get.isSpecial(type)) level -= stats[i].Level;
                stats[i].Level = 0;
            }
            else
            {
                stats[i].active = true;
                stats[i].LevelUp(); //cost update happens here
            }

            if (!Get.isSpecial(type))
            {
                if (!reset) level++;
                invested_cost += stats[i].cost.Amount;
            }
            
            if (use_resource)
            {
                if (reset)
                {
                    Cost cost = StaticStat.getCumulativeCost(runetype, stats[i].effect_type, old_level);
                    if (cost != null)
                    {
                        int amount = (int)cost.Amount;
                        if (stats[i].cost.type == CostType.ScorePoint)
                            ScoreKeeper.Instance.SetTotalScore(ScoreKeeper.Instance.getTotalScore() + amount);
                        else if (stats[i].cost.type == CostType.Dreams)
                            Peripheral.Instance.addDreams(amount, Vector3.zero, false);
                    }
                }
                else
                {
                    Peripheral.Instance.UseResource(init_cost, Vector3.zero);
                }
            }
            
            UpdateStats();

            if (Central.Instance.state == GameState.InGame && (type == EffectType.Range || type == EffectType.Focus || type == EffectType.RapidFire))
                Monitor.Instance.SetSignalSize(getRange() / 2f); // to upgrade the currently displayed range signal            
            else if (Get.isSpecial(type))
                DoSpecialThing(type);

            //onUpgrade is necessary for Firearm to update itself after upgrades
            if (ID != -1) if (onUpgrade != null)
                {
                    onUpgrade(type, ID);
                }

            
            if(use_resource && !force && !reset)
            Tracker.Log(PlayerEvent.SkillUpgrade,!Get.isSpecial(type),
             customAttributes: new Dictionary<string, string>() { { "attribute_2", Peripheral.Instance.difficulty.ToString() }, { "attribute_1", type.ToString() } },
             customMetrics: new Dictionary<string, double>() { { "metric_1", stats[i].level }, { "metric_2", order } });
            

            return stats[i].cost.Amount;
        }
        else {
            Debug.Log("Trying to level up unknown stat " + type + " for " + runetype + ", not sure what to do\n");
            return 0;
        }
    }
}

public class MinStatLvl
{
    public EffectType type;
    public int stat;
}

[System.Serializable]
public class StatReq : IDeepCloneable<StatReq>
{
	public EffectType type;
	public int min_level; 
	public float xp;
	public string required_toy = "";
  //  public MinStatLvl[] stats;
	//public Dictionary<EffectType, int> stats = new Dictionary<EffectType, int> ();	
	public StatReq() { }

	public StatReq(EffectType t, int lvl){
		type = t;
		min_level = lvl;
        
	}

    object IDeepCloneable.DeepClone()
    {
        return this.DeepClone();
    }


    public StatReq DeepClone()
    {
        StatReq clone = new StatReq();
        clone.type = this.type;
        clone.min_level = this.min_level;
        clone.xp = this.xp;
        clone.required_toy = string.Copy(this.required_toy);
        return clone;
    }
	
	public StatReq(EffectType t, int lvl, string _toy){
		type = t;
		min_level = lvl;
		required_toy = _toy;
        
	}
}

