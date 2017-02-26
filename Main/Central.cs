using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;


//DO NOT CHANGE THIS ORDER OR THE PREFABS GET MESSED UP DUH
public enum SaveWhen { Null, MidLevel, EndOfLevel, BetweenLevels, BeginningOfLevel }
public enum SaveStateType { Null, Persistent, MidLevel }

public enum LeanTweenerPreset { Null, DefaultFastButton, DefaultSlowButton, UIBlip, UI3Blips }

public enum ErrorType { Yes, No, NoResources }    

public enum ClickType { Null, Success, Error, Action, Cancel }

public enum EventType { ScoreUpdate, DreamUpdate, WishUpdate}

public enum Bool { Never, True, False }

public enum LabelUnit { Null, Percent, Time, Damage, Distance, Bullets}

public enum LabelName { Null, SkillStrength, TimeRemaining, CurrentLvlSkillDesc, NextLvlSkillDesc, UpgradeCost, Level, Bullets, Range, Name, Requirement, Reward }

public enum CostType { Dreams, Wishes, SensibleHeroPoint, AiryHeroPoint, VexingHeroPoint, ScorePoint};


public enum Condition { TIME, Click, Selected, PlacedToy, WaveStarted, WaveEnded, GotWish, TIMEInterval, ClickedOnToy, GameTimeReached, LevelUp,
                        Lavaburn, OnLastWavelet, LevelWon, WishAppears, LevelWonOrLost, WishUsed, TowerSold, UpgradeSkill, Killer, MaxLvlReached, Null};

//lucid dreamer
public enum WishType { Sensible, Null, MoreXP, MoreDamage, MoreDreams, MoreHealth, GlobalForceAttack, PanicAttack };

public enum RewardType { Null, HeroMobility, Modulator, LaserFinisher, RapidFireFinisher, Determined, SparklesFinisher, FearFinisher, CriticalFinisher, Killer, TransformFinisher };

//interactive skill is special skill
//wish is regular (non sensible) wish
//may also have interactive wish later
public enum SelectedType { Toy, Junk, Island, Null, InteractiveSkill, Wish, DirectIsland };

public enum ArrowType {Sensible, Vexing, Explode,  Slow, Fast, Sparkle, Null, Diffuse};

public enum GameState{InGame, Inventory, Won, Lost, MainMenu, Null, WonGame, Loading, LevelList, LoadingWave, Settings, Quit};

public enum MenuButton{Start, Continue, Settings, Inventory, Quit, ToMainMenu, LoadSnapshot, LoadStartLevelSnapshot, ToMap, LoadLatestGame, Rewards, Play};

public enum ToyType { Hero, Normal, Temporary, Null, Building};

//things that can be in a skill tree
//goddamnit people
public enum EffectType{DirectDamage, Force, Speed, Null, Teleport, Mass, Fear, ReloadTime,  Range, Stun, RapidFire, Explode_Force, Laser, Weaken, DOT, Sparkles,
	AiryToy, VexingToy, SensibleGhost, TowerRange, WishCatcher, BaseHealth, Gnomes, Calamity, Sync, Junk,TowerForce, Swarm, AirAttack, Quicksand, VexingForce,EMP,Bees, Critical, Transform
, TimeSpeed, Plague, Diffuse, Focus};//, Bouncy}; 

//skill tree types
public enum RuneType{Sensible, Airy, Vexing, Null, Castle, Modulator, VexingSpire, SensibleCity,Special, Slow, Fast, Time};

public enum LState{WaveButton, WaveStarted, WaveEnded,WaitingToStartNextWave, Lost, Won,OnLastWavelet}; 

//not used? maybe later?
public enum AnimationType{Animate_Sprite_4Dir,Animate_Sprite_8Dir, Animate_3D, None};

public class Central : MonoBehaviour {

    public static Central Instance { get; private set; }

    public int current_savegame_id;
    public float points;
    public int current_lvl = -1; //level we should be on
    public int loaded_level = -1;// what's actually loaded
 
    public int saved_level;

    public float base_toy_cost_increase;
    public float base_toy_cost_mult = 1;

    //   bool load_file;
    //  bool from_snapshot = false; //try to load from a file
    //  bool from_start_level_snapshot; //load from start level snapshot only
    //   string snapshot_file = "";
    //   string start_level_snapshot_file = "";

    private bool levelchange;
    private SaveWhen levelmakers = SaveWhen.Null;
    
    public GameState state = GameState.MainMenu;
    
    public Dictionary<RuneType, string> effect_toys = new Dictionary<RuneType, string>();
    public List<unitStats> actors = new List<unitStats>();
   
    public LevelList level_list;
    public MultiLevelStateSaver game_saver;
    

    public GameObject button_selected;
  //  public SpyGlass my_spyglass;
    public string Diffuse;
    [HideInInspector]
   // private SaveData save_data;
    Queue<string> file;
    public float medium_difficulty = 1.25f;
    public delegate void PriceUpdateHandler(string type, float price);
    public static event PriceUpdateHandler onPriceUpdate;
    public TextAsset init_file;
    public List<ToySaver> hero_toy_stats = new List<ToySaver>(); //used when loading/saving game, also used by special global rune panel


    public int getCurrentLevel() {
        return current_lvl;
    }

    public float getCurrentDifficultyLevel() {
        level_list.levels[current_lvl].difficulty = _transformSliderDifficulty();
        return level_list.levels[current_lvl].difficulty;
    }

    public Rune getHeroRune(RuneType type)
    {
        //Debug.Log("Getting hero rune of type " + type);
        Rune me = null;
        foreach (ToySaver a in hero_toy_stats)
        {
            if (a.rune.runetype == type)
                me = a.rune;
        }

        return me;
    }
/*
    public string getSnapshotFile(bool start_level)
    {
        if (start_level) return start_level_snapshot_file;
        else return snapshot_file;
    }
    */
    float _transformSliderDifficulty()
    {
        float difficulty = 1.25f;
        int d = (int)level_list.difficulty_slider.value;
        switch (d)
        {
            case 1:
                difficulty = 1.15f;
                break;
            case 2:
                difficulty = 1.25f;
                break;
            case 3:
                difficulty = 1.45f;
                break;
            default:
                //     Debug.Log("Unknown difficulty " + d + ", falling back to normal = 1.25\n");
                difficulty = 1.25f;
                break;
        }
        return difficulty; 
    }


    public void SetDifficulty()
    {
        level_list.levels[current_lvl].difficulty = _transformSliderDifficulty();
    }

	public bool setCurrentLevel(int c){
		if (c > level_list.getMaxLvl()) {
			Debug.Log ("TRYING TO SET CURRENT LEVEL TO " + c + " TO BE ABOVE ALLOWED MAXIMUM " + level_list.getMaxLvl() + "\n");
			return false;
		}
        
		if (c < 0){return true;}
		current_lvl = c;
		level_list.levels[current_lvl].difficulty = (int)level_list.difficulty_slider.value;
        Debug.Log("Set level " + current_lvl + " difficulty to " + level_list.levels[current_lvl].difficulty + "\n");
		return true;
	}
	
	public void setLevelInfo(int c){
	//	Debug.Log("Setting level info " + c + " current level " + current_lvl + "\n");
		if (c == -1){
			level_list.info_panel.SetActive(false);
		}else{
			level_list.info_panel.SetActive(true);
					
			for (int i = 0; i < level_list.levels.Count; i++) {
				Level level = level_list.levels [i];
				
				if (level.number == current_lvl){
				//	Debug.Log("Turning on " + level.number + "\n");
					level.description.SetActive(true);
				}else{
					level.description.SetActive(false);
				}			
			}
		}
		

	}

	public int getMaxLevel(){
		return level_list.getMaxLvl();
	}

	public GameState getState(){
		return state;
	}

    public string GetToyFromRune(RuneType r)
    {
        string effect_toy = null;
        effect_toys.TryGetValue(r, out effect_toy);

        return effect_toy;

    }
    /*
	public bool HaveActiveToy(string name){
		unitStats a = null;
		a = getToy(name);
		if (a == null || !a.isActive()) return false;
		return true;
	}
    */
	public void changeState(GameState s){
		changeState (s, "junk");
	}

    public void changeState(GameState s, string content)
    {
        Noisemaker.Instance.Stop();
        GameState oldstate = state;
        state = s;
     //  Debug.Log("Changing state " + oldstate + " to " + s + "\n");

        if (oldstate != GameState.InGame && s == GameState.InGame)
        {
            
            //my_spyglass.DisableByGameState(false);
        }
        else if (oldstate == GameState.InGame && s != GameState.InGame)
        {
            //my_spyglass.Enable(false);
            if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByGameState(true);
          //  softClearLevel();//clear towers
            Moon.Instance.WaveInProgress = false;
        }

        switch (state)
        {
            case GameState.WonGame:                
                game_saver.SaveGame(SaveWhen.EndOfLevel);
                current_lvl = -1;
                
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                break;
            case GameState.Quit:                
                Application.Quit();//for now
                break;
            case GameState.MainMenu:
                
                EagleEyes.Instance.PlaceState(state);
                if (oldstate == GameState.InGame) clearLevel();
                break;
            case GameState.InGame:
                if (Monitor.Instance != null) Monitor.Instance.my_spyglass.Reset();
                if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByGameState(false);

                if (oldstate == GameState.InGame)
                {
          //          Debug.Log("Switched from InGame to InGame, clearing level\n");
                    clearLevel();
                }

                EagleEyes.Instance.PlaceState(state);
                ScoreKeeper.Instance.SetLevel(current_lvl, getCurrentDifficultyLevel());
                if (EventOverseer.Instance != null) EventOverseer.Instance.StartMe(true); else { Debug.Log("No overseer to start:("); }
                if (RewardOverseer.RewardInstance != null) RewardOverseer.RewardInstance.StartMe(true); else { Debug.Log("No overseer to start:("); }
                break;
            case GameState.Won:                
                game_saver.SaveGame(SaveWhen.EndOfLevel);
                clearLevel();
                incrementLevel();
                Peripheral.Instance.Wave_interval = 0f;
                GameStatCollector.Instance.printStats();

                EagleEyes.Instance.PlaceState(state);
                break;

            case GameState.LevelList:
// softClearLevel();
                if (saved_level == -1 && content == "AskConfirm") content = "Start";

                switch (content)
                {
                    case "AskConfirm"://from mainmenu to map			
                        EagleEyes.Instance.DisplayConfirmPanel(MenuButton.Start, true);
                        break;
                    case "CancelConfirm":
                        EagleEyes.Instance.DisplayConfirmPanel(MenuButton.Start, false);
                        break;
                    case "AskConfirmToMap":// from ingame to map			    
                        EagleEyes.Instance.DisplayConfirmPanel(MenuButton.ToMap, true);
                        break;
                    case "CancelConfirmToMap":
                        EagleEyes.Instance.DisplayConfirmPanel(MenuButton.ToMap, false);
                        break;
                    case "ToMap": //lose progress
                        softClearLevel();
                        clearLevel(); //does this need to be here?		

                        Central.Instance.game_saver.LoadGame(SaveWhen.BetweenLevels);
                        if (EventOverseer.Instance != null) { EventOverseer.Instance.StopMe(); }
                        EagleEyes.Instance.PlaceState(state);
                        //if (EventOverseer.Instance != null) EventOverseer.Instance.StartMe(false);
                        if (RewardOverseer.RewardInstance != null) RewardOverseer.RewardInstance.StartMe(false);
                        break;
                    case "Start":
                        Debug.Log("Start...\n");              
                        RewardOverseer.RewardInstance.StartMe(false);
                        EagleEyes.Instance.PlaceState(state);
                        break;
                    default:
                        Debug.Log("UNKNOWN content " + content + " for GameState LevelList\n");
                        state = oldstate;
                        break;
                }
                break;
            case GameState.Lost:
                EagleEyes.Instance.PlaceState(state);
                softClearLevel();//clear towers
                clearLevel();//clear ui
                GameStatCollector.Instance.printStats();
                GameStatCollector.Instance.SaveStatFile();
                break;
            case GameState.Loading:
                softClearLevel();//clear towers
                switch (content)
                {

                    case "LoadSnapshot": //restart wave button
                        EagleEyes.Instance.PlaceState(state);
                        if (oldstate == GameState.InGame) softClearLevel();                        
                        levelchange = true;
                        levelmakers = SaveWhen.MidLevel;
                        break;
                    case "NewLevel":
                        EagleEyes.Instance.PlaceState(state);                        
                        levelchange = true;
                        levelmakers = SaveWhen.BeginningOfLevel;

                        break;
                    case "LoadStartLevelSnapshot": //restart level button
                        EagleEyes.Instance.PlaceState(state);
                        if (EventOverseer.Instance != null) { EventOverseer.Instance.StopMe(); }
                        if (oldstate == GameState.InGame) softClearLevel();
                        levelchange = true;                        
                        levelmakers = SaveWhen.BeginningOfLevel;                        
                        break;
                    default:
                        EagleEyes.Instance.PlaceState(state);                        
                        Debug.Log("UNKNOWN content for GameState Loading, button " + this.name + "\n");
                        state = oldstate;
                        break;
                }
                break;
            case GameState.LoadingWave:
                Debug.Log("When does this happen?\n");
                EagleEyes.Instance.PlaceState(state);
                if (oldstate == GameState.InGame) softClearLevel();                
                levelchange = true;
                levelmakers = SaveWhen.MidLevel;
                break;
            default:
                break;
        }
    }
	

	void incrementLevel(){
		if (current_lvl > level_list.getMaxLvl()) {
			return;
		}
		if (current_lvl == level_list.getMaxLvl()) level_list.setMaxLvl(current_lvl+1);
		current_lvl++;
		
	//	Debug.Log("Incremented Level to " + current_lvl + "\n");
		//if (max_lvl > level_list.levels.Count) {
//			Debug.Log ("You won the game!");
		//	changeState (GameState.WonGame);
	//	}
	}
	


	public unitStats getToy(string name){
		foreach (unitStats actor in actors){
			if (actor.name.Equals(name)) return actor;
		}
		return null;
	}

    public void setUnitStats(unitStats toy, bool set_max_lvl)
    {
        unitStats current_toy = getToy(toy.name);
     //   toy.setActive(toy.toy_type != ToyType.Temporary);            

        if (current_toy != null)
        {         
            current_toy = toy;
            setUnitStatsSaver(ref current_toy, toy.getSaver(), set_max_lvl);                 
            return;
        }
        actors.Add(toy);
    }

    public void setUnitStats(unitStatsSaver toy, bool set_max_lvl)
    {
        unitStats current_toy = getToy(toy.name);
       

        if (current_toy != null)
        {
            
            setUnitStatsSaver(ref current_toy, toy, set_max_lvl);
            return;
        }else
        {
            Debug.LogError("Could not set actor stats from actorStatsSaver for toy " + toy.name + "\n");
        }
        
    }

    //only for use by the above 2
    void setUnitStatsSaver(ref unitStats current_toy, unitStatsSaver toy, bool set_max_lvl)
    {
        int old_max_lvl = current_toy.getMaxLvl();
        if (set_max_lvl)
        {// I HATE THIS SHIT
            if (toy.toy_type == ToyType.Hero)
            {
                int new_max_lvl = Mathf.Max(toy.getMaxLvl(), old_max_lvl);

                foreach (ToySaver h in hero_toy_stats)
                {
                    if (h.toy_name.Equals(toy.name))
                    {
                        new_max_lvl = Mathf.Max(toy.getMaxLvl(), (int)h.rune.getMaxLevel());
                        h.rune.setMaxLevel(new_max_lvl);
                    }
                }
                current_toy.setMaxLvl(new_max_lvl);

            }
            else
            {
                current_toy.setMaxLvl(toy.getMaxLvl());
            }
        }
        else current_toy.setMaxLvl(old_max_lvl);
    }

    public void LockAllToys()
    {
        foreach (unitStats a in actors) if (a.friendly) a.isUnlocked = false;
        
    }

    public void updateCost(int toys){
	//	Debug.Log("UPDATING COST " + toys + "\n");	
		
		if (base_toy_cost_mult <= 0) base_toy_cost_mult = 1f;
		
		foreach (unitStats actor in actors) {
			if (actor.friendly == true && actor.ammo == -1 && !actor.cost_type.isHero()){
                float diff = actor.cost_type.Amount;
                //Debug.Log("From " + actor.cost_type.cost + " to " + (actor.init_cost + (toys * 2) * base_toy_cost_increase) * base_toy_cost_mult);
                //actor.cost_type.cost = Mathf.FloorToInt((actor.init_cost + (toys*2)*base_toy_cost_increase)*base_toy_cost_mult);
                actor.cost_type.Amount = Mathf.FloorToInt(actor.init_cost * (1 + toys * base_toy_cost_increase));
                diff = actor.cost_type.Amount - diff;
                //old base toy cost mult = 3

                //Debug.Log("Updating cost for " + actor.name + " init_cost " + actor.init_cost + " toys " + toys + " base_toy_cost_mult " + base_toy_cost_mult + " base_toy_cost_increase " + base_toy_cost_increase + " DIFF " + diff);
				if (onPriceUpdate != null){onPriceUpdate(actor.name, actor.cost_type.Amount);}
				
			}
		}
		EagleEyes.Instance.UpdateToyButtons ("blah", ToyType.Normal, false);
        EagleEyes.Instance.UpdateToyButtons("blah", ToyType.Building, false);
    }
	
	
	void onWaveStart(int content)
	{
		//Debug.Log("Central received onWaveStart\n");
		if (content == 0 || ((content+1) % 3 == 0 && (Moon.Instance.waves.Count - content > 2)))
            //game_saver.SaveGame(true, current_lvl.ToString());
            game_saver.SaveGame(SaveWhen.MidLevel);

        saved_level = current_lvl;
	}
	
	
	IEnumerator MakeLevel(){
		float blip = .1f;
        
        if (Peripheral.Instance != null){ Peripheral.Instance.StopAllCoroutines();}
		if (EventOverseer.Instance != null){EventOverseer.Instance.StopMe();}
		loaded_level = current_lvl;
        string gonna_load_this_level_now = level_list.levels[current_lvl].name + "_scene";
     //   Debug.Log(gonna_load_this_level_now + " cuz " + current_lvl + "\n");
        AsyncOperation loadlevel = Application.LoadLevelAsync (gonna_load_this_level_now);
        while (loadlevel.progress < 0.9f)
        {
            yield return new WaitForSeconds(blip);
        }

        loadlevel.allowSceneActivation = true;
        //	Debug.Log("CHANGED SCENE\n");
        updateCost(Peripheral.Instance.getToys());
        EagleEyes.Instance.UpdateToyButtons ("blah",ToyType.Null, false);
		Peripheral.onWaveStart += onWaveStart;		
		
		yield return null;
	}

	public void LateUpdate(){
		if (levelchange && levelmakers != SaveWhen.Null) {
            ScoreKeeper.Instance.Reset();
            Debug.Log("Levelmakers levelchange type " + levelmakers + " current level " + current_lvl + "\n");
            
            levelchange = false;

            string from_file = "";
            int lets_make_level = -1;
            if (levelmakers == SaveWhen.BeginningOfLevel)
            {
                lets_make_level = current_lvl;
            }

            if (levelmakers == SaveWhen.MidLevel)
            {
                lets_make_level = game_saver.getCurrentLevel();                
            }

            if (lets_make_level >= 0)
            {

                if (loaded_level != lets_make_level || Peripheral.Instance.TIME == 0) //TIME is -1 is a proxy for beginning of game, no level loaded yet
                {
                    //need to load scene                    
                    Debug.Log("Loading level " + current_lvl + " from savegame " + levelmakers + "\n");
                    current_lvl = lets_make_level;
                    StartCoroutine("MakeLevel");
                    return;
                } else
                {
                    
                    Debug.Log("Level " + current_lvl + " is already loaded, loading savegame " + levelmakers + "\n");
                    current_lvl = lets_make_level;

                    if (levelmakers == SaveWhen.BeginningOfLevel)
                    {//redo all of the level initialization
                        OnLevelWasLoaded();
                    }
                    else
                    {                     
                        game_saver.LoadGame(levelmakers);
                    }
                }

            }
            else
            {
                Debug.LogError("Don't know what level to load\n");
            }
        
		}	
	}




	void OnLevelWasLoaded(){
        //event triggered by Application.LoadLevelAsync succcess
	//	Debug.Log("ONLEVELWAS LOADED CURRENT LEVEL IS " + current_lvl + "\n");

        bool ok = Peripheral.Instance.Load(level_list.levels[current_lvl]);
        
		
		if (ok) {
            game_saver.LoadGame(levelmakers);

            if (levelmakers == SaveWhen.BeginningOfLevel)
            { 
			//	Debug.Log("Loaded not from file\n");
				Sun.Instance.SetTime(0);
				Sun.Instance.Init();
                Peripheral.Instance.PlaceCastle();                
				changeState(GameState.InGame);      
    //            game_saver.SaveGame(SaveWhen.BeginningOfLevel);
                Peripheral.Instance.Pause(false);
            }
		} else {
			Debug.Log("Could not load level " + level_list.levels [current_lvl] + "\n");

            changeState(GameState.WonGame);
		}		
	}


    //delete towers
    public void softClearLevel()
    {
        Debug.Log("Doing a soft clear\n"); 
        //if (current_lvl == 0) return;
        if (Monitor.Instance == null) return;
        foreach (Island_Button island in Monitor.Instance.islands.Values){
			island.blocked = false;
			island.my_toy = null;
        }
        Peripheral.Instance.zoo.returnAll();
    }

	public void clearLevel(){
		//Debug.Log ("CLEAR LEVEL\n");
		EagleEyes.Instance.ClearGUI (state);
	
	}
	

	void Awake()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
	
		if(Instance != null && Instance != this)
		{
		//	Debug.Log("Remaking Central\n");
			Destroy(gameObject);
		}
		Instance = this;
		DontDestroyOnLoad (gameObject);
       
    }

 

	void Start(){
        
        
        file = new Queue<string>(init_file.text.Split('\n'));
		Loader.Instance.setFile (file);
		Loader.Instance.LoadInitFile ();
	}
    
    //hero has to be placed on the map at least once before its stats are added to hero_toy_stats
    public ToySaver getHeroStats(RuneType type)
    {
        ToySaver me = null;
        foreach (ToySaver toy in hero_toy_stats)
        {
            if (toy.rune.runetype == type) me = toy;

        }

        return me;
    }

    public List<ToySaver> getAllHeroStats()
    {

        return hero_toy_stats;
    }

    


   

}

			
			
public class Label{
	public string title;
	public string description;
	public string proper_name;

	public Label(string n){
		title = n;
		description = n + " is brilliant, really. Does need a description though.";
		proper_name = n.ToUpperInvariant ();
	}

	public Label(string n, string d, string pn){
		title = n;
		description = d;
		proper_name = pn;
	}
}


public class State{
	public Dictionary<string,State> subMenu;
	public string name;
	public GameObject background;

	public void Init(string n, GameObject bg){
		name = n;
		background = bg;
		
	}

}