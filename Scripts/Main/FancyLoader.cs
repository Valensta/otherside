using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;

	public class FancyLoader : MonoBehaviour {
	private string level;
	bool waypoint = false;
	public Vector3 invalid = new Vector3 (999, 999, 999);
	Queue<string> file;

    

    public void TestToJson()
    {
        InitLevel test = new InitLevel();
        test.init_stats = new InitStats(90, 10, TimeName.Dawn, 24, 20, 16);
        

        test.toys = new InitToy[2];
        test.toys[0] = new InitToy("sensible_tower", 1, true);
        test.toys[1] = new InitToy("airy_tower", 1, true);
        

        test.waves = new InitWave[2];
        InitWavelet[] wavelets = new InitWavelet[2];

        InitEnemyCount[] enemies0 = new InitEnemyCount[2];
        enemies0[0] = new InitEnemyCount("soldier", 3, 0);
        enemies0[1] = new InitEnemyCount("furfly", 3, 1);

        wavelets[0] = new InitWavelet(0.5f, 3, enemies0);

        //

        InitEnemyCount[] enemies1 = new InitEnemyCount[2];
        enemies1[0] = new InitEnemyCount("tank", 3, 0);
        enemies1[1] = new InitEnemyCount("furfly", 6, 1);

        wavelets[1] = new InitWavelet(0.8f, 5, enemies1);
        

        test.waves[0] = new InitWave(TimeName.Dawn, TimeName.Day, 0.8f, 90, 55, 3, wavelets);
        test.waves[1] = new InitWave(TimeName.Day, TimeName.Day, 0.8f, 95, 60, 4, wavelets);

        test.wishes = new InitWish[2];
        test.wishes[0] = new InitWish(WishType.MoreDamage, 2);
        test.wishes[1] = new InitWish(WishType.MoreXP, 8);
        Debug.Log(JsonUtility.ToJson(test, true));
       
    }

	public static FancyLoader Instance { get; private set; }


	//%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%        	AWAKE      
	void Awake(){
		Instance = this;
       // TestToJson();
	}

	public void setFile(Queue<string> f){
		file = f;
	}

   
  

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%              LOAD FILE
    public void LoadLevel(string filename)
    {
        //string alt_stuff = Application.persistentDataPath + "/" + filename;
        string stuff = ((TextAsset)Resources.Load("Levels/" + filename)).ToString();
        InitLevel level = JsonUtility.FromJson<InitLevel>(stuff);
        //InitLevel level = JsonUtility.FromJson<InitLevel>(alt_stuff);

        LoadAllToys(level);        
        LoadWish(level);
        LoadWaves(level);
        LoadStats(level);
        
                
        Moon.Instance.CalculateWishes();
    }
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%        	LOAD STATS



    public void LoadStats(InitLevel level)
    {
        
            
            //Central.Instance.points = int.Parse(points.value);

              

                Peripheral.Instance.addDreams(level.init_stats.dreams, Vector3.zero, false);
                Peripheral.Instance.max_dreams = 2000;
                PathfinderType PathType = PathfinderType.GridBased;
                Peripheral.Instance.SetHealth(level.init_stats.health);
                Peripheral.Instance.pathf = PathType;
                Peripheral.Instance.tileSize = 1; // not supported through file
                if (level.init_stats.map_size_x > 0)
                {
                    EagleEyes.Instance.setMapSize(level.init_stats.map_size_x, level.init_stats.map_size_y);
                }
                else
                {
                    Debug.Log("WTF no map size\n");
                }
            
            
    }



    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%        	LOAD WAVES

    public void LoadWaves(InitLevel level)
    {
        //random wave mode is not supported!!!
        List<int> pathlist = new List<int>();

        for (int x = 0; x < level.waves.Length; x++)
        {
            InitWave init_wave = level.waves[x];
            int i = 1;
            wave mywave = new wave();
            mywave.points = init_wave.points;
            mywave.xp = init_wave.xp;

            for (int y = 0; y < init_wave.wavelets.Length; y++)
            {
                InitWavelet init_wavelet = init_wave.wavelets[y];
                //wavelet hi = new wavelet(init_wavelet);
                mywave.add_wavelet(init_wavelet);
                i++;
            }
            mywave.wait = init_wave.wait_time;

            TimeName time_start = EnumUtil.EnumFromString<TimeName>(init_wave.time_start, TimeName.Null);
            TimeName time_end = EnumUtil.EnumFromString<TimeName>(init_wave.time_end, TimeName.Null);
            if (init_wave.time_change > 0 && time_start != TimeName.Null && time_end != TimeName.Null)
            {
                mywave.SetTime(time_start, time_end, init_wave.time_change);
            }
            else if (time_start != TimeName.Null)
            {
                mywave.SetStartTime(time_start);
            }else
            {
                mywave.SetStartTime(TimeName.Day);
               // Debug.Log("WAVE missing start time! Assuming Day\n");
            }
            mywave.adjust_total_run_time();
            Moon.Instance.AddWave(mywave);
        }


    }    

    public void LoadWish(InitLevel level)
    {
        WishDial[] dials = new WishDial[level.wishes.Length];

        for (int i = 0; i < level.wishes.Length; i++)
        {
            WishType type = EnumUtil.EnumFromString<WishType>(level.wishes[i].wishtype, WishType.Null);
            if (type == WishType.Null)
            {
                Debug.Log("Trying to load Null wishtype from file\n");
                continue;
            }

            dials[i] = new WishDial(type, level.wishes[i].count);
        }


        Moon.Instance.SetWishDials(dials);
    }

    public actorStats LoadToy(InitToy toy)
    {
        actorStats test = Central.Instance.getToy(toy.name);

        if (test != null)
        {
            if (toy.hasMaxLvl()) test.setMaxLvl(toy.max_lvl);
            test.setActive(toy.active);
            return test;
        }
        else
        {
            Debug.LogError("THIS IS NOT SUPPORTED YET WHY ARE YOU DOING THIS STOP\n");
            return null;
        }
    }

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%        			LOAD ACTORS
    public void LoadAllToys(InitLevel level)
    {
        for (int i = 0; i < level.toys.Length; i++)
        {
            actorStats actor = LoadToy(level.toys[i]);
            Central.Instance.setToy(actor, true);
            Peripheral.Instance.haveToys.Add(actor.name, true);

        }
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

