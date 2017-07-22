using System;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEditor;
using UnityEngine.EventSystems;

public class MoveHeroHelper : MonoBehaviour,  IPointerDownHandler,IPointerUpHandler
{
    
	public Toy my_toy;
    bool am_pressed;
    float press_timer;
    float move_hero_when_timer = 1f;

    public void OnPointerDown(PointerEventData eventdata)
    {
        // bool drag_mode = EagleEyes.Instance.mobile_tower_scroll_driver.DragMode();
        if (my_toy != null && my_toy.toy_type == ToyType.Hero)
        {
            am_pressed = true;
            
        } 
       
    }

    public void OnPointerUp(PointerEventData eventdata)
    {
        press_timer = 0f;
    }
    private void Update()
    {
        if (am_pressed)
        {
            press_timer += Time.deltaTime;
            if (press_timer >= move_hero_when_timer)
            {
                Peripheral.Instance.sellToy(my_toy, my_toy.getSellCost());
                press_timer = 0f;
                am_pressed = false;
            }
        }
    }

}

