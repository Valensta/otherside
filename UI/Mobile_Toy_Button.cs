using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;


public class Mobile_Toy_Button : MyButton
{
    public Island_Floating_Button_Driver driver;
    public Toy toy_parent; //firearm assigned here. buttons talk directly to the firearm	

    public void Awake()
    {
        content = TowerStore.getBasicName(label.runetype, label.toytype);
        label.content = content;
    }


    public override void InitMe()
    {
    
        Toy.onPriceUpdate += onPriceUpdate;
        EagleEyes.onPriceUpdate += onPriceUpdate;
        Central.onPriceUpdate += onPriceUpdate;
    
    }

    public void OnMyOwnClick()
    {

        if (EagleEyes.Instance.UIBlocked("Mobile_Toy_Button", content)) return;
        //    Debug.Log("on my own click already have " + driver.selected_button + " selecting " + content + "\n");
        if (driver.selected_button != null && driver.selected_button.content.Equals(content))
        {
         //   Debug.Log("placing selected " + Peripheral.Instance.getSelectedToy() + "\n");
            driver.selected_island.Selected_toy = content;
            driver.selected_island.DoAThing();
            Monitor.Instance.SetMainSignal(false);
            driver.ResetSelected();
            driver.UpdatePanel(null);
            driver.SetPanel(false);
        }
        else
        {
        //    Debug.Log("selected? " + selected + "\n");
            if (selected)
            {
                driver.SelectButton(this, false);
            }
            else
            {
                //       Debug.Log("Selecing " + content + "\n");
                driver.SelectButton(this, true);
            }
            
        }
    }

    public void onPriceUpdate(string name, float price)
    {

        //if (name == "sensible_tower")	Debug.Log("On price update " + name + " is? " + content + " or " + content_detail + " " + price + "\n");
        if (content == name || content_detail == name)
        {
            if (text == null)
            {
                //Debug.Log ("toy_selected button " + name + " does not have text assigned, cannot update price!\n");
                return;
            }
            //Debug.Log ("toy_selected button " + name  + " setting price to " + price + "\n");

            text.text = price.ToString();
        }

    }
}