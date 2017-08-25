using System;
using UnityEngine;
using System.Collections;
using System.Security.Cryptography.X509Certificates;


public enum TimeName{Dawn, Dusk, Day, Night, Reset, Null};
	
[Serializable]
public class TimeOfDay : IComparable{	
	public TimeName name;
	public EnvType envType;
	public Color island_color;	
	public Color bg_color;
	public Color glowy_color;	
    
    public TimeOfDay(TimeName _name){
		name = _name;
	}

	bool timeNameEqual(TimeName a, TimeName b)
	{
		return a == b || (a == TimeName.Dawn && b == TimeName.Dusk) || (a == TimeName.Dusk && b == TimeName.Dawn);
	}
	
	public bool equals(TimeName timeOfDay, EnvType envType)
	{
		return (timeNameEqual(name, timeOfDay) && this.envType == envType);
	}
	
	
	public int CompareTo(object obj)
	{
		if (obj == null) return 1;
		TimeOfDay d = obj as TimeOfDay;
		return (timeNameEqual(name, d.name) && envType == d.envType) ? 0 : 1;
	}
	
	public TimeOfDay(TimeName _name, Color islandColor, Color _bg_color, Color _glowy_color, EnvType _envType){		
		name = _name;
		island_color = islandColor;		
		bg_color = _bg_color;
	    glowy_color = _glowy_color;		
		envType = _envType;

	}
}

[Serializable]
public class WaveTime{
	public TimeOfDay time_name_start;
	public TimeOfDay time_name_end;
	public float start_transition;
	public float end_transition;
	public int wave_count;
	
	public void SetTransitions(float _start, float _end){
		start_transition = _start;
		end_transition = _end;
	}
}

public class Sun : MonoBehaviour {	
	private static Sun instance;
				
	public SpriteRenderer glowy;	
	public SpriteRenderer background_image;
	public TimeOfDayIndicator indicator;
	
	public float maxAlphaPercentChange = 0.01f; //absolute max alpha change
	
	public bool is_active;
	
	public float percentComplete;
	
	private TimeOfDay fromTimeOfDay;
	private TimeOfDay toTimeOfDay;
	private int my_current_wave;
	private TimeName currentTimeName;
	
	public delegate void OnDayTimeChangeHandler(TimeName name);
	public static event OnDayTimeChangeHandler OnDayTimeChange;

	
	
    private int getWave()
    {
        int wave = Moon.Instance.GetCurrentWave();
        if (wave >= Moon.Instance.GetWaveCount()) wave--;
	//    Debug.Log($"Get wave {Moon.Instance.GetCurrentWave()} --> {wave}\n");
        return wave;
    }


	public TimeName GetCurrentTime()
	{
		return currentTimeName;
		/*
        int wave = getWave();
        if (wave < 0 || wave >= Moon.Instance.Waves.Count) return TimeName.Day;
		return Moon.Instance.Waves[wave].time_name_start;
		*/
	}
	
	
	public void Init(float percent_done){
		Debug.Log("Sun initializing\n");
		//needs current time change and one_day
		glowy =  Monitor.Instance.glowy_image;		
		background_image = Monitor.Instance.background_image;		
		percentComplete = percent_done;
		
		setCurrentDayTime(Moon.Instance.Waves[getWave()].time_name_start);
		
		setFromToTimeOfDay();
        SetVisuals(true);
	}

	void setCurrentDayTime(TimeName daytime)
	{
		
		indicator.SetTime(daytime);
		OnDayTimeChange?.Invoke(daytime);
		currentTimeName = daytime; //just for reference
	}

	public void setWave()
	{
		int new_wave = getWave();
		Debug.Log($"SUN setting wave from {my_current_wave} to {new_wave}\n");
		if (my_current_wave == new_wave) return;

		my_current_wave = new_wave;
		Moon.Instance.Waves[my_current_wave].time_start = Moon.Instance.TIME;
		Moon.Instance.Waves[my_current_wave].time_change_at = Moon.Instance.TIME +
		                                                      Moon.Instance.Waves[my_current_wave].time_change_percent *
		                                                      Moon.Instance.Waves[my_current_wave].total_run_time; 
		percentComplete = 0f;
		setFromToTimeOfDay();
	}

	public void SetTime()
	{
		if (!is_active) return;
						
		if (fromTimeOfDay.name  == toTimeOfDay.name)	return;
		
		if (percentComplete != 0f) return;
		if (Moon.Instance.TIME < Moon.Instance.Waves[my_current_wave].time_change_at) return;
		
					
		StopCoroutine(startTimeChange());
		StartCoroutine(startTimeChange());
	}


	void setFromToTimeOfDay()
	{
	//	Debug.Log($"Setting from to time of day for wave {my_current_wave}\n");
		fromTimeOfDay =VisualStore.getTimeOfDay(Peripheral.Instance.env_type, Moon.Instance.Waves[my_current_wave].time_name_start);
		
		toTimeOfDay =(Moon.Instance.Waves[my_current_wave].time_name_end == TimeName.Null)? fromTimeOfDay : 
			VisualStore.getTimeOfDay(Peripheral.Instance.env_type, Moon.Instance.Waves[my_current_wave].time_name_end);
		
	}

	private IEnumerator startTimeChange()
	{
		percentComplete = 0f;
		while (percentComplete < 1f)
		{
			SetVisuals(false);
			
			
			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.1f));
		}
		setCurrentDayTime(Moon.Instance.Waves[getWave()].time_name_end);
		
	} 
		
	void SetVisuals(bool fast){
		
        
        if (!fast && fromTimeOfDay.name == toTimeOfDay.name) { Debug.Log("ending set visuals, end time is null, aint doing any of this\n");
            return; }
	
		float targetPercent = 0f;
		
		if (!fast)
		{		
			targetPercent = percentComplete;
	//		Debug.Log($"percent complete {targetPercent}\n");
			percentComplete += maxAlphaPercentChange;			
		}		
//		Debug.Log($"Setting visuals from {fromTimeOfDay.name} to {toTimeOfDay.name} targetPercent {targetPercent}\n");
		if (targetPercent < 0 || targetPercent > 1) return;
		    
		
		if (background_image != null) background_image.color = Color.Lerp(fromTimeOfDay.bg_color, toTimeOfDay.bg_color, targetPercent);
											
		if (Monitor.Instance.color_islands)
        {		
	        Color island_sprite_color = Color.Lerp(fromTimeOfDay.island_color, toTimeOfDay.island_color, targetPercent);		    
            foreach (Island_Button sprite in Monitor.Instance.islands.Values)            
                if (sprite.My_sprite != null) sprite.My_sprite.color = island_sprite_color;
        }
		
		if (glowy != null)	glowy.color = Color.Lerp(fromTimeOfDay.glowy_color, toTimeOfDay.glowy_color, targetPercent);
	
	}

  
					
	public static Sun Instance { get; private set; }
	

	void Awake()
	{
			
		if (Instance != null && Instance != this) {
			Debug.Log ("Sun got destroyed\n");
			Destroy (gameObject);
		}
		Instance = this;
		
		
	}
	

	
	

}//end of class

