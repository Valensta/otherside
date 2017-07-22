using UnityEngine;
using System.Collections.Generic;
//using UnityEditor;



[System.Serializable]
public class GameSound {
	//public AudioClip sound = new AudioClip();
	public string name = "";
    public bool is_continuous;


    public AudioSource[] audio_sources;
    private float[] audio_source_volumes;


    public void Stop()
    {
        foreach (AudioSource a in audio_sources) a.Stop();     
    }

    public void Stop(int i)
    {       
        audio_sources[i].Stop();
    }

    public void Play(int i){		
        
		audio_sources[i].Play();
	}
	
	public void Play(){
		int i = 0;
		if (audio_sources.Length > 0) i = (int)Random.Range(0, audio_sources.Length);
		Play (i);
	}

    
}

public class Noisemaker : MonoBehaviour {
    public static Noisemaker Instance { get; private set; }
    public List<GameSound> sounds = new List<GameSound>();
    private static Noisemaker instance;

    [Range(0, 10)]
    public int global_volume = 0;
    public bool mute = false;

    public void setMute(bool set) { mute = set; }

    public bool isMute() { return mute;}

    void Awake(){
		if (Instance != null && Instance != this) {
			Debug.Log ("Noisemaker got destroyeed\n");
			Destroy (gameObject);
		}		
		Instance = this;
        //    Debug.Log("Starting @  " + AudioListener.volume + "\n");
        //AudioListener.volume = 0.5f +  global_volume * 0.05f;
        AudioListener.volume = global_volume * 0.1f;
        //   Debug.Log("Now at @  " + AudioListener.volume + "\n");
    }

    public void AdjustVolume()
    {
        
    }

    public void Play(string name)
    {
        if (mute) return;

        //bool found = false;
        foreach (GameSound s in sounds)
        {
            if (name == s.name)
            {
                if (s.audio_sources.Length > 0)
                {
                    s.Play();
                 //   found = true;
                }
            }
        }
   //     if (!found) Debug.Log("Could not find sound: " + name + "\n");
    }

    public void Stop()
    {
        foreach (GameSound s in sounds)
        {
            if (s.is_continuous) s.Stop();
        }
    }

    public void Stop(string name)
    {

        bool found = false;
        foreach (GameSound s in sounds)
        {
            if (name == s.name)
            {
                if (s.audio_sources.Length > 0)
                {
                    s.Stop();
                    found = true;
                }
            }
        }
        if (!found) Debug.Log("Could not find sound: " + name + "\n");
    }



    public void Click(ClickType type)
    {
        switch (type)
        {
            case ClickType.Success:
                Play("success_click");
                break;
            case ClickType.Error:
                Play("error_click");
                break;
            case ClickType.Action:
                Play("action_click");
                break;
            case ClickType.Cancel:
                Play("cancel_click");
                break;
            case ClickType.Null:
                Debug.Log("NULL CLICK NOISE\n");
                break;
        }

    }



}