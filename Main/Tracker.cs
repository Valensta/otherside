using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;


    
public static class Tracker {
    public static bool track = true;

	public static void Log(string what)
    {
        if (!track) return;

        Debug.LogError("------- USER ACTION: " + what + "\n");
    }


}
	





	