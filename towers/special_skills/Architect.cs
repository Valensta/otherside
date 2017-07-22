using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class Architect : Interactable
{
    private Toy castle;    
  //  private bool am_active;
    private float percent;

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
        Central.Instance.base_toy_cost_mult = Get.getPercent(0);
        Central.Instance.updateCost(Peripheral.Instance.getToys());
        am_active = false;        
    }
    public override void Deactivate()
    {
        //Central.Instance.base_toy_cost_mult = Get.getPercent(0);
        //Central.Instance.updateCost(Peripheral.Instance.getToys());
        //Debug.Log("Deactivating Architect\n");        

    }

    public override void Activate(StatBit skill)
    {
        castle = Peripheral.Instance.castle;


        float[] _stats = skill.getStats();
        percent = _stats[0];
        
        am_active = true;

        Central.Instance.base_toy_cost_mult = Get.getPercent(percent);
        Central.Instance.updateCost(Peripheral.Instance.getToys());
        am_active = true;

    
    }
  

    public override void Simulate(List<Vector2> positions)
    {
    
    }
}