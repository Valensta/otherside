using System.Collections.Generic;

public static class LevelStore
{


    public static List<List<LevelMod>> level_settings;


    public static List<LevelMod> getLevelSettings(int level)
    {
        if (level < level_settings.Count) return level_settings[level];
        return new List<LevelMod>
        {
            new LevelMod(Difficulty.Normal,0,0,0,false)
        };

    }



    //XP        DREAM        SENSIBLE       remove_cap      LULL     INTERVAL  (LULL IS NOT USED)
    public static void initSettings()
    {
        level_settings = new List<List<LevelMod>>
            {


                new List<LevelMod> //1
                {
                    new LevelMod(Difficulty.Normal, 0, 0.3f, -0.5f, false, 1.2f, 1.2f),
                    new LevelMod(Difficulty.Hard, 0, 0, 0, false)
                },

                new List<LevelMod> //2
                {
                    new LevelMod(Difficulty.Normal, 0.15f, 0, -0.5f, false, 1.15f, 1.15f),
                    new LevelMod(Difficulty.Hard, 0, -0.35f, 0, false),
                    new LevelMod(Difficulty.Insane, -0.10f, -0.40f, -0.10f, false, 0.85f, 0.85f)
                },

                new List<LevelMod> //3
                {
                    new LevelMod(Difficulty.Normal, 0.2f, -0.2f, -0.5f, false),
                    new LevelMod(Difficulty.Hard, 0, -0.4f, 0, false, 0.8f, 0.8f),
                    new LevelMod(Difficulty.Insane, -0.10f, -0.45f, -0.10f, false, 0.65f, 0.65f)
                },

                new List<LevelMod> //4
                {
                    new LevelMod(Difficulty.Normal, 0.35f, -0.15f, -0.5f, false, 1.15f, 1.15f),
                    new LevelMod(Difficulty.Hard, 0, -0.3f, 0, false, 0.8f, 0.8f),
                    new LevelMod(Difficulty.Insane, -0.15f, -0.50f, -0.2f, false, 0.65f, 0.65f)
                },
                new List<LevelMod> //5
                {
                    new LevelMod(Difficulty.Normal, 0.25f, -0.15f, -0.5f, false, 1.2f, 1.2f),
                    new LevelMod(Difficulty.Hard, 0, -0.4f, 0.2f, false),
                    new LevelMod(Difficulty.Insane, -0.15f, -0.45f, 0.05f, false, 0.8f, 0.8f)
                },
                new List<LevelMod> //6
                    //XP        DREAM        SENSIBLE       remove_cap      LULL     INTERVAL  (LULL IS NOT USED)
                    
                    {
                    new LevelMod(Difficulty.Normal, 0.2f, -0.2f, -0.5f, false, 1.3f, 1.3f),
                    new LevelMod(Difficulty.Hard, 0, -0.4f, 0.2f, false, 1.1f, 1.1f),
                    new LevelMod(Difficulty.Insane, -0.15f, -0.45f, 0.05f, false, 0.9f, 0.9f)
                },
                new List<LevelMod> //7
                {
                    new LevelMod(Difficulty.Normal, 0.25f, 0.10f, -0.5f, false, 1.3f, 1.3f),
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
            new LevelMod(Difficulty.Normal, 1.2f, 0.25f, -0.5f, false, 1.15f, 1.15f),
            new LevelMod(Difficulty.Hard, 0.7f, 0.1f, 0f, false, 0.9f, 0.9f),
            new LevelMod(Difficulty.Insane, 0.5f, 0f, 0f, false, 0.8f, 0.8f)
        }
                
    };

}

}