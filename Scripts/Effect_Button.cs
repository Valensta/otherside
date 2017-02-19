using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;


public class Effect_Button : MonoBehaviour,IPointerClickHandler {
	public string content = "";
	public Wish stats;
	//public WishType wish_type;
	public float initStrength;
	float max_height;
	float delta = 0.9965f;
	
    public Transform parent;
	Vector3 position;
	Vector3 direction;
    float velocity;
    public Transform sprite;
    Peripheral my_peripheral;

	
    public void Init(WishType w, float _strength) {
        stats = new Wish(w, initStrength);
        stats.Strength = _strength;
    }
    
    void _InitPeripheral()
    {
        if (my_peripheral == null) my_peripheral = Peripheral.Instance;                
    }

	void OnEnable(){
        _InitPeripheral();

        position = parent.position;
		
		max_height = my_peripheral.tileSize * 50;
        velocity = 2f;           
		direction = new Vector2(0,0);
		if (transform.position.x > 0){
            direction.x = -x_direction();	
		}else{
            direction.x = x_direction();
		}
		if (transform.position.y > 0){
            direction.y = -y_direction() ;		
		}else{
            direction.y = y_direction();
		}

        sprite.transform.localScale = (0.3f + 0.2f*stats.getEffect())  * Vector3.one;
     //   Debug.Log("Setting wish size\n");
		
        _update_position();
		//StartCoroutine("SlowMe");
	}
	float x_direction()
    {
        return Random.RandomRange(.5f, 1f);
    }

    float y_direction()
    {
        return Random.RandomRange(0.2f, .5f);
    }

    void Update(){
        _InitPeripheral();

        if (this.transform.position.y  > max_height)
		{
			my_peripheral.zoo.returnObject(this.gameObject, true);
		}
        _update_position();
		
	}
	
	IEnumerator SlowMe()
	{		
		while (true){

            _update_position();
			yield return new WaitForSeconds(0.02f);
		}
//		yield return null;
	}

    void _update_position()
    {
        velocity = velocity * delta;
        position.x += velocity * Time.deltaTime * direction.x;
        position.y += velocity * Time.deltaTime * direction.y;
        parent.position = position;
    }

	void OnSelect(bool select){
		if (select) {
			//		Debug.Log ("EFFECT " + stats.type + " YAY");
						
			
		}
		
	}
	
	public void OnPointerClick(PointerEventData eventData){
		OnInput();
	}
	
	
	void OnInput(){
        _InitPeripheral();

        if (Noisemaker.Instance != null) Noisemaker.Instance.Play("collected_wish");
        //  Debug.Log("effectbutton adding wish " + stats.type + " " + stats.strength + "\n");
        my_peripheral.my_inventory.AddWish (stats.type, stats.Strength, 1);
        my_peripheral.zoo.returnObject (parent.gameObject,true);

	}

}
