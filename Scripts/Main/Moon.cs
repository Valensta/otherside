using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Moon : MonoBehaviour {	
	private static Moon instance;
	
	public float TIME;
	public List<wave> waves;
    public WishDial[] wish_dials; //ideal spawn counts per wishtype    
    bool wave_in_progress;

    public bool WaveInProgress
    {
        get
        {
            return wave_in_progress;
        }

        set
        {
        //    Debug.Log("Setting wave in progress " + wave_in_progress + "\n");
            wave_in_progress = value;
        }
    }
//	public delegate void OnWaveStartHandler(int content);
//	public static event OnWaveStartHandler onWaveStart; 

    public delegate void OnWaveEndHandler(int content);
	public static event OnWaveEndHandler onWaveEnd;

    public delegate void OnLastWaveletHandler(int content);
    public static event OnLastWaveletHandler onLastWavelet;

    public static Moon Instance { get; private set; }
	int current_wave;
	wave my_wave;	
	InitWavelet my_wavelet;
	float point_factor; //wave point factor
	float xp_factor; //wave point factor
	bool done;
	int current_wavelet;
	int m_count; // Monster within InitEnemyCount counter
    int e_count; // InitEnemyCount counter
	float wait;
	bool make_wave;
	Sun my_sun;
	public float one_day = 24;
	public Transform monsters_transform;
	
	public void SetWave(int w){
		current_wave = w;
		Sun.Instance.SetWave(w);

        EagleEyes.Instance.WaveCountUpdate();
    }

    public float getWaveXpFactor() // factor for wave balancing. peripheral has xp_factor that is modifiable by potions
    {
        return xp_factor;
    }

	void Awake()
	{
		//Debug.Log ("Moon awake");	
		if (Instance != null && Instance != this) {
			Debug.Log ("Moon got destroyed\n");
			Destroy (gameObject);
		}
		Instance = this;		
		Peripheral.onWaveStart += onWaveStart;
		MultiLevelStateSaver.onSaveCompleteStartWave += onSaveCompleteStartWave;
		
	}
	
	public int GetCurrentWave(){
		return current_wave;
	}
	
	void onWaveStart(int i){
		InitWave(i);
        Noisemaker.Instance.Play("wave_start");
        if (my_sun == null) my_sun = Sun.Instance;
	}

	public void InitEmpty(){
		waves = new List<wave>();
		
	}

	public void AddWave(wave w){
		//total_wave_time += w.total_run_time;
		waves.Add(w);
		return;
	}
	
	public void SetLevelDuration(float i){
		one_day = i;
	}
	
	


	public void InitWave(int i){
		wait = -1;
		my_wave = waves [i];
		my_wavelet = null;
		point_factor = my_wave.point_factor();
		xp_factor = my_wave.xp_factor();
		
		done = false;
		current_wavelet = 0;
		m_count = 0;
        WaveInProgress = true;
		current_wave = i;
        EagleEyes.Instance.WaveCountUpdate();
    }
    
    public int GetWaveCount(){
		return waves.Count;
    }

    public string getWaveText()
    {
        if (current_wave < waves.Count)
            return current_wave + "." + current_wavelet + " out of " + waves.Count;
        else
            return "Almost there!";
    }

    public void SetWishDials(WishDial[] list)
    {
        wish_dials = list;
    }

    public float getWishSpawnAdjustment(WishType type)
    {
        foreach (WishDial dial in wish_dials)
        {
            if (dial.type == type) return dial.adjustment;
        }
        return 1;
    }

    public void CalculateWishes()
    {
        //after wish dials and waves are loaded
        Dictionary<string, Int> monster_count = new Dictionary<string, Int>();

        //get total monster count
        foreach (wave w in waves)
        {
            foreach (InitWavelet wlet in w.wavelets)
            {
                
                List<Wish> inventory;
                foreach (InitEnemyCount e in wlet.enemies) {
                    Int count = null;
                    monster_count.TryGetValue(e.name, out count);                    
                    if (count == null) monster_count.Add(e.name, new Int(e.c)); else count.value += e.c;
                    
                }/*
                foreach (string m in wlet.monsters)
                {
                    Int count = null;
                    monster_count.TryGetValue(m, out count);
                    if (count == null) monster_count.Add(m, new Int(1)); else count.value++;
                }*/
            }
        }

        //get total count by wishtype
        Dictionary<WishType, Float> wish_count = new Dictionary<WishType, Float>();

        foreach (KeyValuePair<string, Int> kvp in monster_count)
        {
            List<Wish> inventory = null;
        //    Debug.Log("Get " + kvp.Key + "\n");
            inventory = Central.Instance.getToy(kvp.Key).inventory;
            if (inventory == null) { Debug.Log("moon could not find inventory for " + kvp.Key + ", moving on.\n"); }

            foreach (Wish wish in inventory)
            {
                float more = wish.percent * (float)kvp.Value.value;
                Float count = null;
                wish_count.TryGetValue(wish.type, out count);
                if (count == null) wish_count.Add(wish.type, new Float(more)); else count.value+=more;
            }
        }

        //add dummy 1 dials if we are missing settings for any
        int last_found = wish_dials.Length - 1;
        for (int i = 0; i < wish_dials.Length; i++)
        {
        //    Debug.Log("i " + i + "\n");
            if (wish_dials[i] == null || wish_dials[i].type == WishType.Null)
            {
                last_found = i - 1; break;
            }
        }

        if (last_found != wish_dials.Length - 1)
        {
            foreach (WishType val in Enum.GetValues(typeof(WishType)))
            {
                WishDial found = null;
                for (int i = 0; i < last_found; i++)
                {
                    if (wish_dials[i].type == val) found = wish_dials[i];
                }
                if (found == null) {
                    last_found++;
                    wish_dials[last_found] = new WishDial(val, -1, 1);
                }
            }
        }
                 
        foreach (WishDial dial in wish_dials)
        {
            if (dial.count == -1) continue;        
            Float have = null;
            wish_count.TryGetValue(dial.type, out have);
            if (have == null) continue;
            float want = dial.count;
            dial.adjustment = want / have.value;            
        }

        EagleEyes.Instance.WaveCountUpdate();

    }

    class string_count
    {
        string text;
        int count;
    }

	void Update () {      
		
		if (!WaveInProgress)return;		
		if (wait > 0 && TIME < wait) {TIME += Time.deltaTime; my_sun.IncrementTime(current_wave, Time.deltaTime); return;}
		if (wait > 0 && TIME > wait){//Debug.Log("Done waiting @ " + TIME + "\n"); 
			wait = -1; }
		
		if (!done){
			if (my_wavelet == null || my_wavelet.GetMonsterCount() == 0)
            {
             //   Debug.Log("wavelet is null\n");
                if (current_wavelet < my_wave.wavelets.Count){
                //    Debug.Log("Getting wavelet\n");
					my_wavelet = my_wave.wavelets[current_wavelet];
                    Noisemaker.Instance.Play("wavelet_start");
                    current_wavelet++;
                    EagleEyes.Instance.WaveCountUpdate();
                    m_count = 0;
                    e_count = 0;
                    if (current_wavelet == my_wave.wavelets.Count)
                    {
                        Peripheral.Instance.level_state = LState.OnLastWavelet;
                        Peripheral.Instance.Wave_interval = my_wavelet.lull;
                        if (onLastWavelet != null) onLastWavelet(current_wave);
                    }

                }
                else{
					done = true;
                    Noisemaker.Instance.Play("wave_end");
                }
			}
			if(!done){


                if (m_count >= my_wavelet.enemies[e_count].c) { m_count = 0; e_count++; }
                
                if (e_count < my_wavelet.enemies.Length && m_count < my_wavelet.enemies[e_count].c) { 
                    string m = my_wavelet.enemies[e_count].name;
                    int p = my_wavelet.enemies[e_count].p;
                    m_count++;
                
					Peripheral.Instance.makeMonster(m, p, point_factor, xp_factor);					
					wait = my_wavelet.interval;
					TIME = 0f;
				}else{
               //     Debug.Log("Setting wavelet to null\n");
                    wait = my_wavelet.lull;										
                    if (my_wave.wavelets.Count == current_wavelet) wait = 0f;
					my_wavelet = null;
					TIME = 0f;
				}
			}
			return;
		}else{				
			if (Peripheral.Instance.Wave_interval < 0 && monsters_transform.childCount > 0){return;}
			done = true;
            Noisemaker.Instance.Play("wave_end");
            if (onWaveEnd !=  null) onWaveEnd(current_wave);
			current_wave++;
            //current_wavelet = 0;
			Peripheral.Instance.current_wave = current_wave;
			WaveInProgress = false;
			Peripheral.Instance.level_state = LState.WaveEnded;
            
            //EagleEyes.Instance.WaveCountUpdate();
		}

	}
	
	public void onSaveCompleteStartWave(){
		if (make_wave){
			make_wave = false;
			Debug.Log("onSaveCompleteStartWave\n");
			System.GC.Collect();
			//StartCoroutine("makeWave", current_wave);
			WaveInProgress = true;
            
			Peripheral.Instance.level_state = LState.WaveStarted;
			
		}
	}

    
    

}//end of class

