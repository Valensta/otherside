using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public abstract class Interactable : MonoBehaviour
{   
    
    public StatBit interactable_skill;

    public abstract void Deactivate();
    public abstract void Activate(float[] stats);


}


