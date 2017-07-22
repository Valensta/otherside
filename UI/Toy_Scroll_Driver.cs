using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Toy_Scroll_Driver : MonoBehaviour {

    [SerializeField]
    public List<GenericPanel> panels = new List<GenericPanel>();


	public GameObject my_button;
    public int current_panel = 1;
    int closed_panel_state = 0;
	
    public void Hide(bool hide)
    {
        foreach(GenericPanel panel in panels)
        {
            panel.gameObject.SetActive(!hide);
        }
    }

	public void IncrementPanel()
    {
      //  Debug.Log("increment panel\n");
        int tries = panels.Count - 1;
        bool am_ok = false;
        while (!am_ok)
        {

            current_panel++;
            if (current_panel >= panels.Count)
            {
                current_panel = -1;
            }
            if (current_panel != -1) panels[current_panel].UpdatePanel();
            if (current_panel == -1 || !panels[current_panel].is_empty) am_ok = true;
            if (tries <= 0) am_ok = true;
            tries--;
        }
        SetPanels();
    }

    public void Init()
    {
        current_panel = 0;
        SetPanels();
        my_button.SetActive(true);
    }

    void SetPanels()
    {
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].Init();
            if (i == current_panel)
            {
                panels[i].UpdatePanel();
                panels[i].gameObject.SetActive(true);
            }
            else
            {
                panels[i].gameObject.SetActive(false);
            }
        }
    }

}
