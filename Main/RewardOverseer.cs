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


    public List<Reward> rewards;

    public List<Reward> getRewards()
    {
        return rewards;
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
        }

        foreach (Reward r in rewards)
        {
            r.unlocked = false;
            r.current_number = 0;
        }

        if (current_event < events.Count)
        {
            //      Debug.Log("Running event " + current_event + "\n");
           
            toRun = events[current_event];
            toRun.gameObject.SetActive(true);
            toRun.RunEvent();
        }


        return;
    }

    public void EnableReward(RewardType type)
    {
        Reward reward = getReward(type);
        if (reward == null)
        {
            Debug.LogError("Cannot enable invalid reward " + type + "\n");
            return;
        }
        switch (type)
        {
            case RewardType.Modulator:
                //Peripheral.Instance.UnlockToy("modulator");
                //ListUtil.Add<string>(ref Peripheral.Instance.haveToys, "modulator");
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


    public override void StartMe(bool _ingame)
    {
    //    Debug.Log("RewardOverseer starting\n");
   


        base.StartMe(_ingame);
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
        foreach (Reward r in rewards) { if (r.reward_type == type) pickme = r; }
        return pickme;
    }

    private Reward getReward(EffectType type)
    {
        Reward pickme = null;
        foreach (Reward r in rewards) { if (r.effect_type == type) pickme = r; }
        return pickme;
    }

}





	