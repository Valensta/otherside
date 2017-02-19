using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[System.Serializable]
public class Level {

	public string name;
	public int number;
	public Button button;
	public GameObject description;
	public float difficulty;
    public bool test_mode;
    public bool fancy;


    public void DisableMe()
    {
        button.gameObject.SetActive(false);
    }

}
