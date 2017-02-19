using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;


public abstract class Toy : MonoBehaviour {
    public bool active;
    public float TIME = 0;
    public string my_name;
    public actorStats stats;
    public GameObject death_effect_object;

    public Transform center;
    public List<Firearm> child_toys = new List<Firearm>();
    public Toy parent_toy; // ghost (child) to sensible city (parent) relationship for xp allocation and upgrade control

    public Island_Button island;
    public string death_effect;
    public tower_stats my_tower_stats;
    public ToyType toy_type;
    public RuneType runetype;
    public Rune rune = new Rune();
    public Mini_Toy_Button_Driver rune_buttons;

    public float tilesize;
    public delegate void PriceUpdateHandler(string type, float price);
    public static event PriceUpdateHandler onPriceUpdate;

    //DO WE NEED THIS HERE??!
    public void addXp(float xp) {
        if (toy_type == ToyType.Temporary) return;

        rune.addXp(xp);
        if (GameStatCollector.Instance != null) my_tower_stats.Xp = rune.getXp();
        rune_buttons.UpdateMe();
    }

    abstract public void initStats(actorStats s, Vector3 scaleV, Island_Button i, string _name, Rune _rune);


    public void SetBonus(float bonus) {
        //if (toy_type == ToyType.Building)
        rune.distance_bonus = bonus;
       
    }

    protected abstract void onUpgrade(EffectType type, int ID);    

    public void ToyInit(actorStats s, Vector3 scaleV, Island_Button i, string _name, Rune _rune)
    {
        tilesize = i.getPeripheral().tileSize;

        stats = s;
        float dmg_base = 0;
        //dmg_base = stats.dmg;
        my_name = _name;

        Rune.onUpgrade += onUpgrade;
            

        float bonus = StaticRune.GetDistanceBonus(my_name, this.transform.position, this);
        
        
        if (_rune == null || toy_type != ToyType.Hero)
        {
            rune = new Rune();
            SetBonus(bonus);
            rune.initStats(runetype, stats.getMaxLvl(), toy_type, s.exclude_skills);
        }
        else
        {
            SetBonus(bonus);
            rune = _rune;
            rune.UpdateStats();            
        }
        active = true;

        island = i;

        rune.ID = this.gameObject.GetInstanceID();

        center.localPosition = Vector3.Scale(center.localPosition, scaleV);


      //  my_tower_stats = GameStatCollector.Instance.addTowerStats(this);


    }

    public void InitRuneButtons()
    {
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



    public void UpgradeRune(ToyType toy_type, EffectType effect_type)
    {
        float hey = rune.Upgrade(effect_type, true);
        if (hey != 0) UpgradeTargetGhosts();
    }

	protected void ToyUpdate ()
	{
        if (!active) { return; }     
		
		TIME += Time.deltaTime;
		
		
		
	}
 
    //I dont like this ammo stuff
    //public abstract bool CanAddAmmo(int a);
    //public abstract int Ammo();
    public abstract bool isTemporary();
	public abstract void loadSnapshot(ToySaver saver);
	public abstract ToySaver getSnapshot();
	//public abstract void AddAmmo(int a);
		
	public void OnDisable()
    {
        active = false;        
    }

	public void Die (float delay)
	{		
		if (this.tag.Equals ("Player") && (toy_type == ToyType.Building || toy_type == ToyType.Normal) && runetype != RuneType.Modulator)
			Peripheral.Instance.decrementToys();
		if (island != null) {
            island.my_toy = null;
			this.transform.parent = null;
            
		}
				
				
		
		StartCoroutine (DiePretty(delay));
	}

	IEnumerator DiePretty (float delay){
        yield return new WaitForSeconds(delay);
        if (death_effect == "") {
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
      //  island.blocked = false;
        Peripheral.Instance.zoo.returnObject (this.gameObject);

		//Peripheral.Instance.zoo.returnObject (island.parent, true);
		
	}



    public void AddTargetToy(Firearm toy)
    {
        if (rune_buttons == null) InitRuneButtons();

        foreach (Firearm f in child_toys)
        {
            if (f.my_name == toy.my_name) return;
        }
        child_toys.Add(toy);
        UpgradeTargetGhosts();
    }

    public void RemoveTargetToy(Firearm toy)
    {
        for (int i = 0; i < child_toys.Count; i++)
        {
            if (child_toys[i].my_name == toy.my_name) child_toys.RemoveAt(i);
        }
    }


    void UpgradeTargetGhosts()
    {
        if (child_toys.Count == 0) return;

        StatSum my_stats = rune.GetStats(false);
        float tower_force = rune.getLevel(EffectType.TowerForce);
        float tower_range = rune.getLevel(EffectType.TowerRange);
        foreach (Firearm f in child_toys)
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
