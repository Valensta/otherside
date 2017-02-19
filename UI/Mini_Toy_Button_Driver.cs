using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;


public class Mini_Toy_Button_Driver : MonoBehaviour {
	Toy parent;
	//public Building building_parent;
	public GameObject status_panel;
	public GameObject upgrade;
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
    float time_of_last_xp_update = 0f;

    Color upgrade_me_please = new Color(1f, 186f / 255f, 156f / 255f);
   

	public void InitMiniDriver(Building p){
        Debug.Log("Initializing mini toy button driver as building\n");
        transform.SetParent(p.gameObject.transform);
		transform.localPosition = Vector3.zero;
        
        level_max = false;//cuz parent is not defined yet at this point
        setXPFull(false);
        //building_parent = p;
        canvas.worldCamera = Camera.main;
       // UpdateMe();
        
    }
	
	
	public void InitMiniDriver(Toy p){
     //   Debug.Log("Initializing mini toy button driver as toy\n");
        setXPFull(false);
        
        transform.SetParent(p.gameObject.transform);
		transform.localPosition = Vector3.zero;
		
		parent = p;
		canvas.worldCamera = Camera.main;
       

        if (parent is Firearm && ((Firearm)parent).Ammo() != -1)
        {
			if(ammo != null)ammo.gameObject.SetActive(true);
			SetAmmo();		
				
		}else{
			if(ammo != null)ammo.gameObject.SetActive(false);
			UpdateMe();
			
		}
		if (add_ammo != null)Inventory.onWishChanged += onWishChanged;
        if (upgrade != null) Peripheral.onDreamsChanged += onDreamsChanged;
        Sun.OnDayTimeChange += OnDayTimeChange;
		SetTimeBonus(Sun.Instance.GetCurrentTime());
        UpdateMe();
	}



    void OnDisable(){
        Inventory.onWishChanged -= onWishChanged;
		Sun.OnDayTimeChange -= OnDayTimeChange;
        level_max = false;//no cuz resetting, do it the stupid way
        xp_full = false;

    }

	public void onWishChanged(Wish w, bool added, bool visible, float delta){
        if (w.type != WishType.Sensible) return;
	
		if (add_ammo != null && parent is Firearm && ((Firearm)parent).CanAddAmmo(1))
			add_ammo.SetActive(true); 	
		else 
			add_ammo.SetActive(false);		
		
		SetUpgrade();
	}

    void onDreamsChanged(float i, bool v, Vector3 pos)
    {
        SetUpgrade();
    }

    public void OnDayTimeChange(TimeName name){
		SetTimeBonus(name);
	}
	
	void SetTimeBonus(TimeName name){
		if (time_bonus == null) return;
		if (StaticRune.GetTimeBonus(parent.rune.runetype, parent.rune.toy_type) > 0){
			time_bonus.gameObject.SetActive(true);
		}else{
			time_bonus.gameObject.SetActive(false);
		}
	}
	
	public void UpdateMe(){
	//	SetLevel ();
		SetUpgrade ();			
		SetXP();
	}
	
	public void SetXP(){

        float TIME = Peripheral.Instance.TIME;
        if (TIME > time_of_last_xp_update && TIME - time_of_last_xp_update < 0.5f) return;
		if (XP == null) return;
        if (level_max) return;		
		if (xp_full) return;
				
        if (setLevelMax()) return;//check again

      
        time_of_last_xp_update = Peripheral.Instance.TIME;

        float percent = parent.rune.getCurrentLevelXp() / parent.rune.getXpToNextLevel();
        if (percent >= 1){
           // Debug.Log("Set xp full because " + parent.rune.getCurrentLevelXp() + "/" + parent.rune.getXpToNextLevel() + " >= 1\n");
            percent = 1f;
            setXPFull(true);			
		}
		XP.sizeDelta = new Vector2(percent, XP.sizeDelta.y);		
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
        if (XP_image == null) return;
        if (level_max && xp_full)
        {

            XP_image.color = Color.gray;
            return;
        }
        else if (xp_full)
        {
            XP_image.color = upgrade_me_please;
        }
        else
        {
            XP_image.color = Color.green;
        }
    }
    /*
    public void SetLevel(){
		if (level != null){
			level.sizeDelta = new Vector2(parent.rune.level/10f, level.sizeDelta.y);		
		}
        setLevelMax();

        //level_max = true;
        setXPFull(false);
		//these show already purchased upgrades
		foreach (MyLabel label in labels){
			if (label.content == "effect" && parent.rune.get(label.effect_type) > 0 ){
				label.gameObject.SetActive(true);

			}else{
				label.gameObject.SetActive(false);
			}
		}
	}
    */
	public void SetAmmoPercentage(float i){
		ammo.sizeDelta = new Vector2(i, ammo.sizeDelta.y);
		//	Debug.Log("Setting ammo " + parent.name + " " + a + " " + ammo.sizeDelta + "\n");
		//if (i <= 0)add_ammo.SetActive(false);
	}

	public void SetAmmo(){
        if (!(parent is Firearm)) return;

		int a = ((Firearm)parent).Ammo();
        int max_ammo = (parent is Firearm) ? ((Firearm)parent).max_ammo : 0;

		ammo.sizeDelta = new Vector2(a/(float)max_ammo, ammo.sizeDelta.y);
	//	Debug.Log("Setting ammo " + parent.name + " " + a + " " + ammo.sizeDelta + "\n");
		if (((Firearm)parent).CanAddAmmo(1))add_ammo.SetActive(false);
	}


	void SetUpgrade(){		
		if (upgrade == null){ return;}
	//	Debug.Log("rune_buttons Checking upgrades " + this.parent.name + "\n");
		bool ok = false;
		StatSum sum = parent.rune.GetStats(false);
	    for (int i = 0; i < sum.stats.Length; i++) {
			if (parent.rune.CanUpgrade(sum.stats[i].effect_type, parent.rune.runetype))
			{										
				ok = true;
			}
		}
        if (ok == upgrade.gameObject.activeSelf) return;

        setXPFull(false);
        
        SetXP();
        upgrade.SetActive(ok);
	}

    


}