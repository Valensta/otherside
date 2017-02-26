using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;

	public class Loader : MonoBehaviour {
	private string level;
	bool waypoint = false;
	public Vector3 invalid = new Vector3 (999, 999, 999);
	Queue<string> file;

    



	public static Loader Instance { get; private set; }


	//%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%        	AWAKE      
	void Awake(){
		Instance = this;


	}

	public void setFile(Queue<string> f){
		file = f;
	}

   
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%              LOAD FILE
    public void oldLoadLevel()
    {
        Debug.LogError("!!!!!!USING DEPRICATED LOADLEVEL. STOP!!!!!");
        return;
        string line;
        //	Debug.Break ();
        line = file.Dequeue();
        line = line.TrimStart(':');
        line = line.TrimEnd('\r', '\n');
        Peripheral.Instance.title = line;//first line is always the title
        line = file.Dequeue();

        int count = 0;
        line = line.TrimEnd('\r', '\n');
        while (line != null && !line.Equals(":"))
        {
            switch (line)
            {

                case ":toys":
                    line = oldLoadActors("Toys");
                    break;
                case ":monsters":
                    line = oldLoadActors("Monsters");
                    break;
                case ":waves":
                    line = oldLoadWaves();
                    break;
                case ":wishes":
                    line = oldLoadWishes();
                    break;
                case ":init_stats":
                    line = oldLoadStats(false);
                    break;
                case ":":
                    break;
                default:
                    line = file.Dequeue();
                    break;      
            }
            line = line.TrimEnd('\r', '\n');
        }
        count++;

        Moon.Instance.CalculateWishes();
    }
	//%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%        	LOAD STATS
	


	public string oldLoadStats(bool init){
		string line = file.Dequeue();
	//	Debug.Log("Loading init stats\n");
		while (line != null && !line[0].Equals (':')) {		
			JSONParser p = new JSONParser (line);
			p.parse ();

			JSONObject points = new JSONObject ();
			p.root.TryGetValue ("points", out points);
			Central.Instance.points = int.Parse(points.value);

			if (!init){
				JSONObject health = new JSONObject ();
				JSONObject dreams = new JSONObject ();
				JSONObject max_dreams = new JSONObject ();			
				JSONObject type = new JSONObject ();
				JSONObject tilesize = new JSONObject ();
                JSONObject mapsize = new JSONObject();

                    JSONObject pathf = new JSONObject ();
				p.root.TryGetValue ("dreams", out dreams);
				p.root.TryGetValue ("tilesize", out tilesize);
				p.root.TryGetValue ("health", out health);			
				p.root.TryGetValue ("max_dreams", out max_dreams);			
				p.root.TryGetValue ("pathf", out pathf);
				p.root.TryGetValue ("type", out type);
                p.root.TryGetValue("map_size", out mapsize);

                float dreams_points = int.Parse(dreams.value);
                
                

                Peripheral.Instance.addDreams(dreams_points, Vector3.zero, false);
                //ScoreKeeper.Instance.SetDreams(dreams_points);
				Peripheral.Instance.max_dreams = int.Parse(max_dreams.value);
				PathfinderType PathType = PathfinderType.GridBased;
				Peripheral.Instance.SetHealth(int.Parse(health.value));
				Peripheral.Instance.pathf = PathType;				
				Peripheral.Instance.tileSize = int.Parse(tilesize.value);
                if (mapsize != null)
                {
                    EagleEyes.Instance.setMapSize(int.Parse(mapsize.dict["x"].value), int.Parse(mapsize.dict["y"].value));
                }
                else
                {
                    Debug.Log("WTF no map size\n");
                }
            }
			line = file.Dequeue();			
		}
		return line;
	}


    public string oldLoadWishes()
    {
        string line = file.Dequeue();
        WishDial[] dials = new WishDial[Enum.GetValues(typeof(WishType)).Length];
        int current = 0;

        while (line != null && !line[0].Equals(':'))
        {
            if (current < dials.Length)
            {
                JSONParser p = new JSONParser(line);
                p.parse();

                JSONObject wishtype = new JSONObject();
                JSONObject count = new JSONObject();
                p.root.TryGetValue("wishtype", out wishtype);
                p.root.TryGetValue("count", out count);

                if (wishtype == null || count == null) continue;
                int c = int.Parse(count.value);
                WishType type = Get.WishTypeFromString(wishtype.value);
                dials[current] = new WishDial(type, c);
                current++;
            }
            line = file.Dequeue();
        }
        Moon.Instance.SetWishDials(dials);
        return line;

    }

//%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%        	LOAD WAVES

public string oldLoadWaves()
    {
        Debug.LogError("USE THE NEW FANCY LOADER FOR WAVES\n");
        return "";/*
        string line = file.Dequeue();
        while (line != null && !line[0].Equals(':'))
        {

            Debug.Log(line + "\n");
            JSONParser p = new JSONParser(line);
            p.parse();

            JSONObject nm = new JSONObject();
            JSONObject points = new JSONObject();
            JSONObject xp = new JSONObject();

            JSONObject monster_interval = new JSONObject();
            JSONObject lull_length = new JSONObject();
            JSONObject wait_time = new JSONObject();
            p.root.TryGetValue("wait_time", out monster_interval);
            JSONObject mode = new JSONObject();
            p.root.TryGetValue("points", out points);
            p.root.TryGetValue("xp", out xp);
            p.root.TryGetValue("mode", out mode);

            JSONObject time_start = new JSONObject();
            JSONObject time_end = new JSONObject();
            JSONObject time_change = new JSONObject();

            p.root.TryGetValue("time_start", out time_start);
            p.root.TryGetValue("time_end", out time_end);
            p.root.TryGetValue("time_change", out time_change);

            List<int> pathlist = new List<int>();
            switch (mode.value)
            {
                case "random":
                    JSONObject mon = new JSONObject();
                    p.root.TryGetValue("interval", out monster_interval);

                    p.root.TryGetValue("lull", out lull_length);
                    JSONObject wavelet_size = new JSONObject();

                    p.root.TryGetValue("nightmares", out nm);
                    p.root.TryGetValue("monsters", out mon);

                    p.root.TryGetValue("wavelet_size", out wavelet_size);

                    string[] monlist = (mon.value).Split(',');

                    List<string> monsters = new List<string>();
                    foreach (string a in monlist)
                    {
                        monsters.Add(a);
                    }


                    wave hey = new wave(int.Parse(nm.value), monsters,
                                        float.Parse(monster_interval.value),
                                        float.Parse(lull_length.value), int.Parse(wavelet_size.value), pathlist);
                    if (points != null) { hey.points = int.Parse(points.value); }
                    if (xp != null) { hey.xp = int.Parse(xp.value); }

                    if (wait_time.value != null)
                    {
                        hey.wait = int.Parse(wait_time.value);
                    }

                    Moon.Instance.AddWave(hey);
                    break;
                case "list":
                    //"1":{"monster_invertal":"0.5", "lull_length":"3","list":{"forceball,5,scaleball,3"}}
                    JSONObject wavelet_num = new JSONObject();
                    JSONObject list = new JSONObject();
                    int i = 1;
                    wave mywave = new wave();
                    if (points != null) { mywave.points = int.Parse(points.value); }
                    if (xp != null) { mywave.xp = int.Parse(xp.value); }
                    p.root.TryGetValue(i.ToString(), out wavelet_num);
                    
                    while (wavelet_num != null)
                    {
                     //   Debug.Log("Trying " + i + "\n");
                        
                        if (wavelet_num.dict.ContainsKey("paths") == true)
                        {
                            string[] temp;

                            temp = (wavelet_num.dict["paths"].value).Split(',');
                            foreach (string x in temp) pathlist.Add(int.Parse(x));
                        }

                        monster_interval = wavelet_num.dict["interval"];
                        lull_length = wavelet_num.dict["lull"];
                        list = wavelet_num.dict["list"];

                        string[] splitme = (list.value).Split(',');
                        List<string> LIST = new List<string>();
                     //   Debug.Log("splitme " + list.value + "\n");
                        foreach (string a in splitme)
                        {
                            LIST.Add(a);
                     //       Debug.Log("list " + a + "\n");
                        }
                        wavelet hi = new wavelet(float.Parse(monster_interval.value), float.Parse(lull_length.value), LIST, pathlist);
                        mywave.add_wavelet(hi);
                        i++;
                        p.root.TryGetValue(i.ToString(), out wavelet_num);
                    }



                    if (wait_time.value != null)
                    {
                        mywave.wait = int.Parse(wait_time.value);
                    }
                    //if (retry != null)mywave.retry = true;

                    if (time_change != null && time_end != null && time_start != null)
                    {
                        //		Debug.Log("Got a wave times\n");
                        mywave.SetTime(Get.TimeNameFromString(time_start.value), Get.TimeNameFromString(time_end.value), float.Parse(time_change.value));
                    }
                    else if (time_start != null)
                    {
                        mywave.SetStartTime(Get.TimeNameFromString(time_start.value));
                    } else {
                        mywave.SetStartTime(TimeName.Day);
                        Debug.Log("WAVE missing start time! Assuming Day\n");
                    }

                    mywave.adjust_total_run_time();
                    Moon.Instance.AddWave(mywave);

                    break;
                default:
                    break;
            }


            line = file.Dequeue();
        }
        return line;
        */
    }
    


	public void loadRunes(JSONObject e, ref unitStats stats){
		
		string[] efs;
		efs = (e.dict["type"].value).Split (',');
		string[] strengths;
		strengths = (e.dict["strength"].value).Split (',');
		string[] percents;
		percents = (e.dict["percent"].value).Split (',');
		
		int length = efs.Length;
		for (int i = 0; i < length; i++) {
			if (strengths.Length < i){
				strengths[i] = "0";
			}
			if (percents.Length < i){
				percents[i] = "0";
			}
			Wish enew;
			if (Get.WishTypeFromString(efs[i]) != WishType.Null && float.Parse(percents[i]) > 0 ){
				enew = new Wish(Get.WishTypeFromString(efs[i]), float.Parse(strengths[i]));
				//enew.setStrength(float.Parse(strengths[i]));
				enew.setPercent(float.Parse(percents[i]));
				stats.addWish(enew);
			}
            else
            {
                Debug.Log("Bad wish " + efs[i] + " percent " + percents[i] + "\n");
            }
            				
		}
		
	}
	

	
	
	public string LoadAllActors(string what){
		string line = file.Dequeue();
		while (line != null && !line[0].Equals (':')) {
			LoadActor(line, what);
			line = file.Dequeue();
		}
		return line;
	}

    public void LoadInitFile()
    {
        string line;
        //	Debug.Log("Loading init file\n");
        line = file.Dequeue();
        line = line.TrimStart(':');
        line = line.TrimEnd('\r', '\n');
        line = file.Dequeue();
        int count = 0;
        line = line.TrimEnd('\r', '\n');
        while (line != null && !line.Equals(":"))
        {
            switch (line)
            {

                case ":toys":
                    line = LoadAllActors("Toys");
                    break;
                case ":monsters":
                    line = LoadAllActors("Monsters");
                    break;
                case ":init_stats":
                    line = oldLoadStats(true);
                    break;
                case ":":
                    break;
                default:
                    line = file.Dequeue();
                    break;
            }
            line = line.TrimEnd('\r', '\n');
        }
        count++;
    }


	public unitStats LoadActor(string line, string what){
      //  Debug.Log("Loading actor\n");
		JSONObject pname = new JSONObject ();
		JSONObject pcost = new JSONObject ();
        JSONObject ptoytype = new JSONObject();
        JSONObject pcost_runetype = new JSONObject ();
        JSONObject prune_type = new JSONObject();
        JSONObject pcost_wishtype = new JSONObject();
        JSONObject pscale = new JSONObject ();		
		JSONParser p = new JSONParser (line);
		JSONObject parrow = new JSONObject ();
		JSONObject prunes = new JSONObject ();
        JSONObject ppermanent = new JSONObject();
        JSONObject pammo = new JSONObject ();		
		JSONObject punlock_now= new JSONObject ();
		JSONObject inv = new JSONObject ();
		JSONObject pmax_lvl = new JSONObject ();
        JSONObject exclude_skills = new JSONObject();
        JSONObject required_building = new JSONObject();

        p.parse ();
		p.root.TryGetValue ("name", out pname);
		
		p.root.TryGetValue ("cost", out pcost);
        
        p.root.TryGetValue("toy_type", out ptoytype);        
        p.root.TryGetValue("rune_type", out prune_type);
        p.root.TryGetValue("cost_wishtype", out pcost_wishtype);
        p.root.TryGetValue ("scale", out pscale);
		p.root.TryGetValue ("max_lvl", out pmax_lvl);
		p.root.TryGetValue ("unlock_now", out punlock_now);
	
		p.root.TryGetValue ("arrow", out parrow);
		p.root.TryGetValue ("ammo", out pammo);
		p.root.TryGetValue ("inventory", out inv);
		p.root.TryGetValue ("runes", out prunes);
        p.root.TryGetValue("islandtype", out ppermanent);
        p.root.TryGetValue("exclude_skills", out exclude_skills);
        p.root.TryGetValue("required_building", out required_building);
        unitStats test = Central.Instance.getToy(pname.value);


        if (exclude_skills != null)
        {
            List<EffectType> exclude_list = new List<EffectType>();
            string[] string_list = (exclude_skills.value).Split(',');
            foreach (string s in string_list)
            {
         //       Debug.Log("Getting skill " + s + "\n");
                EffectType skill = Get.EffectTypeFromString(s);
                if (skill != EffectType.Null)
                {
                    exclude_list.Add(skill);
                }
            }
            test.exclude_skills = exclude_list;
     //       Debug.Log("Exclude list has " + exclude_list.Count + " items for " + pname.value);
        }

        if (test != null) {
            if (pmax_lvl != null)
            {
                
                test.setMaxLvl(int.Parse(pmax_lvl.value));
            }
			if (punlock_now != null){
				if (punlock_now.value == "false") {test.isUnlocked = false;} else {test.isUnlocked = true;}
			}
        //    Debug.Log("Loading actor again\n");
			//thing has already been defined in a previous level. do not load most stats again

            return test;
		} else {
	
			int cost = int.Parse (pcost.value);
			Vector3 scaleV = new Vector3 (1, 1, 1);
			
			if (pscale != null) scaleV = Vector2D (pscale);

			bool friendly = false;
			if (what.Equals("Toys")){friendly = true;}
			unitStats stats = new unitStats (pname.value, scaleV, friendly);

			if (pmax_lvl != null) stats.setMaxLvl(int.Parse(pmax_lvl.value));
            stats.isUnlocked = false;
            RuneType rune_type = (prune_type != null) ? Get.RuneTypeFromString(prune_type.value) : RuneType.Null;
            ToyType toy_type = (ptoytype != null) ? Get.ToyTypeFromString(ptoytype.value) : ToyType.Null;
            WishType wish_type = WishType.Null;
            CostType cost_type = CostType.Dreams;

            stats.toy_type = toy_type;
            stats.runetype = rune_type;

            

            cost_type = Get.costType(rune_type, toy_type);

            if (pcost_wishtype != null) wish_type = Get.WishTypeFromString(pcost_wishtype.value);
            if (required_building != null) stats.required_building = (required_building.value);
            stats.setCost(cost_type, cost);

            if (prunes != null) loadRunes(prunes, ref stats);
            if (ppermanent != null) stats.island_type = Get.IslandTypeFromString(ppermanent.value); else stats.island_type = IslandType.Permanent;
            
            JSONObject target = new JSONObject ();	
			
			if (p.root.TryGetValue ("target", out target)) 
				stats.target = int.Parse (target.value);
			
			if (pammo != null) {
				stats.ammo = int.Parse(pammo.value);
			//	Debug.Log("Got an effect toy " + pname.value + ", adding " + Get.RuneTypeFromString(parrow.value) + "\n");
			//	Central.Instance.effect_toys.Add(Get.RuneTypeFromString(parrow.value),pname.value);
			}
			Central.Instance.setUnitStats (stats, false);	
			return stats;
		}
		
	}

	//%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%        			LOAD ACTORS
	public string oldLoadActors(string what){
		string line = file.Dequeue();
		
		while (line != null && !line[0].Equals (':')) {							
			unitStats actor =  LoadActor (line, what);	
			line = file.Dequeue();
	        
			if (what.Equals("Toys")){
                //	Debug.Log("Got a toy\n");
                //if (actor.toy_type == ToyType.Hero)
                    Central.Instance.setUnitStats(actor, true);
             
			}else{
				Peripheral.Instance.haveMonsters.Add(actor.name, true);
			}	
		}
	
	return line;
	}


	
	public Vector3 Vector3D(JSONObject scale){
		float xs, zs, ys;
		xs = float.Parse (scale.dict ["x"].value);
		ys = float.Parse (scale.dict ["y"].value);
		zs = float.Parse (scale.dict ["z"].value);
		
		return new Vector3 (xs, ys, zs);
	}
	public Vector3 Vector2D(JSONObject scale){
		float xs, zs;
		float sizeme;
		xs = float.Parse (scale.dict ["x"].value);
		zs = float.Parse (scale.dict ["z"].value);
		
		sizeme = Mathf.Max (xs, zs);
		return new Vector3 (xs, sizeme, zs);
	}

	public float Distance(int xi, int yi, int xf, int yf){
		Vector2 i = new Vector2 (xi, yi);
		Vector2 f = new Vector2 (xf, yf);
		return Vector2.Distance (i, f);

	}
}//end of class

