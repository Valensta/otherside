using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine.EventSystems;


public class Toy : MonoBehaviour {
    public bool active;
    
    public float TIME = 0;
    public float TIME_AT_START = 0;
    public string my_name;
    public unitStats stats;
    public GameObject death_effect_object;
    public Building building; //everybody has a building
    public Firearm firearm;   //not everybody has a firearm
    public string[] target_toys = new string[0];

    public Transform center;
    public List<Toy> child_toys = new List<Toy>();
    public Toy parent_toy; // ghost (child) to sensible city (parent) relationship for xp allocation and upgrade control

    public Island_Button island;
    public string death_effect;
    public tower_stats my_tower_stats = new tower_stats();
    public ToyType toy_type;
    public RuneType runetype;
    public Rune rune = new Rune();
    public Mini_Toy_Button_Driver rune_buttons;

    

    public float tilesize;

    public bool Active
    {
        get
        {
            return active;
        }

        set
        {            
            active = value;
         //   Debug.Log("Toy set active " + active + " " + this.gameObject.name + "\n");
        }
    }

    public delegate void PriceUpdateHandler(string type, float price);
    public static event PriceUpdateHandler onPriceUpdate;

    


    public void SetBonus(float bonus) {
        //if (toy_type == ToyType.Building)
        rune.distance_bonus = bonus;
       
    }

    
    public void initStats(unitStats s, Vector3 scaleV, Island_Button i, string _name, Rune _rune)    {
        tilesize = i.getPeripheral().tileSize;
        stats = s;
        float dmg_base = 0;
        my_name = _name;
        my_tower_stats = new tower_stats();
        my_tower_stats.island_name = string.Copy(i.ID);
        my_tower_stats.wave_time = Sun.Instance.current_time_of_day;
        my_tower_stats.initSkillStats(runetype);

        float bonus = StaticRune.GetDistanceBonus(my_name, this.transform.position, this);

        if (_rune == null || toy_type != ToyType.Hero)
        {
            rune = new Rune();
            SetBonus(bonus);
            rune.initStats(runetype, stats.getMaxLvl(), toy_type);
        }
        else
        {
            SetBonus(bonus);
            rune = _rune;
            rune.UpdateStats();
        }
        Active = true;
        island = i;
        rune.ID = this.gameObject.GetInstanceID();
        center.localPosition = Vector3.Scale(center.localPosition, scaleV);
        //        my_toy.center.localPosition = Vector3.Scale(my_toy.center.localPosition, scaleV);
        transform.transform.localRotation = Quaternion.identity;

        building.initStats(this);
    //
        

       // if (rune.level > 0 && rune_buttons != null) rune_buttons.UpdateMe();

        if (parent_toy != null) parent_toy.AddTargetToy(this);


        if (toy_type == ToyType.Hero)
        {
            //rune.InitHero(Central.Instance.current_lvl);//for testing, gives hero some xp based on current level, if xp is 0  
            // also sensible hero comes with airattack for free, done by an event in 02level_scene
            rune.setMaxLevel((int)Mathf.Max(Central.Instance.getToy(my_name).getMaxLvl(), rune.getMaxLevel()));
        }

        if (firearm != null) firearm.initStats(this);
        //if (rune_buttons != null) rune_buttons.UpdateMe(); // ghost towers don't get their own rune buttons
        TIME_AT_START = Time.time;
    }

    public ToySaver getSnapshot()
    {        
        float ammo = (firearm !=null)? firearm.Ammo() : -1;

#if UNITY_EDITOR
        return new ToySaver(string.Copy(my_name), ammo, rune.getSnapshot(), toy_type, building.current_construction_time, my_tower_stats.DeepClone());
#else
        //toooo sllooowwww
           return new ToySaver(string.Copy(my_name), ammo, rune.getSnapshot(), toy_type, building.current_construction_time, new tower_stats());
#endif
    }

    public CompleteToySaver getCompleteSnapshot()
    {
        float ammo = (firearm != null) ? firearm.Ammo() : -1;
        return new CompleteToySaver(my_name, ammo, rune, toy_type, building.current_construction_time);
    }

    public void loadSnapshot(ToySaver saver)
    {
        rune = new Rune();
        rune.loadSnapshot(saver.rune_saver);
        building.loadSnapshot(saver);
        if (firearm != null) firearm.loadSnapshot(saver);
                
        if (rune_buttons != null) rune_buttons.UpdateMe();        
        rune.UpdateStats();
#if UNITY_EDITOR
        my_tower_stats = saver.tower_stats.DeepClone();
#else //ugh awkward
        my_tower_stats = new tower_stats();
        my_tower_stats.island_name = string.Copy(island.transform.parent.name);
        my_tower_stats.wave_time = Sun.Instance.current_time_of_day;
        my_tower_stats.initSkillStats(runetype);
#endif

    }

    public void loadSnapshot(RuneSaver rune_saver)
    {
        rune = new Rune();
        rune.loadSnapshot(rune_saver);        

        if (rune_buttons != null) rune_buttons.UpdateMe();
        rune.UpdateStats();

    }

    public bool isToyATarget(string s)
    {
        bool yes = false;
        foreach (string toy in target_toys)
            if (toy.Equals(s)) yes = true;
        return yes;
    }

    public void InitRuneButtons()
    {
        if (rune_buttons != null)
        {
            rune_buttons.InitMiniDriver(this);
            return;
        }
        
        GameObject buttons = Peripheral.Instance.zoo.getObject("GUI/rune_buttons", false);
        rune_buttons = buttons.GetComponent<Mini_Toy_Button_Driver>();
        buttons.transform.SetParent(this.transform);
        buttons.transform.localPosition = Vector3.zero;

        buttons.SetActive(true);
        rune_buttons.InitMiniDriver(this);
    }

 

    public int getSellCost(){
		return (rune.toy_type == ToyType.Hero)? 0 : Mathf.FloorToInt((stats.cost_type.Amount + rune.invested_cost) * 0.75f);
	
	}


    public float getTime()
    {
        return TIME;
        //return Sun.Instance.current_time_of_day;
    }


    public void ResetSkills()
    {
        rune.resetSkills(false);        
        
    }

    public void UpgradeRune(ToyType toy_type, EffectType effect_type)
    {
        float hey = rune.Upgrade(effect_type, true);
        if (hey != 0) UpgradeTargetGhosts();
    }

	public void Update ()
	{
        if (!Active) { return; }     
		
		TIME += Time.deltaTime;
       // Debug.Log(TIME + " " + Duration.deltaTime + "\n");
        
			
	}    
		
	public void OnDisable()
    {
       if(firearm != null) firearm.Active = false;        
        Active = false;
        
    }

    public void DisableMe() //for testing via FakePlayer only!
    {
        Active = false;
        if (firearm != null) firearm.Active = false;
    }

	public void Die (float delay)
	{		
		if (tag.Equals ("Player") && toy_type == ToyType.Normal && runetype != RuneType.Modulator)
			Peripheral.Instance.decrementToys();
		if (island != null) {
            island.my_toy = null;
            island.blocked = false;
            transform.parent = null;            
		}


        
		StartCoroutine (DiePretty(delay));
	}

	IEnumerator DiePretty (float delay){
    
        if (death_effect.Equals("")) {
			death_effect = "Toys/toy_death_puffy_pale_blue";			
		}
		if (death_effect_object == null){
			death_effect_object = Peripheral.Instance.zoo.getObject (death_effect, false);
			death_effect_object.transform.position = this.transform.position;			
		}
		death_effect_object.transform.position = this.transform.position;
		death_effect_object.SetActive(true);
		yield return new WaitForSeconds (.5f);
		StopAllCoroutines ();
		
		if (toy_type == ToyType.Temporary) island.MakeDeadIsland(-1);
        
        if (runetype == RuneType.SensibleCity) Peripheral.Instance.addedCity(false, this);

        Peripheral.Instance.zoo.returnObject (this.gameObject);

		//Peripheral.Instance.zoo.returnObject (island.parent, true);
		
	}



    public void AddTargetToy(Toy toy)
    {
        if (rune_buttons == null) InitRuneButtons();

        foreach (Toy f in child_toys)
        {
            if (f.my_name.Equals(toy.my_name)) return;
        }
        child_toys.Add(toy);
        UpgradeTargetGhosts();
    }

    public void RemoveTargetToy(Toy toy)
    {
        for (int i = 0; i < child_toys.Count; i++)
        {
            if (child_toys[i].my_name.Equals(toy.my_name)) child_toys.RemoveAt(i);
        }
    }


    void UpgradeTargetGhosts()
    {
        if (child_toys.Count == 0) return;

        StatSum my_stats = rune.GetStats(false);
        float tower_force = rune.getLevel(EffectType.TowerForce);
        float tower_range = rune.getLevel(EffectType.TowerRange);
        foreach (Toy f in child_toys)
        {
            int i = 0;
            while (f.rune.getLevel(EffectType.Force) < tower_force && i < 10)
            {
                f.rune.Upgrade(EffectType.Force, false);
             //   Debug.Log(f.name + "am force level " + f.rune.getLevel(EffectType.Force) + " want to be level " + tower_force + "\n");
                i++;
            }
            i = 0;
            while (f.rune.getLevel(EffectType.Range) < tower_range && i < 10)
            {
                Debug.Log(f.name + "am range level " + f.rune.getLevel(EffectType.Range) + " want to be level " + tower_range + "\n");
                f.rune.Upgrade(EffectType.Range, false);
                i++;
            }

        }


    }



}
