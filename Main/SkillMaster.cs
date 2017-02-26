using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SkillMaster : MonoBehaviour {
    [SerializeField]
    public List<SpecialSkill> skills = new List<SpecialSkill>();// this is just a storage space to let Rune update the appropriate button, this has ALL skills whether or not they are turned on
    public int max_toy_skills;

    public List_Panel my_panel;
    public Toy_Scroll_Driver my_panel_driver;
    public List<SpecialSkillSaver> in_inventory = new List<SpecialSkillSaver>(); // this has just the skills that are activated, used for loading/saving games
                                                                   //a skill can be set but not activated


    public void Init()
    {
    //    Debug.Log("initializing inventory\n");
        in_inventory = new List<SpecialSkillSaver>();
    }

    public void resetInventory()
    {
    //    Debug.Log("Resetting inventory\n");
        foreach (SpecialSkill sk in skills)
        {
            setInventory(sk.type, false);
        }
       
     
    }

    public List<SpecialSkillSaver> getInventory()
    {
        foreach(SpecialSkillSaver saver in in_inventory)
        {
            saver.remaining_time = _getSkill(saver.type).remaining_time;
        }
        return in_inventory;
    }

    public void setInventory(EffectType type, bool set)
    {
        SpecialSkillSaver saver = new SpecialSkillSaver();
        saver.type = type;
        saver.remaining_time = 0f;
        setInventory(saver, set);
    }

    public void setInventory(SpecialSkillSaver skillsaver, bool set)
    {
     //   Debug.Log("Setting inventory " + skillsaver.type + " to " + set + "\n");
     
        bool already_added = CheckSkill(skillsaver.type);
        SpecialSkill sk = _getSkill(skillsaver.type);
        if (sk == null) return;

        if (set) sk.SetRemainingTime(skillsaver.remaining_time);
      //  else sk.SetRemainingTime(0f);

        if ((already_added && set) || (!already_added && !set)) return;
        
        

        if (already_added && !set)
        {
       //     Debug.Log("Removing skill " + sk.type + "\n");
            removeFromIntentory(skillsaver.type);
            sk.in_inventory = set;            
            return;
        }
        if (!already_added && set && !inventoryFull())
        {
        //    Debug.Log("Adding skill " + sk.type + "\n");
            in_inventory.Add(skillsaver);
            sk.in_inventory = set;
            
            return;

        }

    }


    void removeFromIntentory(EffectType type)
    {
        List<SpecialSkillSaver> new_list = new List<SpecialSkillSaver>();
        foreach (SpecialSkillSaver s in in_inventory)
        {
            if (s.type != type) new_list.Add(s);
        }
        in_inventory = new_list;
    }

    public bool CheckSkill(EffectType type)
    {
        foreach (SpecialSkillSaver s in in_inventory)
        {
            if (s.type == type) return true; 
        }
        return false;
    }

    public void DisableSkill(EffectType type)
    {
        SpecialSkill skill = _getSkill(type);
        if (skill == null) return;        
        
        my_panel.UpdatePanel();
        SpecialSkillSaver saver = new SpecialSkillSaver();
        saver.type = type;
        setInventory(saver, false);
        Noisemaker.Instance.Click(ClickType.Cancel);
    }

    public bool inventoryFull()
    {        
        return in_inventory.Count >= max_toy_skills;
    }

    public void UseSkill(EffectType type)
    {
        SpecialSkill skill = _getSkill(type);       
        if (skill == null) {
            Debug.Log("Skillmaster does not have a skill of type " + type + " to use\n");
            Noisemaker.Instance.Click(ClickType.Error);
            return;
        }
        Noisemaker.Instance.Play("use_special_skill");
        skill.UseSkill();
    }

    public void CancelSkill(EffectType type)
    {
        SpecialSkill skill = _getSkill(type);
        Noisemaker.Instance.Click(ClickType.Error);
        if (skill == null)
        {
            Debug.Log("Skillmaster does not have a skill of type " + type + " to deactivate\n");            
            return;
        }
        skill.CancelSkill();
    }

    public void ActivateSkill(EffectType type)
    {
        SpecialSkill skill = _getSkill(type);
        if (skill == null) {
            Debug.Log("Skillmaster does not have a skill of type " + type + "\n");
            Noisemaker.Instance.Click(ClickType.Error);
            return;
        }
        Debug.Log("activating " + type + " \n");
        Noisemaker.Instance.Click(ClickType.Action);
        skill.ActivateSkill(true);
        
        setInventory(type, true);
        
    }



    SpecialSkill _getSkill(EffectType type)
    {
        foreach (SpecialSkill s in skills)
        {
            if (s.type == type)
                return s;            
        }
        return null;
    }

    public void SetSkill(StatBit statbit)
    {     
        SpecialSkill s = _getSkill(statbit.effect_type);

        if (s == null) { Debug.Log("Skillmaster does not have a skill of type " + statbit.effect_type + "\n"); return; }
        if (!statbit.hasStat())
        {         
            Debug.Log("Setting null skill?\n");
            return;
        }
        else {        
            s.Skill = statbit;//.GetClone();//should this be a clone or no?            
            s.button.SetSkill();
            my_panel.UpdatePanel();
        }
        
    }




    

}
