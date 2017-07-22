using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public abstract class Interactable : MonoBehaviour
{   
    
    public StatBit interactable_skill;
    public Firearm my_firearm;
    public abstract void Deactivate();
    //public abstract void Activate(float[] stats);
    public abstract void Activate(StatBit skill); 
    public abstract void Simulate(List<Vector2> positions);
    public abstract void Reset();
    public bool am_active;


    
}


