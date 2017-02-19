using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;


[System.Serializable]
public class GameEvent : MonoBehaviour {
	public float time_delay = 0;
	float init_time;
	public List<RegularTrigger> triggers = new List<RegularTrigger> ();
    public RewardTrigger reward_trigger;
    public List<GameAction> actions = new List<GameAction>();	
	public delegate bool run_event();
	public float check_interval = 0.25f;
	run_event myRunEvent;
	public string my_name;
    public bool is_waiting;
    public bool is_concurrent;
    //public bool am_active;
    public bool is_recurring = false;
    public bool reward = false;
    public bool auto_pause = false;

	public delegate void EventFinishedHandler(string name);
	public static event EventFinishedHandler onEventComplete;

    List<Trigger> use_triggers = new List<Trigger>();

	public void setName(string n){my_name = n;}

	public void setTimeDelay(float t){time_delay = t;}

	//public void AddStartCondition(Condition c, string text, float n){triggers.Add (new RegularTrigger (c, text, n));}
	//public void AddStartCondition(Condition c, string t){triggers.Add (new RegularTrigger (c, t));}

	//public void AddStartCondition(Condition c, float f) {triggers.Add(new RegularTrigger(c,f));}
	//public void AddStartCondition(Condition c, TimeName f) {triggers.Add(new RegularTrigger(c,f));}

	public void AddAction(GameAction action)
	{actions.Add (action);}

	public void AddAction(ActionType action, string text, string name, float number, GameObject target,bool boolean)
	{actions.Add (new GameAction (action));}

    public bool isActivated()//meaning this gameevent already occurred
    {
        return (is_concurrent && !is_waiting);
    }

    void OnEnable()
    {
        if (!is_concurrent && is_recurring)
        {
            Debug.Log(this.gameObject.name + " event is not concurrent but is recurring. Incorrect. Disabling is recurring.\n");
            is_recurring = false;
        }
        if (reward)
        {
            use_triggers.Add((Trigger)reward_trigger);
           // check_interval = 3f;
        }
        else
        {
            foreach (RegularTrigger t in triggers) use_triggers.Add((Trigger)t);
        }
    }

	IEnumerator WaitForStart(){
		foreach (Trigger t in use_triggers)
        {
   //         Debug.Log("Initializing trigger " + t.condition + "\n");
            t.Init();
        }

        int togo = use_triggers.Count;
		//Debug.Log("Event " + name + " has " + togo + " conditions\n");
		while (togo > 0) {
			for (int i = 0; i < use_triggers.Count; i++) {

				if(use_triggers[i] != null){			
					bool ok = use_triggers [i].Check ();			
					while (!ok){										
						yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(check_interval));
						ok = use_triggers [i].Check ();
					}
                    togo--;		
				}
			}						
		}
	
		foreach (GameAction a in actions) {	
			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(time_delay));
            
            //need to be able to pass info from trigger to action
            if (reward && reward_trigger.reward_type == RewardType.Killer && (a._type == ActionType.MakeFloaty || a._type == ActionType.MakeWish))
                a._vector = reward_trigger.vector;

            a.Do ();
		}
        is_waiting = false;
       // if (auto_pause) Peripheral.Instance.Pause(true);
        onEventComplete (my_name);
        if (is_recurring) RunEvent();
		yield return null;
		
	}
	

	public void Abort(){
		StopAllCoroutines();
		this.enabled = false;
        is_waiting = false;
	}

	public void RunEvent(){
        is_waiting = false;        
     //   Debug.Log("Running event " + this.name + "\n");
        StartCoroutine (WaitForStart());	
	}


}
	





	