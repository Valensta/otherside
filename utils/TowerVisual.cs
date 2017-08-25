using System;
using System.CodeDom;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.UI;

//Precise = Laser, Focus, Swarm

//Diffuse = Diffuse, RF, Weaken

public enum TowerSpriteType{ Null, Basic, Precise, Diffuse, Precise2ndSkill, Diffuse2ndSkill}
[Serializable]
public class TowerSprite
{
	public TowerSpriteType type;
	public Sprite sprite;	
}

public class TowerVisual : MonoBehaviour
{
	public Toy myToy;
	private TowerSprite currentSprite;
	private TowerSprite currentUpgradeSprite;
	
	public bool haveUpgrades;



	void setSpriteType(TowerSpriteType type)
	{
		if (currentSprite != null && type == currentSprite.type) return;

		if (myToy.building.tower_sprite)
		{
			currentSprite = TowerVisualStore.Instance.getSprite(myToy.runetype, myToy.toy_type, type, false);
			if (currentSprite != null)
			{
			
				myToy.building.tower_sprite.sprite = currentSprite.sprite;
			
				
			}
		}

		if (!haveUpgrades || !myToy.building.upgrade_sprite) return;
		currentUpgradeSprite = TowerVisualStore.Instance.getSprite(myToy.runetype, myToy.toy_type, type, true);
		if (currentUpgradeSprite != null)
		{
			bool turn_me_on = myToy.building.upgrade_sprite.gameObject.activeSelf; 
			myToy.building.upgrade_sprite.gameObject.SetActive(false);
			myToy.building.upgrade_sprite.sprite = currentUpgradeSprite.sprite;
			
			myToy.building.upgrade_sprite.gameObject.SetActive(turn_me_on);
		}
	}

	public void setUpgrade(bool enable)
	{
		if (!haveUpgrades) return;
		if (!myToy.building.upgrade_sprite) return;
		if (myToy.building.upgrade_sprite.gameObject.activeSelf == enable) return;
//		Debug.Log($"Setting upgrade for {this.gameObject.transform.parent.gameObject.name} to {enable}\n");

		myToy.building.upgrade_sprite.gameObject.SetActive(enable);
	}


	public void setSprite(bool enable)
	{
		if (!myToy.building.tower_sprite) return;
		Show.SetAlpha(myToy.building.tower_sprite, enable? 1f : 0f);
	}

	
	
	TowerSprite getSprite(List<TowerSprite> list, TowerSpriteType type)
	{
		foreach (TowerSprite t in list) if (t.type == type) return t;
		return null;
	}
	public void updateVisuals()
	{
		switch (myToy.rune.runetype)
		{
			case RuneType.Airy:

				if (myToy.rune.getLevel(EffectType.Weaken) > 0)
				{
					setSpriteType(TowerSpriteType.Diffuse);
					return;
				}

				if (myToy.rune.getLevel(EffectType.Calamity) > 0)
				{
					setSpriteType(TowerSpriteType.Precise);
					return;
				}

				setSpriteType(TowerSpriteType.Basic);
				break;
			default:
				setSpriteType(TowerSpriteType.Basic);
				break;

		}
	}
	
	
	
}