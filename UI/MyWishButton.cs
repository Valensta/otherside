using System;
using UnityEngine;
using UnityEngine.UI;

public class MyWishButton : UIButton
{
    public Wish my_wish = null;
	    
	public Button my_button;
	public bool holding = false;
	public bool selected = false;	
	public GameObject parent;

    public int count;
    

	public bool interactable = false;
	public Peripheral peripheral;
    public GameObject info_box;
    
	public delegate void ButtonClickedHandler(string type, string content);
	public static event ButtonClickedHandler onButtonClicked; 

	
   // public Text strength;
  //  public Text time;
    public Text count_text;


    public override void Reset() { }
    

    public override void InitMe(){
        
		if (interactable) return;		
		
	}

    public void setCount(int by, bool abs)
    {
   //     Debug.Log($"Setting count for {my_wish.type} to {by} abs {abs}\n");
        if (abs) count = by; else count += by;
        if (count_text) count_text.text = count.ToString();
    }

    public int getCount()
    {
        return count;
    }

    public void SetWish(Wish w)
    {
//        Debug.Log($"Setting wish {w.type} {w.strength}\n");
        interactable = true;
        if (w == null)
        {
            setCount(0, true);
            my_wish = w;
            Zoo.Instance.returnObject(info_box, true);
            Debug.Log("Setting null sprite?\n");
            return;
            
        }
        else {
            my_wish = w;
            SetSprite("GUI/Inventory/" + w.type.ToString() + "_button_image");
                        
            info_box = Zoo.Instance.getObject("GUI/tiny_info/" + w.type.ToString() + "_tiny_info", false);
            info_box.transform.SetParent(transform);
            info_box.transform.localScale = Vector3.one;
            info_box.transform.localRotation = Quaternion.identity;
            info_box.transform.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;            

            updateInfoBoxLabel();
        }

    }

    public void updateInfoBoxLabel()
    {
        if (info_box == null) return;
        String str = (!my_wish.absolute ) ? Show.ToPercent(my_wish.getEffect()) : my_wish.getEffect().ToString();
        String[] hey = { str, Mathf.CeilToInt(my_wish.getTime()).ToString() };
        MyLabel l = info_box.GetComponent<MyLabel>();
        l.text.text = Show.FixText(l.content, hey);
    }

    void SetSprite(string  s)
    {
        Sprite sp = Resources.Load(s, typeof(Sprite)) as Sprite;
        my_button.image.sprite = sp;
    }

	void OnEnabled(){
		InitMe();
	}
	
	void Start(){
		InitMe();
		peripheral = Peripheral.Instance;

		enabled = true;
        SpyGlass.onSelected += onSelected;
        Peripheral.onSelected += onSelected;
        Island_Button.onSelected += onSelected;
        MyButton.onSelected += onSelected;
        MyFastForwardButton.onSelected += onSelected;
    }

    public void onSelected(SelectedType type, string n)
    {

        //if (n.Equals(""))
        if (type == SelectedType.Wish && n.Equals(this.name)) return;
       // Debug.Log("On selected " + type + " " + n + " turning off " + this.name + "\n");

        SetSelectedToy(false);
    }


    void onCreatePeripheral(Peripheral p){	
		peripheral = p;
	}
        		
	public void OnClick(){
	    if (EagleEyes.Instance.UIBlocked("MyWishButton","")) return;
        OnInput();
	}

    public void OnInput() {

        if (!enabled)

        {
            Noisemaker.Instance.Click(ClickType.Error);
            return;
        }
        

        Peripheral.Instance.ClearAll(SelectedType.Wish, this.name);
        if (!selected) { SetSelectedToy(true); //Debug.Log("Selecting inventory slot " + this.name + "\n");
            Noisemaker.Instance.Click(ClickType.Success);
        
        }
        else
        {
            SetSelectedToy(false);
            Noisemaker.Instance.Click(ClickType.Action);
            DoStuff();
            
        }

		if (onButtonClicked != null) {
			onButtonClicked (my_wish.type.ToString(), my_wish.Strength.ToString());
		}

	}

    void DoStuff()
    {
        if (my_wish == null) { Debug.Log("Trying to use an uninitialized wish!\n"); return; }

      //  Debug.Log("Using wish " + my_wish.type + " " + my_wish.strength + "\n");
        Peripheral.Instance.my_inventory.UseWish(my_wish);
    }

    public override void InitStartConditions()
    {
        
    }

    public override void SetSelectedToy(bool set)
    {
        
        selected = set;
        if (set) updateInfoBoxLabel();
        info_box.SetActive(set);
    }

    public override void SetInteractable(bool set)
    {
        interactable = set;
        my_button.interactable = set;
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
}