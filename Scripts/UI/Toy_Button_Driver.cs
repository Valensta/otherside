using UnityEngine;
using System.Collections.Generic;




public class Toy_Button_Driver : MonoBehaviour {
	public Dictionary<RuneType, Sub_Toy_Button_Driver> drivers;
	public List<Toy_Button> buttons;
	public List<MyLabel> labels;
	public Firearm parent;	
	public bool drill_down;
	public bool scroll;
	public bool info;
	public MyLabel upgrade;
    
	public GameObject info_panel;
	public GameObject status_panel;
	public GameObject drill_down_panel;
	public GameObject scroll_panel;
	public RectTransform level;
	public bool global = false;

	//public delegate void ButtonClickedHandler(string type, string content);
	//public static event ButtonClickedHandler onButtonClicked; 

	void Start(){

		//foreach (Toy_Button button in this.transform.GetComponentsInChildren<Toy_Button>()) {
			//buttons.Add(button);  THIS IS DONE IN THE PREFAB OR NO?
			//button.button_driver = this;
		//}
		for (int i = 0; i < buttons.Count; i++) {
			buttons[i].toy_parent = parent;
			//if (buttons[i].content == "main"){ buttons[i].setEnabled(true);}
		}

		setDrillDown (false);
		setInfo (false);

	}

	public void setDrillDown(bool show){
		drill_down = show;
		scroll = false;
		if (show) {
			CheckUpgrades();
		} 

		else {
			info_panel.SetActive(false);
			foreach (Toy_Button button  in buttons) {
				if (button.type == "drilldown" && button.gameObject.activeSelf == true){
				//button.ren
					button.gameObject.SetActive (false);
				}
			}
		}
		drill_down_panel.SetActive (drill_down);
		scroll_panel.SetActive (scroll);
		status_panel.SetActive(!drill_down);
				
		if (!drill_down){
			level.sizeDelta = new Vector2(parent.rune.level/10f, level.sizeDelta.y);
		}
	}


	void CheckUpgrades(){		
	//Debug.Log("Checking upgrades\n");
		foreach (Toy_Button button  in buttons) {
			if ((button.type == "upgrade"  && button.toy_parent.rune.CanUpgrade(button.effect_type, button.toy_parent.rune.runetype)))
			{										
				button.gameObject.SetActive (true);
	//			Debug.Log("Can upgrade " + button.name + "\n");
				scroll = true;
			}else if (button.type != "upgrade") {				
				button.gameObject.SetActive (true);
			}else{
				button.gameObject.SetActive (false);
			}
		}
		upgrade.gameObject.SetActive(scroll);
	}

	public void setInfo(bool show){
		info = false;
		info_panel.SetActive(show);
		if (show) {
			StatSum statsum = parent.rune.GetStats(false);
			for (int i = 0; i < labels.Count; i++){
								//Debug.Log("Checking label " + labels[i].effect_type + "\n");
				StatBit statbit = statsum.GetStatBit(labels[i].effect_type);
				

				if (Get.isGeneric(statbit.effect_type) && statbit.hasStat()){
					if (labels[i].type == "info"){
						labels[i].text.text = statbit.getDetailStats()[0].toString();
						info = true;
					//	Debug.Log("updating text for " + labels[i].effect_type + "\n");
					}else{
						labels[i].gameObject.SetActive(true);
						info = true;
						//Debug.Log("updating image for " + labels[i].effect_type + "\n");
					}
				}else
				if (statbit.hasStat()){
					if (labels[i].type == "info"){
						labels[i].text.text = statbit.getDetailStats()[0].toString();
                        info = true;
						labels[i].gameObject.SetActive(true);
					//	Debug.Log("updating text for " + labels[i].effect_type + "\n");
					}else{
						labels[i].gameObject.SetActive(true);
						info = true;
					//	Debug.Log("updating image for " + labels[i].name + " " + labels[i].effect_type + "\n");
					}
				}else
				{
					labels[i].gameObject.SetActive(false);
				}
			}
		}
	}

	public void toggleInfo(){
	//	Debug.Log ("toggle info\n");
		setInfo (!info);
	}

	public void toggleDrillDown(){

		setDrillDown (!drill_down);
	}


}