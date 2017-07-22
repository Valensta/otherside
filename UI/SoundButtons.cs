using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class SoundButtons : MonoBehaviour {

    public Button play;
    public Button mute;



    public void OnEnable()
    {
        updateButtons();
    }

    public void updateButtons()
    {
        if (Noisemaker.Instance != null && Noisemaker.Instance.isMute())
        {
            mute.gameObject.SetActive(false);
            play.gameObject.SetActive(true);
        }
        else
        {
            mute.gameObject.SetActive(true);
            play.gameObject.SetActive(false);
        }
    }

}