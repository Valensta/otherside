using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class TimeOfDayImage{
	public TimeName name;
	public Image image;
    public LeanTweener tweener;

}
	

public class TimeOfDayIndicator: MonoBehaviour{
	public List<TimeOfDayImage> times;
	public TimeOfDayImage current_image;
	
	
	public void SetTime(TimeName n){
		for (int i = 0; i < times.Count; i++){
			if (times[i].name == n)
			{
                if (current_image.image != null && current_image.image.enabled)
                {
              //      current_image.tweener.StopMeNow();
                    current_image.image.enabled = false;
                    
                }
				
				current_image = times[i];
				current_image.image.enabled = true;
         //       current_image.tweener.Init();
			}
		}
	}
	
}


