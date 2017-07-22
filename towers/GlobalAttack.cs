using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
//does not use lava, this is more of a precision strike against specific enemies, not an area attack
public class GlobalAttack : MonoBehaviour {

	Dictionary<string, HitMe> monsters;	
	bool am_enabled = true;	
	public StatSum my_stats;	
	public float percent_enemies = 0.25f;

    public MonsterType visuals = MonsterType.Burning;		

	void Start(){
		
	}
	
    public void Init(StatSum s)
    {
        my_stats = s;
        am_enabled = true;
    }

	

    public void TurnMeOn()
    {
        am_enabled = true;
        
    }
	
    bool _contains(HitMe[] list, string name)
    {
        
        foreach(HitMe h in list)
        {
            if (h != null && h.name.Equals(name)) return true;
        }
        return false;
    }
   
    
}
