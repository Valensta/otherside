using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;


public class GenericPanel : MonoBehaviour {

    public enum PanelType { Horizontal, Vertical, Circle }    
	public List<PanelObject> list;
    public PanelType panel_type;
    public float spacing;
    public float radius;
    public float x_offset; //for horizontal/vertical only
    public float y_offset; //for horizontal/vertical only
    public RectTransform background_image;
    public int current_buttons;
    public bool is_empty;

    public bool dynamic_spacing = false;
    public float max_width; //if dynamic

    bool initialized = false;
    public GameObject info_panel;

    
    List<RectTransform> transforms = new List<RectTransform>();
    public void Start()
    {
        Init();
    }

    public void Init()
    {
        if (initialized) return;
        //Debug.Log("Initializing panel " + this.name + "\n");
        transforms = new List<RectTransform>();
        foreach (PanelObject l in list)
        {
            AddLabel(l, false, false);
        }
        initialized = true;
        UpdatePanel();
        
    }
    
    public void RemoveLabel(string name)
    {
        
            for (int i = 0; i < list.Count; i++)
        {
            if (list[i].name.Equals(name))
            {
                if (list[i].do_not_display) return;
                list.RemoveAt(i);
                transforms.RemoveAt(i);
            }else
            {
                Debug.Log("Could not find label " + name + " to remove\n");
            }
        }
    }

    public void AddLabel(PanelObject l, bool setparent, bool update)
    {
        if (!l.do_not_display)
        {
            if (setparent)
            {
                l.transform.parent = this.transform;
                l.transform.localScale = Vector3.one;
                l.transform.localRotation = Quaternion.identity;
                list.Add(l);
            }
            l.SetPanel(this);

            transforms.Add(l.GetComponent<RectTransform>());
            if (update) UpdatePanel();
        }
        l.InitButton();
        
    }

    public void UpdatePanel()
    {
        if (!initialized) Init();

        is_empty = true;
        int current = 0;
          //   Debug.Log("Updating list panel " + this.name + "\n");
       
        if (panel_type == PanelType.Circle || dynamic_spacing)
        {
            current_buttons = 0;
            for (int i = 0; i < list.Count; i++)
            {
                PanelObject l = list[i];
                if (l.do_not_display) continue;

                if (l != null && l.gameObject.activeSelf)
                {                                     
                    current_buttons++;
                }
            }
            if (current_buttons == 0)
            {
                setBackground(true, 0);
                return;
            }
     //       Debug.Log("List Panel has " + current_buttons + "\n");
        }

        for (int i = 0; i < list.Count; i++)
        {
            PanelObject l = list[i];
            if (l.do_not_display) continue;
            
            if (l != null && l.gameObject.activeSelf)
            {
                Vector3 pos = getPosition(i, current);
                transforms[i].anchoredPosition = pos;
                current++;
            }
        }

        if (current > 0) is_empty = false;
        setBackground(is_empty, current);
       
    }

    void setBackground(bool is_empty, int current)
    {
        if (background_image != null && current_buttons != current)
        {
            Vector3 bg_pos = background_image.anchoredPosition;
            Vector3 bg_size = Vector3.one;
            if (panel_type == PanelType.Horizontal)
            {
                bg_size.x = current;
                bg_pos.x = spacing * (current - 1) / 2f;
                
            }
            else
            {
                bg_size.y = current;
                bg_pos.y = spacing * (current - 1) / 2f;
                
            }
            background_image.localScale = bg_size;
            background_image.anchoredPosition = bg_pos;
        }
        current_buttons = current;
    }

    Vector3 getPosition(int i, int current)
    {

        Vector3 pos = transforms[i].anchoredPosition;

        switch (panel_type)
        {
            case PanelType.Circle:

                //float angle = current * 2 * Mathf.Asin(spacing / (2 * radius));
                float angle = 2f * Mathf.PI * (float)current / (float)current_buttons;
                pos.x = radius * Mathf.Cos(angle) + x_offset;
                pos.y = radius * Mathf.Sin(angle) + y_offset;
                return pos;
            case PanelType.Horizontal:
                pos.x = (dynamic_spacing)? (float)current * ((float)max_width / (float)(current_buttons - 1f)) : current * spacing;

                pos.x += x_offset;
                pos.y = y_offset;
                return pos;
            case PanelType.Vertical:
                pos.y = (dynamic_spacing) ? (float)current * ((float)max_width / (float)(current_buttons - 1f)) : current * spacing;
                pos.x = x_offset;
                pos.y += y_offset;
                return pos;

        }
        return pos;
    }

}