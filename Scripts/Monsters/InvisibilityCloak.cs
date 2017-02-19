using UnityEngine;
using System.Collections;

public class InvisibilityCloak : Modifier {	
	public float period;
	public float interval;
	public SpriteRenderer my_sprite;
	public Collider2D my_collider;
	
	float TIME;
	
	
	void Start () {
		//if (interval*number > period/2f){
			//interval = period/(2f*number);
		//}
	}
	

	
	void OnEnable(){
		TIME = 0f;
	}
	
	// Update is called once per frame
	protected override void YesUpdate () {
		TIME += Time.deltaTime;
		
		if (TIME  > period){
			TIME = 0;
			StartCoroutine("MakeInvisible");
		}

		
	}

    protected override void SafeDisable()
    {
        StopAllCoroutines();
        _MakeVisible();
    }

    IEnumerator MakeInvisible()
    {
        my_collider.enabled = false;
        my_sprite.color = Color.gray;
        this.gameObject.tag = "Invisible";
        yield return new WaitForSeconds(interval);

        _MakeVisible();

    }
	

    private void _MakeVisible()
    {
        this.gameObject.tag = "Enemy";
        my_collider.enabled = true;
        my_sprite.color = Color.white;
    }
	
}
