using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class InitLevel
{
    public InitStats init_stats;
    public InitToy[] toys;
    public InitWave[] waves;
    public InitWish[] wishes;
    
}

[System.Serializable]
public class InitStats
{
    public int dreams;        
    public int health;
    public string time_of_day;
    public int level_duration;    
    public float map_size_x;
    public float map_size_y;
    public LevelMod[] level_mod;
    public string env;

    public InitStats(int dreams, int health, TimeName time_of_day, int level_duration, int map_size_x, int map_size_y, LevelMod[] level_mod, EnvType env)
    {
        this.dreams = dreams;
        this.health = health;
        this.time_of_day = time_of_day.ToString();
        this.level_duration = level_duration;
        this.map_size_x = map_size_x;
        this.map_size_y = map_size_y;
        this.level_mod = level_mod;
        this.env = env.ToString();
    }

    public InitStats()
    {
        time_of_day = TimeName.Null.ToString();
        level_duration = 24;        
    }
}

[System.Serializable]
public class InitToy
{    
    public string name;
    public int max_lvl = 0;        
    //these are for the init file
    public string toy_type = null;
    public int cost;
    public int ammo;
    public string arrow = null;
    public string required_building = null;
    public string island_type = null;
    public string rune_type = null;
        
//{"name":"slow_ghost",          "toy_type": "temporary","cost":"2","scale":{"x":"1","z":"1"},"ammo":"5", "arrow":"slow","required_building":"sensible_city","islandtype":"temporary","rune_type":"Slow"}

   
    public RuneType getRuneType()
    {
        RuneType rt = EnumUtil.EnumFromString<RuneType>(rune_type, RuneType.Null);
        if (rt == RuneType.Null) { Debug.LogError("Attempting to get an invalid runetype from InitToy " + name + "\n"); }
        return rt;
    }

    public InitToy(string name, int max_lvl)
    {
        this.name = name;
        this.max_lvl = max_lvl;        
    }

    public bool hasMaxLvl() { return max_lvl > 0; }
    

    public InitToy()
    {
                
    }
}


[System.Serializable]
public class InitWave
{
    public string time_start;
    public string time_end;
    public float time_change_percent;
    public int points;    
    public int wait_time;
    public InitWavelet[] wavelets;
    public int id; 

    public InitWave(TimeName time_start, TimeName time_end, float timeChangePercent, int points, int wait_time, InitWavelet[] wavelets)
    {
        this.time_start = time_start.ToString();
        this.time_end = time_end.ToString();
        this.time_change_percent = timeChangePercent;
        this.points = points;        
        this.wait_time = wait_time;
        this.wavelets = wavelets;
    }
    

    public InitWave()
    {

    }

    //{"mode":"list","time_start":"dawn","time_change_at":"day","time_change_percent":"0.8","points":"60","xp":"55","wait_time":"3","1":{"interval":"1", "lull":"12","list":"soldier,5"},"2":{"interval":"1.3", "lull":"10","list":"magical,2"},"3":{"interval":"1.5", "lull":"12","list":"soldier,2,magical,1,soldier,2"},"4":{"interval":"1.5", "lull":"6","list":"soldier,6"},"5":{"interval":"1.5", "lull":"20","list":"plane,1,soldier,3"}}
}

[System.Serializable]
public class InitWavelet: IDeepCloneable<InitWavelet>
{
    public float interval; //between enemies in a group
    public float lull;       //between groups of enemies
    public int end_wait;   //wait at end of wavelet
    public InitEnemyCount[] enemies;
    private int monster_count = 0;
    private float run_time = 0;

    public InitWavelet DeepClone()
    {
        InitWavelet my_clone = new InitWavelet();
        my_clone.enemies = CloneUtil.copyArray<InitEnemyCount>(enemies);
        my_clone.interval = this.interval;
        my_clone.lull = this.lull;
        my_clone.monster_count = this.monster_count;
        my_clone.run_time = this.run_time;
        my_clone.end_wait = this.end_wait;

        return my_clone;
    }

    public int getEndWait()
    {
        return (end_wait != 0) ? end_wait : 30;
    }

    object IDeepCloneable.DeepClone()
    {
        return this.DeepClone();
    }

    public InitWavelet(float interval, int lull, InitEnemyCount[] enemies)
    {
        this.interval = interval;
        this.lull = lull;
        this.enemies = enemies;

       

        run_time = lull + GetMonsterCount() * interval;
    }

    public InitWavelet() { }

    public void Modify(float mult)
    {
        lull *= mult;
        interval *= mult;
        end_wait = Mathf.FloorToInt(end_wait * mult);
    }
    
    public float GetMonsterCount()
    {
        if (enemies == null) return 0;
        if (monster_count == 0)
        for (int i = 0; i < enemies.Length; i++)
            monster_count += enemies[i].c;


        
        return monster_count;
    }
    
    public float GetTotalRunTime(bool old_school)
    {
        if (old_school) return lull + GetMonsterCount() * interval;

        float run_time = (enemies.Length - 1) * lull;
        
        foreach (InitEnemyCount e in enemies) run_time += (e.c - 1) * interval;
                
        return run_time;
    }



}

[System.Serializable]
public class InitEnemyCount : IDeepCloneable<InitEnemyCount>
{
    public string name;
    public int c;
    public int p;

   

    public InitEnemyCount(string name, int number, int path)
    {
        this.name = name;
        this.c = number;
        this.p = path;
    }
    public InitEnemyCount() { }

    object IDeepCloneable.DeepClone()
    {
        return this.DeepClone();
    }

    public InitEnemyCount DeepClone()
    {
        InitEnemyCount my_clone = new InitEnemyCount();
        my_clone.name = string.Copy(this.name);
        my_clone.c = this.c;
        my_clone.p = this.p;
        return my_clone;
    }



}


//{"wishtype":"moredamage", "count":"8"}
[System.Serializable]
public class InitWish
{
    public string wishtype;
    public int count;

    public InitWish(WishType wishtype, int count)
    {
        this.wishtype = wishtype.ToString();
        this.count = count;
    }
}