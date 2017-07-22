using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Peripheral : MonoBehaviour {
    public float dreams;
    public string title;
    public int max_dreams;

    private int toys;
    private int all_toys;  //used for rune IDs for PlayerSimulator and AWS event tracking
    float health;
    private float max_health;

    public LState level_state;
//    bool wave_button_visible;

    public EnemyList enemy_list = new EnemyList();
    public MyArray<HitMe> targets;
    public Transform monsters_transform;
    public Transform arrows_transform;
    public Inventory my_inventory;   //wish inventory
    public SkillMaster my_skillmaster; //special skills
    float init_fixedDeltaTime = 0.0333333f;
    public Difficulty difficulty;
    
    public Toy castle;    
    public Dictionary<RuneType, int> hero_points;
    public List<Toy> toy_parents; //this toy is a parent to enable another toy. should be only sensible city for now
    public List<Firearm> firearms;
    public Dictionary<string, bool> haveMonsters;

    public float next_monster_time;
    bool level_active = false;
    
    public bool make_wave = false;
    

    SelectedToy toy_selected;
    RuneType rune_selected;
    RuneType selected_rune;

    public VariableStat xp_factor;
    public VariableStat damage_factor;
    public VariableStat dream_factor;

    //bool done_making_wave = false;
    public float default_wave_interval;         // waves are spaced at least X seconds apart
    private float wave_interval;         // waves are spaced at least X seconds apart
    float next_wave_time;                   // first wave time
    
    TimeScale previous_timeScale;
    TimeScale current_timeScale;

    public int monster_count; //purely for naming purposes
    public int cheapest_monster_cost;  //for this wave

   // public LevelMod[] level_mod; //xp lvl multiplier for testing purposes, ignore tower lvl caps, etc
    public EnvType env_type; //used to determine island sprites
    Vector3 monster_source;
    GameObject pathfinder;
    public float tileSize;
    public float diagonal;
    private List<int> cities;
    public bool have_cities;
    public bool building_a_city = false;
    public float TIME;
    public GameObject island_selected;
    public EventOverseer overseer;
    public PathfinderType pathf;
    public Zoo zoo;
    Queue<string> file;
    public PsaiTriggerOnSignal on_wave_start_trigger;
    public PsaiTriggerOnSignal on_level_start_trigger;

    private static Peripheral instance;

    public delegate void OnCreatePeripheralHandler(Peripheral p);
    public static event OnCreatePeripheralHandler onCreatePeripheral;

    public delegate void OnSelectHandler(SelectedType type, string content);
    public static event OnSelectHandler onSelected;

    public delegate void OnPlaceToyHandler(string content, RuneType runetype, ToyType toytype);
    public static event OnPlaceToyHandler onPlacedToy;

    public delegate void OnWaveStartHandler(int content);
    public static event OnWaveStartHandler onWaveStart;

    public delegate void onHealthChangedHandler(float i, bool visual);
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
        //    Debug.Log("Setting wave interval\n");
            wave_interval = value;
        }
    }

    public float MaxHealth
    {
        get { return max_health; }
    }


    void Init() {
        Debug.Log("Peripheral INITIALIZING\n");	
        //done_making_wave = false;
        targets = new MyArray<HitMe>();
        monster_count = 0;
        level_state = LState.WaveButton;
        //wave_button_visible = false;
        my_inventory.InitWishes();
        cities = new List<int>();
        have_cities = false;
        building_a_city = false;
        default_wave_interval = 0.1f;           // waves are spaced at least X seconds apart
        next_wave_time = 0.5f;                  // first wave time        
        previous_timeScale = TimeScale.Normal;
        current_timeScale = TimeScale.Normal;
        setTimeScale(current_timeScale);
        toy_parents = new List<Toy>();
        firearms = new List<Firearm>();
       // haveToys = new List<string>();
        haveMonsters = new Dictionary<string, bool>();
        toy_selected = null;
        rune_selected = RuneType.Null;
        //	monsters = 0;
        Wave_interval = 0;
        next_monster_time = 0;
        level_active = false;
        if (Moon.Instance != null) Moon.Instance.InitEmpty();

        island_selected = null;        
        make_wave = false;
        dreams = 0;
        toys = 0;
        all_toys = 0;
        TIME = 0f;
        my_inventory.InitWishes();
        hero_points = new Dictionary<RuneType, int>();
        hero_points.Add(RuneType.Sensible, 1);
        hero_points.Add(RuneType.Airy, 1);
        hero_points.Add(RuneType.Vexing, 1);
        zoo = Zoo.Instance;
    }

    public LevelMod getLevelMod()
    {
        List<LevelMod> level_mod = LevelStore.getLevelSettings(Central.Instance.current_lvl);
        foreach (LevelMod mod in level_mod)
        {
            if (difficulty == mod.difficulty) return mod;
        }
  //      Debug.LogWarning("Could not find a level mod for difficulty " + difficulty + "\n");
        //fallback - need better fallback or don't let people load a difficulty setting that's not defined for the level, duh
        foreach (LevelMod mod in level_mod)
        {
            if (mod.difficulty == Difficulty.Normal) return mod;
        }

        Debug.LogError("Could not find a level mod!\n");
        return null;
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
        setTimeScale(TimeScale.Normal);
        
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
        if (toy_selected != null && toy_selected.name != null)
        {       
            return toy_selected.name;
        }
        else
            return "";
	}

    public Firearm getHeroFirearm (RuneType type)
    {
        
        foreach (Firearm f in firearms)
        {
            if (f.toy.runetype == type && f.toy.toy_type == ToyType.Hero) return f;
        }

        return null;
    }


    void SetAllowedRange(string toy)
    {
        if (!toy.Equals(""))
        {
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

        if (toy.Equals(""))
        {
            toy_selected = null;
            SetAllowedRange("");
        
            if (!quietly && onSelected != null) onSelected(SelectedType.Toy, "");

            return false;
        }
        else {
            rune_selected = rune;            
            unitStats stats = Central.Instance.getToy(toy);
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
   //     Debug.Log("Loaded level " + level + " with difficulty " + difficulty + "\n");

        monsters_transform = Monsters_Bucket.Instance.gameObject.transform;
        arrows_transform = GameObject.Find("Arrows").gameObject.transform;
        Moon.Instance.monsters_transform = monsters_transform;

        
        
       //     Debug.Log("using FancyLoader to load " + level + "\n");
        if (level.fancy)
            FancyLoader.Instance.LoadLevel(level.name);
        else         
        {

            file = new Queue<string>(((TextAsset)Resources.Load("Levels/" + level.name)).text.Split('\n'));
            Debug.Log("Loading file " + level.name + "\n");
            Loader.Instance.setFile(file);
            Loader.Instance.oldLoadLevel();
        }
        //SetDiagonal(Vector3.Distance(WaypointMultiPathfinder.Instance.paths[0].finish.transform.position, WaypointMultiPathfinder.Instance.paths[0].start.transform.position) * 2);
        //the above does nto work for paths that loop back
        SetDiagonal(60f);
        
        overseer = EventOverseer.Instance;

		level_state = LState.WaveButton;
		Moon.Instance.WaveInProgress = false;
		level_active = true;

		//Debug.Log ("Peripheral FINISHED LOADING at " + Duration.realtimeSinceStartup);
		if (GameStatCollector.Instance !=  null) GameStatCollector.Instance.Init();

    //    on_level_start_trigger.OnSignal();
		return true;
	}

    //NOT USING CHECK_RESOURCE
    public GameObject PlaceToyOnIsland(string placeme, GameObject o, bool check_resource)
    {

        if (onSelected != null) onSelected(SelectedType.Island, "");
        Toy f = null;

        GameObject toy = makeToy(placeme, o, ref f);
        if (toy == null)
        {
            EagleEyes.Instance.StopSignal();
            Debug.Log("UH OH\n");
            return null;
        }

        EagleEyes.Instance.ClearInfo();

        return toy;
    }

    public GameObject PlaceToyOnIsland(string placeme, GameObject o){
        return PlaceToyOnIsland(placeme, o, true);
	}
	


    public bool WaveCountdownOngoing()
    {
        return (Peripheral.Instance.level_state == LState.WaitingToStartNextWave && Peripheral.Instance.Wave_interval <= 0);
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
            ScoreKeeper.Instance.useScore(Mathf.FloorToInt(c.Amount));
        }
        else if (c.isHero())
        {
            SetHeroPoint(c.getHeroRuneType(), 0, true);
        }
		
		return true;
	}
	

	



    public void LoadBasicMidLevelShit(SaveState saver)
    {        
        
	//	Debug.Log("Peripheral loaded snapshot, wave " + saver.current_wave + "\n");
        StopAllCoroutines();        		
		level_state = LState.WaveButton;
        ChangeTime(TimeScale.Normal);        
        Wave_interval = 0f;
        TIME = 0f;
        have_cities = false;
        toy_parents = new List<Toy>();
        Moon.Instance.WaveInProgress = false;      
		foreach (unitStatsSaver a in saver.actor_stats)	{                      
            Central.Instance.setUnitStats(a, a.toy_id.toy_type == ToyType.Hero);
		}

        Monitor.Instance.ResetIslands();

        xp_factor.Reset();
        damage_factor.Reset();
        dream_factor.Reset();

        difficulty = saver.difficulty;        
        

        if (saver.type == SaveStateType.MidLevel)
        {
            FancyLoader.Instance.LoadWavesOnly(Central.Instance.level_list.levels[Central.Instance.current_lvl].name);

            Moon.Instance.SetWave(saver.current_wave,0);
            Moon.Instance.WaveInProgress = false;
            Moon.Instance.SetTime(saver.time_of_day);
            foreach (unitStatsSaver a in saver.actor_stats) { Central.Instance.setUnitStats(a, true); }
            
            TIME = next_wave_time;
            Debug.Log("Peripheral Loading snapshot\n");
           // Sun.Instance.SetTimePassively(saver.time_of_day);
            
            Sun.Instance.Init();

            PlaceCastle();
            RuneSaver castle_toysaver = saver.getCastle();
            if (castle_toysaver != null) castle.loadSnapshot(castle_toysaver);//this guy needs to be loaded first
                else Debug.LogError("Could not find castle toysaver!"); 

            //castle.rune.DoSpecialThing(EffectType.Architect); //load gnomesss        


            if (saver.health > 0) SetHealth(saver.health, false);
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
                if (island_saver.island_type != IslandType.Null) island.ChangeType(island_saver.island_type, false);                

                if (island_saver.toy_saver != null)
                {
                    if (island_saver.toy_saver.rune_saver.runetype == RuneType.Castle) continue;
                    if (island_saver.toy_saver.toy_name.Equals(Get.NullToyName())) continue;
                    Toy toy = null;


                    GameObject toy_object = makeToy(island_saver.toy_saver.toy_name, island, false, ref toy, false);
                    if (toy_object == null) { Debug.Log("FAILED TO LOAD TOY! NO FIREARM\n"); return; }  
                    toy.loadSnapshot(island_saver.toy_saver);
                    if (toy.firearm != null)  toy.firearm.CheckForUpgrades();
                    
                    if (island_saver.toy_saver.rune_saver.runetype != RuneType.Modulator &&
                        (island_saver.toy_saver.type == ToyType.Normal))
                    {
                        incrementToys();
                    }

                    if (island_saver.toy_saver.type == ToyType.Hero)
                    {
                  //      Debug.Log("SETTING HERO MAX LEVEL TO " + toy.rune.getMaxLevel() + " OR " + Central.Instance.getToy(island_saver.toy_saver.toy_name).getMaxLvl() + "\n");
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
            EagleEyes.Instance.UpdateToyButtons("blah", ToyType.Normal, false);
        }
        else
        {
            Debug.LogError("Loading midlevel shit on a non midlevel savegame!\n");
            Debug.Log("Peripheral loaded start level snapshot, current wave is " + 0 + "\n");
            Moon.Instance.SetWave(0,0);
            Sun.Instance.SetTimePassively(0);
            Sun.Instance.Init();




            PlaceCastle();

        }// end loading snapshot specific stuff
      
        EagleEyes.Instance.UpdateToyButtons("blah", ToyType.Normal, false);
        level_active = true;
        ChangeTime(TimeScale.Normal);
        // not sure what this is for, what are we saving for castle???
        //if (saver.castle != null){  castle.loadSnapshot(saver.castle);		}
    }


    public void ToggleFast()
    {
        TimeScale change_to = (current_timeScale == TimeScale.Fast) ? TimeScale.Normal : TimeScale.Fast;
        ChangeTime(change_to);
    }

    public void TogglePause()
    {
        TimeScale change_to = (current_timeScale == TimeScale.Pause) ? TimeScale.Resume : TimeScale.Pause;
        ChangeTime(change_to);
    }

    public bool isPaused()
    {
        return current_timeScale == TimeScale.Pause;
    }

    public TimeScale getCurrentTimeScale()
    {
        return current_timeScale;
    }

    public void Pause(bool set)
    {
        TimeScale change_to = (set) ? TimeScale.Pause : TimeScale.Resume;
        ChangeTime(change_to);
    }

    public void ChangeTime(TimeScale type){
        if (type == TimeScale.Null) return;
        if (current_timeScale == type) return;

        if (EagleEyes.Instance.menu && type != TimeScale.Pause) { Debug.Log("Changing time while in menu to " + type + " is no good\n"); return; }
      //    Debug.Log("Changing time: PREVIOUS: " + previous_timeScale + " CURRENT: " + current_timeScale + " CHANGING TO: " + type + "\n");

        

        switch (type)
        {
            case TimeScale.Normal:
                setTimeScale(type);                
                previous_timeScale = TimeScale.Normal;
                break;
            case TimeScale.Pause:
                previous_timeScale = (TimeScaleAbsolute(current_timeScale)) ? current_timeScale : TimeScale.Normal;
                setTimeScale(type);
                Noisemaker.Instance.Stop();
                break;
            case TimeScale.Resume:
                previous_timeScale = (TimeScaleAbsolute(previous_timeScale)) ? previous_timeScale : TimeScale.Normal;
                setTimeScale(previous_timeScale);                
                break;
            case TimeScale.Fast:
                previous_timeScale = (TimeScaleAbsolute(current_timeScale)) ? current_timeScale : TimeScale.Normal;
                setTimeScale(type);
                break;
            case TimeScale.SuperFastPress:
                previous_timeScale = (TimeScaleAbsolute(current_timeScale)) ? current_timeScale : TimeScale.Normal;
                setTimeScale(type);
                break;
            case TimeScale.Null:                
                break;
        }
        current_timeScale = (TimeScaleAbsolute(type) || type == TimeScale.Pause) ? type : previous_timeScale;

        if (Central.Instance.state == GameState.InGame && 
            (current_timeScale == TimeScale.Normal || current_timeScale == TimeScale.Fast || current_timeScale == TimeScale.SuperFastPress ))
            Tracker.Log(PlayerEvent.TimeScaleChange, true,
                       customAttributes: new Dictionary<string, string>() { 
                        { "attribute_1", current_timeScale.ToString() } },
                       customMetrics: null);

        /*
		if (Duration.timeScale == f) {
			if (previous_timeScale == f) previous_timeScale = 1f;
			Duration.timeScale = previous_timeScale;
		} else {
			previous_timeScale = Duration.timeScale;
			Duration.timeScale = f;				
		}
        */
    }

    bool TimeScaleAbsolute(TimeScale type)
    {
        return (type == TimeScale.Normal || type == TimeScale.Fast || type == TimeScale.SuperFastPress);
    }
    void setTimeScale(TimeScale type)
    {       
        if (EagleEyes.Instance != null)EagleEyes.Instance.SetTimeScaleFFButtons(type);
        switch (type)
        {
            case TimeScale.Fast:
                Time.timeScale = 2f;
                return;
            case TimeScale.Normal:
                Time.timeScale = 1;
                return;
            case TimeScale.Pause:
                Time.timeScale = 0f;
                return;
            case TimeScale.SuperFastPress:
                Time.timeScale = 5f;
                return;
            case TimeScale.Null:
                Time.timeScale = 1;
                return;
            default:
                Debug.LogError("Can't set this timeScale cuz it ain't absolute: " + type + ". What did you do.\n");
                return;
        }
    }

    public Firearm getHero(RuneType type)
    {
        foreach (Firearm f in firearms)
        {
            if (f.toy.toy_type == ToyType.Hero && f.toy.runetype == type)
                return f;
        }
        return null;
    }
    

	void Update() {
        if (Time.timeScale == 0) return;

        if (!level_active)return;
		TIME += Time.deltaTime;
        if (current_timeScale == TimeScale.Pause) return;

        if (level_state == LState.WaveEnded)// || (level_state == LState.OnLastWavelet))
        {           
            if (Moon.Instance.GetCurrentWave() < Moon.Instance.GetWaveCount())
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
                    EagleEyes.Instance.SetActiveFFButtons(false);
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
            EagleEyes.Instance.SetActiveFFButtons(false);
            level_state = LState.WaitingToStartNextWave;

        }

        if (level_state == LState.WaitingToStartNextWave)
        {
        //    Debug.Log("wave_interval " + Wave_interval + " TIME " + TIME + " next_wave_time " + next_wave_time + "\n");
            if (Wave_interval > 0 && Mathf.Floor(TIME) > next_wave_time && !LevelBalancer.Instance.am_enabled) {
                StartWave();
                
         //       Debug.Log("Automatically Starting wave\n");
            }
            else {
                EagleEyes.Instance.UpdateWaveTimer(next_wave_time - Mathf.Floor(TIME));
            }
        }

        if (make_wave)
        {
            if (onWaveStart != null) onWaveStart(Moon.Instance.GetCurrentWave());
            make_wave = false;
        //    on_wave_start_trigger.OnSignal();
        }
        
        if (health < 1)
        {
            Debug.Log("YOU LOST\n");
            Noisemaker.Instance.Play("lost_game");
            Central.Instance.changeState(GameState.Lost);
            level_state = LState.Lost;
            level_active = false;
            ChangeTime(TimeScale.Pause);
        }
        
	}

    IEnumerator FinishLevel()
    {
        level_active = false;
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(2f));

        //Tracker.Log("Level Finished" + Central.Instance.current_lvl + " " + Peripheral.Instance.difficulty.ToString() + " health " + Peripheral.Instance.GetHealth());
        


        Debug.Log("YOU WIN LEVEL\n");
        EagleEyes.Instance.SetActiveFFButtons(false);
       // float remaining_wishes = my_inventory.GetWishScore();

        ScoreKeeper.Instance.CalcScore(dreams, health);
        Central.Instance.changeState(GameState.Won);
        level_state = LState.Won;
        
        yield return null;
    }

    public void StartWave()
    {        
        EagleEyes.Instance.SetActiveFFButtons(true);
        float time_left = next_wave_time - Mathf.Floor(TIME);
        if (Wave_interval > 0 && time_left > 0)
        {
            float dreams_awarded = Mathf.RoundToInt(Mathf.Max(5f, time_left / 3f));
            //  Debug.Log("Awarding " + dreams_awarded + " points for starting wave early\n");
            


            if (!LevelBalancer.Instance.am_enabled)
            {
                addDreams(dreams_awarded, Vector3.one, true);//wave button position wtf
                Tracker.Log(PlayerEvent.StartWaveEarly, true,
                 customAttributes: new Dictionary<string, string>(){ { "attribute_2", dreams_awarded.ToString() },
                                                                 { "attribute_1", Moon.Instance.current_wave.ToString()} },
                 customMetrics: new Dictionary<string, double>() { { "metric_2", next_wave_time - TIME },
                                                                  { "metric_1",Wave_interval} });
            }
        }
        make_wave = true;
        next_monster_time = TIME;
        
        level_state = LState.WaveStarted;
        EagleEyes.Instance.WaveButton(false);

        
    }

    public bool canBuildToy(unitStats toy)
    {        
        if (toy.toy_id.rune_type == RuneType.SensibleCity && (have_cities || building_a_city)) return false;
        if (toy.required_building.Equals("")) return true;
        return have_cities;
    }

    public bool canBuildToy(RuneType type, string required_building)
    {
        if (type == RuneType.SensibleCity && (have_cities || building_a_city)) return false;
        if (required_building.Equals("")) return true;
     
        return have_cities;
    }

    public bool addedCity(bool added, Toy added_tower)
    {        
        if (added_tower.runetype != RuneType.SensibleCity) return have_cities;

        if (added)
        {
            Peripheral.Instance.building_a_city = false;
            bool need_to_update_buttons = !have_cities;
            ListUtil.Add<int>(ref cities, added_tower.gameObject.GetInstanceID());
            have_cities = true;
                

            if (need_to_update_buttons) EagleEyes.Instance.UpdateToyButtons("blah", ToyType.Temporary, false);
        }
        else
        { 
            ListUtil.Remove<int>(ref cities, added_tower.gameObject.GetInstanceID());
            bool need_to_update_buttons = have_cities;

            have_cities = (cities.Count > 0);

            if (need_to_update_buttons) EagleEyes.Instance.UpdateToyButtons("blah", ToyType.Temporary, false);
        }
        return have_cities;
    }

    public void SetHealth(float i, bool visual){		
		health = i;
		if (onHealthChanged != null) onHealthChanged(health, visual);
	}

    public void SetInitHealth(float i)
    {
        max_health = i;
        health = i;
        if (onHealthChanged != null) onHealthChanged(health, false);
    }


    public void AdjustHealth(float i)
    {
        AdjustHealth(i, true);
    }


    public void AdjustHealth(float i, bool visual)
    {
        if (i > 0 && health >= MaxHealth) return;

   		health += i;
        if (health > MaxHealth) health = MaxHealth;
		if (i < 0 && visual){
			EagleEyes.Instance.RunHealthVisual();
			if(Noisemaker.Instance != null) Noisemaker.Instance.Play("health_decreased");            
		}
		if (onHealthChanged != null) onHealthChanged(health, visual);
	}

	public float GetHealth(){
		return health;
	}
	
	
	
	public HitMe makeMonster(string name, int path, float point_factor, float xp_factor){
		if (path == -1) path = getPath();
        if (path >= WaypointMultiPathfinder.Instance.paths.Count) return null;
		Vector3 posv = WaypointMultiPathfinder.Instance.paths[path].start.transform.position;
		
		return makeMonster(name, path, posv, point_factor, xp_factor);
	}
	
	public HitMe makeMonster(string name, int path, Vector3 posv, float point_factor, float xp_factor){

        EnemyType enemy_type = EnumUtil.EnumFromString<EnemyType>(name, EnemyType.Null);

        if (enemy_type == EnemyType.Null)
        {
            Debug.LogError("Cannot make monster of type Null from " + name + "\n");
            return null;
        }

        GameObject toy = zoo.getObject ("Monsters/" + name, true);
		toy.tag = "Enemy";
		toy.transform.GetChild(0).tag = "Enemy";
      //  Debug.Log("LAYER " + toy.layer + "\n");
	//toy.layer = 12;//Unit
		toy.name = name + monster_count;	
		monster_count++;

        


		toy.transform.parent = monsters_transform;
		toy.transform.position = posv;
        toy.transform.localScale = Vector3.one;// = Vector3.one;

        HitMe hitme = toy.GetComponent<HitMe>();

		hitme.initStats(Central.Instance.getEnemy(enemy_type));

       // Debug.Log(hitme.gameObject.name + " " + hitme.GetInstanceID() + " " + Duration.time + " SPAWNED!\n");

        hitme.stats.point_factor = point_factor;
	//	Debug.Log("xp_factor " + xp_factor + "\n");
		if (xp_factor >= 0) hitme.stats.SetXp(xp_factor);


        targets.addByID(hitme);

        hitme.my_ai.my_dogtag.my_name = name + " " + Moon.Instance.getEnemyID();
        hitme.my_ai.my_dogtag.wave = Moon.Instance.current_wave;

        AI ai = hitme.my_ai;
		
		ai.path = path;
		ai.player = WaypointMultiPathfinder.Instance.paths[path].finish.transform;
		ai.seewhen = diagonal;
		ai.keepwalking = diagonal;
		ai.notsure = diagonal;
		ai.Init();
		enemy_list.addMonster(toy.transform.GetInstanceID(), ai.my_dogtag);
        if (onPlacedToy != null) { onPlacedToy(toy.name, RuneType.Null, ToyType.Null); }
        //toy.transform.localRotation = ai.GetForwardDirection();
        return hitme;
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

        Tracker.Log(PlayerEvent.SellTower, true,
           customAttributes: new Dictionary<string, string>() { { "attribute_1", toy.my_name }, { "attribute_2", toy.island.ID } },
           customMetrics: new Dictionary<string, double>() { { "metric_1", toy.rune.order } });

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
    
  


	public void PlaceCastle(){
		if (Monitor.Instance.castle_island != null){		
		
			
			makeToy("castle", Monitor.Instance.castle_island, false, ref castle, false);
           // castle.rune.DoSpecialThing(EffectType.Architect);
		//	Debug.Log("made castle with cost mult " + Central.Instance.base_toy_cost_mult + "\n");
		}

	    foreach (Island_Button b in Monitor.Instance.fake_castles)
	    {
	        if (!b.blocked)InitToyObj(b, "fake_castle", true);
	    }
	}


 

    public GameObject makeToy(string name, GameObject parent, ref Toy firearm)
    {
        Island_Button island_b = parent.GetComponent<Island_Button>();
		
		return makeToy(name, island_b, true, ref firearm, false);
    }

    public Toy InitToyObj(Island_Button island_b, string toy_name, bool fake)
    {
        if (island_b.my_toy != null && island_b.my_toy.gameObject.activeSelf)
        {
            throw new Exception("Adding toy " + toy_name + " to an island " + island_b.name + " that already has a toy\n");
        }

        GameObject parent = island_b.parent;
        Vector3 posv = parent.transform.position;

        island_b.blocked = true;
        GameObject toy_obj = zoo.getObject("Toys/" + toy_name, false);

        toy_obj.tag = "Player";               
        toy_obj.layer = 10;
        
        Toy toy = toy_obj.GetComponent<Toy>();        
                
        toy_obj.transform.parent = parent.transform;
        toy_obj.transform.position = posv;
        toy_obj.transform.localScale = Vector3.one;// = Vector3.one;
        toy_obj.transform.localRotation = Quaternion.identity;
        toy_obj.SetActive(true);
        StationaryZ z = toy_obj.GetComponent<StationaryZ>();
        if (z != null) z.Init();

        return toy;
    }

    public GameObject makeToy(string toy_name, Island_Button island_b, bool check_cost, ref Toy _toy, bool force)
    {
    
        
        unitStats my_stats = Central.Instance.getToy(toy_name);
        Cost my_cost_type = my_stats.cost_type;
      
        if (!force && check_cost && my_cost_type.type !=  CostType.Dreams)
        {
            if (!HaveResource(my_stats.cost_type))
            {
                Debug.Log("Cannot afford toy " + toy_name + " of type " + my_cost_type + " for " + my_cost_type.Amount + " cost\n");
                return null;
            }
        }

        

        _toy = InitToyObj(island_b, toy_name, false);
        GameObject toy_obj = _toy.gameObject;

        if (_toy.target_toys.Length > 0) toy_parents.Add(_toy);
        
        if (_toy.firearm != null) firearms.Add(_toy.firearm);
              
        island_b.my_toy = _toy;
        _toy.my_name = toy_name;

        Rune rune = null;

        bool new_hero = false;
        if (_toy.toy_type == ToyType.Hero)
        {
            Rune have = Central.Instance.getHeroStats(_toy.runetype);
            if (have != null) { rune = have; }else { new_hero = true; }
        }

        if (_toy != null) _toy.initStats(my_stats, Vector3.one, island_b, toy_name, rune);
       

        if (_toy.building) _toy.building.StartConstruction();

        //   Debug.Log("NEW TOY cost toytype " + my_cost_type.toytype + "\n");

        _toy.name = toy_name + "|" + all_toys;
        _toy.rune.order = all_toys;

        all_toys++;
        if (check_cost || my_cost_type.isHero())
        {
            if (my_cost_type.type == CostType.Dreams) {
                if (_toy.runetype != RuneType.Modulator)  incrementToys();
                addDreams(-my_cost_type.Amount, toy_obj.transform.position, false);
                Central.Instance.updateCost(toys);
                // if (Central.Instance.getToy(toy_name).cost_type.cost > dreams) { SelectToy(""); }
            }
            else if (my_cost_type.type == CostType.Wishes) {
                my_inventory.AddWish(WishType.Sensible, -my_cost_type.Amount, 1);
            } else if (my_cost_type.isHero())
            {
                SetHeroPoint(my_cost_type.getHeroRuneType(), 0, true);
                SelectToy(null, RuneType.Null);
            }

        }


        if (onPlacedToy != null) onPlacedToy(_toy.my_name, _toy.runetype, _toy.toy_type);


        if (new_hero) Central.Instance.hero_toy_stats.Add(_toy.rune);
        if (check_cost) Noisemaker.Instance.Play("make_tower");

        if (check_cost) Tracker.Log(PlayerEvent.TowerBuilt,true,
            customAttributes: new Dictionary<string, string>() { { "attribute_1", _toy.my_name}, { "attribute_2", island_b.ID} },
            customMetrics: new Dictionary<string, double>() { { "metric_1", _toy.rune.order } });

        
        return toy_obj;

    }

  

    /*
	public void ActivateToy(string _text){
        if (Central.Instance.getToy(_text).isUnlocked)
        {
            Debug.Log("Cannot activate toy " + _text + ", it is not in haveToys\n");
            return;
        }

		unitStats toy = Central.Instance.getToy(_text);
		//Central.Instance.actors.TryGetValue(_text,out toy);
		

		if (toy == null){Debug.Log("Trying to activate an invalid toy " + _text + "\n"); return;}

		else{toy.setActive(true);}
		EagleEyes.Instance.UpdateToyButtons("blah",ToyType.Null, false);
		
	}
    */
    public void SetDreams(float _dreams)
    {
        dreams = _dreams;
        EagleEyes.Instance.UpdateToyButtons("blah", ToyType.Normal, false);
        
        if (onDreamsChanged != null) onDreamsChanged(dreams, false, Vector3.zero);
    }

    public void addDreams(float delta, Vector3 pos, bool factor)
    {
      //  	Debug.Log("Peripheral Add dreams " + delta + "\n");
        if (factor) delta *= dream_factor.getStat();
        dreams += delta;
        EagleEyes.Instance.UpdateToyButtons("blah", ToyType.Normal, false);
        
        if (onDreamsAdded != null) onDreamsAdded(delta, pos);
        
        if (onDreamsChanged != null)
        {
            onDreamsChanged(dreams, true, pos);
        }
    }

    public float getDreams(){
		return dreams;
	}

}//end of class

[System.Serializable]
public class SelectedToy
{
    public string name = "";
    public IslandType island_type;


    public SelectedToy(string n, IslandType type)
    {
        name = n;
        island_type = type;
    }
}
