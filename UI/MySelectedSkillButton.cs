using System;
using UnityEngine;
using UnityEngine.UI;




public class MySelectedSkillButton : MonoBehaviour
{
    public Button my_button;	

    //public StatBit skill;
	public Peripheral peripheral;

    private EffectType type = EffectType.Null;
    
	public delegate void ButtonClickedHandler(string type, string content);
	public static event ButtonClickedHandler onButtonClicked;    
    public LevelList_Toy_Button_Driver my_driver;
    public LeanTweener tweener;
    
    public bool isEmpty()
    {
        return type == EffectType.Null;
    }


    public EffectType getEffectType()
    {
        return type;
}

    private void SetSkill(bool make_noise)
    {
        
        Toy_Button b = my_driver.selected_button;

        if (type != EffectType.Null)
        {
            peripheral.my_skillmaster.DisableSkill(type);
            ShowEmpty();
            if (make_noise) Noisemaker.Instance.Click(ClickType.Cancel);

        }
        else
        {
            if (make_noise) Noisemaker.Instance.Click(ClickType.Action);
            tweener.StopMeNow();
            SetSkill(b, true);
        }
    }

    public void _setSprite(Sprite s)
    {
        if (s == null)
        {
            Show.SetAlpha(my_button.image, 0f);
        }
        else
        {
            Show.SetAlpha(my_button.image, 1f);
            Debug.Log($"SelectedSkillButton setSprite {s}\n");
            my_button.image.sprite = s;
        }
    }

    public void ShowEmpty()
    {
        tweener.StopMeNow();
        _setStuff();
        _setSprite(my_driver.empty_button_sprite);
        type = EffectType.Null;
//        Debug.Log("Showing empty button\n");
        tweener.Init();
    }

    public void SetSkill(Toy_Button b, bool check_inventory)
    {
        _setStuff();
        //string what = (b == null) ? "null" : b.effect_type.ToString();
        

        if (b == null || b.toy_rune == null) // turn it off
        {
            
            _setSprite(null);
            
            //if (check_inventory) peripheral.my_skillmaster.DisableSkill(type);
            type = EffectType.Null;

        }
        else {                                      // turn it on
            Debug.Log($"Setting special skill button {b.gameObject.name} check_inventory {check_inventory}\n");
          //  if (b.rune_type == RuneType.Castle) return;
      //      Debug.Log("SETTING SELECT SKILL BUTTON FOR " + b.effect_type);
            if (check_inventory && peripheral.my_skillmaster.CheckSkill(b.effect_type))
            {

                Debug.Log("skill is already added to list\n");
                return; //skill already added, should play a sad sound here
            }
            if (b.toy_rune.getStatBit(b.effect_type) == null)
            {
                Debug.Log("skill hasn't been purchased yet\n");
                return; //skill already added, should play a sad sound here
            }

            peripheral.my_skillmaster.SetSkill(b.toy_rune.getStatBit(b.effect_type));
            peripheral.my_skillmaster.setInventory(b.effect_type, true);
            _setSprite(b.my_button.image.sprite);
            type = b.effect_type;
        }

    }

    private void Start(){
        _setStuff();
    }

    private void _setStuff()
    {
        if (peripheral == null) peripheral = Peripheral.Instance;
        if (my_driver == null) my_driver = Central.Instance.level_list.special_skill_button_driver;
    }
 		
	public void OnClick(){
	    if (EagleEyes.Instance.UIBlocked("MySelectedSkillButton", type.ToString())) return;
        SetSkill(true);
	}



}