using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class SpecialSkillSaver : IDeepCloneable<SpecialSkillSaver>
{
    public EffectType type;
    public float remaining_time;

    object IDeepCloneable.DeepClone()
    {
        return this.DeepClone();
    }

    public SpecialSkillSaver DeepClone()
    {
        SpecialSkillSaver my_clone = new SpecialSkillSaver();
        my_clone.type = this.type;
        my_clone.remaining_time = this.remaining_time;
        return my_clone;
    }
}

public class SpecialSkill : MonoBehaviour
{

    StateType current_state;
    // bool interactable;          // skill timer is up, skill is ready to use
    //public bool initialized;    // ya got the skill
    private bool in_inventory;   // ya placed the skill into the inventory, controlled by SkillMaster
    private bool hero_is_present;// ya placed the hero whom the skill belongs to
    public float remaining_time;    
    public StatBit skill;
    bool vocal = false;
    public EffectType type;
    public Interactable my_interactable;
    
    //[System.NonSerialized]
    
    public MySpecialButton button;

    public void Simulate(List<Vector2> positions)
    {
        Peripheral.Instance.ChangeTime(TimeScale.Normal);
        my_interactable.Activate(Skill);
        my_interactable.Simulate(positions);
    }

    StateType getState()
    {
        if (!isInitialized()) return StateType.No;
        if (!In_inventory) return StateType.No; //not in inventory
        if (!Hero_is_present) return StateType.No; //build your hero first
        if (remaining_time > 0) return StateType.NoResources; 
        
        return StateType.Yes;        
    }


    

    bool isInitialized()
    {
        if (skill.Level == 0) return false; //not initialized
        if (my_interactable == null) return false; //no interactable, setup fuckup
        if (type == EffectType.Null) return false;
        
        return true;
    }

    public StatBit Skill
    {
        get
        {
            return skill;
        }

        set
        {
            if (vocal) Debug.Log("Setting hero skill for " + this.gameObject.name + "\n");
            skill = value;                                 
            updateState();
        }
    }

    public bool Hero_is_present
    {
        get
        {
            //return Get.isCastleSkill(type); || 
            return hero_is_present;
        }

        set
        {
           if (vocal) Debug.Log("Setting hero is present (" + value.ToString().ToUpper() + ") for " + this.gameObject.name + "\n");
            hero_is_present = value;
            updateState();
        }
    }

    public bool In_inventory
    {
        get
        {
            //return Get.isCastleSkill(type) || 
            return in_inventory;
        }

        set
        {
            if (vocal) Debug.Log("Setting in_inventory for (" + value.ToString().ToUpper() + ") for " + this.gameObject.name + "\n");
            in_inventory = value;
            updateState();
        }
    }

    public void Reset()
    {
        Hero_is_present = false;
        my_interactable.Reset();
    }

    private void Start()
    {
        Peripheral.onPlacedToy += onPlacedToy;
      if (vocal)  Debug.Log("Subscribing to onPlacedToy for " + this.gameObject.name + "\n");
        skill.Level = 0;
    }

    private void OnDisable()
    {
        Peripheral.onPlacedToy -= onPlacedToy;
    }

    void updateState()
    {
        StateType state = getState();
        if (state != current_state)
            if (vocal) Debug.Log("Setting state for (from " + current_state.ToString().ToUpper() + " to " + state.ToString().ToUpper() + ") for " + this.gameObject.name + "\n");
        if (state == StateType.No && current_state != StateType.No) SetInteractable(false);

        if (state == StateType.NoResources && current_state == StateType.No) SetInteractable(true);

        if (state == StateType.NoResources)
        {
            if (Moon.Instance.WaveInProgress || Peripheral.Instance.WaveCountdownOngoing()) remaining_time -= Time.deltaTime;
            button.time.text = Mathf.CeilToInt(remaining_time).ToString();            
            button.SetButtonInteractable(remaining_time <= 0);            
        }

        if (state == StateType.Yes && current_state != StateType.Yes)
        {
            SetInteractable(true);
            if (button.time != null) button.time.text = Mathf.CeilToInt(remaining_time).ToString();
            button.SetButtonInteractable(remaining_time <= 0);
        }

        if (state == StateType.Yes && current_state == StateType.Yes && !button.gameObject.activeSelf)
        {
            if (vocal) Debug.Log("Button's button is interactable " + button.my_button.interactable + "\n");
            if (vocal)Debug.Log("Button gameObject is active " + button.gameObject.activeSelf + "\n");
            SetInteractable(true);
        }

            current_state = state;
    }

    //this shouldn't be in Update
    void Update()
    {
        if (Time.timeScale == 0) return;

        updateState();
 
    }

    

    public void SetInteractable(bool set)
    {
        if (set && !In_inventory) return;

        if (vocal) Debug.Log("Setting special skill interactable " + type + " " + set + "\n");
        
        button.SetButtonInteractable(set);        
        button.gameObject.SetActive(set);
    
        Peripheral.Instance.my_skillmaster.my_panel.UpdatePanel();
    }

    void onPlacedToy(string content, RuneType runetype, ToyType toytype)
    {
        if (vocal) Debug.Log("Received to onPlacedToy for " + this.gameObject.name + "\n");
        // if (!isInitialized()) return;

        if (skill.rune_type == runetype && toytype == ToyType.Hero)
        {
            Hero_is_present = true;
            if (vocal) Debug.Log("Hero  for " + this.gameObject.name + "\n");
        }
    }

    public void CancelSkill()
    {        
        button.CancelButton();        
    }

    public void SetRemainingTime(float time)
    {
        if (vocal) Debug.Log("Setting time remaining for (" + time + ") for " + this.gameObject.name + "\n");
        remaining_time = time;
        updateState();        

    }

    public void UseSkill()
    {


        

        SetRemainingTime(Skill.recharge_time);

        //button.SetButtonInteractable(false);
        //interactable = false;
        
    }

    public void ActivateSkill(bool set) // GO SKILL GO DO IT NOW
    {
        if (vocal) Debug.Log("Activating skill " + this.name + " " + set +  "\n");
        if (my_interactable == null) { Debug.Log("My_interactable is NULL for " + this.name + " FIX IT NOW\n"); }
        if (Peripheral.Instance.getCurrentTimeScale() == TimeScale.Pause) Peripheral.Instance.ChangeTime(TimeScale.Normal);
        if (set) my_interactable.Activate(Skill); else my_interactable.Deactivate();
    }
}


