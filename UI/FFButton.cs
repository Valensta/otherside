using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FFButton : UIButton
{


    public TimeScale timescale;

    public Button button;
    public MyFastForwardButton ff_button;



    public void SetActiveState(bool set)
    {
        
        if (button != null) button.interactable = set;
        if (Central.Instance.current_lvl == 0) set = false;
        if (ff_button != null) ff_button.SetActiveState(set);
    }


}
