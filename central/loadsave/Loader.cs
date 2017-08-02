using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;

	public class Loader : MonoBehaviour {
	private string level;
	//bool waypoint = false;
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
        /*
        Debug.LogError("!!!!!!USING DEPRICATED LOADLEVEL. STOP!!!!!");
        return;
        //string line;
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
        */
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

            JSONObject delete_savegames = new JSONObject();
            p.root.TryGetValue("delete_savegames", out points);

            Central.Instance.points = int.Parse(points.value);
            if (delete_savegames.value == "1") Central.Instance.game_saver.DeleteAllSaveGames();


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
				Peripheral.Instance.SetHealth(int.Parse(health.value), false);
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
        return "";
    }
    
    

    public string LoadAllEnemies()
    {
        string line = file.Dequeue();
        while (line != null && !line[0].Equals(':'))
        {
            LoadEnemy(line);
            line = file.Dequeue();
        }
        return line;
    }


    public string LoadAllActors(){
		string line = file.Dequeue();
		while (line != null && !line[0].Equals (':')) {
			LoadActor(line);
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
                    line = LoadAllActors();
                    break;
                case ":monsters":
                    line = LoadAllEnemies();
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

    public enemyStats LoadEnemy(string line)
    {
        JSONParser p = new JSONParser(line);
        JSONObject pname = new JSONObject();
        JSONObject pcost = new JSONObject();
        JSONObject prunes = new JSONObject();
        JSONObject mass = new JSONObject();

        p.parse();
        p.root.TryGetValue("name", out pname);
        p.root.TryGetValue("cost", out pcost);
        p.root.TryGetValue("mass", out mass); //this is here for wave balancing        
        p.root.TryGetValue("runes", out prunes);

        EnemyType type = EnumUtil.EnumFromString<EnemyType>(pname.value, EnemyType.Null);
        if (type == EnemyType.Null)
        {
            Debug.LogError("Trying to initialize an unknown EnemyType from " + pname.value);
            return null;
        }
        enemyStats enemy = new enemyStats(type);

        Central.Instance.enemies.Add(enemy);
        return enemy;
    }


    public unitStats LoadActor(string line){
      //  Debug.Log("Loading actor\n");
		JSONObject pname = new JSONObject ();
		JSONObject pcost = new JSONObject ();
        JSONObject ptoytype = new JSONObject();
        //JSONObject pcost_runetype = new JSONObject ();
        JSONObject prune_type = new JSONObject();
        JSONObject pcost_wishtype = new JSONObject();        
		JSONParser p = new JSONParser (line);

        JSONObject ppermanent = new JSONObject();
        JSONObject pammo = new JSONObject ();				
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
		p.root.TryGetValue ("max_lvl", out pmax_lvl);
		

		p.root.TryGetValue ("ammo", out pammo);
		p.root.TryGetValue ("inventory", out inv);




        p.root.TryGetValue("islandtype", out ppermanent);
        p.root.TryGetValue("exclude_skills", out exclude_skills);
        p.root.TryGetValue("required_building", out required_building);
        unitStats test = Central.Instance.getToy(pname.value);


        if (test != null) {
            if (pmax_lvl != null)
            {
                
                test.setMaxLvl(int.Parse(pmax_lvl.value));
            }
			
        //    Debug.Log("Loading actor again\n");
			//thing has already been defined in a previous level. do not load most stats again

            return test;
		} else {

                int cost = int.Parse(pcost.value);
            

            
                unitStats stats = new unitStats(pname.value);

                if (pmax_lvl != null) stats.setMaxLvl(int.Parse(pmax_lvl.value));
                stats.isUnlocked = false;
                RuneType rune_type = (prune_type != null) ? EnumUtil.EnumFromString(prune_type.value, RuneType.Null) : RuneType.Null;
                ToyType toy_type = (ptoytype != null) ? EnumUtil.EnumFromString(ptoytype.value, ToyType.Null) : ToyType.Null;
                //WishType wish_type = WishType.Null;
                CostType cost_type = CostType.Dreams;

                stats.toy_id.toy_type = toy_type;
                stats.toy_id.rune_type = rune_type;



                cost_type = TowerStore.costType(rune_type, toy_type);

                //if (pcost_wishtype != null) wish_type = Get.WishTypeFromString(pcost_wishtype.value);
                if (required_building != null) stats.required_building = (required_building.value);
                stats.setCost(cost_type, cost);

                
                stats.island_type = ppermanent != null ? EnumUtil.EnumFromString(ppermanent.value,IslandType.Permanent) : IslandType.Permanent;
                                

                if (pammo != null)
                {
                    stats.ammo = int.Parse(pammo.value);
                }
                Central.Instance.setUnitStats(stats, false);
                return stats;
            
		}
		
	}

	//%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%        			LOAD ACTORS
	public string oldLoadActors(string what){
		string line = file.Dequeue();
		
		while (line != null && !line[0].Equals (':')) {							
			
	        
			if (what.Equals("Toys")){
                unitStats actor = LoadActor(line);
                line = file.Dequeue();
                Central.Instance.setUnitStats(actor, true);
             
			}else{
                enemyStats enemy = LoadEnemy(line);
                line = file.Dequeue();
                Central.Instance.enemies.Add(enemy);
                Peripheral.Instance.haveMonsters.Add(enemy.name, true);
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

