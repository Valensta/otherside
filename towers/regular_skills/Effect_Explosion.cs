using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Effect_Explosion : MonoBehaviour {
	public EffectType type;
	public float strength;
	public CircleCollider2D my_collider;
	public float duration = 0;
	public Arrow arrow;
	float blip = 0.1f;
	public float radius = 0;
//	float upwards = 0;
//	ForceMode mode;
	Vector3 origin = Vector3.zero;
	float original_mass = -1;
	public string sparks = "";
	MyArray<HitMe> monsters = null;
	Vector3 from;
	public void Explode(){
		
		if (gameObject.activeSelf && arrow.gameObject.activeSelf)	StartCoroutine (Boom ());
	}

	public void setOrigin(Vector3 o){
		origin = o;
	}

	public void DisableMe(){
		StopAllCoroutines();
		this.gameObject.SetActive(false);
		if (original_mass > 0){arrow.mass = original_mass;}
	}


	public void InitMe(float[] stats){		
		from = arrow.myFirearm.gameObject.transform.position;
        strength = stats[0];
        radius = stats[1];
        monsters = null;
	}

	IEnumerator Boom(){
    
        if (duration == 0) {yield return null;}

		if (type == EffectType.Force) {
            //my_collider.enabled = true;		
			while (duration > 0) {
				duration = - blip;
				yield return new WaitForSeconds (blip);
			}
            GetVictims();
		}
		DisableMe();
		arrow.MakeMeDie(true);
	}
	
	void GetVictims(){
//	Debug.Log("Getting victims\n");
		if (monsters == null){monsters = Peripheral.Instance.targets;}
		
		List<HitMe> targets = new List<HitMe>();
        for (int i = 0; i < monsters.max_count; i++)
        {
            HitMe enemy = monsters.array[i];
            if (enemy == null || enemy.amDying() || !enemy.gameObject.activeSelf) continue;

            if (Vector2.Distance(enemy.transform.position, this.transform.position) < radius)            
                targets.Add(enemy);            

        }			
		
		if (targets.Count == 0) return;
		
		StatSum type = null;
        
        type = arrow.myFirearm.toy.rune.GetStats (false);
        type.factor = 0.9f;
        StatSum explode_statsum = type.getSubStatSum(EffectType.Explode_Force);
        

       
		
		for(int i = 0; i < targets.Count; i++){
			arrow.myFirearm.addXp(targets[i].HurtMe(explode_statsum, null, EffectType.Null), true);		
			float mass = targets[i].my_rigidbody.mass;
            //Vector3 dir3 = targets[i].transform.position - from;
            Vector3 dir3 = this.transform.position - from;
            Vector2 dir = Vector3.Normalize(new Vector2(dir3.x, dir3.y));
            //float dist = Vector2.Distance(targets[i].transform.position, from);

            float adjust = targets[i].my_ai.speed;
			targets[i].my_rigidbody.AddForce(adjust*strength*dir*mass, ForceMode2D.Impulse);
		}
	
	}
	

}
