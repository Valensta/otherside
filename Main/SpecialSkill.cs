using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class SpecialSkillSaver
{
    public EffectType type;
    public float remaining_time;
}

public class SpecialSkill : MonoBehaviour
{   
    
    
    bool interactable;
    public bool initialized;
    public bool in_inventory;
    public float remaining_time;    
    StatBit skill;
    
    public EffectType type;
    public Interactable my_interactable;

    //[System.NonSerialized]
    
    public MySpecialButton button;

    public bool isCastleSkill()
    {//this is unfortunate    
        return (type == EffectType.Gnomes || type == EffectType.Sync || type == EffectType.BaseHealth);
    }
    



    public StatBit Skill
    {
        get
        {
            return skill;
        }

        set
        {
      //      Debug.Log("Setting skill " + type + "\n");
            skill = value;
            initialized = (value != null);
            remaining_time = 0;
        }
    }



    void Update()
    {
        if (!initialized) return;
        if (initialized && !in_inventory)
        {
            SetInteractable(false);
            initialized = false;
            skill = null;
        }

        if (!in_inventory) return;
        if (interactable) return;

        if (remaining_time > 0)
        {
            remaining_time -= Time.deltaTime;
            button.time.text = Mathf.CeilToInt(remaining_time).ToString();
        }
        else
        {
            SetInteractable(true);
        }
    }

    

    public void SetInteractable(bool set)
    {
        if (set && !in_inventory) return;


        interactable = set;
        button.SetButtonInteractable(set);
        button.gameObject.SetActive(set);
    
        Peripheral.Instance.my_skillmaster.my_panel.UpdatePanel();
    }

    
    
    public void CancelSkill()
    {        
        button.SetButtonInteractable(false);
        interactable = false;
    }

    public void SetRemainingTime(float time)
    {
        remaining_time = time;
        button.time.text = Mathf.CeilToInt(remaining_time).ToString();
        if (!initialized || !in_inventory) return;

        button.gameObject.SetActive(true);
        //this is confusing, why this vs setinteractable
        if (time > 0)
        {
            interactable = false;
            button.SetButtonInteractable(false);
        }
        else
        {
            interactable = true;
            button.SetButtonInteractable(true);
        }
    }

    public void UseSkill()
    {        
        SetRemainingTime(Skill.recharge_time);

        //button.SetButtonInteractable(false);
        //interactable = false;
        
    }

    public void ActivateSkill(bool set)
    {
    //  Debug.Log("Activating skill " + this.name + " " + set +  "\n");
        if (my_interactable == null) { Debug.Log("My_interactable is NULL for " + this.name + " FIX IT NOW\n"); }
        if (set) my_interactable.Activate(Skill.getStats()); else my_interactable.Deactivate();
    }
}


