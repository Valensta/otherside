using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;



[System.Serializable]
public class wave{
	public List<InitWavelet> wavelets = new List<InitWavelet> ();
	public float wait = 0f;
	public bool retry;
	public float total_run_time = 0f;
	public float points = 0f;
	public float xp = 0f;
	public float monster_count = 0f;
	public TimeName time_name_start;
	public TimeName time_name_end;
	public float time_change;
	public float time_start;
	public float time_end;
	
	public void SetTime(TimeName _tstart, TimeName _tend, float _tchange){
		time_name_start = _tstart;
		time_name_end = _tend;		
		time_change = _tchange;
	}
	
	public void SetStartTime(TimeName _tstart){
		time_name_start = _tstart;		
		time_name_end = TimeName.Null;
		time_change = 9999999999;
	}
	
	public wave(){
	}

	public float point_factor(){
	//Debug.Log("points " + points + " monster_count " + monster_count + "\n");
		if (points > 0f) return points/monster_count;
		return 1f;
	}

	public float xp_factor(){
		Debug.Log("xp " + xp + " monster_count " + monster_count + "\n");
		if (xp > 0f) return xp/monster_count;
		return 1f;
	}

    public void adjust_total_run_time()
    {
        //  total_run_time -= wavelets[wavelets.Count - 1].GetTotalRunTime();
        //Debug.Log("from " + total_run_time + " minus " + wavelets[wavelets.Count - 1].GetTotalRunTime() + " " + wavelets[wavelets.Count - 1].lull_length);
        total_run_time -= wavelets[wavelets.Count - 1].lull;
    }

    public void add_wavelet(InitWavelet w){
		wavelets.Add(w);
		monster_count += w.GetMonsterCount();
		total_run_time += w.GetTotalRunTime();
	}

	float calc_cheapest_monster(List<string> haveMonsters){
		actorStats max_monster = new actorStats();
		max_monster.cost_type.Amount = 9999;		
		
		//find the cheapest monster
		foreach (string m in haveMonsters) {
			if (max_monster.cost_type.Amount > Central.Instance.getToy(m).cost_type.Amount) {
				max_monster = Central.Instance.getToy(m);
			}
		}
		
		return max_monster.cost_type.Amount;
	}

    /*
	public wave(float n, List<string> m, float mi, float ll, int ws, List<int> pathlist){
		int count = 0;
		wavelet my_wavelet = new wavelet (mi, ll);
		while (n > calc_cheapest_monster(m)) {									
			while (count < ws){			
				int which = (int)UnityEngine.Random.Range (0, m.Count);
				string monster = m[which];
				n -= Central.Instance.getToy(monster).cost_type.cost;
				my_wavelet.monsters.Add(monster);
				count++;
			}
			wavelets.Add(my_wavelet);
			my_wavelet = new wavelet(mi,ll);
			count = 0;
		}
	}*/
}


//{"mode":"list","1":{"monster_invertal":"0.5", "lull_length":"3","list":{"forceball,5,scaleball,3"}},"2":{"monster_invertal":"1", "lull_length":"5","list":{"speedball,1,drunkenbox,10,forceball"}},"3":{"monster_invertal":"0.5", "lull_length":"3","list":{"forceball,1,bigslidingbox,3"}}}
//{"mode":"random","nightmares":"50","monsters":"drunkenbox","monster_interval":"1","lull_length":"5","wavelet_size":"5"}

/*
[System.Serializable] // should combine this with the InitWavelet class probably
public class wavelet
{
    public int nightmares;
    public List<InitEnemyCount> enemies = new List<InitEnemyCount>();
    public float monster_interval;

    public float lull_length;    
    public int monster_count = 0;
    public float run_time = 0;

    public wavelet(float mi, float ll)
    {
        monster_interval = mi;
        lull_length = ll;
    }

    public float GetTotalRunTime()
    {
        run_time = lull_length + monster_count * monster_interval;
        return run_time;

    }
    
    public wavelet(float monster_interval, float lull_length, List<InitEnemyCount> e)
    {
        this.monster_interval = monster_interval;
        this.lull_length = lull_length;
        enemies = CloneUtil.copyList<InitEnemyCount>(e);        
    }
    
    */
[System.Serializable]
public class wavelet
{
    public int nightmares;
    public List<string> monsters = new List<string>();
    public float monster_interval;

    public float lull_length;
    public List<int> paths = new List<int>();
    public int monster_count = 0;
    public float run_time = 0;

    public wavelet(float mi, float ll)
    {
        monster_interval = mi;
        lull_length = ll;
    }

    public float GetTotalRunTime()
    {
        //	Debug.Log("Wavelet total runtime " + (lull_length + monster_count*monster_interval) + "\n");

        run_time = lull_length + monster_count * monster_interval;
        return run_time;

    }

    public wavelet(float mi, float ll, List<string> c, List<int> p)
    {
        monster_interval = mi;
        lull_length = ll;

        string monster = "";
        int count = 0;
        int path = -1;
        int paths_length = p.Count;
        int path_count = 0;
        //    Debug.Log("Paths length " + paths_length + "\n");
        for (int i = 0; i < c.Count; i++)
        {
            string m = c[i];
            //   Debug.Log("m " + m + "\n");
            if (int.TryParse(m, out count) || (!int.TryParse(m, out count) && monster != ""))
            {
                count = -1;
                int.TryParse(m, out count);
                if (count == -1) { Debug.LogError("Failed to parse line: " + c + "\n"); }
                if (!monster.Equals(""))
                {
                    actorStats stats = Central.Instance.getToy(monster);
                    if (stats != null)
                    {
                        while (count > 0)
                        {
                            monsters.Add(monster);
                            monster_count++;

                            paths.Add(path);
                            count--;
                        }
                        monster = "";
                    }
                    //       else { Debug.Log("Trying to add invalid monster to wavelet " + monster + "\n"); }
                }
            }
            else
            {
                //   Debug.Log("Got monster " + m + "\n");			
                monster = m;
                count = 1;
                if (path_count < paths_length) { path = p[path_count]; path_count++; } else path = -1;
            }
        }

    }
}
    



