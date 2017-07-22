using System;
using UnityEngine;
using UnityEngine.UI;

public class MyToggle : MonoBehaviour {
    public int number;
    public string text;
    public bool currentValue;
    public Toggle myToggle;

    public ToggleAction toggleAction;

    public delegate void ToggleAction(int number, string text, bool currentValue);

    public void clear()
    {
        myToggle.isOn = false;
    }

    public void onValueChanged(bool newValue)
    {
        currentValue = newValue;

        if (toggleAction != null) toggleAction(number, text, currentValue);
    }
       
}