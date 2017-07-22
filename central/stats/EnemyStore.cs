using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using NUnit.Framework;


[System.Serializable]

public enum EnemyType { Null, TestUnit, Magical, Drone, Moth, Soldier, TinyFly, FurFly, SuperFly, TinyPlane, Plane, Tank, TinyTank, Bird, SuperPlane, Ghost, Orc,
                        Turtle, SturdyTank, Frog, ImpossibleTank, AssemblyPlane, MechSoldier};

public static class EnemyStore
{

    public static Dictionary<EnemyType, List<Defense>> enemyDefenses;
    

    public static float low_defense = 0.25f;
    public static float lowmid_defense = 0.35f;
    public static float mid_defense = 0.50f;
    public static float high_defense = 0.75f;

    public static List<Defense> getDefenses(EnemyType type, bool clone)
    {
        if (enemyDefenses == null) initDefenses();

        List<Defense> defenses = enemyDefenses[type];
        return (clone) ? CloneUtil.copyList<Defense>(defenses) : defenses;
        
    }

    public static float getMass(EnemyType type)
    {
        return defineMass(type);
    }


    public static int getHealth(EnemyType type) //for display purposes
    {
        float mass = defineMass(type);
        return Mathf.RoundToInt(mass * 50f / Get.bullshit_damage_factor);
    }

    public static int getDefenseAmount(float def)
    {
        if (def == 0) return 0;
        if (def == low_defense) return 1;
        if (def == lowmid_defense) return 2;
        if (def == mid_defense) return 3;
        if (def == high_defense) return 4;
        if (def == 1) return 99;
        return -1;

    }

    public static void initDefenses()
    {
        enemyDefenses = new Dictionary<EnemyType, List<Defense>>();
        //List<EnemyType> new_list = new List<EnemyType>();

        foreach (EnemyType type in Enum.GetValues(typeof(EnemyType))) {
            enemyDefenses.Add(type, defineDefenses(type));
        }
    }

    public static float getEffectiveMass(EnemyType type)
    {
        float mass = getMass(type);
        List<Defense> def = getDefenses(type, false);

        float avg_def;
        float total_num = 0;
        float total_denum = 0;
        foreach (Defense d in def)
        {
            if (d.type == EffectType.Force)
            {
                total_num += 2f*d.strength;
                total_denum += 2f;                
            }
            if (d.type == EffectType.Magic)
            {
                total_num += 1f*d.strength;
                total_denum += 1f;                
            }
                
        }
        Debug.Log($"{type.ToString()} -> {mass} -> {mass / (1 - total_num / total_denum)}\n");
        return mass / (1 - total_num / total_denum);
    }


    public static void initEnemies()
    {
        foreach (EnemyType type in Enum.GetValues(typeof(EnemyType)))
        {
            enemyStats enemy = new enemyStats(type);            
            Central.Instance.enemies.Add(enemy);
        }
    }

    public static Sprite getEnemySprite(EnemyType type)
    {
        //unfortunately this has to be a separate image from what's actually used for the enemy prefab.
        Sprite s = Get.getSprite("GUI/Enemies/" + type.ToString());
        if (s == null)
        {
            Debug.LogError("EnemyStore could not find a sprite for " + type.ToString() + "\n");
            return Get.getSprite("GUI/Enemies/Soldier");
        }
        return s;

    }

    public static Sprite getInfoButtonSprite(EnemyType type)
    {
        //unfortunately this has to be a separate image from what's actually used for the enemy prefab.
        Sprite s = Get.getSprite("GUI/Enemies/" + type.ToString() + "_info_button");
        if (s == null)
        {
            Debug.LogError("EnemyStore could not find an info button sprite for " + type.ToString() + "\n");
            return Get.getSprite("GUI/Enemies/hint_info_button");
        }
        return s;

    }

    public static String getEnemyDescription(EnemyType type)
    {
        switch (type)
        {

            case EnemyType.AssemblyPlane:
                return "Typical human soldier. Boring. Carry on.";
            case EnemyType.Bird:
                return "BIRD: Hovering box that possesses advanced electrical shielding capabilities and moderate defenses.";
            case EnemyType.Drone:
                return "Drone";
            case EnemyType.Frog:
                return "FROG: A curious, biomechanical invention of superior rubbery strength. Possesses regenerative capabilities.";
            case EnemyType.FurFly:
                return "FURFLY: A slightly improved version of the Tinyfly unit. Resistant to magic.";
            case EnemyType.Ghost:
                return "GHOST: needs a description.";
            case EnemyType.ImpossibleTank:
                return "IMPOSSIBLE TANK: Advanced super-strong tank that makes a sturdy tank when destroyed. :(";
            case EnemyType.Magical:
                return "SPY DRONE: Kill it quick.";
            case EnemyType.MechSoldier:
                return "MECH SOLDIER: Reinforced human soldier.";
            case EnemyType.Moth:
                return "MOTH: Super tiny metal insect. Strength in numbers.";
            case EnemyType.Orc:
                return "ORC: needs a description";
            case EnemyType.Plane:
                return "PLANE: Some kind of metal thing that can glide through the air.";
            case EnemyType.Soldier:
                return "SOLDIER: Just a typical human soldier. Boring. Carry on.";
            case EnemyType.SturdyTank:
                return "STURDY TANK: Tougher tank that turns into a regular tank when destroyed. What.";
            case EnemyType.SuperFly:
                return "SUPERFLY: A superior version of the Furfly scouting unit with reinforced armor.";
            case EnemyType.SuperPlane:
                return "SUPER PLANE: an even bigger and stronger plane than the usual Plane! Oh no!";
            case EnemyType.Tank:
                return "TANK: Converts into a tiny tank when destroyed.";
            case EnemyType.TestUnit:
                return "Test Unit";
            case EnemyType.TinyFly:
                return "TINYFLY: A fast but weak unit used for recon.";
            case EnemyType.TinyPlane:
                return "TINY PLANE: A tiny paper plane. Adorable.";
            case EnemyType.TinyTank:
                return "TINY TANK: Some kind of a tiny bug-like thing.";
            case EnemyType.Turtle:
                return "TURTLE: Slow and very well defended.Keeps making little planes for some reason.";

           default:
                return type.ToString() + ": Some random enemy, I don't know.";
        }

    }


    private static List<Defense> defineDefenses(EnemyType type)
    {
        
        switch (type)
        {


            case EnemyType.Magical:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, 0),
                    new Defense(EffectType.Force, 0),
                    new Defense(EffectType.Fear, 0),
                    new Defense(EffectType.Transform, 0)
                };
            case EnemyType.Soldier:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, 0),
                    new Defense(EffectType.Force, low_defense)
                };                
            case EnemyType.MechSoldier:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, low_defense),
                    new Defense(EffectType.Force, low_defense),
                    new Defense(EffectType.Fear, mid_defense)
                };



            case EnemyType.TinyFly:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, lowmid_defense),
                    new Defense(EffectType.Force, lowmid_defense),
                    new Defense(EffectType.Fear, low_defense)
                };
            case EnemyType.FurFly:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, low_defense),
                    new Defense(EffectType.Force, low_defense),
                    new Defense(EffectType.Fear, low_defense)                    
                };
            case EnemyType.SuperFly:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, mid_defense),
                    new Defense(EffectType.Force, low_defense),
                    new Defense(EffectType.Fear, mid_defense),
                    new Defense(EffectType.Transform, mid_defense)
                };
            case EnemyType.Moth:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, 0),
                    new Defense(EffectType.Force, low_defense),
                    new Defense(EffectType.Fear, low_defense)
                };                
            case EnemyType.TinyPlane:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, 0),
                    new Defense(EffectType.Force, low_defense),
                    new Defense(EffectType.Fear, low_defense)
                };
            case EnemyType.Plane:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, 0),
                    new Defense(EffectType.Force, low_defense),                    
                    new Defense(EffectType.Transform, low_defense),
                    new Defense(EffectType.Fear, low_defense)
                };
            case EnemyType.SuperPlane:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, low_defense),
                    new Defense(EffectType.Force, lowmid_defense),
                    new Defense(EffectType.Fear, mid_defense),
                    new Defense(EffectType.Transform, lowmid_defense)
                };
            case EnemyType.AssemblyPlane:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, mid_defense),
                    new Defense(EffectType.Force, mid_defense),
                    new Defense(EffectType.Fear, low_defense),
                    new Defense(EffectType.Transform, 1)
                };                
            case EnemyType.TinyTank:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, 0),
                    new Defense(EffectType.Force, low_defense),                    
                    new Defense(EffectType.Transform, low_defense),
                    new Defense(EffectType.Fear, low_defense)
                };
            case EnemyType.Tank:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, 0),
                    new Defense(EffectType.Force, low_defense),                   
                    new Defense(EffectType.Transform, lowmid_defense),
                    new Defense(EffectType.Fear, low_defense)
                };
            case EnemyType.SturdyTank:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, 0),
                    new Defense(EffectType.Force, lowmid_defense),                    
                    new Defense(EffectType.Transform, mid_defense),
                    new Defense(EffectType.Fear, low_defense)
                };
            case EnemyType.ImpossibleTank:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, 0f),
                    new Defense(EffectType.Force, lowmid_defense), 
                    new Defense(EffectType.Fear, high_defense),
                    new Defense(EffectType.Transform, mid_defense)
                };
         

            case EnemyType.Bird:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, mid_defense),
                    new Defense(EffectType.Force, lowmid_defense),
                    new Defense(EffectType.Fear, mid_defense),
                    new Defense(EffectType.Transform, lowmid_defense)
                };

            case EnemyType.Frog:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, low_defense),
                    new Defense(EffectType.Force, low_defense),
                    new Defense(EffectType.Fear, mid_defense),
                    new Defense(EffectType.Transform, 0)
                };

            case EnemyType.Ghost:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, low_defense),
                    new Defense(EffectType.Force, low_defense),
                    new Defense(EffectType.Fear, low_defense),                    
                };
            case EnemyType.Turtle:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, mid_defense),
                    new Defense(EffectType.Force, mid_defense),
                    new Defense(EffectType.Fear, mid_defense), //.83
                    new Defense(EffectType.Transform, high_defense) //.6
                };
            case EnemyType.Drone:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, 0),
                    new Defense(EffectType.Force, low_defense),                    
                    new Defense(EffectType.Transform, low_defense),
                    new Defense(EffectType.Fear, 0)
                };
            case EnemyType.Orc:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, 0),
                    new Defense(EffectType.Force, high_defense),
                    new Defense(EffectType.Fear, low_defense),
                    new Defense(EffectType.Transform, low_defense)
                };                
            case EnemyType.TestUnit:
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, 0),
                    new Defense(EffectType.Force, 0)                    
                };
            default:
             //   Debug.LogError("Getting enemy defenses for an unidentified type: " + type + "\n");
                return new List<Defense>
                {
                    new Defense(EffectType.Magic, 0),
                    new Defense(EffectType.Force, 0)
                };
        }


    }

    public static float getSpeed(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Magical:
                return 0f;
            case EnemyType.Drone:
                return 0f;
            case EnemyType.Moth:
                return 0f;
            case EnemyType.Soldier:
                return 0f;
            case EnemyType.TinyFly:
                return 0f;
            case EnemyType.FurFly:
                return 0f;
            case EnemyType.SuperFly:
                return 2.17f;
            case EnemyType.TinyPlane:
                return 2.03f;
            case EnemyType.Plane:
                return 2.69f;
            case EnemyType.Tank:
                return 1.47f;
            case EnemyType.TinyTank:
                return 1.17f;
            case EnemyType.Bird:
                return 1.17f;
            case EnemyType.SuperPlane:
                return 3.8f;
            case EnemyType.Ghost:
                return 0f;
            case EnemyType.Orc:
                return 0f;
            case EnemyType.Turtle:
                return 1.05f;
            case EnemyType.SturdyTank:
                return 1.03f;
            case EnemyType.Frog:
                return 1.53f;
            case EnemyType.ImpossibleTank:
                return 1.88f;
            case EnemyType.AssemblyPlane:
                return 1.7f;
            case EnemyType.TestUnit:
                return 0f;
            case EnemyType.MechSoldier:
                return 1.19f;
            default:
                return 0f;
        }
    }

    public static float defineMass(EnemyType type)
    {
        
        switch (type)
        {
            case EnemyType.Magical:
                return 3f;
            case EnemyType.Drone:
                return 3f;
            case EnemyType.Moth:
                return 4f;
            case EnemyType.Soldier:
                return 3f;
            case EnemyType.TinyFly:
                return 3f;
            case EnemyType.FurFly:
                return 4f;
            case EnemyType.SuperFly:
                return 6f;
            case EnemyType.TinyPlane:
                return 4f;
            case EnemyType.Plane:
                return 4.5f;
            case EnemyType.Tank:
                return 4.5f;
            case EnemyType.TinyTank:
                return 3.5f;
            case EnemyType.Bird:
                return 11f;
            case EnemyType.SuperPlane:
                return 6f;
            case EnemyType.Ghost:
                return 5f;
            case EnemyType.Orc:
                return 5f;
            case EnemyType.Turtle:
                return 7f;
            case EnemyType.SturdyTank:
                return 7f;
            case EnemyType.Frog:
                return 9f;
            case EnemyType.ImpossibleTank:
                return 8.5f;
            case EnemyType.AssemblyPlane:
                return 13f;
            case EnemyType.TestUnit:
                return 6f;
            case EnemyType.MechSoldier:
                return 5f;
            default:
                return 0f;
        }
    }



    public static List<Wish> getInventory(EnemyType type)
    {

        switch (type)
        {
            case EnemyType.Magical:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.8f),
                    new Wish(WishType.MoreXP,0.13f),
                    new Wish(WishType.MoreHealth,0.13f),
                    new Wish(WishType.MoreDamage,0.13f),
                    new Wish(WishType.MoreDreams,0.13f)
                };
            case EnemyType.Drone:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.02f)
                };
            case EnemyType.Moth:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.02f)
                };
            case EnemyType.Soldier:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.05f),
                    new Wish(WishType.MoreXP,0.03f)
                };
            case EnemyType.TinyFly:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.05f)
                };
            case EnemyType.FurFly:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.05f),
                    new Wish(WishType.MoreXP,0.02f)
                };
            case EnemyType.SuperFly:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.04f),
                    new Wish(WishType.MoreXP,0.02f),
                    new Wish(WishType.MoreDamage,0.032f),
                    new Wish(WishType.MoreHealth,0.022f)
                };
            case EnemyType.TinyPlane:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.01f),
                    new Wish(WishType.MoreXP,0.03f)
                };
            case EnemyType.Plane:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.06f),
                    new Wish(WishType.MoreXP,0.02f),
                    new Wish(WishType.MoreDreams,0.02f)
                };
            case EnemyType.Tank:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.05f),
                    new Wish(WishType.MoreXP,0.02f),
                    new Wish(WishType.MoreDamage,0.03f)
                };
            case EnemyType.TinyTank:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.05f),
                    new Wish(WishType.MoreXP,0.03f),
                    new Wish(WishType.MoreDamage,0.02f)
                };
            case EnemyType.Bird:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.05f),
                    new Wish(WishType.MoreXP,0.03f),
                    new Wish(WishType.MoreDamage,0.03f),
                    new Wish(WishType.MoreHealth,0.04f)
                };
            case EnemyType.SuperPlane:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.05f),
                    new Wish(WishType.MoreDreams,0.03f)
                };
            case EnemyType.Ghost:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.05f)
                };
            case EnemyType.Orc:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.05f),
                    new Wish(WishType.MoreXP,0.02f),
                    new Wish(WishType.MoreDreams,0.02f)
                };
            case EnemyType.Turtle:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.07f),
                    new Wish(WishType.MoreDreams,0.03f),
                    new Wish(WishType.MoreHealth,0.025f)
                };
            case EnemyType.SturdyTank:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.08f),
                    new Wish(WishType.MoreDreams,0.02f),
                    new Wish(WishType.MoreHealth,0.04f)
                };
            case EnemyType.Frog:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.08f),
                    new Wish(WishType.MoreDreams,0.02f),
                    new Wish(WishType.MoreHealth,0.03f),
                    new Wish(WishType.MoreDreams,0.05f)
                };
            case EnemyType.ImpossibleTank:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.1f),
                    new Wish(WishType.MoreXP,0.05f),
                    new Wish(WishType.MoreHealth,0.05f),
                    new Wish(WishType.MoreDamage,0.05f),
                    new Wish(WishType.MoreDreams,0.05f)
                };
            case EnemyType.AssemblyPlane:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.1f),
                    new Wish(WishType.MoreXP,0.05f),
                    new Wish(WishType.MoreHealth,0.05f),
                    new Wish(WishType.MoreDamage,0.05f),
                    new Wish(WishType.MoreDreams,0.05f)
                };
            case EnemyType.TestUnit:
                return new List<Wish>();
            case EnemyType.MechSoldier:
                return new List<Wish>
                {
                    new Wish(WishType.Sensible,0.05f),
                    new Wish(WishType.MoreXP,0.53f),
                    new Wish(WishType.MoreHealth,0.025f),
                    new Wish(WishType.MoreDamage,0.03f),
                    new Wish(WishType.MoreDreams,0.025f)
                };
            default:
                return new List<Wish>();
        }
    }

}