using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WishDial
{
    public WishType type;   
    public int count;
    public float adjustment;

    public WishDial(WishType _type, int _count)  //GLOBAL settings per wishtype
    {
        type = _type;        
        count = _count;       
    }

    public WishDial(WishType _type, int _count, float _adj)  //GLOBAL settings per wishtype
    {
        type = _type;
        count = _count;
        adjustment = _adj;
    }

    public WishDial()
    {
        type = WishType.Null;
        count = 0;
        adjustment = 0;
    }
    /*
    public float getSpawnAdjustment()
    {
        float percent = dial;

        if (level_based)
        {
            int overflow = cap - count;

            while (overflow > 0)
            {
                percent *= (1 - penalty);
                overflow--;
            }
        }
        return percent;
    }
    */

}
