using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelList : MonoBehaviour {
	public GameObject content_panel;
	[SerializeField]
	public List<Level> levels;
	// Use this for initialization
	public GameObject info_panel;
	public Slider difficulty_slider;
    public RectTransform CurrentLevelVisual;
    public LevelList_Toy_Button_Driver special_skill_button_driver;
    public Toggle test_mode_toggle;
    public Text current_lvl_text;
    public Text current_score_text;

    public bool test_mode;
    int max_lvl = 0; //max allowed level player has access to
    bool started = false;

    public int getMaxLvl()
    {
        return max_lvl;
    }

    public void SetStuff()
    {
        current_lvl_text.text = Central.Instance.game_saver.getCurrentGame().getDescription();
        current_score_text.text = Central.Instance.game_saver.getCurrentGame().getScoreText();

        int current_level = Central.Instance.getMaxLevel();// FOR NOW FOR TESTING PURPOSES
        int max_level = current_level;
        //	button.GetComponentInChildren<Button> ().interactable = false;

      

        Debug.Log("CURRENT LEVEL IS " + current_level + "\n");
        for (int i = 0; i < levels.Count; i++)
        {
            Level level = levels[i];
            if (i <= current_level || i == 0)
            {//tutorial is always enabled
                level.button.interactable = true;
                if (i == current_level)
                {
                    Debug.Log("setting current level visual to level " + i + "\n");
                    Central.Instance.level_list.CurrentLevelVisual.anchoredPosition = level.button.GetComponent<RectTransform>().anchoredPosition;
                    //central.level_list.CurrentLevelVisual.transform.localPosition = Vector3.zero;
                }
                //	Debug.Log("level " + i + " on\n");
            }
            else
            {
                level.button.interactable = false;
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
        max_lvl = (test_mode) ? 99 : max_lvl;


        if (!test_mode)

        foreach (Level l in levels)
        {

            if (l.test_mode == true && !test_mode) l.DisableMe();
        }

    }

    void Start()
    {

       // test_mode = (Application.isEditor);
        test_mode = false;
              test_mode_toggle.isOn = test_mode;
        

        levels.Sort((a, b) => a.number.CompareTo(b.number));
        setTestMode(test_mode);
        started = true;


    }

}
