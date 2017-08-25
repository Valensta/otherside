using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SaveState: System.Object {
    public SaveStateType type; 
    public List<IslandSaver> islands = new List<IslandSaver>();
    public float dreams;
    public float health;    
    public List<ScoreDetails> score_details = new List<ScoreDetails>();
    public int total_score;
    public List<WishDial> possible_wishes;
    public float possible_dreams;
    public Difficulty difficulty;
    public float score_time;
    public int current_wave;
    public int current_event;        //level-specific eventoverseer
    public int current_global_event; //rewardoverseer
    
	public List<unitStatsSaver> actor_stats = new List<unitStatsSaver> ();
	public List<RuneSaver> hero_stats = new List<RuneSaver>();
    public List<SpecialSkillSaver> skills_in_inventory = new List<SpecialSkillSaver>();
	
    
    public List<string> concurrent_events = new List<string>();
    public List<Wish> wishes = new List<Wish>();
    public List<Reward> rewards = new List<Reward>();
    
    public int current_level;

    public FakePlayerSaver fakeplayer_saver = new FakePlayerSaver();

    public SaveState(){}
   

public void resetMidLevelStuff()
    {
        if (type == SaveStateType.Persistent)
        {
            Debug.Log("Trying to reset mid level stuff for SaveStateType Persistent. WHAT ARE YOU DOING.\n");
            return;
        }

        islands = null;
        dreams = 0;
        health = 1;
        score_details = null;
        total_score = 0;
        possible_wishes = new List<WishDial>(); 
        possible_dreams = 0; 
        current_level = -1;
        current_wave = 0;
        current_event = 0;
        //current_event_is_ingame = false;
        concurrent_events = null;
      //  temporary_effects = null;


        return;
        
    }

  
    public SaveState(float _dreams, float _health, int _current_wave, float _sens, float _airy, float _vex, int _sens_hero, int _airy_hero, int _vex_hero, Difficulty _difficulty)
    {
		
        dreams = _dreams;
        health = _health;
     //   sensible_wish = _sens;
        //airy_wish = _airy;
//        vexing_wish = _vex;
        current_wave = _current_wave;
      //  sensible_hero_point = _sens_hero;
//        airy_hero_point = _airy_hero;
        //vexing_hero_point = _vex_hero;
        difficulty = _difficulty;

    }

  
    public bool isValid()
    {
        return health > 0 && (current_wave > 0 || LevelBalancer.Instance.am_enabled || Central.Instance.level_list.levels[current_level].test_mode);
    }

    public void SaveHeroStats()
    {
        hero_stats = Central.Instance.getAllHeroStats();
        
    }

    public void SaveSkillInventory()
    {
        //wishes = Peripheral.Instance.my_inventory.getWishList();
        skills_in_inventory = Peripheral.Instance.my_skillmaster.getInventory();
        
    }

    public void SaveIslands()
    {
        islands = new List<IslandSaver>();
        foreach (Island_Button island in Monitor.Instance.islands.Values)
        {
            IslandSaver saver = island.getSnapshot();
            if (saver == null) continue;
            islands.Add(saver);
        }
    }

    public RuneSaver getCastle()
    {
        foreach (RuneSaver t in hero_stats) if (t.runetype == RuneType.Castle) return t;
        return null;
    }

    public void LoadHeroStats()
    {        
        List<Rune> hero_toy_stats = new List<Rune>();
        foreach (RuneSaver saveme in hero_stats)
        {
            Rune new_rune = new Rune();
            new_rune.loadSnapshot(saveme);
            new_rune.LoadSpecialSkills();
            /*
            for (int i = 0; i < hero_toy_stats.Count; i++)
            {
                if (hero_toy_stats[i].runetype == new_rune.runetype)
                {
                    hero_toy_stats[i] = new_rune;
                    hero_toy_stats[i].LoadSpecialSkills();
                    return;
                }
            }*/
            hero_toy_stats.Add(new_rune);
            //hero_toy_stats[hero_toy_stats.Count - 1].LoadSpecialSkills();

        }
        Central.Instance.hero_toy_stats = hero_toy_stats;
    }

    public void SaveToyStats(){

        actor_stats = new List<unitStatsSaver>();
		foreach (unitStats a in Central.Instance.actors)   actor_stats.Add (a.getSnapshot());

        
    }

    public void LoadToyStats(){
        if (actor_stats.Count == 0)
        {
            Central.Instance.LockAllToys();
            foreach (unitStats a in Central.Instance.actors)
            {
                a.isUnlocked = StaticStat.isUnlocked(a.toy_id.rune_type, a.toy_id.toy_type == ToyType.Hero, a.isUnlocked);
            }
            return; //new game
        }

        Central.Instance.LockAllToys();
        foreach (unitStatsSaver a in actor_stats) Central.Instance.getToy(a.name).loadSnapshot(a);
        foreach (unitStats a in Central.Instance.actors) a.isUnlocked = StaticStat.isUnlocked(a.toy_id.rune_type, a.toy_id.toy_type == ToyType.Hero, a.isUnlocked);

    }




    public void SaveBasicMidLevelShit(){

    //    Debug.Log("Savig basic midlevel shit\n");
        dreams = Peripheral.Instance.dreams;
        health = Peripheral.Instance.GetHealth();
        current_wave = Moon.Instance.GetCurrentWave();
        difficulty = Peripheral.Instance.difficulty;
        //SaveState saver = new SaveState(dreams, health, current_wave, 0, 0, 0, sens_h, airy_h, vex_h, difficulty);
        current_level = Central.Instance.current_lvl;
     
    }

    public void LoadWishes()
    {
        
        Peripheral.Instance.my_inventory.setWishList(wishes);
    }

    public void SaveWishes()
    {
        wishes = Peripheral.Instance.my_inventory.getWishList();
    }

    public void LoadScore(bool resetMidLevel)
    {
        if (ScoreKeeper.Instance == null)
        {
            Debug.Log("Cannot load score, ScoreKeeper is null\n");
            return;
        }
       
        ScoreKeeper.Instance.setScoreDetails(score_details);
        ScoreKeeper.Instance.SetTotalScore(total_score);

        if (!resetMidLevel && type == SaveStateType.MidLevel)
        {
            ScoreKeeper.Instance.setLevelTime(score_time);
            ScoreKeeper.Instance.setPossibleDreams(possible_dreams);
            ScoreKeeper.Instance.setPossibleWishes(possible_wishes);
        }
        if (resetMidLevel) ScoreKeeper.Instance.ResetMidLevelScore();
                
    }

    public void LoadSkillsInventory(bool reset_remaining_time)
    {

        Peripheral.Instance.my_skillmaster.resetInventory();

        foreach (SpecialSkillSaver t in skills_in_inventory)
        {
       //     Debug.Log("Loading skill " + t.type + "\n");
            if (reset_remaining_time) t.remaining_time = 0;
            Peripheral.Instance.my_skillmaster.setInventory(t, true);
        }
    }

    public void SaveRewards()
    {
        rewards = RewardOverseer.RewardInstance.getRewards();


        current_global_event = RewardOverseer.RewardInstance.current_event;

    }

    public virtual void LoadRewards()
    {
     
        // all rewards are saved, regardless of status
        foreach (Reward r in rewards)
        {
            RewardOverseer.RewardInstance.setReward(r.reward_type, r.unlocked, r.current_number);
            
            
        }

        RewardOverseer.RewardInstance.SetEvent(current_global_event);
    }

    public virtual void LoadEvents()
    {
        if (EventOverseer.Instance == null) return;

        foreach (GameEvent overseer_event in EventOverseer.Instance.concurrent_events)
        {
            overseer_event.is_waiting = true;
        }
  //      Debug.Log("Overseer loading snapshot\n");
        EventOverseer.Instance.SetEvent(current_event);


        foreach (string saver_event in concurrent_events)
        {
            foreach (GameEvent overseer_event in EventOverseer.Instance.concurrent_events)
            {
                if (overseer_event.my_name.Equals(saver_event))
                    overseer_event.is_waiting = false;

            }
        }
    }

    public virtual void SaveEvents()
    {
        if (EventOverseer.Instance == null) return;

        current_event = EventOverseer.Instance.current_event;
       // current_event_is_ingame = EventOverseer.Instance.ingame;
        List<string> active_concurrent_events = new List<string>();
        foreach (GameEvent ge in EventOverseer.Instance.concurrent_events)
        {
            if (!ge.is_waiting) active_concurrent_events.Add(ge.my_name);
        }
        concurrent_events = active_concurrent_events;
    }

    public void SaveScore()
    {
        if (ScoreKeeper.Instance == null)
        {
            Debug.Log("Cannot save score, ScoreKeeper is null\n");
            return;
        }
        
        score_details = ScoreKeeper.Instance.getScoreDetails();
        total_score = ScoreKeeper.Instance.getTotalScore();

        if (type == SaveStateType.MidLevel)
        {
            score_time = ScoreKeeper.Instance.getLevelTime();
            possible_dreams = ScoreKeeper.Instance.getPossibleDreams();
            possible_wishes = ScoreKeeper.Instance.getPossibleWishes();
        }
    }


    public void SaveFakePlayer()
    {
    //    if (FakePlayer.Instance != null) fakeplayer_saver = FakePlayer.Instance.getSnapshot();
        if (fakeplayer_saver != null && FakeRunner.Instance != null && FakeRunner.Instance.hasFakePlayer()) FakeRunner.Instance.current_player.fake_player.getSnapshot();
    }


    public void LoadFakePlayer()
    {
        if (fakeplayer_saver != null && FakeRunner.Instance != null && FakeRunner.Instance.hasFakePlayer()) FakeRunner.Instance.current_player.fake_player.loadSnapshot(fakeplayer_saver);


    }

}