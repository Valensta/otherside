using UnityEngine;
using System;

[System.Serializable]
public class Wish : IDeepCloneable<Wish> {


    public string name;
	public WishType type;
    public float strength;
    public int count;
    public float percent; //spawn %
    public bool absolute; //strength is in percent or absolute #

	public float max_strength = 100;

    public float Strength
    {
        get
        {
            return strength;
        }

        set
        {
            strength = value;
        }
    }

    public int Count
    {
        get
        {
            return count;
        }

        set
        {
            count = value;
        }
    }

    //public Color color;

    public float getEffect()
    {
        //Debug.Log("Getstrength " + _s + " " + type + "\n");
        switch (type)
        {
            case WishType.Sensible:
                return 1f*strength;
            case WishType.MoreXP:
                return 0.25f * strength;
            case WishType.MoreHealth:
                return 1f * strength;
            case WishType.MoreDreams:
                return 0.25f * strength;
            case WishType.MoreDamage:
                return 0.25f * strength;
            default:
                return 0.25f * strength;
        }

    }

	public float getTime()
    {
        switch (type)
        {
            case WishType.MoreXP:
                return Mathf.Floor(Strength * 75/4f);
            case WishType.MoreHealth:
                return 0f;
            case WishType.MoreDreams:
                return Mathf.Floor(Strength * 75/4f);
            case WishType.MoreDamage:
                return Mathf.Floor(Strength * 66.6664f/4);
            default:
                return Mathf.Floor(Strength * 50f/4);
        }        
    }

	public void setName(string n){
		
		name = n;
	}

	public void addStrength(float s){

		Strength += s;
	}
    
	public void setPercent(float p){
		percent = p;
	}

	public Wish (WishType t, float _strength, string n){
		type = t;
        Strength = _strength; 
        percent = 0;		
		name = n;
	}

	public Wish (string t){        
        type = Get.WishTypeFromString(t);
        Strength = 1;
        percent = 0;		
	}

	public Wish (WishType _type, float _percent){
		type = _type;
        Strength = 1;
        percent = _percent;
		
	}

    public Wish(WishType _type, float _strength, float _percent)
    {
        type = _type;
        Strength = _strength;
        percent = _percent;

    }

    public void setAbs()
    {
        absolute = true;
        if (type == WishType.MoreDreams || type == WishType.Sensible) absolute = false;
    }

	public Wish (){
	}

    object IDeepCloneable.DeepClone()
    {
        return this.DeepClone();
    }

    public Wish DeepClone()
    {
        Wish f = new Wish();
        f.name = (this.name == null)? null : string.Copy(this.name);
        f.type = this.type;
        f.Strength = this.Strength;
        f.percent = this.percent;
        f.absolute = this.absolute;
        f.Count = this.Count;
        return f;
    }


}



