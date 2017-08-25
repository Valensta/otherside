using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public class TowerMaxLevel
{
    public ToyType toyType;
    public RuneType runeType;    
    public int maxLevel;

    public TowerMaxLevel(ToyType toyType, RuneType runeType, int maxLevel)
    {

        this.toyType = toyType;
        this.runeType = runeType;
        this.maxLevel = maxLevel;
    }
}

public static class LevelStore
{
    public static int getMaxLevel(int level, Difficulty difficulty, RuneType runeType, ToyType toyType)
    {

        //this is only relevant for between levels, and this is OK because the Castle doesn't have any level caps.
        difficulty = difficulty == Difficulty.Null ? Difficulty.Normal : difficulty;
        level = level == -1 ? 1 : level;
        
        
        LevelMod mod = getLevelMod(level,difficulty);
        if (mod.tower_max_level_settings == null && difficulty != Difficulty.Normal)        
            mod = getLevelMod(level, Difficulty.Normal);
        
        
        for (int i = 0; i < mod.tower_max_level_settings.Count; i++)
        {
            if (mod.tower_max_level_settings[i].runeType == runeType && mod.tower_max_level_settings[i].toyType == toyType)
                return mod.tower_max_level_settings[i].maxLevel;
        }

        if (difficulty != Difficulty.Normal)
            return getMaxLevel(level, Difficulty.Normal, runeType, toyType);
        
        Debug.Log($"LevelStore does not have a difficulty setting for level: {level} {difficulty} toy: {runeType} {toyType}\n");
        return 0;
    }
    

    public static List<List<LevelMod>> level_settings;

    public static LevelMod getLevelMod(int level,Difficulty difficulty)
    {
        //List<LevelMod> level_mod = LevelStore.getLevelSettings(Central.Instance.current_lvl);
        List<LevelMod> level_mod = getLevelSettings(level);
        foreach (LevelMod mod in level_mod)
        {
            if (difficulty == mod.difficulty) return mod;
        }
        //      Debug.LogWarning("Could not find a level mod for difficulty " + difficulty + "\n");
        //fallback - need better fallback or don't let people load a difficulty setting that's not defined for the level, duh
        foreach (LevelMod mod in level_mod)
        {
            if (mod.difficulty == Difficulty.Normal) return mod;
        }

        UnityEngine.Debug.LogError("Could not find a level mod!\n");
        return null;
    }

    public static List<LevelMod> getLevelSettings(int level)
    {
        if (level < level_settings.Count) return level_settings[level];
        return new List<LevelMod>
        {
            new LevelMod(Difficulty.Normal,0,0,0,false)
        };

    }



    //XP        DREAM        SENSIBLE       remove_cap      LULL     INTERVAL  (LULL IS NOT USED)       PERCENT_XP
    public static void initSettings()
    {
        Debug.Log("INITIALIZING LEVEL STORE!!!!!\n");
        level_settings = new List<List<LevelMod>>
            {


                new List<LevelMod> //1
                {
                    new LevelMod(Difficulty.Normal, 0, 0.3f, -0.4f, false, 1.2f, 1.2f, 5f),
                    new LevelMod(Difficulty.Hard, 0, 0, 0, false,1f, 1f, 7f)
                },

                new List<LevelMod> //2
                {
                    new LevelMod(Difficulty.Normal, 0.15f, 0, -0.4f, false, 1.15f, 1.15f),
                    new LevelMod(Difficulty.Hard, 0, -0.35f, 0, false, 1f, 1f),
                    new LevelMod(Difficulty.Insane, -0.10f, -0.40f, -0.10f, false, 0.85f, 0.85f)
                },

                new List<LevelMod> //3
                {
                    new LevelMod(Difficulty.Normal, 0.2f, -0.2f, -0.4f, false, 1f, 1f),
                    new LevelMod(Difficulty.Hard, 0, -0.4f, 0, false, 0.8f, 0.8f),
                    new LevelMod(Difficulty.Insane, -0.10f, -0.45f, -0.10f, false, 0.65f, 0.65f)
                },

                new List<LevelMod> //4
                {
                    new LevelMod(Difficulty.Normal, 0.35f, -0.15f, -0.4f, false, 1.15f, 1.15f),
                    new LevelMod(Difficulty.Hard, 0, -0.3f, 0, false, 0.8f, 0.8f),
                    new LevelMod(Difficulty.Insane, -0.15f, -0.50f, -0.2f, false, 0.65f, 0.65f)
                },
                new List<LevelMod> //5
                {
                    new LevelMod(Difficulty.Normal, 0.25f, -0.15f, -0.4f, false, 1.2f, 1.2f),
                    new LevelMod(Difficulty.Hard, 0, -0.4f, 0.2f, false),
                    new LevelMod(Difficulty.Insane, -0.15f, -0.45f, 0.05f, false, 0.8f, 0.8f)
                },
                new List<LevelMod> //6
                    //XP        DREAM        SENSIBLE       remove_cap      LULL     INTERVAL  (LULL IS NOT USED)
                    
                    {
                    new LevelMod(Difficulty.Normal, 0.25f, -0.15f, -0.4f, false, 1.4f, 1.4f),
                    new LevelMod(Difficulty.Hard, 0, -0.4f, 0.2f, false, 1.1f, 1.1f),
                    new LevelMod(Difficulty.Insane, -0.15f, -0.45f, 0.05f, false, 0.9f, 0.9f)
                },
                new List<LevelMod> //7
                {
                    new LevelMod(Difficulty.Normal, 0.25f, 0.10f, -0.4f, false, 1.3f, 1.3f),
                    new LevelMod(Difficulty.Hard, 0.1f, 0f, 0f, false, 1.1f, 1.1f),
                    new LevelMod(Difficulty.Insane, 0f, 0f, 0f, false, 1f, 1f)
                },
                new List<LevelMod> //8
                {
                    new LevelMod(Difficulty.Normal, 1.2f, 0, -0.25f, false, 1, 1),
                    new LevelMod(Difficulty.Hard, 1f, -0.2f, 0f, false, 0.85f, 0.85f),
                    new LevelMod(Difficulty.Insane, 0.8f, 0f, 0f, false, 0.75f, 0.75f)
               },                        
        new List<LevelMod> //9
        {
            new LevelMod(Difficulty.Normal, 1.2f, 0.25f, -0.4f, false, 1.15f, 1.15f),
            new LevelMod(Difficulty.Hard, 0.7f, 0.1f, 0f, false, 0.9f, 0.9f),
            new LevelMod(Difficulty.Insane, 0.5f, 0f, 0f, false, 0.8f, 0.8f)
        }
                
                
       
                
    };
        level_settings[0][0].tower_max_level_settings = new List<TowerMaxLevel>
        {
            new TowerMaxLevel(ToyType.Normal, RuneType.Sensible, 1),
            new TowerMaxLevel(ToyType.Normal, RuneType.Airy, 0),
            new TowerMaxLevel(ToyType.Normal, RuneType.Vexing, 0),
            new TowerMaxLevel(ToyType.Hero, RuneType.Castle, 1)            
        };
        level_settings[1][0].tower_max_level_settings = new List<TowerMaxLevel>
        {
            new TowerMaxLevel(ToyType.Hero, RuneType.Sensible, 1),
            new TowerMaxLevel(ToyType.Normal, RuneType.Sensible, 1),
            new TowerMaxLevel(ToyType.Normal, RuneType.Airy, 0),
            new TowerMaxLevel(ToyType.Normal, RuneType.Vexing, 0),
            new TowerMaxLevel(ToyType.Hero, RuneType.Castle, 1),
            new TowerMaxLevel(ToyType.Normal, RuneType.SensibleCity, 0)                       
        };
        level_settings[2][0].tower_max_level_settings = new List<TowerMaxLevel>
        {
            new TowerMaxLevel(ToyType.Hero, RuneType.Sensible, 2),
            new TowerMaxLevel(ToyType.Normal, RuneType.Sensible, 2),
            new TowerMaxLevel(ToyType.Hero, RuneType.Airy, 1),
            new TowerMaxLevel(ToyType.Normal, RuneType.Airy, 1),
            new TowerMaxLevel(ToyType.Normal, RuneType.Vexing, 0),
            new TowerMaxLevel(ToyType.Hero, RuneType.Castle, 1),
            new TowerMaxLevel(ToyType.Normal, RuneType.SensibleCity, 1)                       
        };    
        level_settings[3][0].tower_max_level_settings = new List<TowerMaxLevel>
        {
            new TowerMaxLevel(ToyType.Hero, RuneType.Sensible, 3),
            new TowerMaxLevel(ToyType.Normal, RuneType.Sensible, 3),
            new TowerMaxLevel(ToyType.Hero, RuneType.Airy, 2),
            new TowerMaxLevel(ToyType.Normal, RuneType.Airy, 2),
            new TowerMaxLevel(ToyType.Hero, RuneType.Vexing, 1),
            new TowerMaxLevel(ToyType.Normal, RuneType.Vexing, 1),
            new TowerMaxLevel(ToyType.Hero, RuneType.Castle, 1),
            new TowerMaxLevel(ToyType.Normal, RuneType.SensibleCity, 2)                       
        };
        level_settings[4][0].tower_max_level_settings = new List<TowerMaxLevel>
        {
            new TowerMaxLevel(ToyType.Hero, RuneType.Sensible, 4),
            new TowerMaxLevel(ToyType.Normal, RuneType.Sensible, 4),
            new TowerMaxLevel(ToyType.Hero, RuneType.Airy, 3),
            new TowerMaxLevel(ToyType.Normal, RuneType.Airy, 3),
            new TowerMaxLevel(ToyType.Hero, RuneType.Vexing, 2),
            new TowerMaxLevel(ToyType.Normal, RuneType.Vexing, 2),
            new TowerMaxLevel(ToyType.Hero, RuneType.Castle, 1),
            new TowerMaxLevel(ToyType.Normal, RuneType.SensibleCity, 3)                       
        };
        level_settings[5][0].tower_max_level_settings = new List<TowerMaxLevel>
        {
            new TowerMaxLevel(ToyType.Hero, RuneType.Sensible, 5),
            new TowerMaxLevel(ToyType.Normal, RuneType.Sensible, 5),
            new TowerMaxLevel(ToyType.Hero, RuneType.Airy, 4),
            new TowerMaxLevel(ToyType.Normal, RuneType.Airy, 4),
            new TowerMaxLevel(ToyType.Hero, RuneType.Vexing, 3),
            new TowerMaxLevel(ToyType.Normal, RuneType.Vexing, 3),
            new TowerMaxLevel(ToyType.Hero, RuneType.Castle, 1),
            new TowerMaxLevel(ToyType.Normal, RuneType.SensibleCity, 4)                       
        };
        level_settings[6][0].tower_max_level_settings = new List<TowerMaxLevel>
        {
            new TowerMaxLevel(ToyType.Hero, RuneType.Sensible, 6),
            new TowerMaxLevel(ToyType.Normal, RuneType.Sensible, 6),
            new TowerMaxLevel(ToyType.Hero, RuneType.Airy, 5),
            new TowerMaxLevel(ToyType.Normal, RuneType.Airy, 5),
            new TowerMaxLevel(ToyType.Hero, RuneType.Vexing, 4),
            new TowerMaxLevel(ToyType.Normal, RuneType.Vexing, 4),
            new TowerMaxLevel(ToyType.Hero, RuneType.Castle, 1),
            new TowerMaxLevel(ToyType.Normal, RuneType.SensibleCity, 5)                       
        };
        level_settings[7][0].tower_max_level_settings = new List<TowerMaxLevel>
        {
            new TowerMaxLevel(ToyType.Hero, RuneType.Sensible, 7),
            new TowerMaxLevel(ToyType.Normal, RuneType.Sensible, 7),
            new TowerMaxLevel(ToyType.Hero, RuneType.Airy, 6),
            new TowerMaxLevel(ToyType.Normal, RuneType.Airy, 6),
            new TowerMaxLevel(ToyType.Hero, RuneType.Vexing, 5),
            new TowerMaxLevel(ToyType.Normal, RuneType.Vexing, 5),
            new TowerMaxLevel(ToyType.Hero, RuneType.Castle, 1),
            new TowerMaxLevel(ToyType.Normal, RuneType.SensibleCity, 6)                       
        };
        level_settings[8][0].tower_max_level_settings = new List<TowerMaxLevel>
        {
            new TowerMaxLevel(ToyType.Hero, RuneType.Sensible, 8),
            new TowerMaxLevel(ToyType.Normal, RuneType.Sensible, 8),
            new TowerMaxLevel(ToyType.Hero, RuneType.Airy, 7),
            new TowerMaxLevel(ToyType.Normal, RuneType.Airy, 7),
            new TowerMaxLevel(ToyType.Hero, RuneType.Vexing, 6),
            new TowerMaxLevel(ToyType.Normal, RuneType.Vexing, 6),
            new TowerMaxLevel(ToyType.Hero, RuneType.Castle, 1),
            new TowerMaxLevel(ToyType.Normal, RuneType.SensibleCity, 7)                       
        };/*
        level_settings[9][0].tower_max_level_settings = new List<TowerMaxLevel>
        {
            new TowerMaxLevel(ToyType.Hero, RuneType.Sensible, 9),
            new TowerMaxLevel(ToyType.Normal, RuneType.Sensible, 9),
            new TowerMaxLevel(ToyType.Hero, RuneType.Airy, 8),
            new TowerMaxLevel(ToyType.Normal, RuneType.Airy, 8),
            new TowerMaxLevel(ToyType.Hero, RuneType.Vexing, 7),
            new TowerMaxLevel(ToyType.Normal, RuneType.Vexing, 7),
            new TowerMaxLevel(ToyType.Hero, RuneType.Castle, 1),
            new TowerMaxLevel(ToyType.Normal, RuneType.SensibleCity, 8)                       
        };*/
        
}

}