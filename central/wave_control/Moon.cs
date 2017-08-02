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

    public WaveBalanceHelper wave_balance_helper;

    public bool WaveInProgress { get; set; }/*
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
    }*/
    public delegate void OnWaveEndHandler(int content);
	public static event OnWaveEndHandler onWaveEnd;

    public delegate void OnLastWaveletHandler(int content);
    public static event OnLastWaveletHandler onLastWavelet;

    public static Moon Instance { get; private set; }
	
	wave my_wave;	
	InitWavelet my_wavelet;
	float point_factor; //wave point factor
	float xp_factor; //wave point factor

	bool done;    
	float wait;

    public float making_a_wave;
  //  float waiting_for_a_wave_in_progress;
	bool make_wave;
	Sun my_sun;
	public float one_day = 24;
	public Transform monsters_transform;

    public int current_wave;
    public int current_wavelet;
    public int m_count; // Monster within InitEnemyCount counter
    public int e_count; // InitEnemyCount counter   wave -> wavelet -> list of enemycounts -> list of enemies

    //float skip_forward_gap = 3.5f; // if skipping forward due to no enemies to kill, leave this much time before starting the next wavelet

    public float extra_end_wait = 0;

    // factor for wave balancing. peripheral has xp_factor that is modifiable by potions
    public float getWaveXpFactor() => xp_factor;

    public void SetLevelDuration(float i) => one_day = i;

    public int GetCurrentWave() => current_wave;

    bool balanceMode() => (LevelBalancer.Instance.am_enabled);

    public string getEnemyID() => current_wave + "." + current_wavelet + "." + e_count + "." + m_count;

    public int GetWaveCount() => waves.Count;




    public void SetWave(int wave, int wavelet){              
		current_wave = wave;
        current_wavelet = wavelet;
        EagleEyes.Instance.WaveCountUpdate();
	    
    }

    public void SetWavelet(int w)
    {
        current_wavelet = w;
        EagleEyes.Instance.WaveCountUpdate();
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
	public void SetTime(float time)
    {
        TIME = time;
        Sun.Instance.SetTimePassively(time);
    }

	

    void onWaveStart(int i){ //i is always current_wave but we need this for eventoverseers
     
		InitWave(i);
        Noisemaker.Instance.Play("wave_start");
        if (my_sun == null) my_sun = Sun.Instance;
	}

	public void InitEmpty(){
		waves = new List<wave>();
        SetWave(0,0);
        

    }

	public void AddWave(wave w){
		//total_wave_time += w.total_run_time;
		waves.Add(w);
		return;
	}
	

	
	
    //for use by Distractor
    public void enemySpawned(int wave)
    {
        if (wave < 0 || wave >= waves.Count) return;

        
        if (waves[wave].enemies_left < 0)
        {
            Debug.LogError("enemy spawned after we saved the wave?!\n");
            return;
        }

        waves[wave].enemies_left++;
    }


	
    public void enemyDied(int wave)
    {
       // Debug.Log("Enemy Died for wave " + wave + "\n");
        if (Central.Instance.current_lvl == 0) return; //not on the tutorial
        if (wave < 0 || wave >= waves.Count) return;
        waves[wave].enemies_left--;

        if (waves[wave].enemies_left == 0)
        {
            Central.Instance.SaveCurrentGame();
            waves[wave].enemies_left = -99;
            return;
        }

        if (waves[wave].enemies_left < 0 && !LevelBalancer.Instance.am_enabled)                    
            Debug.LogError("enemy Died for wave " + wave + " but we already saved the wave!!?\n");
        
    }

    



    public void InitWave(int i)
    {
        if (i == 0) TIME = 0f;
        
        wait = TIME;



        my_wave = waves[i];
		my_wavelet = null;

        LevelMod mod = Peripheral.Instance.getLevelMod();
        point_factor = (mod.dream_uplift + 1f) * my_wave.point_factor();
		xp_factor = (mod.xp_uplift + 1f) * my_wave.xp_factor();

	    Debug.Log($"INITIALIZING WAVE {i} point_factor {point_factor} mod.dream_uplift {mod.dream_uplift} + {Central.Instance.getCurrentDifficultyLevel().ToString()}\n");
	    
        my_wave.enemies_left = (int)my_wave.monster_count;
        done = false;
        
		m_count = 0;
        WaveInProgress = true;
        SetWave(i,0);
        
    }
    
    


    public string getWaveText()
    {
        if (current_wave < waves.Count - 1)
            return $"{current_wave+1}.{current_wavelet} out of {waves.Count}";
        else if (current_wave == waves.Count - 1)        
            return $"Last Wave! ({current_wave+1}.{current_wavelet})";
        else
            return "Almost\n        there!";
    }

    public void SetWishDials(WishDial[] list) => wish_dials = list;
    
    

    public float getWishSpawnAdjustment(WishType type)
    {

        
        foreach (WishDial dial in wish_dials)
        {
            if (dial.type == type)
            {
                float base_adjustment = dial.adjustment;
                float level_mod = Peripheral.Instance.getLevelMod().sensible_wish_uplift;
                if (type == WishType.Sensible) base_adjustment *= (1 + level_mod);

                if (level_mod == -99) return 0; //used by tutorial gameevent

                WishDial current_wishes_dial = ScoreKeeper.Instance.getPossibleWish(type);
                int current_wishes = (current_wishes_dial != null) ? current_wishes_dial.count : 0;
                    //Peripheral.Instance.my_inventory.GetWishCount(type);
                float max_wishes = (float)(dial.count);

                if (current_wishes < max_wishes * 0.50f) return base_adjustment;
                if (current_wishes < max_wishes * 0.75f) return base_adjustment * 0.8f;
                if (current_wishes < max_wishes) return base_adjustment * 0.6f;
                
             
                return base_adjustment*.50f;

            }
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
                    
                }
            }
        }

        //get total count by wishtype
        Dictionary<WishType, Float> wish_count = new Dictionary<WishType, Float>();

        foreach (KeyValuePair<string, Int> kvp in monster_count)
        {
            List<Wish> inventory = null;
   //         Debug.Log("Get " + kvp.Key + "\n");
            inventory = Central.Instance.getEnemy(kvp.Key).inventory;
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


    void Update () {

        if (!WaveInProgress) { return; }

        

            TIME += Time.deltaTime; 

        if (monsters_transform.childCount == 0 && (wait - TIME) > Get.getSkipForwardGap(Central.Instance.current_lvl))
        {

            int buffer = 2;
            Debug.Log($"!!!!! {current_wave}.{current_wavelet} wavelet defined {my_wavelet!=null} SKIPPED {TIME - wait - buffer} seconds!\n");
            

            Tracker.Log(PlayerEvent.WaveTiming, true,
              customAttributes: new Dictionary<string, string>(){ { "attribute_1", $"{current_wave}.{current_wavelet} wavelet defined {my_wavelet!=null}"  },
                  {"attribute_2", "SKIPPED" } },
              customMetrics: new Dictionary<string, double>() { { "metric_1",(TIME - wait - buffer)} });
            
            TIME = wait - buffer; // if we ran out of enemies, let's speed things along

        }
        if (!balanceMode()) my_sun.SetTime(TIME);
        

        if (my_wavelet == null)
        {
            if (monsters_transform.childCount >= 3)
            {
                extra_end_wait += Time.deltaTime;
                return;
            }
            else
            {
                TIME = wait;                
                Debug.Log($"!!!!! Extra end wait {current_wave}.{current_wavelet} wavelet defined {my_wavelet!=null} was {extra_end_wait}\n");

                Tracker.Log(PlayerEvent.WaveTiming, true,
                            customAttributes: new Dictionary<string, string>(){ { "attribute_1", $"{current_wave}.{current_wavelet} wavelet defined {my_wavelet!=null}"  },
                                                                        {"attribute_2", "WAITED" } },
                      customMetrics: new Dictionary<string, double>() { { "metric_1", extra_end_wait } });
                
                
                extra_end_wait = 0f;
            }
        }
        if (wait > 0 && TIME < wait) {  return;}


        if (!done){
			if (my_wavelet == null || my_wavelet.GetMonsterCount() == 0)
            {             
                if (current_wavelet < my_wave.wavelets.Count){
                
					my_wavelet = my_wave.wavelets[current_wavelet];
                    Noisemaker.Instance.Play("wavelet_start");
                    //if (!balanceMode()) 
                    SetWavelet(current_wavelet + 1);
                //    Debug.Log("Wavelet start end " + " Duration.time " + Duration.time + " SUN " + Sun.Instance.current_time_of_day + "\n");

                    m_count = 0;
                    e_count = 0;
                    if (current_wavelet == my_wave.wavelets.Count)
                    {
                        Peripheral.Instance.level_state = LState.OnLastWavelet;
	                    Peripheral.Instance.Wave_interval = my_wavelet.getEndWait();
                        if (onLastWavelet != null) onLastWavelet(current_wave);
                    }

                }
                else{
					done = true;
                  //  Debug.Log("Wave end " + " Duration.time " + Duration.time + "\n");
                    Noisemaker.Instance.Play("wave_end");
                }
			}
			if(!done)
			{
                
				
					bool reset = (m_count >= my_wavelet.enemies[e_count].c);
					bool last = (m_count == my_wavelet.enemies[e_count].c - 1);

					if (reset)
					{
						m_count = 0;
						e_count++;
					}

					if (e_count < my_wavelet.enemies.Length)
					{

						if (last) incrementWait(my_wavelet.lull);
						else incrementWait(my_wavelet.interval);

						StartCoroutine(makeMonster(my_wavelet.enemies[e_count].name, my_wavelet.enemies[e_count].p));
						m_count++;

					}
					else
					{

						if (current_wavelet != my_wave.wavelets.Count) incrementWait(my_wavelet.end_wait);
						my_wavelet = null;

					}
				
			}
			return;
		}else{				
			if (Peripheral.Instance.Wave_interval < 0 && monsters_transform.childCount > 0){return;}
			done = true;
            Noisemaker.Instance.Play("wave_end");
            if (onWaveEnd !=  null) onWaveEnd(current_wave);
            if (!balanceMode())SetWave(current_wave + 1,0);
            
            WaveInProgress = false;
			Peripheral.Instance.level_state = LState.WaveEnded;
            
		}

	}

    void incrementWait(float f)
    {
       // Debug.Log("incrementing Wait by " + f + " TIME is " + TIME + "\n");
        wait += f;
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


    public void abortWave()
    {
        WaveInProgress = false;
        my_wavelet = null;
    }
    
    IEnumerator makeMonster(string m, int p)
    {
        HitMe monster = Peripheral.Instance.makeMonster(m, p, point_factor, xp_factor);


        

        yield return null;
    }



}//end of class

