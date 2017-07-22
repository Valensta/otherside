using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class Laser : MonoBehaviour {
	

	public float damage_frequency; //how frequently to calculate damage
	public float ammo_frequency; //how long counts as 1 ammo
	public GameObject myTarget;
    float TIME;
	float next_ammo_time;
	float next_damage_time; 
	public Firearm firearm = null;
//	bool halo_active = false;
	Body targetBody = null;
	public StatSum type;
    float laser_z = 3.25f;
	StatSum statsum;
//	private float range;
	LineRenderer _line_renderer;
	float redraw_frequency = 0.02f;


	public void initStats(Firearm _firearm){
       // setFirearm(_firearm);
		statsum = _firearm.toy.rune.GetStats(false);
        statsum.towerID = this.gameObject.GetInstanceID();
		//float strength = statsum.GetStatBit(EffectType.Laser).stat;
	//	Debug.Log("Initializing laser with strength " + strength + "\n");
	//	float times = 6;

        
		ammo_frequency = statsum.getReloadTime(false);
        damage_frequency = Get.laser_damage_frequency;

        statsum.factor = Get.laser_damage_factor;
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
        ResetLaser();
    }


    void ResetLaser()
    {
        if (_line_renderer == null) return;
	    Vector3 start = firearm.arrow_origin.position;
        start.z = laser_z;
        _line_renderer.SetPosition(0, start);
        _line_renderer.SetPosition(1, start);
        Noisemaker.Instance.Stop("laser");
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
	
		if (myTarget != null && target.gameObject.GetInstanceID() == myTarget.GetInstanceID())	return;

        //Debug.Log("Setting Target " + target.name + "\n");
        TIME = 0f;
        next_damage_time = 0f;
        next_ammo_time = 0f;
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
		
		TIME += Time.deltaTime;

		if (TIME >= next_ammo_time){
			firearm.UseAmmo();
            next_ammo_time += ammo_frequency;            
		}		
		
		if (TIME >= next_damage_time)
        {
			targetBody.DoTheThing(this.firearm, statsum);
            if (firearm.isSparkles) firearm.sparkles.AskSparkles(targetBody.my_hitme);
            next_damage_time += damage_frequency;			
		}		
		
			
	}

    private void OnDisable()
    {
        ResetLaser();
    }


    IEnumerator DrawLaser(){
    
        Vector3 end;
		while(myTarget != null){
			//Debug.Log("Drawing line " + myTarget.transform.position + "\n");
			
			end = myTarget.transform.position;
            end.z = laser_z;
            _line_renderer.SetPosition(1, end);
			yield return new WaitForSeconds(redraw_frequency);
		}
	    Vector3 draw_to = firearm.arrow_origin.position;
		_line_renderer.SetPosition(1, draw_to);
	 //   Debug.Log($"arrow origin {draw_to}\n");
		yield return null;
	}
	

}
