using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[System.Serializable]
public class SpyGlass : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
	private Vector3 init_position;
	private Vector3 v3OrgMouse;
	//private float time;
	private Plane plane = new Plane(Vector3.up, Vector3.one);
	private bool disabled_by_drag_button;
    private bool disabled_by_event;
    private bool disabled_by_gamestate;
    public float max_x = 10f;
	public float max_y = 10f;
    public float map_x_size = 1f;
    public float map_y_size = 1f;
    Transform my_transform;

    public Monitor my_monitor = null;
    public Peripheral my_peripheral = null;
    WishType interactive_wish = WishType.Null;
    EffectType interactive_skill = EffectType.Null;

    public delegate void OnSelectedHandler(SelectedType type, string n);
    public static event OnSelectedHandler onSelected;

    float total_shift;
    bool initiated;

	void Start () {
		//y = transform.position.y;  // Distance camera is above map
		
        initiated = false;
        if (my_transform == null) my_transform = Camera.main.gameObject.transform;
        init_position = my_transform.position;
        float screen_x = Screen.currentResolution.width;
        float screen_y = Screen.currentResolution.height;
        float screen_ratio = screen_x / screen_y;
        //float default_ratio = 1920f / 1080f;

        float ortho_size = Camera.main.orthographicSize;

        float bg_sprite_scale = Monitor.Instance.background_image.gameObject.transform.localScale.x;
	    
	    
        max_y = (map_y_size / 2f - ortho_size) * bg_sprite_scale;
        max_x = (map_x_size / 2f - ortho_size * screen_ratio) * bg_sprite_scale; //not quite right
	    
//	    Debug.Log($"bg_sprite_scale {bg_sprite_scale} : map_x_size {map_x_size} -> {max_x},  map_y_size {map_y_size} -> {max_y}");
  
    }

    public bool OverBackground()
    {

        bool mouse = EventSystem.current.IsPointerOverGameObject();
        bool touch = Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

        //Debug.Log("Over background>? " + blah + "\n");
        return mouse || touch;


    }

    public void Reset(){
        if (my_transform == null) return; 
        my_transform.position = init_position;
        initiated = false;
    }
    
	public void DisableByEvent(bool _enabled){        
        disabled_by_event= _enabled;
        initiated = false;
	}

    public void DisableByGameState(bool _enabled)
    {
     //   Debug.Log("Disabled by gamestate " + _enabled + "\n");
        disabled_by_gamestate = _enabled;
        initiated = false;
    }

    public void DisableByDragButton(bool _enabled)
    {
      //  if (disabled_by_drag_button != _enabled) Debug.Log("Disabled by drag button " + _enabled + "\n");
        disabled_by_drag_button = _enabled;
        initiated = false;
    }

    public bool isDisabledByDragButton()
    {
        return disabled_by_drag_button;
    }


    public void PointSpyglass(Vector2 new_pos,float window, bool force)
    {
        //bool in_window = true;
        Vector2 me = my_transform.position;
        Vector2 dist = new_pos - me;
        //  Debug.Log(me + " to " + dist + " -> x = " + map_x_size + " y = " + map_y_size + "\n");
        Debug.Log("Point spyglass to " + new_pos + " from " + my_transform.position + "\n");
        if (!force && Mathf.Abs(dist.x) < map_x_size * window && Mathf.Abs(dist.y) < map_y_size * window) return;



     
        new_pos = CheckBoundaries(new_pos);
      //  Debug.Log("Checked Point spyglass to " + new_pos + "\n");
       
        StartCoroutine(LerpSpyGlass(0.6f, new_pos));
    }
   

    public void PointSpyglass(Vector2 new_pos, bool force)
    {        
        PointSpyglass(new_pos, 1f, force);   
    }
    

    IEnumerator LerpSpyGlass(float time, Vector3 new_pos)
    {
        
        disabled_by_gamestate = true;
        float per_second = 25f;
        float steps = time * per_second;
        Vector3 start_pos = my_transform.position;
        new_pos.z = start_pos.z;
        float i = 0f;
        while (i < steps)        
        {
            float t = i / steps;
            t = t * t * t * (t * (6f * t - 15f) + 10f);
            my_transform.position = Vector3.Lerp(start_pos, new_pos, t);
            i += 1f;
            yield return new WaitForSeconds(1f/per_second);
        }
        disabled_by_gamestate = false;
        yield return null;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (EagleEyes.Instance.UIBlocked("SpyGlass", "")) return;
        initiated = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (EagleEyes.Instance.UIBlocked("SpyGlass", "")) return;
     //   Debug.Log("SpyGlass OnPointerClick\n");
        
        if (Central.Instance.state != GameState.InGame) return;
        if (onSelected != null) { onSelected(SelectedType.Null, ""); }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (EagleEyes.Instance.UIBlocked("SpyGlass", "")) return;
        //     Debug.Log("SpyGlass OnPointerDown\n");

        InitiateSpyglass();
    }

    public void InitiateSpyglass()
    {
        if (initiated) return;
        if (disabled_by_event || disabled_by_drag_button || disabled_by_gamestate || Peripheral.Instance.getCurrentTimeScale() == TimeScale.SuperFastPress) return;
        //Debug.Log("Spyglass initiated\n");
        initiated = true;
    }

    void Update () {

        //if (disabled_by_event || disabled_by_drag_button || disabled_by_gamestate) return;
        if (!initiated) return;
        if (!OverBackground()) return;

        if (Peripheral.Instance.getCurrentTimeScale() == TimeScale.SuperFastPress)
        {
            initiated = false;
            return;
        } 

        if (Input.GetMouseButtonDown (0)) {
            //  Debug.LogError("Got mouse button down\n");
            total_shift = 0f;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			float dist;
            
            plane.Raycast (ray, out dist);
			v3OrgMouse = ray.GetPoint (dist);
			//time = Time.deltaTime;
			//v3OrgMouse.y = 0;
		} 
		else if (Input.GetMouseButton (0)) {
         //   Debug.Log("Got mouse button up\n");
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			float dist;
			plane.Raycast (ray, out dist);
			
			Vector3 shift = ray.GetPoint (dist) - v3OrgMouse;
            total_shift += Vector3.Magnitude(shift);

            if (total_shift < 0.5f) return;
			Vector3 new_pos = my_transform.position - shift;
            new_pos = CheckBoundaries(new_pos);
            my_transform.position = new_pos;
           //transform.position = new_pos;
            
        }
	}

    Vector3 CheckBoundaries(Vector3 new_pos)
    {
        if (new_pos.x > max_x) new_pos.x = max_x;
        if (new_pos.x < -max_x) new_pos.x = -max_x;
        if (new_pos.y > max_y) new_pos.y = max_y;
        if (new_pos.y < -max_y) new_pos.y = -max_y;
        return new_pos;
    }
}