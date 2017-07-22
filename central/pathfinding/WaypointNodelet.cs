using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WaypointNodelet 
{
    public Vector3 position;
    public int ID = 0;
    public WaypointNodelet()
    {
        //Empty node
    }

    public WaypointNodelet(Vector3 p, int id)
    {
        position = p;
        ID = id;
    
    }

    public WaypointNodelet(WaypointListNode node)
    {
        position = node.position;
        ID = node.ID;
    }


    public WaypointNodelet(WaypointNode node)
    {
        position = node.position;
        ID = node.ID;
    }


}


