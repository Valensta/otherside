using UnityEngine;
using UnityEngine.UI;


using System.Collections;

public class VersionDisplay: MonoBehaviour {
	public Text text;
	// Update is calle;d once per frame
	
	void Start () {        
        text.text = "Version: " + Application.version; 
        
    }
    
}
