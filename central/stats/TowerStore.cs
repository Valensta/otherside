using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

[System.Serializable]

public static class TowerStore
{


    public static Dictionary<EnemyType, List<Defense>> enemyDefenses;

    public static float low_defense = 0.25f;
    public static float lowmid_defense = 0.35f;
    public static float mid_defense = 0.50f;
    public static float high_defense = 0.75f;

    public static void initTowers()
    {
        
            
        Central.Instance.setUnitStats(getBasicStats(RuneType.Sensible, ToyType.Normal), false);
        Central.Instance.setUnitStats(getBasicStats(RuneType.Airy, ToyType.Normal), false);
        Central.Instance.setUnitStats(getBasicStats(RuneType.Vexing, ToyType.Normal), false);

        Central.Instance.setUnitStats(getBasicStats(RuneType.Sensible, ToyType.Hero), false);
        Central.Instance.setUnitStats(getBasicStats(RuneType.Airy, ToyType.Hero), false);
        Central.Instance.setUnitStats(getBasicStats(RuneType.Vexing, ToyType.Hero), false);

        Central.Instance.setUnitStats(getBasicStats(RuneType.SensibleCity, ToyType.Normal), false);
        Central.Instance.setUnitStats(getBasicStats(RuneType.Modulator, ToyType.Normal), false);

        Central.Instance.setUnitStats(getBasicStats(RuneType.Fast, ToyType.Temporary), false);
        Central.Instance.setUnitStats(getBasicStats(RuneType.Slow, ToyType.Temporary), false);
        Central.Instance.setUnitStats(getBasicStats(RuneType.Time, ToyType.Temporary), false);

        Central.Instance.setUnitStats(getBasicStats(RuneType.Castle, ToyType.Normal), false);


    }
    public static unitStats getBasicStats(RuneType runetype, ToyType toytype)
    
    {
        unitStats stats = null;
        switch (runetype)
        {

            case RuneType.Sensible:
                if (toytype != ToyType.Hero){
                    stats = new unitStats("sensible_tower", runetype, toytype);
                    stats.setCost(costType(runetype, toytype), 40);
                }else{
                    stats = new unitStats("sensible_tower_hero", runetype, toytype);
                    stats.setCost(costType(runetype, toytype), 1);
                }
                stats.island_type = IslandType.Permanent;

                break;


            case RuneType.Airy:
                if (toytype != ToyType.Hero) {
                    stats = new unitStats("airy_tower", runetype, toytype);
                    stats.setCost(costType(runetype, toytype), 30);
                }else{
                    stats = new unitStats("airy_tower_hero", runetype, toytype);
                    stats.setCost(costType(runetype, toytype), 1);
                }
                stats.island_type = IslandType.Permanent;
                break;
            case RuneType.Vexing:
                if (toytype != ToyType.Hero){
                    stats = new unitStats("vexing_tower", runetype, toytype);
                    stats.setCost(costType(runetype, toytype), 50);
                }else{
                    stats = new unitStats("vexing_tower_hero", runetype, toytype);
                    stats.setCost(costType(runetype, toytype), 1);
                }
                stats.island_type = IslandType.Permanent;
                break;

            case RuneType.Castle:
                stats = new unitStats("castle", runetype, toytype);
                stats.setCost(costType(runetype, toytype), 9999);
                stats.island_type = IslandType.Permanent;
                break;
            case RuneType.SensibleCity:
                stats = new unitStats("sensible_city", runetype, toytype);
                stats.setCost(costType(runetype, toytype), 20);
                stats.island_type = IslandType.Permanent;
                break;
            case RuneType.Modulator:
                stats = new unitStats("modulator", runetype, toytype);
                stats.setCost(costType(runetype, toytype), 5);
                stats.island_type = IslandType.Either;
                break;
            case RuneType.Fast:
                stats = new unitStats("sensible_tower_ghost", runetype, toytype);
                stats.setCost(costType(runetype, toytype), 2);
                stats.island_type = IslandType.Temporary;
                stats.required_building = "sensible_city";
                stats.ammo = 5;
                break;
            case RuneType.Slow:
                stats = new unitStats("slow_ghost", runetype, toytype);
                stats.setCost(costType(runetype, toytype), 2);
                stats.island_type = IslandType.Temporary;
                stats.required_building = "sensible_city";
                stats.ammo = 5;
                break;
            case RuneType.Time:
                stats = new unitStats("time_ghost", runetype, toytype);
                stats.setCost(costType(runetype, toytype), 2);
                stats.island_type = IslandType.Temporary;
                stats.required_building = "sensible_city";
                stats.ammo = 5;
                break;
        }
        stats.isUnlocked = false;
        /*

        { "name":"castle", "toy_type": "hero", "cost":"9999", "scale":{ "x":"2","z":"2"},"rune_type":"Castle"}
        { "name":"sensible_city", "toy_type": "normal", "cost":"20", "scale":{ "x":"2","z":"2"},"rune_type":"SensibleCity"}
        { "name":"sensible_tower",      "toy_type": "normal",   "cost":"40", "rune_type":"Sensible"}
        { "name":"airy_tower",          "toy_type": "normal",   "cost":"30", "rune_type":"Airy"}
        { "name":"vexing_tower",        "toy_type": "normal",   "cost":"50", "rune_type":"Vexing"}
        { "name":"sensible_tower_hero", "toy_type": "hero",     "cost":"1", "rune_type":"Sensible"}
        { "name":"airy_tower_hero",     "toy_type": "hero",     "cost":"1","rune_type":"Airy"}
        { "name":"vexing_tower_hero",   "toy_type": "hero",     "cost":"1","rune_type":"Vexing"}
        { "name":"sensible_tower_ghost","toy_type": "temporary","cost":"2","ammo":"5", "required_building":"sensible_city","islandtype":"temporary","rune_type":"Fast"}
        { "name":"slow_ghost",          "toy_type": "temporary","cost":"2","ammo":"5", "required_building":"sensible_city","islandtype":"temporary","rune_type":"Slow"}
        { "name":"time_ghost",          "toy_type": "temporary","cost":"2","ammo":"5","required_building":"sensible_city","islandtype":"temporary","rune_type":"Duration"}
        { "name":"modulator",           "toy_type": "normal"   ,"cost":"5","islandtype":"either","rune_type":"Modulator"}
        */
        
        return stats;


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



    public static CostType costType(RuneType rune_type, ToyType toy_type)
    {
        //{ Dreams, Wishes, SensibleHeroPoint, AiryHeroPoint, VexingHeroPoint, ScorePoint };

        if (toy_type == ToyType.Hero)
        {
            switch (rune_type)
            {
                case RuneType.SensibleCity:
                    return CostType.SensibleCityHeroPoint;
                case RuneType.Sensible:
                    return CostType.SensibleHeroPoint;
                case RuneType.Airy:
                    return CostType.AiryHeroPoint;
                case RuneType.Vexing:
                    return CostType.VexingHeroPoint;
                default:
                    Debug.Log("Invalid costtype for " + rune_type + " " + toy_type + " \n");
                    return CostType.Dreams;
            }
        }
        else if (toy_type == ToyType.Temporary) { return CostType.Wishes; }
        else return CostType.Dreams;
    }
    
    
    
    
    public static Sprite getImage(RuneType rune_type, ToyType toy_type)
    {
        //{ Dreams, Wishes, SensibleHeroPoint, AiryHeroPoint, VexingHeroPoint, ScorePoint };

        
            switch (rune_type)
            {
                case RuneType.SensibleCity:
                    return Get.getSprite("GUI/Toys/sensible_city");
                case RuneType.Sensible:

                if (toy_type == ToyType.Hero) return Get.getSprite("GUI/Toys/sensible_tower_hero");
                else return Get.getSprite("GUI/Toys/sensible_tower");

            case RuneType.Airy:
                if (toy_type == ToyType.Hero) return Get.getSprite("GUI/Toys/airy_tower_hero");
                else return Get.getSprite("GUI/Toys/airy_tower");

            case RuneType.Vexing:
                if (toy_type == ToyType.Hero) return Get.getSprite("GUI/Toys/vexing_tower_hero");
                else return Get.getSprite("GUI/Toys/vexing_tower");
            case RuneType.Slow:
                return Get.getSprite("GUI/Toys/slow_ghost");
            case RuneType.Fast:
                return Get.getSprite("GUI/Toys/sensible_tower_ghost");
            case RuneType.Time:
                return Get.getSprite("GUI/Toys/time_ghost");
            case RuneType.Modulator:
                return Get.getSprite("GUI/Toys/modulator");
            default:
                    return Get.getSprite("GUI/Toys/selected_island_image");

        }
        
    }

    //this is aweful but necessary for the support of too much old shit that I don't feel like refactoring/trawling through a ton of prefabs to fix
    public static string getBasicName(RuneType rune_type, ToyType toy_type)
    {
        

        switch (rune_type)
        {
            case RuneType.SensibleCity:
                return "sensible_city";
            case RuneType.Sensible:

                if (toy_type == ToyType.Hero) return "sensible_tower_hero";
                else return "sensible_tower";

            case RuneType.Airy:
                if (toy_type == ToyType.Hero) return "airy_tower_hero";
                else return "airy_tower";

            case RuneType.Vexing:
                if (toy_type == ToyType.Hero) return "vexing_tower_hero";
                else return "vexing_tower";
            case RuneType.Slow:
                return "slow_ghost";
            case RuneType.Fast:
                return "sensible_tower_ghost";
            case RuneType.Time:
                return "time_ghost";
            case RuneType.Modulator:
                return "modulator";
            default:
                return "castle";

        }

    }
}