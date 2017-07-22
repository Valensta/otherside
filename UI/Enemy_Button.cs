using System;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEditor;
using UnityEngine.EventSystems;


public class Enemy_Button : MonoBehaviour, IPointerClickHandler
{
    public HitMe my_hitme;

    
    public delegate void OnSelectedHandler(SelectedType type, string n);
    public static event OnSelectedHandler onSelected;
     

    public void OnPointerClick(PointerEventData eventData)
    {
    //   Debug.Log("Enemy button on click\n");
        if (onSelected != null) onSelected(SelectedType.Enemy, my_hitme.stats.name);

        foreach (SpecialSkill skill in Peripheral.Instance.my_skillmaster.skills)
        {
            if (skill.type == EffectType.Architect || skill.type == EffectType.Renew) continue;
            
            if (skill.my_interactable.am_active)
            {
                Debug.LogError("Registered enemy button click while a skill is active\n");
            }
        }
    }
}

