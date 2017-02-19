using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class Arrow : MonoBehaviour {

	public float speed;
    public float initial_speed;
	public StatSum type;
	public string sparks = "sparks";
	public string finalsparks = "sparks";
	float dist;
	public float mass;
	public bool seek_target = false;
	public float range;
	GameObject explosion;
	public Transform myTarget;
    public Vector3 myStaticTarget = Vector3.zero;
    float init_mass = -1;
	public Firearm myFirearm = null;
	Vector3 origin;
	public int laser_length = 20;
	public bool explode;
	public Effect_Explosion effect_explosion;
	public GameObject sprite;
	public SpriteRenderer sprite_renderer;
	Rigidbody2D rb = null;
	Peripheral my_peripheral;
    public Diffuse diffuse;
    public int sourceID;

    public void InitArrow(StatSum statsum, Transform target, float _speed, Firearm _firearm)
    {
        myTarget = target;
        InitArrow(statsum, target.transform.position, _speed, _firearm);
    }
    public void InitArrow(StatSum statsum, Vector3 target, float _speed, Firearm _firearm){
        
		if (_speed > 0){ speed = _speed;}
		type = statsum;
		 if (rb == null) {rb = GetComponent<Rigidbody2D> ();}
         if (init_mass == -1) { init_mass = rb.mass; }

		if (statsum.runetype == RuneType.Sensible){     
			rb.mass = init_mass * type.getPrimary();

        }
        	
		myStaticTarget = target;		
		myFirearm = _firearm;
		if (my_peripheral == null) my_peripheral = Peripheral.Instance;
		Color c = sprite_renderer.color;
		c.a = 1f;
		sprite_renderer.color = c;

        if (diffuse != null) diffuse.Init(statsum);
	}
	
 	void OnEnable (){
        if (myTarget == null && myStaticTarget == Vector3.zero) {Debug.Log("Arrow has no target assigned!\n"); return; }
		if (effect_explosion!= null && effect_explosion.gameObject.activeSelf){
			effect_explosion.setOrigin(transform.position);
		//	effect_explosion.gameObject.SetActive(true);
		}
		
		origin = transform.position;

        Vector3 direction = getDirection();
		rb.isKinematic = true;
		rb.isKinematic = false;
        rb.velocity = Vector2.zero;
		rb.AddForce (100 * rb.mass * direction * speed);
     //   Debug.Log("Adding force " + 100 * rb.mass * direction * speed + "\n");
        
		sprite.transform.localPosition = Vector3.zero;
	
	}

    Vector3 getDirection()
    {
        Vector3 take_me_there = (myTarget) ? myTarget.transform.position : myStaticTarget;
        return -(this.transform.position - take_me_there).normalized;        
    }


	void Update () {
	
		if (Mathf.Abs (Vector3.Distance (transform.position, origin)) >= my_peripheral.tileSize * range) {
			//Debug.Log(gameObject.name + " Too far away");
			Explode ();
		}

		if (rb.velocity.magnitude > 0 && rb.velocity.magnitude < speed*0.7f){	
			//Debug.Log(gameObject.name + " Too slow @ " + rb.velocity.magnitude + " compared to " + speed*0.7f +  "\n");
			Explode ();
		}
			
	
        if (seek_target)
        {            
            rb.AddForce(rb.mass * getDirection() * speed * 50f);           
        }
	}

	void Sparks(string s, Vector3 pos){
		if (s == null) return;
	
		explosion = my_peripheral.zoo.getObject ("Arrows/" + s, true);
		explosion.transform.position = pos;
	}



    void Explode()
    {

        Sparks(finalsparks, sprite.transform.position);
        my_peripheral.zoo.returnObject(this.gameObject);
    }  

    public void RegisterHit(Vector3 pos)
    {
        if (myFirearm != null) myFirearm.my_tower_stats.Hits++;
        Sparks(sparks, pos);
        if (effect_explosion != null && effect_explosion.gameObject.activeSelf)
        {
            effect_explosion.Explode();
        }

        if (diffuse != null)
        {
            diffuse.MakeDiffuse(pos);        
        }else
        {
            StartCoroutine("Die");
        }

    }

    public void MakeMeDie(){
		StartCoroutine ("Die");
	}

	IEnumerator Die(){
        	
		LeanTween.alpha(sprite, 0, 0.01f);
		yield return new WaitForSeconds(.1f);
		Explode ();
		yield return null;
	}

}

