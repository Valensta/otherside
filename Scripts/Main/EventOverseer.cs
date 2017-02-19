using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

//addwish just adds 1, makewish makes the actual potion/wish floaty
public enum ActionType { Panel, StopTween, RemoveObject, HideUIElement, RemoveEventObjects, DisableUIElement, AddWish, Pause, Resume, ActivateToy, DisableMonitor, PointSpyGlass, EnableSpyGlass, EnableReward, MakeFloaty, MakeWish, PanelPause};



public class EventOverseer : MonoBehaviour
{
    //public Central central;
    //public EagleEyes eyes;
    //public Peripheral peripheral;
    public List<GameEvent> events = new List<GameEvent>();
    public List<GameEvent> intra_level_events = new List<GameEvent>();
    public List<GameEvent> concurrent_events = new List<GameEvent>();
    public bool ingame;
    public int level;
    GameEvent toRun;
    public int next_event = 0;
    public int current_event = 0;
    public int current_intra_map_event = 0;
    public bool ingame_finished = false;
    public bool initialized;
    private static EventOverseer instance;
    // Use this for initialization
    public static EventOverseer Instance { get; private set; }

    void Start()
    {
        GameEvent.onEventComplete += onEventComplete;
        //DoStuff();

    }

    public virtual void StartMe(bool _ingame)
    {
        if (_ingame != ingame) current_event = 0;
        if (toRun != null) toRun.StopAllCoroutines();
     //   Debug.Log("Overseer starting (ingame = " + _ingame + ")\n");
        ingame = _ingame;
        
        DoStuff();
        initialized = true;
    }



  

    public void StopMe()
    {
        GameEvent.onEventComplete -= onEventComplete;
        if (toRun != null) toRun.StopAllCoroutines();
        ingame_finished = true;

    }

    public void SetEvent(int i, bool _ingame)
    {
        next_event = i;
        current_event = next_event;
        ingame = _ingame;
        ingame_finished = !ingame;

    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("overseer got destroyeed\n");
            Destroy(gameObject);
        }

        Instance = this;
        //   Debug.Log("Overseer waking up\n");
        foreach (GameEvent ge in concurrent_events)
        {
            ge.is_waiting = true;
        }
    }

    protected void DoStuff()
    {

        if (!ingame)
        {
            if (current_event < intra_level_events.Count)
            {
                toRun = intra_level_events[current_event];
                toRun.gameObject.SetActive(true);
                toRun.RunEvent();
            }
            return;
        }
        /////////////////////

        foreach (GameEvent ge in concurrent_events)
        {
            if (ge.is_waiting && (!ge.reward || (ge.reward && !ge.reward_trigger.getReward().unlocked)))
            {
           //     Debug.Log("Running Game Event " + ge.name + "\n");
                ge.gameObject.SetActive(true);
                ge.RunEvent();
            }
        }

        if (current_event < events.Count)
        {
            //      Debug.Log("Running event " + current_event + "\n");
            Peripheral.Instance.ResumeNormalSpeed();
            toRun = events[current_event];
            toRun.gameObject.SetActive(true);
            toRun.RunEvent();
        }
        else
        {
            ingame_finished = true;
        }
    }







    void onEventComplete(string name)
    {
        bool ok = false;
        //  Debug.Log("onEventComplete " + name + "\n");
        if (ingame)
        {
            foreach (GameEvent my_event in events)
                if (my_event.my_name == name) ok = true;

        }
        else
        {
            foreach (GameEvent my_event in intra_level_events)
                if (my_event.my_name == name) ok = true;

        }
        if (!ok) return;

        current_event++;
        next_event++;
        toRun.gameObject.SetActive(false);
        DoStuff();

    }


}





