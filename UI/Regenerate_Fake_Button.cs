using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class Regenerate_Fake_Button: MonoBehaviour
{
    public MySpecialButton button;

    void Start()
    {
        Peripheral.onHealthChanged += onHealthChanged;
    }
    

    void onHealthChanged(float i, bool visual)
    {
        //button interactability is controlled by 
        //Debug.Log("Fake button REGISTERED\n");
        if (i >= Peripheral.Instance.MaxHealth) return;
       // Debug.//Log("button.my_button.interactable " + button.my_button.interactable +
//                  " button.my_special.my_interactable.isActive() " +
  //                ((Regenerate_SpecialSkill) button.my_special.my_interactable).isActive() + "\n");
            if (button.my_button.interactable  && !((Regenerate_SpecialSkill)button.my_special.my_interactable).isActive())
        {        
             button.OnClick();            
        }
    }


    
}