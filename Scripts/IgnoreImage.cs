using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IgnoreImage : MonoBehaviour, ICanvasRaycastFilter
{
	public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
	{
		return false;
	}
}