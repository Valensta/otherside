using UnityEngine;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine.EventSystems;



public class Monitor : MonoBehaviour {

	string island_selected = "BAD";
	string last_island = "BAD";
	public InGame_Toy_Button_Driver global_rune_panel;	
	public AudioSource test;
	//public GameObject signal_object;
	public Range_Indicator signal;
    public List<Range_Indicator> allowed_area_indicators;
   
    public GameObject islands_parent;
	private static Monitor instance;
	public static Monitor Instance { get; private set; }
	public Dictionary<string, Island_Button> islands = new Dictionary<string, Island_Button>();
    public List<SpriteRenderer> island_sprites = new List<SpriteRenderer>();
	public Island_Button castle_island;
	public bool is_active = true;
    //public ClearBackground background;
    bool showing_island_sprites = false;
        
    public List<TimeOfDay> color_settings = new List<TimeOfDay>();
	public SpriteRenderer background_image;
	public SpriteRenderer glowy_image;
	public SpriteRenderer light;
    public SpyGlass my_spyglass;


	void Awake(){
	
		Instance = this;

		if (islands_parent == null){Debug.Log("Monitor cannot find Islands gameobject!!\n"); return;}
		//might want to store this in the prefab later on
		foreach (Transform i in islands_parent.transform){
			Island_Button b = i.gameObject.GetComponentInChildren<Island_Button>();
			if (b != null){
				islands.Add (i.name, b);
			}
            if (b == castle_island) continue;//castle or anything else that has a permanent sprite
            if (b.transform.childCount > 0) {
                Transform child = b.transform.GetChild(0);
                SpriteRenderer sprite = child.gameObject.GetComponent<SpriteRenderer>();
                if (sprite != null) island_sprites.Add(sprite);
            }
		}
		global_rune_panel = EagleEyes.Instance.global_rune_panel;
        SpyGlass.onSelected += onSelected;
        Island_Button.onSelected += onSelected;
        Peripheral.onSelected += onSelected;
        MyButton.onSelected += onSelected;
        MyFastForwardButton.onSelected += onSelected;
    }

    public void ShowIslandSprites(bool show, string toy)
    {
        return;
        if (showing_island_sprites == show) return;
        IslandType show_me = IslandType.Either;
        if (show)
        {
            unitStats stats = Central.Instance.getToy(toy).GetActorStats();
            show_me = (stats.runetype == RuneType.SensibleCity)? IslandType.Either : stats.island_type; //show both for sensible city cuz it might be nice to see blue islands           
        }

        showing_island_sprites = show;
        float new_alpha = (show) ? 175f / 255f : 75f / 255f;
        
        
        foreach (Island_Button island in islands.Values)
        {
            if (show_me == IslandType.Either || island.island_type == show_me)
                Show.SetAlpha(island.My_sprite, new_alpha);
        }
    }

    public Color GetColorSetting(TimeName tod, bool glowy)//false sprite bg,  true glowy bg
    {
        foreach(TimeOfDay t in color_settings)
        {
            if (tod== t.name)
            {
                if (!glowy) return t.bg_color;
                else
                    return t.light_color;
            }
        }
        return Color.red;
    }


    void OnDisable()
    {
        SpyGlass.onSelected -= onSelected;
        Island_Button.onSelected -= onSelected;
        Peripheral.onSelected -= onSelected;
    }

    public bool SetAllowedRange(string toy_name)
    {
        bool added_some = false;
//        Debug.Log("Setting allowed range for " + toy_name + "\n");
        foreach (Range_Indicator r in allowed_area_indicators)
        {
            r.gameObject.SetActive(false);
        }
        if (toy_name == "") return false;

        foreach (KeyValuePair<string, Island_Button> kvp in islands)
        {
            if (kvp.Value.my_toy != null && kvp.Value.my_toy.my_name == toy_name)
            {
                AddAllowedRange(kvp.Value);
                added_some = true;
            }
        }

        return added_some;
    }

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


    void onSelected(SelectedType type, string n)
    {
     //   Debug.Log("Monitor on selected " + type + " " + n + "\n");
        if (type == SelectedType.Island || n.Equals("") || type == SelectedType.InteractiveSkill || type == SelectedType.Wish)
        {
            if (n.Equals("")) n = "BAD";
            SetIsland(n);
            
        }
        if (type == SelectedType.DirectIsland || type == SelectedType.Null)
        {
         //   Debug.Log("YES\n");
            SetIslandDirectly(n);
        }
        
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

        

        if (new_island == "BAD" && island_selected != null && islands.ContainsKey(island_selected)) islands[island_selected].Am_selected = false;
        island_selected = new_island;

        if (island_selected == "BAD")
        {
            driver.UpdatePanel(null);
            global_rune_panel.DisableMe(); //forconsistency
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


        if (Peripheral.Instance.getSelectedToy() == "" &&
            (!islands.ContainsKey(new_island) || (islands[new_island].island_type == IslandType.Permanent && islands[new_island].my_toy == null)))
        {
            new_island = "BAD";
        }

        if (new_island.Equals("BAD") && island_selected != null && islands.ContainsKey(island_selected)) islands[island_selected].Am_selected = false;

        island_selected = new_island;

        if (island_selected.Equals("BAD"))
        {
            global_rune_panel.DisableMe();            
            
            last_island = "BAD";
            SetMainSignal(false);
            return;
        }





        if (islands[island_selected].island_type == IslandType.Permanent)
        {
            if (islands[island_selected].Selected_toy == "TOOFAR")
            {
                TooFarSignal();
                Debug.Log("Cannot place toy, island does not have a selected toy!\n");
            }
            else if (islands[island_selected].Selected_toy != "" || islands[island_selected].my_toy != null)
            {
                PlaceStuff(true);
            }

        }
        else
        {
            if (islands[island_selected].Selected_toy == "TOOFAR")
            {
                TooFarSignal();
                Debug.Log("Cannot place toy, island does not have a selected toy!\n");
            }
            else if (islands[island_selected].Selected_toy != "" || islands[island_selected].my_toy != null)
            {
                PlaceStuff(false);
            }

        }




        if (last_island != island_selected && islands.ContainsKey(last_island))
            islands[last_island].Am_selected = false;

        if (islands.ContainsKey(island_selected))
        {

            if (islands[island_selected].my_toy != null && islands[island_selected].my_toy is Firearm && islands[island_selected].my_toy.toy_type
                != ToyType.Building && islands[island_selected].island_type == IslandType.Temporary)
                ((Firearm)islands[island_selected].my_toy).AddAmmo(1);  //temp towers with temp islands don't get selected, no special behavior here, just add ammo and pulse range, and you're done
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
			if (selected != ""){
			
				if (islands[island_selected].Am_selected){
					islands[island_selected].DoAThing();
					SetMainSignal(false);
					return;				
				}
               
                size = getSignalSize(selected);
			}
			global_rune_panel.DisableMe();         
        }
        else{		
			if (!islands[island_selected].Am_selected){
                
                size = my_toy.rune.getRange()/2f;
                if (size == -2)
                {
                    Debug.Log("Trying to get range for " + my_toy.name + " " + my_toy.my_name + " " + my_toy.gameObject.name + "\n");
                    size = -1;
                }
                
                if (size == 0) size = -1;
				if (panel) global_rune_panel.SetParent(my_toy);
							
			}else{
				SetMainSignal (false);
				global_rune_panel.DisableMe();
				return;
			}
		}
		if (size > 0) InitSignal(signal, islands[island_selected], size,true);		
        if (size == -1) SetMainSignal(false);

		
	}

    public float getSignalSize(string selected)
    {
        float size = 0.5f;
        unitStats s = Central.Instance.getToy(selected);
        float distance_bonus = 0f;
        if (s.toy_type == ToyType.Temporary)
        {
            distance_bonus = StaticRune.GetDistanceBonus(s.name, islands[island_selected].transform.position, null);
        }
        if (s != null)
            size = StaticRune.time_bonus_aff(StaticStat.getBaseFactor(s.runetype, EffectType.Range, s.toy_type==ToyType.Hero),
                                             EffectType.Range, s.runetype, s.toy_type, distance_bonus) / 2f;

     //   Debug.Log("Getting signal size for " + selected + " got " + size + "\n");
        return size;
    }

    public void InitMainSignal(string selected)//for external use
    {
      //  Debug.Log("Init signal " + selected + " " + " island " + island_selected  + "\n");
        if (selected.Equals(""))
        {
            SetMainSignal(false);
        }
        else
        {
            float size = getSignalSize(selected);
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

        my_signal.gameObject.transform.parent = island.transform;
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