using System;
using UnityEngine;
using UnityEngine.UI;




public class MySpecialButton : UIButton
{
    public Button my_button;
	public bool holding = false;
	public bool selected = false;	
	public GameObject parent;

    //public StatBit skill;
	public bool interactable = false;
	public Peripheral peripheral;

    public SpecialSkill my_special;
	public delegate void ButtonClickedHandler(string type, string content);
	public static event ButtonClickedHandler onButtonClicked;
    public GameObject info_box;
    public MyImageLabel level_pips;

    public Text strength;
    public Text time;

    public override void Reset()
    { }

    public override void InitMe(){
        
		if (interactable) return;		
		
	}

    public void CancelButton()
    {        
        if (info_box != null) info_box.SetActive(false);
        selected = false;
    }

    public void SetButtonInteractable(bool set)
    {
     //   Debug.Log("Set interactable "+ this.gameObject.name + "\n");
        if (my_button != null) my_button.interactable = set;
        if (!set && info_box != null) info_box.SetActive(false);
        selected = false;
    }

    public void SetSkill()
    {

    //    Debug.Log("Setting skill " + my_special.Skill.effect_type + "\n");
        if (time != null) time.text = Mathf.CeilToInt(my_special.remaining_time).ToString();

        if (level_pips != null) level_pips.setLabel("", my_special.Skill.level);        

        if (!Get.isCastleSkill(my_special.Skill.effect_type))
        {
            info_box = Zoo.Instance.getObject("GUI/tiny_info/" + my_special.Skill.effect_type.ToString() + "_tiny_info",
                false);
            info_box.transform.SetParent(this.transform);
            info_box.transform.localScale = Vector3.one;
            info_box.transform.localRotation = Quaternion.identity;
            info_box.transform.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

            info_box.GetComponent<MyLabel>().text.text =
                my_special.Skill.getCompactDescription(LabelName.Null);
        }

        
    }
   
	void OnEnabled(){
		InitMe();
	}
	
	void Start(){
		InitMe();
		peripheral = Peripheral.Instance;

		enabled = true;
        SpyGlass.onSelected += onSelected;
        Peripheral.onSelected += onSelected;
        Island_Button.onSelected += onSelected;
        MyButton.onSelected += onSelected;
        MyFastForwardButton.onSelected += onSelected; 
    }

    public void onSelected(SelectedType type, string n)
    {
   
        if (type == SelectedType.InteractiveSkill && n.Equals(this.name)) return;
        
        SetSelectedToy(false);
      
    }


    void onCreatePeripheral(Peripheral p){	
		peripheral = p;
	}
        		
	public void OnClick(){
	    if (EagleEyes.Instance.UIBlocked("MySpecialButton","")) return;
        OnInput();
	}

	public void OnInput(){
		
		if (!enabled)
        {
            Noisemaker.Instance.Click(ClickType.Error);
            return;
        }
        
        
        Peripheral.Instance.ClearAll(SelectedType.InteractiveSkill, this.name);
        //if (!selected) { SetSelected(true); Debug.Log("Selecting inventory slot " + this.name + "\n"); }
        if (selected) Noisemaker.Instance.Click(ClickType.Cancel); else Noisemaker.Instance.Click(ClickType.Success);
        SetSelectedToy(!selected);

     

        if (onButtonClicked != null) {
			onButtonClicked (my_special.Skill.effect_type.ToString(), my_special.Skill.getDetailStats()[0].ToString());
		}

	}

    void DoStuff()
    {
      //  Noisemaker.Instance.Play("use_special_skill");
        if (my_special.Skill == null) { Debug.Log("Trying to use an uninitialized special skill! the hell!\n"); return; }

        Debug.Log("Using skill " + my_special.Skill.effect_type + " " + my_special.Skill.getDetailStats()[0].ToString() + " lets do some special stuff\n");               
        
        Peripheral.Instance.my_skillmaster.ActivateSkill(my_special.Skill.effect_type);
    }

    public override void InitStartConditions()
    {
        
    }

    public override void SetSelectedToy(bool set)
    {
        //     Debug.Log("Special button set selected " + set + "\n");
        if (selected == set) return;
        selected = set;
        if (info_box != null) info_box.SetActive(set);
        my_special.ActivateSkill(set);
    }

    public override void SetInteractable(bool set)
    {
        //Debug.Log("Set interactable "+ this.gameObject.name + "\n");
        interactable = set;
        if (my_button != null) my_button.interactable = set;
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
}