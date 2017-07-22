using System;
using UnityEngine;
using UnityEngine.UI;

public class MyInputField : MonoBehaviour {
    public int number;
    public string text;
    public bool currentValue;
    

    public InputFieldAction inputFieldAction;

    public delegate void InputFieldAction(int number, string text, bool currentValue);

    public void onValueChanged(bool newValue)
    {
        currentValue = newValue;

        if (inputFieldAction != null) inputFieldAction(number, text, currentValue);
    }
       
}