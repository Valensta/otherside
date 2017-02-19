using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class StaticStat {


    public static float getMultiplier(RuneType rune_type, EffectType effect_type, int level, bool is_hero) {
        return getBaseFactor(rune_type, effect_type, level, is_hero);

    }

    public static float getBaseFactor(RuneType rune_type, EffectType effect_type, bool is_hero)
    {
        return getBaseFactor(rune_type, effect_type, 0, is_hero);
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
                        return (level == 0) ? 1f : 0.6f;
                    case EffectType.RapidFire:
                        return (level <= 0) ? 1f : (level == 1) ? 0.15f : ((level == 2)?  0.4f : 0.8f); 
                    case EffectType.DOT:
                        return (level <= 0) ? 1f : (level == 1) ? 0.9f : ((level == 2) ? 1.5f : 1.75f);
                    case EffectType.Laser:
                        return (level <= 1) ? 0.95f : ((level == 2) ? 0.75f : 0.75f); //.83 .93
                    case EffectType.Sparkles:
                        return (level <= 0) ? 1f : (level == 1) ? 0.2f : ((level == 2) ? 1f : 1.4f);
                    case EffectType.AirAttack:
                        return (level == 0) ? 2f : 2f;
                }
                return 1f;                                
            case RuneType.Airy:
                switch (effect_type)
                {                    
                    case EffectType.ReloadTime:
                        return (level == 0) ? 3f : -.8f;
                    case EffectType.Range:
                        if (is_hero)
                            return (level == 0) ? 2.5f : 0.15f;
                        else
                            return (level == 0) ? 2.25f : 0.12f;
                    case EffectType.Speed:
                        return (level == 0) ? 0.5f : 0.4f;
                    case EffectType.Force:
                        return (level == 0) ? 0.005f : 0.005f;
                    case EffectType.Focus:
                        return 0.0015f;
                    case EffectType.Calamity:
                        return 0.08f;
                    case EffectType.Swarm:
                        return (level == 0) ? 0.11f : 0.12f;
                    case EffectType.Weaken:
                        return 0.2f;
                    case EffectType.WishCatcher:
                        return (level == 0) ? 0.33f : 0.66f;
                    case EffectType.Quicksand:
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
                    case EffectType.Diffuse:
                        //return (level <= 0) ? 1f : (level == 1) ? 0.1f : ((level == 2) ? 0.8f : 1f);
                        return (level <= 0) ? 1f : (level == 1) ? 0.1f : ((level == 2) ? 0.6f : 0.8f);
                    case EffectType.Transform:
                        return (level == 0) ? 0.4f : 0.25f;
                    case EffectType.Focus:
                        return (level <= 0) ? 1f : (level == 1) ? 1.4f : ((level == 2) ? 1.7f : 2f);
                    case EffectType.Fear:
                        return 0.5f;
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
                        return (level == 0) ? 4.5f : .6f;
                    case EffectType.ReloadTime:
                        return (level == 0) ? .2f : -.02f;
                    case EffectType.Force:
                        return (level == 0) ? 0.4f : 0.25f;                    
                }
                return 1f;                               
            case RuneType.Fast:
                switch (effect_type)
                {
                    case EffectType.Range:
                        return (level == 0) ? 2.25f : .4f;
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
                        return (level == 0) ? 2f : .4f;
                    case EffectType.ReloadTime:
                        return (level == 0) ? 5f : -.8f;
                    case EffectType.TimeSpeed:
                        return (level == 0) ? 0.5f : 0.5f;
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
                        return (level == 0) ? 7f : 0f;
                }
                return 1f;
            case RuneType.Castle:
                switch (effect_type)
                {
                    case EffectType.BaseHealth:
                        return (level == 0) ? 1f : 2f;
                    case EffectType.Gnomes:
                        return (level == 0) ? 1f : -.15f;
                }
                return 1f;
        }
        return 1f;
    }




    public static Cost getCost(EffectType effect_type, int level)
    {
        Cost cost = new Cost();
        if (Get.isSpecial(effect_type))
        {
            cost.type = CostType.ScorePoint;
            switch (effect_type)
            {
                case EffectType.AirAttack:
                    cost.Amount = (level == 0) ? 200 : (level == 1) ? 300 : 400;
                    break;
                case EffectType.Quicksand:
                    cost.Amount = (level == 0) ? 200 : (level == 1) ? 300 : 400;
                    break;
                case EffectType.EMP:
                    cost.Amount = (level == 0) ? 200 : (level == 1) ? 300 : 400;
                    break;
                case EffectType.Plague:
                    cost.Amount = (level == 0) ? 200 : (level == 1) ? 300 : 400;
                    break;
                case EffectType.Bees:
                    cost.Amount = (level == 0) ? 200 : (level == 1) ? 300 : 400;
                    break;
                case EffectType.Teleport:
                    cost.Amount = (level == 0) ? 200 : (level == 1) ? 300 : 400;
                    break;
                case EffectType.Gnomes:
                    cost.Amount = (level == 0) ? 250 : (level == 1) ? 400 : 550;
                    break;
                case EffectType.BaseHealth:
                    cost.Amount = (level == 0) ? 250 : (level == 1) ? 400 : 550;
                    break;
                default:
                    cost.Amount = (level == 0) ? 200 : (level == 1) ? 300 : 400;
                    break;
            }
        }
        else
        {
            cost.type = CostType.Dreams;
            cost.Amount = (level == 0) ? 15 : (level == 1) ? 20 : 20;
        }
        return cost;
    }
}
