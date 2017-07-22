using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;


public static class Show {
    //	static int iTweenNumber = 0;
    public static float IslandRegularAlpha = 175f / 255f;
    public static float IslandHighlightedAlpha = 1f;

    public static string ToPercent(float f)
    {
        return (f * 100) + "%";
    }


    public static String FixText(String text, String[] input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            text = Regex.Replace(text, "<" + i + ">", input[i]);
        }
        return text;
    }

	
    public static void UIToWorld(RectTransform canvas, Vector3 world_obj, RectTransform ui_element )
    {

        //then you calculate the position of the UI element
        //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. 
        //Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.

        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(world_obj);
        ViewportPosition.y -= world_obj.y * 0.025f;
        Vector2 WorldObject_ScreenPosition = new Vector2(
        (-(ViewportPosition.x * canvas.sizeDelta.x) + (canvas.sizeDelta.x * 0.5f)),
        (-(ViewportPosition.y * canvas.sizeDelta.y) + (canvas.sizeDelta.y * 0.5f)));
        
     //   Debug.Log(world_obj + " ->  " + ViewportPosition + " -> " + -(ViewportPosition.y * canvas.sizeDelta.y) + " " + (canvas.sizeDelta.y * 0.5f));

        //now you can set the position of the ui element
        ui_element.anchoredPosition = WorldObject_ScreenPosition;
    }


	public static void SetAlpha(SpriteRenderer sprite, float alpha){        

        try
        {            
            Color new_color = sprite.color;
            new_color.a = alpha;
            sprite.color = new_color;
        }catch(Exception e)
        {
            Debug.LogError("Caught " + e.ToString() + " on " + sprite.gameObject.name + "\n");
        }
	}



    public static void SetAlpha(Image sprite, float alpha)
    {
        Color new_color = sprite.color;
        new_color.a = alpha;
        sprite.color = new_color;
    }


    public static bool checkPosition(Vector2 pos){        
		float y_border = 15f/2f;
		float x_border = 12f;
	   
        if (Mathf.Abs(pos.y) > y_border || Mathf.Abs(pos.x) > x_border)
        {
            return false;
        }
        return true;   
    }

    public static Vector2 fixPosition(Vector2 pos)
    {
        float inner_x_border = 0.5f;
        float inner_y_border = -1.5f;
        float outer_x_border = 0.5f;        
        float outer_y_border = -1.5f;
        if (Mathf.Abs(pos.y) > (Camera.main.orthographicSize + outer_y_border))    
        {
            pos.y = (Camera.main.orthographicSize * pos.y / Mathf.Abs(pos.y) - inner_y_border);
        }

        if (Mathf.Abs(pos.x) > (Camera.main.orthographicSize * Screen.width / Screen.height + outer_x_border))
        {
            pos.x = ((Camera.main.orthographicSize * Screen.width / Screen.height ) * pos.x / Mathf.Abs(pos.x) - inner_x_border);        
        }
        return pos;
    }
	



	public static Color RuneColor(RuneType type){
    //    Debug.Log("something is still using runecolor\n");
        switch (type) {
		case RuneType.Sensible:		
			return new Color (155/255f, 93/255f, 1.0f, 1f);
		case RuneType.Airy:
			return new Color (0.0f, 1.0f, 1.0f, 1f);		
		case RuneType.Vexing:
			return new Color (163/255f, 1.0f, 23/2550f, 1f);
		default:
			return Color.white;
		}
	}

	public static Color EffectColor(EffectType type){
        Debug.Log("something is still using effectcolor\n");
		switch (type) {
		case EffectType.Force:		
			return new Color (1.0f, 0.0f, 0.0f, 0.5f);		
		case EffectType.Speed:
			return new Color (0.0f, 1.0f, 1.0f, 0.5f);
		case EffectType.Teleport:
			return new Color (1.0f, 1.0f, 1.0f, 0.5f);
		case EffectType.Stun:
			return new Color (1.0f, 181f/255f, 54f/255f, 0.5f);
		default:
			return Color.white;
		}
	}
	
	

	
	
}

