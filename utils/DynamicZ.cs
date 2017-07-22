using UnityEngine;
using System.Collections;


public class DynamicZ : StationaryZ
{

    public bool above_it_all; //for planes and stuff

    void Update()
    {
        float offset = (above_it_all) ? 0.5f : 0f;
        SetZ(offset);
        
    }

}
