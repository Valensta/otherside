using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;


[System.Serializable]

public class EnemyID{
	public int ID;	
	public DogTag Tag;
	
	public EnemyID(int _id, DogTag _tag){
		ID = _id;
		Tag = _tag;	
	}
}
[System.Serializable]


public class EnemyList
{
    public Dictionary<int, EnemyID> list;
    int count;

    public EnemyList()
    {
        list = new Dictionary<int, EnemyID>();
        count = 0;
    }

    public void addMonster(int id, DogTag tag)
    {
        if (list.ContainsKey(id)) return;        
        list.Add(id, new EnemyID(id, tag));
        count = list.Count;
    }

    public float getID(int id)
    {
        EnemyID ID = null;
        list.TryGetValue(id, out ID);
        if (ID == null) return 0;
        if (ID.Tag == null) return 0;
        return ID.Tag.getID();        
    }
}


/*
public class EnemyList{
	public List<EnemyID> list;
    int count;
	
	public EnemyList(){
		list = new List<EnemyID>();
        count = 0;
	}



    public void addMonster(int id, DogTag tag)
    {
        for (int x = 0; x < list.Count; x++)
        {
            if (list[x].ID == id) return;
        }
        list.Add(new EnemyID(id, tag));
        count = list.Count;
    }
	
	public float getID(int id){
        for (int x = 0; x < count; x++) { 
            EnemyID i = list[x];
			if (i.ID == id){
				if (i.Tag != null) 
				{
					return i.Tag.getID();
				}
				else 
				{
					return 0;
				}
			}	
		}
		return 0;
	}
}
*/
