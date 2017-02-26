using System;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEditor;
using UnityEngine.EventSystems;

public enum IslandType { Permanent, Temporary, Either, Null };

public class Island_Button : MonoBehaviour, IPointerClickHandler, IDropHandler,IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
	public string content = "";
	public bool blocked = false;
	public GameObject parent;
	public GameObject explosion_parent;
	public float size =1;	
	public IslandType island_type;
	GameObject signal;
	public Transform center;
	public Toy my_toy;
	//public Building my_building;
	
	Peripheral peripheral;
	public delegate void ButtonClickedHandler(string type, string content);
	public static event ButtonClickedHandler onButtonClicked;
    private string selected_toy = "";
    private bool am_selected = false;
    public string dead_island = "";
	dead_island block;
    public IslandType original_island_type = IslandType.Null;
    SpriteRenderer my_sprite;

    public SpriteRenderer My_sprite
    {
        get
        {
            if (my_sprite == null && this.transform.childCount > 0) my_sprite = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
            return my_sprite;
        }
        
    }

    public bool Am_selected
    {
        get
        {
            
            return am_selected;
        }

        set
        {
            am_selected = value;
          //  if (Selected_toy.Equals("")) HighlightMe(am_selected);
        }
    }

    public string Selected_toy
    {
        get
        {
            return selected_toy;
        }

        set
        {
            selected_toy = value;
          //  HighlightMe(false);
        }
    }

    public delegate void OnSelectedHandler(SelectedType type, string n);
    public static event OnSelectedHandler onSelected;

    void onCreatePeripheral(Peripheral p){  
		peripheral = p;
	}

    public void ChangeType(IslandType new_type)
    {
        
        Debug.Log(transform.parent.gameObject.name + " Going from " + island_type + " to " + new_type + "\n");
        if (blocked || my_toy != null) { Debug.Log("Attempting to change an island (" + transform.parent.gameObject.name + ") that is not empty!\n"); }
        
        if (My_sprite == null) { Debug.Log("Islandbutton " + transform.parent.gameObject.name + " could not find a sprite renderer!\n"); }
        else
        {
            Sprite new_sprite = null;
            if (new_type == IslandType.Permanent) new_sprite = Get.getSprite("Levels/red_island");
            else new_sprite = Get.getSprite("Levels/blue_island");
            My_sprite.sprite = new_sprite;
        }
        
        island_type = new_type;
    }

	public Peripheral getPeripheral()
    {
        if (peripheral == null) { peripheral = Peripheral.Instance; }
        return peripheral;
    }

    public void ResetIslandType()
    {
     //   Debug.Log((int)original_island_type + " " + original_island_type + " is nott " + (int)island_type + " " + island_type + "\n");
        if (original_island_type.Equals(island_type)) return;
            ChangeType(original_island_type);
    }

    void Awake()
    {
        if (original_island_type == IslandType.Null) original_island_type = island_type;
        
    }

	void Start(){
        
		size = this.transform.parent.localScale.magnitude;
		if (peripheral == null){ peripheral = Peripheral.Instance;}
	}


	public void MakeDeadIsland(float timer){
		if (dead_island == ""){ dead_island = "Props/dead_island";}
        my_toy = null;
        block = Peripheral.Instance.zoo.getObject(dead_island, true).GetComponent<dead_island>();
		Vector3 pos = this.transform.position;
		pos.z += 1f;
        float make_me = (timer > 0) ? timer : 10f;
        
            block.EnableMe(make_me, this);
        
		block.transform.position = pos;

        blocked = true;
	}
    

    public IslandSaver getSnapshot()
    {
        IslandSaver island_saver = null;
        if (my_toy != null)
        {
			island_saver = new IslandSaver(this.parent.name, my_toy.getSnapshot());
        }else if (block != null)
        {
            island_saver = new IslandSaver(this.parent.name, block.my_time);
        }
        if (island_type != original_island_type)
        {
            island_saver = (island_saver == null) ? new IslandSaver() : island_saver;
            island_saver.name = this.parent.name;
            island_saver.island_type = island_type;
        }

        return island_saver;
    }

	public void OnPointerClick(PointerEventData eventData){
		OnInput(true);
 
    }


    public string verify_toy_for_distance(string toy_name)
    {

        string return_me;
        if (Central.Instance.getToy(toy_name).required_building == "" || StaticRune.GetDistanceBonus(toy_name, this.transform.position, null) > 0) return_me = toy_name;
        else return_me = "TOOFAR";
    //    Debug.Log("Checking distance for " + Central.Instance.getToy(toy_name).required_building  + " : " + toy_name + " -> " + return_me + "\n");
        return return_me;
    }


    public float getSize()
    {
        if (island_type == IslandType.Permanent) return 1; else return size;
    }

	public bool DoAThing(){		
		if (Selected_toy == "") 
		{
			return false;
		}
        
	
		blocked = peripheral.PlaceToyOnIsland(Selected_toy, this.gameObject);
        Am_selected = false;
        Peripheral.Instance.SelectToy("", RuneType.Null);
		return blocked;
	}
    
    public void OnDrop(PointerEventData eventData)
    {
        if (!peripheral.SomethingSelected())
        {
         //   Monitor.Instance.my_spyglass.InitiateSpyglass();
            return;
        }
   //     Debug.Log("On island drop");
   
        OnInput(false);
    }

    public void OnPointerDown(PointerEventData eventdata)
    {
       // bool drag_mode = EagleEyes.Instance.mobile_tower_scroll_driver.DragMode();

         Monitor.Instance.my_spyglass.InitiateSpyglass();
    }

public void OnPointerEnter(PointerEventData eventData)
    {        
        if (!EagleEyes.Instance.floating_tower_scroll_driver.DragMode()) return;

        bool something_selected = peripheral.SomethingSelected();
        if (!something_selected) return;
        if (something_selected && (blocked || my_toy != null)) return;        
        OnInput(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!EagleEyes.Instance.floating_tower_scroll_driver.DragMode()) return;

        if (Selected_toy.Equals("")) return;

        Selected_toy = "";        
        if (onSelected != null) onSelected(SelectedType.Island, "");
    }

   void HighlightMe(bool set)
    {
        if (set) Show.SetAlpha(My_sprite, Show.IslandHighlightedAlpha);
        else Show.SetAlpha(My_sprite, Show.IslandRegularAlpha);
    }

    void OnInput(bool is_click)
    {
        Tracker.Log("Island_Button OnInput is_click " + is_click);
        if (Central.Instance.getState() != GameState.InGame) return;
        bool drag_mode = EagleEyes.Instance.floating_tower_scroll_driver.DragMode();

        if (!drag_mode) Monitor.Instance.my_spyglass.InitiateSpyglass();

        if (block != null && block.gameObject.activeSelf == false) block = null;
        if (blocked && block == null && my_toy == null) blocked = false;
        if (blocked || my_toy != null) {
            //Monitor.Instance.SetIsland(parent.name);
            if (onSelected != null) onSelected(SelectedType.Island, parent.name);
            return;
        }

        Selected_toy = "";

        string selected = peripheral.getSelectedToy();

        if (is_click && onSelected != null)
        {
            onSelected(SelectedType.DirectIsland, parent.name);//when tapping an empty island, deselect everything else, only need bool is_click for mouse, not an issue for phone?
        }
      //  if (is_click && !blocked && selected != null) Noisemaker.Instance.Click(ClickType.Success);

        unitStats stats = Central.Instance.getToy(selected);
        if (stats == null)
        {            
            //I DON'T KNOW
            //  if (is_click && onSelected != null) onSelected(SelectedType.DirectIsland, parent.name);//when tapping an empty island, deselect everything else, only need bool is_click for mouse, not an issue for phone?
            return;
        }

        

        if (stats.island_type != IslandType.Either && stats.island_type != island_type)
        {            
            return;
        }

        if (selected != "") Selected_toy = verify_toy_for_distance(selected);

     //   Debug.Log("Selected toy is " + selected_toy + "\n");
        //Monitor.Instance.SetIsland(parent.name);
        if (drag_mode && onSelected != null) onSelected(SelectedType.Island, parent.name);
    }

    void OnInputOwnButtonPanel()
    {

    }

}

