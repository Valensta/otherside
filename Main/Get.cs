using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Int
{
    public int value;

    public Int(int v)
    {
        value = v;
    }
}

public class Float
{
    public float value;

    public Float(float v)
    {
        value = v;
    }
}


public static class Get {
    //	static int iTweenNumber = 0;
    public static String savegame_location = Application.persistentDataPath + "/";
    public static String preloadgame_location = Application.persistentDataPath + "/";


    public static float RandomNormal(){
		float mean = 0;
		float stdDev = 1;
		
		float u1 = UnityEngine.Random.Range (0, 1f);
		float u2 = UnityEngine.Random.Range (0, 1f);
		
		
		float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2); //random normal(0,1)
		float randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
		//Debug.Log ("random normal is " + randNormal);
		float normal_range = 3f;	
		float thing = randNormal/normal_range; // now should be rougly -1 to 1
		if (thing > 1)thing = 1;
		if (thing < -1)thing = -1;
		//	Debug.Log ("random normal FINAL is " + thing);
		return thing;
	}

    /*
    public static List<Defense> getDefenses(List<Defense> from)
    {
        List<Defense> copy = new List<Defense>();

        foreach (Defense d in from)
        {
            d.ValidateMe();
            copy.Add(new Defense(d.type, d.strength));
        }

        return copy;
    }*/

        


    public static Sprite getSprite(string name)
    {
        Sprite s = Resources.Load(name, typeof(Sprite)) as Sprite;
        if (!s) {
            Debug.Log("Get cannot locate sprite " + name + "\n");
            s = Resources.Load("GUI/Inventory/empty_inventory_slot", typeof(Sprite)) as Sprite;
        }
        return s;
    }

    public static PhysicsMaterial2D getPhysicsMaterial(string name)
    {
        PhysicsMaterial2D s = Resources.Load("Monsters/Physics Materials/" + name, typeof(PhysicsMaterial2D)) as PhysicsMaterial2D;
        if (!s)
        {
            Debug.Log("Get cannot locate PhysicsMaterial2D " + name + "\n");
            s = Resources.Load("Monsters/Physics Materials/default_2d_material", typeof(PhysicsMaterial2D)) as PhysicsMaterial2D;
        }
        return s;
    }

    public static bool isSpecial(EffectType type)
    {
        return (type == EffectType.AirAttack || type == EffectType.Quicksand || type == EffectType.Teleport || type == EffectType.Bees || type == EffectType.EMP || type == EffectType.Plague);
    }


    public static string NullToyName()
    {
        return "asdf";
    }

    public static int Factorial(int i)
    {
        int f = i;
        int me = i--;
        while (me > 1)
        {
            f *= me;
            me--;
        }
        return f;
    }

    public static float Round(float num, int place)
    {
        return Mathf.Round(num * (place+1) * 10f) / ((place+1)* 10f);
    }
  
	
   

	public static bool checkPosition(Vector2 pos){
        float outer_y_border = -1.5f;
        float outer_x_border = 0.5f;		
		float y_border = 15f/2f;
		float x_border = 12f;

        y_border = EagleEyes.Instance.getMapYSize()/2f;
        x_border = EagleEyes.Instance.getMapXSize();
 // Debug.Log("Checking " + pos + " vs " + x_border + ", " + y_border + "\n");
        if (Mathf.Abs(pos.y) > y_border || Mathf.Abs(pos.x) > x_border)
        {
            return false;
        }
        return true;   
    }

    public static Vector2 fixPosition(Vector2 pos)
    {
        float inner_x_border = 0.5f;
        float inner_y_border = -1.5f;
        float outer_x_border = 0.5f;        
        float outer_y_border = -1.5f;
        if (Mathf.Abs(pos.y) > (Camera.main.orthographicSize + outer_y_border))    
        {
            pos.y = (Camera.main.orthographicSize * pos.y / Mathf.Abs(pos.y) - inner_y_border);
        }

        if (Mathf.Abs(pos.x) > (Camera.main.orthographicSize * Screen.width / Screen.height + outer_x_border))
        {
            pos.x = ((Camera.main.orthographicSize * Screen.width / Screen.height ) * pos.x / Mathf.Abs(pos.x) - inner_x_border);        
        }
        return pos;
    }
	
	public static Vector2 GetDirection(float angle, float magnitude){
		return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * magnitude;
	}

    public static float GetAngle(Vector3 dir)
    {        
        return Mathf.Atan(dir.y / dir.x);
               
    }

    // public Enum EnumFromString


    public static WishType WishTypeFromString(string s)
    {
        foreach (WishType val in Enum.GetValues(typeof(WishType)))        
            if (val.ToString().ToLower() == s.ToLower())           
                return val;
         
        Debug.Log("Could not find wishtype from string: " + s + "\n");
        return WishType.Sensible;
    }


    public static CostType costType(RuneType rune_type, ToyType toy_type)
    {
        //{ Dreams, Wishes, SensibleHeroPoint, AiryHeroPoint, VexingHeroPoint, ScorePoint };

        if (toy_type == ToyType.Hero)
        {
            switch (rune_type)
            {
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
        /*
        foreach (CostType val in Enum.GetValues(typeof(CostType)))
            if (val.ToString().ToLower() == s.ToLower())
                return val;

        Debug.Log("Could not find CostType from string: " + s + "\n");
        return CostType.Dreams;*/
    }




    //is it possible to do a generic version of this?
    public static TimeName TimeNameFromString(string s)
    {



        foreach (TimeName val in Enum.GetValues(typeof(TimeName)))        
            if (val.ToString().ToLower() == s.ToLower()) return val; 
               
        Debug.Log("Could not find timename from string: " + s + "\n");
        return TimeName.Day;
    }

    //this is slightly awkward but I want to keep Transform target for guided arrows
    public static Arrow MakeArrow(string arrow_name, Vector3 origin, Transform arrow_target, StatSum arrow_stats, float speed, Firearm _firearm, bool play_sound)
    {
        Arrow a = _MakeArrow(arrow_name, origin, arrow_stats, speed, _firearm, play_sound);
        a.InitArrow(arrow_stats, arrow_target, speed, _firearm);
        return a;
    }

    public static Arrow MakeArrow(string arrow_name, Vector3 origin, Vector3 arrow_target, StatSum arrow_stats, float speed, Firearm _firearm, bool play_sound)
    {
        Arrow a = _MakeArrow(arrow_name, origin, arrow_stats, speed, _firearm, play_sound);
        a.InitArrow(arrow_stats, arrow_target, speed, _firearm);
        return a;
    }

    private static Arrow _MakeArrow(string arrow_name, Vector3 origin, StatSum arrow_stats, float speed, Firearm _firearm, bool play_sound)
    {
        GameObject newMissile = Peripheral.Instance.zoo.getObject(arrow_name, false);
       // Debug.Log("Making arrow " + arrow_name + "\n");
        newMissile.transform.position = origin;
        newMissile.transform.parent = Peripheral.Instance.arrows_transform;

        newMissile.tag = "PlayerArrow";

        Arrow arrow = newMissile.GetComponent<Arrow>();
        if (play_sound && Noisemaker.Instance != null) Noisemaker.Instance.Play("arrow_fired");
        
        return arrow;
    }

    public static ToyType ToyTypeFromString(string s)
    {
        foreach (ToyType val in Enum.GetValues(typeof(ToyType)))
            if (val.ToString().ToLower() == s.ToLower()) return val;

        Debug.Log("Could not find ToyType from string: " + s + "\n");
        return ToyType.Null;
    }

    public static RuneType RuneTypeFromString(string s)
    {
        if (s == null || s.Equals("")) return RuneType.Null;
        foreach (RuneType val in Enum.GetValues(typeof(RuneType)))
            if (val.ToString().ToLower() == s.ToLower()) return val;

        Debug.Log("Could not find ToyType from string: " + s + "\n");
        return RuneType.Null;
    }
    
    
    public static EffectType EffectTypeFromString(string s)
    {      
        foreach (EffectType val in Enum.GetValues(typeof(EffectType)))
            if (val.ToString().ToLower() == s.ToLower())     
                return val;
        
        Debug.Log("Could not find effecttype from string: " + s + "\n");
        return EffectType.Null;
    }

    public static IslandType IslandTypeFromString(string s)
    {
        foreach (IslandType val in Enum.GetValues(typeof(IslandType)))
            if (val.ToString().ToLower() == s.ToLower())
                return val;

        Debug.Log("Could not find IslandType from string: " + s + "\n");
        return IslandType.Permanent;
    }

    
    /*
    public static List<Wish> copyWishTypeList(List<Wish> list)
    {
        List<Wish> new_list = new List<Wish>();

        for (int i = 0; i < list.Count; i++)
        {
            Wish w = list[i].DeepClone();            
        }

        return new_list;
    }
    */
    


    public static float MaxLvl(EffectType type){
		switch (type) {
		case EffectType.Speed:
			return 3f;	
		case EffectType.Mass:
			return 3f;
		case EffectType.RapidFire:
			return 3f;
		case EffectType.Stun:
			return 3f;
		default:
			return 3f;
		}
	}
    
	public static bool isGeneric(EffectType type){
		if (type == EffectType.Range || type == EffectType.ReloadTime) {
			return true;
		}
		return false;
	}
  
    

	public static bool isBasic(EffectType type){
		if (type == EffectType.Force || type == EffectType.Speed) {
			return true;
		}
		return false;
	}

	
	public static Vector3 CoordFromMouseInput(Vector3 mouse){
		float x = (mouse.x - Screen.width / 2) / (Screen.width / 2);
		float y = (mouse.y - Screen.height / 2) / (Screen.height / 2);
		return new Vector3(x, y, 0);                                                
	}
	
	public static Vector3 MapCoordFromMouseInput(Vector3 mouse){
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (mouse);
		
		
		int layerMask = 1 << 11;
		if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) {
			mouse.z = Vector3.Distance (Camera.main.transform.position, hit.point);// - 10f;
			//	Debug.Log ("z is " + mouse.z);
			Vector3 loc = Camera.main.ScreenToWorldPoint (mouse);
			return loc;
		} else {
			return Vector3.zero;
		}
	}
	
	
}

