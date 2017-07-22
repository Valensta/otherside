using System;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
//reserved for one time wishes
public class Reward : IDeepCloneable<Reward>
{
    public RewardType reward_type = RewardType.Null;
    public float current_number;
    public string name;
    public bool unlocked;
    public EffectType effect_type;

    object IDeepCloneable.DeepClone()
    {
        return this.DeepClone();
    }

    public Reward DeepClone()
    {
        Reward my_clone = new Reward();
        my_clone.reward_type = this.reward_type;
        my_clone.current_number = this.current_number;
        my_clone.name = string.Copy(this.name);
        my_clone.unlocked = this.unlocked;
        my_clone.effect_type = this.effect_type;

        return my_clone;
    }
}