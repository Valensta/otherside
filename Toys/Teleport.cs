using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;

public class Teleport: MonoBehaviour {
	public AI my_ai;
	Dictionary<int, Vector3> path = new Dictionary<int, Vector3> ();
	int smallest = 100;
	int biggest = -1;
	
	

	
	public void Init(HitMe hitme){
		
		my_ai = hitme.my_ai;
		
		foreach (WaypointNode p in WaypointMultiPathfinder.Instance.paths[my_ai.path].Map) {				 
			path.Add(p.order, p.transform.position);
			if (smallest > p.order){smallest = p.order;}
			if (biggest < p.order){biggest = p.order;}				
		}


        hitme.EnableVisuals(MonsterType.Teleport, 1f);
    }
		
	
		
	public float TeleportMe(float[] stats){
        //where is from -1 to 1, -1 is beginning of path, 1 is end of path
        //aff is probability of teleportation, 0 to 1
        float aff = -stats[0]*1.5f;
        float where = -stats[1];
	//	Debug.Log("Teleporting: %: " + aff + " where: " + where + "\n");
		float prob = UnityEngine.Random.Range (0, 1f);
		//if (prob >= aff) 
		//{
		//	Debug.Log("No go on teleport, aff " + aff + ", rolled a " + prob + "\n");
      //      return 0;
		//}
		
	
		Vector3 next_waypoint = my_ai.Path [0].position;
        int next_waypoint_order = my_ai.Path[0].ID;
     //   Debug.Log("Teleporting to " + next_waypoint_order + "\n");

        float new_center = next_waypoint_order;
		if (where > 0) {	
			new_center = next_waypoint_order + (biggest - next_waypoint_order) * where;
		} else {
			new_center = next_waypoint_order + (next_waypoint_order - smallest) * where;
		}

		float width = (biggest - smallest)/2;
		float random = Get.RandomNormal()*width; 	
	//	Debug.Log ("pure random is " + random);
		//random += (new_center - width); // shift to be centered around current order
		random += new_center;
	//	Debug.Log ("Random premature is " + random);
		int new_order = 0;
		if (where <= 0) new_order = Mathf.FloorToInt (random);
		if (where > 0) new_order = Mathf.CeilToInt (random);
		//	Debug.Log ("new order is " + new_order);
		if (new_order < smallest) new_order = smallest;
		if (new_order > biggest) new_order = biggest;
		//	Debug.Log ("new order is " + new_order);
		Vector3 new_pos;
		path.TryGetValue (new_order, out new_pos);
		
				
		this.transform.position = new_pos;
		my_ai.getNewPath (true);

        return (aff/2f);
	}
	

}
