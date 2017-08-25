using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DynamicLoadingBackground : MonoBehaviour
{
	public static DynamicLoadingBackground Instance { get; private set; }
	public Image image;
	public Sprite desert;
	public Sprite forest;

	public void InitRandom()
	{
	
		image.sprite = (Central.Instance.current_lvl < 4) ? forest : desert;
	}

	void Awake()
	{


		if(Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		Instance = this;
		DontDestroyOnLoad (gameObject);
       
	}

}