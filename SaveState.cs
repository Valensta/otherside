using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SaveState: System.Object {
    public SaveStateType type; 
    public List<IslandSaver> islands = new List<IslandSaver>();
    public float dreams;
    public float health;    
    public List<ScoreDetails> score_details = new List<ScoreDetails>();
    public float total_score;
    public float possible_wishes;
    public float possible_dreams;
    public float difficulty;    
    //public int sensible_hero_point;
    //public int airy_hero_point;
    //public int vexing_hero_point;
    public int current_wave;
    public int current_event;
    public bool current_event_is_ingame;
	public List<actorStats> actor_stats = new List<actorStats> ();
	public List<ToySaver> hero_stats = new List<ToySaver>();
    public List<SpecialSkillSaver> skills_in_inventory = new List<SpecialSkillSaver>();
//	public bool snapshot;
	public float time_of_day;
    public List<string> concurrent_events = new List<string>();
    public List<Wish> wishes = new List<Wish>();
    public List<Reward> rewards = new List<Reward>();
    public int current_level;
    public List<TemporarySaver> temporary_effects = new List<TemporarySaver>();

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
        possible_wishes = 0;
        possible_dreams = 0;        
        current_level = -1;
        current_wave = 0;
        current_event = 0;
        current_event_is_ingame = false;
        concurrent_events = null;
        temporary_effects = null;


        return;
        concurrent_events = null;
        score_details = null;
        current_level = -1;
        current_wave = 0;
        current_event = 0;
        time_of_day = 0f;
        concurrent_events = null;
        health = 1;
        dreams = 0;
        islands = null;
    }

  
    public SaveState(float _dreams, float _health, int _current_wave, float _sens, float _airy, float _vex, int _sens_hero, int _airy_hero, int _vex_hero, float _difficulty)
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

    public void SaveRewards()
    {
        rewards = RewardOverseer.RewardInstance.getRewards();
    }

    public virtual void LoadRewards()
    {
        foreach (Reward r in rewards)
        {
            Reward overseer_reward = RewardOverseer.RewardInstance.getReward(r.reward_type);
            overseer_reward.current_number = r.current_number;
            overseer_reward.unlocked = r.unlocked;
        }
        Debug.Log("Rewards snapshot loaded\n");
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

    public ToySaver getCastle()
    {
        foreach (ToySaver t in hero_stats) if (t.rune.runetype == RuneType.Castle) return t;
        return null;
    }

    public void LoadHeroStats()
    {
        List<ToySaver> hero_toy_stats = Central.Instance.hero_toy_stats;
        foreach (ToySaver saveme in hero_stats)
        {
            //   Debug.Log("SETTING HERO STATS FOR " + saveme.rune.runetype);
            for (int i = 0; i < hero_toy_stats.Count; i++)
            {
                if (hero_toy_stats[i].rune.runetype == saveme.rune.runetype)
                {
                    hero_toy_stats[i] = saveme;
                    hero_toy_stats[i].rune.LoadSpecialSkills();
                    return;
                }
            }
            hero_toy_stats.Add(saveme);
            hero_toy_stats[hero_toy_stats.Count - 1].rune.LoadSpecialSkills();

        }
    }

    public void SaveToyStats(){

        actor_stats = new List<actorStats>();
		foreach (actorStats a in Central.Instance.actors){
			if (a.friendly) //monsters never change
				actor_stats.Add (a);
		}			

	}

    public void LoadToyStats()
    {
        foreach (actorStats a in actor_stats)
        {

            Central.Instance.getToy(a.name).setActive(a.isActive());
        }
    }

    public void SaveBasicMidLevelShit()
    {
        Debug.Log("Savig basic midlevel shit\n");
        time_of_day = Sun.Instance.current_time_of_day;

        temporary_effects = new List<TemporarySaver>();
        foreach (TemporarySaver ts in Peripheral.Instance.dream_factor.getTemporarySavers()) temporary_effects.Add(ts);
        foreach (TemporarySaver ts in Peripheral.Instance.xp_factor.getTemporarySavers()) temporary_effects.Add(ts);
        foreach (TemporarySaver ts in Peripheral.Instance.damage_factor.getTemporarySavers()) temporary_effects.Add(ts);       

        dreams = Peripheral.Instance.dreams;
        health = Peripheral.Instance.GetHealth();
        current_wave = Peripheral.Instance.current_wave;
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

    public void LoadScore()
    {
        if (ScoreKeeper.Instance == null)
        {
            Debug.Log("Cannot load score, ScoreKeeper is null\n");
            return;
        }
       
        ScoreKeeper.Instance.setScoreDetails(score_details);
        ScoreKeeper.Instance.setTotalScore(total_score);

        if (type == SaveStateType.MidLevel)
        {
            ScoreKeeper.Instance.setPossibleDreams(possible_dreams);
            ScoreKeeper.Instance.setPossibleWishes(possible_wishes);
        }
    }

    public void LoadSkillsInventory(bool reset_remaining_time)
    {
        foreach (SpecialSkillSaver t in skills_in_inventory)
        {
            if (reset_remaining_time) t.remaining_time = 0;
            Peripheral.Instance.my_skillmaster.setInventory(t, true);
        }       
    }

    public virtual void LoadEvents()
    {
        if (EventOverseer.Instance == null) return;

        foreach (GameEvent overseer_event in EventOverseer.Instance.concurrent_events)
        {
            overseer_event.is_waiting = false;
        }
        Debug.Log("Overseer loading snapshot\n");
        EventOverseer.Instance.SetEvent(current_event, current_event_is_ingame);
        foreach (string saver_event in concurrent_events)
        {
            foreach (GameEvent overseer_event in EventOverseer.Instance.concurrent_events)
            {
                if (overseer_event.my_name == saver_event)
                    overseer_event.is_waiting = true;

            }
        }
    }

    public virtual void SaveEvents()
    {
        if (EventOverseer.Instance == null) return;

        current_event = EventOverseer.Instance.current_event;
        current_event_is_ingame = EventOverseer.Instance.ingame;
        List<string> active_concurrent_events = new List<string>();
        foreach (GameEvent ge in EventOverseer.Instance.concurrent_events)
        {
            if (ge.is_waiting) active_concurrent_events.Add(ge.my_name);
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
            possible_dreams = ScoreKeeper.Instance.getPossibleDreams();
            possible_wishes = ScoreKeeper.Instance.getPossibleWishes();
        }
    }
}