using UnityEngine;
using System.Collections;


public class AutoCorrectZ : MonoBehaviour{
	public float z = 0f;
    
    void Awake()
    {
        Vector3 pos = this.transform.position;
        pos.z = z;
        this.transform.position = pos;
    }

}