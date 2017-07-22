using UnityEngine;
using UnityEngine.EventSystems;

public class ClearBackground : MonoBehaviour, IPointerClickHandler {
	
	public Monitor my_monitor = null;
	public Peripheral my_peripheral = null;
    WishType interactive_wish = WishType.Null;
    EffectType interactive_skill = EffectType.Null;
    bool interactive;


    public delegate void OnSelectedHandler(SelectedType type, string n);
    public static event OnSelectedHandler onSelected;

    //public delegate void OnInteractiveSkillClickHandler(WishType wish, EffectType skill);
    //public static event OnInteractiveSkillClickHandler onInteractiveSkillClick;

   public  void OnPointerClick(PointerEventData eventData){
		OnInput();
	}

    public bool OverBackground()
    {

        bool mouse = EventSystem.current.IsPointerOverGameObject();
        bool touch = false;
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) touch = true;
        
        //Debug.Log("Over background>? " + blah + "\n");
        return mouse || touch;


    }

    void OnInput(){
        //         Debug.Log("YES\n");
        if (EagleEyes.Instance.UIBlocked("ClearBackground", "")) return;
        if (Central.Instance.state != GameState.InGame) return;
        if (onSelected != null) { onSelected(SelectedType.Null, "");  }
      //  if (onInteractiveSkillClick != null) { onInteractiveSkillClick(interactive_wish, interactive_skill); }
 
    }
}
