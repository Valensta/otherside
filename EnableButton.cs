using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnableButton : MonoBehaviour {
	public string mytype;
    public Button mybutton;
	// Use this for initialization
	void Start () {
		MyButton.onButtonClicked  += onButtonClicked;
	}
	
	void onButtonClicked(string type, string content){
	//	Debug.Log ("Got onbuttonclicked event " + type + " " + content + "\n");
		if (mytype == type){
            getButton();
			mybutton.interactable = true;
		}
	}

    void getButton()
    {
        if (mybutton == null) mybutton = this.GetComponent<Button>();
    }
}
