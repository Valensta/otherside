using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;


public class RewardOverseer : EventOverseer
{
    private static RewardOverseer rewardinstance;
    // Use this for initialization
    public static RewardOverseer RewardInstance { get; private set; }
    

    
    public delegate void onRewardEnabledHandler(RewardType reward_type, EffectType effect_type);
    public static event onRewardEnabledHandler onRewardEnabled;


    //public List<Reward> rewards;

    public List<Reward> getRewards()
    {
        List<Reward> list = new List<Reward>();
        foreach (GameEvent ge in concurrent_events)
        {
            if (isReward(ge))
            {
                list.Add(ge.my_reward.DeepClone());
            }
        }
        return list;
    }

  
    bool isReward(GameEvent ge)
    {
        return (ge.reward && ge.my_reward != null);
    }

    void Awake()
    {
        if (RewardInstance != null && RewardInstance != this)
        {
            Debug.Log("reward overseer got destroyeed\n");
            Destroy(gameObject);
        }

        RewardInstance = this;

        foreach (GameEvent ge in concurrent_events)
        {
            ge.is_waiting = true;
            ge.reward_trigger.SetReward();
            ge.my_reward.unlocked = false;
            ge.my_reward.current_number = 0;
        }


        return;
    }
    private void Start()
    {
        StartOverseer();
        Debug.Log("Starting reward overseer\n");
    }

    
    public void EnableReward(RewardType type)
    {
        Reward reward = getReward(type);
        if (reward == null)
        {
            Debug.LogError("Cannot enable/disable invalid reward " + type + "\n");
            return;
        }
        if (reward.unlocked) return;

        switch (type)
        {
            case RewardType.Modulator:                
                Central.Instance.getToy("modulator").isUnlocked = true;
                break;
            case RewardType.LaserFinisher:                
                break;
            case RewardType.RapidFireFinisher:
                break;
            case RewardType.HeroMobility:
                break;
            case RewardType.Determined:
                Peripheral.Instance.my_inventory.AddWish(WishType.Sensible, 10, 1);
                break;
            default:
                Debug.Log("RewardOverseerer is trying to enable an unsupported reward: " + type + "\n");
                break;
        }        
        reward.unlocked = true;
        if (onRewardEnabled != null) onRewardEnabled(type, reward.effect_type);
    }


    public override void StartOverseer()
    {        

        base.StartOverseer();


    }



    public bool getRewardStatus(RewardType type)
    {
        Reward r = getReward(type);
        return (r != null && r.unlocked);
    }

    public bool getRewardStatus(EffectType type)
    {
        Reward r = getReward(type);
        return (r != null && r.unlocked);
    }

    public Reward getReward(RewardType type)
    {
        Reward pickme = null;
        foreach (GameEvent ge in concurrent_events)        
            if (isReward(ge) && ge.my_reward.reward_type == type) pickme = ge.my_reward; 

        return pickme;
    }

    private Reward getReward(EffectType type)
    {
        Reward pickme = null;
        foreach (GameEvent ge in concurrent_events)
            if (isReward(ge) && ge.my_reward.effect_type == type) pickme = ge.my_reward;

        return pickme;
    }

    public void setReward(RewardType type, bool unlocked, float current_number)
    {
    //    Debug.Log("Set reward " + type + "\n");
        foreach (GameEvent ge in concurrent_events)
            if (isReward(ge) && ge.my_reward.reward_type == type)
            {
                ge.my_reward.unlocked = unlocked;
                ge.my_reward.current_number = current_number;
                ge.is_waiting = !unlocked;
            }
    }


}





