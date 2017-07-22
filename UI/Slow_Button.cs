using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class Slow_Button : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	public GameObject pop_up;
	public string content;// do we need this?
    public float timer; //hold for how long
    float current_time;
    bool am_pressed;
    

    public void DoTheThing()
    {
        Debug.Log("Doing the thing\n");
        pop_up.SetActive(true);
    }

    public void Update()
    {
        if (Time.timeScale == 0) return;
        if (!am_pressed) return;

        current_time += Time.deltaTime;
        if (current_time >= timer)
        {
            DoTheThing();
            am_pressed = false;
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        current_time = 0f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
     //   Debug.Log("Slow button on pointer down\n");
        if (pop_up.activeSelf) return;
        am_pressed = true;    
    }

    public void OnPointerUp(PointerEventData eventData)
    {
     //   Debug.Log("Slow button on pointer up\n");
        am_pressed = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        am_pressed = false;
        current_time = 0f;
    }
}
