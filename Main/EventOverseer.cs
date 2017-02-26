using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;







//addwish just adds 1, makewish makes the actual potion/wish floaty
public enum ActionType { Panel, StopTween, RemoveObject, HideUIElement, RemoveEventObjects, DisableUIElement, AddWish, Pause, Resume, UnlockToy, DisableMonitor, PointSpyGlass, EnableSpyGlass, EnableReward, MakeFloaty, MakeWish, PanelPause, GiveSpecialSkill};



public class EventOverseer : MonoBehaviour
{
    //public Central central;
    //public EagleEyes eyes;
    //public Peripheral peripheral;
    public List<GameEvent> events = new List<GameEvent>();
   // public List<GameEvent> intra_level_events = new List<GameEvent>();
    public List<GameEvent> concurrent_events = new List<GameEvent>();
    public bool ingame;
    public int level;
    public GameEvent toRun;
    public int next_event = 0;
    public int current_event = 0;
   // public int current_intra_map_event = 0;
    public bool ingame_finished = false;
    public bool initialized;
    public bool reward_overseer = false;
    private static EventOverseer instance;
    // Use this for initialization
    public static EventOverseer Instance { get; private set; }

    void Start()
    {
      
        //DoStuff();

    }

    public virtual void StartMe(bool _ingame)
    {
  //      Debug.Log("Starting overseer, it is already initialized? " + initialized + "\n");
        if (initialized) return;
            if (_ingame != ingame) current_event = 0;
        if (toRun != null) toRun.StopAllCoroutines();
     //   Debug.Log("Overseer starting (ingame = " + _ingame + ")\n");
        ingame = _ingame;
        GameEvent.onEventComplete += onEventComplete;
        DoStuff();
        initialized = true;
    }



  

    public void StopMe()
    {
        GameEvent.onEventComplete -= onEventComplete;
        if (toRun != null) toRun.StopAllCoroutines();
        ingame_finished = true;
        initialized = false;

    }

    public void SetEvent(int i, bool _ingame)
    {
     //   Debug.Log("Setting overseer event to " + i + "\n");
        for (int m = i; m < events.Count; m++)
        {
            events[m].is_waiting = true;
        }
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







    void onEventComplete(string name, bool reward_overseer)
    {
        if (reward_overseer != this.reward_overseer) return;

     //   Debug.Log("Reward? " + this.reward_overseer + " Registered on eventcomplete " + name + "\n");
        bool ok = false;
        //  Debug.Log("onEventComplete " + name + "\n");
        
        if (!toRun.my_name.Equals(name) || !toRun.gameObject.activeSelf) return;

        if (ingame)
        {
            foreach (GameEvent my_event in events)
                if (my_event.my_name.Equals(name)) ok = true;

        }
        if (!ok) return;

   //     Debug.Log("Gonna increment event\n");
        current_event++;
        next_event++;
        toRun.gameObject.SetActive(false);
        DoStuff();

    }


}





