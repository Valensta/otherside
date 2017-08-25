using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;



public class Monitor : MonoBehaviour {

	string island_selected = "BAD";
	string last_island = "BAD";
	public InGame_Toy_Button_Driver global_rune_panel;	
	public AudioSource test;
	//public GameObject signal_object;
	public Range_Indicator signal;
    //public List<Range_Indicator> allowed_area_indicators; //let's only have 1 range indicator
   
    public GameObject islands_parent;
	private static Monitor instance;
	public static Monitor Instance { get; private set; }
	public Dictionary<string, Island_Button> islands = new Dictionary<string, Island_Button>();
    public List<SpriteRenderer> island_sprites = new List<SpriteRenderer>();
	public Island_Button castle_island;
    public List<Island_Button> fake_castles;
	public bool is_active = true;
    //public ClearBackground background;
    bool showing_island_sprites = false;
            
	public SpriteRenderer background_image;
	public SpriteRenderer glowy_image;
	public SpriteRenderer light;
    public SpyGlass my_spyglass;
    public String bad = "BAD";//wtf is this
    public bool color_islands = false;
    public bool enable_overpasses = false;

    public Island_Button getIslandByID(string id)
    {
        foreach (Island_Button b in islands.Values)
        {
            if (b.ID.Equals(id)) return b;
        }
        return null;
    }

    void Awake(){
	
		Instance = this;

		if (islands_parent == null){Debug.Log("Monitor cannot find Islands gameobject!!\n"); return;}
		//might want to store this in the prefab later on
		foreach (Transform i in islands_parent.transform){
			Island_Button b = i.gameObject.GetComponentInChildren<Island_Button>();
            b.setID();
		    b.Hidden = false;

            if (b != null){
                try
                {
                    islands.Add(i.name, b);
                }catch(Exception e)
                {
                    Debug.LogError(e + ": " + i.name + "\n");
                }
			}
            if (b == castle_island) continue;//castle or anything else that has a permanent sprite
            if (b.transform.childCount > 0) {
                Transform child = b.transform.GetChild(0);
                SpriteRenderer sprite = child.gameObject.GetComponent<SpriteRenderer>();
                if (sprite != null) island_sprites.Add(sprite);
            }
		}
		if (EagleEyes.Instance.global_rune_panel != null) global_rune_panel = EagleEyes.Instance.global_rune_panel;
        SpyGlass.onSelected += onSelected;
        Island_Button.onSelected += onSelected;
        Peripheral.onSelected += onSelected;
        MyButton.onSelected += onSelected;
        MyFastForwardButton.onSelected += onSelected;
        Enemy_Button.onSelected += onSelected;

    }

    public void ShowIslandSprites(bool show, string toy)
    {
        return;/*
        if (showing_island_sprites == show) return;
        IslandType show_me = IslandType.Either;
        if (show)
        {
            unitStats stats = Central.Instance.getToy(toy).GetActorStats();
            show_me = (stats.toy_id.rune_type == RuneType.SensibleCity)? IslandType.Either : stats.island_type; //show both for sensible city cuz it might be nice to see blue islands           
        }

        showing_island_sprites = show;
        float new_alpha = (show) ? 175f / 255f : 75f / 255f;
        
        
        foreach (Island_Button island in islands.Values)
        {
            if (show_me == IslandType.Either || island.island_type == show_me)
                Show.SetAlpha(island.My_sprite, new_alpha);
        }*/
    }

    public void ShowIslandSprites(bool show, IslandType type)
    {                                        
        
        foreach (Island_Button island in islands.Values)
        {
            if (island.island_type == type)
            {
                island.My_sprite.gameObject.SetActive(show);
                island.Hidden = !show;
            }
        }
    }

    public void ResetIslands()
    {
        foreach (Island_Button i in islands.Values) { i.ResetIslandType(); }
    }

    


    void OnDisable()
    {
        SpyGlass.onSelected -= onSelected;
        Island_Button.onSelected -= onSelected;
        Peripheral.onSelected -= onSelected;
    }
    
    public bool SetAllowedRange(string toy_name)
    {
        foreach (KeyValuePair<string, Island_Button> kvp in islands)
        {
            if (kvp.Value.my_toy != null && kvp.Value.my_toy.my_name == toy_name)
            {
                InitSignal(signal, kvp.Value, kvp.Value.my_toy.rune.getRange() / 2f, true);
                
            }
        }
        return true;
        /*
        bool added_some = false;
//        Debug.Log("Setting allowed range for " + toy_name + "\n");
        foreach (Range_Indicator r in allowed_area_indicators)
        {
            r.gameObject.SetActive(false);
        }
        if (toy_name.Equals("")) return false;

        foreach (KeyValuePair<string, Island_Button> kvp in islands)
        {
            if (kvp.Value.my_toy != null && kvp.Value.my_toy.my_name == toy_name)
            {
                AddAllowedRange(kvp.Value);
                added_some = true;
            }
        }

        return added_some;
        */
    }
    
    /*
    void AddAllowedRange(Island_Button island)
    {
        bool ok = false;
        foreach (Range_Indicator r in allowed_area_indicators)
        {
            if (r.gameObject.activeSelf == false)
            {
                r.gameObject.SetActive(true);
                InitSignal(r, island, island.my_toy.rune.getRange()/ 2f,false);
                ok = true;
                return;
            }
        }
        if (!ok)
        {
            Range_Indicator new_range = Peripheral.Instance.zoo.getObject("GUI/allowed_range_sphere", true).GetComponent<Range_Indicator>();
            allowed_area_indicators.Add(new_range);
            InitSignal(new_range, island, island.my_toy.rune.getRange() / 2f,false);
        }
    }
    */

    void onSelected(SelectedType type, string n)
    {
     //   Debug.Log("Monitor on selected " + type + " " + n + "\n");
        //if (type == SelectedType.Island || n.Equals("") || type == SelectedType.InteractiveSkill || type == SelectedType.Wish)
        if (type == SelectedType.Island || notAnIsland(type, n))
        {
            if (notAnIsland(type, n)) n = bad;
            SetIsland(n);
            return;
        }
        if (type == SelectedType.DirectIsland || type == SelectedType.Null)
        {
         //   Debug.Log("YES\n");
            SetIslandDirectly(n);
        }
        
    }

    bool notAnIsland(SelectedType type, string n)
    {
        if (n.Equals("") || type == SelectedType.InteractiveSkill || type == SelectedType.Wish) return true;
        return false;
    }

    void SetIslandDirectly(string new_island)
    {
      //  Debug.Log("Monitor setting island directly " + new_island + "\n");
        if (!is_active) return;

        Island_Floating_Button_Driver driver = EagleEyes.Instance.floating_tower_scroll_driver;
        driver.ResetSelected();

        last_island = island_selected;

        
        if (!islands.ContainsKey(new_island))
        {
            new_island = "BAD";
            driver.SetPanel(false);            
        }

        

        if (new_island.Equals("BAD") && island_selected != null && islands.ContainsKey(island_selected)) islands[island_selected].Am_selected = false;
        island_selected = new_island;

        if (island_selected.Equals("BAD"))
        {
            driver.UpdatePanel(null);
            if (global_rune_panel != null) global_rune_panel.DisableMe(); //forconsistency
            last_island = "BAD";
            SetMainSignal(false);
            return;
        }

        if (driver.selected_island == null || driver.selected_island.GetInstanceID() != islands[island_selected].GetInstanceID())
        {
            
            driver.UpdatePanel(islands[island_selected]);
            return;
        }

    }

    void SetIsland(string new_island)
    {
        if (!is_active) return;
        
        EagleEyes.Instance.floating_tower_scroll_driver.UpdatePanel(null);
       last_island = island_selected;


        if (Peripheral.Instance.getSelectedToy().Equals("") &&
            (!islands.ContainsKey(new_island) || (islands[new_island].island_type == IslandType.Permanent && islands[new_island].my_toy == null)))
        {
            new_island = bad;
        }
        
        if (new_island.Equals(bad) && island_selected != null && islands.ContainsKey(island_selected)) islands[island_selected].Am_selected = false;

        island_selected = new_island;

        if (island_selected.Equals(bad))
        {
            if (global_rune_panel != null) global_rune_panel.DisableMe();

            last_island = bad;
            SetMainSignal(false);
            return;
        }





        if (islands[island_selected].island_type == IslandType.Permanent)
        {
            if (islands[island_selected].Selected_toy.Equals("TOOFAR"))
            {
                TooFarSignal();
                Debug.Log("Cannot place toy, island does not have a selected toy!\n");
            }
            else if (!islands[island_selected].Selected_toy.Equals("") || islands[island_selected].my_toy != null)
            {
                PlaceStuff(true);
            }

        }
        else
        {
            if (islands[island_selected].Selected_toy.Equals("TOOFAR"))
            {
                TooFarSignal();
                Debug.Log("Cannot place toy, island does not have a selected toy!\n");
            }
            else if (!islands[island_selected].Selected_toy.Equals("") || islands[island_selected].my_toy != null)
            {
                PlaceStuff(false);
            }

        }




        if (!last_island.Equals(island_selected) && islands.ContainsKey(last_island))
            islands[last_island].Am_selected = false;

        if (islands.ContainsKey(island_selected))
        {

            if (islands[island_selected].my_toy != null && islands[island_selected].my_toy.firearm != null && islands[island_selected].island_type == IslandType.Temporary)
                islands[island_selected].my_toy.firearm.AddAmmo(1);  //temp towers with temp islands don't get selected, no special behavior here, just add ammo and pulse range, and you're done
            else            
                islands[island_selected].Am_selected = !islands[island_selected].Am_selected;
            //     Debug.Log("toggling am selected to " + islands[island_selected].am_selected + "\n");
        }

        return;

    }

    public void TooFarSignal() {
        Noisemaker.Instance.Play("island_too_far");
    }


	public void PlaceStuff(bool panel){
//	Debug.Log("Placing stuff\n");
		Toy my_toy = islands[island_selected].my_toy;
		string selected = islands[island_selected].Selected_toy;
		float size = 0f;		
		if (my_toy == null){		
			if (!selected.Equals("")){
			
				if (islands[island_selected].Am_selected){
					islands[island_selected].DoAThing();
					SetMainSignal(false);
					return;				
				}
               
                size = getSignalSize(selected);
			}
		    if (global_rune_panel != null) global_rune_panel.DisableMe();         
        }
        else{		
			if (!islands[island_selected].Am_selected){
                
                size = my_toy.rune.getRange()/2f;
         //       Debug.Log("SIZE " + size + "\n");
                if (size == -2)
                {
                    Debug.Log("Trying to get range for " + my_toy.name + " " + my_toy.my_name + " " + my_toy.gameObject.name + "\n");
                    size = -1;
                }
                if (size > 10)
                {
                    size = 1f;
                }
                if (size == 0) size = -1;
				if (panel && global_rune_panel != null) global_rune_panel.SetParent(my_toy);
							
			}else{
				SetMainSignal (false);
			    if (global_rune_panel != null) global_rune_panel.DisableMe();
				return;
			}
		}
		if (size > 0) InitSignal(signal, islands[island_selected], size,true);		
        if (size == -1) SetMainSignal(false);

		
	}

    public float getSignalSize(string selected)
    {
        unitStats s = Central.Instance.getToy(selected);
        return _getSignalSize(s);
    }

    public float getSignalSize(RuneType runeType, ToyType toyType)
    {
       
        unitStats s = Central.Instance.getToy(runeType, toyType);
        return _getSignalSize(s);
    }

    public float _getSignalSize(unitStats s)
    {
        float size = 0.5f;
        
     
        float distance_bonus = 0f;
        if (s.toy_id.toy_type == ToyType.Temporary)
        {
            distance_bonus = StaticRune.GetDistanceBonus(s.name, islands[island_selected].transform.position, null);
        }
        if (s.toy_id.rune_type == RuneType.SensibleCity) return 0.5f;
        if (s != null)
            size = StaticRune.time_bonus_aff(StaticStat.getBaseFactor(s.toy_id.rune_type, EffectType.Range, s.toy_id.toy_type ==ToyType.Hero),
                                             EffectType.Range, s.toy_id.rune_type, s.toy_id.toy_type, distance_bonus) / 2f;

        //   Debug.Log("Getting signal size for " + selected + " got " + size + "\n");
        
        return size;
    }

    public void InitMainSignal(RuneType runeType, ToyType toyType)//for external use
    {
      //  Debug.Log("Init signal " + selected + " " + " island " + island_selected  + "\n");
        if (runeType == RuneType.Null || toyType == ToyType.Null)
        {
            SetMainSignal(false);
        }
        else
        {
            
            if (toyType == ToyType.Hero)
            {
                Rune rune = Central.Instance.getHeroStats(runeType);
                if (rune != null)
                {
                    float hero_range = rune.getRange() / 2f;
                    InitSignal(signal, islands[island_selected], rune.getRange() / 2f, true);
                    return;
                }
            }
            float size = getSignalSize(runeType, toyType);
            InitSignal(signal, islands[island_selected], size, true);
        }
    }

    void InitSignal(Range_Indicator my_signal, Island_Button island, float _size, bool main)
    {        
        if (main && my_signal == null) {
            my_signal = Zoo.Instance.getObject("GUI/range_sphere", true).GetComponent<Range_Indicator>();
            signal = my_signal;
        }
        my_signal.gameObject.SetActive(true); //soft clear removes signal

        my_signal.gameObject.transform.SetParent(island.transform);
        my_signal.gameObject.transform.position = island.transform.position;
        SetSignalSize(my_signal, _size);


        my_signal.Set(true, island.island_type == IslandType.Temporary && island.my_toy != null);

        
    }

    public void SetSignalSize(float _size)
    {
        SetSignalSize(signal, _size);
    }

    public void SetMainSignal(bool set) {
        if (signal == null) signal = Zoo.Instance.getObject("GUI/range_sphere", true).GetComponent<Range_Indicator>();
        signal.Set(set, false);
    }


    public void SetSignalSize(Range_Indicator my_signal, float _size){
        
		my_signal.gameObject.transform.localScale = new Vector3 (_size, _size, _size);
	}
	
}