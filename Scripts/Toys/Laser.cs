using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class Laser : MonoBehaviour {
	

	public float damage_frequency; //how frequently to calculate damage
	public float ammo_frequency; //how long counts as 1 ammo
	public GameObject myTarget;
	
	float AMMO_TIME;
	float DAMAGE_TIME; 
	public Firearm firearm = null;
	bool halo_active = false;
	Body targetBody = null;
	public StatSum type;
	
	StatSum statsum;
//	private float range;
	LineRenderer _line_renderer;
	float redraw_frequency = 0.02f;


	public void initStats(Firearm _firearm){
       // setFirearm(_firearm);
		statsum = _firearm.rune.GetStats(false);
        statsum.towerID = this.gameObject.GetInstanceID();
		//float strength = statsum.GetStatBit(EffectType.Laser).stat;
	//	Debug.Log("Initializing laser with strength " + strength + "\n");
		float times = 6;

        
		ammo_frequency = statsum.getReloadTime(false);
        damage_frequency = ammo_frequency/times;

		statsum.factor = 1f/(times);
        initLaser();
    }


	
    //before firing, to set the line renderer
    public void initLaser()
    {
        if (_line_renderer == null)
        {
            _line_renderer = gameObject.AddComponent<LineRenderer>() as LineRenderer;
            _line_renderer.material = firearm.GetLaserMaterial();
            //	Debug.Log("Line renderer stat " + this.transform.position + "\n");
        }
        Vector3 start = this.transform.position;
        start.z = 2f;
        _line_renderer.SetPosition(0, start);
        _line_renderer.SetPosition(1, start);
    }


    public void NullTarget(){
		myTarget = null;
		firearm.myTarget = null;
        Noisemaker.Instance.Stop("laser");
    }

	public void SetTarget(Transform target){
		if (target == null){
			NullTarget();
			return;
		}
	
		if (myTarget != null && target.name.Equals(myTarget.name)){
		//	Debug.Log("Target same " + target.name + " " + myTarget.name + "\n");
			return;
		}
		//Debug.Log("Setting Target " + target.name + "\n");
		myTarget = target.gameObject;
		targetBody = myTarget.GetComponent<Body>();
		StartCoroutine("DrawLaser");
        Noisemaker.Instance.Play("laser");
    }


	void Update () {
		if (myTarget == null){ return;}
		if (myTarget != null && !myTarget.gameObject.activeSelf){
			NullTarget();
			return;
		}
		if (Vector2.Distance(myTarget.transform.position, this.transform.position) > firearm.getCurrentRange()){
		//	Debug.Log("Laser out of range\n");
			NullTarget();
			return;
		}
		
		AMMO_TIME += Time.deltaTime;
		DAMAGE_TIME += Time.deltaTime;
		
			

		if (AMMO_TIME > ammo_frequency){
			firearm.UseAmmo();
			AMMO_TIME = 0;
		}		
		
		if (DAMAGE_TIME > damage_frequency){
			targetBody.DoTheThing(this.firearm, statsum);
            if (firearm.isSparkles) firearm.sparkles.AskSparkles(targetBody.my_hitme);
			DAMAGE_TIME = 0;
		}		
		
			
	}	
	


	IEnumerator DrawLaser(){
		
		Vector3 end;
		while(myTarget != null){
			//Debug.Log("Drawing line " + myTarget.transform.position + "\n");
			end = myTarget.transform.position;
			end.z = 1f;
			_line_renderer.SetPosition(1, end);
			yield return new WaitForSeconds(redraw_frequency);
		}
		_line_renderer.SetPosition(1, this.transform.position);
		yield return null;
	}
	

}
