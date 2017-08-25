using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class StaticStat {


    public static float getBaseFactorForLevel(RuneType rune_type, EffectType effect_type, int level, bool is_hero) {
        return getBaseFactor(rune_type, effect_type, level, is_hero);

    }

    public static float getBaseFactor(RuneType rune_type, EffectType effect_type, bool is_hero)
    {
        return getBaseFactor(rune_type, effect_type, 0, is_hero);
    }

    

    public static bool isUnlocked(RuneType rune_type, bool hero, bool unlocked)
    {
        //   Debug.Log("Checking if " + rune_type + " hero " + hero + " is unlocked (current: " + unlocked + ")\n");
        if (rune_type == RuneType.Null) return false;
        bool test_mode = Central.Instance.level_list.test_mode;
        int current_lvl = Central.Instance.current_lvl;
        //bool ok = false;
        switch (rune_type)
        {
            case RuneType.Castle:
                return true;

            case RuneType.Sensible:
                return unlocked || !hero || current_lvl >= 1;

            case RuneType.Airy:
                return unlocked || !hero || current_lvl >= 2;

            case RuneType.Vexing:
                return unlocked || !hero || current_lvl >= 3;

            case RuneType.SensibleCity:
                return (unlocked || (test_mode && current_lvl >= 1));  //unlocked by event

            case RuneType.Modulator:
                return (unlocked);  //unlocked by reward                

            case RuneType.Fast:
                return (unlocked || (test_mode && current_lvl >= 1));  //unlocked by event

            case RuneType.Slow:
                return unlocked || current_lvl >= 2;

            case RuneType.Time:
                return unlocked || current_lvl >= 3;

            default:
                Debug.Log("Checking unlock status for unknown runetype " + rune_type + "\n");
                return false;

        }
        
    }


    public static float getBaseFactor(RuneType rune_type, EffectType effect_type, int level, bool is_hero)
    {
      

        switch (rune_type)
        {
            case RuneType.Sensible:
                switch (effect_type)
                {                    
                    case EffectType.Range:
                        if (is_hero)
                            return (level == 0) ? 3f : 0.2f;
                        else
                            return (level == 0) ? 2.5f : 0.15f;
                    case EffectType.ReloadTime:
                        return (level == 0) ? 0.8f : -.15f;
                    case EffectType.Force:
                        return (level == 0) ? 1.1f : 0.6f;
                    case EffectType.Diffuse:                        
                        return (level <= 0) ? 1f : (level == 1) ? 0.1f : ((level == 2) ? 0.23f : 0.35f);
                    case EffectType.Transform:
                        return (level == 0) ? 0.6f : (level == 1) ? 0.5f : 0.5f;
                    case EffectType.Laser:
                        return (level <= 0) ? 1f : (level == 1)? 0.65f : ((level == 2) ? 0.9f : 1.2f); //1.1
                    case EffectType.Sparkles:
                        return (level <= 0) ? 1f : (level == 1) ? 0.2f : ((level == 2) ? 0.55f : 0.9f);
                    case EffectType.AirAttack:
                        return (level == 0) ? 2f : 2f;
                    case EffectType.Meteor:
                        return 1f;
                }
                return 1f;                                
            case RuneType.Airy:
                switch (effect_type)
                {                    
                    case EffectType.ReloadTime:
                        return (level == 0) ? 3f : -.8f;
                    case EffectType.Range:
                        if (is_hero)
                            return (level == 0) ? 2.5f : 0.35f;
                        else
                            return (level == 0) ? 2.25f : 0.20f;
                    case EffectType.Speed:                        
                        return (level == 0) ? 45f : 50f; //55
                    case EffectType.Force:
                        return (level == 0) ? 0.065f : 0.06f;                         
                    case EffectType.Calamity:
                        return (level <= 0) ? 1f : (level == 1)? 2f : (level == 2) ? 1.2f :1.4f;
                    case EffectType.Swarm:
                        return (level <= 0) ? 1f : (level == 1) ? 1f : (level == 2) ? 1.25f : 1.6f;
                    case EffectType.Weaken:                      
                        return (level == 0) ? 1 : (level == 1)? 30f : (level == 2) ? 25f : 25f;
                    case EffectType.WishCatcher:
                        return (level == 0) ? 1 : (level == 1)? 20f : (level == 3) ? 35f : 50f;
                    case EffectType.Frost: //not used
                        return (level == 0) ? 0.8f : 0.7f;
                    case EffectType.Foil:
                        return (level == 0) ? 0.8f : 0.7f;
                    case EffectType.EMP:
                        return (level == 0) ? 0.8f : 0.7f;
                    case EffectType.Plague:
                        return (level == 0) ? 0.8f : 0.7f;
                }
                return 1f;                
            case RuneType.Vexing:
                switch (effect_type)
                {
                    case EffectType.Range:
                        if (is_hero)
                            return (level == 0) ? 3.5f : 0.2f;
                        else
                            return (level == 0) ? 3f : 0.15f;
                    case EffectType.ReloadTime:
                        return (level == 0) ? 1.75f : -.3f;                    
                    case EffectType.VexingForce:
                        return 2.75f;
                    case EffectType.RapidFire:
                        return (level <= 0) ? 1f : (level == 1) ? 0.15f : ((level == 2) ? 0.22f : 0.32f);
                    case EffectType.DOT:
                        return (level <= 0) ? 1f : (level == 1) ? 1f : ((level == 2) ? 2.5f : 4.5f);
                    case EffectType.Focus:                        
                        return (level <= 0) ? 1f : (level == 1) ? 4f : ((level == 2) ? 3.5f : 3.9f);
                    case EffectType.Fear:
                        return (level <= 0) ? 0.5f : (level == 1) ? 0.8f : 1.1f;
                    case EffectType.Critical:
                        return (level <= 0) ? 1f : (level == 1) ? 0.65f : ((level == 2) ? 0.5f : 0.65f);
                    case EffectType.Teleport:
                        return -0.8f;
                    case EffectType.Bees:
                        return 1f;                       
                }
                return 1f;
            case RuneType.Slow:
                switch (effect_type)
                {
                    case EffectType.Range:
                        return (level == 0) ? 3.5f : .6f;
                    case EffectType.ReloadTime:
                        return (level == 0) ? .3f : -.02f;
                    case EffectType.Force:
                        return (level == 0) ? 0.4f : 0.25f;                    
                }
                return 1f;                               
            case RuneType.Fast:
                switch (effect_type)
                {
                    case EffectType.Range:
                        return (level == 0) ? 2.75f : .4f;
                    case EffectType.ReloadTime:
                        return (level == 0) ? 0.9f : -.1f;
                    case EffectType.Force:
                        return (level == 0) ? 1.3f : 1.3f;
                }
                return 1f;
            case RuneType.Time:
                switch (effect_type)
                {
                    case EffectType.Range:
                        return (level == 0) ? 1.8f : .4f;
                    case EffectType.ReloadTime:
                        return (level == 0) ? 0.5f : -.1f;
                    case EffectType.Speed:
                        return (level == 0) ? 70f : 80f;
                }
                return 1f;
            case RuneType.SensibleCity:
                switch (effect_type)
                {
                    case EffectType.TowerForce:
                        return (level == 0) ? 0f : 4f;
                    case EffectType.TowerRange:
                        return (level == 0) ? 0f : 4f;
                    case EffectType.Range:
                        return (level == 0) ? 50f : 0f;
                }
                return 1f;
            case RuneType.Castle:
                switch (effect_type)
                {
                    case EffectType.Renew:
                        return (level == 0) ? 1f : 2f; // not used
                    case EffectType.Architect:
                        return (level <= 0) ? 1f : (level == 1) ? 5f : ((level == 2) ? 10f : 15f); //not used
                }
                return 1f;
        }
        return 1f;
    }


    public static int StatLength(EffectType type,  bool fin)
    {
        return StatLength(type, EffectSubType.Null, fin);
    }

    public static int StatLength(EffectType type, EffectSubType sub_type, bool fin)
    {
        
        switch (type)
        {
           
            case EffectType.Speed:                            
            case EffectType.Stun:
                return (sub_type == EffectSubType.Freeze)? 6: (!fin) ? 3 : 4;
            case EffectType.Frost:
                return 7;
            case EffectType.Plague:
                return 5;
            case EffectType.Renew:
                return 2;
            case EffectType.Architect:
                return 1;
            case EffectType.DOT:
                return (!fin) ? 2 : 5;
            case EffectType.Teleport:
                return 4;
            case EffectType.Bees:
                return 5;
            case EffectType.Transform:
                return (!fin) ? 2 : 3;            
            case EffectType.Diffuse:
                return (!fin) ? 6 : 7;
            case EffectType.Focus:
                return (!fin) ? 3 : 4;
            case EffectType.Foil:
            case EffectType.EMP:
                return 4;
            case EffectType.Laser:
                return (!fin) ? 2 : 3;
            case EffectType.Sparkles:
                return (!fin) ? 3 : 5;
            case EffectType.Force:
                return 2;
            case EffectType.VexingForce:
                return 2;
            case EffectType.RapidFire:
                return (!fin) ? 6 : 7;
            case EffectType.AirAttack:
                return 5;
            case EffectType.Meteor:
                return 5;
            case EffectType.Explode_Force:
                return (!fin) ? 2 : 3;
            case EffectType.Calamity:
                return (!fin) ? 4 : 5;
            case EffectType.Swarm:
                return (!fin) ? 4 : 5;
            case EffectType.Fear:
                return (!fin) ? 1 : 4;
            case EffectType.WishCatcher:
                return (!fin) ? 2 : 3;
            case EffectType.Critical:
                return (!fin) ? 3 : 4;
            case EffectType.Weaken:
                return (!fin) ? 2 : 3;
            case EffectType.DirectDamage:
                return 2;
            default:
                Debug.LogError("UNspported Stat length (" + type + ") !!!!!!!!!!!!!\n");
                return 99;
                    
        }
    }


    public static int getFinisherLvl()
    {
        return 3;
    }


    public static float getInitRechargeTime(EffectType effect_type)
    {
        if (!Get.isSpecial(effect_type)) return 0;

        switch (effect_type)
        {
            case EffectType.AirAttack:
                return 80;
            case EffectType.Meteor:
                return 80;
            case EffectType.Bees:
                return 80;
            case EffectType.EMP:
                return 50;
            case EffectType.Plague:
                return 60;
            case EffectType.Frost:
                return 60;
            case EffectType.Teleport:
                return 50;
            case EffectType.Renew:
                return 0f;
            default:
                return 60;
                    
        }
    }

    public static Cost getCumulativeCost(RuneType rune_type, EffectType effect_type, int level)
    {
        Cost cost = null;
        for (int i = 0; i < level; i++)
        {
            if (cost == null)
            {
                cost = getCost(rune_type, effect_type, i);
            }else            
            {
                cost.Amount += getCost(rune_type, effect_type, i).Amount;
            }
        }
        return cost;
    }

    public static Cost getCost(RuneType rune_type, EffectType effect_type, int level)
    {
        Cost cost = new Cost();
        if (Get.isSpecial(effect_type))
        {
            cost.type = CostType.ScorePoint;
            switch (effect_type)
            {
                case EffectType.AirAttack:
                    cost.Amount = (level == 0) ? 40 : (level == 1) ? 60 : 80;
                    break;
                case EffectType.Meteor:
                    cost.Amount = (level == 0) ? 40 : (level == 1) ? 60 : 80;
                    break;
                case EffectType.Frost:
                    cost.Amount = (level == 0) ? 40 : (level == 1) ? 60 : 80;
                    break;
                case EffectType.EMP:
                    cost.Amount = (level == 0) ? 40 : (level == 1) ? 60 : 80;
                    break;
                case EffectType.Plague:
                    cost.Amount = (level == 0) ? 40 : (level == 1) ? 60 : 80;
                    break;
                case EffectType.Bees:
                    cost.Amount = (level == 0) ? 40 : (level == 1) ? 60 : 80;
                    break;
                case EffectType.Teleport:
                    cost.Amount = (level == 0) ? 40 : (level == 1) ? 60 : 80;
                    break;
                case EffectType.Architect:
                    cost.Amount = (level == 0) ? 80 : (level == 1) ? 100 : 120;
                    break;
                case EffectType.Renew:
                    cost.Amount = (level == 0) ? 80 : (level == 1) ? 100 : 120;
                    break;
                default:
                    cost.Amount = (level == 0) ? 40 : (level == 1) ? 60 : 80;
                    break;
            }
        }
        else
        {
            cost.type = CostType.Dreams;
            
            switch (rune_type){
                case RuneType.Sensible:
                    cost.Amount = (level == 0) ? 20 : (level == 1) ? 30 : 45;
                    break;
                case RuneType.Airy:
                    cost.Amount = (level == 0) ? 18 : (level == 1) ? 28 : 40;
                    break;
                case RuneType.Vexing:
                    cost.Amount = (level == 0) ? 25 : (level == 1) ? 35 : 55;
                    break;
                default:
                    cost.Amount = (level == 0) ? 20 : (level == 1) ? 30 : 40;
                    break;


            }

            
        }
        
        return cost;
    }
}
