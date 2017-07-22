using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;

public abstract class PanelObject : MonoBehaviour
{
    public GenericPanel my_panel;
    public bool do_not_display;

    public void SetPanel(GenericPanel panel)
    {
        my_panel = panel;
    }

    public abstract void InitButton();
}

