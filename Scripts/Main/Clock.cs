using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*

public class Clock : MonoBehaviour {	
	private static Clock instance;
	
	public float TIME;
	
	public delegate void OnWaveEndHandler(int content);
	public static event OnWaveEndHandler onWaveEnd;

    public delegate void OnLastWaveletHandler(int content);
    public static event OnLastWaveletHandler onLastWavelet;

    public static Clock Instance { get; private set; }

    Peripheral my_peripheral;

    public List<VariableStat> effects;

	void Awake()
	{   
		Debug.Log ("Clock awake\n");	
		if (Instance != null && Instance != this) {
			Debug.Log ("Clock got destroyed");
			Destroy (gameObject);
		}
		Instance = this;
        
	}

    public void AddEffect()
    {
        if (my_peripheral == null) my_peripheral = Peripheral.Instance;
    }

    public void EnableEffect(WishType e, float eff, bool enable)
    {
        switch (e)
        {
            case WishType.MoreXP:
                if (enable)
                {
                    my_peripheral.xp_factor *= eff;
                }
                else
                    my_peripheral.xp_factor *= eff;
                break;
            default:
                Debug.Log("Clock does not know what to do with wishtype " + e.type + "\n");
                break;
        }

    }

	void Update()
    {
        if (effects.Count == 0) return;

        float time = Time.deltaTime;

        foreach (TemporaryEffect e in effects)
        {
            if (e.remaining_time > 0) e.remaining_time -= time;
            else
            {
                
                

            }


        }
    }

}
   //end of class
   */
