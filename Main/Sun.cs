using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TimeName{Dusk, Dawn, Day, Night, Reset, Null};
	
[System.Serializable]
public class TimeOfDay{	
	public TimeName name;
	public Color light_color;	
	public Color bg_color;
	public float glowy_intensity;
	public Vector3 light_position;		
	
	public TimeOfDay(TimeName _name){
		name = _name;
	}
		
	public TimeOfDay(TimeName _name, Color _light_color, Color _bg_color, float _glowy_intensity, Vector3 _light_position){		
		name = _name;
		light_color = _light_color;		
		bg_color = _bg_color;
		glowy_intensity = _glowy_intensity;
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
	
	
	float change_total_time = 20f;
	float change_steps = 150f;
	public bool is_active;
	public bool finished_transition;
	public TimeOfDayIndicator indicator;
	int current_wave;


   // public WavePipController my_wave_pip_controller;
	public delegate void OnDayTimeChangeHandler(TimeName name);
	public static event OnDayTimeChangeHandler OnDayTimeChange; 
	

	public void SetWave(int i){
		current_wave = i;
      //  if (my_wave_pip_controller != null) my_wave_pip_controller.SetCurrentWave(i);
	}
	
    public int GetWave()
    {
        return current_wave;

    }

	public TimeName GetCurrentTime(){
		return times[current_wave].time_name_start.name;
	}
	
	
	public void BeginDay(){
		if (!is_active) return;
	//	Debug.Log("Beginning day\n");
					
		indicator.SetTime(times[current_wave].time_name_start.name);		
		SetVisuals(true);
       
    }
	
	public void Init(){
	//	Debug.Log("Sun initializing\n");
		//needs current time change and one_day
		glowy =  Monitor.Instance.glowy_image;
		light_source =  Monitor.Instance.light;
		background_image = Monitor.Instance.background_image;
		InitChangeTimes();
		indicator.SetTime(times[current_wave].time_name_start.name);
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
	
	//(TimeName _name, float _time, Color _light_color, Color _bg_color, float _glowy_intensity, Vector3 _light_position){
	public void InitChangeTimes(){// these have to be in order
	
		InitBasicTimes();
		float time = 0f;
		float end_time = 0f;
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
				wave_time.start_transition = my_wave.time_change * my_wave.total_run_time + time;			
				
			}
        
            time += my_wave.total_run_time;			
			times.Add(wave_time);
		}				
		
        
	}
	
	
	public void SetTime(float t){	
		current_time_of_day = t;			
		
	}
	
	public void IncrementTime(int _wave, float t){
		if (!is_active) return;

		current_time_of_day += t;
		if (current_wave != _wave){
			current_wave = _wave;
        //    if (my_wave_pip_controller != null) my_wave_pip_controller.SetCurrentWave(current_wave);
            if (OnDayTimeChange != null) OnDayTimeChange(times[current_wave].time_name_start.name);
			indicator.SetTime(times[current_wave].time_name_start.name);
		}
		if (times[current_wave].start_transition >= 0 && current_time_of_day > times[current_wave].start_transition) SetVisuals(false);	
	}
		
	void SetVisuals(bool fast){
		Vector3 light_position;
        if (!fast && times[current_wave].time_name_end.name == TimeName.Null) { Debug.Log("ending set visuals, end time is null, aint doing any of this\n");
            return; }

        if (fast){
		//	Debug.Log("Setting fast visuals for Sun for " + times[current_wave].time_name_start.name + "\n");
			if (light_source != null){
				light_source.color = times[current_wave].time_name_start.light_color;			
			//	Debug.Log("Doing fast light\n");
				light_position = light_source.transform.localPosition;
				light_position.x = times[current_wave].time_name_start.light_position.x;
				light_position.y = times[current_wave].time_name_start.light_position.y;
				light_source.transform.localPosition = light_position;
			}
			
			if (background_image != null) {
				background_image.color = times[current_wave].time_name_start.bg_color;
			//	Debug.Log("Doing fast background image\n");
			}
			if (glowy != null){
				Show.SetAlpha(glowy, times[current_wave].time_name_start.glowy_intensity);
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
	//	Debug.Log("Time " + current_time_of_day + " Percent complete " + percent_complete + " current wave " + current_wave + "\n");
		if (percent_complete < 0 || percent_complete > 1) return;
		TimeOfDay new_time = times[current_wave].time_name_end;
		TimeOfDay current_time = times[current_wave].time_name_start;
     //   Debug.Log("Going from " + current_time.name + " to " + new_time.name + "\n");
		//public TimeOfDay(TimeName _name, float _time, Color _light_color, Color _bg_color, float _glowy_intensity, Vector2 _light_position, float _finish_transition_time){time = _time;
		
		if (background_image != null){
		
			float bg_r = current_time.bg_color.r + (new_time.bg_color.r - current_time.bg_color.r)*percent_complete;
			float bg_g = current_time.bg_color.g + (new_time.bg_color.g - current_time.bg_color.g)*percent_complete;
			float bg_b = current_time.bg_color.b + (new_time.bg_color.b - current_time.bg_color.b)*percent_complete;
			Color bg_color = new Color(bg_r, bg_g, bg_b);			
			background_image.color = bg_color;
		}
									
		if (light_source != null){
			float light_r = current_time.light_color.r + (new_time.light_color.r - current_time.light_color.r)*percent_complete;
			float light_g = current_time.light_color.g + (new_time.light_color.g - current_time.light_color.g)*percent_complete;
			float light_b = current_time.light_color.b + (new_time.light_color.b - current_time.light_color.b)*percent_complete;
			float light_a = current_time.light_color.a + (new_time.light_color.a - current_time.light_color.a)*percent_complete;
			Color light_color = new Color(light_r, light_g, light_b, light_a);	
			light_source.color = light_color;
		
			light_position = light_source.transform.localPosition;				
			float light_x = current_time.light_position.x + (new_time.light_position.x - current_time.light_position.x)*percent_complete;
			float light_y = current_time.light_position.y + (new_time.light_position.y - current_time.light_position.y)*percent_complete;
			light_position.x = light_x;
			light_position.y = light_y;		
			light_source.transform.localPosition = light_position;
		}
		
		if (glowy != null){
		
			Color glowy_color = glowy.color;			
			float glowy_a = current_time.glowy_intensity + (new_time.glowy_intensity - current_time.glowy_intensity)*percent_complete;						
			glowy_color.a = glowy_a;
		
			glowy.color = glowy_color;
		}

	
	}





    public void InitBasicTimes()
    {
        //-----NIGHT
        Color bg_night = new Color(58f / 255f, 57f / 255f, 71f / 255f, 1f);
        Color getme = Monitor.Instance.GetColorSetting(TimeName.Night, false);
        bg_night = (getme == Color.red)? bg_night: getme;
        
        float glowy_intensity_night = 180f / 255f;
        getme = Monitor.Instance.GetColorSetting(TimeName.Night, true);
        glowy_intensity_night = (getme == Color.red) ? glowy_intensity_night : getme.a;


        //-----DAWN/DUSK
        Color bg_dawn = new Color(158f / 255f, 155f / 255f, 165f / 255f, 1f);
        getme = Monitor.Instance.GetColorSetting(TimeName.Dawn, false);
        bg_dawn = (getme == Color.red) ? bg_dawn : getme;

        float glowy_intensity_dawn = 120f / 255f;
        getme = Monitor.Instance.GetColorSetting(TimeName.Dawn, true);
        glowy_intensity_dawn = (getme == Color.red) ? glowy_intensity_dawn : getme.a;


        //-----DAY
        Color bg_day = new Color(1f, 1f, 1f, 1f);
        getme = Monitor.Instance.GetColorSetting(TimeName.Day, false);
        bg_day = (getme == Color.red) ? bg_day : getme;

        float glowy_intensity_day = 25f / 255f;
        getme = Monitor.Instance.GetColorSetting(TimeName.Day, true);
        glowy_intensity_day = (getme == Color.red) ? glowy_intensity_day : getme.a;




        Color light_day = new Color(1f, 1f, 1f, 0f);
        Color light_dawn = new Color(1f, 179f / 255f, 105f / 255f, 70f / 255f);

        basic_times = new List<TimeOfDay>();

        basic_times.Add(new TimeOfDay(TimeName.Reset, Color.black, bg_night, glowy_intensity_night, new Vector3(20f, -1.33f)));   // prepare for dawn
        basic_times.Add(new TimeOfDay(TimeName.Dawn, light_dawn, bg_dawn, glowy_intensity_dawn, new Vector3(10, -1.33f)));
        basic_times.Add(new TimeOfDay(TimeName.Day, light_day, bg_day, glowy_intensity_day, new Vector3(0f, 6f)));
        basic_times.Add(new TimeOfDay(TimeName.Dusk, light_dawn, bg_dawn, glowy_intensity_dawn, new Vector3(-10f, -1.33f)));
        basic_times.Add(new TimeOfDay(TimeName.Night, Color.black, bg_night, glowy_intensity_night, new Vector3(-20f, -1.33f)));// make it disappear yo		

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

