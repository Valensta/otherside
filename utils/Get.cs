using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

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
    
    public static String savegame_location;// = Application.persistentDataPath + "/";  Set in Central Awake()
    public static String preloadgame_location;// = Application.persistentDataPath + "/";
    public static int midlevel_savegames = 3;
    public static float laser_damage_frequency = 0.1f;
    public static float lava_damage_frequency = 0.25f;
    public static float bullshit_damage_factor = 5f;
    public static float laser_damage_factor = 1/6f;
    public static float regenerator_repeat_rate = 0.25f;
    public delegate void onXpAddedHandler(float xp, Vector3 pos);
    public static event onXpAddedHandler onXpAdded;
    public static int hiddenMonsterLayer = 14;
    public static int flyingMonsterLayer = 19;
    public static int regularMonsterLayer = 12;
    public static int regularProjectileLayer = 15;
    public static int flyingProjectileLayer = 22;
    public static int overpassLayer = 23;
    public static float getDifficultyMult()
    {
        return 1.25f;
    }


    public static bool myDevice()
    {
        return (SystemInfo.deviceUniqueIdentifier.Equals("205aeb1e7c0f9311ea5ef76d0911faea") || 
                SystemInfo.deviceUniqueIdentifier.Equals("9c6df417c0613e8a12db58b92623287d866aaaeb"));
}
    public static float getModLavaFactor(float factor, float every_so_often, float lifespan, float range)
    {
        return factor * every_so_often / (lifespan * range);
    }

    public static string getDifficultyName(Difficulty diff)
    {
        switch (diff)
        {
            case Difficulty.Insane:
                return "Insane";
            case Difficulty.Hard:
                return "Normal";
            case Difficulty.Normal:
                return "Casual";
            default:
                return "Null";
        }
    }

    public static string getCarryOverInventory()
    {//special skills in inventory, wishes, potions
        StringBuilder sb = new StringBuilder();

        sb.Append("R:");

        foreach (Rune r in Central.Instance.hero_toy_stats)
        {
            StatSum sum = r.GetStats(false);
            foreach (StatBit s in sum.stats)
            {
                if (s.level <= 0) continue;
                sb.Append((int)s.effect_type);
                sb.Append("_");
                sb.Append(s.level);
                sb.Append(":");
            }                        
        }

        sb.Append("|S:");

        foreach (SpecialSkill skill in Peripheral.Instance.my_skillmaster.skills)
        {
            bool in_inv = false;
            foreach (SpecialSkillSaver saver in Peripheral.Instance.my_skillmaster.in_inventory)
                if (skill.type == saver.type) in_inv = true;

            if (skill.skill.level <= 0) continue;
            
            sb.Append((int)skill.type);
            sb.Append("_");
            sb.Append(skill.skill.level);
            sb.Append("_");
            sb.Append(in_inv);                
            sb.Append(":");
        }
        sb.Append("|W:");        
        foreach (Wishlet w in Peripheral.Instance.my_inventory.wishes)
        {
            
            sb.Append((int)w.my_wish.type);
            sb.Append("_");
            if (w.my_wish.type == WishType.Sensible) sb.Append(w.my_wish.strength); else sb.Append(w.my_wish.count);            
            sb.Append(":");
        }

        Debug.Log(sb.ToString() + "\n");
        return sb.ToString();
    }

    //skip forward if time diff > this
    public static float getSkipForwardGap(int level)
    {
        switch (level)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 6:
                return 6f;
            case 5:
                return 6f;
            default:
                return 3.5f;            
        }
        
    }

    



    public static float getPercent(float f)
    {
        return 1 - f / 100f;
    }

    public static float getRegeneratorModRate(float repeat_rate, float damage, float duration) {
      return repeat_rate* damage / duration;
    }

    public static bool SaveGameNow(int content)
    {
        //bool yes = (content == 0 || ((content + 1) % 3 == 0 && (Moon.Instance.waves.Count - content > 2)));
        bool yes = (content == 0 || ((content + 1) % 3 == 0));
        //     Debug.Log("SaveGameNow " + content + " ? " + yes + "\n");
        return yes;
    }

    public static EffectType GetDefenseType(EffectType type)
    {
        switch (type)
        {
            case EffectType.Null:
                return type;
            case EffectType.Teleport:
                return type;
            case EffectType.Fear:
                return type;
            case EffectType.Transform:
                return type;
            case EffectType.Range:
                return type;
            case EffectType.Magic:
            case EffectType.Speed:
            case EffectType.Weaken:
            case EffectType.WishCatcher:            
            case EffectType.Plague:
            case EffectType.EMP:            
            case EffectType.Frost:
            case EffectType.Critical:
            case EffectType.Calamity:
            case EffectType.Swarm:
//            case EffectType.Diffuse:
      //      case EffectType.VexingForce:
                return EffectType.Magic;
            default:
                return EffectType.Force;

        }
    }

    public static string getDifficultyText(Difficulty diff)
    {        

        return (diff == Difficulty.Normal || diff == Difficulty.Null) ? "" : getDifficultyName(diff);
        
    }

    

    public static float GetDefense(List<Defense> defenses, EffectType t)
    {
        float v = 0;
        EffectType getme = GetDefenseType(t);

        foreach (Defense d in defenses)
            if (d.type == getme) v = d.strength;

        return v;
    }

    public static float RandomNormal(){
		const float mean = 0;
		const float stdDev = 1;
		
		float u1 = UnityEngine.Random.Range (0, 1f);
		float u2 = UnityEngine.Random.Range (0, 1f);
		
		
		float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2); //random normal(0,1)
		float randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
		//Debug.Log ("random normal is " + randNormal);
		const float normal_range = 3f;	
		float thing = randNormal/normal_range; // now should be rougly -1 to 1
		if (thing > 1)thing = 1;
		if (thing < -1)thing = -1;
		//	Debug.Log ("random normal FINAL is " + thing);
		return thing;
	}

    public static Sprite getIslandSprite(EnvType env, IslandType island_type)
    {
        switch (env)
        {
            case EnvType.Desert:
                return (island_type == IslandType.Temporary) ? getSprite("Levels/Islands/desert_temporary_island") : getSprite("Levels/Islands/desert_permanent_island");
            case EnvType.Forest:
                return (island_type == IslandType.Temporary) ? getSprite("Levels/Islands/forest_temporary_island") : getSprite("Levels/Islands/forest_permanent_island");
            case EnvType.DarkForest:
                return (island_type == IslandType.Temporary) ? getSprite("Levels/Islands/dark_forest_temporary_island") : getSprite("Levels/Islands/dark_forest_permanent_island");           
            default:
                return (island_type == IslandType.Temporary) ? getSprite("Levels/Islands/forest_temporary_island") : getSprite("Levels/Islands/forest_permanent_island");

        }
    }


    public static void assignXP(float xp, int level, HitMe hitme, Firearm firearm, Vector3 pos, EffectType type)
    {
        if (xp <= 0)
        {
        //    Debug.LogError("WTF " + xp + " " + hitme.gameObject.name + "\n");
            return;
        }
        if (firearm == null) return;
        float return_xp = firearm.addXp(xp, true);//if tower is at max xp, return the xp,

        // if xp is from damage done, it is tied to health. Otherwise, 
        //if xp is from Speed/Teleport/Weaken etc, just assign the XP. this is handled by HitMe.stats.getXp though


        if (return_xp > 0) hitme.stats.returnXp(return_xp);
        float added = xp - return_xp;
        onXpAdded?.Invoke(added, pos);

        firearm.toy.my_tower_stats.addXp(type, level, xp);


    }
    
    public static Sprite getSprite(string name)
    {
        Sprite s = Resources.Load(name, typeof(Sprite)) as Sprite;
        if (s) return s;
        
        Debug.Log("Get cannot locate sprite " + name + "\n");
        s = Resources.Load("GUI/Inventory/empty_inventory_slot", typeof(Sprite)) as Sprite;
        return s;
    }

    public static PhysicsMaterial2D getPhysicsMaterial(string name)
    {
        PhysicsMaterial2D s = Resources.Load("Monsters/Physics Materials/" + name, typeof(PhysicsMaterial2D)) as PhysicsMaterial2D;
        if (s) return s;
        
        Debug.Log("Get cannot locate PhysicsMaterial2D " + name + "\n");
        s = Resources.Load("Monsters/Physics Materials/default_2d_material", typeof(PhysicsMaterial2D)) as PhysicsMaterial2D;
        return s;
    }

    public static bool isSpecial(EffectType type)
    {
        return (type == EffectType.Meteor || type == EffectType.AirAttack || type == EffectType.Frost || type == EffectType.Teleport 
            || type == EffectType.Bees || type == EffectType.EMP || type == EffectType.Plague || type == EffectType.Architect || type == EffectType.Renew);
    }

    public static bool isCastleSkill(EffectType type)
    {
        return (type == EffectType.Architect || type == EffectType.Renew);
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
        return !(Mathf.Abs(pos.y) > y_border) && !(Mathf.Abs(pos.x) > x_border);
	}

    public static Vector2 fixPosition(Vector2 pos)
    {
        const float inner_x_border = 0.5f;
        const float inner_y_border = 0.75f;
        
        const float outer_x_border = -0.5f;        
        const float outer_y_border = -0.75f;
       
    //    Debug.Log($"Fix position {old_pos} -> {pos}  .... max_x {Monitor.Instance.my_spyglass.max_x} max_y  {Monitor.Instance.my_spyglass.max_y}\n");
        
        if (Mathf.Abs(pos.y) > (Monitor.Instance.my_spyglass.map_y_size/2 + outer_y_border))    
            pos.y = Monitor.Instance.my_spyglass.map_y_size/2 - inner_y_border;

        
        if (Mathf.Abs(pos.x) > (Monitor.Instance.my_spyglass.map_x_size/2 + outer_x_border))
            pos.x = Monitor.Instance.my_spyglass.map_x_size/2 - inner_x_border;
   
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







    //is it possible to do a generic version of this?
    public static TimeName TimeNameFromString(string s)
    {



        foreach (TimeName val in Enum.GetValues(typeof(TimeName)))        
            if (string.Equals(val.ToString(), s, StringComparison.CurrentCultureIgnoreCase)) return val; 
               
        Debug.Log("Could not find timename from string: " + s + "\n");
        return TimeName.Day;
    }

    //this is slightly awkward but I want to keep Transform target for guided arrows
    public static Arrow MakeArrow(ArrowName type, Vector3 origin, Transform arrow_target, StatSum arrow_stats, float speed, Firearm _firearm, bool play_sound)
    {
        Arrow a = _MakeArrow(type, origin, arrow_stats, speed, _firearm, play_sound);
        a.InitArrow(arrow_stats, arrow_target, speed, _firearm);
        return a;
    }

    public static float getColliderSize(Collider2D collider)
    {
        if (collider is BoxCollider2D)
        {
            Vector2 size = ((BoxCollider2D)collider).size;
            return Mathf.Max(size.x, size.y);                
        }
        else if (collider is CircleCollider2D)
        {

            return ((CircleCollider2D)collider).radius;
        }
        return 0;
    }

    public static Arrow MakeArrow(ArrowName type, Vector3 origin, Vector3 arrow_target, StatSum arrow_stats, float speed, Firearm _firearm, bool play_sound)
    {
        Arrow a = _MakeArrow(type, origin, arrow_stats, speed, _firearm, play_sound);
        a.InitArrow(arrow_stats, arrow_target, speed, _firearm);
        return a;
    }

    private static Arrow _MakeArrow(ArrowName type, Vector3 origin, StatSum arrow_stats, float speed, Firearm _firearm, bool play_sound)
    {
        

        GameObject newMissile = Peripheral.Instance.zoo.getObject(type.name, false);
        // Debug.Log("Making arrow " + arrow_name + "\n");
        origin.z = 0f;
        newMissile.transform.position = origin;
        newMissile.transform.SetParent(Peripheral.Instance.arrows_transform);

        newMissile.tag = "PlayerArrow";

        Arrow arrow = newMissile.GetComponent<Arrow>();
        arrow.arrow_type = type.type;
        if (play_sound && Noisemaker.Instance != null) Noisemaker.Instance.Play("arrow_fired");
        
        return arrow;
    }

    


    public static float MaxLvl(EffectType type){

        return (isSpecial(type) && type != EffectType.EMP) ? 5 : 3;
		
	}
    
	public static bool isGeneric(EffectType type)
	{
	    return type == EffectType.Range || type == EffectType.ReloadTime;
	}


    public static string getLabelText(LabelText lt)
    {
        switch (lt)
        {
            case LabelText.PCT:
                return "EFF %";

            case LabelText.Null:
                return "";
            case LabelText.Duration:
                return "DUR";
            default:
                return lt.ToString();
        }
    }

	public static bool isBasic(EffectType type)
	{
	    return type == EffectType.Force || type == EffectType.Speed || type == EffectType.VexingForce;
	}

	
	public static Vector3 CoordFromMouseInput(Vector3 mouse){
		float x = (mouse.x - Screen.width / 2) / (Screen.width / 2);
		float y = (mouse.y - Screen.height / 2) / (Screen.height / 2);
		return new Vector3(x, y, 0);                                                
	}
	
	public static Vector3 MapCoordFromMouseInput(Vector3 mouse){
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (mouse);
		
		
		const int layerMask = 1 << 11;
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

