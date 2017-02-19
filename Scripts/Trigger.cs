using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

[System.Serializable]
public abstract class Trigger 
{
    public Condition condition;
    public string text;
    protected float init_number = -1;
    public float number;
    protected bool clicked;
    protected bool selected = false;
    public TimeName my_timename;
    protected float default_delay = 0.15f;
    

    abstract public void Init();
    abstract public void DisableMe();

    public bool Check()
    {
        bool yes = CheckConditions();
        if (yes) DisableMe();
        return yes;
    }

    abstract public bool CheckConditions();

}


[System.Serializable]
public class RegularTrigger : Trigger {
	
    public RegularTrigger() { }
	public RegularTrigger(Condition c, string t, float n){
		condition = c;
		text = t;
		number = n;
		Init();
	}

	public RegularTrigger(Condition c, string t){
		condition = c;
		text = t;
		Init ();	
	}
	
	public RegularTrigger(Condition c, TimeName t){
		condition = c;
		my_timename = t;
		Init ();	
	}
	
	
	public RegularTrigger(Condition c, float f){
		condition = c;
		
		if (c == Condition.TIME){
			number = Peripheral.Instance.TIME + f;		
		}else {
			number = f;
		}	
		Init ();
	
	}

    private void Validate()
    {
        bool ok = true;
        if ((condition == Condition.GotWish  || condition == Condition.WishUsed ) && (text.Equals("") && number <= 0))
        {
            ok = false;
        }


        if (!ok) Debug.Log("Invalid parameters for trigger " + condition + " text " + text + " number " + number + "\n");
    }
	
	public override void  Init(){
		MyButton.onButtonClicked += onButtonClicked;
		Button_Event.onButtonClicked += onButtonClicked;
        Validate();
		if (condition == Condition.Selected)Peripheral.onSelected += onSelected;
		if (condition == Condition.PlacedToy)Peripheral.onPlacedToy += onPlacedToy;
		if (condition == Condition.WaveStarted)Peripheral.onWaveStart += onWaveStart;
		if (condition == Condition.WaveEnded)Moon.onWaveEnd += onWaveEnd;
        if (condition == Condition.OnLastWavelet) Moon.onLastWavelet += onLastWavelet;
        if (condition == Condition.LevelUp)Rune.onLevelUp += onLevelUp;
		if (condition == Condition.Lavaburn) Lava.OnLavaBurn += OnLavaBurn;
        if (condition == Condition.WishUsed || condition == Condition.GotWish) Inventory.onWishChanged += onWishChanged;
    }
	
    public override void DisableMe()
    {
        if (condition == Condition.Selected) Peripheral.onSelected -= onSelected;
        if (condition == Condition.PlacedToy) Peripheral.onPlacedToy -= onPlacedToy;
        if (condition == Condition.WaveStarted) Peripheral.onWaveStart -= onWaveStart;
        if (condition == Condition.WaveEnded) Moon.onWaveEnd -= onWaveEnd;
        if (condition == Condition.OnLastWavelet) Moon.onLastWavelet -= onLastWavelet;
        if (condition == Condition.LevelUp) Rune.onLevelUp -= onLevelUp;
        if (condition == Condition.Lavaburn) Lava.OnLavaBurn -= OnLavaBurn;
        if (condition == Condition.WishUsed || condition == Condition.GotWish) Inventory.onWishChanged -= onWishChanged;

    }



    public override bool CheckConditions()
    {

        switch (condition)
        {
            case Condition.TIME:
                if (Peripheral.Instance != null)
                {
                    return Peripheral.Instance.TIME > number;
                }
                else {
                    return false;
                }
            case Condition.TIMEInterval:
                if (Peripheral.Instance != null)
                {
                    float t = Peripheral.Instance.TIME;
                    if (init_number == -1) { init_number = t; }
                    if (init_number == 0) { init_number = default_delay; }
                    return Peripheral.Instance.TIME > init_number + number;
                }
                else { Debug.Log("Cannot find peripheral\n"); return false; }

            case Condition.Click:
                return clicked;
            case Condition.GameTimeReached:
           //     Debug.Log("CHecking for time of day reached, need " + my_timename + "\n");
                if (Sun.Instance.GetCurrentTime() == my_timename) return true;
                return false;
            case Condition.Selected:
                return selected;
            case Condition.LevelUp:
                return selected;
            case Condition.Lavaburn:
                return selected;
            case Condition.PlacedToy:
                return selected;
            case Condition.WaveStarted:
           //     Debug.Log("number " + number + " current wave " + Peripheral.Instance.current_wave + " astate " + Peripheral.Instance.level_state + "\n");
           
                if (Peripheral.Instance.current_wave == number && Peripheral.Instance.level_state == LState.WaveStarted) return true;// respect the start wave event which is triggered by clicking on the wave start button
                return false;
                //return selected;
            case Condition.WaveEnded:
                if (Peripheral.Instance.current_wave > number && Peripheral.Instance.monsters_transform.childCount <= 0) return true;
                return selected;
            case Condition.LevelWon: //this will never work for regular events, because peripheral waits for overseer.ingame_finished == true before declaring that the level is won
                if (Peripheral.Instance.level_state == LState.Won) return true;
          //      Debug.Log("Ok you won\n");
                return false;
            case Condition.OnLastWavelet:
            //    if (Peripheral.Instance.current_wave == number) return true;
                return selected;
            case Condition.WishAppears:
                if (Wishes.Instance.transform.childCount == 0) return false;

                foreach (Transform w in Wishes.Instance.transform)
                {                    
          //          Debug.Log("text " + text + " name " + w.name + "\n");
                    if (text.Equals("other"))
                    {
                        if (!Regex.Match(w.name, "Sensible").Success) return true;
                    }
                    else {
             //           Debug.Log("testing direct match\n");

                        if (Regex.Match(w.name.ToLower(), text).Success) return true;
                    }

                }
                return false;
            case Condition.GotWish:
                return selected;
              //  return CheckWish();
            case Condition.ClickedOnToy:
                if (text != "")
                {
                    //   if (Monitor.Instance.global_rune_panel.parent != null)
                    //       Debug.Log( (Monitor.Instance.global_rune_panel.parent.my_name == text) + " " + Monitor.Instance.global_rune_panel.show + "\n");
                    return (Monitor.Instance.global_rune_panel.parent != null && Monitor.Instance.global_rune_panel.parent.my_name == text && Monitor.Instance.global_rune_panel.show);
                }
                else {
                    return (!Monitor.Instance.global_rune_panel.show);
                }

            default:
                return false;
        }
    }

    public void onWishChanged(Wish w, bool added, bool visible, float delta)
    {
        WishType tryme = Get.WishTypeFromString(text);
        if (condition == Condition.GotWish)
        {
            if (w.type == tryme)
            {
                selected = CheckWish(tryme);
            }
        }
        else if (condition == Condition.WishUsed)
        {
            //selected = (RewardOverseer.Instance.Total_wishes_used >= number);
        }
    }


    bool CheckWish(WishType tryme) //RETIRED
    {
        //satisfied by any wish of that type

        List<Wish> all = Peripheral.Instance.my_inventory.getWishList();
        
            
            foreach (Wish w in all)
            {
                if (w.type == tryme && w.Strength >= number) return true;
            }

        
        return false;
    }
		

	void onButtonClicked(string type, string content){
     //   if (condition == Condition.Click) Debug.Log("Clicked on " + content + " need " + text + "\n");
		if (text.Equals(content))
			clicked = true;
	}
	
	void onLevelUp(RuneType type){
		if (type.ToString().Equals(text)){
			selected = true;
		}
	}
	
	void onSelected(SelectedType type, string content){
        Debug.Log("Got selected " + type + " " + content + "\n");
		if (text.Equals(content) || text.Equals(type.ToString().ToLower()))
			selected = true;
	}
	
	void onWaveStart(int content){
        //	Debug.Log ("wave start got " + content + " and looking for " + number + "\n");
        if (number < 0 || number == content)
			selected = true;
	}

	void OnLavaBurn(EffectType type){
	//	Debug.Log("saw a lava event\n");
		if (type.ToString().Equals(text)){
	//		Debug.Log("need " + text + " have " + type.ToString() + "\n");
			selected = true;
			Lava.OnLavaBurn -= OnLavaBurn;
		}
	}

	void onWaveEnd(int content){	
		int current_wave = Peripheral.Instance.current_wave;
	//	Debug.Log ("wave end got " + current_wave + " and looking for " + number + "\n");
		if (number == current_wave)
			selected = true;
	}

    void onLastWavelet(int content)
    {
    //    Debug.Log("Caught on last wavelet event, " + content + " want " + number + "\n");
       
        //	Debug.Log ("wave end got " + current_wave + " and looking for " + number);
        if (number == content)
            selected = true;
    }


    void onPlacedToy(string content){
  //      Debug.Log("trigger Got onplacedtoy " + content + ", need " + text + "\n");
		if (text == "" || content.Contains(text))
				selected = true;
	}
	
			
}	





	