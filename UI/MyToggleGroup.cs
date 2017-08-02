using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MySelectable : UIButton
{
    private bool selected;
    public MyToggleGroup myToggleGroup;
    public bool interactable;
    public bool toggle_button;
    public bool can_untoggle;

    private void OnEnable()
    {
        ShowSelectedAccent(selected);
    }

    public bool Selected
    {
        get { return selected; }
        set
        {
            if (!interactable) return;
            
            selected = value; 
            ShowSelectedAccent(selected);
            ActionOnSelected(selected);
        }
    }

    public bool Interactable
    {
        get { return interactable; }
        set { interactable = value; }
    }

    public virtual void ActionOnSelected(bool set)
    {        
    }
        

    public void OnClicked()
    {
        if (!interactable) return;
        if (toggle_button && !can_untoggle && Selected) return;
        if (myToggleGroup)
        {
            myToggleGroup.setToggle(this);
        }
        else
        {
            Selected = !Selected;    
        }
        
    }
    
   

}

public class MyToggleGroup : MonoBehaviour
{
    public List<MySelectable> toggles;


    
    public void setToggle(MySelectable setMe)
    {
        
        int ID = setMe.GetInstanceID();
        bool turnMeOn = !setMe.Selected;
        Debug.Log($"MyToggleGroup toggled {setMe.gameObject.name}, turning it on {turnMeOn}\n");
        foreach (MySelectable button in toggles)
        {

            if (button.GetInstanceID() == ID)
            {
                button.Selected = turnMeOn;
                Debug.Log($"{button.gameObject.name} selected {turnMeOn}\n");
            }
            else if (turnMeOn)
            {
                Debug.Log($"{button.gameObject.name} UNselected\n");
                button.Selected = false;
            }
            

        }
    }
}