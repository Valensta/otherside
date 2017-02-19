using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class GameStatCollector : MonoBehaviour { 
	private static GameStatCollector instance;
	public static GameStatCollector Instance { get; private set; }
	public List<tower_stats> tower_snapshot;
    string save_stats;
    string stat_file;
    public List<string> castle_invaded;
    	
    void onHealthChanged(int i) {
        save_stats += "Health: " + i.ToString() + " Time: " + Time.realtimeSinceStartup.ToString() + " Monster Count: " +
            Peripheral.Instance.monster_count.ToString() + "\n";
}

    

    public void SaveStatFile()
    {

    }

    void Awake(){
		Instance = this;
		Init ();
        stat_file = Application.persistentDataPath + "/stats" + System.DateTime.Today.ToString() + ".uml";
    }
	
	public void Init(){
	//Debug.Log("Initializing GameStatCollector\n");
		tower_snapshot = new List<tower_stats>();
		
	}
	
	public tower_stats addTowerStats(Toy firearm){
        tower_stats hey = new tower_stats();
        hey.ID = firearm.GetInstanceID();
        hey.name = firearm.gameObject.name;

        tower_snapshot.Add(hey);
        return hey;
	}
	
	public void printStats(){
        //Debug.Log(LogHeader() + "\n");

        //printXP();
        //Debug.Log("Tweens running: " + LeanTween.tweensRunning + "\n");
    //    Debug.Log("Wishes: " + ScoreKeeper.Instance.getPossibleWishes());
   //     Debug.Log("Invaders:\n");
        foreach (string s in castle_invaded) { Debug.Log(s + "\n"); }
	}
	
	public void printXP(){
		
		for (int i = 0; i < tower_snapshot.Count; i++){
			if (tower_snapshot[i].ID == 0){return;}
			Debug.Log(tower_snapshot[i].name + " XP: " + tower_snapshot[i].Xp + "\n");
		}
	}
	
	string LogHeader(){
		return "Level: " + Central.Instance.current_lvl + " Wave: " + Peripheral.Instance.current_wave + "\n";
	}
	
	public void CastleInvaded(string what)
    {
        castle_invaded.Add(what);
    }
	
}//end of class

[System.Serializable]
public class tower_stats
{
    private int id = 0;
    public string name = "";
    public float xp = 0;
    public int hits = 0;
    public int shots_fired = 0;
    private int lava_count = 0;
    private float lava_xp = 0;

    public float Xp
    {
        get
        {
            return xp;
        }

        set
        {
            xp = value;
        }
    }

    public int Hits
    {
        get
        {
            return hits;
        }

        set
        {
            hits = value;
        }
    }

    public int Lava_count
    {
        get
        {
            return lava_count;
        }

        set
        {
            lava_count = value;
        }
    }

    public float Lava_xp
    {
        get
        {
            return lava_xp;
        }

        set
        {
            lava_xp = value;
        }
    }

    public int ID
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
        }
    }

    public int Shots_fired
    {
        get
        {
            return shots_fired;
        }

        set
        {
            shots_fired = value;
        }
    }

    public tower_stats()
    {
        id = 0;
        Xp = 0;
        Hits = 0;
    }
}