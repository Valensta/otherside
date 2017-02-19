using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MyFastForwardButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {	
	public Peripheral peripheral;
    public Image image;
    public bool interactable;

    public delegate void OnSelectedHandler(SelectedType type, string n);
    public static event OnSelectedHandler onSelected;

    void Start(){
		peripheral = Peripheral.Instance;
		enabled = true;
	}

    public void SetActiveState(bool set)
    {
        interactable = set;
        Color c = image.color;
        if (set) c.a = 1f; else c.a = 65f/255f;
        image.color = c;
    }
				
	public void OnPointerDown(PointerEventData eventData){
		if (!enabled)
			return;

        if (onSelected != null) onSelected(SelectedType.Null, "");

        peripheral.ChangeTime(6);
	}

	public void OnPointerUp(PointerEventData eventData){
		if (!enabled)
			return;
		
		peripheral.ChangeTime(6);
	}




}