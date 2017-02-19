using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;

[System.Serializable]
public class Sub_Toy_Button_Driver
{
	public List<Toy_Button> buttons;
	public List<MyLabel> labels;
	public GameObject panel;
	public GameObject info_panel;
	//public GameObject sell_panel;
    public GameObject hero_panel;
	public RuneType type;
    

	public Dictionary<Toy_Button, Button> button_map = new Dictionary<Toy_Button, Button>();
    
}

public abstract class Global_Toy_Button_Driver : MonoBehaviour {
    public List<Sub_Toy_Button_Driver> drivers = new List<Sub_Toy_Button_Driver>();
    public Dictionary<RuneType, int> map = new Dictionary<RuneType, int>();
    public Toy parent;
    public int current_driver;
    public bool show;
    public GameObject all;
    public Toy_Button selected_button;
    public MyLabel verbose_label;
    public RectTransform selected_buttom_image;


    void Start() {

        show = false;
    }


    public EffectType getSelectedEffectType()
    {
        return (selected_button == null) ? EffectType.Null : selected_button.effect_type;
    }

    public virtual void DisableMe()
    {
        show = false;

        SetAll(show);
    }


    public virtual void Init() {


        for (int i = 0; i < drivers.Count; i++) {
            Sub_Toy_Button_Driver driver = drivers[i];
            map[driver.type] = i;
            if (driver.button_map.Keys.Count > 0) continue;
            foreach (Toy_Button button in driver.buttons) {

                Button b = button.gameObject.GetComponent<Button>();
                if (b != null) {
                    driver.button_map.Add(button, b);
                }
            }
        }

        Peripheral.onDreamsChanged += onDreamsChanged;

    }

    void onDreamsChanged(float i, bool v, Vector3 pos)
    {
        if (show) CheckUpgrades();
    }

    public void setVerboseLabel()
    {
        if (selected_button == null)
        {
            verbose_label.gameObject.SetActive(false);
            // Debug.Log("No selected button, no verbose label\n");
            return;
        }
    //    Debug.Log("YES selected button, yes verbose label " + selected_button.toy_parent.rune.level + "\n");
        EffectType selected_type = getSelectedEffectType();

        verbose_label.gameObject.SetActive(true);
        verbose_label.gameObject.transform.SetParent(selected_button.transform);
        verbose_label.my_transform.localPosition = new Vector3(0f, 0.35f, 0f);
        verbose_label.my_transform.localScale = (Vector3.one) * 0.08f;
        setText(verbose_label, selected_type, selected_button.toy_parent.rune, false);
    }


    protected void SetAll(bool s) {
        // 	Debug.Log("Set all " + s + "\n");
        Sub_Toy_Button_Driver sub = drivers[current_driver];

        sub.panel.SetActive(s);
        if (sub.hero_panel != null && parent != null)
            sub.hero_panel.SetActive(parent.toy_type == ToyType.Hero);

        //if (s == false){
        if (sub.info_panel != null) sub.info_panel.SetActive(false);
        //if (sub.sell_panel != null) sub.sell_panel.SetActive(false);        
        Peripheral.Instance.Pause(false);
        //	}
        if (!s) setSelectedButton(null);
    }



    protected void SetDriver(RuneType type) {

        if (map.ContainsKey(type))
        {
            if (map[type] != current_driver)
            {
                Sub_Toy_Button_Driver sub = drivers[current_driver];
                sub.panel.SetActive(false);

                if (sub.hero_panel != null) sub.hero_panel.SetActive(false);
                if (sub.info_panel != null) sub.info_panel.SetActive(false);
                //if (sub.sell_panel != null) sub.sell_panel.SetActive(false);
                Peripheral.Instance.Pause(false);
            }
            current_driver = map[type];
        }
    }

    public virtual void setSelectedButton(Toy_Button select_me) {
        //Debug.Log("STUFF " + select_me + "\n");
        if (selected_buttom_image == null) return;

        if (select_me == null)
        {
            selected_buttom_image.gameObject.SetActive(false);
        } else
        {
            selected_buttom_image.gameObject.SetActive(true);
            RectTransform set_to = select_me.my_button.image.GetComponent<RectTransform>();
            selected_buttom_image.parent = set_to;
            selected_buttom_image.localScale = set_to.localScale;
            selected_buttom_image.anchoredPosition = set_to.anchoredPosition;
        }

    }

    public abstract void updateOtherLabels();
    
    public virtual void SetParent(Toy p){
        
		show = true;
		setStuff();
        
        SetAll(show);
        setSelectedButton(null);
    }

    protected bool UpgradeButton(string s){
		return (s == "upgrade" || s == "building_selected" || s == "upgrade_select");
	}

  

    Rune getCheckRune(Toy_Button button)
    {
        return (this is LevelList_Toy_Button_Driver) ?  button.toy_rune : parent.rune;
    }

    public void CheckUpgrades()
    {
        bool special_skills_mode = (this is LevelList_Toy_Button_Driver);


        foreach (KeyValuePair<Toy_Button, Button> kvp in drivers[current_driver].button_map)
        {
            Toy_Button button = kvp.Key;
            if (parent == null && !special_skills_mode) return;
            Rune check_rune = getCheckRune(button);



            //	Debug.Log("button upgrade " + kvp.Value + "\n");
            int level = 0;
            if (button.content_detail != "") { level = int.Parse(button.content_detail); }

            RuneType check_me_instead = RuneType.Sensible; // instead of parent.rune.runetype

            if (check_rune != null && UpgradeButton(button.type)
                        && (check_rune.CanUpgrade(button.effect_type, check_me_instead) || special_skills_mode))
            {//specialskillmode has a separate upgrade button
             //	Debug.Log("Level is " + level + " parent is " + parent.rune.getLevel(button.effect_type) + "\n");	
                if (level == 0 || check_rune.getLevel(button.effect_type) >= level)
                    kvp.Value.interactable = true;
                else
                    kvp.Value.interactable = false;

            }
            else if (!UpgradeButton(button.type))
            {
                if (check_rune != null && button.type.Equals("sell"))
                    kvp.Value.gameObject.SetActive(!(check_rune.toy_type == ToyType.Hero || check_rune.runetype == RuneType.Castle));
                    
                

                if (check_rune != null && button.type.Equals("move"))
                    kvp.Value.gameObject.SetActive(check_rune.toy_type == ToyType.Hero
                            && RewardOverseer.RewardInstance.getReward(RewardType.HeroMobility).unlocked);
                    

            }
            else if (check_rune != null && UpgradeButton(button.type) && check_rune.getLevel(button.effect_type) > level)
            {
                kvp.Value.interactable = false;
                //ColorBlock cb = kvp.Value.colors;
                //cb.colorMultiplier = 2.5f;
                //kvp.Value.colors = cb;

            }
            else
            {
                //ColorBlock cb = kvp.Value.colors;
                //cb.colorMultiplier = 1f;
                //kvp.Value.colors = cb;
                kvp.Value.interactable = false;

            }
        }
        StatSum statsum = null;
        if (!special_skills_mode) statsum = parent.rune.GetStats(false);
        List<MyLabel> labels = drivers[current_driver].labels;

        for (int i = 0; i < labels.Count; i++)
        {
            MyLabel label = labels[i];
            Rune check_rune = null;

            if (special_skills_mode)
                check_rune = Central.Instance.getHeroRune(label.runetype);
            else
                check_rune = parent.rune;

            statsum = (check_rune != null) ? check_rune.GetStats(special_skills_mode) : null;

            if (label.type.Equals("current_score"))
            {
                labels[i].text.text = Get.Round(ScoreKeeper.Instance.getTotalScore(), 0).ToString();
            }

        }
    }



    
    //only ingame but maybe?
	public void toggleInfo(){
		bool isactive = drivers[current_driver].info_panel.activeSelf;
		drivers[current_driver].info_panel.SetActive(!isactive);
		Peripheral.Instance.Pause (!isactive);
	}
	

	public void setStuff(){
	//Debug.Log("Setting info \n");
		List<MyLabel> labels = drivers[current_driver].labels;

        bool special_skills_mode = (this is LevelList_Toy_Button_Driver);

        if (show) {
            StatSum statsum = null;
            Rune check_rune = null;
            if (!special_skills_mode)
            {
                check_rune = parent.rune;
                statsum = check_rune.GetStats(false);
            }

            for (int i = 0; i < labels.Count; i++)
            {
                if (labels[i].text == null) continue;
                if (labels[i].type.Equals("info"))
                {

                    if (special_skills_mode)
                    {
                        check_rune = Central.Instance.getHeroRune(labels[i].runetype);
                        statsum = (check_rune != null) ? check_rune.GetStats(true) : null;
                    }


                    StatBit statbit = (statsum != null) ? statsum.GetStatBit(labels[i].effect_type) : null;

                    labels[i].text.text = "";
                    if (statbit != null && statbit.hasStat())
                    {
                        // Debug.Log(statbit.type + "\n");
                        //labels[i].text.text = statbit.getDetailStats()[0].toCompactString();
                        labels[i].text.text = "lvl: " + check_rune.getLevel(labels[i].effect_type).ToString();

                    }
                }

            }
        }
		
		CheckUpgrades();
       // setVerboseLabel();
        updateOtherLabels();

    }

    //special mode
    public void setText(MyLabel label, EffectType type, Rune r, bool verbose)
    {
        string now = "Current: ";
        string upgrade = "Upgrade: ";
     //   Debug.Log("Setting label for " + type + "\n");

        MyText current_desc = label.getText(LabelName.CurrentLvlSkillDesc);
        MyText next_desc = label.getText(LabelName.NextLvlSkillDesc);
        MyText generic_desc = label.getText(LabelName.Null);

        if (r == null || type == EffectType.Null || selected_button == null)
        {
            if (current_desc != null) current_desc.gameObject.SetActive(false);
            if (next_desc != null) next_desc.gameObject.SetActive(false);
        }
        else {

            StatBit stat = r.getStat(type);
            if (stat == null) { return; }

            if (current_desc != null)
            {
                current_desc.gameObject.SetActive(true);

                if (r.getLevel(type) > 0 || type == EffectType.Range)
                {
                    
                    if (verbose)
                        current_desc.setText(stat.getVerboseDescription());
                    else
                        current_desc.setText(now + stat.getCompactDescription(LabelName.CurrentLvlSkillDesc));
                }
                else {
                    if (verbose)
                        current_desc.setText("you don't have this skill yet, haha");
                else
                        current_desc.setText(now + " - don't have - ");
                }
            }

            if (next_desc != null)
            {
                next_desc.gameObject.SetActive(true);
                if (r.getLevel(type) == r.getMaxLevel())
                {
                    if (verbose)
                        next_desc.setText("can't upgrade anymore. enough. go away");
                    else
                        next_desc.setText(upgrade + " - no more - ");
                }
                else {
                    
                    if (verbose)
                        next_desc.setText(stat.getVerboseDescription(1));
                    else
                        next_desc.setText(upgrade + stat.getCompactDescription(LabelName.NextLvlSkillDesc,1));
                }
            }

            if (generic_desc != null)
            {
                generic_desc.gameObject.SetActive(true);
                
                if (!verbose)
                    generic_desc.setText(stat.getCompactDescription(LabelName.Null));

            }



        }

    }


}