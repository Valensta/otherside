using System;
using UnityEngine;
using UnityEngine.UI;

public class MyButton : UIButton {
    public string type = "";
    public string content = "";
    public string content_detail = "";
    public int order = 0;
    public MenuButton menu_button;
    public Button my_button;
    public bool holding = false;
    public bool selected = false;
    public bool make_a_toy = false;
    public GameObject parent;
    Sprite default_sprite;
    public MyLabel label;
   // public Image selected_accent;

    bool am_initialized = false;
    public Peripheral peripheral;

    public delegate void ButtonClickedHandler(string type, string content);
    public static event ButtonClickedHandler onButtonClicked;

    public delegate void OnSelectedHandler(SelectedType type, string n);
    public static event OnSelectedHandler onSelected;

    public Text text;

    public override void Reset()
    { }

    public override void InitButton()
    {

    }

    public override void InitMe() {
        if (am_initialized) return;
        if (make_a_toy == true) {
            Peripheral.onCreatePeripheral += onCreatePeripheral;
            Toy.onPriceUpdate += onPriceUpdate;
            EagleEyes.onPriceUpdate += onPriceUpdate;
            Central.onPriceUpdate += onPriceUpdate;
            am_initialized = true;
        }

    }



    public void onPriceUpdate(string name, float price) {

        //	Debug.Log("On price update " + name + " is? " + content + " or " + content_detail + " " + price + "\n");
        if (make_a_toy && (content.Equals(name) || content_detail.Equals(name))) {
            if (text == null) {
                //Debug.Log ("toy_selected button " + name + " does not have text assigned, cannot update price!\n");
                return;
            }
            //Debug.Log ("toy_selected button " + name  + " setting price to " + price + "\n");

            text.text = price.ToString();
        }

    }

    void OnEnabled() {
        default_sprite = my_button.image.sprite;
        InitMe();
    }


    void Start()
    {
        if (type.Equals("toy_selected") || type.Equals("meter_selected")) { make_a_toy = true; }
        if (!type.Equals("Level")) ShowSelectedAccent(false);
        InitMe();
        peripheral = Peripheral.Instance;

        enabled = true;
    }

	void onCreatePeripheral(Peripheral p){
	//	Debug.Log ("peripheral initiated in button");
		peripheral = p;
	}



    public override void SetSelectedToy(bool selected)
    {
        if (selected)
        {
            if (peripheral == null) peripheral = Peripheral.Instance;
            selected = peripheral.SelectToy(content, RuneType.Null);
        }
        else
        {
            EagleEyes.Instance.ClearInfo();
            selected = false;
        }
        
    }


	public void OnClick(){
	    if (EagleEyes.Instance.UIBlocked(type, content)) return;
        OnInput();
	}

    void OnInput()
    {
       
        if (!enabled)
        {
            Noisemaker.Instance.Click(ClickType.Error);
            return;
        }

        ClickType click_outcome = ClickType.Null;

        if (type.Equals("toy_selected"))
        {
            click_outcome = ClickType.Success;
            SetSelectedToy(true);
        }

        if ((type.Equals("selected")) && selected)
        {
            click_outcome = ClickType.Cancel;
            SetSelectedToy(false);
        }
        if (type.Equals("meter_selected") && selected)
        {
            click_outcome = ClickType.Cancel;
            selected = false;
            SetSelectedToy(false);
        }
        else if (type.Equals("meter_selected"))
        {
            peripheral.SelectToy(content, EnumUtil.EnumFromString(content_detail, RuneType.Null));
            click_outcome = ClickType.Success;
            selected = true;

        }


        if (type.Equals("info"))
        {
            Debug.Log("Info button pressed? Is this still used?\n");
            click_outcome = ClickType.Success;
            Destroy(parent);
        }

        if (type.Equals("InGame"))
        {
            click_outcome = ClickType.Success;
            onSelected?.Invoke(SelectedType.Null, "");
            switch (content)
            {
                case "StartWave":
                    Peripheral.Instance.StartWave();
                    click_outcome = ClickType.Null;
                    break;
                case "MainMenu":
                    peripheral.ChangeTime(TimeScale.Pause);
                    EagleEyes.Instance.PlaceMenu(true);                                        
                    break;
                case "Pause":                    
                    selected = !selected;                    
                    peripheral.TogglePause();
                    break;
                case "FastForward":                    
                    peripheral.ToggleFast();
                    break;
                case "FastForward_SUPERFAST":
                    peripheral.ChangeTime(TimeScale.SuperFastPress);
                    break;
                default:
                    break;
            }

        }

        if (type.Equals("MainMenu"))
        {
            click_outcome = ClickType.Success;
            switch (menu_button)
            {
                case MenuButton.Play:
                    Central.Instance.game_saver.toggleSaveGamePanel();
                    break;
                case MenuButton.Start:
                    click_outcome = content.Equals("CancelConfirm") ? ClickType.Cancel : ClickType.Success;
                    Central.Instance.changeState(GameState.LevelList, content);
                    break;
                case MenuButton.Continue:
                    Debug.Log("Continue play from main menu\n");                    
                    EagleEyes.Instance.PlaceMenu(false);                    
                    break;
                case MenuButton.Quit:
                    Central.Instance.changeState(GameState.Quit);
                    break;
                case MenuButton.LoadSnapshot:
                    Central.Instance.changeState(GameState.Loading, content); //why the hell is this all done via strings
                    break;
                case MenuButton.GoBack1:
                    Central.Instance.changeState(GameState.Loading, menu_button.ToString());
                    break;
                case MenuButton.GoBack2:
                    Central.Instance.changeState(GameState.Loading, menu_button.ToString());
                    break;
                case MenuButton.LoadLatestGame:
                    Central.Instance.changeState(GameState.Loading, content);
                    break;
                case MenuButton.LoadStartLevelSnapshot:
                    Central.Instance.changeState(GameState.Loading, content);
                    break;
                case MenuButton.ToMap:
                    if (content.Equals("CancelConfirmToMap"))
                    {
                        click_outcome = ClickType.Cancel;
                        Central.Instance.changeState(GameState.InGame, content);
                    }
                    else
                    {
                        click_outcome = ClickType.Success;
                        Central.Instance.changeState(GameState.LevelList, content);
                    }
                    break;
                case MenuButton.Settings:
                    EagleEyes.Instance.my_settings_panel.TogglePanel();
                    break;
                case MenuButton.Rewards:
                    EagleEyes.Instance.rewards_scroll_driver.Toggle();
                    break;
                default:
                    break;
            }
        }

        if (type.Equals("Lost"))
        {
            switch (menu_button)

            {
                case MenuButton.LoadStartLevelSnapshot:
                    Debug.Log("Button change state to loading\n");
                    Central.Instance.changeState(GameState.Loading, content);
                    click_outcome = ClickType.Success;
                    break;
                case MenuButton.Quit:
                    Debug.Log("I wanna quit\n");
                    Application.Quit();
                    break;
                default:
                    break;
            }
        }
        if (type.Equals("Settings"))
        {
            //bool ok = false;
            switch (content)
            {
                case "VolumePlus":
                    click_outcome = (EagleEyes.Instance.my_settings_panel.IncreaseVolume()) ? ClickType.Success : ClickType.Error;
                    break;
                case "VolumeMinus":
                    click_outcome = (EagleEyes.Instance.my_settings_panel.DecreaseVolume()) ? ClickType.Success : ClickType.Error;
                    break;
                case "Play":
                    Noisemaker.Instance.setMute(false);
                    EagleEyes.Instance.mySoundButtons.updateButtons();
                    click_outcome = ClickType.Success;
                    break;
                case "Mute":
                    Noisemaker.Instance.setMute(true);
                    EagleEyes.Instance.mySoundButtons.updateButtons();
                    click_outcome = ClickType.Success;
                    break;
            }


        }
        if (type.Equals("Marketplace"))
        {
            EagleEyes.Instance.marketplace_driver.Init(true);
        }
        if (type.Equals("LevelList"))
        {
            switch (menu_button)
            {
                case MenuButton.Start:
                    //	Debug.Log("Button change state to loading");

                    Central.Instance.changeState(GameState.Loading, content);                    
                    break;
                
                case MenuButton.Inventory:
                    switch (content)
                    {
                        case "cancel":

                            Central.Instance.level_list.special_skill_button_driver.DisableMe();
                            Central.Instance.game_saver.SaveGame(SaveWhen.BetweenLevels);
                            click_outcome = ClickType.Cancel;
                            break;
                        case "upgrade":

                            click_outcome = ClickType.Action;
                            Central.Instance.level_list.special_skill_button_driver.upgradeSelected();
                            break;
                        case "givemestuff":
                            click_outcome = ClickType.Action;
                            ScoreKeeper.Instance.SetTotalScore(820);
                            Central.Instance.game_saver.SaveGame(SaveWhen.BetweenLevels);
                            break;
                        case "reset_special_skills":
                            break;
                        default:
                          //  Debug.Log($"Special skill button set parent {content}\n");
                            Central.Instance.level_list.special_skill_button_driver.SetParent(null);
                            break;
                    }
                    break;
                case MenuButton.ToMainMenu:
                    click_outcome = ClickType.Success;
                    Central.Instance.changeState(GameState.MainMenu);
                    break;
                default:
                    break;
            }
        }

        if (type.Equals("Level"))
        {
            click_outcome = ClickType.Success;
            int lvl = int.Parse(content);
            if (lvl == -1) click_outcome = ClickType.Cancel;

            if (!Central.Instance.setCurrentLevel(lvl)) {
                click_outcome = ClickType.Cancel;
                return;
            }
            Central.Instance.level_list.setLevelInfo(lvl);
            //Otherwise do onButtonClick below
        }

        if (type.Equals("Won"))
        {
            click_outcome = ClickType.Success;
            if (menu_button == MenuButton.ToMap) Central.Instance.changeState(GameState.LevelList, content);            
        }
        if (onButtonClicked != null)onButtonClicked(type, content);        
        Noisemaker.Instance.Click(click_outcome);
    }

    public override void InitStartConditions()
    {
        
    }

    public override void SetInteractable(bool set)
    {
        
    }
}