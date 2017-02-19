using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;

[System.Serializable]
public class RewardTrigger : Trigger
{    
    public RewardTrigger() { }
    private Reward my_reward;
    public RewardType reward_type;
    float timer;
    public float max_timer = 5f;
    public Vector3 vector = Vector3.zero;
    public int min_level = 1;
    
    public bool Ok()
    {
        if (Central.Instance.state != GameState.InGame) return false;
        if (Central.Instance.current_lvl < min_level) return false;
        return true;
    }


    public override bool CheckConditions()
    {
        if (!Ok()) return false;

        switch (condition)
        {
            case Condition.WishUsed:
                return my_reward.current_number >= number;
            case Condition.TowerSold:
                return my_reward.current_number >= number;
            case Condition.TIME:
                if (Central.Instance.getState() == GameState.InGame && Time.timeScale > 0f) my_reward.current_number += 0.25f * Time.timeScale;//set in game event
                return my_reward.current_number >= number;
            case Condition.Killer:

                if (timer > 0 && Central.Instance.getState() == GameState.InGame) timer += 0.1f;//set in game event

                if (my_reward.current_number >= number)
                {
                    timer = 0;
                    my_reward.current_number = 0;
                    Debug.Log("Killer successful\n");
                    return true;
                }

                if (timer > max_timer || Central.Instance.getState() != GameState.InGame)
                {
                    timer = 0;
                    my_reward.current_number = 0;                   
                }
                return false;
            case Condition.UpgradeSkill:
                return my_reward.current_number >= number;
                Debug.LogError("Condition " + condition + " is unsupported by RewardTrigger\n");
            default:
                return true;

        };
    }

    public Reward getReward()
    {
        if (my_reward == null)
        {
            Debug.Log("Trying to get blank reward: " + this.condition + "\n");
        }
        return (my_reward.reward_type == RewardType.Null)? null : my_reward;
    }

    public void SetReward()
    {
        my_reward = RewardOverseer.RewardInstance.getReward(reward_type);
    }


    public override void Init()
    {
    
        
        if (my_reward == null || my_reward.reward_type == RewardType.Null) Debug.LogError("RewardOverseer does not contain reward " + reward_type + "\n");
        if (condition == Condition.WishUsed) Inventory.onWishChanged += onWishChanged;
        if (condition == Condition.Killer) Body.onXpAdded += onXpAdded;
        if (condition == Condition.UpgradeSkill) Rune.onUpgrade += onUpgrade;
        if (condition == Condition.TowerSold) Peripheral.onSellToy += onSellToy;
   //     Debug.Log("Initialized rewardTrigger " + my_reward.reward_type + "\n");
        
    }

    

    public override void DisableMe()
    {         
        
        if (condition == Condition.WishUsed) Inventory.onWishChanged -= onWishChanged;
        if (condition == Condition.Killer) Peripheral.onDreamsAdded -= onXpAdded;
        if (condition == Condition.UpgradeSkill) Rune.onUpgrade -= onUpgrade;
        if (condition == Condition.TowerSold) Peripheral.onSellToy -= onSellToy;
    }


    public void onSellToy()
    {
        if (!Ok()) return;
        my_reward.current_number++;
    }

    public void onUpgrade(EffectType type, int ID)
    {
        if (!Ok()) return;
        if (type.ToString().Equals(text))
        {
            my_reward.current_number++;
        }
    }

    public void onWishChanged(Wish w, bool added, bool visible, float delta)
    {
        if (!Ok()) return;
        if (w.type != WishType.Sensible) return;
        if (added) return;        
        my_reward.current_number += Mathf.Abs(delta);
    }

    
    public void onXpAdded(float i, Vector3 pos)
    {
        if (!Ok()) return;
        if (i <= 0) return;
        if (vector != Vector3.zero && Vector3.Distance(vector, pos) < 3) return;

            //Debug.Log("On xp added " + i + " xp factor " + Moon.Instance.getWaveXpFactor() + "\n");
        my_reward.current_number += i / Moon.Instance.getWaveXpFactor();
        timer += 0.001f;
        vector = pos;
        
    }
}


	