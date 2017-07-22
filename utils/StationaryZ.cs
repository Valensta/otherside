using UnityEngine;
using System.Collections;


public class StationaryZ : MonoBehaviour
{
    Transform my_transform;
    float Z_upper = 2;
    float Z_lower = 1;
    float y_range;
    

    private void OnEnable()
    {
        if (Monitor.Instance == null) return;
        my_transform = this.transform;
        y_range = Monitor.Instance.my_spyglass.map_y_size / 2;
    }

    public void Init()
    {    
     
        SetZ(0f);
    }

    public void SetZ(float offset) { 

        Vector3 pos = my_transform.position;

        

        float new_z = (Z_upper + Z_lower) / 2f - pos.y / y_range * (Z_upper - Z_lower) / 2 + offset;

     //   Debug.Log("Stationary Z " + this.gameObject.name + " " + my_transform.position + " " + y_range + " -> " + new_z + "\n");

        pos.z = new_z;
        my_transform.position = pos;

    }
}