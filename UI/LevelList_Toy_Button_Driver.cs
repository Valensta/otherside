using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
//using UnityEditor;

[System.Serializable]

public class LevelList_Toy_Button_Driver : Global_Toy_Button_Driver
{

    
    //public List_Panel selected_skill_panel;
    public List<MySelectedSkillButton> chosen_skills;
    public MyButton upgrade_button;
    public MyButton reset_button;
    
    public Sprite empty_button_sprite;

    void Start(){
		
		show = false;
        selected_button = null;
	}

    public override void Init()
    {
        base.Init();

        upgrade_button.gameObject.SetActive(false);


        
    }



    void UpdatePassiveLabels()
    {
        foreach (MyLabel l in drivers[current_driver].labels)
        {
            if (!l.type.Equals("info")) continue;

            
            MyText level_text = l.getText(LabelName.Level);
            MyText cost_text = l.getText(LabelName.UpgradeCost);
            if (level_text == null && cost_text == null) return;

            Rune r = ((Toy_Button)l.ui_button).toy_rune;
            setInfoLabel(l, r);
            if (r == null)
            {
                if (level_text != null) level_text.setText("");
                if (cost_text != null) cost_text.setText("");
                continue;
            }       

            StatBit s = r.getStat(l.effect_type);
            if (s == null)
            {
                if (level_text != null) level_text.setText("");
                if (cost_text != null) cost_text.setText("");
                continue;
            }
            if (level_text != null) level_text.setText((s.Level > 0)? s.Level.ToString(): "");
            if (cost_text != null) cost_text.setText(s.cost.Amount.ToString());
        }
    }

    public override void DisableMe()
    {
        selected_button = null;
        foreach (MySelectedSkillButton sk in chosen_skills)
        {
            sk.SetSkill(null, false);
        }
        base.DisableMe();
    }

    public override void SetParent(Toy p)
    {
        if (drivers[current_driver].button_map.Count == 0) { Init(); }
        parent = null;
        SetDriver(RuneType.Special);

        for (int i = 0; i < drivers[current_driver].buttons.Count; i++)
        {
            drivers[current_driver].buttons[i].toy_rune = Central.Instance.getHeroRune(drivers[current_driver].buttons[i].rune_type);
        }
        base.SetParent(p);
      //  setSelectedButton(null);
        UpdatePassiveLabels();

        int current_button = 0;
        foreach(Toy_Button b in drivers[current_driver].buttons)
        {
            
            if (Peripheral.Instance.my_skillmaster.CheckSkill(b.effect_type))
            {
                chosen_skills[current_button].SetSkill(b, false);
                chosen_skills[current_button].gameObject.SetActive(true);
                current_button++;
            }
        }

        setSelectedButton(null);
    }

    
    bool canSelectSkill()
    {
        if (selected_button == null || selected_button.toy_rune == null || selected_button.toy_rune.runetype == RuneType.Null)
        {
            //   Debug.Log("toy rune is not valid\n"); 
            return false;
        }
        if (selected_button == null)
        {
            //Debug.Log("no selected button\n");
            return false;
        }
        /*
        if (selected_button.rune_type == RuneType.Castle)
        {         
            return false;
        }*/
        if (selected_button.effect_type == EffectType.Null)
        {
            //Debug.Log("selected button effect type is null, no good\n");
            return false;
        }
        if (Peripheral.Instance.my_skillmaster.inventoryFull())
        {
            //Debug.Log("inventory is full, no good\n");
            return false;
        }
        if (Peripheral.Instance.my_skillmaster.CheckSkill(selected_button.effect_type))
        {
            //Debug.Log("skill has already been turned on, no good\n");
            return false;
        } //skill has already been turned on
        StatBit statbit = selected_button.toy_rune.getStatBit(selected_button.effect_type);
        if (statbit == null || statbit.level == 0)
        {
            //Debug.Log("skill has not been upgraded yet, no good\n");
            return false;
        } // skill has not been upgraded yet

        return true;
    }

    public void resetSkills(EffectType effectType)
    {
        selected_button.toy_rune.resetSkills(selected_button.effect_type, true);
       
        UpdatePassiveLabels();
        foreach (MySelectedSkillButton chosen in chosen_skills)
        {
            
            EffectType type = chosen.getEffectType();
            if (effectType != EffectType.Null && type != effectType) continue;

                if (type != EffectType.Null && selected_button.toy_rune.HasUpgrade(type))
            {            
                Peripheral.Instance.my_skillmaster.DisableSkill(type);
                chosen.ShowEmpty();
            }
        }
        setSelectedButton(null);
    }

    public void showEmptyButton()
    {          
        bool can_select = canSelectSkill();

        foreach (MySelectedSkillButton b in chosen_skills)
        {
            if (!b.isEmpty()) continue;
            if (can_select)
            {
                b.ShowEmpty();
                b.gameObject.SetActive(true);
                return;
            }
            else
            {
                b.gameObject.SetActive(false);
            }                
        }
        
    }

    public void upgradeSelected()
    {
        if(selected_button == null) { Debug.Log("trying to upgrade null button, what\n"); return; }

        if (selected_button.toy_rune.Upgrade(selected_button.effect_type, true) > 0)//upgrade successful
        {
            showEmptyButton();

        }
        selected_button.setButtonImage(StateType.Yes);
        setText(verbose_label, selected_button.effect_type, selected_button.toy_rune, true);
        UpdatePassiveLabels();
        setSelectedButton(selected_button);
    }
        
    
    public override void setSelectedButton(Toy_Button b)        
    {
        base.setSelectedButton(b);

        selected_button = b;
        if (b == null || b.toy_rune == null || b.toy_rune.runetype == RuneType.Null)
        {
            setText(verbose_label, EffectType.Null, null, true);
            upgrade_button.gameObject.SetActive(false);
            reset_button.gameObject.SetActive(false);
        }
        else
        {
            setText(verbose_label, b.effect_type, b.toy_rune, true);
            bool yes_please = b.toy_rune.CanUpgrade(b.effect_type, RuneType.Sensible, true) == StateType.Yes;       
            upgrade_button.gameObject.SetActive(yes_please);

            bool can_reset = b.toy_rune.getLevel(b.effect_type) > 0;
            reset_button.gameObject.SetActive(can_reset);
            if (can_reset) ((Toy_Button)reset_button).effect_type = b.effect_type;
        }

        showEmptyButton();
        
    }

    public override void updateOtherLabels()
    {
        
    }
}