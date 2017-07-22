using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;


public class Toy_Button : MyButton
{    
    public EffectType effect_type;
    public RuneType rune_type;
    //public MyLabel my_label;
    Color initial_color;
    public Rune toy_rune;  //used by global rune panel
    public Toy toy_parent; //firearm assigned here. buttons talk directly to the firearm	
    public Global_Toy_Button_Driver global_rune_panel; //driver controls them
                                                       //	public Tweener tweener;
                                                       //	public Text text;
                                                       //bool selected;

    public StateType current_state = StateType.No;

    public Image have_skill_image;
    public Image do_not_have_skill_image;
    public Image lock_accent;
    public Image no_resources_accent;
    public Image can_do_accent;
    public GameObject helpers;
    string lock_icon = "GUI/InGame/locked_upgrade_button_accent";
    string no_resources_icon = "GUI/InGame/no_resources_upgrade_button_accent";
    string can_do_icon = "GUI/InGame/can_upgrade_button_accent";

    public void setButtonImage(StateType type)
    {

        if (have_skill_image == null || do_not_have_skill_image == null) return;

        if (type == StateType.Yes)
        {
            have_skill_image.gameObject.SetActive(true);
            do_not_have_skill_image.gameObject.SetActive(false);
        }
        else if (type == StateType.No || type == StateType.NoResources)
        {
            
            have_skill_image.gameObject.SetActive(false);
            do_not_have_skill_image.gameObject.SetActive(true);
        }else
        {
            
            have_skill_image.gameObject.SetActive(false);
            do_not_have_skill_image.gameObject.SetActive(false);
        }
        setHelpers(type);

    }

    public void setHelpers(StateType type)
    {
        if (helpers == null) return;

        if (type == StateType.Yes || type == StateType.No || type == StateType.NoResources)
        {
            helpers.SetActive(true);
        
        }
        else helpers.SetActive(false);
            
    }



    void setAccent(StateType type)
    {

        switch (type)
        {
            case StateType.Yes:
               if (no_resources_accent != null) no_resources_accent.gameObject.SetActive(false);
                if (no_resources_accent != null) no_resources_accent.gameObject.SetActive(false);
                if (lock_accent != null) lock_accent.gameObject.SetActive(false);

               
                if (can_do_accent == null)
                {
                    can_do_accent = Zoo.Instance.getObject(can_do_icon, true).GetComponent<Image>();
                    InitAccent(can_do_accent.gameObject.GetComponent<RectTransform>());
                }
                can_do_accent.gameObject.SetActive(true);

                break;
            case StateType.NoResources:
                if (can_do_accent != null) can_do_accent.gameObject.SetActive(false);
                if (lock_accent != null) lock_accent.gameObject.SetActive(false);

                
                if (no_resources_accent == null)
                {
                    no_resources_accent = Zoo.Instance.getObject(no_resources_icon, true).GetComponent<Image>();
                    InitAccent(no_resources_accent.GetComponent<RectTransform>());
                }else
                {

                }
                no_resources_accent.gameObject.SetActive(true);
                break;
            case StateType.No:
                if (can_do_accent != null) can_do_accent.gameObject.SetActive(false);
                if (no_resources_accent != null) no_resources_accent.gameObject.SetActive(false);

                
                if (lock_accent == null)
                {
                    lock_accent = Zoo.Instance.getObject(lock_icon, true).GetComponent<Image>();
                    InitAccent(lock_accent.GetComponent<RectTransform>());
                }
                lock_accent.gameObject.SetActive(true);
                break;
            case StateType.WrongType:
                
                if (can_do_accent != null) can_do_accent.gameObject.SetActive(false);
                if (no_resources_accent != null) no_resources_accent.gameObject.SetActive(false);
                if (lock_accent != null) lock_accent.gameObject.SetActive(false);
                return;
            default:
                return;
        }
      
        
    }

    void InitAccent(RectTransform accent)
    {
        //accent.parent = my_button.image.transform;
        accent.SetParent(my_button.transform);
       // RectTransform set_me_to = my_button.image.GetComponent<RectTransform>();
        RectTransform set_me_to = my_button.GetComponent<RectTransform>();
        RectTransform image = my_button.image.GetComponent<RectTransform>();
        //accent.sizeDelta = my_button.GetComponent<RectTransform>().sizeDelta;
        //accent.anchoredPosition = set_me_to.anchoredPosition;
        accent.anchoredPosition = image.anchoredPosition;
        accent.localScale = image.localScale;
        accent.sizeDelta = image.sizeDelta;
        
        if (helpers != null) helpers.transform.SetAsLastSibling();
    }

    public void setState(StateType state)
    {
        if (my_button == null) { Debug.LogError("Possibly misconfigured Toy_Button: " + this.gameObject.name + "\n"); return; }
        if (my_button.image == null) { Debug.LogError("Toy_Button my_button is missing an image assignment!\n"); return; }

      //  if (current_state == state) return;

        switch (state)
        {
            case StateType.No:
                my_button.interactable = true;
                my_button.image.gameObject.SetActive(true);
                //my_button.image.color = Color.white;
                setAccent(state);
                break;
            case StateType.NoResources:
                my_button.interactable = true;
                //my_button.image.color = Color.white;
                my_button.image.gameObject.SetActive(true);
                setAccent(state);
                break;
            case StateType.Yes:
                my_button.interactable = true;
                //my_button.image.color = Color.white;
                my_button.image.gameObject.SetActive(true);
                setAccent(state);
             

                break;
            case StateType.WrongType:
                my_button.interactable = false;
                //my_button.image.color = Color.clear;
                setAccent(state);
                my_button.image.gameObject.SetActive(false);
                break;
        }

        current_state = state;
    }

    public void OnClick()
    {
        //ToyButton is generally only used ingame. it is also used midlevel by the special skill global rune panel, which not THE global_rune_panel but the levellist panel
        //midlevel use - "upgrade_select"
        if (EagleEyes.Instance.UIBlocked("Toy_Button", type + "_" + effect_type)) return;

        if (global_rune_panel == null)
        {
            if (Central.Instance.state == GameState.InGame)
            { global_rune_panel = Monitor.Instance.global_rune_panel; }
            else { global_rune_panel = Central.Instance.level_list.special_skill_button_driver; }
        }

        ClickType click = ClickType.Success;

        switch (type)
        {
            case "main":

                break;
            case "global_rune_panel_info":
                global_rune_panel.toggleInfo();
                break;
            case "ammo":
                //	Debug.Log("Wanna add ammo\n");
                click = ClickType.Null;
                if (toy_parent.firearm != null) toy_parent.firearm.AddAmmo(1);
                break;
            case "sell":                
                ((InGame_Toy_Button_Driver)global_rune_panel).toggleSell();
                break;
            case "sell_confirm":
                click = ClickType.Action;
                ((InGame_Toy_Button_Driver)global_rune_panel).sellToy();              
                break;
            case "sell_cancel":
                click = ClickType.Cancel;
                ((InGame_Toy_Button_Driver)global_rune_panel).toggleSell();
                break;
       //     case "move":                
                //((InGame_Toy_Button_Driver)global_rune_panel).toggleMovePanel();
//                break;
            case "move_confirm":
                click = ClickType.Action;
                ((InGame_Toy_Button_Driver)global_rune_panel).moveToy();
                break;
            //      case "move_cancel":
            //click = ClickType.Cancel;
            //                ((InGame_Toy_Button_Driver)global_rune_panel).toggleMovePanel();
            //              break;
            case "reset_special_skills":
                click = ClickType.Action;
                
                ((LevelList_Toy_Button_Driver) global_rune_panel).resetSkills(effect_type);
                break;
            case "reset_skills":
                click = ClickType.Action;
                toy_parent.ResetSkills();
                global_rune_panel.setStuff();
                global_rune_panel.setSelectedButton(null);
                break;
            case "upgrade":
                if (selected && current_state != StateType.Yes)
                {
                    click = ClickType.Cancel;
                    ((InGame_Toy_Button_Driver)global_rune_panel).setSelectedButton(null);
                    return;
                }

                if (selected) //ie deselected cuz you tapped it twice
                {
                    click = ClickType.Action;
               
                    toy_parent.UpgradeRune(toy_parent.toy_type, effect_type);
                    setButtonImage(StateType.Yes);
                    global_rune_panel.setStuff();
                    
                }                
                    ((InGame_Toy_Button_Driver)global_rune_panel).setSelectedButton(this);

                if (toy_parent.rune_buttons != null) toy_parent.rune_buttons.UpdateMe(); // what is this for
                break;
            case "upgrade_select":
               
                ((LevelList_Toy_Button_Driver)global_rune_panel).setSelectedButton(this);
      
                break;
            case "building_selected":
                click = (selected) ? ClickType.Cancel : ClickType.Success;
                SetSelectedToy(!selected);
                break;
            case "toy_selected":
                click = (selected)? ClickType.Cancel : ClickType.Success;
                SetSelectedToy(!selected);

                break;
            default:
                break;
        }
        Noisemaker.Instance.Click(click);
            
    }

}