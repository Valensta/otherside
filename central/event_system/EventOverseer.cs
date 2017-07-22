using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;







//addwish just adds 1, makewish makes the actual potion/wish floaty
public enum ActionType { Panel, StopTween, RemoveObject, HideUIElement, RemoveEventObjects, DisableUIElement, AddWish, Pause, Resume, UnlockToy, DisableMonitor, PointSpyGlass, EnableSpyGlass, EnableReward, MakeFloaty, MakeWish, PanelPause, GiveSpecialSkill, EnemyDescription, UIFilter, AddDream, SetSensibleWishUplift, ShowIslands};



public class EventOverseer : MonoBehaviour
{
    //public Central central;
    //public EagleEyes eyes;
    //public Peripheral peripheral;
    public List<GameEvent> events = new List<GameEvent>();
   // public List<GameEvent> intra_level_events = new List<GameEvent>();
    public List<GameEvent> concurrent_events = new List<GameEvent>();
    
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

    public virtual void StartOverseer()
    {
 
        if (initialized) return;
        initialized = true;

        if (!reward_overseer && ScoreKeeper.Instance.checkIfAlreadyHaveScore())
        {
            ingame_finished = true;
            Debug.Log("ALready have a score for this level, " + this.gameObject.name + " is not started.\n");
            return;
        }
          //  Debug.Log(this.gameObject.name + " Overseer is STARTING\n");



        if (toRun != null) toRun.StopAllCoroutines(); 
 
        GameEvent.onEventComplete += onEventComplete;

        foreach (GameEvent ge in concurrent_events)
        {
            if (ge.is_waiting && (!ge.reward || (ge.reward && ge.my_reward != null && !ge.my_reward.unlocked)))
            {
                //     Debug.Log("Running Game Event " + ge.name + "\n");
                ge.gameObject.SetActive(true);
                ge.RunEvent();
            }
        }

        DoStuff();
        
    }



  

    public void StopOverseer()
    {
        if (!initialized) return;
     //   Debug.Log(this.gameObject.name + " Overseer is STOPPING\n");
        initialized = false;
        GameEvent.onEventComplete -= onEventComplete;
        if (toRun != null) toRun.StopAllCoroutines();
        ingame_finished = true;
        
        foreach (GameEvent ge in concurrent_events)
        {
            ge.Abort();
        }

    }

    public void SetEvent(int i)
    {
     //   Debug.Log("Setting overseer event to " + i + "\n");
        for (int m = 0; m < i; m++)
        {
            events[m].is_waiting = false;
            events[m].gameObject.SetActive(false);
        }

        for (int m = i; m < events.Count; m++)
        {
            events[m].is_waiting = true;
            events[m].gameObject.SetActive(true);
        }

        next_event = i;
        current_event = next_event;
        
        

    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("overseer got destroyeed\n");
            Destroy(gameObject);
        }

        Instance = this;
           Debug.Log(this.gameObject.name + " Overseer waking up\n");
        foreach (GameEvent ge in concurrent_events)
        {
            ge.is_waiting = true;
        }
    }

    protected void DoStuff()
    {
     



        if (current_event < events.Count)
        {
            //      Debug.Log("Running event " + current_event + "\n");
            //if (Peripheral.Instance != null) Peripheral.Instance.ChangeTime(TimeScale.Normal);
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
      //    Debug.Log("onEventComplete " + name + "\n");
        
        if (toRun == null || !toRun.my_name.Equals(name) || !toRun.gameObject.activeSelf) return;

        
            foreach (GameEvent my_event in events)
                if (my_event.my_name.Equals(name)) ok = true;

        
        if (!ok) return;

   //     Debug.Log("Gonna increment event\n");
        current_event++;
        next_event++;
        toRun.gameObject.SetActive(false);
        DoStuff();

    }


}





