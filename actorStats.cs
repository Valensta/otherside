using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//I hate this class
[System.Serializable]
public class actorStats : IDeepCloneable<actorStats>
{
    public RuneType runetype = RuneType.Null;
    public ToyType toy_type;
    public string name;
    public int init_cost = 1;
    //public float dmg;
    public int target = 0;   
    public IslandType island_type;
    public int ammo = -1;
    public List<Wish> inventory = new List<Wish>();
    public bool friendly;
    public int max_lvl;
    public bool is_active = true;
    public Cost cost_type;
    public float remaining_xp;
    public float init_xp;
    public string required_building = "";

    public List<EffectType> exclude_skills = new List<EffectType>();
    [System.NonSerialized]
    private Vector3 scale = new Vector3();




    public int basic_cost = 20;

    public Vector3 getScale()
    {
        return scale;
    }

    public void returnXp(float _xp)
    {
        remaining_xp += _xp;
    //    Debug.Log("Returning " + _xp + " xp\n");
    }
    public void SetXp(float _xp)
    {
        //	Debug.Log("Setting xp to " + _xp + "\n");
        init_xp = _xp;
        remaining_xp = _xp;
    }

    public float getXp(float percent)
    {

        if (percent * init_xp < remaining_xp)
        {
            remaining_xp -= percent * init_xp;
            //	Debug.Log("(" + percent + ") Getting XP: " + percent*init_xp + " out of " + init_xp + "\n");
            return percent * init_xp;
        }
        else {
            float r = remaining_xp;
            remaining_xp = 0f;
            //	Debug.Log("(" + percent + ") Getting remaining XP: " + r + " out of " + init_xp + "\n");
            return r;
        }
    }

    public actorStats(string n, Vector3 sc, bool _friendly)
    {
        name = n;
        scale = sc;
        friendly = _friendly;
        setActive(true);
    }

    public bool isActive()
    {
        return is_active;
    }

    public void setActive(bool a)
    {
    //    Debug.Log("Setting toy active " + a + " " + name  + "\n");
        is_active = a;
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

    public actorStats GetActorStats()
    {
        return this;
    }

    public Wish getWish(WishType type)
    {
        foreach (Wish w in inventory)
        {
            if (w.type == type) return w;
        }
        return null;
    }

    public void addWish(Wish e)
    {
        Wish have = null;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].type == e.type)
            {
                inventory[i].Strength += e.Strength;
                return;
            }
        }
        inventory.Add(e);

    }

    public actorStats()
    {
    }

    object IDeepCloneable.DeepClone()
    {
        return this.DeepClone();
    }

    public actorStats DeepClone()
    {
        actorStats my_clone = new actorStats();
        my_clone.runetype = this.runetype;
        my_clone.toy_type = this.toy_type;
        my_clone.name = string.Copy(this.name);
        my_clone.init_cost = this.init_cost;
        my_clone.target = this.target;
        my_clone.island_type = this.island_type;
        my_clone.ammo = this.ammo;
        my_clone.friendly = this.friendly;
        my_clone.max_lvl = this.max_lvl;
        my_clone.is_active = this.is_active;
        my_clone.remaining_xp = this.remaining_xp;
        my_clone.init_xp = this.init_xp;
        my_clone.required_building = string.Copy(this.required_building);
        my_clone.cost_type = this.cost_type.clone();
        my_clone.inventory = CloneUtil.copyList(this.inventory);
        my_clone.exclude_skills = EnumUtil.copyEnumList<EffectType>(this.exclude_skills);

        //exclude list
        //inventory



        return my_clone;
    }

}

