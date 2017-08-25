using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;


public class Building : MonoBehaviour {

    public SpriteRenderer construction_sprite;
    public SpriteRenderer tower_sprite;
    public SpriteRenderer upgrade_sprite;
    public  TowerVisual tower_visual;
    
    public Mini_Toy_Button_Driver construction_indicator = null;

    public Toy my_toy;


    public float init_construction_time; //set on the prefab
    public float current_construction_time;
    public bool construction_in_progress;
    float min_to_show_progress_bar = 0.2f; //if construction takes less time than this, don't bother with a progress bar
    float min_to_show_construction = 0.12f;

        
    public void OnEnable()
    {
        //if (init_construction_time <= 0) init_construction_time = 0.1f;

        /*
        if (my_toy.rune_buttons && my_toy.rune_buttons.upgrade)
        {         
            my_toy.rune_buttons.upgrade.gameObject.SetActive(false);
        }        
*/
         if (tower_visual) tower_visual.setUpgrade(false);

        
        try
        {
            //if (showConstruction()) Assert.IsNotNull(tower_visual);
            if (showConstruction()) Assert.IsNotNull(construction_sprite);
            Assert.IsNotNull(my_toy);
        }catch(Exception e)
        {
            Debug.Log("Prefab issue on " + this.gameObject.name + ": " + e.ToString());
            throw new Exception("Prefab issue on " + this.gameObject.name + ": " + e.ToString());
        }
    }



    public void initStats(Toy toy)
    {

        my_toy = toy;

        if (!showProgress()) return;

        GameObject buttons = Peripheral.Instance.zoo.getObject("GUI/construction_indicator", false);
        construction_indicator = buttons.GetComponent<Mini_Toy_Button_Driver>();
        construction_indicator.InitMiniDriver(this);
        buttons.SetActive(true);


    }

    void FinishConstruction()
    {
//        Debug.Log("Finished construction\n");
        if (showConstruction())
        {
            //Show.SetAlpha(building_sprite_parent, 1f);
            if (tower_visual) tower_visual.setSprite(true);
            Show.SetAlpha(construction_sprite, 0f);
        }
        
        if (showProgress()) construction_indicator.ammo.gameObject.SetActive(false);

        if (my_toy.runetype != RuneType.Castle && my_toy.runetype != RuneType.Modulator && my_toy.runetype != RuneType.SensibleCity) my_toy.firearm.Active = true;
        if (my_toy.runetype == RuneType.Modulator)    DoModulatorStuff();
        
        if (my_toy.runetype == RuneType.SensibleCity) Peripheral.Instance.addedCity(true, my_toy);
        
        my_toy.InitHelperPanels(true);


        if (my_toy.firearm != null && my_toy.firearm.area_effector != null) my_toy.firearm.InitAreaEffector();
        
        
    }

    public bool showConstruction()
    {
        return (init_construction_time > min_to_show_construction);
    }

    public bool showProgress()
    {
        return (init_construction_time > min_to_show_progress_bar);
    }
   
	
	 public void loadSnapshot(ToySaver saver)
    {
		if (saver.construnction_time > 0) {
			StartConstruction(saver.construnction_time);

		}
		else {
			current_construction_time = 0f;
			FinishConstruction();
		}
    }
    					
	
  
    void DoModulatorStuff()
    {
        
        my_toy.Die(0f);
        IslandType new_type = (my_toy.island.island_type == IslandType.Permanent) ? IslandType.Temporary : IslandType.Permanent;
        my_toy.island.ChangeType(new_type, false);
    }

    void Update()
    {
        if (Time.timeScale == 0) return;

        if (!construction_in_progress) return;        
     

        if (current_construction_time > 0) { current_construction_time -= Time.deltaTime; }
        else
        {
            current_construction_time = -1;
            construction_in_progress = false;
            FinishConstruction();
        }
        if (showProgress()) construction_indicator.SetAmmoPercentage(current_construction_time / init_construction_time);


    }

    public void StartConstruction()
    {        
        StartConstruction(init_construction_time);
    }
    public void StartConstruction(float time)
    {
        if(my_toy.runetype == RuneType.SensibleCity) Peripheral.Instance.building_a_city = true;

        if (init_construction_time == 0)
        {
       //     Debug.Log($"Aborting construction {this.gameObject.name}\n");
            current_construction_time = -1;
            construction_in_progress = false;
            FinishConstruction();
            return;
        }
        
        my_toy.InitHelperPanels(false);
       // Debug.Log($"Starting Construction {this.gameObject.name} for {init_construction_time} seconds\n");
        construction_in_progress = true;
        current_construction_time = time;     
        
        
        if (showConstruction())
        {
            //Show.SetAlpha(building_sprite_parent, 0f);
            if (tower_visual) tower_visual.setSprite(false);
            Show.SetAlpha(construction_sprite, 1f);
        }

        if (showProgress()) construction_indicator.ammo.gameObject.SetActive(true);
    }
}
