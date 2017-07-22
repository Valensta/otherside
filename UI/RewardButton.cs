using System;
using UnityEngine;
using UnityEngine.UI;

public class RewardButton : UIButton {
	public string type = "";
	public string content = "";
   
	public Button my_button;	
	public bool selected = false;	
	public GameObject parent;       
	public MyLabel label;
    public GameEvent game_event;
    public RewardOverseer rewardOverseer;
    public RewardList_Toy_Button_Driver driver;
    public override void InitMe(){		

	}
    
	void OnEnabled(){
        rewardOverseer = RewardOverseer.RewardInstance;
		InitMe();
	}

    

    void Start()
    {
        InitMe();
        enabled = true;
    }
   
		
        		
	public override void SetSelectedToy(bool selected){
        this.selected = selected;
	}

	public void OnClick()
	{
	    if (EagleEyes.Instance.UIBlocked("RewardButton","")) return;
		OnInput();
	}

	public void OnInput(){
		if (driver == null) { Debug.LogError(this.name + " has no driver assigned!\n"); return; }

        selected = !selected;
        

        if (selected)
            driver.setSelectedButton(this);
        else
            driver.setSelectedButton(null);
        
	}

    public override void InitStartConditions()
    {
        
    }

    public override void SetInteractable(bool set)
    {
        
    }

    public override void Reset()
    {
        throw new NotImplementedException();
    }
}

