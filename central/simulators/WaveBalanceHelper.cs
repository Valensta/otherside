using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;



#if UNITY_EDITOR

using UnityEditor;
[CustomEditor(typeof(WaveBalanceHelper))]

public class WaveBalanceHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        WaveBalanceHelper myTarget = (WaveBalanceHelper)target;

        if (GUILayout.Button("Print Stats By Wavelet"))
            ((WaveBalanceHelper)target).CalcStatsByWavelet(false);

        if (GUILayout.Button("Print Stats By Wave"))
            ((WaveBalanceHelper)target).CalcStatsByWave(false);

        if (GUILayout.Button("Print SUMMARY By Wavelet"))
            ((WaveBalanceHelper)target).CalcStatsByWavelet(true);

        if (GUILayout.Button("Print SUMMARY By Wave"))
            ((WaveBalanceHelper)target).CalcStatsByWave(true);

        if (GUILayout.Button("Print SUMMARY By ENEMIES"))
            ((WaveBalanceHelper)target).PrintEnemySummary();

    }
}
#endif
[System.Serializable]
public class WaveBalanceHelper : MonoBehaviour{

    //public Dictionary<string, MonsterStat> monster_list = new Dictionary<string, MonsterStat>(); //THIS IS FOR BALANCING LEVELS
    public List<WaveStat> stats = new List<WaveStat>();
    string all = "ALL";



 /*       
    public void InitMonsterStat(string name, float mass, float defense, float speed)
    {
        MonsterStat stat = new MonsterStat();
        stat.defense = defense;
        stat.speed = speed;
        stat.mass = mass;
      
        if (!monster_list.ContainsKey(name)) monster_list.Add(name, stat);

    }
*/
    public void CalcStatsByWavelet(bool summary)
    {
        List<wave> waves = Moon.Instance.Waves;
        stats = new List<WaveStat>();

        for (int wave_number = 0; wave_number < waves.Count; wave_number++)
        {
            wave w = waves[wave_number];

            for (int wavelet_number = 0; wavelet_number < w.wavelets.Count; wavelet_number++)
            {
                InitWavelet wlet = w.wavelets[wavelet_number];
                Dictionary<string, IntFloat> wavelet_count = new Dictionary<string, IntFloat>();

                foreach (InitEnemyCount e in wlet.enemies)
                {

                    IntFloat count = null;                    
                    wavelet_count.TryGetValue(e.name, out count);
                    if (count == null) wavelet_count.Add(e.name, new IntFloat(e.c, wlet.interval * e.c));
                    else
                    {
                        count.myInt += e.c;
                        count.myFloat += wlet.interval * e.c;
                    }
                    
                }

                AssignWaveStat(wavelet_count, wave_number, wavelet_number);
            }
        }
        PrintStats(summary, true);
    }


    public void CalcStatsByWave(bool summary)
    {
        List<wave> waves = Moon.Instance.Waves;
        stats = new List<WaveStat>();
        for (int wave_number = 0; wave_number < waves.Count; wave_number++)
        {
            wave w = waves[wave_number];
            Dictionary<string, IntFloat> wavelet_count = new Dictionary<string, IntFloat>();
           
            for (int wavelet_number = 0; wavelet_number < w.wavelets.Count; wavelet_number++)
            {
                InitWavelet wlet = w.wavelets[wavelet_number];

                foreach (InitEnemyCount e in wlet.enemies)
                {

                    IntFloat count = null;
              
                    wavelet_count.TryGetValue(e.name, out count);
                    if (count == null) wavelet_count.Add(e.name, new IntFloat(e.c, wlet.interval * e.c));
                    else
                    {
                        count.myInt += e.c;
                        count.myFloat += wlet.interval * e.c;
                    }
                    
                }                
            }
            AssignWaveStat(wavelet_count, wave_number, -1);
        }
        PrintStats(summary, false);
    }

    public void AssignWaveStat(Dictionary<string, IntFloat> monster_count, int wave, int wavelet) {
        float total_mass = 0;
        float total_time = 0;

        foreach (string name in monster_count.Keys)
        {
            int count = monster_count[name].myInt;
            
            enemyStats stat = Central.Instance.getEnemy(name);
            
            if (stat != null)
            {
                WaveStat wavestat = getWaveStat(Central.Instance.current_lvl, wave, wavelet, name);
                wavestat.speed = stat.speed;
                wavestat.total_modified_mass = stat.getModifiedMass() * count;
              

                wavestat.time = monster_count[name].myFloat;
                total_mass += wavestat.total_modified_mass;
                total_time += wavestat.time;                
            }
            else
            {
                Debug.Log("Count not assign a WaveStat for " + name + "\n");
            }
        }
        WaveStat total_wavestat = getWaveStat(Central.Instance.current_lvl, wave, wavelet, all);
        total_wavestat.speed = -1;
        total_wavestat.total_modified_mass = total_mass;
        total_wavestat.time = total_time;
    }


    public void PrintEnemySummary()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("name,mass,defense,effective_mass\n");
        
        foreach (enemyStats stat in Central.Instance.enemies)
        {
            sb.Append(stat.name);
            sb.Append(",");
            sb.Append(stat.mass);
            sb.Append(",");
            sb.Append(stat.getAvgDefense());
            sb.Append(",");
            sb.Append(stat.getModifiedMass());
            sb.Append("\n");
        }
        Debug.Log(sb.ToString());
    }

    public WaveStat getWaveStat(int level, int wave, int wavelet, string name)
    {
        
        foreach (WaveStat check in stats)
        {
            if (check.level != level) continue;
            if (check.wave != wave) continue;
            if (check.wavelet != wavelet) continue;
            if (!check.name.Equals(name)) continue;
            return check;
        }
        
        WaveStat hey = new WaveStat(level, wave, wavelet, name);
        stats.Add(hey);
        return hey;
    }

    public void calcExtra(WaveStat stat)
    {
        stat.mass_per_second = (stat.time > 0) ? stat.total_modified_mass / stat.time : 0f;
        if (stat.wavelet > -1) return;
        if (stat.wave == 0) return;
        if (!stat.name.Equals(all)) return;

        WaveStat previous_stat = getWaveStat(stat.level, stat.wave - 1,-1, all);

        stat.mass_increase = stat.total_modified_mass / previous_stat.total_modified_mass - 1f;

        float prev_mass_per_second = (previous_stat.time > 0) ? previous_stat.total_modified_mass / previous_stat.time : 0f;
        stat.mass_per_second_increase = (prev_mass_per_second > 0)? stat.mass_per_second / prev_mass_per_second - 1 : 0f;
    }

    public void PrintStats(bool summary, bool by_wavelet) {

        StringBuilder sb = new StringBuilder();
        if (summary && !by_wavelet) sb.Append("Level,Wave,Duration,Damage,DPS,Damage Increase,DPS Increase\n");
        if (summary && by_wavelet) sb.Append("Level,Wave,Wavelet,Duration,Damage,DPS,Damage Increase,DPS Increase\n");
        if (!summary) sb.Append("Level,Wave,Wavelet,Name,Duration,Damage,DPS,Damage Increase,DPS Increase\n");


        foreach (WaveStat stat in stats)
        {
            if (summary && !stat.name.Equals(all)) continue;
            calcExtra(stat);
            sb.Append(stat.level);
            sb.Append(",");
            sb.Append(stat.wave);
            sb.Append(",");
            if (by_wavelet)
            {
                sb.Append(stat.wavelet);
                sb.Append(",");
            }
            if (!summary)
            {
                sb.Append(stat.name);
                sb.Append(",");
            }
            sb.Append(stat.time);
            sb.Append(",");
            sb.Append(stat.total_modified_mass);
            sb.Append(",");
            sb.Append(stat.mass_per_second);
            sb.Append(",");
            sb.Append(stat.mass_increase);
            sb.Append(",");
            sb.Append(stat.mass_per_second_increase);
            sb.Append("\n");
        }

        Debug.Log(sb.ToString());
    }


}


public class IntFloat
{
    public int myInt;
    public float myFloat;

    public IntFloat(int _int, float _float)
    {
        myInt = _int;
        myFloat = _float;
    }
}

[System.Serializable]
public class WaveStat
{
    public int level;
    public int wave;
    public int wavelet;
    public string name;
    public float speed;
    public float total_modified_mass;
    public int count;
    public float time;
    public float mass_per_second = 0f;
    public float mass_increase = 0f;
    public float mass_per_second_increase = 0f;


    public WaveStat()
    {
        
    }

    public WaveStat(int level, int wave, int wavelet, string name)
    {
        this.level = level;
        this.wave = wave;
        this.wavelet = wavelet;
        this.name = name;
    }

}
[System.Serializable]
public class MonsterStat
{
    public float mass;
    public float defense;
    public float speed;
}