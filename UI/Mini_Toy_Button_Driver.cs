using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;


public class Mini_Toy_Button_Driver : MonoBehaviour {
	private Toy parent;
	//public Building building_parent;
	public GameObject status_panel;
	
	public GameObject time_bonus;
	public RectTransform XP;
    public Image XP_image;
	//public RectTransform level;
  //  public Image level_image;
    public RectTransform ammo;
	public GameObject add_ammo;
	public Canvas canvas;
	public List<MyLabel> labels;
	public bool xp_full = false;
	public bool level_max = false;
	private float time_of_last_xp_update = 0f;
	private float percent_xp;
    //Color upgrade_me_please = new Color(1f, 186f / 255f, 156f / 255f, 0f);
	private bool upgrade_initialized = false;   

	public void InitMiniDriver(Building p){
   //     Debug.Log("Initializing mini toy button driver as building\n");
        transform.SetParent(p.gameObject.transform);
		transform.localPosition = Vector3.zero;
        
        level_max = false;//cuz parent is not defined yet at this point
        setXPFull(false);
		upgrade_initialized = false;
        //building_parent = p;
        canvas.worldCamera = Camera.main;
       // UpdateMe();
        
    }
	
	
	public void InitMiniDriver(Toy p){
       // Debug.Log("Initializing mini toy button driver as toy for " + p.my_name + "\n");
        parent = p;
                
        transform.SetParent(p.gameObject.transform);
		transform.localPosition = Vector3.zero;
		canvas.worldCamera = Camera.main;

        setXPFull(false);
        setLevelMax();


        if (parent.firearm != null && parent.firearm.Ammo() != -1)
        {
			if(ammo != null)ammo.gameObject.SetActive(true);
			SetAmmo();		
				
		}else{
			if(ammo != null)ammo.gameObject.SetActive(false);
			UpdateMe();
			
		}
		if (add_ammo != null)Inventory.onWishChanged += onWishChanged;
        
        Sun.OnDayTimeChange += OnDayTimeChange;
		SetTimeBonus(Sun.Instance.GetCurrentTime());
        UpdateMe();
	}


	private void OnEnable()
	{
		//if (upgrade) Show.SetAlpha(upgrade,1);

	}


	private void OnDisable(){
        Inventory.onWishChanged -= onWishChanged;
		Sun.OnDayTimeChange -= OnDayTimeChange;
        level_max = false;//no cuz resetting, do it the stupid way
        xp_full = false;
	    //if(upgrade) upgrade.gameObject.SetActive(false);
		if (parent && parent.building && parent.building.tower_visual) parent.building.tower_visual.setUpgrade(false);
    }

	public void onWishChanged(Wish w, bool added, bool visible, float delta){
        if (w.type != WishType.Sensible) return;
	
		if (add_ammo != null && parent.firearm != null && parent.firearm.CanAddAmmo(1))
			add_ammo.SetActive(true); 	
		else 
			add_ammo.SetActive(false);		
		
		SetUpgrade();
	}

    void onDreamsChanged(float i, bool v, Vector3 pos)
    {
	//    Debug.Log($"On dreams changed {i}\n");
        if (parent == null || !parent.active) return;
        SetUpgrade();
    }

    public void OnDayTimeChange(TimeName name){
        if (parent == null || !parent.active) return;
        SetTimeBonus(name);
	}
	
	void SetTimeBonus(TimeName name){
		if (time_bonus == null) return;
		if (StaticRune.GetTimeBonus(parent.rune.runetype, parent.rune.toy_type) > 0){
			time_bonus.gameObject.SetActive(false);
		}else{
			time_bonus.gameObject.SetActive(false);
		}
	}
	
	public void UpdateMe(){
	//	SetLevel ();
		SetUpgrade ();			
		SetXP();
	}

	public void InitUpgradeVisual()
	{
		if (upgrade_initialized) return;
		if (!parent || !parent.building || !parent.building.tower_visual) return; 
		if (parent == null || parent.rune.runetype == RuneType.Castle) return;

		if (!(parent.runetype == RuneType.Sensible || parent.runetype == RuneType.Airy ||
		      parent.runetype == RuneType.Vexing)) return;

		upgrade_initialized = true;
		Peripheral.onDreamsChanged += onDreamsChanged;

		
	}

	public void SetXP(){

        float TIME = Peripheral.Instance.TIME;
        if (TIME > time_of_last_xp_update && TIME - time_of_last_xp_update < 0.5f) return;
		if (XP == null) return;
      //  if (level_max) return;		
		//if (xp_full) return;
				
        if (setLevelMax()) return;//check again

      
        time_of_last_xp_update = Peripheral.Instance.TIME;

        float new_percent_xp = parent.rune.getCurrentLevelXp() / parent.rune.getXpToNextLevel();



		if (new_percent_xp >= 1)
		{
			new_percent_xp = 1f;
			setXPFull(true);
			return;
		}
		else
		{
			if (xp_full) setXPFull(false);
		}
        if (Mathf.Abs(new_percent_xp - XP.sizeDelta.y) < 0.05) return;
        
        XP.sizeDelta = new Vector2(0.042f, new_percent_xp);		
	}
	
    private bool setLevelMax()
    {
        if (parent == null) return false;
        level_max = parent.rune.isMaxLevel();
        _setColor();
        return level_max;
    }

    private void setXPFull(bool b)
    {
     //   Debug.Log("Set xp full " + b + "\n");
        xp_full = b;
        _setColor();
    }

    private void _setColor()
    {
	    
	    //if (parent != null) Debug.Log($"{parent.gameObject.name} Setting xp color level_max {level_max} xp_full {xp_full}\n");
        if (XP_image == null) return;
        if (level_max || xp_full)
        {
            XP_image.color = Color.clear;
        }
        else
        {
            XP_image.color = Color.green;
        }
    }
   
	public void SetAmmoPercentage(float i){
		ammo.sizeDelta = new Vector2(i, ammo.sizeDelta.y);
		//	Debug.Log("Setting ammo " + parent.name + " " + a + " " + ammo.sizeDelta + "\n");
		//if (i <= 0)add_ammo.SetActive(false);
	}

	public void SetAmmo(){
        if (parent.firearm == null) return;

		float a = parent.firearm.Ammo();
        float max_ammo = parent.firearm.max_ammo;
        float hey = a / max_ammo;
  //      Debug.Log($"{parent.gameObject.name} Ammo {a} / {max_ammo} = {hey}\n");



        ammo.sizeDelta = new Vector2(hey, ammo.sizeDelta.y);
	//	Debug.Log("Setting ammo " + parent.name + " " + a + " " + ammo.sizeDelta + "\n");
		if (parent.firearm.CanAddAmmo(1)) add_ammo.SetActive(false);
	}


	void SetUpgrade()
	{
		InitUpgradeVisual();


		if (parent.building.construction_in_progress)
		{
			if (parent.building.tower_visual) parent.building.tower_visual.setUpgrade(false);
			return;
		}
		if (!parent.building.tower_visual || !parent.building.tower_visual.haveUpgrades) return;
		
		//if (upgrade == null){ return;}
//		Debug.Log("rune_buttons Checking upgrades " + this.parent.name + "\n");
		bool ok = false;
		StatSum sum = parent.rune.GetStats(false);
	    for (int i = 0; i < sum.stats.Length; i++) {
			if (parent.rune.CanUpgrade(sum.stats[i].effect_type, parent.rune.runetype) == StateType.Yes)
			{										
				ok = true;
			}
		}
        //if (ok == upgrade.gameObject.activeSelf) return;

        //setXPFull(false);
        
        SetXP();
		parent.building.tower_visual.setUpgrade(ok);
		
		//Show.SetAlpha(upgrade, 1);
        //upgrade.gameObject.SetActive(ok);
	//	Debug.Log("Enabled upgrage visual\n");
	}

    


}