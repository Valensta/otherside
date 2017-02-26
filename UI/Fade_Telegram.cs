using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;


public class Fade_Telegram : MonoBehaviour
{
    public FadeMe from;
    public FadeMe to;
    public bool pause;


    public void OnClick()
    {

        Tracker.Log("Fade_Telegram " + this.gameObject.name);
        from.OnClick();
        to.FadeIn();
        if (pause)
        {
            Peripheral.Instance.Pause(true);
        }
    }

    public void OnClickDone()
    {
        Peripheral.Instance.Pause(false);
        to.OnClick();
    }
   
}