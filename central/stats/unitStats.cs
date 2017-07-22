using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//I hate this class
[System.Serializable]
public class unitStats : IDeepCloneable<unitStats>, IComparable
{
    public ToyID toy_id = new ToyID();    
    public string name;
    public int init_cost = 1;  
    
    public IslandType island_type;
    public int ammo = -1;        
    public int max_lvl;  
    public bool is_unlocked = false;
    public Cost cost_type;    
    public string required_building = "";    

    public int CompareTo(RuneType runetype, ToyType toy_type)
    {
        return toy_id.CompareTo(runetype, toy_type);        
    }

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;
        unitStats d = obj as unitStats;
        return toy_id.CompareTo(d.toy_id);
               
    }



    public int basic_cost = 20;

    public bool isUnlocked
    {
        get
        {
            return is_unlocked;
        }

        set
        {
            is_unlocked = value;
        }
    }

   
    public void loadSnapshot(unitStatsSaver  load_me)
    {
        //if (load_me.name.Equals(this.name)
        this.isUnlocked = load_me.isUnlocked;        
    }

    public unitStatsSaver getSnapshot()
    {
        unitStatsSaver saver = new unitStatsSaver();
        saver.name = this.name;
        saver.toy_id = this.toy_id.DeepClone();
        saver.isUnlocked = this.isUnlocked;        
        saver.max_lvl = this.max_lvl;
        
        return saver;
    }

    public unitStats(string n)
    {
        name = n;     
    }
   
    public void setCost(CostType _cost_type, int _cost)
    {
        //   Debug.Log("Setting initian cost " + toy_type + " " + rune_type + " " + wish_type + " " + cost);
        cost_type = new Cost(_cost_type, _cost);
        init_cost = _cost;
    }



    //public void setDmg(float d) { dmg = d; }

    public int getMaxLvl()
    {
        return max_lvl;
    }

    public void setMaxLvl(int d) {
     //   Debug.Log("ACTORSTATS Setting max lvl to " + d + " for " + name + "\n");
        max_lvl = d;
    }

  //  public void setRange(float r) { range = r; }

    public unitStats GetActorStats()
    {
        return this;
    }
   

    public unitStats()
    {
    }

    public unitStats(string name, RuneType rune_type, ToyType toy_type)        
    {
        this.name = name;
        this.toy_id = new ToyID(rune_type, toy_type);
    }

    object IDeepCloneable.DeepClone()
    {
        return this.DeepClone();
    }

    public unitStats DeepClone()
    {
        unitStats my_clone = new unitStats();
        my_clone.toy_id = this.toy_id.DeepClone();        
        my_clone.name = string.Copy(this.name);
        my_clone.init_cost = this.init_cost;
        
        my_clone.island_type = this.island_type;
        my_clone.ammo = this.ammo;        
        my_clone.max_lvl = this.max_lvl;    
        my_clone.required_building = string.Copy(this.required_building);
        my_clone.cost_type = this.cost_type.clone();        

        //exclude list
        //inventory
        return my_clone;
    }

}

[System.Serializable]
public class unitStatsSaver : IComparable
{
    public string name;
    public bool is_unlocked;
    public ToyID toy_id = new ToyID();
    public int max_lvl;

    public bool isUnlocked
    {
        get
        {
            return is_unlocked;
        }

        set
        {
            is_unlocked = value;
        }
    }

    public unitStatsSaver()
    {
    }

    public int getMaxLvl()
    {
        return max_lvl;
    }

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;
        unitStats d = obj as unitStats;
        return toy_id.CompareTo(d.toy_id);
    }

    public void setMaxLvl(int d)
    {
        //   Debug.Log("ACTORSTATS Setting max lvl to " + d + " for " + name + "\n");
        max_lvl = d;
    }
}