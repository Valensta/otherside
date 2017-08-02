using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine.UI;

public class LevelList : MonoBehaviour {
	public GameObject content_panel;
	[SerializeField]
	public List<Level> levels;
	// Use this for initialization
	public GameObject info_panel;
	
    public RectTransform CurrentLevelVisual;
    public LevelList_Toy_Button_Driver special_skill_button_driver;
    public GameObject special_skill_upgrade_flag;
    public Text current_lvl_text;
    public Text current_score_text;
    public GameObject difficulty_parent_object;
    public Slider difficulty_slider;
    public MyToggleGroup difficulty_buttons;
    public Text difficulty_label;
    public bool test_mode;
    int max_lvl = 0; //max allowed level player has access to
    bool started = false;

    public int getActualMaxLvl() //for events
    {
        return max_lvl;
    }

    public int getMaxLvl()
    {
        return (test_mode)? 99 :  max_lvl;
    }

    

    void onScoreUpdate(int score)
    {
        Debug.Log("LevelList Onscoreupdate\n");
        special_skill_button_driver.CheckUpgrades();
        setScore();
        setUpgradeFlag();
    }

    public void initDifficultyButtons()
    {
        List<LevelMod> level_mod = LevelStore.getLevelSettings(Central.Instance.current_lvl);

        Difficulty max_diff = level_mod[level_mod.Count - 1].difficulty;
        bool insane_allowed = ScoreKeeper.Instance.checkIfAlreadyHaveScore(Central.Instance.current_lvl, Difficulty.Hard);
        Debug.Log($"Max is {max_diff} insane allowed? {insane_allowed}\n");
        if (max_diff == Difficulty.Insane)
        {
                                    
            setDifficultyButton(2, true, insane_allowed, false);            
            setDifficultyButton(1, true, true, false);
            
        }else if (max_diff == Difficulty.Hard)
        {          
            setDifficultyButton(2, false, false, false);
            setDifficultyButton(1, true, true, false);
        }
        else
        {            
            setDifficultyButton(2, false, false, false);
            setDifficultyButton(1, false, false, false);            
        }
        setDifficultyButton(0, true, true, true);        
        
        
    }

    void setDifficultyButton(int i, bool active, bool interactable, bool selected)
    {
        Debug.Log($"Setting {((DifficultyButton)difficulty_buttons.toggles[i]).type} active {active} interactable {interactable}\n");
        ((DifficultyButton)difficulty_buttons.toggles[i]).SetInteractable(interactable);            
        difficulty_buttons.toggles[i].gameObject.SetActive(active);
        difficulty_buttons.toggles[i].Selected = selected;
    }
    
    public void initDifficultySlider()
    {
        
        List<LevelMod> level_mod = LevelStore.getLevelSettings(Central.Instance.current_lvl);

        int max_diff = (int)level_mod[level_mod.Count - 1].difficulty;

        if (max_diff > 1)
        {
            difficulty_parent_object.SetActive(true);
            difficulty_slider.minValue = 1;
            difficulty_slider.maxValue = max_diff;
            difficulty_slider.value = 1;
        }else
        {
            difficulty_slider.value = 1;
            difficulty_parent_object.SetActive(false);
        }
    }

    public void SetDifficulty(Difficulty diff)
    {
        levels[Central.Instance.current_lvl].difficulty = diff;
        if (Central.Instance.getState() == GameState.LevelList)
        {
            SetDifficultyLabel();
        }
    }
    
    public void SetDifficulty(String diff_string)
    {
        Difficulty diff = EnumUtil.EnumFromString(diff_string, Difficulty.Normal);
        levels[Central.Instance.current_lvl].difficulty = diff;
        if (Central.Instance.getState() == GameState.LevelList)
        {
            SetDifficultyLabel();
        }
    }

    public void SetDifficulty()
    {
        Debug.Log("Settting difficulty from SLIDER\n");
        Difficulty diff = _transformSliderDifficulty();

        SetDifficulty(diff);
    }

    void SetDifficultyLabel()
    {
        
        Difficulty diff = levels[Central.Instance.current_lvl].difficulty;
        int score = ScoreKeeper.Instance.getLevelScore(Central.Instance.current_lvl, diff);
        //difficulty_label.text = (score > 0) ? "(replay this difficulty)\n" + diff.ToString() : diff.ToString();
        difficulty_label.text = Get.getDifficultyName(diff);


    }

    public void setLevelInfo(int c)
    {
        //	Debug.Log("Setting level info " + c + " current level " + current_lvl + "\n");
        if (c == -1)
        {
            info_panel.SetActive(false);
        }
        else
        {
            info_panel.SetActive(true);
            initDifficultyButtons();
            //initDifficultySlider();
            for (int i = 0; i < levels.Count; i++)
            {
                Level level = levels[i];

                if (level.number == Central.Instance.current_lvl)
                {
                    //	Debug.Log("Turning on " + level.number + "\n");
                    level.description.SetActive(true);
                }
                else
                {
                    level.description.SetActive(false);
                }
            }
        }


    }

    Difficulty _transformSliderDifficulty()
    {

        int d = (int)difficulty_slider.value;
        switch (d)
        {
            case 1:
                return Difficulty.Normal;
            case 2:
                return Difficulty.Hard;                
            case 3:
                return Difficulty.Insane;
            default:
                return Difficulty.Normal;

        }

    }

    void setScore()
    {
        current_score_text.text = ScoreKeeper.Instance.getTotalScore().ToString();
    }

    void setLvl()
    {
        current_lvl_text.text = Central.Instance.game_saver.getCurrentGame().getDescription(true);
    }

    public void setUpgradeFlag()
    {
        if (Central.Instance.level_list.getActualMaxLvl() <= 1)
        {
            special_skill_upgrade_flag.SetActive(false);
            return;
        }

            bool yes_please = false;
        foreach (Rune rune in Central.Instance.hero_toy_stats)
        {
            if (rune.CanUpgradeASpecialSkill()) {
                yes_please = true;
                break;
            }
        }
        special_skill_upgrade_flag.SetActive(yes_please);
    }

    public void SetStuff()
    {
        setLvl();
        setScore();
        setUpgradeFlag();
    

            Debug.Log("CURRENT LEVEL IS " + max_lvl + "\n");
        for (int i = 0; i < levels.Count; i++)
        {
            Level level = levels[i];
            if (i <= max_lvl || i == 0 || test_mode)
            {//tutorial is always enabled
                level.button.my_button.interactable = true;
                if (level.button.selected_accent != null) level.button.ShowSelectedAccent(true);                

                if (i == max_lvl && level.button.selected_accent != null)
                {

                    Central.Instance.level_list.CurrentLevelVisual.position = level.button.selected_accent.GetComponent<RectTransform>().position;
                    //Central.Instance.level_list.CurrentLevelVisual.SetParent(level.button.selected_accent.transform);
                    //Central.Instance.level_list.CurrentLevelVisual.anchoredPosition = Vector3.zero;
                    Central.Instance.level_list.CurrentLevelVisual.gameObject.SetActive(!level.test_mode || (level.test_mode && test_mode));                                    
                }
                //	Debug.Log("level " + i + " on\n");
            }
            else
            {
                if (level.button.selected_accent != null) level.button.ShowSelectedAccent(false);
                level.button.my_button.interactable = false;
                 
                //		Debug.Log("level " + i + " off\n");
            }
        }



    }



    public void setMaxLvl(int _max_lvl)
    {
        max_lvl = _max_lvl;
    }

    public void toggleTestMode()
    {
        if (!started) return;
        test_mode = !test_mode;
        setTestMode(test_mode);
    }

    public void setTestMode(bool set)
    {
       // max_lvl = (test_mode) ? 99 : max_lvl;


        if (!test_mode)

        foreach (Level l in levels)
        {

            if (l.test_mode == true && !test_mode) l.DisableMe();
        }

    }

    void Start()
    {
#if UNITY_EDITOR
        test_mode = true;
#else

        test_mode = Get.myDevice();
#endif



        levels.Sort((a, b) => a.number.CompareTo(b.number));
        setTestMode(test_mode);
        started = true;
        ScoreKeeper.onScoreUpdate += onScoreUpdate;


    }

}
