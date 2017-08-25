using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;



[System.Serializable]
public class wave : IDeepCloneable<wave> {
	public List<InitWavelet> wavelets = new List<InitWavelet> ();
	public float wait = 0f;
	public bool retry;
	public float total_run_time = 0f;
	public float points = 0f;
	public float xp = 0f;
	public float monster_count = 0f;
	public TimeName time_name_start;
	public TimeName time_name_end;
	public float time_change_percent;
	public float time_start;
	public float time_change_at;
    public int enemies_left;


    public wave DeepClone()
    {
        wave my_clone = new wave();
        my_clone.wavelets = CloneUtil.copyList<InitWavelet>(wavelets);
        my_clone.wait = this.wait;
        my_clone.retry = this.retry;
        my_clone.total_run_time = this.total_run_time;
        my_clone.points = this.points;
        my_clone.xp = this.xp;

        my_clone.monster_count = this.monster_count;
        my_clone.time_name_start = this.time_name_start;
        my_clone.time_name_end = this.time_name_end;
        my_clone.time_change_percent = this.time_change_percent;
        my_clone.time_start = this.time_start;
        my_clone.time_change_at = this.time_change_at;
        my_clone.enemies_left = this.enemies_left;

        return my_clone;
    }

    object IDeepCloneable.DeepClone()
    {
        return this.DeepClone();
    }
    public void SetTime(TimeName _tstart, TimeName _tend, float _tchange){
		time_name_start = _tstart;
		time_name_end = _tend;		
		time_change_percent = _tchange;
	}
	
	public void SetStartTime(TimeName _tstart){
		time_name_start = _tstart;		
		time_name_end = TimeName.Null;
		time_change_percent = 9999999999;
	}
	
	public wave(){
	}

	public float point_factor(){
        //Debug.Log("points " + points + " monster_count " + monster_count + "\n");
        //if (points > 0f) 
        return points/monster_count;
		return 1f;
	}

	public float xp_factor(){
	
            return xp/monster_count;
		return 1f;
	}

    public void adjust_total_run_time()
    {
     
        //Debug.Log("from " + total_run_time + " minus " + wavelets[wavelets.Count - 1].GetTotalRunTime() + " " + wavelets[wavelets.Count - 1].lull_length);
        total_run_time -= wavelets[wavelets.Count - 1].lull;
    }

    public void add_wavelet(InitWavelet w, bool old_school){
		wavelets.Add(w);
		monster_count += w.GetMonsterCount();
		total_run_time += w.GetTotalRunTime(old_school);
	}

}

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

    public float GetTotalRunTime(float lull_mult, float interval_mult)
    {
        //	Debug.Log("Wavelet total runtime " + (lull_length + monster_count*monster_interval) + "\n");        

        run_time = lull_length * lull_length + monster_count * monster_interval * interval_mult;
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
                    unitStats stats = Central.Instance.getToy(monster);
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
    



