using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

//AKA SKILL TREE
[System.Serializable]
public class Rune
{
    public RuneType runetype = RuneType.Null;
    public StatBit[] stats;
    public StatSum stat_sum;
    public StatSum special_stat_sum;
    public int ID = -1;
    //float stat_mult = 0.1f;
    public int level = 0;
    public float[] xp_reqs;
    //float upgrade_cost = 2f;
    public float xp = 0;
    int max_level = 1;
    float base_upgrade_cost = 2f; //used for sell cost
    public float invested_cost;
    
    public float distance_bonus = 0f;
    
    public bool hero;
    
    public ToyType toy_type;
    float reloadTime_base = 1f;
    //public Cost cost_type = new Cost();
    public float init_cost = 1f;  //to upgrade per level
    public List<EffectType> exclude_skills = new List<EffectType>();
    //public List<EffectType> upgraded_list = new List<EffectType>();
    public int basic_cost = 20;

    public delegate void OnLevelUpHandler(RuneType type);
    public static event OnLevelUpHandler onLevelUp;

    public delegate void OnUpgradeHandler(EffectType type, int ID);
    public static event OnUpgradeHandler onUpgrade;


    public Rune()
    {
    }
    
    /*
    public void AddUpgradeToList(EffectType type)
    {
        for (int i = 0; i < upgraded_list.Count; i++)
            if (upgraded_list[i] == type) return;
        upgraded_list.Add(type);
    }*/

    public void initStats(RuneType rtype, int _max_lvl, ToyType _toy_type, List<EffectType> exclude_us)
    {        
        runetype = rtype;
        if (_max_lvl < 1) _max_lvl = 1;
        setMaxLevel(_max_lvl);
        foreach (EffectType e in exclude_us)
        {
            exclude_skills.Add(e);
            //   Debug.Log("Rune added skill " + e + " to exclude list\n");
        }
        toy_type = _toy_type;
        invested_cost = 0;

        switch (runetype)
        {
            case RuneType.Airy:
                initAiry();
                break;
            case RuneType.Sensible:
                initSensible();
                break;
            case RuneType.Vexing:
                initVexing();
                break;
            case RuneType.Castle:
                initCastle();
                break;
            case RuneType.SensibleCity:
                initSensibleCity();
                break;
            case RuneType.Slow:
                initSlow();
                break;
            case RuneType.Fast:
                initFast();
                break;
            case RuneType.Time:
                initTime();
                break;
            case RuneType.Modulator:
                initModulator();
                break;

        }

        xp_reqs = new float[20];
        
        switch (toy_type)
        {
            case ToyType.Hero:
                xp_reqs[0] = 60f;
                break;
            case ToyType.Normal:
                xp_reqs[0] = 40f;
                break;
            case ToyType.Building:
                if (runetype == RuneType.Castle)
                    xp_reqs[0] = 0;
                else
                    xp_reqs[0] = 10f;
                break;
            case ToyType.Temporary:
                xp_reqs[0] = 100000f;
                break;
        }
        for (int i = 1; i < 20; i++)
        {
            xp_reqs[i] = xp_reqs[i - 1] * 2f;
        }
     //   setMaxLevel(xp_reqs.Length - 1);
        UpdateStats();
    }
    //public Stat(EffectType mytype, float effect, float mult, float price, bool active, float diff, StatReq[] reqs){
    //(base_effect + multiplier*level)/(1-(1-difficulty)/3f);


    public void InitHero(int level)
    {
        if (xp > 0) return;
        if (runetype == RuneType.Sensible) level = level - 1;
        if (runetype == RuneType.Airy) level = level - 2;
        if (runetype == RuneType.Vexing) level = level - 3;        

        if (level > 0) Debug.Log("Initializing Hero " + runetype + " to toy level " + level + "\n");
        else return;
        float dreams = 15f;

        

        for (int i = 0; i < Mathf.Min(level,max_level); i++)
        {
            addXp(xp_reqs[i] + 1);
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
                me = (hey < 0.3333f) ? EffectType.EMP : (hey < 0.66666) ? EffectType.Plague : EffectType.Quicksand;
            else
                me = (hey < 0.5f) ? EffectType.Plague : EffectType.Quicksand;
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


    void initSensible()
    {
        bool is_hero = (toy_type == ToyType.Hero);
        bool is_active = true;
        if (is_hero)
            stats = new StatBit[8];
        else
            stats = new StatBit[7];

        /// BASIC SHIT
               
        StatReq[] rt = new StatReq[1];
        rt[0] = new StatReq(EffectType.ReloadTime, 0);
        stats[0] = new StatBit(this, RuneType.Sensible, EffectType.ReloadTime, is_active, rt, 0, 0, is_hero);
        //(RuneType.Sensible, EffectType.ReloadTime, is_active, rt, is_hero);

        StatReq[] r = new StatReq[1];
        r[0] = new StatReq(EffectType.Range, 0);
        stats[1] = new StatBit(this, RuneType.Sensible, EffectType.Range, is_active, r, 0, 0, is_hero);
        //(RuneType.Sensible, EffectType.Range, is_active, r, is_hero);

        StatReq[] f = new StatReq[1];
        f[0] = new StatReq(EffectType.Force, 0);
        stats[2] = new StatBit(this, RuneType.Sensible, EffectType.Force, is_active, f, 0, 0, is_hero);
        //(RuneType.Sensible, EffectType.Force, is_active, f, is_hero);

        /// UPGRADES
        is_active = false;
        /// POWER/RAPID FIRE BRANCH

        StatReq[] rapid_fire = new StatReq[1];
        rapid_fire[0] = new StatReq(EffectType.Laser, -1);        
        stats[3] = new StatBit(this, RuneType.Sensible, EffectType.RapidFire, is_active, rapid_fire, 0, 0, is_hero);
        //(RuneType.Sensible, EffectType.RapidFire, is_active, rapid_fire, is_hero);

        StatReq[] DOT = new StatReq[1];
        DOT[0] = new StatReq(EffectType.RapidFire, 1);
        stats[4] = new StatBit(this, RuneType.Sensible, EffectType.DOT, is_active, DOT, 0, 0, is_hero);
        //(RuneType.Sensible, EffectType.DOT, is_active, DOT, is_hero);

        /// LASER BRANCH

        StatReq[] l = new StatReq[1];
        l[0] = new StatReq(EffectType.RapidFire, -1);        
        stats[5] = new StatBit(this, RuneType.Sensible, EffectType.Laser, is_active, l, 0, 0, is_hero);
        //(RuneType.Sensible, EffectType.Laser, is_active, l, is_hero);


        StatReq[] l2 = new StatReq[1];
        l2[0] = new StatReq(EffectType.Laser, 1);
        stats[6] = new StatBit(this, RuneType.Sensible, EffectType.Sparkles, is_active, l2, 0, 0, is_hero);
        //(RuneType.Sensible, EffectType.Sparkles, is_active, l2,  is_hero);        

        /// HERO STUFF
        if (is_hero)
        {
            StatReq[] aa = new StatReq[1];
            aa[0] = new StatReq(EffectType.Force, 0);
            stats[7] = new StatBit(this, RuneType.Sensible, EffectType.AirAttack, is_active, aa, 70, 0, is_hero);
            //(RuneType.Sensible, EffectType.AirAttack, is_active, aa, is_hero);
            //stats[7].SetRechargeTime(120f);
        }
    }



    void initAiry()
    {        
        Sun.OnDayTimeChange += OnDayTimeChange;
        bool is_hero = (toy_type == ToyType.Hero);
        bool is_active = true;
        if (is_hero)
            stats = new StatBit[11];
        else
            stats = new StatBit[8];

        //this damage should be duration of effect
        StatReq[] rt = new StatReq[1];        
        rt[0] = new StatReq(EffectType.ReloadTime, 0);
        stats[0] = new StatBit(this, RuneType.Airy, EffectType.ReloadTime, is_active, rt, 0, 0, is_hero);
        //(RuneType.Airy, EffectType.ReloadTime, is_active, rt, is_hero);

        StatReq[] r = new StatReq[1];
        r[0] = new StatReq(EffectType.Range, 0);
        stats[1] = new StatBit(this, RuneType.Airy, EffectType.Range, is_active, r, 0, 0, is_hero);
        //(RuneType.Airy, EffectType.Range, is_active, r, is_hero);

        //this damage should be speed reduction
        StatReq[] def = new StatReq[1];
        def[0] = new StatReq(EffectType.Speed, 0);
        stats[2] = new StatBit(this, RuneType.Airy, EffectType.Speed, is_active, def, 0, 0, is_hero);
        //(RuneType.Airy, EffectType.Speed, is_active, def, is_hero);

        StatReq[] f = new StatReq[1];
        f[0] = new StatReq(EffectType.Force, 0); //paired with speed, updateded when speed is upgraded
        stats[3] = new StatBit(this, RuneType.Airy, EffectType.Force, is_active, f, 0, 0, is_hero);
        //(RuneType.Airy, EffectType.Force, is_active, f, is_hero);

        //UPGRADES
        is_active = false;

        StatReq[] m = new StatReq[2];
        m[0] = new StatReq(EffectType.Weaken, -1);
        m[1] = new StatReq(EffectType.WishCatcher, -1);
        stats[4] = new StatBit(this, RuneType.Airy, EffectType.Calamity, is_active, m, 0, 0, is_hero);       
        //(RuneType.Airy, EffectType.Calamity, is_active, m, is_hero);       

        stats[5] = new StatBit(this, RuneType.Airy, EffectType.Swarm, is_active, m, 0, 0, is_hero);       
        //(RuneType.Airy, EffectType.Swarm, is_active, m, is_hero);


        StatReq[] w = new StatReq[2];
        w[0] = new StatReq(EffectType.Calamity, -1);
        w[1] = new StatReq(EffectType.Swarm, -1);
        stats[6] = new StatBit(this, RuneType.Airy, EffectType.Weaken, is_active, w, 0, 0, is_hero);
        //(RuneType.Airy, EffectType.Weaken, is_active, w, is_hero);        
        stats[7] = new StatBit(this, RuneType.Airy, EffectType.WishCatcher, is_active, w, 0, 0, is_hero);
        //(RuneType.Airy, EffectType.WishCatcher, is_active, w, is_hero);

        if (toy_type == ToyType.Hero)
        {
           
            stats[8] = new StatBit(this, RuneType.Airy, EffectType.Quicksand, is_active, def, 60, 0, is_hero);
            //(RuneType.Airy, EffectType.Quicksand, is_active, def, is_hero);
            //stats[8].SetRechargeTime(120f);//this is multiplied by 10 in the end

           //same requirements anyway
            stats[9] = new StatBit(this, RuneType.Airy, EffectType.EMP, is_active, def, 60, 0, is_hero);
            //(RuneType.Airy, EffectType.EMP, is_active, def, is_hero);
            //stats[9].SetRechargeTime(120f);//this is multiplied by 10 in the end

            stats[10] = new StatBit(this, RuneType.Airy, EffectType.Plague, is_active, def, 60, 0, is_hero);
            //(RuneType.Airy, EffectType.Plague, is_active, def, is_hero);
            //stats[10].SetRechargeTime(120f);//this is multiplied by 10 in the end
        }

    }

    
    void initVexing()
    {
        int i = 0;
        bool is_hero = (toy_type == ToyType.Hero);
        bool is_active = true;
        if (is_hero)
            stats = new StatBit[10];
        else
            stats = new StatBit[8];
        
        Sun.OnDayTimeChange += OnDayTimeChange;

        init_cost = basic_cost;//init cost for upgrades
        base_upgrade_cost = 5f;
        StatReq[] rt = new StatReq[1];
        rt[0] = new StatReq(EffectType.ReloadTime, 0);
        stats[i++] = new StatBit(this, RuneType.Vexing, EffectType.ReloadTime, is_active, rt, 0, 0, is_hero);
        //(RuneType.Vexing, EffectType.ReloadTime, is_active, rt, is_hero);

        StatReq[] r = new StatReq[1];
        r[0] = new StatReq(EffectType.Range, 0);
        stats[i++] = new StatBit(this, RuneType.Vexing, EffectType.Range, is_active, r, 0, 0, is_hero);
        //(RuneType.Vexing, EffectType.Range, is_active, r, is_hero);

        StatReq[] fr = new StatReq[1];
        fr[0] = new StatReq(EffectType.VexingForce, 0);
        stats[i++] = new StatBit(this, RuneType.Vexing, EffectType.VexingForce, is_active, fr, 0, 0, is_hero);
        //(RuneType.Vexing, EffectType.VexingForce, is_active, fr, is_hero);

        //UPGRADES
        is_active = false;

        //vexing Diffuse
        StatReq[] no_focus = new StatReq[1];
        no_focus[0] = new StatReq(EffectType.Focus, -1);
        stats[i++] = new StatBit(this, RuneType.Vexing, EffectType.Diffuse, is_active, no_focus, 0, 0, is_hero);
        //(RuneType.Vexing, EffectType.Diffuse, is_active, no_focus, is_hero);


        StatReq[] have_diffuse = new StatReq[1];
        have_diffuse[0] = new StatReq(EffectType.Diffuse, 1);        
        stats[i++] = new StatBit(this, RuneType.Vexing, EffectType.Transform, is_active, have_diffuse, 0, 0, is_hero);
        //(RuneType.Vexing, EffectType.Transform, is_active, have_diffuse, is_hero);


        //vexing Focus
        StatReq[] no_diffuse = new StatReq[1];
        no_diffuse[0] = new StatReq(EffectType.Diffuse, -1);
        stats[i++] = new StatBit(this, RuneType.Vexing, EffectType.Focus, is_active, no_diffuse, 0, 0, is_hero);
        //(RuneType.Vexing, EffectType.Focus, is_active, no_diffuse, is_hero);

        StatReq[] have_focus = new StatReq[1];
        have_focus[0] = new StatReq(EffectType.Focus, 1);
        stats[i++] = new StatBit(this, RuneType.Vexing, EffectType.Fear, is_active, have_focus, 0, 0, is_hero);
        //(RuneType.Vexing, EffectType.Fear, is_active, have_focus, is_hero);
        stats[i++] = new StatBit(this, RuneType.Vexing, EffectType.Critical, is_active, have_focus, 0, 0, is_hero);
        //(RuneType.Vexing, EffectType.Critical, is_active, have_diffuse, is_hero);


        if (toy_type == ToyType.Hero)
        {
            //teleport uses damage for % of teleport and teleport for where
            //teleport has to be < 0
            //damage has to be < 1, well, not really

            StatReq[] t = new StatReq[1];
            t[0] = new StatReq(EffectType.Teleport, 0);
            stats[i++] = new StatBit(this, RuneType.Vexing, EffectType.Teleport, is_active, t, 70, 0, is_hero);
            //(RuneType.Vexing, EffectType.Teleport, is_active, t, is_hero);
            //stats[i++].SetRechargeTime(120f);//this is multiplied by 10 in the end


            StatReq[] b = new StatReq[1];
            b[0] = new StatReq(EffectType.Bees, 0);
            stats[i++] = new StatBit(this, RuneType.Vexing, EffectType.Bees, is_active, b, 70, 0, is_hero);
            //(RuneType.Vexing, EffectType.Bees, is_active, b, is_hero);
            //stats[i++].SetRechargeTime(120f);//this is multiplied by 10 in the end

        }

        
    }




    void initSlow()
    {
        stats = new StatBit[3];
        Sun.OnDayTimeChange += OnDayTimeChange;
        bool is_hero = false;
        bool is_active = true;


        StatReq[] rt = new StatReq[1];
        rt[0] = new StatReq(EffectType.ReloadTime, 0);
        stats[0] = new StatBit(this, RuneType.Slow, EffectType.ReloadTime, is_active, rt, 0, 0, is_hero);
        //(RuneType.Slow, EffectType.ReloadTime, is_active, rt, is_hero);

        StatReq[] r = new StatReq[1];
        r[0] = new StatReq(EffectType.Range, 0);
        stats[1] = new StatBit(this, RuneType.Slow, EffectType.Range, is_active, r, 0, 0, is_hero);
        //(RuneType.Slow, EffectType.Range, is_active, r, is_hero);

        StatReq[] f = new StatReq[1];
        f[0] = new StatReq(EffectType.Force, 0);
        stats[2] = new StatBit(this, RuneType.Slow, EffectType.Force, is_active, f, 0, 0, is_hero);
        //(RuneType.Slow, EffectType.Force, is_active, f, is_hero);

    }


    void initFast()
    {
        stats = new StatBit[3];
        Sun.OnDayTimeChange += OnDayTimeChange;
        bool is_hero = false;
        bool is_active = true;

        StatReq[] rt = new StatReq[1];
        rt[0] = new StatReq(EffectType.ReloadTime, 0);
        stats[0] = new StatBit(this, RuneType.Fast, EffectType.ReloadTime, is_active, rt, 0, 0, is_hero);
        //(RuneType.Fast, EffectType.ReloadTime, is_active, rt, is_hero);

        StatReq[] r = new StatReq[1];
        r[0] = new StatReq(EffectType.Range, 0);
        stats[1] = new StatBit(this, RuneType.Fast, EffectType.Range, is_active, r, 0, 0, is_hero);
        //(RuneType.Fast, EffectType.Range, is_active, r, is_hero);

        StatReq[] f = new StatReq[1];
        f[0] = new StatReq(EffectType.Force, 0);
        stats[2] = new StatBit(this, RuneType.Fast, EffectType.Force, is_active, f, 0, 0, is_hero);
        //(RuneType.Fast, EffectType.Force, is_active, f, is_hero);
    }


    void initTime()
    {
        stats = new StatBit[3];
        Sun.OnDayTimeChange += OnDayTimeChange;
        bool is_hero = false;
        bool is_active = true;

        StatReq[] rt = new StatReq[1];       
        rt[0] = new StatReq(EffectType.ReloadTime, 0);
        stats[0] = new StatBit(this, RuneType.Time, EffectType.ReloadTime, is_active, rt, 0, 0, is_hero);
        //(RuneType.Time, EffectType.ReloadTime, is_active, rt, is_hero);

        StatReq[] r = new StatReq[1];
        r[0] = new StatReq(EffectType.Range, 0);
        stats[1] = new StatBit(this, RuneType.Time, EffectType.Range, is_active, r, 0, 0, is_hero);
        //(RuneType.Time, EffectType.Range, is_active, r, is_hero);

        StatReq[] f = new StatReq[1];
        f[0] = new StatReq(EffectType.TimeSpeed, 0);
        stats[2] = new StatBit(this, RuneType.Time, EffectType.TimeSpeed, is_active, f, 0, 0, is_hero);
        //(RuneType.Time, EffectType.TimeSpeed, is_active, f, is_hero);

    }

    void initModulator()
    {
        stats = new StatBit[0];        
    }


    void initCastle()
    {
        //Castle = + health, + wish % , construction efficiency
        bool is_hero = false;
        bool is_active = true;

        init_cost = 25f;
       base_upgrade_cost = 5f;

        stats = new StatBit[2];

        StatReq[] ph = new StatReq[1];
        ph[0] = new StatReq(EffectType.BaseHealth, 0);
        stats[0] = new StatBit(this, RuneType.Castle, EffectType.BaseHealth, is_active, ph, 0, 0, is_hero);
            //RuneType.Castle, EffectType.BaseHealth, is_active, ph, is_hero);

        StatReq[] ce = new StatReq[1];
        ce[0] = new StatReq(EffectType.Gnomes, 0);
        stats[1] = new StatBit(this, RuneType.Castle, EffectType.Gnomes, is_active, ce, 0, 0, is_hero);
        //(RuneType.Castle, EffectType.Gnomes, is_active, ce, is_hero);

    }




    void initSensibleCity()
    {
        //Sensible city = summon sensible temporary towers, base ammo
        stats = new StatBit[3];
        bool is_hero = false;
        bool is_active = true;

        StatReq[] tr = new StatReq[1];
        tr[0] = new StatReq(EffectType.TowerRange, 0);
        stats[0] = new StatBit(this, RuneType.SensibleCity, EffectType.TowerRange, is_active, tr, 0, 0, is_hero);
        //(RuneType.SensibleCity, EffectType.TowerRange, is_active, tr, is_hero);

        StatReq[] tf = new StatReq[1];
        tf[0] = new StatReq(EffectType.TowerForce, 0);
        stats[1] = new StatBit(this, RuneType.SensibleCity, EffectType.TowerForce, is_active, tf, 0, 0, is_hero);
        //(RuneType.SensibleCity, EffectType.TowerForce, is_active, tf, is_hero);

        StatReq[] r = new StatReq[1];
        r[0] = new StatReq(EffectType.Range, 0);
        stats[2] = new StatBit(this, RuneType.SensibleCity, EffectType.Range, is_active, r, 0, 0, is_hero);
        //(RuneType.SensibleCity, EffectType.Range, is_active, r, is_hero);

    }



    public void OnDayTimeChange(TimeName name)
    {
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
        for (int i = 0; i < stats.Length; i++)
        {
            if (!stats[i].active) continue;
            stats[i].setModifier(current_time_bonus, distance_bonus);
        }
    }
    
    public void UpdateStats()
    {
        //     Debug.Log("Updating stats " + stats.Length + "\n");
        int actives = 0;
        int special_actives = 0;
        for (int i = 0; i < stats.Length; i++)
        {
            bool special = Get.isSpecial(stats[i].effect_type);            
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
            if (!stats[i].active) continue;
            bool special = Get.isSpecial(stats[i].effect_type);
            stats[i].setModifier(current_time_bonus, distance_bonus);

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

        if (!Get.isSpecial(type) && isMaxLevel())
        {
            if (is_vocal) Debug.Log("Reached max level " + max_level + "\n");
            return false;
        }
        int s = getStatID(type);
        StatReq[] statreqs = stats[s].stat_reqs;


        if (statreqs != null && statreqs.Length > 0)
        {
            for (int i = 0; i < statreqs.Length; i++)
            {
                StatReq statreq = statreqs[i];

                if (!Peripheral.Instance.canBuildToy(statreq.required_toy)) return false;
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
                if (xp < statreq.xp)
                {
                    if (is_vocal) Debug.Log("Below required xp to upgrade " + type + "\n");
                    return false;
                }

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

    public float addXp(float _xp)
    {
        if (xp_reqs.Length <=  max_level) { Debug.Log("WTF\n"); return _xp; }
        if (isMaxLevel() || xp >= xp_reqs[max_level]) return _xp;
        
        float add_me = Mathf.Min(xp_reqs[max_level] - xp, _xp);
        xp += add_me;
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


    public float getMaxLevel()
    {
        return max_level;
    }

    public void setMaxLevel(int _max_level)
    {
   //     Debug.Log("RUNE Setting max level " + runetype + " " + toy_type + " to " + _max_level + "\n");
        max_level = _max_level;
    }


    public bool isMaxLevel()
    {
        return level >= max_level;
    }

    public float getXpToNextLevel()
    {
        if (isMaxLevel()) return 99999f;
        if (level >= 1) return xp_reqs[level] - xp_reqs[level - 1]; else return xp_reqs[level];
    }

    bool InExcludeList(EffectType skill)
    {
        foreach (EffectType e in exclude_skills)
        {
            if (e == skill) return true;
        }
        return false;
    }

    public bool CanUpgrade(EffectType type, RuneType _runetype)
    {
        return CanUpgrade(type, _runetype, false); 
    }

    public bool CanUpgrade(EffectType type, RuneType runetype, bool vocal )
    {
        vocal = false;
        if (!Get.isSpecial(type) && isMaxLevel() )
        {
         if(vocal)  Debug.Log("Too high level to upgrade " + type + "\n");
            return false;
        }

        if (!Get.isSpecial(type) && xp < xp_reqs[level])
        {
            if (vocal) Debug.Log("Not enough xp to upgrade " + type + "\n");
            return false;
        }
        if (InExcludeList(type))
        {
            if (vocal) Debug.Log("Skill is in exclusion list: " + type + "\n");
           return false;
        }

        int i = getStatID(type);

        if (i != -1)
        {

            if (stats[i].Level >= Get.MaxLvl(type))
            {
                if (vocal) Debug.Log("Skill " + type + " is already at its maximum " + Get.MaxLvl(type) + "\n");
                return false;
            }

            //ToyType check_this_type = ToyType.Temporary;
            //if (toy_type == ToyType.Building) check_this_type = ToyType.Building;

            if (!Peripheral.Instance.HaveResource(stats[i].cost))
            {

                if (vocal) Debug.Log("Cannot afford upgrade " + type + " for " + stats[i].cost.type + " " + stats[i].cost.Amount + "\n");
                return false;
            }
            else { if (vocal) Debug.Log("CAN afford upgrade " + type + " for " + stats[i].cost.type + " " + stats[i].cost.Amount + "\n"); }


            if (CheckStatReqs(type, runetype) == false)
            {
                if (vocal) Debug.Log("Skill  " + type + " stat reqs not met for upgrade\n");
                return false;
            }
        }
        else {
            if (vocal) Debug.Log("Invalid stat " + type + " " + runetype + "\n");
            return false;
        }

        if (onLevelUp != null) { onLevelUp(runetype); }
        return true;
    }

 
    
    public Cost GetUpgradeCost(EffectType type)
    {

        int s = getStatID(type);
        return (s != -1) ? StaticStat.getCost(type, stats[s].Level) : null;

    }
    
    public int getSellCost()
    {
        return Mathf.FloorToInt(level * base_upgrade_cost);

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
        switch (type)
        {
            case EffectType.Gnomes:
                //   Debug.Log("SPECIAL thing is " + s.level + " " + s.multiplier + " " + s.base_effect + " " + thing + "\n");
                Central.Instance.base_toy_cost_mult = getStatBit(type).getStats()[0];

                Central.Instance.updateCost(Peripheral.Instance.getToys());
                break;
            case EffectType.Speed:
                Upgrade(EffectType.Force, false);
                break;            
            default:
                StatBit g = special_stat_sum.GetStatBit(type);
                if (g == null) return;
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



    public float Upgrade(EffectType type, bool use_resource)
    {
        return Upgrade(type, use_resource, false);
    }

    public float Upgrade(EffectType type, bool use_resource, bool force)
    {//ghosts should be verifying with parent to see if they can upgrade
        if (!force && toy_type != ToyType.Temporary && !CanUpgrade(type, runetype, true)) { Debug.Log("You shouldn't be trying to upgrade this!"); return 0; }
        int i = getStatID(type);
        // FALSE USE_RESOURCE IS ONLY FOR TESTING or parent tower to ghost tower upgrades

        if (i != -1)
        {
            
            //	Debug.Log("Leveling up " + type + "\n");
            
            stats[i].active = true;
            if (use_resource) Peripheral.Instance.UseResource(stats[i].cost, Vector3.zero);
            stats[i].LevelUp(); //cost update happens here

            if (!Get.isSpecial(type))
            {
                level++;
                invested_cost += stats[i].cost.Amount;
            }
            UpdateStats();            

            if (type == EffectType.Range || type == EffectType.Focus)            
                Monitor.Instance.SetSignalSize(getRange()/2f); // to upgrade the currently displayed range signal            
            else if (Get.isSpecial(type))            
                DoSpecialThing(type);            
            else if (type == EffectType.BaseHealth)
                Peripheral.Instance.AdjustHealth(getStatBit(type).getStats()[0]);


           // float upgrade_cost = (Get.isSpecial(type)) ? base_upgrade_cost * 10 : base_upgrade_cost;
            //Debug.Log("type " + type + " is special? " + (Get.isSpecial(type)) + " base upgrade cost " + base_upgrade_cost + " init_cost " + init_cost + " lvl " + level + "\n");
            //stats[i].cost.Amount = init_cost + upgrade_cost * (stats[i].Level * 0.5f);
            //use skill level to determine upgrade cost, instead of rune level
            
            if (use_resource && ID != -1) onUpgrade(type, ID);
            return stats[i].cost.Amount;
        }
        else {
          //  Debug.Log("Trying to level up unknown stat " + type + ", not sure what to do\n");
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
public class StatReq{
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
	
	
	public StatReq(EffectType t, int lvl, string _toy){
		type = t;
		min_level = lvl;
		required_toy = _toy;
        
	}
}

