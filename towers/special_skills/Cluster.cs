using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Cluster{
    List<Transform> objects = new List<Transform>();
    public Lava my_lava;
    public Vector3 center;
    public bool has_stuff;

    public bool contains(int ID)
    {
        foreach (Transform t in objects)
        {
            if (t.gameObject.GetInstanceID() == ID) return true;
        }
        return false;
    }

    public void addObject(Transform t)
    {
        has_stuff = true;
        objects.Add(t);
    }

    public void updateLava()
    {
        CheckObjects();
        if (!has_stuff)
        {
            //Debug.Log("Cluster is empty\n"); 
            my_lava.KillMe();
            return;
        }
        if (my_lava == null || my_lava.walk == null) return;
       
        updateCenter();
        my_lava.walk.UpdateMe(center);
    }


    public void startLava()
    {
      //  Debug.Log("want to start lava\n");
        if (!has_stuff)
        {
            Debug.Log("Cluster is empty\n"); return;
        }
        if (my_lava == null || my_lava.walk == null) return;

        updateCenter();
        my_lava.walk.StartMe(center);
    //    Debug.Log("Started lava\n");
    }

    public void updateCenter()
    {
        if (!has_stuff)
        {
            Debug.Log("Cluster is empty\n"); return;
        }
        center = Vector3.zero;
        float x_total = 0f;
        float y_total = 0f;
        float z_total = 0f;
      

        foreach (Transform o in objects)
        {         
            x_total += o.position.x;
            y_total += o.position.y;
            z_total += o.position.z;
        }
        center.x = x_total / objects.Count;
        center.y = y_total / objects.Count;
        center.z = z_total / objects.Count;
        
    }

    public void CheckObjects()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            if (!objects[i].gameObject.activeSelf)
            {
                objects.RemoveAt(i);
            }
        }
        if (objects.Count == 0) { has_stuff = false; return; }
    }
		
	
}