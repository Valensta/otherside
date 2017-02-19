using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;

[System.Serializable]


public class InGame_Toy_Button_Driver : Global_Toy_Button_Driver
{
    public MyLabel primary_label;
    public GameObject update_group;
    public MyLabel upgrade_cost_label;
    public GameObject sell_panel;
    public GameObject move_panel;
    public MyLabel sell_cost_label;

    void Start(){
		
		show = false;
        base.Init();
	}

    public override void Init()
    {
         Peripheral.onDreamsChanged += onDreamsChanged;
    }
    
	public void onDreamsChanged(float i, bool visual, Vector3 pos)
    {
        //if (drivers[current_driver].type != type) return; eh,they all currently use WishType sensible for upbrades        
		CheckUpgrades();
	}

    public override void setSelectedButton(Toy_Button select_me)
    {
        selected_button = select_me;

        EffectType selected_type = getSelectedEffectType();
        // Debug.Log("Setting selected button " + selected_type);

        foreach (Toy_Button b in drivers[current_driver].buttons)
        {
            bool ok = (b.effect_type == selected_type);
          //  Debug.Log("button " + b.effect_type + " is " + ok);
            b.selected = (ok);           
        }
        
        base.setSelectedButton(select_me);
        setUpgradeCost();
        setVerboseLabel();
    }

    public override void updateOtherLabels()
    {
        setPrimaryLabel();
    }

    void setUpgradeCost()
    {
      //  Debug.Log("setting upgrade cost\n");
        if (upgrade_cost_label.text == null) return;


        if (selected_button == null)
        {
         //   Debug.Log("non\n");
            upgrade_cost_label.text.text = "";
            update_group.SetActive(false);
            return;
        }
        Rune check_rune = parent.rune;
        if (check_rune == null)
        {
          //  Debug.Log("non\n");
            update_group.SetActive(false);
            upgrade_cost_label.text.text = "";
            return;
        }
    
        Cost upgrade_cost = check_rune.GetUpgradeCost(selected_button.effect_type);
        if (upgrade_cost == null)
        {
          //  Debug.Log("non\n");
            update_group.SetActive(false);
            upgrade_cost_label.text.text = "";
            return;
        }

       // Debug.Log("Upgrade cost " + upgrade_cost.cost + " " + upgrade_cost.type + "\n");
        update_group.SetActive(true);
        
        upgrade_cost_label.text.text = upgrade_cost.Amount.ToString();

    }

    public void SetParent(Toy p)
    {
        
        if (drivers[current_driver].button_map.Count == 0) { Init(); }
        parent = p;
        SetDriver(p.runetype);


        for (int i = 0; i < drivers[current_driver].buttons.Count; i++)
        {
            drivers[current_driver].buttons[i].toy_parent = parent;
        }
        base.SetParent(p);
        setPrimaryLabel();
        move_panel.SetActive(false);
        sell_panel.SetActive(false);

    }



    public override void DisableMe()
    {
        primary_label.gameObject.SetActive(false);
        base.DisableMe();
    }

    public void setPrimaryLabel()
    {
        if (primary_label == null) return;
        
        
        MyText primary_desc = primary_label.getText(LabelName.Null);

        string text = StaticRune.getPrimaryDescription(parent.rune);

        if (text.Equals(""))
        {
            primary_label.gameObject.SetActive(false);
            return;
        }
        else
        {
            primary_label.gameObject.SetActive(true);
            primary_desc.setText(text);
        }


        MyText tower_name = primary_label.getText(LabelName.Name);

        string my_name = StaticRune.getProperName(parent);           
        tower_name.setText(my_name);        


    }


    public void sellToy()
    {                       
        Peripheral.Instance.sellToy(parent, parent.getSellCost());
        toggleSell();
    }

    public void moveToy()
    {
        toggleMovePanel();
        Peripheral.Instance.sellToy(parent, parent.getSellCost());
        
    }

    public void toggleInfo(){
        bool isactive = drivers[current_driver].info_panel.activeSelf;
        drivers[current_driver].info_panel.SetActive(!isactive);
        Peripheral.Instance.Pause (!isactive);
        
    }

    public void toggleMovePanel()
    {
        if (!(parent.toy_type == ToyType.Hero && RewardOverseer.RewardInstance.getReward(RewardType.HeroMobility).unlocked)) return;

        bool isactive = move_panel.activeSelf;

        move_panel.SetActive(!isactive);
        Peripheral.Instance.Pause(!isactive);
    }

    public void toggleSell(){                    
        if (parent.toy_type == ToyType.Hero || parent.runetype == RuneType.Castle) return;

        bool isactive = sell_panel.activeSelf;
        
		if (!isactive){            
            float cost = parent.getSellCost();
            sell_cost_label.text.text = cost.ToString(); 
        }
        
        sell_panel.SetActive(!isactive);
        Peripheral.Instance.Pause (!isactive);
	}
	


}