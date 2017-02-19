using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
//using UnityEditor;

[System.Serializable]

public class RewardList_Toy_Button_Driver : MonoBehaviour
{
    //public List_Panel selected_skill_panel;
    public List<RewardButton> buttons;
    public RewardButton selected_button;
    public MyLabel verbose_label;
    public GameObject everything;
    public bool show_rewards;

    public void Start(){
        show_rewards = false;
	}

    
    public void Toggle()
    {
        show_rewards = !show_rewards;

        if (show_rewards)
        {
            everything.gameObject.SetActive(true);
            Init();
        }
        else DisableMe();
    }

    public void Init()
    {
        setSelectedButton(null);
        foreach (RewardButton b in buttons)
        {            
         //   b.my_button.enabled = b.game_event.isActivated();
        }
    }
    
    public void DisableMe()
    {
        setSelectedButton(null);
        everything.gameObject.SetActive(false);
    }
    
    
    public void setSelectedButton(RewardButton b)        
    {
        if (selected_button != null) selected_button.SetSelectedToy(false);
        selected_button = b;
        
        if (b == null)
            setText(null);
        else
        {        
            setText(b);                       
        }

       
    }
    
    void setText(RewardButton button)
    {
        
        if (button == null)
        {
         //   Debug.Log("SetText blank\n");
            foreach (MyText t in verbose_label.moretext)
            {
                t.setText("");
            }
        }
        else
        {
       //     Debug.Log("Setting text for " + button.game_event.reward_trigger.condition + "\n");

            verbose_label.getText(LabelName.Name).setText(GetText.getName(button.game_event.reward_trigger.getReward().reward_type));

            string[] req = new string[2];
            req[0] = button.game_event.reward_trigger.number.ToString();
            req[1] = button.game_event.reward_trigger.getReward().current_number.ToString();
            string requirement = Show.FixText(GetText.getLabel(button.game_event.reward_trigger.getReward().reward_type), req);
            if (button.game_event.reward_trigger.getReward().unlocked) requirement += " YOU ALREADY UNLOCKED THIS!";
            verbose_label.getText(LabelName.Requirement).setText(requirement);

            verbose_label.getText(LabelName.Reward).setText(GetText.getReward(button.game_event.reward_trigger.getReward().reward_type));

        }
     
    }
}