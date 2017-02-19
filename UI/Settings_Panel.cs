using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;


public class Settings_Panel : MonoBehaviour {
		
	public GameObject parent;	
	public MyLabel current_volume_label;
	public MyButton plus_volume;
	public MyButton minus_volume;
	int max_volume = 10;
	int min_volume = 0;
	public int current_volume;
    
	
	
	public void DisablePanel(){
		parent.SetActive(false);
	}
	
	public void TogglePanel(){
		parent.SetActive(!parent.activeSelf);
	}


    public bool IncreaseVolume()
    {
        if (current_volume == max_volume) return false;
        SetVolume(current_volume + 1);
        return true;
    }

    public bool DecreaseVolume()
    {
        if (current_volume == 0) return false;
        SetVolume(current_volume - 1);
        return true;
    }

    public void SetVolume(int v)
    {
        current_volume = v;
        AudioListener.volume = v / 10f;
        bool plus = true;
        bool minus = true;
        if (current_volume == 10) { plus = false; }
        if (current_volume == 0) { minus = false; }
        current_volume_label.text.text = current_volume.ToString();
        plus_volume.my_button.interactable = plus;
        minus_volume.my_button.interactable = minus;
        Debug.Log("Settings panel set volume to " + AudioListener.volume + "\n");
    }


}