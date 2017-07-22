using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TimeName{Dusk, Dawn, Day, Night, Reset, Null};
	
[System.Serializable]
public class TimeOfDay{	
	public TimeName name;
	public Color island_color;	
	public Color bg_color;
	public Color glowy_color;
	public Vector3 light_position;
    
    public TimeOfDay(TimeName _name){
		name = _name;
	}
		
	public TimeOfDay(TimeName _name, Color islandColor, Color _bg_color, Color _glowy_color, Vector3 _light_position){		
		name = _name;
		island_color = islandColor;		
		bg_color = _bg_color;
	    glowy_color = _glowy_color;
		light_position = _light_position;
	
	}
}

[System.Serializable]
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
	
	//public float TIME;
	public float current_time_of_day;
	public SpriteRenderer light_source;
	public List<WaveTime> times = new List<WaveTime>();
	List<TimeOfDay> basic_times = new List<TimeOfDay>();
	public SpriteRenderer glowy;	
	public SpriteRenderer background_image;

    int my_current_wave;
	
	
	public bool is_active;
	public bool finished_transition;
	public TimeOfDayIndicator indicator;    

   // public WavePipController my_wave_pip_controller;
	public delegate void OnDayTimeChangeHandler(TimeName name);
	public static event OnDayTimeChangeHandler OnDayTimeChange; 
	
    private int getWave()
    {
        int wave = Moon.Instance.GetCurrentWave();
        if (wave >= Moon.Instance.waves.Count) wave--;
        return wave;
    }


	public TimeName GetCurrentTime(){
        int wave = getWave();
        if (wave < 0 || wave >= times.Count) return TimeName.Day;
		return times[wave].time_name_start.name;
	}
	
	
	public void BeginDay(){
		if (!is_active) return;
	//	Debug.Log("Beginning day\n");
					
		indicator.SetTime(times[getWave()].time_name_start.name);		
		SetVisuals(true);
       
    }
	
	public void Init(){
	//	Debug.Log("Sun initializing\n");
		//needs current time change and one_day
		glowy =  Monitor.Instance.glowy_image;
		light_source =  Monitor.Instance.light;
		background_image = Monitor.Instance.background_image;
		InitChangeTimes();
		indicator.SetTime(times[getWave()].time_name_start.name);
    //    if (my_wave_pip_controller != null) my_wave_pip_controller.SetCurrentWave(current_wave);
        SetVisuals(true);
	}
	
	TimeOfDay GetTimeOfDay(TimeName name){
		foreach (TimeOfDay tod in basic_times){
			if (tod.name == name) return tod;
		}
		TimeOfDay me_null = new TimeOfDay(TimeName.Null);
		
		return me_null;
	}
	
	//(TimeName _name, float _time, Color islandColor, Color _bg_color, float _glowy_intensity, Vector3 _light_position){
	public void InitChangeTimes(){// these have to be in order
	
		InitBasicTimes();
		float time = 0f;
		//float end_time = 0f;
		times = new List<WaveTime>();
        
	//	Debug.Log("Initializing " + Moon.Instance.waves.Count + " waves\n");
		for (int i = 0; i < Moon.Instance.waves.Count; i++){
			wave my_wave = Moon.Instance.waves[i];
            
			WaveTime wave_time = new WaveTime();
			wave_time.time_name_end = GetTimeOfDay(my_wave.time_name_end);	
			wave_time.wave_count = i;
			wave_time.time_name_start = GetTimeOfDay(my_wave.time_name_start);

            
            if (my_wave.time_change > 0){				
				wave_time.time_name_end = GetTimeOfDay(my_wave.time_name_end);				
				wave_time.end_transition =  my_wave.total_run_time + time;
          //      Debug.Log("Wave " + i + " Duration change " + my_wave.time_change + " * " + my_wave.total_run_time + " + " + time + "\n");
				wave_time.start_transition = my_wave.time_change * my_wave.total_run_time + time;			
				
			}
        
            time += my_wave.total_run_time;			
			times.Add(wave_time);
		}				
		
        
	}
	
	
	public void SetTimePassively(float t){	
		current_time_of_day = t;			
		
	}
	
	public void SetTime(float t){
		if (!is_active) return;
        int new_wave = getWave();

        current_time_of_day = t;
		if (new_wave != my_current_wave){
            //Debug.Log("Sun wave change " + my_current_wave + " to " + current_wave + "\n");
            my_current_wave = new_wave;
            if (OnDayTimeChange != null) OnDayTimeChange(times[my_current_wave].time_name_start.name);
			indicator.SetTime(times[my_current_wave].time_name_start.name);            
		}
		if (times[my_current_wave].start_transition >= 0 && current_time_of_day > times[my_current_wave].start_transition) SetVisuals(false);	
	}
		
	void SetVisuals(bool fast){
		Vector3 light_position;
        int current_wave = getWave();
        if (!fast && times[current_wave].time_name_end.name == TimeName.Null) { Debug.Log("ending set visuals, end time is null, aint doing any of this\n");
            return; }

        if (fast){
            /*	
                    if (light_source != null){
                        light_source.color = times[current_wave].time_name_start.island_color;			
                    //	Debug.Log("Doing fast light\n");
                        light_position = light_source.transform.localPosition;
                        light_position.x = times[current_wave].time_name_start.light_position.x;
                        light_position.y = times[current_wave].time_name_start.light_position.y;
                        light_source.transform.localPosition = light_position;
                    }*/
            TimeOfDay newTimeOfDay = times[current_wave].time_name_start;

            if (Monitor.Instance.color_islands)
            {
                float light_r = newTimeOfDay.island_color.r;
                float light_g = newTimeOfDay.island_color.g;
                float light_b = newTimeOfDay.island_color.b;
                float light_a = newTimeOfDay.island_color.a;
                Color island_sprite_color = new Color(light_r, light_g, light_b, light_a);                

                foreach (Island_Button sprite in Monitor.Instance.islands.Values)
                {
                    if (sprite.My_sprite != null) sprite.My_sprite.color = island_sprite_color;
                }
            }
            if (background_image != null) {
				background_image.color = times[current_wave].time_name_start.bg_color;
			//	Debug.Log("Doing fast background image\n");
			}
			if (glowy != null){
				glowy.color = times[current_wave].time_name_start.glowy_color;
		//		Debug.Log("Doing fast glowy " + current_wave + " intensity " + times[current_wave].time_name_start.glowy_intensity + "\n");
			}
			return;
		
		}
	
	
		if (times[current_wave].time_name_end.name == TimeName.Null){//Debug.Log("ending set visuals, end time is null\n");
            return; }
		float finish = times[current_wave].end_transition;
		float start = times[current_wave].start_transition;
		float percent_complete = Mathf.Min (1f - (finish - current_time_of_day)/(finish - start), 1f);
		if (current_time_of_day == times[current_wave].start_transition) percent_complete  = 0f;
	//	Debug.Log("Duration " + current_time_of_day + " Percent complete " + percent_complete + " current wave " + current_wave + "\n");
		if (percent_complete < 0 || percent_complete > 1) return;
		TimeOfDay new_time = times[current_wave].time_name_end;
		TimeOfDay current_time = times[current_wave].time_name_start;
     //   Debug.Log("Going from " + current_time.name + " to " + new_time.name + "\n");
		//public TimeOfDay(TimeName _name, float _time, Color islandColor, Color _bg_color, float _glowy_intensity, Vector2 _light_position, float _finish_transition_time){time = _time;
		
		if (background_image != null){
		
			float bg_r = current_time.bg_color.r + (new_time.bg_color.r - current_time.bg_color.r)*percent_complete;
			float bg_g = current_time.bg_color.g + (new_time.bg_color.g - current_time.bg_color.g)*percent_complete;
			float bg_b = current_time.bg_color.b + (new_time.bg_color.b - current_time.bg_color.b)*percent_complete;
			Color bg_color = new Color(bg_r, bg_g, bg_b);			
			background_image.color = bg_color;
		}
									
		if (Monitor.Instance.color_islands)
        {
		    float light_r = current_time.island_color.r + (new_time.island_color.r - current_time.island_color.r) * percent_complete;
		    float light_g = current_time.island_color.g + (new_time.island_color.g - current_time.island_color.g) * percent_complete;
		    float light_b = current_time.island_color.b + (new_time.island_color.b - current_time.island_color.b) * percent_complete;
		    float light_a = current_time.island_color.a + (new_time.island_color.a - current_time.island_color.a) * percent_complete;
		    Color island_sprite_color = new Color(light_r, light_g, light_b, light_a);		    

            foreach (Island_Button sprite in Monitor.Instance.islands.Values)
            {
                if (sprite.My_sprite != null) sprite.My_sprite.color = island_sprite_color;
            }



            /*
                light_position = light_source.transform.localPosition;				
                float light_x = current_time.light_position.x + (new_time.light_position.x - current_time.light_position.x)*percent_complete;
                float light_y = current_time.light_position.y + (new_time.light_position.y - current_time.light_position.y)*percent_complete;
                light_position.x = light_x;
                light_position.y = light_y;		
                light_source.transform.localPosition = light_position;
                */
        }
		
		if (glowy != null){
		    float light_r = current_time.glowy_color.r + (new_time.glowy_color.r - current_time.glowy_color.r) * percent_complete;
		    float light_g = current_time.glowy_color.g + (new_time.glowy_color.g - current_time.glowy_color.g) * percent_complete;
		    float light_b = current_time.glowy_color.b + (new_time.glowy_color.b - current_time.glowy_color.b) * percent_complete;
		    float light_a = current_time.glowy_color.a + (new_time.glowy_color.a - current_time.glowy_color.a) * percent_complete;
		    Color glowy_color = new Color(light_r, light_g, light_b, light_a);
		
			glowy.color = glowy_color;
		}

	
	}





    public void InitBasicTimes()
    {
        //-----NIGHT
        Color bg_night = new Color(58f / 255f, 57f / 255f, 71f / 255f, 1f);
        Color getme = Monitor.Instance.GetColorSetting(TimeName.Night, false);
        bg_night = (getme == Color.red)? bg_night: getme;

        Color island_night = (Monitor.Instance.color_islands) ? Monitor.Instance.GetIslandColorSetting(TimeName.Night) : new Color(1f, 1f, 1f, 1f);

        //Color glowy_intensity_night = 180f / 255f;
        Color glowy_intensity_night = Monitor.Instance.GetColorSetting(TimeName.Night, true);
        


        //-----DAWN/DUSK
        Color bg_dawn = new Color(158f / 255f, 155f / 255f, 165f / 255f, 1f);
        getme = Monitor.Instance.GetColorSetting(TimeName.Dawn, false);
        bg_dawn = (getme == Color.red) ? bg_dawn : getme;

        Color island_dawn = (Monitor.Instance.color_islands) ? Monitor.Instance.GetIslandColorSetting(TimeName.Dawn) : new Color(1f, 1f, 1f, 1f);

        //Color glowy_intensity_dawn = 120f / 255f;
        Color glowy_intensity_dawn = Monitor.Instance.GetColorSetting(TimeName.Dawn, true);
      

        //-----DAY
        Color bg_day = new Color(1f, 1f, 1f, 1f);
        getme = Monitor.Instance.GetColorSetting(TimeName.Day, false);
        bg_day = (getme == Color.red) ? bg_day : getme;

        Color island_day = (Monitor.Instance.color_islands)? Monitor.Instance.GetIslandColorSetting(TimeName.Day) : new Color(1f, 1f, 1f, 1f);

        //Color glowy_intensity_day = 25f / 255f;
        Color glowy_intensity_day = Monitor.Instance.GetColorSetting(TimeName.Day, true);        

        basic_times = new List<TimeOfDay>();

        basic_times.Add(new TimeOfDay(TimeName.Reset, island_night, bg_night, glowy_intensity_night, new Vector3(20f, -1.33f)));   // prepare for dawn
        basic_times.Add(new TimeOfDay(TimeName.Dawn, island_dawn, bg_dawn, glowy_intensity_dawn, new Vector3(10, -1.33f)));
        basic_times.Add(new TimeOfDay(TimeName.Day, island_day, bg_day, glowy_intensity_day, new Vector3(0f, 6f)));
        basic_times.Add(new TimeOfDay(TimeName.Dusk, island_dawn, bg_dawn, glowy_intensity_dawn, new Vector3(-10f, -1.33f)));
        basic_times.Add(new TimeOfDay(TimeName.Night, island_night, bg_night, glowy_intensity_night, new Vector3(-20f, -1.33f)));// make it disappear yo		

    }
					
	
	
	
	
	public static Sun Instance { get; private set; }
	

	void Awake()
	{
		//Debug.Log ("Sun awake\n");	
		if (Instance != null && Instance != this) {
			Debug.Log ("Sun got destroyed\n");
			Destroy (gameObject);
		}
		Instance = this;
		
		
	}
	

	
	

}//end of class

