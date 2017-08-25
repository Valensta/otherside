using System;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyButton : MySelectable {
    public Difficulty type = Difficulty.Null;
    
    bool am_initialized = false;
    public Button button;
    
    
    public override void InitButton()
    {

    }


    public override void ActionOnSelected(bool set)
    {
        if (set) setDifficulty();
    }

    
    public void setDifficulty()
    {
        Central.Instance.level_list.SetDifficulty(type);
    }

    public override void InitMe() {
        if (am_initialized) return;
    
    }


    public override void InitStartConditions()
    {
        
    }

    public override void SetInteractable(bool set)
    {
 //       Debug.Log($"DifficultyButton setInteractable {set} -> {button.gameObject.name}\n");
        Interactable = set;
        button.interactable = set;
    }
}