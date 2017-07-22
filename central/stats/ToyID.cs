using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[System.Serializable]
public class ToyID : IDeepCloneable<ToyID>, IComparable
{
    public RuneType rune_type;
    public ToyType toy_type;

    public ToyID() { }

    public bool isNull()
    {
        return rune_type == RuneType.Null || toy_type == ToyType.Null;
    }

    public void setNull()
    {
        rune_type = RuneType.Null;
        toy_type = ToyType.Null;
    }

    public ToyID(RuneType runetype, ToyType toytype)
    {
        this.rune_type = runetype;
        this.toy_type = toytype;
    }

    public string toString()
    {
        return (toy_type + "_" + rune_type);
    }

    public int CompareTo(RuneType runetype, ToyType toy_type)
    {
        if (runetype == this.rune_type && toy_type == this.toy_type) return 0;
        return 1;
    }

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;
        ToyID d = obj as ToyID;
        if (d.rune_type == this.rune_type && d.toy_type == this.toy_type) return 0;
        return 1;
    }

    object IDeepCloneable.DeepClone()
    {
        return this.DeepClone();
    }

    public ToyID DeepClone()
    {
        ToyID my_clone = new ToyID();
        my_clone.rune_type = this.rune_type;
        my_clone.toy_type = this.toy_type;
        return my_clone;

    }
}