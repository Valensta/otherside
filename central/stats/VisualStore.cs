
using UnityEngine;

public static class VisualStore
{
    public static TimeOfDay[] times = new TimeOfDay[0];


    public static TimeOfDay getTimeOfDay(EnvType envType, TimeName timeName)
    {
        if (times.Length == 0) initSettings();

        foreach (TimeOfDay t in times)
        {
            if (t.equals(timeName, envType)) return t;
        }
        Debug.LogError($"Could not find a time of day for {envType} {timeName}\n!!");
        return new TimeOfDay(TimeName.Day, Color.white, Color.white, Color.white, EnvType.Forest);
    }



    //TimeName.Day         islandColor          _bg_color         _glowy_color         env_type
    public static void initSettings()
    {
        times = new TimeOfDay[9]
        {
            //FOREST
            new TimeOfDay(TimeName.Day,        new Color(1f, 1f, 1f, 90 / 255f), 
                                               Color.white, 
                                               Color.clear, EnvType.Forest),           
            new TimeOfDay(TimeName.Night,      new Color(219f / 255f, 219f / 255f, 219f / 255f, 90 / 255f),
                                               new Color(170f / 255f, 161f / 255f, 238f / 255f, 1f), 
                                                new Color(1f, 1f, 1f, 214f/255f),
                                                EnvType.Forest),
            new TimeOfDay(TimeName.Dawn, new Color(1f, 1f, 1f, 90 / 255f),
                                               new Color(198 / 255f, 196/ 255f, 253f / 234f, 1f), 
                                               new Color(1f, 1f, 1f, 151 / 255f), EnvType.Forest),
            
            //DESERT
            new TimeOfDay(TimeName.Day, new Color(1f, 1f, 1f, 181 / 255f), Color.white, Color.clear, EnvType.Desert),
            new TimeOfDay(TimeName.Night, new Color(1f, 1f, 1f, 181 / 255f),new Color(160 / 255f, 159/ 255f, 193/ 255f, 1f), Color.clear, EnvType.Desert),
            new TimeOfDay(TimeName.Dawn, new Color(1f, 1f, 1f, 181 / 255f), new Color(215 / 255f, 221 / 255f, 246f / 255f, 1f), Color.clear,
                EnvType.Desert),
            
                        
            //DARK FOREST
            new TimeOfDay(TimeName.Day,        new Color(1f, 1f, 1f, 179 / 255f), 
                Color.white, 
                Color.clear, EnvType.DarkForest),           
            new TimeOfDay(TimeName.Night,      new Color(219f / 255f, 219f / 255f, 219f / 255f, 163 / 255f),
                new Color(163f / 255f, 193f / 255f, 196f / 255f, 1f), 
                Color.white, EnvType.DarkForest),
            new TimeOfDay(TimeName.Dawn, new Color(1f, 1f, 1f, 184 / 255f),
                new Color(220 / 255f, 228 / 255f, 253f / 255f, 1f), 
                new Color(1f, 1f, 1f, 151 / 255f), EnvType.DarkForest)
        };
    

}

}