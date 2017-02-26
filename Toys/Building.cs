using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;




public class Building : Toy {

	public SpriteRenderer construction_sprite;
	public SpriteRenderer building_sprite;	
	public Mini_Toy_Button_Driver construction_indicator = null;
    public string[] target_toys = new string[0];



    public float init_construction_time;	
	public float current_construction_time;
	public bool construction_in_progress;
	
	public override void initStats(unitStats s, Vector3 scaleV, Island_Button i, string _name, Rune _rune)
	{
	//	Debug.Log("Initializing " + _name + "\n");
		
		ToyInit(s,scaleV, i, _name, _rune);     
		if (runetype != RuneType.Castle){
			//Peripheral.Instance.castle.rune.Upgrade(ToyType.Building, building_id.effect_type);
			
			GameObject buttons = Peripheral.Instance.zoo.getObject("GUI/construction_indicator", false);
			construction_indicator = buttons.GetComponent<Mini_Toy_Button_Driver>();
			construction_indicator.InitMiniDriver(this);
			buttons.SetActive(true);
            
        }
        if (rune_buttons != null) rune_buttons.UpdateMe();
    }    

	public void StartConstruction(float time){
		if (my_name == "castle") return;
	//	Debug.Log("Starting constructionszz\n");
		Show.SetAlpha(building_sprite, 0f);
        Show.SetAlpha(construction_sprite, 1f);
		construction_in_progress = true;
		current_construction_time = time;
		if (construction_indicator != null) construction_indicator.ammo.gameObject.SetActive(true);
	}


    public override bool isTemporary(){
		return false;
	}

    protected override void onUpgrade(EffectType type, int ID)
    {

    }


    public override ToySaver getSnapshot()
    {					
		return new ToySaver(my_name, current_construction_time, rune, toy_type);	
    }

	void Update ()
	{
		if (construction_in_progress){
			if (current_construction_time > 0) {current_construction_time -= Time.deltaTime;} else{
				current_construction_time = -1;
				construction_in_progress = false;
				FinishConstruction();				
			}
			if (construction_indicator != null) construction_indicator.SetAmmoPercentage(current_construction_time/init_construction_time);
		}

	}

	void FinishConstruction(){
		if (my_name.Equals("castle")) return;
	//	Debug.Log("Construction finished for " + my_name + "\n");
		Show.SetAlpha(building_sprite, 1f);
		Show.SetAlpha(construction_sprite, 0f);       

		if (construction_indicator != null) construction_indicator.ammo.gameObject.SetActive(false);

        if (runetype == RuneType.Modulator) DoModulatorStuff();

        if (runetype == RuneType.SensibleCity)Peripheral.Instance.addedCity(true, this);
	}

    public bool isToyATarget(string s)
    {
        bool yes = false;
        foreach (string toy in target_toys)
            if (toy.Equals(s)) yes = true;
        return yes;        
    }

    void OnDisable()
    {
        active = false;        
    }
	
	 public override void loadSnapshot(ToySaver saver)
    {
		Debug.Log("Loading snapshot for " + saver.toy_name + "\n");
		rune = saver.rune;	
		
		if (saver.ammo > 0) {
			StartConstruction(saver.ammo);
		//	Debug.Log("Starting construction " + saver.ammo + "\n");
		}
		else {
			current_construction_time = 0f;
			FinishConstruction();// Debug.Log("Finishing construction in loadsnapshot\n");
		}
			
		if (rune_buttons != null) rune_buttons.UpdateMe();
		
    }
    					
	
  
    void DoModulatorStuff()
    {
        
        Die(0f);
        IslandType new_type = (island.island_type == IslandType.Permanent) ? IslandType.Temporary : IslandType.Permanent;
        island.ChangeType(new_type);
    }
}
