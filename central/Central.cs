using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Amazon;
using Fabric.Crashlytics;

//DO NOT CHANGE THIS ORDER OR THE PREFABS GET MESSED UP DUH
//for UI button filter
public enum MarketplaceID { Dreams, ScorePoint, Sensible, MoreXP, MoreDamage, MoreHealth, MoreDreams}
public enum EffectSubType {  Null, Junk, Freeze, Ultra }
public enum PlayerEvent { Null, AmmoRecharge, TowerBuilt, SpecialSkillUsed, UsedInventory,SkillUpgrade,LoadGame, CastleReached,FinishedLevel,NightTowerHit,StartGame, StartWaveEarly, TimeScaleChange,SellTower, ResetSkills, Error, XP, WaveTiming, MarketPlace }
public enum UIButtonType {  RewardButton, Fade}
public enum LabelText { Null, DPS, Damage, PCT, Range, Recharge, Duration, Attacks }
public enum EnvType { Default , Forest, Desert, Urban, ForestGreen}
public enum TransformType { Null, Toad, StickFigure, FruitFly, Whale }
public enum TimeScale { Null, Normal, Fast, SuperFastPress, Pause, Resume }
public enum SaveWhen { Null, MidLevel, EndOfLevel, BetweenLevels, BeginningOfLevel, GoBack1, GoBack2 }
public enum SaveStateType { Null, Persistent, MidLevel }

public enum LeanTweenerPreset { Null, DefaultFastButton, DefaultSlowButton, UIBlip, UI3Blips, GentleSlow,DefaultSlowButtonAlpha, GentleSlowAlpha }

public enum StateType { Yes, No, NoResources, WrongType  }    

public enum ClickType { Null, Success, Error, Action, Cancel }

public enum UpdateType { ScoreUpdate, DreamUpdate, WishUpdate}

public enum LabelUnit { Null, Percent, Duration, Damage, Distance, Bullets, DPS, Recharge, Victims, DamagePercent, Regen}

public enum LabelName { Null, SkillStrength, TimeRemaining, CurrentLvlSkillDesc, NextLvlSkillDesc, UpgradeCost, Level, Bullets, Range, Name, Requirement, Reward }

public enum CostType { Dreams, Wishes, SensibleHeroPoint, AiryHeroPoint, VexingHeroPoint, SensibleCityHeroPoint, ScorePoint};


public enum Condition { TIME, Click, Selected, PlacedToy, WaveStarted, WaveEnded, GotWish, TIMEInterval, ClickedOnToy, GameTimeReached, CanUpgrade,
                        Lavaburn, OnLastWavelet, LevelWon, WishAppears, LevelWonOrLost, WishUsed, TowerSold, UpgradeSkill, Killer, MaxLvlReached, Null, HaveCity, Resume};

public enum WishType { Sensible, Null, MoreXP, MoreDamage, MoreDreams, MoreHealth, GlobalForceAttack, PanicAttack };

public enum RewardType { Null, HeroMobility, Modulator, LaserFinisher, RapidFireFinisher, Determined, SparklesFinisher, FearFinisher, CriticalFinisher, Killer, TransformFinisher };

public enum Difficulty { Null, Normal, Hard, Insane}

//interactive skill is special skill
//wish is regular (non sensible) wish
//may also have interactive wish later
public enum SelectedType { Toy, Junk, Island, Null, InteractiveSkill, Wish, DirectIsland, Enemy };

public enum ArrowType {Sensible, Vexing, Explode,  Slow, Fast, Sparkle, Null, Diffuse, Focus, RapidFire, Critical};

public enum GameState{InGame, Inventory, Won, Lost, MainMenu, Null, WonGame, Loading, LevelList, LoadingWave, Settings, Quit};

public enum MenuButton{Start, Continue, Settings, Inventory, Quit, ToMainMenu, LoadSnapshot, LoadStartLevelSnapshot, ToMap, LoadLatestGame, Rewards, Play, GoBack1, GoBack2, Marketplace};

public enum ToyType { Hero, Normal, Temporary, Null};

//things that can be in a skill tree
//goddamnit people
public enum EffectType{DirectDamage, Force, Speed, Null, Teleport, Mass, Fear, ReloadTime,  Range, Stun, RapidFire, Explode_Force, Laser, Weaken, DOT, Sparkles,
	AiryToy, VexingToy, SensibleGhost, TowerRange, WishCatcher, Renew, Architect, Calamity, Sync, Magic ,TowerForce, Swarm, AirAttack, Frost, VexingForce,EMP,Bees, Critical, Transform
, Junk1, Plague, Diffuse, Focus, Meteor, Junk, Foil};//, Bouncy}; 

//skill tree types
public enum RuneType{Sensible, Airy, Vexing, Null, Castle, Modulator, VexingSpire, SensibleCity,Special, Slow, Fast, Time};

public enum LState{WaveButton, WaveStarted, WaveEnded,WaitingToStartNextWave, Lost, Won,OnLastWavelet}; 

//not used? maybe later?
public enum AnimationType{Animate_Sprite_4Dir,Animate_Sprite_8Dir, Animate_3D, Animate_Vehicle_4Dir, None};



public class Central : MonoBehaviour {

    public static Central Instance { get; private set; }

    public SaveWhen Levelmakers
    {
        get
        {
            return levelmakers;
        }

        set
        {
            Debug.Log("!!!!!!Levelmakers set to " + value + "\n");
            levelmakers = value;
        }
    }
    public AWSManager myAWSManager;
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
    public List<enemyStats> enemies = new List<enemyStats>();

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
    public List<Rune> hero_toy_stats = new List<Rune>(); //used when loading/saving game, also used by special global rune panel


    public int getCurrentLevel() {
        return current_lvl;
    }

    public Difficulty getCurrentDifficultyLevel() {
        //level_list.SetDifficulty();
        return level_list.levels[current_lvl].difficulty;
    }

    

    public Rune getHeroRune(RuneType type)
    {
        //Debug.Log("Getting hero rune of type " + type);
        Rune me = null;
        foreach (Rune a in hero_toy_stats)
        {
            if (a.runetype == type)
                me = a;
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
  


    

	public bool setCurrentLevel(int c){
		if (c > level_list.getMaxLvl()) {
			Debug.Log ("TRYING TO SET CURRENT LEVEL TO " + c + " TO BE ABOVE ALLOWED MAXIMUM " + level_list.getMaxLvl() + "\n");
			return false;
		}
        
		if (c < 0){return true;}
		current_lvl = c;
        level_list.SetDifficulty();
        //level_list.levels[current_lvl].difficulty = _transformSliderDifficulty();//_getlevel_list.difficulty_slider.value;
      //  Debug.Log("Set level " + current_lvl + " difficulty to " + level_list.levels[current_lvl].difficulty + "\n");
		return true;
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
       Debug.Log("Changing state " + oldstate + " to " + s + "\n");

        if (oldstate != GameState.InGame && s == GameState.InGame)
        {            
            //my_spyglass.DisableByGameState(false);c
        }
        else if (oldstate == GameState.InGame && s != GameState.InGame)
        {          
            if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByGameState(true);
          
            Moon.Instance.WaveInProgress = false;
            Peripheral.Instance.ClearAll(SelectedType.Null, "");
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

                if (oldstate == GameState.InGame) clearLevel();

                EagleEyes.Instance.PlaceState(state);
                ScoreKeeper.Instance.SetLevel(current_lvl, getCurrentDifficultyLevel());
                if (EventOverseer.Instance != null) EventOverseer.Instance.StartOverseer(); else {
                    //Debug.Log("No overseer to start:(");
                }
                EagleEyes.Instance.ResetHealthVisual();
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

                //if (saved_level == -1 && content.Equals("AskConfirm")) content = "Start";

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
                        if (EventOverseer.Instance != null) { EventOverseer.Instance.StopOverseer(); }
                        //if (RewardOverseer.RewardInstance != null) RewardOverseer.RewardInstance.StartMe(false);

                        EagleEyes.Instance.PlaceState(state);                                                
                        break;
                    case "Start":
                        Debug.Log("Start...\n");              
                   //     RewardOverseer.RewardInstance.StartMe(false);
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
                
                break;
            case GameState.Loading:
                softClearLevel();//clear towers
                switch (content)
                {

                    case "LoadSnapshot": //restart wave button
                        EagleEyes.Instance.PlaceState(state);
                        if (oldstate == GameState.InGame) softClearLevel();
                        if (EventOverseer.Instance != null) { EventOverseer.Instance.StopOverseer(); }
                        levelchange = true;
                        Levelmakers = SaveWhen.MidLevel;
                        
                        break;
                    case "GoBack1": //restart wave button
                        EagleEyes.Instance.PlaceState(state);
                        if (oldstate == GameState.InGame) softClearLevel();
                        if (EventOverseer.Instance != null) { EventOverseer.Instance.StopOverseer(); }
                        levelchange = true;
                        Levelmakers = SaveWhen.GoBack1;

                        break;
                    case "GoBack2": //restart wave button
                        EagleEyes.Instance.PlaceState(state);
                        if (oldstate == GameState.InGame) softClearLevel();
                        if (EventOverseer.Instance != null) { EventOverseer.Instance.StopOverseer(); }
                        levelchange = true;
                        Levelmakers = SaveWhen.GoBack2;

                        break;


                    case "NewLevel":

                        EagleEyes.Instance.PlaceState(state);                        
                        levelchange = true;
                        Levelmakers = SaveWhen.BeginningOfLevel;

                        break;
                    case "LoadStartLevelSnapshot": //restart level button
                        EagleEyes.Instance.PlaceState(state);
                        if (EventOverseer.Instance != null) { EventOverseer.Instance.StopOverseer(); }
                        if (oldstate == GameState.InGame) softClearLevel();
                        levelchange = true;                        
                        Levelmakers = SaveWhen.BeginningOfLevel;                        
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
                Levelmakers = SaveWhen.MidLevel;
                break;
            default:
                break;
        }
    }
	

	void incrementLevel(){
		if (current_lvl > level_list.getActualMaxLvl()) {
			return;
		}
        if (current_lvl == level_list.getActualMaxLvl())
        {
            level_list.setMaxLvl(current_lvl + 1);
            current_lvl++;
        }
		

	}

    public unitStats getToy(RuneType rt, ToyType tt)
    {
        foreach (unitStats actor in actors)
        {
            if (actor.CompareTo(rt, tt) == 0) return actor;
        }
        return null;
    }

    public unitStats getToy(unitStats toy){
		foreach (unitStats actor in actors){
			if (actor.CompareTo(toy) == 0) return actor;
		}
		return null;
	}

    public unitStats getToy(string name)
    {
        foreach (unitStats actor in actors)
        {
            if (actor.name.Equals(name)) return actor;
        }

    //    Debug.LogError("Central cannot find toy by name " + name + "\n");

        return null;
    }

    public enemyStats getEnemy(string name)
    {
        foreach (enemyStats enemy in enemies)
        {
            if (enemy.name.Equals(name)) return enemy;
        }
        Debug.LogError("Central cannot find enemy by name " + name + "\n");

        return null;
    }

    public enemyStats getEnemy(EnemyType  type)
    {
        foreach (enemyStats enemy in enemies)
        {
            if (enemy.getEnemyType() == type) return enemy;
        }
        Debug.LogError("Central cannot find enemy by type " + type + "\n");

        return null;
    }

    public void setUnitStats(unitStats toy, bool set_max_lvl)
    {
        unitStats current_toy = getToy(toy);
     //   toy.setActive(toy.toy_type != ToyType.Temporary);            

        if (current_toy != null)
        {         
            current_toy = toy;
            setUnitStatsSaver(ref current_toy, toy.getSnapshot(), set_max_lvl);                 
            return;
        }
        actors.Add(toy);
    }

    public void setUnitStats(unitStatsSaver toy, bool set_max_lvl)
    {
        unitStats current_toy = getToy(toy.toy_id.rune_type, toy.toy_id.toy_type);
       

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
            if (toy.toy_id.toy_type == ToyType.Hero)
            {
                int new_max_lvl = Mathf.Max(toy.getMaxLvl(), old_max_lvl);

                foreach (Rune h in hero_toy_stats)
                {
                    //if (h.toy_name.Equals(toy.name))
                    if (h.runetype == toy.toy_id.rune_type && h.toy_type == toy.toy_id.toy_type)
                    {
                        new_max_lvl = Mathf.Max(toy.getMaxLvl(), (int)h.getMaxLevel());
                        h.setMaxLevel(new_max_lvl);
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
        foreach (unitStats a in actors)  a.isUnlocked = false;
        
    }

    public void updateCost(int toys){
	//	Debug.Log("UPDATING COST " + toys + "\n");	
		
		//if (base_toy_cost_mult <= 0) base_toy_cost_mult = 1f;
		
		foreach (unitStats actor in actors) {
			if (actor.ammo == -1 && !actor.cost_type.isHero()){
                //float diff = actor.cost_type.Amount;
			    actor.cost_type.Amount = Mathf.FloorToInt(actor.init_cost * base_toy_cost_mult);
                //actor.cost_type.Amount = Mathf.FloorToInt(actor.init_cost * (1 + toys * base_toy_cost_increase));
                //diff = actor.cost_type.Amount - diff;
                //old base toy cost mult = 3

                if (onPriceUpdate != null){onPriceUpdate(actor.name, actor.cost_type.Amount);}
				
			}
		}
		EagleEyes.Instance.UpdateToyButtons ("blah", ToyType.Normal, false);
        
    }
	
	
    public void SaveCurrentGame()
    {
        game_saver.SaveGame(SaveWhen.MidLevel);
        saved_level = current_lvl;
    }
	
	
	IEnumerator MakeLevel(){
		float blip = .1f;
        
        if (Peripheral.Instance != null){ Peripheral.Instance.StopAllCoroutines();}
		if (EventOverseer.Instance != null){EventOverseer.Instance.StopOverseer();}
		loaded_level = current_lvl;
        string gonna_load_this_level_now = level_list.levels[current_lvl].name + "_scene";
      //  Debug.Log(gonna_load_this_level_now + " cuz " + current_lvl + "\n");
        AsyncOperation loadlevel = Application.LoadLevelAsync (gonna_load_this_level_now);
        while (loadlevel.progress < 0.9f)
        {
            yield return new WaitForSeconds(blip);
        }

        loadlevel.allowSceneActivation = true;
        //	Debug.Log("CHANGED SCENE\n");
        updateCost(Peripheral.Instance.getToys());
        EagleEyes.Instance.UpdateToyButtons ("blah",ToyType.Null, false);
		//Peripheral.onWaveStart += onWaveStart;		
		
		yield return null;
	}

	public void LateUpdate(){
		if (levelchange && Levelmakers != SaveWhen.Null) {
            
          //  Debug.Log("Levelmakers levelchange type " + Levelmakers + " current level " + current_lvl + "\n");
            
            levelchange = false;

            
            int lets_make_level = -1;
            if (Levelmakers == SaveWhen.BeginningOfLevel)
            {
                lets_make_level = current_lvl;
            }

            if (Levelmakers == SaveWhen.MidLevel || Levelmakers == SaveWhen.GoBack1 || Levelmakers == SaveWhen.GoBack2)
            {
                lets_make_level = game_saver.getCurrentLevel();                
            }

            if (lets_make_level >= 0)
            {

                if (loaded_level != lets_make_level || Peripheral.Instance.TIME == 0) //TIME is -1 is a proxy for beginning of game, no level loaded yet
                {
                    //need to load scene                    
                    Debug.Log("Loading level " + current_lvl + " from savegame " + Levelmakers + "\n");
                    current_lvl = lets_make_level;
                    StartCoroutine("MakeLevel");
                    return;
                } else
                {
                    
              //      Debug.Log("Level " + current_lvl + " is already loaded, loading savegame " + Levelmakers + "\n");
                    current_lvl = lets_make_level;

                    if (Levelmakers == SaveWhen.BeginningOfLevel)
                    {//redo all of the level initialization
                        OnLevelWasLoaded();
                    }
                    else
                    {                     
                        game_saver.LoadGame(Levelmakers);

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
            game_saver.LoadGame(Levelmakers);

            if (Levelmakers == SaveWhen.BeginningOfLevel)
            { 
			//	Debug.Log("Loaded not from file\n");
				Sun.Instance.SetTimePassively(0);
				Sun.Instance.Init();
                Peripheral.Instance.PlaceCastle();
                ScoreKeeper.Instance.ResetMidLevelScore();              
				changeState(GameState.InGame);          
                Peripheral.Instance.ChangeTime(TimeScale.Normal);
            }
		} else {
			Debug.Log("Could not load level " + level_list.levels [current_lvl] + "\n");

            changeState(GameState.WonGame);
		}		
	}


    //delete towers
    public void softClearLevel()
    {
     //   Debug.Log("Doing a soft clear\n"); 
        //if (current_lvl == 0) return;
        if (Monitor.Instance == null) return;
        foreach (Island_Button island in Monitor.Instance.islands.Values){
			island.blocked = false;
			island.my_toy = null;
        }
        Peripheral.Instance.my_skillmaster.resetSkills();
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
     //   Fabric.Runtime.Fabric.Initialize();
        Crashlytics.SetDebugMode(true);
        TowerStore.initTowers();
        EnemyStore.initEnemies();
        LevelStore.initSettings();
        
        //file = new Queue<string>(init_file.text.Split('\n'));
		//Loader.Instance.setFile (file);
		//Loader.Instance.LoadInitFile ();
	}
    
    //hero has to be placed on the map at least once before its stats are added to hero_toy_stats
    public Rune getHeroStats(RuneType type)
    {
        Rune me = null;
        foreach (Rune toy in hero_toy_stats)
        {
            if (toy.runetype == type) me = toy;

        }

        return me;
    }

    public List<RuneSaver> getAllHeroStats()
    {
        List<RuneSaver> savers = new List<RuneSaver>();
        foreach (Rune r in hero_toy_stats)
        {
            savers.Add(r.getSnapshot());
        }
        return savers;
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