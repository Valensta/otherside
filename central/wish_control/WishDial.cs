using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

[System.Serializable]
public class WishDial : IComparable, IDeepCloneable<WishDial>
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

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;
        WishDial d = obj as WishDial;
        if (d.type == this.type) return 0;
        return 1;
    }

    object IDeepCloneable.DeepClone()
    {
        return this.DeepClone();
    }

    public WishDial DeepClone()
    {
        WishDial my_clone = new WishDial();
        my_clone.type= this.type;
        my_clone.count= this.count;
        my_clone.adjustment = this.adjustment;
        

        return my_clone;
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
