using System;
using UnityEngine;
using System.Collections.Generic;


//Precise = Laser, Focus, Swarm

//Diffuse = Diffuse, RF, Weaken
[Serializable]
public class TowerSpriteList
{
	public RuneType runeType;
	public ToyType toyType;
	
	public List<TowerSprite> regular_list;
	public List<TowerSprite> upgrade_list;
}

public class TowerVisualStore : MonoBehaviour
{

	public List<TowerSpriteList> sprites;	

	public static TowerVisualStore Instance { get; private set; }

	public TowerSprite getSprite(RuneType runeType, ToyType toyType, TowerSpriteType type, bool upgrade)
	{
		return getSprite(getList(runeType, toyType), upgrade, type);
	}

	TowerSpriteList getList(RuneType runeType, ToyType toyType)
	{
		foreach (TowerSpriteList list in sprites)
		{
			if (list.runeType == runeType && list.toyType == toyType) return list;
		}
		Debug.Log($"!!! Could not find a tower sprite list for {runeType} {toyType}\n");
		return null;
	}

	TowerSprite getSprite(TowerSpriteList list, bool upgrade, TowerSpriteType type)
	{
		if (list == null) return null;

		if (!upgrade) foreach (TowerSprite t in list.regular_list) if (t.type == type) return t;
		if (upgrade) foreach (TowerSprite t in list.upgrade_list) if (t.type == type) return t;


		
			
		
		Debug.Log(
			$"!!! Could not find a tower sprite for {list.runeType} {list.toyType} upgrade = {upgrade} type = {type}\n");

		if (type != TowerSpriteType.Basic && type != TowerSpriteType.Null)
			return getSprite(list, upgrade, TowerSpriteType.Basic);
		
		return null;
	}

	void Awake()
	{
	if(Instance != null && Instance != this)
		{
			//	Debug.Log("Remaking Central\n");
			Destroy(gameObject);
		}
		Instance = this;
		DontDestroyOnLoad (gameObject);
       
	}
	
}