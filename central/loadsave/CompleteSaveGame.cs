using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.Xml.Serialization;


[System.Serializable]
public class CompleteSaveGame
{
    public SaveGameSummary summary;
    public CompleteSaveState save_state;
    public string description;//break it out into an array or something, contails current level, score
    private bool _isLoaded;
    
    
    


    public bool DeleteFile()
    {
    //    Debug.Log("Gonna delete " + summary.getFileName() + "\n");
        if (System.IO.File.Exists(summary.getFileName()))
        {
            System.IO.File.Delete(summary.getFileName());
            description = "New Game";
            return true;
        }
        else
        {
            return false;
        }

        
    }
    

    public void SaveFile()
    {
     //   Debug.Log("SAVING FILE!!!!\n");
        SaveData save_data = new SaveData(summary.getFileName());
        save_data["summary"] = summary;
        save_data["save_state"] = save_state;

        DeleteFile();
        save_data.Save(summary.getFileName());

    }    
}

[System.Serializable]
public class CompleteSaveState : System.Object
{

   
    public List<CompleteIslandSaver> islands = new List<CompleteIslandSaver>();
    public float dreams;
    public float health;
    public List<ScoreDetails> score_details = new List<ScoreDetails>();
    public float total_score;
    public List<WishDial> possible_wishes;
    public float possible_dreams;
    public Difficulty difficulty;
    public float percent_sun_change;

    public int current_wave;
    public int current_wavelet;    
    public int current_event;        //level-specific eventoverseer
    public int current_global_event; //rewardoverseer
    public List<tower_stats> tower_stats;
    public List<unitStatsSaver> actor_stats = new List<unitStatsSaver>();
    public List<Rune> hero_stats = new List<Rune>();
    public List<SpecialSkillSaver> skills_in_inventory = new List<SpecialSkillSaver>();

    public float time_of_day;
    public List<string> concurrent_events = new List<string>();
    public List<Wish> wishes = new List<Wish>();
    public List<Reward> rewards = new List<Reward>();

    public int current_level;


    public CompleteSaveState() { }

  
    public void SaveHeroStats()
    {
        hero_stats = Central.Instance.hero_toy_stats;

    }

    public void SaveSkillInventory()
    {
        //wishes = Peripheral.Instance.my_inventory.getWishList();
        skills_in_inventory = Peripheral.Instance.my_skillmaster.getInventory();
    }

    public void SaveIslands()
    {
        islands = new List<CompleteIslandSaver>();
        foreach (Island_Button island in Monitor.Instance.islands.Values)
        {
            CompleteIslandSaver saver = island.getCompleteSnapshot();
            if (saver == null) continue;            
            islands.Add(saver);
        }
    }

    
    

    public void SaveToyStats()
    {

        actor_stats = new List<unitStatsSaver>();
        foreach (unitStats a in Central.Instance.actors) actor_stats.Add(a.getSnapshot());


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

    public void SaveTowerStats()
    {
        tower_stats = new List<tower_stats>();
        foreach (Island_Button island in Monitor.Instance.islands.Values)
        {
            if (island.isBlocked() && island.my_toy != null)
            {
                tower_stats.Add(island.my_toy.my_tower_stats.TrimClone());
            }
        }
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

            possible_dreams = ScoreKeeper.Instance.getPossibleDreams();
            possible_wishes = ScoreKeeper.Instance.getPossibleWishes();

    }

    public void SaveBasicMidLevelShit()
    {
     
        dreams = Peripheral.Instance.dreams;
        health = Peripheral.Instance.GetHealth();
        current_wave = Moon.Instance.GetCurrentWave();
        difficulty = Peripheral.Instance.difficulty;
        current_wavelet = Moon.Instance.current_wavelet;        
        percent_sun_change = Sun.Instance.percentComplete;
        current_level = Central.Instance.current_lvl;
    }
    
    public void SaveWishes()
    {
        wishes = Peripheral.Instance.my_inventory.getWishList();
    }
    

    public void SaveRewards()
    {
        rewards = RewardOverseer.RewardInstance.getRewards();        

    }
    
}