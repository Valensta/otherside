using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

[System.Serializable]

public class WavePipController : MonoBehaviour {
	
	public List<WavePip> pips;
    int current_pip = 0;
    public Image current_pip_indicator;

    public void Awake()
    {
        
        pips = new List<WavePip>();
        foreach (Transform t in transform)
        {
            if (t.gameObject.name == current_pip_indicator.gameObject.name) continue;
            WavePip p = t.GetComponent<WavePip>();
            if (p != null) { pips.Add(t.GetComponent<WavePip>()); }
        }
    }

    public void Init()
    {
        if (Sun.Instance == null)
        {
            Debug.Log("No Sun to initialize the Wave pip controller from\n");
            return;
        }

        Reset();
       // Debug.Log("Initializing wave pip controller\n");
        List<WaveTime> waves = Sun.Instance.times;
        TimeName current_time_name = TimeName.Day;
        for (int i = 0; i< waves.Count; i++)
        {
            if (waves[i].time_name_start.name != null)
            {
                InitPip(i, waves[i].time_name_start.name);
                
                current_time_name = waves[i].time_name_start.name;

            }
            else
            {
                InitPip(i, current_time_name);
            }
            
       //     Debug.Log("wave " + i + " is " + pips[i].timename);
        }

        SetCurrentWave(Sun.Instance.GetWave());
    }

    public void Reset()
    {//disable all 
       
        foreach (WavePip p in pips)
        {
            p.Init(-1, TimeName.Day);
        }
    }

    public void SetCurrentWave(int i)
    {
        
     if (current_pip != i) { pips[i].Init(-1, TimeName.Day); }
        current_pip = i;
        SetCurrentPipIndicator();
    }

    public void SetCurrentPipIndicator()
    {
        //Debug.Log("Setting current pip indicator to " + current_pip + " " + pips[current_pip].gameObject.name + " " + pips[current_pip].transform.localPosition + "\n");


        //current_pip_indicator.transform.localPosition = pips[current_pip].transform.localPosition;
        current_pip_indicator.gameObject.transform.SetParent(pips[current_pip].transform);
        current_pip_indicator.gameObject.transform.localPosition = Vector3.zero;
        current_pip_indicator.gameObject.transform.localScale = Vector3.one;
    }

    public void InitPip(int i, TimeName name) {
        pips[i].Init(i, name);
        
        
    }
    
	
}