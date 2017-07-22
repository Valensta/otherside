using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameSaver : MonoBehaviour {

    public List<IslandSaver> islands = new List<IslandSaver>();
    public float dreams;
    public float health;
    public float sensible_wish;
    public float airy_wish;
    public float vexing_wish;
		
    public GameSaver() { }
	
	public void Init(float _dreams, float _health, float _sens, float _airy, float _vex)
	{
        dreams = _dreams;
        health = _health;
        sensible_wish = _sens;
        airy_wish = _airy;
        vexing_wish = _vex;

        foreach (Island_Button island in Monitor.Instance.islands.Values)
        {
            Debug.Log("Checking island " + island.name + "\n");
            IslandSaver saver = island.getSnapshot();
            if (saver != null) islands.Add(saver);

        } 
	}



	

}