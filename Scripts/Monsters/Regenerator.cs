using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum RegeneratorType{Both, Self, Neighbors};

public class Regenerator : Modifier
{
	public float period;  // |xxx----------|  DURATION duration < period
	public float duration;// |-------------| PERIOD
	public float total_time = -1;    // for how long
	public float rate;
	public HitMe my_hitme;
	public RegeneratorType type;
	public float range = 0;
	float repeat_rate = 0.15f;
	float heal_duration;
	float init_mass; //for xp purposes for DOT
	List<HitMe> friends;
    MyArray<HitMe> monsters = null;
	float TIME;
	bool am_doing_stuff;
	bool done_for_good;
	public bool auto_run;
    bool first_invoke = true;
    List<Firearm> towers = new List<Firearm>();
    int towerID;
    Firearm my_firearm;
    public string lava_name = "Wishes/dot_lava";
    Lava my_lava;

    void Start () {				
		if (auto_run) done_for_good = false;
		if (duration > period) {
			period = -1;
			//duration = -1;
		}//lol what		
		heal_duration = duration;
		if (type != RegeneratorType.Self && range <= 0) type = RegeneratorType.Self;
		
	}
	

    //regenerator doing damage => has negative stats[0]
	public float Init(HitMe _hitme, float[] stats, RegeneratorType _type, int _towerID){		
		heal_duration = -1;
		type = _type;
		my_hitme = _hitme;
        if (_towerID != 0 && towerID != _towerID)
        {
            towerID = _towerID;
            my_firearm = getFirearm(towerID);
        }
        monsters = null;
        duration = stats[1];
        EnableVisuals(_hitme, duration);
        TIME = 0f;
        if (is_active)
        {                                           
            if (my_lava != null) my_lava.lifespan = duration;
            return 0f;
        }
        float finisher_percent = (stats.Length == 5) ? stats[3] : 0;
        if (finisher_percent > 0 && UnityEngine.Random.RandomRange(0, 1) < finisher_percent)
        {
          //  SetEverythingOnFire(stats);
            //    Debug.Log("Fin\n");
        }

        total_time = duration + .1f; // this isn only intended for 1 time use, not repeating
        period = -1;
		rate = repeat_rate * stats[0] / duration;
		type = _type;
		done_for_good = false;
        is_active = true;
        am_doing_stuff = false;
        first_invoke = true;
    //    Debug.Log("initializing regenerator total time " + total_time + " duration " + duration + " rate " + rate 
     //                   + " repeat_rate " + repeat_rate + " stat " + stats[0] + "\n");        
        return 0;
	}

    void SetEverythingOnFire(float[] stats)
    {
        my_lava = Zoo.Instance.getObject(lava_name, false).GetComponent<Lava>();
        //my_lava.SetLocation(this.transform, this.transform.position, stats[1], Quaternion.identity);

        StatBit[] lava_statbits = new StatBit[1];
        lava_statbits[0] = new StatBit(EffectType.Fear, stats[0], 1, true);
        StatSum lava_stats = new StatSum(1, 0, lava_statbits, RuneType.Vexing);
        my_lava.SetLocation(my_hitme.transform, my_hitme.transform.position, stats[1], Quaternion.identity);
        my_lava.Init(lava_stats, duration, true, null);
        my_lava.gameObject.SetActive(true);
    }

    void OnEnable(){
		TIME = 0f;
		if (auto_run) done_for_good = false;
	}
	
	// Update is called once per frame
	protected override void YesUpdate () {

        if (done_for_good) { return; }

        TIME += Time.deltaTime;
		if (period == -1) {
            if (!am_doing_stuff)
            {
          //      Debug.Log("Starting " + this.gameObject.name + "\n");
                InvokeRepeating("HealMe", 0, repeat_rate);
                am_doing_stuff = true;
                return;
            }
            else if (TIME > duration)
            {
            //    Debug.Log("Stopping " + this.gameObject.name + " TIME " + TIME + " duration " + duration + "\n");
                StopMe();
                return;
            }
            
            return;
		}

        //healing stuff
        if (total_time != -1 && TIME > total_time)
        {
            StopMe();
            return;
        }

        if ((Mathf.Repeat (TIME, period) < duration) && !am_doing_stuff) {
			if (type != RegeneratorType.Self) GetFriends();
			InvokeRepeating("HealMe", 0, repeat_rate);			
			am_doing_stuff = true;
		} else if (Mathf.Repeat (TIME, period) >= duration && am_doing_stuff){
            CancelInvoke("HealMe");
            am_doing_stuff = false;
            first_invoke = true;			
		}

		
	}


    void EnableVisuals(HitMe me, float timer)
    {
        if (rate > 0)
            me.EnableVisuals(MonsterType.Mass, timer);
        else
            me.EnableVisuals(MonsterType.Regeneration, timer);
    }

    void HealMe()
    {
        float[] stats = new float[2];
        stats[0] = -rate;
        stats[1] = heal_duration;
        //Debug.Log("Regenerator doing its thing\n");
        if (type != RegeneratorType.Neighbors)
        {            
            float xp = my_hitme.MassMe(stats);
       //     Debug.Log("Hurting for " + xp + " stat " + stats[0]  + "\n");
            if (my_firearm != null) my_firearm.addXp(xp);
                
            if (first_invoke) EnableVisuals(my_hitme, duration);
           
        }
        if (type != RegeneratorType.Self)
        {
            foreach (HitMe friend in friends)
            {
                friend.MassMe(stats);
                if (first_invoke) EnableVisuals(friend, duration);
            }
        }
        first_invoke = false;
    }

    protected override void SafeDisable()
    {
        towers = new List<Firearm>();
        StopMe();
    }

    Firearm getFirearm(int ID)
    {
        foreach (Firearm f in towers)        
            if (f.gameObject.GetInstanceID() == ID) return f;
        
        foreach (Firearm f in Peripheral.Instance.firearms)        
            if (f.gameObject.GetInstanceID() == ID)
            {
                towers.Add(f);
                return f;
            }       
        return null;
    }

    public void OnDisable(){
        
        StopMe();
	}
	
	
	public void StopMe(){
		CancelInvoke("HealMe");
        //	Debug.Log("Disabling regenertor");
        done_for_good = true;
	}

    void GetFriends() {
        friends = new List<HitMe>();
        if (monsters == null) { monsters = Peripheral.Instance.targets; }
        for (int i = 0; i < monsters.max_count; i++)
        {
            HitMe friend = monsters.array[i];
            if (friend == null || friend.amDying() || !friend.gameObject.activeSelf) continue;

            if (Vector2.Distance(friend.transform.position, this.transform.position) < range)            
                friends.Add(friend);
            
        }
    			
		
		
	}
	

}
