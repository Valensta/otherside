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
    public Image selected_accent;

    bool am_initialized = false;
    public Peripheral peripheral;

    public delegate void ButtonClickedHandler(string type, string content);
    public static event ButtonClickedHandler onButtonClicked;

    public delegate void OnSelectedHandler(SelectedType type, string n);
    public static event OnSelectedHandler onSelected;

    public Text text;

    public override void Reset()
    { }


    public override void InitMe() {
        if (am_initialized) return;
        if (make_a_toy == true) {
            Peripheral.onCreatePeripheral += onCreatePeripheral;
            Firearm.onPriceUpdate += onPriceUpdate;
            EagleEyes.onPriceUpdate += onPriceUpdate;
            Central.onPriceUpdate += onPriceUpdate;
            am_initialized = true;
        }

    }

    public void ShowSelectedAccent(bool show)
    {
        if (selected_accent == null) return;

        Show.SetAlpha(selected_accent, (show)? 1f: 0f);
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
        ShowSelectedAccent(false);
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
		OnInput();
	}

    public void OnInput()
    {
        //Debug.Log ("button on click " + this.name + " type " + type + " content " + content + "\n");
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
            peripheral.SelectToy(content, Get.RuneTypeFromString(content_detail));
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
            if (onSelected != null) onSelected(SelectedType.Null, "");
            switch (content)
            {
                case "StartWave":
                    Peripheral.Instance.StartWave();
                    click_outcome = ClickType.Null;
                    break;
                case "MainMenu":
                    peripheral.Pause(true);
                    EagleEyes.Instance.PlaceMenu(true);                                        
                    break;
                case "Pause":
                    //Debug.Log("pressed pause\n");
                    
                    selected = !selected;
                    ShowSelectedAccent(selected);
                    peripheral.Pause(selected);
                    break;
                case "FastForward":
                    peripheral.ChangeTime(1.5f);
                    break;
                case "FastForward_SUPERFAST":
                    peripheral.ChangeTime(16);
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
                    peripheral.Pause(false);
                    EagleEyes.Instance.PlaceMenu(false);
                    break;
                case MenuButton.Quit:
                    Central.Instance.changeState(GameState.Quit);
                    break;
                case MenuButton.LoadSnapshot:
                    Central.Instance.changeState(GameState.Loading, content);
                    break;
                case MenuButton.LoadLatestGame:
                    Central.Instance.changeState(GameState.Loading, content);
                    break;
                case MenuButton.LoadStartLevelSnapshot:
                    Central.Instance.changeState(GameState.Loading, content);
                    break;
                case MenuButton.ToMap:
                    click_outcome = content.Equals("CancelConfirmToMap") ? ClickType.Cancel : ClickType.Success;
                    Central.Instance.changeState(GameState.LevelList, content);// why is this levellist?
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

        if (type == "Lost")
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
            bool ok = false;
            switch (content)
            {
                case "VolumePlus":
                    click_outcome = (EagleEyes.Instance.my_settings_panel.IncreaseVolume()) ? ClickType.Success : ClickType.Error;
                    break;
                case "VolumeMinus":
                    click_outcome = (EagleEyes.Instance.my_settings_panel.DecreaseVolume()) ? ClickType.Success : ClickType.Error;
                    break;
            }


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
                    if (content.Equals("cancel"))
                    {
                        Central.Instance.level_list.special_skill_button_driver.DisableMe();
                        Central.Instance.game_saver.SaveGame(SaveWhen.BetweenLevels);
                        click_outcome = ClickType.Cancel;
                    }
                    else if (content.Equals("upgrade"))
                    {

                        click_outcome = ClickType.Action;
                        Central.Instance.level_list.special_skill_button_driver.upgradeSelected();
                    }
                    else if (content.Equals("givemestuff"))
                    {
                        click_outcome = ClickType.Action;
                        //Rune srune = new Rune();
                        actorStats sstats = Central.Instance.getToy("sensible_tower_hero");
                        sstats.setActive(true);
                        //srune.initStats(RuneType.Sensible, sstats.getMaxLvl(), ToyType.Hero, sstats.exclude_skills);
                        //ToySaver s = new ToySaver("sensible_tower_hero", -1, srune, ToyType.Hero);
                        //Central.Instance.setHeroStats(s);

                        //Rune arune = new Rune();
                        actorStats astats = Central.Instance.getToy("airy_tower_hero");
                        astats.setActive(true);
                        //arune.initStats(RuneType.Airy, astats.getMaxLvl(), ToyType.Hero, astats.exclude_skills);
                        //ToySaver a = new ToySaver("airy_tower_hero", -1, arune, ToyType.Hero);
                        //Central.Instance.setHeroStats(a);

                        Rune vrune = new Rune();
                        actorStats vstats = Central.Instance.getToy("vexing_tower_hero");
                        vstats.setActive(true);
                      //  vrune.initStats(RuneType.Vexing, vstats.getMaxLvl(), ToyType.Hero, vstats.exclude_skills);
                        //ToySaver v = new ToySaver("vexing_tower_hero", -1, vrune, ToyType.Hero);
                        //Central.Instance.setHeroStats(v);

                        ScoreKeeper.Instance.SetScore(820f);
                        Central.Instance.game_saver.SaveGame(SaveWhen.BetweenLevels);
                    }
                    else
                    {
                        Central.Instance.level_list.special_skill_button_driver.SetParent(null);
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

        if (type == "Level")
        {
            click_outcome = ClickType.Success;
            int lvl = int.Parse(content);
            if (lvl == -1) click_outcome = ClickType.Cancel;

            if (!Central.Instance.setCurrentLevel(lvl)) {
                click_outcome = ClickType.Cancel;
                return;
            }
            Central.Instance.setLevelInfo(lvl);
            //Otherwise do onButtonClick below
        }

        if (type == "Won")
        {
            click_outcome = ClickType.Success;
            if (menu_button == MenuButton.ToMap) Central.Instance.changeState(GameState.LevelList, content);            
        }
        if (onButtonClicked != null)
        {
            onButtonClicked(type, content);
        }

        Noisemaker.Instance.Click(click_outcome);
    }

    public override void InitStartConditions()
    {
        
    }

    public override void SetInteractable(bool set)
    {
        
    }
}