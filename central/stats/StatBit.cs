using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

[System.Serializable]
public class StatBitSaver : IDeepCloneable<StatBitSaver>
{
    public int level;
    public EffectType effect_type;
    public EffectSubType effect_sub_type;
    public RuneType rune_type;
    public bool is_hero;
    public bool active;

    public StatBitSaver() { }

    object IDeepCloneable.DeepClone()
    {
        return this.DeepClone();
    }


    public StatBitSaver DeepClone()
    {
        StatBitSaver my_clone = new StatBitSaver();
        my_clone.level = this.level;
        my_clone.effect_type = this.effect_type;
        my_clone.rune_type = this.rune_type;
        my_clone.is_hero = this.is_hero;
        my_clone.active = this.active;
        my_clone.effect_sub_type = this.effect_sub_type;      

        return my_clone;
    }
}

[System.Serializable]
public class StatBit
{
    //   public float stat; // Base_stat * multiplier  USFL
    public float base_stat;       //depends on StaticStat.getBaseFactor, difficulty, level   USFL
    public float multiplier = 1;  //depends on distance_bonus, time of day, and effecttype . Rune has distance_bonus
    public int level;
    public float recharge_time;    //set using StaticStat getInitRechargeTime, then modified for level, etc  USFL
    public EffectType effect_type;
    public bool dumb = false;         //never update stats on your own. this is used for lava based skills. take stat as it is given
    public bool very_dumb = false;     //use base_stat, do not get base stat on your own. for some skills
    bool finisher;             //depends on level
    bool permanent;            //skill is permanent, depends on level
    public EffectSubType effect_sub_type = EffectSubType.Null;
    public RuneType rune_type;
    public bool is_hero;
    public bool active;
    public Cost cost;          //StaticStat.getCost(effect_type, level); USFL
    public StatReq[] stat_reqs;

    [System.NonSerialized]
    private Rune rune;

    public void loadSnapshot(StatBitSaver saver, Rune rune)
    {
        this.effect_type = saver.effect_type;
        this.effect_sub_type = saver.effect_sub_type;
        this.rune_type = saver.rune_type;
        this.is_hero = saver.is_hero;
        this.active = saver.active;
        this.rune = rune;
        Level = saver.level;

        //updateStatsForLevel();
    }

    public StatBitSaver getSnapshot()
    {
        StatBitSaver s = new StatBitSaver();
        s.level = this.level;
        s.effect_type = this.effect_type;
        s.rune_type = this.rune_type;
        s.is_hero = this.is_hero;
        s.active = this.active;
        return s;
    }

    public int Level
    {
        get
        {
            return level;
        }

        set
        {
            level = value;
            
            if (level == -1) permanent = true;
            updateStatsForLevel();
            checkFinisher();
        }
    }

    public void checkFinisher()
    {
        bool old_finisher = finisher;
        //if (this.effect_type == EffectType.Fear && level == StaticStat.getFinisherLvl())
        // Debug.Log("Checking finisher " + this.effect_type + " Lvl " + this.level + " finisher? " + finisher + "\n");            
        finisher = (level >= StaticStat.getFinisherLvl() && RewardOverseer.RewardInstance.getRewardStatus(effect_type));

        if (old_finisher != finisher)
            if (!finisher) RewardOverseer.onRewardEnabled += onRewardEnabled;
            else RewardOverseer.onRewardEnabled -= onRewardEnabled;

    }

    public void onRewardEnabled(RewardType _reward_type, EffectType _effect_type)
    {
        if (_effect_type == this.effect_type)
        {
            checkFinisher();
            RewardOverseer.onRewardEnabled -= onRewardEnabled;
        }
    }
    /*
    public float Stat
    {
        get  {            return Base_stat;}
        
    }
    */
    public float Base_stat
    {
        get
        {
            return base_stat;
        }

        set
        {
            base_stat = value;
        }
    }

    public StatBit() { }

    public void updateStat(float f)
    {
        Base_stat = f;
    }

    public bool hasStat()
    {
        float f = Mathf.Abs(Base_stat);
        return (f > 0.001f);
    }
    public StatBit(Rune rune, RuneType rune_type, EffectType type, bool active, StatReq[] stat_reqs, float recharge_time, int level, bool is_hero)
    {
        this.rune = rune;
        this.effect_type = type;
        this.effect_sub_type = EffectSubType.Null;
        this.active = active;
        this.recharge_time = recharge_time;
        Level = level;
        this.rune_type = rune_type;
        this.stat_reqs = stat_reqs;
        this.is_hero = is_hero;
        updateStatsForLevel();
        this.very_dumb = false;

    }

    public StatBit(EffectType type, float stat, int level, bool is_hero)
    {
        this.rune = null;
        this.effect_type = type;
        this.effect_sub_type = EffectSubType.Null;
        this.active = true;
        this.recharge_time = 0f;
        this.base_stat = stat;

        this.dumb = true;
        this.very_dumb = false;
        Level = level;
        this.rune_type = RuneType.Null;
        this.stat_reqs = null;
        this.is_hero = is_hero;
        updateStatsForLevel();

    }

    public void setModifier(float current_time_bonus, float distance_bonus)
    {
        multiplier = StaticRune.time_bonus_aff(1, effect_type, current_time_bonus, distance_bonus);

    }

    public StatBit clone()
    {
        StatBit clone = new StatBit();
        //return new StatBit(rune, rune_type, effect_type, active, stat_reqs, recharge_time, Level, is_hero);
        clone.rune = this.rune;
        clone.effect_type = this.effect_type;
        clone.effect_sub_type = this.effect_sub_type;
        clone.active = this.active;
        clone.recharge_time = this.recharge_time;
        clone.base_stat = this.base_stat;
        clone.dumb = this.dumb;
        clone.level = this.level;
        clone.rune_type = this.rune_type;
        clone.stat_reqs = CloneUtil.copyArray<StatReq>(this.stat_reqs); 
        clone.is_hero = this.is_hero;
        clone.very_dumb = this.very_dumb;
        clone.multiplier = this.multiplier;
        clone.finisher = this.finisher;
        clone.permanent = this.permanent;
        clone.cost = this.cost.clone();
        return clone;
        
}

    public MyFloat[] getDetailStats()
    {
        return getDetailStats(0);
    }

    public MyFloat[] getDetailStats(int lvl_increase)
    {
        MyFloat[] f = null;
        switch (effect_type)
        {
            case EffectType.DirectDamage:
                f = getDirectDamage(lvl_increase);
                break;

            case EffectType.Range:
                f = getRange(lvl_increase);
                break;

            case EffectType.ReloadTime:
                f = getReloadTime(lvl_increase);
                break;


            //AIRY AGGRESSIVE
            case EffectType.Calamity:
                f = getCalamity(lvl_increase);
                break;
            case EffectType.Swarm:
                f = getSwarm(lvl_increase);
                break;

            //AIRY PASSIVE
            case EffectType.Weaken:
                f = getWeaken(lvl_increase);
                break;
            case EffectType.Speed:
                f = getSpeed(lvl_increase);
                break;
            case EffectType.WishCatcher:
                f = getWishCatcher(lvl_increase);
                break;
            case EffectType.Foil:
                f = getFoil(lvl_increase);
                break;


            //AIRY SPECIAL
            case EffectType.Frost:
                f = getFrost(lvl_increase);
                break;

            case EffectType.Plague:
                f = getPlague(lvl_increase);
                break;                
            case EffectType.EMP:
                f = getEMP(lvl_increase);
                break;



            //CASTLE
            case EffectType.Renew:
                f = getRenew(lvl_increase);
                break;
            case EffectType.Sync:
                f = getSync(lvl_increase);
                break;
            case EffectType.Architect:
                f = getArchitect(lvl_increase);
                break;

            //VEXING
            case EffectType.VexingForce:
                f = getVexingForce(lvl_increase);
                break;

            case EffectType.Diffuse:
                f = getDiffuse(lvl_increase);
                break;

            case EffectType.Focus:
                f = getFocus(lvl_increase);
                break;

            //VEXING SPACE
            case EffectType.Fear:
                f = getFear(lvl_increase);
                break;

            case EffectType.Transform:
                f = getTransform(lvl_increase);
                break;

            //VEXING SPECIAL
            case EffectType.Teleport:
                f = getTeleport(lvl_increase);
                break;

            case EffectType.Bees:
                f = getBees(lvl_increase);
                break;


            //SENSIBLE
            case EffectType.Force:
                f = getForce(lvl_increase);
                break;

            //SENSIBLE LASER
            case EffectType.Laser:
                f = getLaser(lvl_increase);
                break;

            case EffectType.Sparkles:
                f = getSparkles(lvl_increase);
                break;
            case EffectType.DOT:
                f = getDOT(lvl_increase);
                break;
            //SENSIBLE FIRE
            case EffectType.RapidFire:
                f = getRapidFire(lvl_increase);
                break;
            case EffectType.Explode_Force:
                f = getExplodeForce(lvl_increase);
                break;
            case EffectType.Stun:
                f = getStun(lvl_increase);
                break;

            case EffectType.Critical:
                f = getCritical(lvl_increase);
                break;

            //SENSIBLE SPECIAL
            case EffectType.AirAttack:
                f = getAirAttack(lvl_increase);
                break;

            case EffectType.Meteor:
                f = getMeteor(lvl_increase);
                break;
            //TIME 
            

        }

        return f;

    }

    private MyFloat[] getReloadTime(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers = new MyFloat[1];
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Recharge);
        //   Debug.Log("ReloadTime: " + stat + " -> " + numbers[0].toString());
        return numbers;
    }

    private MyFloat[] getRange(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers = new MyFloat[1];
        numbers[0] = new MyFloat(current_stat, LabelName.Range, LabelUnit.Distance);

        return numbers;
    }

    private MyFloat[] getDirectDamage(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers = new MyFloat[2];
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Damage);
        numbers[1] = new MyFloat(0, LabelName.Null, LabelUnit.Null);

        return numbers;
    }

    private MyFloat[] initStats()
    {
        MyFloat[] numbers;
        numbers = (finisher) ? new MyFloat[StaticStat.StatLength(effect_type,effect_sub_type, true)] :
                              new MyFloat[StaticStat.StatLength(effect_type, effect_sub_type, false)];
        return numbers;
    }

    private MyFloat[] getSpeed(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers = initStats();

        float time = (rune == null) ? StaticStat.getBaseFactor(rune_type, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Percent); //speed modifier

        if (effect_sub_type == EffectSubType.Null)
        {
            numbers[1] = new MyFloat(time / 2f, LabelName.SkillStrength, LabelUnit.Duration); //time to normal  - how quickly to return to normal         
            numbers[2] = new MyFloat(time / 4f, LabelName.SkillStrength, LabelUnit.Duration); //lifetime              
        }else if (effect_sub_type == EffectSubType.Ultra)
        {
            numbers[1] = new MyFloat(time, LabelName.SkillStrength, LabelUnit.Duration); //time to normal  - how quickly to return to normal 
            numbers[2] = new MyFloat(time, LabelName.SkillStrength, LabelUnit.Duration); //lifetime         
        }else if (effect_sub_type == EffectSubType.Freeze)
        {
            numbers[1] = new MyFloat(current_stat * time * 1f / 100f, LabelName.SkillStrength, LabelUnit.Duration); //time to normal  - how quickly to return to normal 
            numbers[2] = new MyFloat(current_stat * time * 1f / 100f, LabelName.SkillStrength, LabelUnit.Duration); //lifetime         
            numbers[3] = new MyFloat(current_stat * 3f * (level + lvl_increase) / 100f, LabelName.SkillStrength, LabelUnit.Percent); //% freeze per level
            numbers[4] = new MyFloat(current_stat * 70f * (level + lvl_increase) / 100f, LabelName.SkillStrength, LabelUnit.Percent); //% weaken per freeze
            numbers[5] = new MyFloat(current_stat * time * 6f / 100f, LabelName.SkillStrength, LabelUnit.Duration); //Freeze duration
            //all the #s have the adjusted by current_stat/100f to account for factor because this is a lava.
            //current stat already has factor baked in and is in mid upper 90's so close enough.
        }

            if (finisher) numbers[3] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);

        return numbers;

    }

    private MyFloat[] getWeaken(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers;
        numbers = (finisher) ? new MyFloat[StaticStat.StatLength(effect_type, true)] :
                              new MyFloat[StaticStat.StatLength(effect_type, false)];

        float time = (rune == null) ? StaticStat.getBaseFactor(rune_type, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        int current_level = level + lvl_increase;

        float time_mult = (current_level == 1) ? 1 : (current_level == 2) ? 1.5f : 2f;


        float weaken_mult = 0f;
        if (effect_sub_type == EffectSubType.Null)
        {
            weaken_mult = (current_level == 1) ? 35f : (current_level == 2) ? 45f : 55f;

            if (permanent)
                numbers[1] = new MyFloat(99f, LabelName.TimeRemaining, LabelUnit.Duration);
            else
                numbers[1] = new MyFloat(time_mult * 3f * time / 4f, LabelName.TimeRemaining, LabelUnit.Duration);
        }
        else if (effect_sub_type == EffectSubType.Freeze)
        {
            weaken_mult = (current_level == 1) ? 70f : (current_level == 2) ? 80f : 90f;
            numbers[1] = new MyFloat(time_mult * 6f * time, LabelName.TimeRemaining, LabelUnit.Duration);
        //    Debug.Log(time_mult + " " + 6 + " " + time + "\n");
        }
        numbers[0] = new MyFloat(weaken_mult, LabelName.SkillStrength, LabelUnit.Percent);


        if (finisher) numbers[2] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);

        return numbers;
    }



    //used by meteor
    private MyFloat[] getStun(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers = initStats();
        
        int current_level = level + lvl_increase;        
        float time = (current_level == 1) ? 5f : (current_level == 2) ? 7f : 9f;
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Percent); //speed modifier
        numbers[1] = new MyFloat(time / 6f, LabelName.SkillStrength, LabelUnit.Duration); //time to normal  - how quickly to return to normal 
        numbers[2] = new MyFloat(time, LabelName.SkillStrength, LabelUnit.Duration); //lifetime 

        if (finisher) numbers[3] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);

        return numbers;
        //6 , 20

    }

    //SPECIAL
    private MyFloat[] getFrost(int lvl_increase)
    {
        
        MyFloat[] numbers = initStats();

        int cl = level + lvl_increase;
        float damage =    (cl == 1) ? 10f : (cl == 2) ? 20f : (cl == 3)? 30f : (cl == 4)? 50f : 100f;
        float lava_life = (cl == 1) ? 8f :   (cl == 2) ? 10f : (cl == 3)? 12f : 14;
        float slow = 100f;
        float radius =    (cl == 1) ? 3f : (cl == 2) ? 3.75f : (cl==3)? 4.5f : (cl==4)? 5f : 5.5f;
        float rt = (cl == 1) ? recharge_time : (cl == 2) ? recharge_time - 5 : (cl == 3) ? recharge_time - 9 : (cl == 4) ? (recharge_time - 14) : (recharge_time - 20);
        numbers[1] = new MyFloat(damage, LabelName.SkillStrength, LabelUnit.DPS); //damage            
        numbers[2] = new MyFloat(lava_life, LabelName.TimeRemaining, LabelUnit.Duration); //lava life                
        numbers[3] = new MyFloat(1, LabelName.Bullets, LabelUnit.Distance); //bullets
        numbers[4] = new MyFloat(rt, LabelName.TimeRemaining, LabelUnit.Recharge);  //recharge time
        numbers[5] = new MyFloat(radius, LabelName.Range, LabelUnit.Distance);  //lava size
        //--- this is so high because of the inevitable lava factor = lava_life*lava_size
        numbers[0] = new MyFloat(slow * numbers[5].num * numbers[2].num/ Get.lava_damage_frequency, LabelName.SkillStrength, LabelUnit.Percent); //speed  
        numbers[6] = new MyFloat(level+lvl_increase, LabelName.SkillStrength, LabelUnit.Percent);  //% freeze
        
        return numbers;
    }


    //SPECIAL
    private MyFloat[] getPlague(int lvl_increase)
    {        
        //1 how many;  2 min % damage; 3 max % damage; 4 % speed and defense decrease
        MyFloat[] numbers = initStats();

        int cl = level + lvl_increase;
        int victims = (cl == 1) ? 12 : (cl == 2) ? 16 : (cl == 3) ? 20 : (cl == 4) ? 25 : 30;
        float min_damage = (cl == 1) ? .35f : (cl == 2) ? 0.4f : (cl == 3) ? 0.47f : (cl == 4) ? .54f : .6f;
        float max_damage = (cl == 1) ? .60f : (cl == 2) ? 0.75f : (cl == 3) ? 0.9f : (cl == 4) ? 1f : 1f;
        float rt = (cl == 1) ? recharge_time : (cl == 2) ? recharge_time - 5 : (cl == 3) ? recharge_time - 9 : (cl == 4) ? (recharge_time - 14) : (recharge_time - 20);

        numbers[0] = new MyFloat(victims, LabelName.Bullets, LabelUnit.Victims);
        numbers[1] = new MyFloat(min_damage, LabelName.SkillStrength, LabelUnit.DamagePercent); //min % damage            
        numbers[2] = new MyFloat(max_damage, LabelName.SkillStrength, LabelUnit.Percent); //max % damage
        numbers[3] = new MyFloat(50f, LabelName.SkillStrength, LabelUnit.Percent); //weaken/decrease speed by by
        numbers[4] = new MyFloat(rt, LabelName.TimeRemaining, LabelUnit.Recharge);  //recharge time

        return numbers;
    }
    //SPECIAL
    private MyFloat[] getRenew(int lvl_increase)
    {        
        MyFloat[] numbers = initStats();

        int current_level = level + lvl_increase;
        float rate = (current_level == 1) ? 0.025f : (current_level == 2) ? 0.0275f : 0.03f;
        float freq = (current_level == 1) ? 2f : (current_level == 2) ? 2f : 2f;
        numbers[0] = new MyFloat(rate, LabelName.SkillStrength, LabelUnit.Regen); //rate
        numbers[1] = new MyFloat(freq, LabelName.SkillStrength, LabelUnit.Regen); //how often
        return numbers;
    }

    private MyFloat[] getSync(int lvl_increase)
    {
        throw new NotImplementedException();
    }
    //SPECIAL
    private MyFloat[] getArchitect(int lvl_increase)
    {     
        MyFloat[] numbers = initStats();

        int current_level = level + lvl_increase;

        float percent = (current_level == 1) ? 5f: (current_level == 2) ? 8f : 12;


        numbers[0] = new MyFloat(percent, LabelName.SkillStrength, LabelUnit.Percent);
        return numbers;
    }

    private MyFloat[] getDOT(int lvl_increase)
    {

        float current_stat = get(lvl_increase);
        MyFloat[] numbers = initStats();
        //rapidfire factor affects this
        float time = (rune == null) ? StaticStat.getBaseFactor(rune_type, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        int current_level = level + lvl_increase;

        float damage_mult = (current_level == 1) ? 3.5f : (current_level == 2) ? 2.5f : 1.6f; //5f : (current_level == 2) ? 8f : 12f;

        numbers[0] = new MyFloat(-current_stat * damage_mult, LabelName.SkillStrength, LabelUnit.DPS);//amount
        numbers[1] = new MyFloat(time * 2f, LabelName.TimeRemaining, LabelUnit.Duration);//time       

        if (finisher)
        {

            numbers[2] = new MyFloat(current_stat, LabelName.Range, LabelUnit.Distance); //lava range range
            numbers[3] = new MyFloat(current_stat / 3f, LabelName.SkillStrength, LabelUnit.Percent); //% strength (time) of base for lava
            numbers[4] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent); //% change to set everything on fire
        }

        return numbers;
    }

    //SPECIAL
    private MyFloat[] getTeleport(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers = initStats();

        int cl = level + lvl_increase;
        float percent = (cl == 1) ? -0.8f : -1f;
        float range = (cl == 1) ? 1.5f : (cl == 2) ? 2.2f : (cl == 3)? 3f : (cl == 4) ? 3.5f : 4f;
        float lava_life = (cl == 1) ? 2.5f : (cl == 2) ? 3f : (cl == 3)? 3.6f : (cl == 4) ? 4.3f : 5f;
        float rt = (cl == 1) ? recharge_time : (cl == 2) ? recharge_time - 5 : (cl == 3) ? recharge_time - 9 : (cl == 4) ? (recharge_time - 14) : (recharge_time - 20);
        numbers[0] = new MyFloat(percent, LabelName.SkillStrength, LabelUnit.Percent);
        numbers[1] = new MyFloat(range, LabelName.Range, LabelUnit.Distance); //teleport range    
        numbers[2] = new MyFloat(rt, LabelName.TimeRemaining, LabelUnit.Recharge);  //recharge time
        numbers[3] = new MyFloat(lava_life, LabelName.TimeRemaining, LabelUnit.Duration);  //lava life

        return numbers;

    }

    //SPECIAL
    private MyFloat[] getBees(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        //bullets = 0, damage = 1, range = 2, lava life = 3
        MyFloat[] numbers = initStats();

        int cl = level + lvl_increase;
        int bullets = (cl == 1) ? 2 : (cl == 2) ? 3 : 4;
        float DPS = (cl == 1) ? 36 : (cl == 2) ? 52 : (cl == 3) ? 75 : (cl == 4) ? 110 : 150;
        float size = (cl == 1) ? 0.75f : (cl == 2) ? 1 : (cl == 3) ? 1.2f : (cl == 4) ? 1.4f : 1.7f;
        float duration = (cl == 1) ? 10 : (cl == 2) ? 12 : (cl == 3) ? 15 : (cl == 4) ? 18 : 20;
        float rt = (cl == 1) ? recharge_time : (cl == 2) ? recharge_time - 5 : (cl == 3) ? recharge_time - 9 : (cl == 4) ? (recharge_time - 14) : (recharge_time - 20);
        numbers[0] = new MyFloat(bullets, LabelName.SkillStrength, LabelUnit.Bullets);        
        numbers[1] = new MyFloat(DPS, LabelName.SkillStrength, LabelUnit.DPS);
        numbers[2] = new MyFloat(size, LabelName.Range, LabelUnit.Distance); //size
        numbers[3] = new MyFloat(duration, LabelName.TimeRemaining, LabelUnit.Duration);
        numbers[4] = new MyFloat(rt, LabelName.TimeRemaining, LabelUnit.Recharge);  //recharge time

        return numbers;

    }

    private MyFloat[] getTransform(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers = initStats();

        numbers[0] = new MyFloat(current_stat * 100 / 2f, LabelName.SkillStrength, LabelUnit.Percent);//% of transform
        numbers[1] = new MyFloat(current_stat * 6, LabelName.TimeRemaining, LabelUnit.Duration); //timer   

        if (finisher)
        {
            numbers[2] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);
            Debug.Log("Transform finisher stats!!! % chance: " + numbers[2] + "\n");
        }

        return numbers;
    }

    private MyFloat[] getDiffuse(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers = initStats();

        int current_level = level + lvl_increase;
        float time_increase = 0.4f;
        float time = (rune == null) ? StaticStat.getBaseFactor(rune_type, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        float force = StaticStat.getBaseFactor(RuneType.Sensible, EffectType.Force, is_hero);
        

        float lava_extra = (current_level == 1) ? 0 : (current_level == 2) ? 0.27f : .7f; //.45 -? .55
        float bullet_extra = (current_level == 1) ? 0 : (current_level == 2) ? 0.25f : 0.4f;
        ////this is getting basic reload time, ie ignoring the below reload time increase
        numbers[0] = new MyFloat(.5f + current_stat * .25f, LabelName.SkillStrength, LabelUnit.Distance);//range .5 + current_stat*.18
        numbers[1] = new MyFloat(2f * (time + time_increase) / 3f, LabelName.TimeRemaining, LabelUnit.Duration); //lifetime
        
        numbers[2] = new MyFloat(current_stat * .75f + lava_extra, LabelName.SkillStrength, LabelUnit.DPS); //lava factor is multiplied by this
        numbers[3] = new MyFloat(current_stat * .34f + bullet_extra, LabelName.SkillStrength, LabelUnit.Damage); //main arrow factor is multiplied by this
        numbers[4] = new MyFloat(time_increase, LabelName.SkillStrength, LabelUnit.Recharge);// reload time increase
        numbers[5] = new MyFloat(force, LabelName.SkillStrength, LabelUnit.DPS);// damage
        
        if (finisher) numbers[6] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);

        return numbers;
    }

    private MyFloat[] getFocus(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers = initStats();

        //float time = (rune == null) ? StaticStat.getBaseFactor(rune_type, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();

        float range_factor = (level == 1) ? 1 : (level == 2) ? 1.75f : 2.5f;
        float damage_mult = (level == 1) ? 1.375f : (level == 2) ? 1.275f : 1.375f;
        numbers[0] = new MyFloat(.635f * range_factor, LabelName.Range, LabelUnit.Distance);// range increase
        numbers[1] = new MyFloat(current_stat*damage_mult, LabelName.SkillStrength, LabelUnit.Damage);// damage increase

        numbers[2] = new MyFloat(0.54f, LabelName.SkillStrength, LabelUnit.Recharge);// reload time increase

        if (finisher) numbers[3] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent); // no idea
                                                                                                               //    Debug.Log("Focus lvl " + level + " range_increase " + numbers[0].num + " damage_increase " + numbers[1].num + " reload time " + numbers[2].num + "\n");
        return numbers;
    }






    private MyFloat[] getLaser(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers = initStats();
        int current_level = level + lvl_increase;
        float damage_mult = (current_level == 1) ? 1.03f : (current_level == 2) ? 1.15f : 1.1f; 
        numbers[0] = new MyFloat(damage_mult*current_stat, LabelName.SkillStrength, LabelUnit.DPS);
        numbers[1] = new MyFloat(0, LabelName.Null, LabelUnit.Null);


        if (finisher) numbers[2] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);

        return numbers;
    }

    private MyFloat[] getSparkles(int lvl_increase)   //0 = bullets, 1 = fire time, 2 = total damage       
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers = initStats();
        int current_level = level + lvl_increase;

        float damage_mult = (current_level == 1) ? 0.48f : (current_level == 2) ? 0.55f : 0.66f; //.39 .35
        float time = (rune == null) ? StaticStat.getBaseFactor(rune_type, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        numbers[0] = new MyFloat(5 + current_stat * 1.2f, LabelName.SkillStrength, LabelUnit.Bullets);
        numbers[1] = new MyFloat(time * 1.2f, LabelName.TimeRemaining, LabelUnit.Recharge);
        numbers[2] = new MyFloat(current_stat * damage_mult, LabelName.SkillStrength, LabelUnit.Damage);

        if (finisher) numbers[3] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);
        if (finisher) numbers[4] = new MyFloat(current_stat * 2 / 3, LabelName.SkillStrength, LabelUnit.Bullets);
        return numbers;
    }

    private MyFloat[] getForce(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers = initStats();
        LabelUnit label = (rune_type == RuneType.Airy) ? LabelUnit.DPS : LabelUnit.Damage;
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, label);
        numbers[1] = new MyFloat(0, LabelName.Null, LabelUnit.Null);

        return numbers;

    }



    private MyFloat[] getVexingForce(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers = initStats();

        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Damage);
        numbers[1] = new MyFloat(0, LabelName.Null, LabelUnit.Null);

        return numbers;
    }

    private MyFloat[] getRapidFire(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers = initStats();

        
    
        int current_level = level + lvl_increase;
        float damage_mult = (current_level == 1) ? 0.84f : (current_level == 2) ? 1.3f : 1.9f; //1.64f : 2.6f;
        float time = (rune == null) ? StaticStat.getBaseFactor(rune_type, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        time -= 0.8f; //0.6 this is what it used to be when this was in sensible
        numbers[0] = new MyFloat(3, LabelName.SkillStrength, LabelUnit.Bullets);
        numbers[1] = new MyFloat(0.35f + 0.55f * current_stat, LabelName.SkillStrength, LabelUnit.Percent); //arrow mass modifier
        numbers[2] = new MyFloat(2.5f * (0.275f + 0.15f * current_stat), LabelName.SkillStrength, LabelUnit.Percent);  //arrow speed modifier
        numbers[3] = new MyFloat((float)(numbers[0].num + 1) * 0.3f / time + 0.25f, LabelName.SkillStrength, LabelUnit.Recharge);  //period between bursts of shots        
        numbers[4] = new MyFloat(current_stat * .4f, LabelName.Range, LabelUnit.Distance);  //range increase        
       // numbers[5] = new MyFloat(0.17f + 0f * (current_level - 1), LabelName.SkillStrength, LabelUnit.Percent);  //damage multiplier
        numbers[5] = new MyFloat(damage_mult, LabelName.SkillStrength, LabelUnit.Percent);  //damage multiplier
       
                                                                                                                 //.23 to .28

        //Debug.Log("Rapid Fire lvl " + current_level + " stat " + current_stat + " " + numbers[0].num + " " + numbers[1].num + " " + numbers[2].num + " " + numbers[3].num + " " + numbers[4].num + " " + numbers[5].num + "\n");
        if (finisher) numbers[6] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);
        return numbers;

    }

    //SPECIAL
    private MyFloat[] getAirAttack(int lvl_increase)
    {        
        //lava size is hardcoded in AirAttack lava.SetLocation :(
        MyFloat[] numbers = initStats();
        int cl = level + lvl_increase;
        float damage_mult = (cl == 1) ? 7f : (cl == 2) ? 9f : (cl == 3)? 12f : (cl == 4)? 15 : 18;
        int bullets = (cl == 1) ? 7 : (cl == 2) ? 12 : (cl == 3) ? 16 : (cl == 4) ? 22 : 28;
        float rt = (cl == 1) ? recharge_time : (cl == 2) ? recharge_time - 5 : (cl == 3) ? recharge_time - 9 : (cl == 4) ? (recharge_time - 14) : (recharge_time - 20);
        float lava_size = (cl == 1) ? 1.7f : (cl == 2) ? 2 : (cl == 3) ? 2.3f : (cl == 4) ? 2.5f : 2.8f;
        numbers[0] = new MyFloat(damage_mult, LabelName.SkillStrength, LabelUnit.DPS);
        numbers[1] = new MyFloat(3f, LabelName.TimeRemaining, LabelUnit.Duration); //lava life
        numbers[2] = new MyFloat(bullets, LabelName.Bullets, LabelUnit.Bullets);  //bullets
        numbers[3] = new MyFloat(rt, LabelName.TimeRemaining, LabelUnit.Recharge);  //recharge time
        numbers[4] = new MyFloat(lava_size, LabelName.Range, LabelUnit.Distance);  //lava size

        return numbers;
    }

    //SPECIAL
    private MyFloat[] getMeteor(int lvl_increase)
    {       
        MyFloat[] numbers = initStats();
        int cl = level + lvl_increase;

        float rt = (cl == 1) ? recharge_time : (cl == 2) ? recharge_time - 5 : (cl == 3) ? recharge_time - 9 : (cl == 4) ? (recharge_time - 14) : (recharge_time - 20);
        float damage_mult = (cl == 1) ? 70f : (cl == 2) ? 120f : (cl == 3) ? 170f : (cl == 4) ? 220 : 280;
        float slow = (cl == 1) ? 85f : (cl == 2) ? 90f : 100f;
                
        numbers[0] = new MyFloat(damage_mult, LabelName.SkillStrength, LabelUnit.Damage);
        numbers[1] = new MyFloat(1f, LabelName.TimeRemaining, LabelUnit.Duration); //lava life               
        numbers[3] = new MyFloat(rt, LabelName.TimeRemaining, LabelUnit.Recharge);  //recharge time
        numbers[4] = new MyFloat(3.5f, LabelName.Range, LabelUnit.Distance);  //lava size
        numbers[2] = new MyFloat(slow * numbers[4].num * numbers[1].num / Get.lava_damage_frequency, LabelName.SkillStrength, LabelUnit.Percent); //TimeSpeed  
        return numbers;
    }

    private MyFloat[] return_default(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers = new MyFloat[1];
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Null);
        return numbers;
    }


    private MyFloat[] getExplodeForce(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers;
        numbers = (finisher) ? new MyFloat[StaticStat.StatLength(effect_type, true)] :
                              new MyFloat[StaticStat.StatLength(effect_type, false)];

        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Damage);
        numbers[1] = new MyFloat(1.25f + current_stat * 1.5f, LabelName.Range, LabelUnit.Distance); //explode range        

        if (finisher) numbers[2] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);
        return numbers;
    }
    

    private MyFloat[] getCalamity(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers;
        numbers = (finisher) ? new MyFloat[StaticStat.StatLength(effect_type, true)] :
                              new MyFloat[StaticStat.StatLength(effect_type, false)];
        int current_level = level + lvl_increase;
        float time = (rune == null) ? StaticStat.getBaseFactor(rune_type, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        float range = (current_level == 1) ? 0.5f : (current_level == 2) ? 0.7f : 0.9f;
        float damage_mult = (current_level == 1) ? 0.28f : (current_level == 2) ? 0.5f : .8f;
        numbers[0] = new MyFloat(damage_mult * current_stat, LabelName.SkillStrength, LabelUnit.DPS);
        numbers[1] = new MyFloat(range, LabelName.Range, LabelUnit.Distance);  //lava size
        numbers[2] = new MyFloat(2.9f * time, LabelName.TimeRemaining, LabelUnit.Duration); //make new lava timer % of reload time
        numbers[3] = new MyFloat(1.4f * time, LabelName.TimeRemaining, LabelUnit.Duration);  // lava life as % of reload time        

        if (finisher) numbers[4] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);

        return numbers;

    }

    private MyFloat[] getFoil(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers = initStats();

        int current_level = level + lvl_increase;
        float time = (rune == null) ? StaticStat.getBaseFactor(rune_type, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        float range = (current_level == 1) ? 0.5f : (current_level == 2) ? 0.7f : 0.9f;
        numbers[0] = new MyFloat(0.5f, LabelName.SkillStrength, LabelUnit.Percent); //effect for each lava hurtme, so very short
        numbers[1] = new MyFloat(range, LabelName.Range, LabelUnit.Distance); //emp range    
        numbers[2] = new MyFloat(2.9f * time, LabelName.TimeRemaining, LabelUnit.Duration); //make new lava timer % of reload time
        numbers[3] = new MyFloat(2f, LabelName.TimeRemaining, LabelUnit.Duration);  //lava life

        return numbers;

    }

    //SPECIAL or not?
    private MyFloat[] getEMP(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers = initStats();
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Percent);
        numbers[1] = new MyFloat(current_stat * 2f, LabelName.Range, LabelUnit.Distance); //emp range    
        numbers[2] = new MyFloat(recharge_time * 2f - (level + lvl_increase - 1) * 5, LabelName.TimeRemaining, LabelUnit.Recharge);  //recharge time
        numbers[3] = new MyFloat(6f, LabelName.TimeRemaining, LabelUnit.Duration);  //lava life

        return numbers;

    }

    private MyFloat[] getSwarm(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers;
        numbers = (finisher) ? new MyFloat[StaticStat.StatLength(effect_type, true)] :
                              new MyFloat[StaticStat.StatLength(effect_type, false)];
        int current_level = level + lvl_increase;
        float time = (rune == null) ? StaticStat.getBaseFactor(rune_type, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        float range = (rune == null) ? StaticStat.getBaseFactor(rune_type, EffectType.Range, false) : rune.GetStats(false).getRange();
        float damage_mult = (current_level == 1) ? 0.7f : (current_level == 2) ? 0.85f : 1.0f;
        numbers[0] = new MyFloat(current_stat * damage_mult, LabelName.SkillStrength, LabelUnit.DPS); //.32 -> .5
        numbers[1] = new MyFloat(range, LabelName.Range, LabelUnit.Distance);  //lava size
        numbers[2] = new MyFloat(2f * time, LabelName.TimeRemaining, LabelUnit.Duration); //make new lava timer % of reload time
        numbers[3] = new MyFloat(0.5f * time, LabelName.TimeRemaining, LabelUnit.Duration);  // lava timer % of reload time        

        if (finisher) numbers[4] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);

        return numbers;
    }



    private MyFloat[] getFear(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers;
        numbers = (finisher) ? new MyFloat[StaticStat.StatLength(effect_type, true)] :
                              new MyFloat[StaticStat.StatLength(effect_type, false)];

        numbers[0] = new MyFloat(current_stat * 3f, LabelName.TimeRemaining, LabelUnit.Duration);


        if (finisher)
        {
            numbers[1] = new MyFloat(current_stat, LabelName.Range, LabelUnit.Distance); //mass panic range
            numbers[2] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Percent); //% strength (time) of base for lava
            numbers[3] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent); //% change to cause mass panic
                                                                                                     //    Debug.Log("Fear finisher stats!!! Range: " + numbers[1].num + " strength: " + numbers[2].num + " % chance: " + numbers[3].num + "\n");
        }

        return numbers;
    }

    private MyFloat[] getWishCatcher(int lvl_increase)
    {
        //float current_stat = get(lvl_increase);
        MyFloat[] numbers;
        numbers = (finisher) ? new MyFloat[StaticStat.StatLength(effect_type, true)] :
                              new MyFloat[StaticStat.StatLength(effect_type, false)];
        int current_level = level + lvl_increase;

        float percent = 20; // is actually 1 - this
        
        float time = (rune == null) ? StaticStat.getBaseFactor(rune_type, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        float time_mult = (current_level == 1) ? 1.3f : (current_level == 2) ? 2f : 3f;

        numbers[0] = new MyFloat(percent, LabelName.SkillStrength, LabelUnit.Percent);
        numbers[1] = new MyFloat(time_mult * time, LabelName.TimeRemaining, LabelUnit.Duration);

        if (finisher) numbers[2] = new MyFloat(1 / 4f, LabelName.SkillStrength, LabelUnit.Percent);

        return numbers;
    }


    private MyFloat[] getCritical(int lvl_increase)
    {
        float current_stat = get(lvl_increase);
        MyFloat[] numbers;
        numbers = (finisher) ? new MyFloat[StaticStat.StatLength(effect_type, true)] :
                              new MyFloat[StaticStat.StatLength(effect_type, false)];

        numbers[0] = new MyFloat(current_stat * 2f / 3f, LabelName.SkillStrength, LabelUnit.Percent); //min % damage increase
        numbers[1] = new MyFloat(current_stat * 2f, LabelName.SkillStrength, LabelUnit.Percent);  //max % damage increase
        numbers[2] = new MyFloat(current_stat / 7f, LabelName.SkillStrength, LabelUnit.Percent); // % change of critical hit

        if (finisher) numbers[3] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);

        return numbers;
    }

   

    
    public string getCompactDescription(LabelName name)
    {
        return getCompactDescription(name, 0);
    }



    string addLabelText(List<MyFloat> currentFloats, List<MyFloat> nextFloats)
    {
        string text = "";
        if (currentFloats == null && nextFloats == null) return "";
        List<MyFloat> currentSorted = currentFloats.OrderByDescending(x => (int) (x.type)).ToList();
        List<MyFloat> nextSorted = (nextFloats == null)? null : nextFloats.OrderByDescending(x => (int)(x.type)).ToList();
        for (int i = 0; i < currentSorted.Count; i++)
        {
            MyFloat f = currentSorted[i];
            text += Padded(f.getLabelTypeString(), 10,false) + "     " + Padded(f.toCompactString(), 5, true);
            if (nextFloats != null) text += " ->  " + nextSorted[i].toCompactString();
            text += "\n";

        }
        if (text.Equals("")) return text;
        if (nextFloats != null) return "Next Level:\n" + text;
        return text;
        
    }

    //this is dumb
    string Padded(string pad_me, int num, bool before)
    {
        string a = pad_me;
        if (pad_me.Length < num)
        {
            pad_me = (before)
                ? new String(' ', Mathf.FloorToInt(1.5f*(num - pad_me.Length))) + pad_me
                : pad_me + new String(' ', Mathf.FloorToInt(1.5f * (num - pad_me.Length)));
        }

  //      Debug.Log("Padded '" + a + "' to '" + pad_me + "'");
        return pad_me;
    }


    public string getCompactDescription(LabelName name, int lvl_increase)
    {
        string text = "";
        MyFloat[] current_floats = null;
        MyFloat[] next_floats = null;
       // Debug.Log("Getting " + name + " lvl " + level + " increase " + lvl_increase);

        if (!active)
        {
            if (lvl_increase > 0)
                current_floats = getDetailStats(lvl_increase);
            else if (!(name == LabelName.Null && !Get.isSpecial(effect_type))) // lvl 0 inactive is ok for regular skills, they have no numbers
            {
                Debug.LogError("Getting Compact description for an inactive skill " + effect_type + ", level 0\n");
                return "";
            }
        }
        else
        {
            current_floats = getDetailStats(0);
            if (lvl_increase > 0) next_floats = getDetailStats(lvl_increase);
        }


        switch (effect_type)
        {
            case EffectType.RapidFire:                
                if (name == LabelName.Null) return "High damage. Multiple bullets. Low accuracy.";

                float base_damage = (rune != null) ? rune.stat_sum.getPrimary() : 99;
                            
                current_floats[5].num *= base_damage;
                current_floats[5].type = LabelUnit.Damage;
                //current_floats[3].num = rune.GetStats(false).getReloadTime();

                if (next_floats != null)
                {
                    next_floats[5].num *= base_damage;
                    next_floats[5].type = LabelUnit.Damage;
                  //  next_floats[3].num = rune.GetStats(false).getReloadTime();

                    text += addLabelText(new List<MyFloat> {current_floats[0], current_floats[5]},
                        new List<MyFloat> {next_floats[0], next_floats[5]});
                }
                else                
                    text += addLabelText(new List<MyFloat> {current_floats[0], current_floats[3], current_floats[5]},null);
                
                break;
            case EffectType.DOT:
                if (name == LabelName.Null) return "Some clever description. Damage over time.";
                              
                current_floats[0].num = Get.getRegeneratorModRate(1, -current_floats[0].num, current_floats[1].num);
                if (next_floats != null)
                {                    
                    next_floats[0].num = Get.getRegeneratorModRate(1, -next_floats[0].num, next_floats[1].num);
                    text += addLabelText(new List<MyFloat> { current_floats[0], current_floats[1]},
                        new List<MyFloat> { next_floats[0], next_floats[1]});
                }
                else
                    text += addLabelText(new List<MyFloat> { current_floats[0], current_floats[1]}, null);
                break;
            case EffectType.Laser:
                if (name == LabelName.Null) return "Laser never misses.";
                
                current_floats[0].num *= Get.laser_damage_factor / Get.laser_damage_frequency;
                if (next_floats != null)
                {
                    next_floats[0].num *= Get.laser_damage_factor / Get.laser_damage_frequency;
                    text += addLabelText(new List<MyFloat> {current_floats[0]},
                        new List<MyFloat> {next_floats[0]});
                }else
                    text += addLabelText(new List<MyFloat> { current_floats[0]}, null);
                break;
            case EffectType.Sparkles:
                if (name == LabelName.Null) return "Sparkles do area damage!";
                if (next_floats != null)
                {
                    text += addLabelText(new List<MyFloat> {current_floats[0], current_floats[1], current_floats[2]},
                        new List<MyFloat> {next_floats[0], next_floats[1], next_floats[2]});
                }
                else
                    text += addLabelText(new List<MyFloat> {current_floats[0], current_floats[1], current_floats[2]},null);
                break;
            case EffectType.Range:
                if (name == LabelName.Null) return "Increase firing range.";
                if (next_floats != null)                
                    text += addLabelText(new List<MyFloat> { current_floats[0]},new List<MyFloat> { next_floats[0]});                
                else
                    text += addLabelText(new List<MyFloat> { current_floats[0]},null);
                break;
            case EffectType.Speed:
                if (name == LabelName.Null) return "Slow them down. THIS IS NEVER USED.";
                if (next_floats != null)               
                    text += addLabelText(new List<MyFloat> { current_floats[0] }, new List<MyFloat> { next_floats[0] });                
                else
                    text += addLabelText(new List<MyFloat> { current_floats[0] }, null);

                break;
            case EffectType.Weaken:
                if (name == LabelName.Null) return "Lower enemy defenses.";
                                
                if (next_floats != null)
                    //floats[0].num = floats[0].num;
                    text += addLabelText(new List<MyFloat> { current_floats[0], current_floats[1] }, new List<MyFloat> { next_floats[0], next_floats[1] });
                else
                    text += addLabelText(new List<MyFloat> { current_floats[0], current_floats[1] }, null);

                break;
            case EffectType.WishCatcher:
                if (name == LabelName.Null) return "Increase the chance to get a wish from a dying enemy.";
                                                
                    current_floats[0].num = 100 - current_floats[0].num;
                if (next_floats != null)
                {
                    next_floats[0].num = 100 - next_floats[0].num;
                    text += addLabelText(new List<MyFloat> {current_floats[0], current_floats[1]},
                        new List<MyFloat> {next_floats[0], next_floats[1]});
                }
                else
                    text += addLabelText(new List<MyFloat> {current_floats[0], current_floats[1]}, null);
                break;
            case EffectType.Foil:
                if (name == LabelName.Null) return "Summon a magical EMP cloud to disrupt all enemy tech within range.";

                current_floats[0].num = 100 - current_floats[0].num;
                if (next_floats != null)
                {
                    next_floats[1].num *= 1;
                    text += addLabelText(
                        new List<MyFloat> { current_floats[1], current_floats[2], current_floats[3] },
                        new List<MyFloat> { next_floats[1], next_floats[2], next_floats[3] });
                }
                else
                    text += addLabelText(
                        new List<MyFloat> { current_floats[1], current_floats[2], current_floats[3] }, null);
                break;
            case EffectType.Calamity:
                if (name == LabelName.Null) return "Some unlucky souls get followed by their own miniature tornadoes.";
                current_floats[0].num = current_floats[0].num * Get.getModLavaFactor(1f, 1, current_floats[3].num, current_floats[1].num);

                if (next_floats != null)
                {
                    next_floats[0].num = next_floats[0].num * Get.getModLavaFactor(1f, 1, next_floats[3].num, next_floats[1].num);                    
                    text += addLabelText(new List<MyFloat> { current_floats[0], current_floats[3] },
                        new List<MyFloat> { next_floats[0], next_floats[3] });
                }
                else
                    text += addLabelText(new List<MyFloat> { current_floats[0], current_floats[3] }, null);
                
                break;
            case EffectType.Swarm:
                if (name == LabelName.Null) return "Pestilence strikes all enemies within range.";
                
                current_floats[0].num = current_floats[0].num * Get.getModLavaFactor(1f, 1, current_floats[3].num, current_floats[1].num);

                if (next_floats != null)
                {
                    next_floats[0].num = next_floats[0].num * Get.getModLavaFactor(1f, 1, next_floats[3].num, next_floats[1].num);
                    text += addLabelText(new List<MyFloat> {current_floats[0]}, new List<MyFloat> {next_floats[0]});
                }
                else
                    text += addLabelText(new List<MyFloat> {current_floats[0]}, null);

                break;
            case EffectType.Fear:
                if (name == LabelName.Null) return "Make them run away in terror.";
                if (next_floats != null)                                    
                    text += addLabelText(new List<MyFloat> { current_floats[0] }, new List<MyFloat> { next_floats[0] });                
                else
                    text += addLabelText(new List<MyFloat> { current_floats[0] }, null);

                break;
            case EffectType.Diffuse:
                if (name == LabelName.Null) return "A magic mist of decay that burns everything.";

                current_floats[2].num = current_floats[5].num * Get.getModLavaFactor(current_floats[2].num, 1, current_floats[1].num, current_floats[0].num);
                current_floats[3].num *= current_floats[5].num * multiplier;//is handled in arrow/hitme, not in statbit
                current_floats[4].num += rune.GetStats(false).getReloadTime();
                if (next_floats != null)
                {
                    next_floats[2].num = next_floats[5].num * Get.getModLavaFactor(next_floats[2].num, 1, next_floats[1].num, next_floats[0].num);
                    next_floats[3].num *= next_floats[5].num * multiplier;//is handled in arrow/hitme, not in statbit
                    next_floats[4].num += rune.GetStats(false).getReloadTime();
                    
                    text += addLabelText(new List<MyFloat> { current_floats[2], current_floats[3], current_floats[4] }, 
                                         new List<MyFloat> { next_floats[2], next_floats[3], next_floats[4] });
                }
                else
                    text += addLabelText(new List<MyFloat> { current_floats[2], current_floats[3], current_floats[4] }, null);

                break;
            case EffectType.Focus:
                if (name == LabelName.Null) return "Sentient arrows never miss. Looong Range.";
                
                current_floats[2].num += rune.GetStats(false).getReloadTime();
                if (next_floats != null)
                {
                    next_floats[2].num += rune.GetStats(false).getReloadTime();
                    text += addLabelText(new List<MyFloat> { current_floats[1], current_floats[2]},new List<MyFloat> { next_floats[1],next_floats[2]});
                }
                else
                    text += addLabelText(new List<MyFloat> { current_floats[1], current_floats[2]}, null);

                break;
            case EffectType.Transform:
                if (name == LabelName.Null) return "Turns humans into frogs. Halves enemy defenses and lowers speed.";

                if (next_floats != null)                                    
                    text += addLabelText(new List<MyFloat> { current_floats[0], current_floats[1] }, new List<MyFloat> { next_floats[0], next_floats[1] });                
                else
                    text += addLabelText(new List<MyFloat> { current_floats[0], current_floats[1] }, null);
                break;
            case EffectType.Critical:
                if (name == LabelName.Null)
                    return "Randomly do tons of damage.";
                if (next_floats != null)
                {                 
                    text += addLabelText(new List<MyFloat> { current_floats[0], current_floats[1], current_floats[2] },
                        new List<MyFloat> { next_floats[0], next_floats[1], next_floats[2] });
                }
                else
                    text += addLabelText(new List<MyFloat> { current_floats[0], current_floats[1], current_floats[2] }, null);
                break;
                /// SPECIAL SKILLS
            case EffectType.Frost:
                current_floats[0].num = current_floats[0].num *
                                Get.getModLavaFactor(1f, Get.lava_damage_frequency, current_floats[2].num, current_floats[5].num);
                current_floats[1].num = current_floats[1].num * Get.getModLavaFactor(1f, 1, current_floats[2].num, current_floats[5].num);

                if (name == LabelName.Null)
                    text = FixText(current_floats, "Tap on the map to summon a blizzard!");

                else
                {

                    text = "Summon a frightul blizzard to turn your enemies into ice.\n\n";

                    if (next_floats != null)
                    {
                        next_floats[0].num = next_floats[0].num *
                                             Get.getModLavaFactor(1f, Get.lava_damage_frequency, next_floats[2].num, next_floats[5].num);
                        next_floats[1].num = next_floats[1].num * Get.getModLavaFactor(1f, 1, next_floats[2].num, next_floats[5].num);

                        text += addLabelText(new List<MyFloat> { current_floats[1], current_floats[2], current_floats[4], current_floats[5] },
                            new List<MyFloat> { next_floats[1], next_floats[2], next_floats[4], next_floats[5] });
                    }
                    else
                        text += addLabelText(new List<MyFloat> { current_floats[1], current_floats[2], current_floats[4], current_floats[5] }, null);
                }
                break;
            case EffectType.AirAttack:
                current_floats[0].num = current_floats[0].num * Get.getModLavaFactor(1f, 1, current_floats[1].num, current_floats[4].num);

                if (name == LabelName.Null)                
                    text = FixText(current_floats, "Draw a line on the map to unleash a chain of attacks for <0>!");                
                else
                {
                    text = "Launch aerial attacks on your enemies.\n\n";
                    if (next_floats != null)
                    {
                        next_floats[0].num = next_floats[0].num * Get.getModLavaFactor(1f, 1, next_floats[1].num, next_floats[4].num);
                        text += addLabelText(new List<MyFloat> { current_floats[0], current_floats[1], current_floats[2], current_floats[3] },
                            new List<MyFloat> { next_floats[0], next_floats[1], next_floats[2], next_floats[3] });
                    }
                    else
                        text += addLabelText(new List<MyFloat> { current_floats[0], current_floats[1], current_floats[2], current_floats[3] }, null);
                }                
                
                break;
            case EffectType.Meteor:
           //     Debug.Log("Meteor " + current_floats[0].num + " \n");
                current_floats[0].num = current_floats[1].num * current_floats[0].num * Get.getModLavaFactor(1f, 1, current_floats[1].num, current_floats[4].num);
              //  Debug.Log("Meteor TO " + current_floats[0].num + " \n");

                if (name == LabelName.Null)
                    text = FixText(current_floats, "Tap on the map to drop a meteor for <0>!");
                else
                {
                    text = "A giant meteor strikes from the heavens.\n\n";
                    if (next_floats != null)
                    {
                        
                        next_floats[0].num = next_floats[1].num * next_floats[0].num * Get.getModLavaFactor(1f, 1, next_floats[1].num, next_floats[4].num);
                        
                        text += addLabelText(new List<MyFloat> { current_floats[0], current_floats[3] }, new List<MyFloat> { next_floats[0], next_floats[3] });
                    }
                    else
                        text += addLabelText(new List<MyFloat> { current_floats[0], current_floats[3] }, null);
                }
               

                break;
            case EffectType.Plague:
                current_floats[1].num *= 100f;
                current_floats[2].num *= 100f;
                if (name == LabelName.Null)
                    text = FixText(current_floats, "Tap on the map to summon the plague on up to <0> enemies for <1>-<2> damage!");
                else
                {
                    text = "A plague strikes random victims, hurting them for a percent of their HP and permanently weakening the survivors.\n\n";

                    if (next_floats != null)
                    {
                        next_floats[1].num *= 100f;
                        next_floats[2].num *= 100f;
                        text += addLabelText(new List<MyFloat> {current_floats[0], current_floats[1] , current_floats[2] }, 
                                             new List<MyFloat> {next_floats[0], next_floats[1] , next_floats[2] });
                    }
                    else
                        text += addLabelText(new List<MyFloat> {current_floats[0], current_floats[1] , current_floats[2] }, null);
                }

                break;
            case EffectType.EMP:
                
                if (name == LabelName.Null)
                    text = FixText(current_floats, "Tap on the map to disable all enemy special abilities for <3> within a <1> radius.");            
                else
                {
                    text = "Temporarily disrupt the enemies' special abilities.\n\n";
                    if (next_floats != null)
                    {
                        next_floats[1].num *= 1;
                        text += addLabelText(
                            new List<MyFloat> {current_floats[1], current_floats[2], current_floats[3]},
                            new List<MyFloat> {next_floats[1], next_floats[2], next_floats[3]});
                    }
                    else
                        text += addLabelText(
                            new List<MyFloat> {current_floats[1], current_floats[2], current_floats[3]}, null);
                }
                break;
            case EffectType.Teleport:
                if (name == LabelName.Null)
                    text = FixText(current_floats, "Tap on the map to teleport enemies within a <1> radius!");
                else
                {
                    text = "Distort space itself to teleport enemies far, far away from the castle.\n\n";

                    if (next_floats != null)
                    {                        
                        text += addLabelText(
                            new List<MyFloat> { current_floats[1], current_floats[2], current_floats[3] },
                            new List<MyFloat> { next_floats[1], next_floats[2], next_floats[3] });
                    }
                    else
                        text += addLabelText(
                            new List<MyFloat> { current_floats[1], current_floats[2], current_floats[3] }, null);
                }
                break;
            case EffectType.Bees:
                if (name == LabelName.Null)
                    text = FixText(current_floats, "Tap anywhere on the map to initiate <0> bee attacks for <1>!");
                else
                {
                    text = "Summon bees to attack random victims.\n\n";
                    if (next_floats != null)
                    {
                        text += addLabelText(
                            new List<MyFloat> { current_floats[0], current_floats[1], current_floats[3], current_floats[4] },
                            new List<MyFloat> { next_floats[0], next_floats[1], next_floats[3], next_floats[4] });
                    }
                    else
                        text += addLabelText(
                            new List<MyFloat> { current_floats[0], current_floats[1], current_floats[3], current_floats[4] }, null);
                }                
                break;           
            case EffectType.Architect:
            
                text = "Increase the productivity of your employees. Permanent towers are cheaper to build.\n\n";

                if (next_floats != null)                
                    text += addLabelText(new List<MyFloat> { current_floats[0] }, new List<MyFloat> { next_floats[0] });                
                else
                    text += addLabelText(new List<MyFloat> { current_floats[0] }, null);

                break;

            case EffectType.Renew:
                
                text = "Carry out repairs to the castle. Repair 1HP every X seconds.\n\n";
                current_floats[0].num = Mathf.RoundToInt(current_floats[1].num / current_floats[0].num);

                if (next_floats != null)
                {
                    next_floats[0].num = Mathf.RoundToInt(next_floats[1].num / next_floats[0].num);
                    text += addLabelText(new List<MyFloat> {current_floats[0]}, new List<MyFloat> {next_floats[0]});
                }
                else
                    text += addLabelText(new List<MyFloat> {current_floats[0]}, null);

                break;

            default:                
                if (next_floats != null)
                    text += addLabelText(new List<MyFloat> { current_floats[0] }, new List<MyFloat> { next_floats[0] });
                else
                    text += addLabelText(new List<MyFloat> { current_floats[0] }, null);
                break;
                

        }

        return text;
    }

    string FixText(MyFloat[] stats, string text)
    {
        try
        {
            string[] vars = new string[stats.Length];
            for (int i = 0; i < stats.Length; i++)
            {
                vars[i] = stats[i].toString();
            }
            text = Show.FixText(text, vars);
        }
        catch (Exception e)
        {
            Debug.LogError(effect_type + " " + e.ToString());
        }



        return text;
    }

    public float[] getStats()
    {
        MyFloat[] floats = getDetailStats();
        float[] stats = new float[floats.Length];
        for (int i = 0; i < stats.Length; i++)
        {
            stats[i] = floats[i].num;
        }

        return stats;

    }

    public float[] getModifiedStats(float factor, float defense)
    {

        MyFloat[] floats = getDetailStats();
        float[] stats = new float[floats.Length];
        for (int i = 0; i < stats.Length; i++)
        {

            if (floats[i].type == LabelUnit.Null)
                stats[i] = floats[i].num;
            else if (i >= StaticStat.StatLength(effect_type, false))  //ie it's a finisher stat, all or nothing defense
                stats[i] = (defense == 1) ? 0f : floats[i].num * factor;
            else if (floats[i].label == LabelName.TimeRemaining)
            {
          //      if (factor != 1) Debug.Log(effect_type + " remaining time not affected by factor " + factor + "!\n");
                stats[i] = floats[i].num * (1f - defense);
            }else
            {
                stats[i] = floats[i].num * factor *(1f - defense);
            }
        }

        return stats;
    }


    public void LevelUp()
    {
        Level++;

    }

    void updateStatsForLevel()
    {
        if (dumb) return;
        this.cost = StaticStat.getCost(rune_type, effect_type, level);
        this.Base_stat = get();
        //this.stat = Base_stat * multiplier;
        this.recharge_time = getRechargeTime();
    }

    public float get() { return get(0); }


    //get but at current level + i
    public float get(int i)
    {
   
        
        int check_level = level + i;
        float check_level_multiplier = StaticStat.getBaseFactorForLevel(rune_type, effect_type, check_level, is_hero);
        float base_effect = StaticStat.getBaseFactor(rune_type, effect_type, is_hero);
        float factor = (check_level_multiplier > 0) ? 1f : -1f;

        float difficulty = Get.getDifficultyMult();
        

        float g = base_effect;
        float diff_scaler = factor * Central.Instance.medium_difficulty / difficulty;

        g = (factor > 0) ? diff_scaler - 1 + g : diff_scaler + 1 + g;
        g += base_effect * check_level_multiplier * check_level / difficulty;
        g *= multiplier;

        if (effect_type == EffectType.Renew) g = Mathf.Floor(g);

        
        if (very_dumb)
        {            
            float dumb_g = get_simple();
            //Debug.Log(effect_type + " SMART " + g + " VERY DUMB " + dumb_g + "\n");
            return dumb_g;
        }

        return g;

    }

    public float get_simple()
    {

        float difficulty = Get.getDifficultyMult();
        

        float g = base_stat;
        float diff_scaler = Central.Instance.medium_difficulty / difficulty;

        g = diff_scaler - 1 + g;
        g *= multiplier;
        return g;
    }

    public float getRechargeTime()
    {
        return getRechargeTime(0);
    }

    //get but at current level + i
    public float getRechargeTime(int i)
    {
      
        int check_level = level + i;
        //if (Get.isSpecial(effect_type)) check_level
        float difficulty = Get.getDifficultyMult();

        float g = StaticStat.getInitRechargeTime(effect_type);
        float diff_scaler = -Central.Instance.medium_difficulty / difficulty;
        float time_multiplier = .1f;
        //   Debug.Log("getting recharge time " + difficulty + "\n");
        g = diff_scaler + 1 + g;
        g -= recharge_time * time_multiplier * check_level / difficulty;

        return Mathf.CeilToInt(g);
    }


    public void SetRechargeTime(float f)
    {
        recharge_time = f;
    }

    public void addStatReqs(StatReq[] r)
    {
        stat_reqs = r;
    }





    public StatBit(int lvl)
    {
        Level = lvl;
    }

   

}


public class LabelTextGroup
{
    public LabelText labelText;
    public MyFloat currentLevel;
    public MyFloat nextLevel;


    public LabelTextGroup(LabelText labelText, MyFloat currentLevel, MyFloat nextLevel)
    {
        this.labelText = labelText;
        this.currentLevel = currentLevel;
        this.nextLevel = nextLevel;
        
    }
    

}