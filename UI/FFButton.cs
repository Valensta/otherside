using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FFButton : MonoBehaviour
{



    public Button button;
    public MyFastForwardButton ff_button;

    public void SetActiveState(bool set)
    {
        if (button != null) button.interactable = set;
        if (ff_button != null) ff_button.SetActiveState(set);
    }
    
}
