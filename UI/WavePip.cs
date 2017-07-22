using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

[System.Serializable]

public class WavePip : MonoBehaviour {
	
	public int id;
    public TimeName timename;
    public Image my_image;
    public float scale = 1f;

    public void Init(int i, TimeName name) {
  //      Debug.Log("Initializing pip " + i + "\n");
        if (i < 0) {
          //  SetCurrent(false);
            my_image.gameObject.SetActive(false);
            return;
        }
        id = i;
        my_image.gameObject.SetActive(true);
        timename = name;
        SetColor();
        
        
        
    }



    void SetColor(){
        switch (timename)
        {
            case TimeName.Dawn:
                my_image.color = new Color(1f, 181f / 255f, 69f / 255f);
                break;
            case TimeName.Day:
                my_image.color = Color.yellow;
                break;
            case TimeName.Dusk:
                my_image.color = new Color(58f/255f, 62f/255f, 129f/255f);
                break;
            case TimeName.Night:
                my_image.color = Color.black;
                break;
            default:
                break;
        }
        
	}

	
}