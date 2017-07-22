using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class GameStatCollector : MonoBehaviour { 
	private static GameStatCollector instance;
	public static GameStatCollector Instance { get; private set; }
	public List<tower_stats> tower_snapshot;
    string save_stats;
    
    public List<string> castle_invaded;

    void onHealthChanged(int i)
    {
        save_stats += "Health: " + i.ToString() + " Duration: " + Time.realtimeSinceStartup.ToString() + " Monster Count: " +
            Peripheral.Instance.monster_count.ToString() + "\n";
    }


    void Awake(){
		Instance = this;
		Init ();
        //stat_file = Application.persistentDataPath + "/stats" + System.DateTime.Today.ToString() + SaveData.extension;
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
     //   foreach (string s in castle_invaded) { Debug.Log(s + "\n"); }
	}
	

	
	string LogHeader(){
		return "Level: " + Central.Instance.current_lvl + " Wave: " + Moon.Instance.GetCurrentWave() + "\n";
	}
	
	public void CastleInvaded(string what)
    {
        castle_invaded.Add(what);
    }
	
}//end of class
