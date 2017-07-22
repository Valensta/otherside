using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class Regenerate_SpecialSkill : Interactable
{
    private Toy castle;
    private Vector3 castle_position;
    private float rate;
    private float frequency;

    private string visual = "GUI/regenerator_popup";
    private Floaty my_visual;
    public bool isActive()
    {
        return am_active;
    }

    void Start()
    {

        Deactivate();
        
    }

    public override void Reset()
    {
        StopMe();
    }

    void StopMe()
    {
        am_active = false;
        StopAllCoroutines();
    }
    public override void Deactivate()
    {
        Debug.Log("Deactivating Renew\n");
        //StopAllCoroutines();
        //am_active = false;

    }

    public override void Activate(StatBit skill)
    {
        castle = Peripheral.Instance.castle;
        castle_position = castle.transform.position;

        float[] _stats = skill.getStats();
        rate = _stats[0];
        frequency = _stats[1];
        am_active = true;

        StopAllCoroutines();
        StartCoroutine(Fire());
    }

    void ShowVisual()
    {
        if (my_visual == null)
        {
            GameObject popup = Peripheral.Instance.zoo.getObject(visual, true);
            //RectTransform rt = popup.GetComponent<RectTransform>();
            my_visual = popup.GetComponent<Floaty>();
        }

        my_visual.Init(castle_position);

    }

    private IEnumerator Fire()
    {

        while (Peripheral.Instance.GetHealth() < Peripheral.Instance.MaxHealth)
        {
            ShowVisual();
            Peripheral.Instance.AdjustHealth(rate, false);
            yield return new WaitForSeconds(frequency);
        }
        StopMe();
    }

  

    public override void Simulate(List<Vector2> positions)
    {
    
    }
}