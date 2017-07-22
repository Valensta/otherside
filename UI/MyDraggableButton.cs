using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;
using System;
//this should be in some kind of an interface 
public class MyDraggableButton : UIButton, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public Text text;
    bool am_initialized = false;
    public string content;
    public string type;
    public string content_detail;
    public RuneType rune_type;
    private Vector3 v3OrgMouse;
    private Plane plane = new Plane(Vector3.up, Vector3.zero);
    public Vector3 start_position;
	public Peripheral peripheral;
    SpyGlass my_spyglass;
    public bool interactable;
    public Image my_image;
    Color image_color;
    public float shift_scale;
    public Vector3 shift_offset;
  //  public float max_alpha = 1f;

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    public override void InitMe()
    {     
        if (am_initialized) return;        
        InitStartConditions();        
        if (Monitor.Instance != null) my_spyglass = Monitor.Instance.my_spyglass;
        
        Peripheral.onCreatePeripheral += onCreatePeripheral;
        Toy.onPriceUpdate += onPriceUpdate;
        EagleEyes.onPriceUpdate += onPriceUpdate;
        Central.onPriceUpdate += onPriceUpdate;

        am_initialized = true;
        image_color = my_image.color;
        interactable = true;       
    }
    
    public override void InitStartConditions()
    {     
        start_position = my_image.rectTransform.anchoredPosition;
        Reset();
    }

	public override void SetInteractable(bool i)            
    {
        
      //  Debug.Log("Setting draggable button interactable "  + content + " : " + i + "\n");
        if (interactable == i) return;
 
        interactable = i;
        Color c = image_color;
        if (i)
        {
            c.a = 1f;
        }
        else
        {
            c.a = 0.20f;
        }
        my_image.color = c;
    }

	void OnEnabled(){
		InitMe();
	}
	
	void Start(){

		InitMe();
		peripheral = Peripheral.Instance;

		enabled = true;
	}

	void onCreatePeripheral(Peripheral p){
		peripheral = p;
	}


    public override void SetSelectedToy(bool s)
    {
        if (!s && Peripheral.Instance.getSelectedToy() != content) return;
        
        if (peripheral == null) peripheral = Peripheral.Instance;
        if (peripheral == null) { Debug.Log("Peripheral is not present yet, why are you trying to do stuff with MyDraggableButton?\n"); return; }

        if (type.EndsWith("selected"))
        {
            if (s)
            {

                peripheral.SelectToy(content, rune_type);
            }
            else
            {
                peripheral.SelectToy(null, RuneType.Null);
            }
        }

        if (!s) EagleEyes.Instance.ClearInfo();

    }
    

	
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!interactable) return;
        
        SetSelectedToy(true);
        my_image.raycastTarget = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;
        plane.Raycast(ray, out dist);
        v3OrgMouse = ray.GetPoint(dist);
        
        if (Monitor.Instance != null) my_spyglass = Monitor.Instance.my_spyglass;
        my_spyglass.DisableByDragButton(true);
        Monitor.Instance.ShowIslandSprites(true, content);
        EagleEyes.Instance.floating_tower_scroll_driver.SetPanel(false);
        EagleEyes.Instance.global_rune_panel.DisableMe();
    }

    public void OnDrag(PointerEventData eventData)
    {
    //    Debug.Log("ON DRAG BEGIN\n");
        if (!interactable) return;

        EagleEyes.Instance.floating_tower_scroll_driver.UpdatePanel(null);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;
        plane.Raycast(ray, out dist);

        Vector3 shift = ray.GetPoint(dist) - v3OrgMouse;
        Vector3 scale = transform.lossyScale;
        shift.y *= -1;
        float blah = Vector3.Magnitude(scale);///2f;

        Vector3 offset = new Vector3(1f, 1f, 0f);
        Vector3 new_pos = start_position + shift_offset - shift / shift_scale;

        my_image.rectTransform.anchoredPosition = new_pos;

    }
    

    public void OnEndDrag(PointerEventData eventData)
    {
     //   Debug.Log("ON DRAG END\n");
        Reset();
    }

    public override void Reset()
    {

    //    Debug.Log("Resetting draggable button\n");
        if (Central.Instance.state != GameState.InGame) return;
        my_image.rectTransform.anchoredPosition = start_position;
        SetSelectedToy(false);
        if (Monitor.Instance != null) my_spyglass = Monitor.Instance.my_spyglass;
        if (my_spyglass != null) my_spyglass.DisableByDragButton(false);
        Monitor.Instance.ShowIslandSprites(false, "");
        my_image.raycastTarget = true;
    }

    public void onPriceUpdate(string name, float price)
    {

        //if (name == "sensible_tower")	Debug.Log("On price update " + name + " is? " + content + " or " + content_detail + " " + price + "\n");
        if (content.Equals(name) || content_detail.Equals(name))
        {
            if (text == null)
            {
                //Debug.Log ("toy_selected button " + name + " does not have text assigned, cannot update price!\n");
                return;
            }
       //     Debug.Log ("toy_selected button " + name  + " setting price to " + price + "\n");

            text.text = price.ToString();
        }

    }


}