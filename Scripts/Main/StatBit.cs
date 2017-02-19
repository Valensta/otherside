using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

[System.Serializable]
public class StatBit
{
    private float stat; //
    private float base_stat;
    public float multiplier = 1;
    private int level;
    public float recharge_time;
    //public float upgrade_cost;
    public EffectType effect_type;
    //public bool isGeneric;

    bool finisher;
    bool permanent;

    public RuneType rune_type;
    bool is_hero;

    public float min_level; //minimum level to required to upgrade this stat
    float max_level = 10f;
    public bool active;
    public Cost cost;
    public StatReq[] stat_reqs;

    [System.NonSerialized]
    private Rune rune;
    

    public int Level
    {
        get
        {
            return level;
        }

        set
        {
            level = value;
            if (value == null) {
                Debug.Log("Setting null level for StatBit\n");
                return;
            }
            if (level == -1) permanent = true;
            
            updateStatsForLevel();
            if (level >= 3)
            {
                checkFinisher();
                if (!finisher) RewardOverseer.onRewardEnabled += onRewardEnabled;
            }
            //  Debug.Log("Setting statbit " + type + " to level " + level + "\n");
        }
    }

    private void checkFinisher()
    {
        if (RewardOverseer.RewardInstance != null)
        finisher = RewardOverseer.RewardInstance.getRewardStatus(effect_type);
    }

    public void onRewardEnabled(RewardType _reward_type, EffectType _effect_type)
    {
        if (_effect_type == this.effect_type)
        {
            finisher = true;
            RewardOverseer.onRewardEnabled -= onRewardEnabled;
        }
    }

    public float Stat
    {
        get
        {
            return Base_stat * multiplier;
        }
        
    }

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

    public StatBit()
    {
  //      Debug.Log("Using default statbit constructor\n");
    }

    public void updateStat(float f)
    {        
        Base_stat = f;
    }

    public bool hasStat()
    {
        float f = Mathf.Abs(Stat);
        return (f > 0.001f);
    }
    public StatBit(Rune rune, RuneType rune_type, EffectType type, bool active, StatReq[] stat_reqs, float recharge_time, int level, bool is_hero)
    {
        this.rune = rune;
        this.effect_type = type;
        this.active = active;
        this.recharge_time = recharge_time;
        Level = level;
        this.rune_type = rune_type;
        this.stat_reqs = stat_reqs;
        this.is_hero = is_hero;
        checkFinisher();
        updateStatsForLevel();
               
    }

    public StatBit(EffectType type, float stat, int level, bool is_hero)
    {
        this.rune = null;
        this.effect_type = type;
        this.active = true;
        this.recharge_time = 0f;
        Level = level;
        this.rune_type = RuneType.Null;
        this.stat_reqs = null;
        this.is_hero = is_hero;
        checkFinisher();
        updateStatsForLevel();
        
    }

    public void setModifier(float current_time_bonus, float distance_bonus)
    {
        multiplier = StaticRune.time_bonus_aff(1, effect_type, current_time_bonus, distance_bonus);        
    }

    public StatBit clone()
    {
        return new StatBit(rune, rune_type, effect_type, active, stat_reqs, recharge_time, Level, is_hero);
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


            //AIRY SPECIAL
            case EffectType.Quicksand:
                f = getQuicksand(lvl_increase);
                break;

            case EffectType.Plague:
                f = getPlague(lvl_increase);
                break;

            case EffectType.EMP:
                f = getEMP(lvl_increase);
                break;



            //CASTLE
            case EffectType.BaseHealth:
                f = getBaseHealth(lvl_increase);
                break;            
            case EffectType.Sync:
                f = getSync(lvl_increase);
                break;
            case EffectType.Gnomes:
                f = getGnomes(lvl_increase);
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

            //TIME 
            case EffectType.TimeSpeed:
                f = getTimeSpeed(lvl_increase);
                break;

        }

        return f;

    }

    private MyFloat[] getReloadTime(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers = new MyFloat[1];
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Time);
     //   Debug.Log("ReloadTime: " + stat + " -> " + numbers[0].toString());
        return numbers;
    }

    private MyFloat[] getRange(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers = new MyFloat[1];
        numbers[0] = new MyFloat(current_stat, LabelName.Range, LabelUnit.Distance);
        
        return numbers;
    }

    private MyFloat[] getDirectDamage(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers = new MyFloat[2];
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Damage);
        numbers[1] = new MyFloat(0, LabelName.Null, LabelUnit.Null);

        return numbers;
    }


    private MyFloat[] getSpeed(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers;
        if (finisher) numbers = new MyFloat[4]; else numbers = new MyFloat[3];

        float time = (rune == null) ? StaticStat.getBaseFactor(RuneType.Airy, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();

        numbers[0] = new MyFloat(0.8f - 0.7f * current_stat / Get.MaxLvl(EffectType.Speed), LabelName.SkillStrength, LabelUnit.Percent); //speed modifier
        numbers[1] = new MyFloat(time / 4f, LabelName.SkillStrength, LabelUnit.Time); //time to normal  - how quickly to return to normal 

        if (permanent)                    
            numbers[2] = new MyFloat(99f, LabelName.SkillStrength, LabelUnit.Time); //lifetime         
        else
            numbers[2] = new MyFloat(time / 4f, LabelName.SkillStrength, LabelUnit.Time); //lifetime              

        if (finisher) numbers[3] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);

        return numbers;

    }

    private MyFloat[] getTimeSpeed(int lvl_increase)//Time, lasts longer, is stronger
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers;
        if (finisher) numbers = new MyFloat[4]; else numbers = new MyFloat[3];

        float time = (rune == null) ? StaticStat.getBaseFactor(RuneType.Time, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        numbers[0] = new MyFloat(1f - current_stat / Get.MaxLvl(EffectType.TimeSpeed), LabelName.SkillStrength, LabelUnit.Percent); //speed modifier
        numbers[1] = new MyFloat(time / 15f, LabelName.SkillStrength, LabelUnit.Time); //time to normal  - how quickly to return to normal 
        numbers[2] = new MyFloat(time / 30f, LabelName.SkillStrength, LabelUnit.Time); //lifetime         

        if (finisher) numbers[3] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);

        return numbers;

    }

    private MyFloat[] getStun(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers;
        if (finisher) numbers = new MyFloat[4]; else numbers = new MyFloat[3];
                
        float time = (rune == null) ? StaticStat.getBaseFactor(RuneType.Sensible, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        numbers[0] = new MyFloat((1 - (current_stat - 1) / (Get.MaxLvl(EffectType.Stun) - 1)) / 3f, LabelName.SkillStrength, LabelUnit.Percent); //speed modifier
        numbers[1] = new MyFloat(time/6f, LabelName.SkillStrength, LabelUnit.Time); //time to normal  - how quickly to return to normal 
        numbers[2] = new MyFloat(time/20f, LabelName.SkillStrength, LabelUnit.Time); //lifetime 

        if (finisher) numbers[3] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);

        return numbers;
        //6 , 20

    }

    private MyFloat[] getQuicksand(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers = new MyFloat[5];
        numbers[0] = new MyFloat(current_stat * 1.5f, LabelName.SkillStrength, LabelUnit.Percent); //speed
        numbers[1] = new MyFloat(current_stat / 2.5f, LabelName.SkillStrength, LabelUnit.Damage); //damage            
        numbers[2] = new MyFloat(current_stat * 3f, LabelName.TimeRemaining, LabelUnit.Time); //lava life        
        numbers[3] = new MyFloat(current_stat * 4f, LabelName.Bullets, LabelUnit.Distance); //bullets
        numbers[4] = new MyFloat(recharge_time, LabelName.TimeRemaining, LabelUnit.Time);  //recharge time

        return numbers;
    }

    private MyFloat[] getPlague(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        //1 how many;  2 min % damage; 3 max % damage; 4 % speed and defense decrease
        MyFloat[] numbers = new MyFloat[5];
        numbers[0] = new MyFloat(current_stat * 7, LabelName.Bullets, LabelUnit.Bullets); 
        numbers[1] = new MyFloat(current_stat / 5f, LabelName.SkillStrength, LabelUnit.Percent); //min % damage            
        numbers[2] = new MyFloat(2* current_stat / 5f, LabelName.SkillStrength, LabelUnit.Percent); //max % damage
        numbers[3] = new MyFloat(current_stat / 3f, LabelName.SkillStrength, LabelUnit.Percent); //weaken/decrease speed by by
        numbers[4] = new MyFloat(recharge_time, LabelName.TimeRemaining, LabelUnit.Time);  //recharge time

        return numbers;
    }

    private MyFloat[] getBaseHealth(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers = new MyFloat[1];
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Null);
        return numbers;
    }    

    private MyFloat[] getSync(int lvl_increase)
    {
        throw new NotImplementedException();
    }

    private MyFloat[] getGnomes(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers = new MyFloat[1];
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Percent);
        return numbers;
    }

    private MyFloat[] getDOT(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers;
        if (finisher) numbers = new MyFloat[5]; else numbers = new MyFloat[2];
                             
        float time = (rune == null) ? StaticStat.getBaseFactor(RuneType.Sensible, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        numbers[0] = new MyFloat(-current_stat * 0.4f, LabelName.SkillStrength, LabelUnit.Percent);//amount
        numbers[1] = new MyFloat(time*5f, LabelName.TimeRemaining, LabelUnit.Null);//time       

        if (finisher)
        {

            numbers[2] = new MyFloat(current_stat, LabelName.Range, LabelUnit.Distance); //lava range range
            numbers[3] = new MyFloat(current_stat / 3f, LabelName.SkillStrength, LabelUnit.Percent); //% strength (time) of base for lava
            numbers[4] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent); //% change to set everything on fire
        }       

        return numbers;
    }

    private MyFloat[] getTeleport(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers = new MyFloat[4];
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Percent);
        numbers[1] = new MyFloat(-current_stat * 3, LabelName.Range, LabelUnit.Distance); //teleport range    
        numbers[2] = new MyFloat(recharge_time, LabelName.TimeRemaining, LabelUnit.Time);  //recharge time
        numbers[3] = new MyFloat(2f, LabelName.TimeRemaining, LabelUnit.Time);  //lava life

        return numbers;

    }

    private MyFloat[] getBees(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        //bullets = 0, damage = 1, range = 2, lava life = 3
        MyFloat[] numbers = new MyFloat[5];
        numbers[0] = new MyFloat(current_stat * 2f, LabelName.SkillStrength, LabelUnit.Bullets);
        numbers[1] = new MyFloat(current_stat / 8f, LabelName.SkillStrength, LabelUnit.Damage);
        numbers[2] = new MyFloat(current_stat * 3/4, LabelName.Range, LabelUnit.Distance);
        numbers[3] = new MyFloat(current_stat * 4f, LabelName.TimeRemaining, LabelUnit.Time);
        numbers[4] = new MyFloat(recharge_time, LabelName.TimeRemaining, LabelUnit.Time);  //recharge time

        return numbers;

    }

    private MyFloat[] getTransform(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers;
        if (finisher) numbers = new MyFloat[3]; else numbers = new MyFloat[2];
        
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Percent);//% of transform
        numbers[1] = new MyFloat(current_stat * 6, LabelName.TimeRemaining, LabelUnit.Time); //timer   

        if (finisher) numbers[2] = new MyFloat(Stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);

        return numbers;
    }

    private MyFloat[] getDiffuse(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers;
        if (finisher) numbers = new MyFloat[4]; else numbers = new MyFloat[3];
               
        float time = (rune == null) ? StaticStat.getBaseFactor(RuneType.Vexing, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        numbers[0] = new MyFloat(.5f + current_stat * .25f, LabelName.SkillStrength, LabelUnit.Percent);//range .5 + current_stat*.18
        numbers[1] = new MyFloat(2f*time/3f, LabelName.TimeRemaining, LabelUnit.Time); //lifetime
        numbers[2] = new MyFloat(current_stat * 0.1f, LabelName.SkillStrength, LabelUnit.Percent); //lava factor is multiplied by this
       // Debug.Log("Diffuse level " + level + " stat " + stat + " range " + numbers[0].num + " factor " + numbers[2].num + "\n");
        if (finisher) numbers[3] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);

        return numbers;
    }

    private MyFloat[] getFocus(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers;
        if (finisher) numbers = new MyFloat[4]; else numbers = new MyFloat[3];
        
        float time = (rune == null) ? StaticStat.getBaseFactor(RuneType.Vexing, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        numbers[0] = new MyFloat(current_stat * .3f, LabelName.Range, LabelUnit.Distance);// range increase
        numbers[1] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Damage);// damage increase
        numbers[2] = new MyFloat(0.4f + current_stat * .05f, LabelName.SkillStrength, LabelUnit.Damage);// reload time increase

        if (finisher) numbers[3] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent); // no idea

        return numbers;
    }


    private MyFloat[] getEMP(int lvl_increase)
    { 
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers = new MyFloat[4];
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Percent);
        numbers[1] = new MyFloat(-current_stat * 4, LabelName.Range, LabelUnit.Distance); //emp range    
        numbers[2] = new MyFloat(recharge_time, LabelName.TimeRemaining, LabelUnit.Time);  //recharge time
        numbers[3] = new MyFloat(7f, LabelName.TimeRemaining, LabelUnit.Time);  //lava life

        return numbers;

    }

    private MyFloat[] getLaser(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers;
        if (finisher) numbers = new MyFloat[3]; else numbers = new MyFloat[2];
                
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Damage);
        numbers[1] = new MyFloat(0, LabelName.Null, LabelUnit.Null);

        if (finisher) numbers[2] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);

        return numbers;
    }
    
    private MyFloat[] getSparkles(int lvl_increase)   //0 = bullets, 1 = fire time, 2 = total damage       
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers;
        if (finisher) numbers = new MyFloat[5]; else numbers = new MyFloat[3];
        
        float time = (rune == null) ? StaticStat.getBaseFactor(RuneType.Sensible, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        numbers[0] = new MyFloat(5 + current_stat * 1.2f, LabelName.SkillStrength, LabelUnit.Bullets);
        numbers[1] = new MyFloat(time*1.2f, LabelName.TimeRemaining, LabelUnit.Time);
        numbers[2] = new MyFloat(current_stat * 0.4f, LabelName.SkillStrength, LabelUnit.Damage);

        if (finisher) numbers[3] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);
        if (finisher) numbers[4] = new MyFloat(current_stat * 2/3, LabelName.SkillStrength, LabelUnit.Bullets);
        return numbers;
    }

    private MyFloat[] getForce(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers = new MyFloat[2];
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Damage);
        numbers[1] = new MyFloat(0, LabelName.Null, LabelUnit.Null);   

        return numbers;

    }

    private MyFloat[] getVexingForce(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers = new MyFloat[2];
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Damage);
        numbers[1] = new MyFloat(0, LabelName.Null, LabelUnit.Null);     

        return numbers;
    }

    private MyFloat[] getRapidFire(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers;
        if (finisher) numbers = new MyFloat[5]; else numbers = new MyFloat[4];
        
        float time = (rune == null) ? StaticStat.getBaseFactor(RuneType.Sensible, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        numbers[0] = new MyFloat(level+lvl_increase+1, LabelName.SkillStrength, LabelUnit.Bullets);
        numbers[1] = new MyFloat(0.15f + 0.55f * current_stat, LabelName.SkillStrength, LabelUnit.Percent); //arrow mass modifier
        numbers[2] = new MyFloat(0.625f + 0.15f * current_stat, LabelName.SkillStrength, LabelUnit.Percent);  //arrow speed modifier
        numbers[3] = new MyFloat((float)(numbers[0].num + 1) * 0.3f / time + 0.25f, LabelName.SkillStrength, LabelUnit.Time);  //period between bursts of shots

        if (finisher) numbers[4] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);
        return numbers;

    }

    private MyFloat[] getAirAttack(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers = new MyFloat[4];
        numbers[0] = new MyFloat(current_stat*2f, LabelName.SkillStrength, LabelUnit.Damage);
        numbers[1] = new MyFloat(current_stat / 2.5f, LabelName.TimeRemaining, LabelUnit.Time); //lava life
        numbers[2] = new MyFloat(Mathf.CeilToInt(current_stat), LabelName.Bullets, LabelUnit.Null);  //bullets
        numbers[3] = new MyFloat(recharge_time, LabelName.TimeRemaining, LabelUnit.Time);  //recharge time

        return numbers;
    }

    private MyFloat[] return_default(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers = new MyFloat[1];
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Null);
        return numbers;
    }


    private MyFloat[] getExplodeForce(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers;
        if (finisher) numbers = new MyFloat[3]; else numbers = new MyFloat[2];
        
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Damage);
        numbers[1] = new MyFloat(1.25f + current_stat * 1.5f, LabelName.Range, LabelUnit.Distance); //explode range        

        if (finisher) numbers[2] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);
        return numbers;
    }


    private MyFloat[] getCalamity(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers;
        if (finisher) numbers = new MyFloat[5]; else numbers = new MyFloat[4];

        float time = (rune == null) ? StaticStat.getBaseFactor(RuneType.Airy, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Damage);
        numbers[1] = new MyFloat(1/6f, LabelName.Range, LabelUnit.Distance);  //lava size
        numbers[2] = new MyFloat(2.9f * time, LabelName.TimeRemaining, LabelUnit.Time); //make new lava timer % of reload time
        numbers[3] = new MyFloat(1.4f * time, LabelName.TimeRemaining, LabelUnit.Time);  // lava life as % of reload time        

        if (finisher) numbers[4] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);

        return numbers;

    }

    private MyFloat[] getSwarm(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers;
        if (finisher) numbers = new MyFloat[5]; else numbers = new MyFloat[4];

        float time = (rune == null) ? StaticStat.getBaseFactor(RuneType.Airy, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        numbers[0] = new MyFloat(current_stat * 0.8f, LabelName.SkillStrength, LabelUnit.Damage);
        numbers[1] = new MyFloat(1f, LabelName.Range, LabelUnit.Distance);  //lava size
        numbers[2] = new MyFloat(2f * time, LabelName.TimeRemaining, LabelUnit.Time); //make new lava timer % of reload time
        numbers[3] = new MyFloat(0.5f * time, LabelName.TimeRemaining, LabelUnit.Time);  // lava timer % of reload time        

        if (finisher) numbers[4] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);

        return numbers;
    }



    private MyFloat[] getFear(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers;
        if (finisher) numbers = new MyFloat[4]; else numbers = new MyFloat[1];

        numbers[0] = new MyFloat(current_stat * 3f, LabelName.TimeRemaining, LabelUnit.Time);


        if (finisher)
        {

            numbers[1] = new MyFloat(current_stat, LabelName.Range, LabelUnit.Distance); //mass panic range
            numbers[2] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Percent); //% strength (time) of base for lava
            numbers[3] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent); //% change to cause mass panic
        }
       
        return numbers;
    }

    private MyFloat[] getWishCatcher(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers;
        if (finisher) numbers = new MyFloat[3]; else numbers = new MyFloat[2];
        
        float time = (rune == null) ? StaticStat.getBaseFactor(RuneType.Airy, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        numbers[0] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Percent);
        numbers[1] = new MyFloat(time * 1.3f, LabelName.TimeRemaining, LabelUnit.Time);        

        if (finisher) numbers[2] = new MyFloat(1 / 4f, LabelName.SkillStrength, LabelUnit.Percent);

        return numbers;
    }


    private MyFloat[] getCritical(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers;
        if (finisher) numbers = new MyFloat[4]; else numbers = new MyFloat[3];
        
        numbers[0] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent); //min % damage increase
        numbers[1] = new MyFloat(current_stat, LabelName.SkillStrength, LabelUnit.Percent);  //max % damage increase
        numbers[2] = new MyFloat(current_stat / 5f, LabelName.SkillStrength, LabelUnit.Percent); // % change of critical hit

        if (finisher) numbers[3] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);

        return numbers;
    }

    private MyFloat[] getWeaken(int lvl_increase)
    {
        float current_stat = (lvl_increase > 0) ? get(lvl_increase) : Stat;
        MyFloat[] numbers;
        if (finisher) numbers = new MyFloat[3]; else numbers = new MyFloat[2];
        
        float time = (rune == null) ? StaticStat.getBaseFactor(RuneType.Airy, EffectType.ReloadTime, false) : rune.GetStats(false).getReloadTime();
        float x = 1 - current_stat / Get.MaxLvl(EffectType.Weaken);
        numbers[0] = new MyFloat(x, LabelName.SkillStrength, LabelUnit.Percent);

     //   Debug.Log("Getting weaken " + level + " " + permanent + "\n");
        if (permanent)
            numbers[1] = new MyFloat(99f, LabelName.TimeRemaining, LabelUnit.Time);
        else
            numbers[1] = new MyFloat(time / 2f, LabelName.TimeRemaining, LabelUnit.Time);

        if (finisher) numbers[2] = new MyFloat(current_stat / 4f, LabelName.SkillStrength, LabelUnit.Percent);

        return numbers;
    }


    public string getVerboseDescription()
    {
        return getVerboseDescription(0);
    }

    public string getVerboseDescription(int lvl_increase)
    {
        string text = "";
        switch (effect_type)
        {
            case EffectType.AirAttack:
                text = "Launch <3> aerial attacks on your enemies for <1>. Can be used every <4>.";
                break;
            case EffectType.Teleport:
                text = "Distort space itself to teleport enemies far, far away from the castle.  The teleportation effect lasts for <4>. Use every <3>.";
                break;
            case EffectType.Quicksand:
                text = "Summon quicksand to slow down your enemies by <1> and hurt them a wee bit for <2>. Can use every <5>. Quicksand lasts for <3>.";
                break;
            case EffectType.Plague:
                text = "A plague strikes <1> random victims hurting them for <2> to <3> of their health and permanently crippling the survivors.";
                break;
            case EffectType.Bees:
                text = "Summon <1> flocks of bees to attack random victims for <2> per second. Bees last for <4>. Can use every <5>.";
                break;
            case EffectType.EMP:
                text = "Disable all special abilities that the pesky humans might have for <4> within a certain range. Use every <3>.";
                break;
            case EffectType.Gnomes:
                text = "Increase the productivity of your employees. Permanent towers now cost <1> less.";
                break;
            case EffectType.BaseHealth:
                text = "Build lots of very tall WALLS to increase the castle defenses by <1> health.";
                break;
            case EffectType.Sync:
                text = "Towers can get <1> of certain upgrades from the non-chosen skill tree.";
                break;            
            default:
                return "I don't have one for " + effect_type;

        }

        MyFloat[] stats = getDetailStats(lvl_increase);
        string[] vars = new string[stats.Length];
        for (int i = 0; i < stats.Length; i++)
        {
            vars[i] = stats[i].toString();
        }
        text = Show.FixText(text, vars);

        return text;

    }

    public string getCompactDescription(LabelName name)
    {
        return getCompactDescription(name, 0);
    }



    public string getCompactDescription(LabelName name, int lvl_increase)
    {
        string text = "";
        MyFloat[] floats = null;
        switch (effect_type)
        {
            case EffectType.RapidFire:
                if (name == LabelName.Null)
                    return "Shoot several modified bullets in rapid succession.";
                else
                {
                    text = "<1>, <2> bullets every <3>";
                    if (rune == null) Debug.Log(" Rune is null\n");
                    float base_damage = (rune != null) ? rune.stat_sum.getPrimary() : 99;
                    float time = (rune != null) ? rune.stat_sum.getReloadTime(false) : StaticStat.getBaseFactor(RuneType.Sensible, EffectType.ReloadTime, false);
                    //Debug.Log("Got base damage " + base_damage + "\n");
                    floats = getDetailStats(lvl_increase);
                    floats[1].num *= base_damage;
                    floats[1].type = LabelUnit.Damage;
                    floats[2] = new MyFloat(time, LabelName.TimeRemaining, LabelUnit.Time);
                }
                break;
            
            case EffectType.Laser:
                if (name == LabelName.Null)
                    return "Laser never misses.";
                else
                { 
                    text = "<1> per second";
                    floats = getDetailStats(lvl_increase);
                }
                break;
            case EffectType.Sparkles:
                if (name == LabelName.Null)
                    return "Sparkles do area damage!";
                else
                { 
                    text = "<1> sparkles for <3>";
                    floats = getDetailStats(lvl_increase);
                }
                break;
            case EffectType.Range:
                if (name == LabelName.Null)
                    return "Increase firing range.";
                else
                { 
                    text = "<1>";
                    floats = getDetailStats(lvl_increase);
                }
                break;
            case EffectType.Speed:
                if (name == LabelName.Null)
                    return "Slow them down.";
                else
                { 
                    text = "By <1> for <2>";
                    floats = getDetailStats(lvl_increase);                    
                }
                break;
            case EffectType.TimeSpeed:
                if (name == LabelName.Null)
                    return "Super slow them down.";
                else
                { 
                    text = "By <1> for <2>";
                    floats = getDetailStats(lvl_increase);                    
                }
                break;
            case EffectType.Weaken:
                if (name == LabelName.Null)
                    return "Lower enemy defenses.";
                else
                {
                    text = "by <1> for <2>";
                    floats = getDetailStats(lvl_increase);
                    floats[0].num = 1 - floats[0].num;
                }
                break;
            case EffectType.WishCatcher:
                if (name == LabelName.Null)
                    return "Increase the chance to get a wish from a dying enemy.";
                else
                {
                    text = "by <1> for <2>";
                    floats = getDetailStats(lvl_increase);
                    floats[0].num = 1 - floats[0].num;
                }
                break;
            case EffectType.Calamity:
                if (name == LabelName.Null)
                    return "Summon your own miniature tornado.";
                else
                { 
                    text = "<1> per second for <4>";
                    floats = getDetailStats(lvl_increase);
                    floats[0].num /= StaticRune.getLavaDamageFrequency();
                }
                break;
            case EffectType.Swarm:
                if (name == LabelName.Null)
                    return "Pestilence strikes all enemies within range.";
                else
                { 
                    text = "<1> per second for <4>";
                    floats = getDetailStats(lvl_increase);
                    floats[0].num /= StaticRune.getLavaDamageFrequency();
                }
                break;
            case EffectType.Fear:
                if (name == LabelName.Null)
                    return "Make them run away in terror.";
                else
                {
                    text = "for <1>";
                    floats = getDetailStats(lvl_increase);
                }
                break;
            case EffectType.Diffuse:
                if (name == LabelName.Null)
                    return "A magic mist of decay.";
                else {
                    text = "<1> for <2>";
                    floats = getDetailStats(lvl_increase);
                    floats[0].num /= StaticRune.getLavaDamageFrequency();
                }
                break;
            case EffectType.Focus:
                if (name == LabelName.Null)
                    return  "Sentient arrows never miss.";
                else
                {
                    text = "Range increased by <1>";
                    floats = getDetailStats(lvl_increase);
                }
        break;
            case EffectType.Transform:
                if (name == LabelName.Null)
                    return "Transform humans into toads.";
                else
                {
                    text = "<1> for <2>";
                    floats = getDetailStats(lvl_increase);
                }
                break;

            case EffectType.Critical:
                if (name == LabelName.Null)                
                    return "Randomly do tons of damage.";                
                else
                {
                    text = "extra <1> - <2> damage <3> of the time";
                    floats = getDetailStats(lvl_increase);
                }
                break;
            default:
                return "duno what " + effect_type + " of labelname " + name + " is";

        }
        
        return FixText(floats, text);
    }

    string FixText(MyFloat[] stats, string text)
    {
        
        string[] vars = new string[stats.Length];
        for (int i = 0; i < stats.Length; i++)
        {
            vars[i] = stats[i].toString();
        }
        
        
        text = Show.FixText(text, vars);

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
            if (floats[i].type == LabelUnit.Null) stats[i] = floats[i].num;
            else
                stats[i] = floats[i].num * factor * (1f - defense);
        }

        return stats;
    }


    public void LevelUp()
    {
        level++;
        updateStatsForLevel();
    }

    void updateStatsForLevel()
    {
        this.cost = StaticStat.getCost(effect_type, level);
        this.Base_stat = get();
        this.stat = Base_stat * multiplier;
    }

    public float get() { return get(0); }


    //get but at current level + i
    public float get(int i)
    {
        int check_level = level + i;

        float multiplier = StaticStat.getMultiplier(rune_type, effect_type, check_level, is_hero);
        float base_effect = StaticStat.getBaseFactor(rune_type, effect_type, is_hero);
        float factor = (multiplier > 0) ? 1f : -1f;

        float difficulty = Peripheral.Instance.difficulty;
        difficulty = (difficulty == 0) ? Central.Instance.medium_difficulty : difficulty;

        float g = base_effect;
        float diff_scaler = factor * Central.Instance.medium_difficulty / difficulty;

        g = (factor > 0) ? diff_scaler - 1 + g : diff_scaler + 1 + g;
        g += base_effect * multiplier * check_level / difficulty;

        //	Debug.Log (type + "got " + g + "\n");
        if (effect_type == EffectType.BaseHealth) g = Mathf.Floor(g);
        if (Mathf.Abs(g) > 10) { Debug.Log("mult " + multiplier + " level " + level + " difficulty " + difficulty + "\n"); }

        // if (effect_type == EffectType.Laser) Debug.Log(rune_type + " " + effect_type + " level " + check_level + " -> " + g + "\n");

        //fuck this shit
        //if (type == EffectType.Calamity || type == EffectType.Swarm || type == EffectType.Force) return g; else return (Mathf.Round(g * 10f) / 10f);
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
        float difficulty = Peripheral.Instance.difficulty;
        difficulty = (difficulty == 0) ? Central.Instance.medium_difficulty : difficulty;

        float g = recharge_time;
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
        level = lvl;
    }

   

}


