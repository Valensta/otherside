using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class Stat_Change_Floaty : Floaty {
    public enum Numeric_Type { Positive, Negative, All };

    public Text text;
    
    public Numeric_Type numeric_type;
    

   
    Color positive_color = new Color(141f / 255f, 221f / 255f, 19f / 255f);
    Color negative_color = new Color(1f, 186f / 255f, 156f / 255f);


    public void Init(Vector3 pos, float num)
    {


        // Debug.Log("Initializing floaty " + pos + " " + num + "\n");
        if (numeric_type == Numeric_Type.Positive && num < 0
                        ) { text.text = ""; TIME = lifespan + 1f; return; }
        if (numeric_type == Numeric_Type.Negative && num > 0
                        ) { text.text = ""; TIME = lifespan + 1f; return; }

        TIME = 0f;
        if(numeric_type == Numeric_Type.All)
        {
            if (num > 0) text.color = positive_color; else text.color = negative_color;
        }
        num = Mathf.Round(num * 10f) / 10f;

       if (num > 0) text.text = "+" + num.ToString(); else if (num < 0) text.text = num.ToString(); else
       {
            text.text = "";
            TIME = lifespan + 1f;
            Debug.Log("Returning floaty\n");
            _returnMe();
            return;
       }
        base.Init(pos);

    }



    public void Init(Vector3 pos, string t)
    {
        
        text.text = t;
    }


}
