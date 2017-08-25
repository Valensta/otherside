using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using UnityEngine.UI;


public class LevelBalancer : MonoBehaviour
{
    private static Moon instance;

    public bool am_enabled;

    public EnemyType currentEnemyType;
    public float currentLull;
    public float currentInterval;
    public int repeatTimes = 1;
    public Dictionary<int, bool> currentPaths = new Dictionary<int, bool>();

    public GameObject panel;
    public List<wave> original_waves;
    public bool day;
    public GameObject enemyDetails;
    public List<WaveletBuilder> waveletBuilders;
    public string waveStore = "";
    public static LevelBalancer Instance { get; private set; }
    public float currentPointPercent = 0;
    public int currentWave = 0;
    wave waveInProgress = null;
    public WaveBalanceHelper helper;
    public List<Text> summary;

    public List<EnemyType> enabled_enemy_types;
    
    public void showDetailsPanel(bool show) => enemyDetails.SetActive(true);
    

    public void OnEnable()
    {
        panel.SetActive(am_enabled);
        Init();
    }


    public void ToggleMe()
    {
        am_enabled = !am_enabled;
        panel.SetActive(am_enabled);
        if (am_enabled)
        {
            Init();
            LevelBalancer.Instance.original_waves = CloneUtil.copyList<wave>(Moon.Instance.Waves);
            LevelBalancer.Instance.AutoSetPaths();
            EagleEyes.Instance.WaveButton(false);
            Moon.Instance.abortWave();
        }
        else
        {
            
            EagleEyes.Instance.WaveButton(true);
        }


    }


    public void ReloadWaves()
    {
        Peripheral.Instance.Pause(true);
        FancyLoader.Instance.LoadWavesOnly(Central.Instance.level_list.levels[Central.Instance.current_lvl].name);
        Moon.Instance.updateMyWave();
        Peripheral.Instance.Pause(false);
    }
    
    public void PlayOriginalWave()
    {
        Moon.Instance.Waves[currentWave] = original_waves[currentWave];
        Moon.Instance.current_wave = currentWave;
        Peripheral.Instance.StartWave();
    }

    public void ClearAllEnemies()
    {
        foreach (WaveletBuilder wb in waveletBuilders)
        {
            wb.currentEnemyType = EnemyType.Null;
            foreach (MyToggle t in wb.enemyToggles)
            {
                t.clear();
            }
        }
    }

    public void startWave()
    {
        currentWave = Moon.Instance.current_wave;
        if (!AnyValid())
        {
            Noisemaker.Instance.Click(ClickType.Error);
            return;
        }
        wave my_wave = getWave();

        my_wave.points = original_waves[currentWave].points * currentPointPercent;
        my_wave.xp = original_waves[currentWave].xp * currentPointPercent;

        Debug.Log(JsonUtility.ToJson(my_wave.wavelets[0], false));
        Debug.Log(JsonUtility.ToJson(my_wave, false));

        Moon.Instance.Waves[currentWave] = my_wave;
        Peripheral.Instance.StartWave();
        ShowStats();
    }


    
    public void ResetWaveInProgress()
    =>
        waveInProgress = new wave();


    public void PlayWaveInProgress()
    {
        foreach (InitWavelet wlet in waveInProgress.wavelets)
        {
            if (wlet.lull == 0) wlet.lull = 15;
        }

        Moon.Instance.Waves[currentWave] = waveInProgress;
        Peripheral.Instance.StartWave();
        Debug.Log(JsonUtility.ToJson(waveInProgress, true) + "\n");

    }

    public void setLull(string lull)
    {
        currentLull = (lull != null && !lull.Equals("")) ? float.Parse(lull) : 0.1f;
    }

    public void setPointPercent(string a)
        =>
        currentPointPercent = ((bool)!a?.Equals("")) ? float.Parse(a) : 0;


    public void setCurrentWave(string a)
         =>
        currentWave = ((bool)!a?.Equals("")) ? int.Parse(a) : 0;

    //currentWave = (a!= null && !a.Equals("")) ? int.Parse(a) : 0;


    public void setInterval(string a)
    =>
        currentInterval = ((bool)!a?.Equals("")) ? float.Parse(a) : 0.1f;
    //currentInterval = (interval != null && !interval.Equals("")) ? float.Parse(interval) : 0;


    public void setRepeatTimes(string interval)
    =>
        repeatTimes = (interval != null && !interval.Equals("")) ? int.Parse(interval) : 0;


    public void setDay(bool set) => day = set;


    public void addToWaveInProgress()
    {
        wave part = getWave();
        waveStore += JsonUtility.ToJson(part, true) + "\n";

        if (waveInProgress == null) waveInProgress = new wave();
        foreach (InitWavelet wlet in part.wavelets)
            waveInProgress.add_wavelet(wlet, false);

        waveInProgress.xp = original_waves[currentWave].xp;
        waveInProgress.points = original_waves[currentWave].points;


    }




    public void clearWave() => waveStore = "";
    



    wave getWave()
    {
        wave new_wave = new wave();
        InitWavelet wavelet = generateWavelet();
        new_wave.add_wavelet(wavelet, false);
        new_wave.monster_count = wavelet.GetMonsterCount();
        new_wave.time_name_start = (day) ? TimeName.Day : TimeName.Night;
        new_wave.time_name_end = (day) ? TimeName.Day : TimeName.Night;
        
        return new_wave;
    }

    public void AutoSetPaths()
    {

        //int p = WaypointMultiPathfinder.Instance.paths.Count - 1;
        int p = 0;
        foreach (WaveletBuilder wb in waveletBuilders)
        {
            if (p >= WaypointMultiPathfinder.Instance.paths.Count) p = 0;
            wb.ForcePath(p);
            p++;
        }
    }

    public bool AnyValid()
    {
        foreach (WaveletBuilder wb in waveletBuilders)
            if (wb.Valid() && currentInterval > 0) return true;
        return false;
    }

    public InitWavelet generateWavelet()
    {
        //if more than 1 path is selected, distribute enemies across all paths
        InitWavelet wavelet = new InitWavelet();


        int sub_count = 0;
        foreach (WaveletBuilder wb in waveletBuilders)
            if (wb.Valid()) sub_count++;

        wavelet.enemies = new InitEnemyCount[sub_count * repeatTimes];
        int i = 0;
        for (int r = 1; r <= repeatTimes; r++)
        {

            foreach (WaveletBuilder wb in waveletBuilders)
            {
                if(!wb.Valid()) continue;
                
                wavelet.enemies[i] = wb.getSubWavelet();
                i++;
                
            }
        }
        wavelet.end_wait = 10;
        wavelet.lull = currentLull;
        wavelet.interval = currentInterval;
        return wavelet;
    }


    public void ShowStats()
    {
        foreach (Text t in summary) t.text = "";
        
        if (!AnyValid()) return;

        String log = "";
        WaveletStatDetails stats = calculateStats(generateWavelet(), false);

        StringBuilder sb = new StringBuilder();
        //sb.Append($"DPS: {Get.Round(stats.summary.mass_per_second, 1)}\n");
        //sb.Append($"Total: {Get.Round(stats.summary.total_modified_mass, 1)}\n");
        sb.Append($"Time: {stats.summary.time}\n");
        sb.Append($"Count: {stats.summary.count}\n");
        summary[0].text = sb.ToString();

        log += sb.ToString().Replace("\n", "\t");
        log += "\n";
        
        int i = 1;
        foreach (WaveStat s in stats.details)
        {
            //summary[i].text = $"{s.name}\nC: {s.count}\nDPS: {Get.Round(s.mass_per_second,1)}\nTotal: {Get.Round(s.total_modified_mass,1)}\nTime: {s.time}\n";
            summary[i].text = $"{s.name}\nC: {s.count}\nTime: {s.time}\n";
            
            log += summary[i].text.Replace("\n", "\t");
            log += "\n";
            i++;
        }
        
        Debug.Log(log);
        
    }

    public void ShowTimeForAllWaves()
    {
        StringBuilder sb = new StringBuilder();
        for (int wcount = 0; wcount < Moon.Instance.Waves.Count; wcount++)
        {
            float total = 0f;
            float full_total = 0f;
            for (int wletCount = 0; wletCount < Moon.Instance.Waves[wcount].wavelets.Count; wletCount++)
            {
                WaveletStatDetails details = calculateStats(Moon.Instance.Waves[wcount].wavelets[wletCount], false);
                WaveletStatDetails details_full = calculateStats(Moon.Instance.Waves[wcount].wavelets[wletCount], true);
                
                sb.Append("W: ");
                sb.Append(wcount);
                sb.Append("  WLET: ");
                sb.Append(wletCount);
                sb.Append("  TIME: ");
                sb.Append(details.summary.time);
                sb.Append("  FULL: ");
                sb.Append(details_full.summary.time);
                sb.Append("\n");
                
                total += details.summary.time;
                full_total += details_full.summary.time;
            }
            sb.Append($"WAVE:\t{wcount}\tTOTAL:\t{total}\tFULL:\n");            
        }
        
        summary[0].text = sb.ToString();
        Debug.Log(summary[0].text);
    }
    
    public WaveletStatDetails calculateStats(InitWavelet wavelet, bool include_end_time)
    {
        WaveletStatDetails details = new WaveletStatDetails();

        details.details = new List<WaveStat>();
        WaveStat summary = new WaveStat();
        int max = (repeatTimes > 0)? wavelet.enemies.Length / repeatTimes : wavelet.enemies.Length ;
        for (int x = 0; x < max; x++)
        {
            InitEnemyCount i = wavelet.enemies[x];            
            WaveStat subStat = new WaveStat();
            
            float mass = EnemyStore.getEffectiveMass(EnumUtil.EnumFromString(i.name, EnemyType.Null));
            float speed = EnemyStore.getSpeed(EnumUtil.EnumFromString(i.name, EnemyType.Null));
            
      //      Debug.Log($"BEFORE {i.name} Mass {mass} Speed {speed}\n");
            
            //if (speed == 0) Debug.LogError($"Trying to get speed for an unsupported enemy {i.name}\n");
            float time = i.c * wavelet.interval;

            if (i.name.Equals("Tank"))
            {
                mass += EnemyStore.getEffectiveMass(EnemyType.TinyTank);

                float speed_adjusted =
                    EnemyStore.getSpeed(EnemyType.Tank) * EnemyStore.getEffectiveMass(EnemyType.Tank)
                    + EnemyStore.getSpeed(EnemyType.TinyTank) * EnemyStore.getEffectiveMass(EnemyType.TinyTank);                                       
                speed_adjusted /= mass;
                speed = speed_adjusted;
            } 
            
            if (i.name.Equals("SturdyTank"))
            {
                mass += EnemyStore.getEffectiveMass(EnemyType.Tank) + EnemyStore.getEffectiveMass(EnemyType.TinyTank);

                float speed_adjusted =
                    EnemyStore.getSpeed(EnemyType.SturdyTank) * EnemyStore.getEffectiveMass(EnemyType.SturdyTank)
                    + EnemyStore.getSpeed(EnemyType.Tank) * EnemyStore.getEffectiveMass(EnemyType.Tank)
                    + EnemyStore.getSpeed(EnemyType.TinyTank) * EnemyStore.getEffectiveMass(EnemyType.TinyTank);                                       
                speed_adjusted /= mass;
                speed = speed_adjusted;
            } 
            
            if (i.name.Equals("ImpossibleTank"))
            {
                mass += EnemyStore.getEffectiveMass(EnemyType.SturdyTank) + EnemyStore.getEffectiveMass(EnemyType.Tank) + EnemyStore.getEffectiveMass(EnemyType.TinyTank);                 

                float speed_adjusted =
                    EnemyStore.getSpeed(EnemyType.SturdyTank) * EnemyStore.getEffectiveMass(EnemyType.SturdyTank)
                    + EnemyStore.getSpeed(EnemyType.Tank) * EnemyStore.getEffectiveMass(EnemyType.Tank)
                    + EnemyStore.getSpeed(EnemyType.TinyTank) * EnemyStore.getEffectiveMass(EnemyType.TinyTank)
                    + EnemyStore.getSpeed(EnemyType.ImpossibleTank) * EnemyStore.getEffectiveMass(EnemyType.ImpossibleTank);
                speed_adjusted /= mass;
                speed = speed_adjusted;
            }               
            
            if (i.name.Equals("Turtle")) mass += 8*EnemyStore.getEffectiveMass(EnemyType.TinyPlane);
            
      //      Debug.Log($"AFTER {i.name} Mass {mass} Speed {speed}\n");

            
            summary.count += i.c * repeatTimes;
            summary.total_modified_mass += i.c * mass * repeatTimes;
         //   Debug.Log($"{i.c - 1} * {wavelet.interval} * {repeatTimes}\n");
            summary.time += ((i.c - 1) * wavelet.interval) * repeatTimes;
            summary.speed += speed * i.c  * repeatTimes;

            subStat.speed = speed;
            subStat.time = time;
            subStat.count = i.c;
            subStat.total_modified_mass = mass * i.c;
            subStat.name = i.name;
            
            subStat.mass_per_second = speed * i.c * mass / time;             
            
            details.details.Add(subStat);
        }        
        float extra = (include_end_time) ? wavelet.end_wait : 0;
        summary.speed /= summary.count;
        summary.time += (wavelet.enemies.Length - 1) * wavelet.lull + extra;
//        Debug.Log($" + {wavelet.enemies.Length - 1} * {wavelet.lull} + {extra}\n");
    //    Debug.Log($"Speed: {summary.speed} Mass: {summary.total_modified_mass} Time: {summary.time}\n");
        summary.mass_per_second = summary.speed * summary.total_modified_mass / summary.time;

        details.summary = summary;
        return details;
    }


 
    public void Init()
    {
        if (!am_enabled) return;

      //  Debug.Log("initializing LevelBalancer\n");

        foreach (WaveletBuilder wb in waveletBuilders)
        {
            wb.Init(enabled_enemy_types);

        }



    }


    

    void Awake()
    {

        if (Instance != null && Instance != this)
        {

            Destroy(gameObject);
        }
        Instance = this;

    }
}//end of class

[System.Serializable]
public class WaveletStatDetails
{
    public WaveStat summary;
    public List<WaveStat> details;


}
