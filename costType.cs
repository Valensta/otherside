using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Cost
{
    public CostType type;

    private float amount;    

    public float Amount
    {
        get
        {
            return amount;
        }

        set
        {
     //       Debug.Log("Setting cost amount " + value + "\n");
            amount = value;
        }
    }

    public Cost(CostType _type, float _cost)
    {
        type = _type;
        Amount = _cost;
        if (!(type == CostType.Dreams || type == CostType.ScorePoint))
            Amount = Mathf.Round(Amount);
        
    }


    public Cost clone()
    {
        return new Cost(this.type, this.Amount);
    }

    public Cost() { }

    public bool isHero()
    {
        return (type == CostType.SensibleHeroPoint || type == CostType.AiryHeroPoint || type == CostType.VexingHeroPoint);
    }


    public RuneType getHeroRuneType()
    {
        switch (type)
        {
            case CostType.AiryHeroPoint:
                return RuneType.Airy;
            case CostType.SensibleHeroPoint:
                return RuneType.Sensible;
            case CostType.VexingHeroPoint:
                return RuneType.Vexing;
            default:
                return RuneType.Null;
        }
    }
}