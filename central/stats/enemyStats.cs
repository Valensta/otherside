using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[System.Serializable]
public class enemyStats : IDeepCloneable<enemyStats>, IComparable
{    
    public string name;
    
    public int target = 0;    
    public int ammo = -1;
    public List<Wish> inventory = new List<Wish>();        
    public float remaining_xp;
    public float init_xp;
    public float point_factor;
    private EnemyType enemy_type;

    public float speed;//these are here for wave balancing
    public float mass;
    public List<Defense> defenses;

    public enemyStats(EnemyType type)
    {
        this.enemy_type = type;
        this.defenses = EnemyStore.getDefenses(type, true);
        this.mass = EnemyStore.getMass(type);
        this.name = type.ToString();
        this.inventory = EnemyStore.getInventory(type);
    }

    public EnemyType getEnemyType()
    {
        return enemy_type;
    }

    public float getAvgDefense()
    {
        return (defenses[0].strength + defenses[1].strength) / 2f;
    }

    public float getModifiedMass()
    {
        float modified = mass / (1 - getAvgDefense());

        if (name.Equals("Tank") || name.Equals("SturdyTank"))
        {
            enemyStats tiny_tank = Central.Instance.getEnemy("TinyTank");
            modified += tiny_tank.getModifiedMass();
        }

        if (name.Equals("SturdyTank"))
        {
            enemyStats tank = Central.Instance.getEnemy("Tank");
            modified += tank.getModifiedMass();
        }

        return modified;
    }
    
    public int CompareTo(string name)
    {
        return (this.name.Equals(name)) ? 0 : 1;
    }

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;
        enemyStats d = obj as enemyStats;
        return d.CompareTo(this);

    }

    public void returnXp(float _xp)
    {
        remaining_xp += _xp;    
    }
    public void SetXp(float _xp)
    {        
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
        else
        {
            float r = remaining_xp;
            remaining_xp = 0f;
            //	Debug.Log("(" + percent + ") Getting remaining XP: " + r + " out of " + init_xp + "\n");
            return r;
        }
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

    public enemyStats()
    {
    }



    object IDeepCloneable.DeepClone()
    {
        return this.DeepClone();
    }

    public enemyStats DeepClone()
    {
        enemyStats my_clone = new enemyStats();        
        my_clone.name = string.Copy(this.name);        
        my_clone.target = this.target;        
        my_clone.ammo = this.ammo;        
        my_clone.remaining_xp = this.remaining_xp;
        my_clone.init_xp = this.init_xp;        
        my_clone.inventory = CloneUtil.copyList(this.inventory);
        my_clone.mass = this.mass;
        my_clone.speed = this.speed;
        my_clone.defenses = CloneUtil.copyList(defenses);
        //my_clone.defense = this.defense;
        //my_clone.vf_defense = this.vf_defense;
        return my_clone;
    }

}
