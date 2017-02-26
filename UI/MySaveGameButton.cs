using System;
using UnityEngine;
using UnityEngine.UI;

public class MySaveGameButton : UIButton
{
    public int id;
	    
	public Button my_button;	
	public bool selected = false;	
	public GameObject parent;
    public MultiLevelStateSaver game_saver;

	public bool interactable = false;

    
    public Text level;
    public Text score;
    
    public override void Reset() { }
    

    public override void InitMe(){
        
		if (interactable) return;		
		
	}

    public override void InitStartConditions()
    {
     //   throw new NotImplementedException();
    }

    public void OnInputSetSelectedToy(bool set)
    {
        Tracker.Log("MySaveGameButton SetSelected set " + set + " id " + id);
        SetSelectedToy(set);
    }

    public override void SetSelectedToy(bool set)
    {

        selected = set;        
        if (set)game_saver.SelectSaveGame(id, set);
    }

    public override void SetInteractable(bool set)
    {
     //   throw new NotImplementedException();
    }

    public void OnInputDeleteSaveGame()
    {
        Tracker.Log("MySaveGameButton DeleteSaveGame id " + id);

        ClickType click = (game_saver.DeleteSaveGame(id)) ? ClickType.Action : ClickType.Error;
        Noisemaker.Instance.Click(click);
    }

    public void OnInputCopyPreMadeSaveGame(int lvl)        
    {
        Tracker.Log("MySaveGameButton CopyPreMadeSaveGame lvl " + lvl);
        ClickType click = (game_saver.CopyPremadeSaveGame(id, lvl)) ? ClickType.Action : ClickType.Error;
        Noisemaker.Instance.Click(click);
    }

}