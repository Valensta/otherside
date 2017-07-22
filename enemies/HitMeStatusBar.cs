using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class HitMeStatusBar : MonoBehaviour {

	float max;
	public GameObject my_status_bar;

	public void Init(float m)
	{
		max = m;
	}

    public void UpdateStatus(float c)
    {
     //  Debug.Log(this.gameObject.GetInstanceID() + "Updating status to " + c + "/" + max + "\n");

        if (Mathf.Abs(max - c) < 0.05 && my_status_bar.activeSelf == true)
        {
            my_status_bar.SetActive(false);
         //   Debug.Log(this.gameObject.GetInstanceID() + " Turning off\n");
        }
        else if (Mathf.Abs(max - c) >= 0.05)
        {
            Vector3 scale = Vector3.one;
            scale.x = c / max;
            my_status_bar.SetActive(true);
            my_status_bar.transform.localScale = scale;
          //  Debug.Log(this.gameObject.GetInstanceID() + " Turning on " + scale + "\n");
        }
        

    }
	
}
