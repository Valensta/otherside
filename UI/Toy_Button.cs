using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
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

    public void OnClick()
    {
        Tracker.Log("Toy_Button type " + this.gameObject.name + " type " + type);
        int c;
        //ToyButton is generally only used ingame. it is also used midlevel by the special skill global rune panel, which not THE global_rune_panel but the levellist panel
        //midlevel use - "upgrade_select"

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
                if (toy_parent is Firearm) ((Firearm)toy_parent).AddAmmo(1);
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
            case "move":                
                ((InGame_Toy_Button_Driver)global_rune_panel).toggleMovePanel();
                break;
            case "move_confirm":
                click = ClickType.Action;
                ((InGame_Toy_Button_Driver)global_rune_panel).moveToy();
                break;
            case "move_cancel":
                click = ClickType.Cancel;
                ((InGame_Toy_Button_Driver)global_rune_panel).toggleMovePanel();
                break;
            case "upgrade":
                if (selected) //ie deselected cuz you tapped it twice
                {
                    //    Debug.Log("Upgrading " + effect_type + "\n");
                    click = ClickType.Action;
                    toy_parent.UpgradeRune(toy_parent.toy_type, effect_type);
                    global_rune_panel.setStuff();
                    
                }                
                    ((InGame_Toy_Button_Driver)global_rune_panel).setSelectedButton(this);

                if (toy_parent.rune_buttons != null) toy_parent.rune_buttons.UpdateMe(); // what is this for
                break;
            case "upgrade_select":
               // Debug.Log("Select upgrade " + effect_type + "\n");
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