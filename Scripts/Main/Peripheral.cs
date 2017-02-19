using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Peripheral : MonoBehaviour {
    public float dreams;
    public string title;
    public int max_dreams;
    public float nightmares;
    private int toys;
    //	public int monsters;
    float health;

    
    public LState level_state;
    public bool pause;
    bool wave_button_visible;

    public EnemyList enemy_list = new EnemyList();
    public MyArray<HitMe> targets;
    public Transform monsters_transform;
    public Transform arrows_transform;
    public Inventory my_inventory;   //wish inventory
    public SkillMaster my_skillmaster; //special skills

    public float difficulty;
    
    public Toy castle;    
    public Dictionary<RuneType, int> hero_points;
    public List<Building> buildings;
    public List<Firearm> firearms;
    public Dictionary<string, bool> haveToys;
    public Dictionary<string, bool> haveMonsters;


    public int current_wave;
    public float next_monster_time;
    bool level_active = false;
    public bool ok_next_wave = true;
    public bool make_wave = false;
    

    SelectedToy toy_selected;
    RuneType rune_selected;
    RuneType selected_rune;

    public VariableStat xp_factor;
    public VariableStat damage_factor;
    public VariableStat dream_factor;

    bool done_making_wave = false;
    public float default_wave_interval;         // waves are spaced at least X seconds apart
    private float wave_interval;         // waves are spaced at least X seconds apart
    float next_wave_time;                   // first wave time

    public float timeScale;
    float previous_timeScale;

    public int monster_count; //purely for naming purposes
    public int cheapest_monster_cost;  //for this wave

    Vector3 monster_source;
    GameObject pathfinder;
    public float tileSize;
    public float diagonal;

    public float TIME;
    public GameObject island_selected;
    public EventOverseer overseer;
    public PathfinderType pathf;
    public Zoo zoo;
    Queue<string> file;
    private static Peripheral instance;

    public delegate void OnCreatePeripheralHandler(Peripheral p);
    public static event OnCreatePeripheralHandler onCreatePeripheral;

    public delegate void OnSelectHandler(SelectedType type, string content);
    public static event OnSelectHandler onSelected;

    public delegate void OnPlaceToyHandler(string content);
    public static event OnPlaceToyHandler onPlacedToy;

    public delegate void OnWaveStartHandler(int content);
    public static event OnWaveStartHandler onWaveStart;

    public delegate void onHealthChangedHandler(float i);
    public static event onHealthChangedHandler onHealthChanged;

    public delegate void onDreamsChangedHandler(float i, bool visual, Vector3 pos);
    public static event onDreamsChangedHandler onDreamsChanged;

    public delegate void onDreamsAddedHandler(float i, Vector3 pos);
    public static event onDreamsAddedHandler onDreamsAdded;

    public delegate void onSellToyHandler();
    public static event onSellToyHandler onSellToy;

    public static Peripheral Instance { get; private set; }

    public float Wave_interval
    {
        get
        {
            return wave_interval;
        }

        set
        {
            Debug.Log("Setting wave interval\n");
            wave_interval = value;
        }
    }

    void Init() {
        //	Debug.Log("Peripheral Initializing\n");	
        done_making_wave = false;
        targets = new MyArray<HitMe>();
        monster_count = 0;
        level_state = LState.WaveButton;
        wave_button_visible = false;
        my_inventory.InitWishes();
        //	meters = new Dictionary<RuneType, GameObject> (); //meter gui objects
        nightmares = 0f;
        default_wave_interval = 0.1f;           // waves are spaced at least X seconds apart
        next_wave_time = 0.5f;                  // first wave time
        timeScale = 1f;
        previous_timeScale = 1f;
        buildings = new List<Building>();
        firearms = new List<Firearm>();
        haveToys = new Dictionary<string, bool>();
        haveMonsters = new Dictionary<string, bool>();
        toy_selected = null;
        rune_selected = RuneType.Null;
        //	monsters = 0;


        current_wave = 0;
        next_monster_time = 0;
        level_active = false;
        if (Moon.Instance != null) Moon.Instance.InitEmpty();

        island_selected = null;

        ok_next_wave = true;
        make_wave = false;
        dreams = 0;
        toys = 0;
        TIME = 0f;
        my_inventory.InitWishes();
        hero_points = new Dictionary<RuneType, int>();
        hero_points.Add(RuneType.Sensible, 1);
        hero_points.Add(RuneType.Airy, 1);
        hero_points.Add(RuneType.Vexing, 1);

    }


    void Awake()
    {// Debug.Log("Peripheral awake\n");
        Init();

        if (Instance != null && Instance != this) {
            Debug.Log("peripheral got destroyeed\n");
            Destroy(gameObject);
        }
        if (onCreatePeripheral != null) {
            Debug.Log("Peripheral on created\n");
            onCreatePeripheral(this);
        }
        Instance = this;
        Time.timeScale = timeScale;
        
    }

    void Start()
    {
        
       // Central.Instance.game_saver.Init();
    }


    public void decrementToys()
    {
        toys--;
     //   Debug.Log("decrement toy\n");
    }

    public void incrementToys(){toys++;
       // Debug.Log("increment toy\n");
    }
    public int getToys(){return toys;}
    public void setToys(int i) {// Debug.Log("Setting toys " + toys + "\n");
        toys = i;}



    public string getSelectedToy(){
        if (toy_selected != null)
            return toy_selected.name;
        else
            return "";
	}




    void SetAllowedRange(string toy)
    {
        if (!toy.Equals(""))
        {
            /* wrong place for this
            actorStats stats = Central.Instance.getToy(toy);            
            Cost toy_cost = null;
            if (stats != null) toy_cost = stats.cost_type;
            if (toy_cost == null) return;            
            if (!HaveResource(toy_cost)) return;
            */

            string required = Central.Instance.getToy(toy).required_building;
            if (!required.Equals(""))
            {
                Monitor.Instance.SetAllowedRange(required);
            }
        }
        else
        {
            Monitor.Instance.SetAllowedRange("");
        }
    }

    public bool SomethingSelected()
    {
        return (toy_selected != null || rune_selected != RuneType.Null);
    }

    public float XpFactor()
    {
        return xp_factor.getStat();
    }

	public RuneType getSelectedRune(){
		return rune_selected;
	}


    public void ClearAll(SelectedType type, string name)
    {
        if (onSelected != null) { onSelected(type, name); }
    }

    public bool SelectToy(string toy, RuneType rune)
    {
        return SelectToy(toy, rune, false);
    }

    public bool SelectToy(string toy, RuneType rune, bool quietly)
    {
        //   Debug.Log("Select toy " + toy + "\n") ;
        if (toy == null) toy = "";

        if (toy.Equals(toy_selected)) return toy_selected.Equals("");

        if (toy == "")
        {
            toy_selected = null;
            SetAllowedRange("");
        
            if (!quietly && onSelected != null) onSelected(SelectedType.Toy, "");

            return false;
        }
        else {
            rune_selected = rune;            
            actorStats stats = Central.Instance.getToy(toy);
            if (stats == null) return false;            

            toy_selected = new SelectedToy(toy, stats.island_type);
            
            SetAllowedRange(toy_selected.name);

            if (!quietly && onSelected != null) onSelected(SelectedType.Toy, toy_selected.name);
            
            return true;
        }
    }

    public void SetDiagonal(float d){
		if (d > diagonal){diagonal = d;}
	}

	public string IncrementMonsterCount(){
		monster_count++;
		return monster_count.ToString();
	}


	public bool Load(Level level)
	{
		Init ();
		difficulty = Central.Instance.getCurrentDifficultyLevel();
        Debug.Log("Loaded level " + level + " with difficulty " + difficulty + "\n");

        monsters_transform = Monsters_Bucket.Instance.gameObject.transform;
        arrows_transform = GameObject.Find("Arrows").gameObject.transform;
        Moon.Instance.monsters_transform = monsters_transform;

        
        
            Debug.Log("using FancyLoader to load " + level + "\n");
        if (level.fancy)
            FancyLoader.Instance.LoadLevel(level.name);
        else         
        {

            file = new Queue<string>(((TextAsset)Resources.Load("Levels/" + level.name)).text.Split('\n'));
            Debug.Log("Loading file " + level.name + "\n");
            Loader.Instance.setFile(file);
            Loader.Instance.LoadLevel();
        }
        SetDiagonal(Vector3.Distance(WaypointMultiPathfinder.Instance.paths[0].finish.transform.position, WaypointMultiPathfinder.Instance.paths[0].start.transform.position) * 2);

        zoo = Zoo.Instance;
        overseer = EventOverseer.Instance;

		level_state = LState.WaveButton;
		Moon.Instance.WaveInProgress = false;
		level_active = true;

		//Debug.Log ("Peripheral FINISHED LOADING at " + Time.realtimeSinceStartup);
		if (GameStatCollector.Instance !=  null) GameStatCollector.Instance.Init();
		
		return true;
	}
    
	public GameObject PlaceToyOnIsland(string placeme, GameObject o){
		//Monitor.Instance.SetIsland("BAD");
        if (onSelected != null) onSelected(SelectedType.Island, "");
        Toy f = null;
		GameObject toy = makeToy (placeme,o, ref f);
		if (toy == null){
			EagleEyes.Instance.StopSignal();
			Debug.Log("UH OH\n");
			return null;
		}
		
		EagleEyes.Instance.ClearInfo();

		return toy;
	}
	
	public int getCurrentWave(){
		return current_wave;
	}






    public void SetHeroPoint(RuneType type, int point, bool update_ui)
    {
        if (point > 1) { Debug.Log("Trying to set Hero point for type " + type + " to an invalid value " + point + ", setting to 1."); point = 1; }
        int current_point = -1;
        hero_points.TryGetValue(type, out current_point);
        if (current_point != -1)
        {
            hero_points[type] = point;
       //     Debug.Log("Setting hero point " + type + " to " + point);
        }
        else
        {
     //       Debug.Log("Setting NEW hero point " + type + " to " + point);
            hero_points.Add(type, point);
        }

        EagleEyes.Instance.UpdateToyButtons("meh", ToyType.Hero, false);
    }

    public bool HaveHero(RuneType type)
    {
        int current_point = -1;
        hero_points.TryGetValue(type, out current_point);
      //  Debug.Log("Have hero " + type + "? " + current_point);
        return (current_point > 0);
    }

    public void AddHeroPoint(RuneType type, int point)
    {
        if (point > 1) { Debug.Log("Trying to set Hero point for type " + type + " to an invalid value " + point + ", setting to 1.\n"); point = 1; }
        int current_point = -1;
        hero_points.TryGetValue(type, out current_point);
        if (current_point != -1)
        {
            if (current_point + point > 1) { Debug.Log("Trying to set Hero point for type " + type + " to an invalid value " + current_point + point + ", setting to 1.\n"); point = 1; }
            hero_points[type] += point;
        }
        else
        {
            hero_points.Add(type, point);
        }

        EagleEyes.Instance.UpdateToyButtons("meh", ToyType.Hero, false);
    }

    public void UpdateUIOnWishChange()
    {

        EagleEyes.Instance.UpdateToyButtons("blah", ToyType.Temporary, false);
    }
    
 





    public bool HaveResource(Cost c) {
        bool ok = true;
        //Debug.Log("Checking " + kind + " amt " + amt + "\n");
        if (c.type == CostType.Dreams) {
            if (dreams < c.Amount) ok = false;
        } else if (c.type == CostType.Wishes) {
            ok = my_inventory.HaveWish(WishType.Sensible, c.Amount);
        } else if (c.type == CostType.ScorePoint)
        {
            ok = (ScoreKeeper.Instance.getTotalScore() > c.Amount);
        } else if (c.isHero()) { 
			ok = HaveHero(c.getHeroRuneType());			
		}
		return ok;
	}
	


	public bool UseResource(Cost c, Vector3 pos){
		if (!HaveResource(c)){//Debug.Log("Not enough resource " + c.type + " " + c.cost + "\n");
            return false;}
        //	Debug.Log("UseResource " + c.Amount + " " + c.type + "\n");
        if (c.type == CostType.Dreams)
        {
            addDreams(-c.Amount, pos, false);
        }
        else if (c.type == CostType.Wishes)
        {
            my_inventory.SubtractWish(WishType.Sensible, c.Amount);
            
        }
        else if (c.type == CostType.ScorePoint)
        {
            ScoreKeeper.Instance.useScore(c.Amount);
        }
        else if (c.isHero())
        {
            SetHeroPoint(c.getHeroRuneType(), 0, true);
        }
		
		return true;
	}
	

	




    public void LoadBasicMidLevelShit(SaveState saver)
    {        
        
		Debug.Log("Peripheral loaded snapshot, wave " + saver.current_wave + "\n");
        StopAllCoroutines();        		
		level_state = LState.WaveButton;
        previous_timeScale = 1f;
        timeScale = 1f;
        Wave_interval = 0f;
        TIME = 0f;
        Moon.Instance.WaveInProgress = false;      
		foreach (actorStats a in saver.actor_stats)	{          
            bool is_active = a.isActive();                       
           a.setActive(is_active);           
            Central.Instance.setToy(a, a.toy_type == ToyType.Hero);
		}

        foreach (Island_Button i in Monitor.Instance.islands.Values) { i.ResetIslandType(); }


        difficulty = saver.difficulty;
        if (saver.type == SaveStateType.MidLevel)
        {
            foreach(TemporarySaver ts in saver.temporary_effects)
            {
                switch (ts.type) //this is awkward but whatever
                {
                    case WishType.MoreDamage:
                        damage_factor.AddEffect(ts.percent, ts.remaining_time);
                        break;
                    case WishType.MoreDreams:
                        dream_factor.AddEffect(ts.percent, ts.remaining_time);
                        break;
                    case WishType.MoreXP:
                        xp_factor.AddEffect(ts.percent, ts.remaining_time);
                        break;
                }
            }



            foreach (actorStats a in saver.actor_stats) { Central.Instance.setToy(a, true); }

            Moon.Instance.WaveInProgress = false;
            current_wave = saver.current_wave;
            Moon.Instance.SetWave(current_wave);
            TIME = next_wave_time;
            Debug.Log("Loading snapshot\n");
            Sun.Instance.SetTime(saver.time_of_day);
            Sun.Instance.Init();

            PlaceCastle();
            ToySaver castle_toysaver = saver.getCastle();
            if (castle_toysaver != null) castle.loadSnapshot(castle_toysaver);//this guy needs to be loaded first
                else Debug.LogError("Could not find castle toysaver!"); 

            castle.rune.DoSpecialThing(EffectType.Gnomes); //load gnomesss        
            if (saver.health > 0) SetHealth(saver.health);
            dreams = saver.dreams;
            setToys(0);
            monster_count = 0;
                     
            IslandSaver island_saver;

            for (int i = 0; i < saver.islands.Count; i++)
            {
                island_saver = saver.islands[i];
                Island_Button island = null;                
                Monitor.Instance.islands.TryGetValue(island_saver.name, out island);

                if (island == null) { Debug.Log("Could not find island " + island_saver.name + "\n"); continue; }
                if (island_saver.island_type != IslandType.Null) island.ChangeType(island_saver.island_type);                

                if (island_saver.toy_saver != null)
                {
                    if (island_saver.toy_saver.rune.runetype == RuneType.Castle) continue;
                    if (island_saver.toy_saver.toy_name.Equals(Get.NullToyName())) continue;
                    Toy toy = null;


                    GameObject toy_object = makeToy(island_saver.toy_saver.toy_name, island, false, ref toy);
                    if (toy_object == null) { Debug.Log("FAILED TO LOAD TOY! NO FIREARM\n"); return; }  
                    toy.loadSnapshot(island_saver.toy_saver);

                    
                    if (island_saver.toy_saver.rune.runetype != RuneType.Modulator &&
                        (island_saver.toy_saver.type == ToyType.Normal || island_saver.toy_saver.type == ToyType.Building))
                    {
                        incrementToys();
                    }

                    if (island_saver.toy_saver.type == ToyType.Hero)
                    {
                        Debug.Log("SETTING HERO MAX LEVEL TO " + toy.rune.getMaxLevel() + " OR " + Central.Instance.getToy(island_saver.toy_saver.toy_name).getMaxLvl() + "\n");
                    }
                }
                if (island_saver.block_timer > 0)
                {
                    island.MakeDeadIsland(island_saver.block_timer);
                    Debug.Log("Place blocker on island " + island_saver.name + "\n");
                }

            }            
            //skillmaster is initialized indirectly, when the toys are initialized, through Rune
            Central.Instance.level_list.special_skill_button_driver.Init();
          //  EagleEyes.Instance.UpdateToyButtons("blah", ToyType.Normal);
            EagleEyes.Instance.UpdateToyButtons("blah", ToyType.Building, false);
        }
        else
        {
            Debug.LogError("Loading midlevel shit on a non midlevel savegame!\n");
            Debug.Log("Peripheral loaded start level snapshot, current wave is " + current_wave + "\n");
            Moon.Instance.SetWave(current_wave);
            Sun.Instance.SetTime(0);
            Sun.Instance.Init();

            PlaceCastle();

        }// end loading snapshot specific stuff


        List<Building> buildings = Peripheral.Instance.buildings;
        foreach (actorStats a in Central.Instance.actors)
        {
            if (a.required_building.Equals("")) continue;
            a.setActive(false);
            foreach (Building b in buildings)
            {
                if (b.name.Equals(a.required_building)) a.setActive(true);
            }
        }
        EagleEyes.Instance.UpdateToyButtons("blah", ToyType.Normal, false);
        level_active = true;
        ResumeNormalSpeed();
        // not sure what this is for, what are we saving for castle???
        //if (saver.castle != null){  castle.loadSnapshot(saver.castle);		}
    }
    
    public void ChangeTime(float f){
		if (Time.timeScale == f) {
			if (previous_timeScale == f) previous_timeScale = 1f;
			Time.timeScale = previous_timeScale;
		} else {
			previous_timeScale = Time.timeScale;
			Time.timeScale = f;				
		}
	}


	public void TogglePause(){	
		pause = !pause;
		Pause (pause);
	}


    public void ResumeNormalSpeed()
    {
        if (Time.timeScale > 1f)
        {
            Time.timeScale = 1f;
            previous_timeScale = 1f;
        }
    }

	public void Pause(bool _pause){
		if (_pause){
			pause = true;
		//	Debug.Log("PAUSED\n");
			previous_timeScale = Time.timeScale;
			Time.timeScale = 0.0f;
            Noisemaker.Instance.Stop();
            
		}else{
			if (previous_timeScale == 0f) previous_timeScale = 1f;
			Time.timeScale = previous_timeScale;
			pause = false;
        //    Debug.Log("UNPAUSED\n");
        }
	}

    public Firearm getHero(RuneType type)
    {
        foreach (Firearm f in firearms)
        {
            if (f.toy_type == ToyType.Hero && f.runetype == type)
                return f;
        }
        return null;
    }
    

	void Update() {
        if (!level_active)return;
		TIME += Time.deltaTime;
        if (pause) return;

        if (level_state == LState.WaveEnded)// || (level_state == LState.OnLastWavelet))
        {           
            if (current_wave < Moon.Instance.GetWaveCount())
            {
                if (Wave_interval > 0)
                {
           //         Debug.Log("Next wave time " + next_wave_time + " wave_interval " + wave_interval + "\n");
                    next_wave_time = Mathf.Floor(TIME + Wave_interval);
                    EagleEyes.Instance.WaveButton(true);
                    EagleEyes.Instance.UpdateWaveTimer(next_wave_time - Mathf.Floor(TIME));
                    level_state = LState.WaitingToStartNextWave;

                }
                else if (monsters_transform.childCount <= 0)
                {
                    EagleEyes.Instance.WaveButton(true);
                    //don't do anything until the wave is actually over, see above
                    EagleEyes.Instance.SetFFButtons(false);
                    level_state = LState.WaitingToStartNextWave;
                }

            }
            else if (level_active && monsters_transform.childCount <= 0 && (overseer == null || (overseer != null && overseer.ingame_finished == true)))
            {
                StartCoroutine(FinishLevel());
            }
     //       Debug.Log("Childcount " + monsters_transform.childCount + "\n");
        }
                
        
        if (level_state == LState.WaveButton && Mathf.Floor(TIME) > next_wave_time)
        {//this is only for start of level I think
            EagleEyes.Instance.WaveButton(true);
            EagleEyes.Instance.SetFFButtons(false);
            level_state = LState.WaitingToStartNextWave;

        }

        if (level_state == LState.WaitingToStartNextWave)
        {
            if (Wave_interval > 0 && Mathf.Floor(TIME) > next_wave_time) {
                StartWave();
                
                Debug.Log("Automatically Starting wave\n");
            }
            else {
                EagleEyes.Instance.UpdateWaveTimer(next_wave_time - Mathf.Floor(TIME));
            }
        }       

		if (make_wave){if (onWaveStart != null)onWaveStart (current_wave);make_wave = false;}
		
        if (health <= 0)
        {
            Debug.Log("YOU LOST\n");
            Noisemaker.Instance.Play("lost_game");
            Central.Instance.changeState(GameState.Lost);
            level_state = LState.Lost;
            level_active = false;         
               Pause(true);
        }
        
	}

    IEnumerator FinishLevel()
    {
        level_active = false;
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(2f));

        Debug.Log("YOU WIN LEVEL\n");
        EagleEyes.Instance.SetFFButtons(false);
        float remaining_wishes = my_inventory.GetWishScore();

        ScoreKeeper.Instance.CalcScore(dreams, health, remaining_wishes);
        Central.Instance.changeState(GameState.Won);
        level_state = LState.Won;
        
        yield return null;
    }

    public void StartWave()
    {        
        EagleEyes.Instance.SetFFButtons(true);
        float time_left = next_wave_time - Mathf.Floor(TIME);
        if (Wave_interval > 0 && time_left > 0)
        {
            float dreams_awarded = Mathf.RoundToInt(Mathf.Max(5f, time_left / 3f));
          //  Debug.Log("Awarding " + dreams_awarded + " points for starting wave early\n");
            addDreams(dreams_awarded, Vector3.one, true);//wave button position wtf
        }
        make_wave = true;
        next_monster_time = TIME;
        
        level_state = LState.WaveStarted;
        EagleEyes.Instance.WaveButton(false);

        
    }
	
	public void SetHealth(float i){		
		health = i;
		if (onHealthChanged != null) onHealthChanged(health);
	}
	
	public void AdjustHealth(float i){
//	Debug.Log("Adjusting health\n");
		health += i;
		if (i < 0){
			EagleEyes.Instance.RunHealthVisual();
			if(Noisemaker.Instance != null) Noisemaker.Instance.Play("health_decreased");		
		}
		if (onHealthChanged != null) onHealthChanged(health);
	}

	public float GetHealth(){
		return health;
	}
	
	
	
	public GameObject makeMonster(string name, int path, float point_factor, float xp_factor){
		if (path == -1) path = getPath();
		
		Vector3 posv = WaypointMultiPathfinder.Instance.paths[path].start.transform.position;
		
		return makeMonster(name, path, posv, point_factor, xp_factor);
	}
	
	public GameObject makeMonster(string name, int path, Vector3 posv, float point_factor, float xp_factor){
	//Debug.Log("Making monster " + name + "\n");
		GameObject toy = zoo.getObject ("Monsters/" + name, true);
		toy.tag = "Enemy";
		toy.transform.GetChild(0).tag = "Enemy";
		toy.layer = 12;//Unit
		toy.name = name + monster_count;	
		monster_count++;


		toy.transform.parent = monsters_transform;
		toy.transform.position = posv;
		
		
		HitMe hitme = toy.GetComponent<HitMe>();
		hitme.initStats(Central.Instance.getToy(name));
		
		hitme.stats.cost_type.Amount = point_factor;
	//	Debug.Log("xp_factor " + xp_factor + "\n");
		if (xp_factor > 0) hitme.stats.SetXp(xp_factor);


        targets.addByID(hitme);
		
       
		nightmares -= Central.Instance.getToy(name).cost_type.Amount;
        AI ai = hitme.my_ai;
		
		ai.path = path;
		ai.player = WaypointMultiPathfinder.Instance.paths[path].finish.transform;
		ai.seewhen = diagonal;
		ai.keepwalking = diagonal;
		ai.notsure = diagonal;
		ai.Init();
		enemy_list.addMonster(toy.gameObject.GetInstanceID(), ai.my_dogtag);
        if (onPlacedToy != null) { onPlacedToy(toy.name); }
        //toy.transform.localRotation = ai.GetForwardDirection();
        return toy;
	}
	
	int getPath(){
		return Mathf.FloorToInt(UnityEngine.Random.Range(0,WaypointMultiPathfinder.Instance.paths.Count));
	}

    /*
	public void getToy(ref string s){
		string name;		
		Central.Instance.effect_toys.TryGetValue (Get.RuneTypeFromString (s), out name);
		if (name != null) {
			s = name;
		}
	}
    */



	public void sellToy(Toy toy, int cost){
		addDreams(cost, toy.transform.position, false);
		toy.Die(0f);
        if (toy.toy_type == ToyType.Hero)
        {
            int i = 0;
            hero_points.TryGetValue(toy.runetype, out i);
            SetHeroPoint(toy.runetype, i + 1, true);
        }else
        {
            if (onSellToy != null) onSellToy();
        }

        Monitor.Instance.global_rune_panel.DisableMe();
        if (onSelected != null) onSelected(SelectedType.Island, "");
    }
    
    public bool checkObjectScale(string name, Vector2 size)
    {
        Vector3 scaleV = Central.Instance.getToy(name).getScale();
        if (scaleV.x > size.x || scaleV.z > size.y)
        {
      //      Debug.Log(name + " is too big for this location!\n");
            return false;
        }
        return true;
    }


	public void PlaceCastle(){
		if (Monitor.Instance.castle_island != null){		
		
			
			makeToy("castle", Monitor.Instance.castle_island, false, ref castle);
           // castle.rune.DoSpecialThing(EffectType.Gnomes);
		//	Debug.Log("made castle with cost mult " + Central.Instance.base_toy_cost_mult + "\n");
		}
		
	}


 

    public GameObject makeToy(string name, GameObject parent, ref Toy firearm)
    {
        Island_Button island_b = parent.GetComponent<Island_Button>();
		
		return makeToy(name, island_b, true, ref firearm);
    }

    public GameObject makeToy(string name, Island_Button island_b, bool check_cost, ref Toy _toy)
    {
       // Debug.Log("MakeToy " + name + "\n");

        float range_multiplier = island_b.getSize();
        Vector3 posv;
        GameObject parent = island_b.gameObject;
        actorStats my_stats = Central.Instance.getToy(name);

        Cost my_cost_type = my_stats.cost_type;
    //    Debug.Log("Cost type for " + name + " is " + my_cost_type.toytype + "\n");
        if (check_cost && my_cost_type.type !=  CostType.Dreams)
        {
            if (!HaveResource(my_stats.cost_type))
            {
                Debug.Log("Cannot afford toy " + name + " of type " + my_cost_type + " for " + my_cost_type.Amount + " cost\n");
                return null;
            }
        }

//        getToy(ref name);
  //      Debug.Log("MakeToyeeeee " + name + "\n");
        Vector3 scaleV = Central.Instance.getToy(name).getScale();

        island_b.blocked = true;

        parent = island_b.parent; //parent that's used for transform
        posv = parent.transform.position;
        GameObject toy = zoo.getObject("Toys/" + name, true);

        toy.tag = "Player";
        toy.layer = 10;
        toy.name = name + getToys();
        toy.transform.parent = parent.transform;
        toy.transform.position = posv;
        toy.transform.localScale = Vector3.one;// = Vector3.one;
        Building b = null;
        float bonus = 0f;
        //why is this so awkward. this whole function is so unpleasant.
        Toy TOY = toy.GetComponent<Toy>();
        if (TOY.toy_type == ToyType.Building || TOY.runetype == RuneType.Castle)
        {
            b = toy.GetComponent<Building>();
            buildings.Add(b);
            _toy = b;
           
        }
        else {            
            _toy = toy.GetComponent<Firearm>();
            firearms.Add((Firearm)_toy);
        }
        if (island_b.my_toy != null && island_b.my_toy.gameObject.activeSelf)
        {
            throw new Exception("Adding toy " + name + " to an island " + island_b.name + " that already has a toy\n");
        }

    //    Debug.Log("Added toy " + name + " to an island " + island_b.name + "\n");
        island_b.my_toy = _toy;
        _toy.my_name = name;

        Rune rune = null;

        bool new_hero = false;
        if (TOY.toy_type == ToyType.Hero)
        {
            ToySaver toysaver = Central.Instance.getHeroStats(_toy.runetype);
            if (toysaver != null) { rune = toysaver.rune; }else { new_hero = true; }
        }

        if (_toy != null)
        {
            _toy.initStats(my_stats, scaleV, island_b, name, rune);
        }


        if (b != null) b.StartConstruction(b.init_construction_time);

        //   Debug.Log("NEW TOY cost toytype " + my_cost_type.toytype + "\n");

        if (check_cost || my_cost_type.isHero())
        {
            if (my_cost_type.type == CostType.Dreams) {
                if (_toy.runetype != RuneType.Modulator)  incrementToys();
                addDreams(-my_cost_type.Amount, toy.transform.position, false);
                Central.Instance.updateCost(toys);
                // if (Central.Instance.getToy(name).cost_type.cost > dreams) { SelectToy(""); }
            } else if (my_cost_type.type == CostType.Wishes) {
                my_inventory.AddWish(WishType.Sensible, -my_cost_type.Amount, 1);
            } else if (my_cost_type.isHero())
            {
                SetHeroPoint(my_cost_type.getHeroRuneType(), 0, true);
                SelectToy(null, RuneType.Null);
            }

        }


        if (onPlacedToy != null) {// Debug.Log("Peripheral placed " + _toy.my_name + "\n");
            onPlacedToy(_toy.my_name); }

        if (new_hero) Central.Instance.hero_toy_stats.Add(_toy.getSnapshot());

        if (check_cost) Noisemaker.Instance.Play("make_tower");
        return toy;

    }


	public void ActivateToy(string _text){
        if (!haveToys.ContainsKey(_text))
        {
            Debug.Log("Cannot activate toy " + _text + ", it is not in haveToys\n");
            return;
        }

		actorStats toy = Central.Instance.getToy(_text);
		//Central.Instance.actors.TryGetValue(_text,out toy);
		

		if (toy == null){Debug.Log("Trying to activate an invalid toy " + _text + "\n"); return;}

		else{toy.setActive(true);}
		EagleEyes.Instance.UpdateToyButtons("blah",ToyType.Null, false);
		
	}

    public void SetDreams(float _dreams)
    {
        dreams = _dreams;
        EagleEyes.Instance.UpdateToyButtons("blah", ToyType.Normal, false);
        EagleEyes.Instance.UpdateToyButtons("blah", ToyType.Building, false);
        if (onDreamsChanged != null) onDreamsChanged(dreams, false, Vector3.zero);
    }

    public void addDreams(float delta, Vector3 pos, bool factor)
    {
        //	Debug.Log("Peripheral Add dreams " + delta + "\n");
        if (factor) delta *= dream_factor.getStat();
        dreams += delta;
        EagleEyes.Instance.UpdateToyButtons("blah", ToyType.Normal, false);
        EagleEyes.Instance.UpdateToyButtons("blah", ToyType.Building, false);
        if (onDreamsAdded != null) onDreamsAdded(delta, pos);
        if (onDreamsChanged != null) onDreamsChanged(dreams, true, pos);
    }

    public float getDreams(){
		return dreams;
	}

}//end of class

[System.Serializable]
public class SelectedToy
{
    public string name;
    public IslandType island_type;


    public SelectedToy(string n, IslandType type)
    {
        name = n;
        island_type = type;
    }
}
