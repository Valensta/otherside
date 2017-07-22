using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MyFastForwardButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {	
	public Peripheral peripheral;
    public Image image;
    public bool interactable;
    public FFButton my_ffbutton;
    

    public delegate void OnSelectedHandler(SelectedType type, string n);
    public static event OnSelectedHandler onSelected;

    void Start(){
		peripheral = Peripheral.Instance;
		enabled = true;
        my_ffbutton.ShowSelectedAccent(false);
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

        //my_ffbutton.ShowSelectedAccent(true);
        if (onSelected != null) onSelected(SelectedType.Null, "");
        peripheral.ChangeTime(TimeScale.SuperFastPress);
    }

	public void OnPointerUp(PointerEventData eventData){
		if (!enabled)
			return;
      //  my_ffbutton.ShowSelectedAccent(false);
        peripheral.ChangeTime(TimeScale.Normal);
    }




}