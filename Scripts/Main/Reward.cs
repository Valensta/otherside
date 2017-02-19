using System;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
//reserved for one time wishes
public class Reward
{
    public RewardType reward_type = RewardType.Null;
    public float current_number;
    public string name;
    public bool unlocked;    
    public EffectType effect_type;
}