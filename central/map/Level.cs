﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class LevelMod : IDeepCloneable<LevelMod>
{
    public bool remove_lvl_caps = false;
    public float xp_uplift = 0;
    public float dream_uplift = 0;
    public float sensible_wish_uplift = 0;
    public float lull_multiplier_unused = 1f;     //between wavelets
    public float wave_time_multiplier = 1f; //between individual enemies in a wavelet
    public float destination_time_based_level;
    public Difficulty difficulty = Difficulty.Normal;
    public List<TowerMaxLevel> tower_max_level_settings;

    public override string ToString()
    {
        List<string> me = new List<string>();
        if (xp_uplift > 0) me.Add("xp_" + xp_uplift);
        if (dream_uplift > 0) me.Add("dream_" + dream_uplift);
        if (lull_multiplier_unused != 1) me.Add("lull_mult_" + lull_multiplier_unused);
        if (wave_time_multiplier != 1) me.Add("interval_mult_" + wave_time_multiplier);
        if (remove_lvl_caps) me.Add("nolvlcap");
        me.Add($"percent_lvl_xp_per_wave_{destination_time_based_level}");
        
        return me.Count > 0 ? string.Join("|", me.ToArray()) : "default";
    }
 
    public void setIntervals(float lull_mult, float interval_mult)
    {
        lull_multiplier_unused = lull_mult;
        wave_time_multiplier = interval_mult;
            
    }

  
    
    public LevelMod(Difficulty difficulty, float xp_uplift, float dream_uplift, float sensible_wish_uplift, bool remove_lvl_caps)
    {
        this.difficulty = difficulty;
        this.xp_uplift = xp_uplift;
        this.dream_uplift = dream_uplift;
        this.sensible_wish_uplift = sensible_wish_uplift;
        this.remove_lvl_caps = remove_lvl_caps;
    }

    public LevelMod(Difficulty difficulty, float xp_uplift, float dream_uplift, float sensible_wish_uplift, bool remove_lvl_caps, float lull_mult, float waveTimeMult, float percent_xp)
    {
        this.difficulty = difficulty;
        this.xp_uplift = xp_uplift;
        this.dream_uplift = dream_uplift;
        this.sensible_wish_uplift = sensible_wish_uplift;
        this.remove_lvl_caps = remove_lvl_caps;
        this.lull_multiplier_unused = lull_mult;
        this.wave_time_multiplier = waveTimeMult;
        this.destination_time_based_level = percent_xp;
    }

    public LevelMod(Difficulty difficulty, float xp_uplift, float dream_uplift, float sensible_wish_uplift, bool remove_lvl_caps, float lull_mult, float waveTimeMult)
    {
        this.difficulty = difficulty;
        this.xp_uplift = xp_uplift;
        this.dream_uplift = dream_uplift;
        this.sensible_wish_uplift = sensible_wish_uplift;
        this.remove_lvl_caps = remove_lvl_caps;
        this.lull_multiplier_unused = lull_mult;
        this.wave_time_multiplier = waveTimeMult;
        this.destination_time_based_level = difficulty == Difficulty.Normal
            ? 5f
            : difficulty == Difficulty.Hard
                ? 4f
                : difficulty == Difficulty.Insane
                    ? 3f : 5f;
    }
    
    object IDeepCloneable.DeepClone()
    {
        return this.DeepClone();
    }

    public LevelMod DeepClone()
    {
        LevelMod clone = new LevelMod(this.difficulty, this.xp_uplift, this.dream_uplift, sensible_wish_uplift, remove_lvl_caps, lull_multiplier_unused, wave_time_multiplier);
        return clone;
    }
}



[System.Serializable]
public class Level {

	public string name;
	public int number;
	public MyButton button;
	public GameObject description;
    //public float difficulty;
    public Difficulty difficulty;
    public bool test_mode;
    public bool fancy;    


    public void DisableMe()
    {
        button.gameObject.SetActive(false);
    }

}
