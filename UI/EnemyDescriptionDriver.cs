using System;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEditor;
using UnityEngine.EventSystems;


public class EnemyDescriptionDriver : MonoBehaviour
{
    
    public GameObject panel;    
	public EnemyDescriptionLabel label;


    private void Start()
    {
        Enemy_Button.onSelected += onSelected;
        SpyGlass.onSelected += onSelected;
        Island_Button.onSelected += onSelected;
        Peripheral.onSelected += onSelected;
        MyButton.onSelected += onSelected;
        MyFastForwardButton.onSelected += onSelected;
        
    }

    void onSelected(SelectedType type, string n)
    {
        if (type != SelectedType.Enemy)
        {
            panel.SetActive(false);
        }
        else {
            panel.SetActive(true);
            label.setType(n);
        }
    }

    
}

