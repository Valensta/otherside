using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;


public class Island_Floating_Button_Driver : MonoBehaviour {

    public GenericPanel my_panel;
    public Island_Button selected_island = null;
    public Mobile_Toy_Button selected_button;
    public RectTransform selected_button_image = null;
    public Image selected_island_image = null;


    List<RectTransform> transforms = new List<RectTransform>();
/*
    public bool DragMode()
    {
        return selected_island == null;
    }
    */
    public void Start()
    {

        SetPanel(false);
        
    }


    void OnEnabled()
    {
        SetPanel(false);
    }


    public void SetPanel(bool set)
    {
        if (set) Debug.Log("Floating setting panel " + set + "\n");
        my_panel.gameObject.SetActive(set);
    }

    public void ResetSelected()
    {
    //    Debug.Log("Resetting selected\n");
        selected_button = null;
        Peripheral.Instance.SelectToy("", RuneType.Null, false);
        Monitor.Instance.InitMainSignal(RuneType.Null, ToyType.Null);
        foreach (MyLabel l in my_panel.list)
        {
            l.ui_button.SetSelectedToy(false);
            l.SetHidden(false);
        }
        selected_button_image.gameObject.SetActive(false);
        selected_island_image.sprite = TowerStore.getPreviewSprite(RuneType.Null, ToyType.Null);
        selected_button_image.gameObject.SetActive(false);
    }

    public void SelectButton(Mobile_Toy_Button button, bool set)
    {
        if (selected_button != null) selected_button.SetSelectedToy(false);
        if (button != null) button.SetSelectedToy(set);
        if (set)
        {
            selected_button = button;
            Monitor.Instance.InitMainSignal(button.label.runetype, button.label.toytype);

            selected_button_image.gameObject.SetActive(true);
            selected_button_image.SetParent(selected_button.transform);
            RectTransform set_me_to = selected_button.my_button.image.GetComponent<RectTransform>();
            

            
            selected_button_image.anchoredPosition = set_me_to.anchoredPosition;

            selected_button_image.localScale = set_me_to.localScale;            
            setSelectedIslandImage(button.label.runetype, button.label.toytype);

            Noisemaker.Instance.Click(ClickType.Success);
        }
        else
        {
            selected_button_image.gameObject.SetActive(false);
            setSelectedIslandImage(RuneType.Null, ToyType.Null);            

        }
    }

    void ShowNoResourcesPopup(Vector3 pos)
    {
        GameObject popup = Peripheral.Instance.zoo.getObject("GUI/no_resources_popup", false);
        RectTransform rt = popup.GetComponent<RectTransform>();
                
        popup.GetComponent<Floaty>().Init(pos);
        
    }

    public void setSelectedIslandImage(RuneType runeType, ToyType toyType)
    {
        selected_island_image.sprite = TowerStore.getPreviewSprite(runeType, toyType);

        if (selected_island == null) return;

        Vector3 set_to = selected_island.transform.position;
        //if (runeType != RuneType.Null && toyType != ToyType.Null) set_to.y += 0.17f;            
        
        selected_island_image.GetComponent<RectTransform>().position = set_to;
    }

    public void UpdatePanel(Island_Button button)
    {
        
        if (button == null && selected_island == null)
        {
            return;
        }
        if (button != null && selected_island != null && button.gameObject.GetInstanceID() == selected_island.gameObject.GetInstanceID()) return;

        if (button == null)
        {
            selected_island = null;
            selected_island_image.gameObject.SetActive(false);
            my_panel.gameObject.SetActive(false);
            return;
        }


        //if (selected_island != null && button.gameObject.GetInstanceID() != selected_island.gameObject.GetInstanceID()) ResetSelected();

        selected_island = button;

        bool temp_ok = false;
        temp_ok = (selected_island.island_type == IslandType.Permanent || (selected_island.island_type == IslandType.Temporary && Monitor.Instance.SetAllowedRange("sensible_city")));
        
        
        int ok_buttons = 0;
        EagleEyes.Instance.UpdateToyButtons("blah", ToyType.Normal, true);

        foreach (MyLabel label in my_panel.list)
        {
            if (!label.AmIActive())
            {
                label.SetHidden(true);
                continue;
            }    
            
                    
            unitStats stats = Central.Instance.getToy(label.runetype, label.toytype);
            if (stats != null)
            {                 
                if (stats.island_type != IslandType.Either && stats.island_type != selected_island.island_type)
                {
                    label.SetHidden(true);
                    label.ShowButtons(false);
                    continue;
                }
                if (stats.island_type == IslandType.Temporary)
                {
                    if (!temp_ok)
                    {
                        label.SetHidden(true);
                        label.ShowButtons(false);
                        continue;
                    }
                    string ok = selected_island.verify_toy_for_distance(label.content);
                    if (ok.Equals("TOOFAR"))
                    {
                        label.SetHidden(true);
                        label.ShowButtons(false);
                        continue;
                    }
                }

                //if (label.button.IsInteractable()){
                    label.SetHidden(false);
                    label.ShowButtons(true);
                    ok_buttons++;
                    /*
                }else{                    
                    label.SetHidden(true);
                    label.ShowButtons(false);
                }*/
            }            
        }
        Vector3 set_to = button.transform.position;
        if (ok_buttons > 0)
        {
            
            my_panel.transform.position = set_to;
            my_panel.gameObject.SetActive(true);

            my_panel.radius = (ok_buttons <= 6) ? 1.2f :
                              (ok_buttons == 7) ? 1.35f :
                              (ok_buttons == 8) ? 1.5f : 1.6f;
            my_panel.spacing = 2f * Mathf.PI * my_panel.radius / ok_buttons;
            my_panel.UpdatePanel();
           // if (Monitor.Instance != null) Monitor.Instance.my_spyglass.PointSpyglass(button.transform.position,.20f);

            selected_island_image.gameObject.SetActive(true);            
            selected_island_image.transform.position = set_to;
        }
        else
        {
            selected_island_image.gameObject.SetActive(false);
            Noisemaker.Instance.Play("island_too_far");
            ShowNoResourcesPopup(set_to);
        }

    }
   
}