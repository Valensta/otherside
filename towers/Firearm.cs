using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Firearm))]

 
public class FirearmEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Firearm myTarget = (Firearm)target;

        GUILayout.Label("SENSIBLE   -    DIFFUSE");
        if (GUILayout.Button("Upgrade Diffuse"))
        {
            ((Firearm)target).toy.rune.Upgrade(EffectType.Diffuse, false, true);
            ((Firearm)target).CheckForUpgrades();
        }


        if (GUILayout.Button("Upgrade Transform"))
            ((Firearm)target).toy.rune.Upgrade(EffectType.Transform, false, true);


        GUILayout.Label("SENSIBLE   -   LASER");

        if (GUILayout.Button("Upgrade Laser"))
        {
            ((Firearm)target).toy.rune.Upgrade(EffectType.Laser, false, true);
            ((Firearm)target).CheckForUpgrades();
        }

        if (GUILayout.Button("Upgrade Sparkles"))
        {
            ((Firearm)target).toy.rune.Upgrade(EffectType.Sparkles, false, true);
            ((Firearm)target).CheckForUpgrades();
        }
    

        //
        GUILayout.Label("AIRY    -    AGGRESSIVE");
        if (GUILayout.Button("Upgrade Calamity"))
            ((Firearm)target).toy.rune.Upgrade(EffectType.Calamity, false, true);


        if (GUILayout.Button("Upgrade Swarm"))
            ((Firearm)target).toy.rune.Upgrade(EffectType.Swarm, false, true);
        GUILayout.Label("AIRY    -    PASSIVE");

        if (GUILayout.Button("Upgrade Weaken"))
            ((Firearm)target).toy.rune.Upgrade(EffectType.Weaken, false, true);
        /*
        if (GUILayout.Button("Upgrade Wishful"))
            ((Firearm)target).toy.rune.Upgrade(EffectType.WishCatcher, false, true);
            */
        if (GUILayout.Button("Upgrade Foil"))
            ((Firearm)target).toy.rune.Upgrade(EffectType.Foil, false, true);
        //


        GUILayout.Label("VEXING    -    RF");
        if (GUILayout.Button("Upgrade RapidFire"))
        {
            ((Firearm)target).toy.rune.Upgrade(EffectType.RapidFire, false, true);
            ((Firearm)target).CheckForUpgrades();
        }


        if (GUILayout.Button("Upgrade DOT"))
            ((Firearm)target).toy.rune.Upgrade(EffectType.DOT, false, true);


        GUILayout.Label("VEXING    -    FOCUS");

        if (GUILayout.Button("Upgrade Focus"))
            ((Firearm)target).toy.rune.Upgrade(EffectType.Focus, false, true);

        if (GUILayout.Button("Upgrade Fear"))
            ((Firearm)target).toy.rune.Upgrade(EffectType.Fear, false, true);

        if (GUILayout.Button("Upgrade Critical"))
            ((Firearm)target).toy.rune.Upgrade(EffectType.Critical, false, true);

        
        GUILayout.Label("What");

        if (GUILayout.Button("Upgrade AirAttack"))
            ((Firearm)target).toy.rune.Upgrade(EffectType.AirAttack, false, true);

        if (GUILayout.Button("Upgrade Meteor"))
            ((Firearm)target).toy.rune.Upgrade(EffectType.Meteor, false, true);


    }

}
#endif
public class Firearm : MonoBehaviour {

    public Transform arrow_origin;
    public bool ammo_by_time;
    float ammo_timer;
    public float ammo_per_shot = 1;
    bool start_ammo_by_time;
    public ArrowType default_arrow_type;
    public ArrowName current_arrow_name;
	public float max_ammo = 20;
	public float firePauseTime;
	private float nextMoveTime;
	private float nextFireTime;
    float previousFireTime;
    private Quaternion desiredRotation;
	private Quaternion turn;
	private float errorAmount = 0.1f;
	private float aimError = 0.1f;
	public Transform myTarget = null;
    public Vector2 myPreviousTarget = Vector2.zero;
    public Vector2 myTargetPosition = Vector2.zero;
    
    float ammo = -1;
	Vector2 myAngle = new Vector2 (0, -1);
	public float turnSpeed = 0.1f; //(between 0 and 1, should be angle/360) this is only for turning the tower, not for aiming the arrows unfortunately
    public float ghost_timer;
    public int restricted_path = -1; //FOR TESTING ONLY
    public AreaEffector area_effector;
    Transform monsters_transform;
	int last_target;
	public bool orient_me = true;
	RapidFire rapid_fire;
    float current_range;
    private bool active;

    float lastFireTime;
    public Toy toy;
    
	EnemyList enemy_list;
	public Mini_Toy_Button_Driver ammo_panel;
    	
    //THIS IS AWEFUL
	public bool isLaser = false;
    public bool isCritical = false;
    public bool isSparkles = false;

	public Laser laser;
    public Critical critical;
    public Sparkles sparkles;
    public float reload_time = 0f;

	Material laser_material;
    float distince_bonus;

	string arrow_name;
	public int arrow_count = 0;

    int current_shot = 0;

    public bool Active
    {
        get
        {
            return active;
        }

        set
        {
            active = value;
         //   Debug.Log(this.gameObject.name + " Setting firearm active " + value.ToString().ToUpper() + " \n");
        }
    }

    public bool isTemporary(){
		return (ammo > -1);
	}

    
    public void onTimeBasedXpAssigned(float xp)
    {
        
        addXp(xp, false);
    }
    


    public float addXp(float xp, bool damage_based){
        //if (toy_type == ToyType.Temporary) return;
        float return_xp = 0f;
        
    //    Debug.Log($"Adding XP {xp} damage based: {damage_based}\n");
        //xp = (damage_based)? xp * 0.1f : xp;
        if (toy.parent_toy != null)
        {            
            return_xp = toy.parent_toy.rune.addXp(xp * Peripheral.Instance.XpFactor(), damage_based);
            //toy.my_tower_stats.Xp = toy.parent_toy.rune.getXp();
            toy.parent_toy.rune_buttons.UpdateMe();            
        }
        else
        {
            return_xp = toy.rune.addXp(xp * Peripheral.Instance.XpFactor(), damage_based);
            //toy.my_tower_stats.Xp = toy.rune.getXp();
            if (toy.rune_buttons != null) toy.rune_buttons.UpdateMe(); //else { Debug.Log("Want to update rune_buttons but they are null for " + name + "\n"); }
        }
        return return_xp;
    }

	public void SetDistanceBonus(float bonus){
        Debug.Log($"Setting distance bonus {bonus}\n");
	    
        toy.rune.distance_bonus = bonus;
	}




    public void initStats(Toy toy)
    {
        if (toy.runetype != RuneType.Castle && toy.runetype != RuneType.SensibleCity)
        {
            Moon.onTimeBasedXpAssigned += onTimeBasedXpAssigned;
            //Debug.Log($"{gameObject.nam}e} wants xp\n");
        }

        this.toy = toy;
        Active = false;
        start_ammo_by_time = false;
		arrow_count = 0;                
        last_target = 0;
        CheckForUpgrades();
        Rune.onUpgrade += onUpgrade;
        nextFireTime = 0f;
        monsters_transform = Peripheral.Instance.monsters_transform;	
        enemy_list = Peripheral.Instance.enemy_list;
        current_shot = 0;


    }

    public void InitAmmoPanel()
    {
        
        if (toy.toy_type != ToyType.Temporary) return;

        if (ammo_panel == null)
        {
            
            GameObject buttons = Peripheral.Instance.zoo.getObject("GUI/ammo_panel", false);
        //    Debug.Log($"Initializing ammo panel for ${this.gameObject.name} ${buttons.GetInstanceID()}\n");
            ammo_panel = buttons.GetComponent<Mini_Toy_Button_Driver>();                       
        }
        
        ammo_panel.gameObject.SetActive(true);
        ammo_panel.InitMiniDriver(this.toy);
    }

    

    public void InitAreaEffector(){			
		area_effector.initStats(this);
	}

	public Material GetLaserMaterial(){
		if (laser_material == null){
            UnityEngine.Debug.Log("Laser material not defined! Resources.Loading default material!\n");
			laser_material = (Material) Resources.Load("Materials/default_laser_material", typeof(Material));
		}
		
		return laser_material;
	}





    void DoTheThing()
    {
        if (Time.timeScale == 0) return;
        if (!toy.Active) { return; }
        if (!Active) { return; }

        if (myTarget && !EnemyOk(myTarget)) 
        {
            myTarget = null;
        }
        

       

        if (area_effector != null) return;
        if (myTarget && !myTarget.gameObject.activeSelf) myTarget = null;
        if ((!myTarget || !isLaser) && (toy.getTime() > nextFireTime || isLaser))        getEnemy();

        

        if (myTarget && myTarget.gameObject.activeSelf)
        {
            if (ammo_by_time && start_ammo_by_time)
            {
                ammo_timer += Time.deltaTime;
                if (ammo_timer >= ghost_timer)
                {
                    UseAmmo();
                    ammo_timer = 0f;
                }
            }

            if (toy.getTime() > nextFireTime && !isLaser)            
            {
                if (orient_me) toy.center.transform.rotation = turn;                
                FireProjectile();                
            }
            if (isLaser) FireLaser();
        }
    }


    void incrementNextFireTime()
    {
        //  lastFireTime = Duration.time;

        if (toy.rune.get(EffectType.RapidFire) > 0 && rapid_fire != null)
        {
            nextFireTime = toy.getTime() + (rapid_fire.GetTimeToNextArrow());
        }
        else
        {
            nextFireTime = toy.getTime() + reload_time;
            //nextFireTime = toy.getTime() + reload_time;
        }
    }

    void getTargetPosition()
    {
        if (myPreviousTarget == Vector2.zero)
        {
            myTargetPosition.x = myTarget.position.x;
            myTargetPosition.y = myTarget.position.y;
            myPreviousTarget = myTargetPosition;
        }
        else
        {
            float lerp_factor = (current_arrow_name.type == ArrowType.RapidFire) ? 0.35f : 5f; //rapidfire or slow
            myTargetPosition = Vector2.Lerp(myPreviousTarget, myTarget.position, lerp_factor * (toy.TIME - previousFireTime));
            myPreviousTarget = myTargetPosition;

            if (current_arrow_name.type == ArrowType.RapidFire)
            {
                Vector2 sideways = new Vector2(Random.RandomRange(0f, 0.65f), Random.RandomRange(0f, 0.65f));
                myTargetPosition += sideways;
            }
        }

        previousFireTime = toy.getTime();
    }


    void FireProjectile()
    {
        if (myTarget == null) { return; }

        incrementNextFireTime();

        arrow_count++;
        if (!ammo_by_time)
        {
            UseAmmo();
        }
        else start_ammo_by_time = true;

        Vector3 firefrom = transform.position;
        if (arrow_origin != null) firefrom = arrow_origin.position;

        StatSum statsum = toy.rune.GetStats(false);
        statsum.towerID = this.gameObject.GetInstanceID();
        float arrow_speed = -1;
        Arrow arrow = null;
        if (current_arrow_name.type == ArrowType.RapidFire || current_arrow_name.type == ArrowType.Slow)
        {
            getTargetPosition();
            arrow = Get.MakeArrow(current_arrow_name, firefrom, myTargetPosition, statsum, arrow_speed, this, true);
        }
        else
        {
            myTargetPosition.x = myTarget.position.x;
            myTargetPosition.y = myTarget.position.y;
            arrow = Get.MakeArrow(current_arrow_name, firefrom, myTarget, statsum, arrow_speed, this, true);//focus has to aim for an actual moving target, nobody else cares though
        }






        CalcAimPosition(myTarget.position);
        //modifiers on regular arrows
        if (current_arrow_name.type == ArrowType.RapidFire)
        {

            //StatBit force = statsum.GetStatBit(EffectType.VexingForce);
          //  force.updateStat(rapid_fire.GetMass());
            rapid_fire.modifyArrow(arrow);
        }


        toy.my_tower_stats.Shots_fired++;
        arrow.gameObject.SetActive(true);
        if (!EnemyInRange(myTarget)) myTarget = null;
        //    incrementNextFireTime(); should be first
    }



    void Update()
	{
        //if (current_shot >= ((toy.getTime() - (toy.getTime() % reload_time)) / reload_time)) return;
        
        DoTheThing();
	}

  

    public void CheckForUpgrades()
    {
        updateRange();
        reload_time = toy.rune.stat_sum.getReloadTime(false);
        CheckIfLaser();
        CheckIfSparklesSkill();
        CheckIfCriticalSkill();
        CheckIfRapidFireSkill();
        updateArrowType();


    }

    void updateArrowType()
    {
        if (toy.rune.runetype == RuneType.Sensible && toy.rune.get(EffectType.Diffuse) > 0)
            current_arrow_name = new ArrowName(ArrowType.Diffuse);        
        else if (toy.rune.runetype == RuneType.Vexing && toy.rune.get(EffectType.Focus) > 0)
            current_arrow_name = new ArrowName(ArrowType.Focus);
        else if (toy.rune.runetype == RuneType.Vexing && toy.rune.get(EffectType.RapidFire) > 0)
            current_arrow_name = new ArrowName(ArrowType.RapidFire);
        else if (toy.rune.runetype == RuneType.Vexing && toy.rune.get(EffectType.Critical) > 0)
            current_arrow_name = new ArrowName(ArrowType.Critical);
        else
            current_arrow_name = new ArrowName(default_arrow_type);
            

        return;
    }

    protected void onUpgrade(EffectType type, int ID)
    {
        if (ID == toy.rune.ID)
        {
            if (type == EffectType.Laser) CheckIfLaser();
            else if (type == EffectType.Range) updateRange();
            else if (type == EffectType.Sparkles) CheckIfSparklesSkill();
            else if (type == EffectType.Critical)
            {
                CheckIfCriticalSkill();
                updateArrowType();
            }
            else if (type == EffectType.Focus)
            {
                reload_time = toy.rune.stat_sum.getReloadTime(false);
                updateRange();
                updateArrowType();
            }
            else if (type == EffectType.Diffuse)
            {
                reload_time = toy.rune.stat_sum.getReloadTime(false);
                updateArrowType();
            }
            else if (type == EffectType.RapidFire)
            {
                CheckIfRapidFireSkill();
                updateRange();
                updateArrowType();
            }
        }
    }

    void CheckIfLaser()
    {
        if (toy.rune.runetype == RuneType.Sensible && toy.rune.getLevel(EffectType.Laser) > 0)
        {
            if (!isLaser) laser.initLaser();
            isLaser = true;
            laser.initStats(this);            
        }
        else isLaser = false;        
    }

    void CheckIfCriticalSkill()
    {
        if (toy.rune.runetype == RuneType.Vexing && toy.rune.getLevel(EffectType.Critical) > 0)
        {
            isCritical = true;
            critical.Init(toy.rune.getStatBit(EffectType.Critical).getStats(), toy.rune.get(EffectType.VexingForce));
        }
        else isCritical = false;        
    }

    void CheckIfSparklesSkill()
    {
        if (sparkles == null) return;
        if (toy.rune.runetype == RuneType.Sensible && toy.rune.getLevel(EffectType.Sparkles) > 0)
        {
            sparkles.initStats(toy.rune.getStatBit(EffectType.Sparkles), this.gameObject.GetInstanceID());
            isSparkles = true;
        }
        else isSparkles = false;
    }

    void CheckIfRapidFireSkill()
    {
        if (toy.rune.runetype == RuneType.Vexing && toy.rune.getLevel(EffectType.RapidFire) > 0)
        {
            InitRapidFire();
            StatBit rf = toy.rune.getStatBit(EffectType.RapidFire);
            rapid_fire.Init(rf.getStats(), toy.rune.get(EffectType.VexingForce), rf.level);

        }
    
    }

    void getEnemy()
    {
        if (Peripheral.Instance.monsters_transform.childCount == 0)
        {
            myTarget = null;
            return;
        }


        if (toy.rune.runetype == RuneType.Vexing && current_arrow_name.type != ArrowType.RapidFire)
            getRandomEnemy();
        else if (isLaser || current_arrow_name.type == ArrowType.RapidFire)
            //getOldestEnemy();
            getNewestEnemy();
        else
            getOldestEnemy();
        //getOldestEnemy();

    }

    void OnDisable()
    {
        active = false;
        CancelInvoke();
        myTarget = null;
        Rune.onUpgrade -= onUpgrade;
        if (toy.runetype != RuneType.Castle && toy.runetype != RuneType.SensibleCity)
        {
            Moon.onTimeBasedXpAssigned -= onTimeBasedXpAssigned;
        }

    }

	

    public void loadSnapshot(ToySaver saver)
    {
        ammo = (int)saver.ammo;
        //my_toy.rune = saver.rune;	
        //if (my_toy.rune_buttons != null) my_toy.rune_buttons.UpdateMe();
        InitAmmoPanel();
        //my_toy.rune.UpdateStats();

    }


	public void UseAmmo(){
        if (ammo == -1) return;

        ammo -= ammo_per_shot;

        if (ammo_panel != null) ammo_panel.SetAmmo(); else Debug.Log($"{gameObject.name} Ammo panel is empty!\n");

        if (ammo == 0)
        {
            active = false;
            if (toy.parent_toy != null) toy.parent_toy.RemoveTargetToy(toy);

            toy.Die(reload_time*.8f);
        }
    }


    public void setAmmo(int a)
    {
        ammo = a;
    }
    
	public float Ammo(){
		return ammo;		
	}
	
	public bool CanAddAmmo(int a){
	//	Debug.Log("Can add ammo? " + ammo + "\n");
		if (ammo >= max_ammo) {return false;}
		return Peripheral.Instance.HaveResource(new Cost(CostType.Wishes, a));
	}

    public void AddAmmo(int a)
    {
        AddAmmo(a, false);
    }

    public void AddAmmo(int a, bool force){

		if (ammo >= max_ammo) { Debug.Log("no good\n");return;}
		if (force || Peripheral.Instance.UseResource((new Cost(CostType.Wishes, a)), Vector3.zero)){
			ammo += a* toy.stats.ammo;
			if (ammo > max_ammo) ammo = max_ammo;
			if (ammo_panel != null){ammo_panel.SetAmmo();}

            Tracker.Log(PlayerEvent.AmmoRecharge, true,
                customAttributes: new Dictionary<string, string>() { { "attribute_1", toy.my_name } },
                customMetrics: new Dictionary<string, double>() { { "metric_2", toy.rune.order }, { "metric_1", Peripheral.Instance.my_inventory.GetWishCount(WishType.Sensible)} });

        }
        else{
        //    UnityEngine.Debug.Log("No currency to add ammo " + runetype + " a " + "\n");
			if (Noisemaker.Instance != null){ Noisemaker.Instance.Play("no_resource_to_add_ammo");}
		}
	}

	void FireLaser()
	{
		if (myTarget == null){ UnityEngine.Debug.Log("no target\n");return;}				
		if(laser == null) { UnityEngine.Debug.Log("no laser\n"); return; }

        laser.SetTarget(myTarget);
                
    }



    void InitRapidFire() {
        if (rapid_fire == null) { rapid_fire = this.gameObject.AddComponent<RapidFire>(); }
    } 



    void CalculateAimError ()
	{
	//	aimError = Random.Range (-errorAmount, errorAmount);
	aimError = 0f;
	}

	
    public float getCurrentRange()
    {
        return current_range;
    }

	public void updateRange(){        
        current_range = toy.rune.stat_sum.getRange();
        if (area_effector != null) area_effector.updateHaloSize();

	}

    bool EnemyInRange(Transform m)
    {       
        return (getDistance(m) < current_range);
    }
	
	void getOldestEnemy ()
	{
		Transform best = null;
		float best_age = -1;		
		Transform only_child = null;
		int in_range = 0;

        for (int i = 0; i < monsters_transform.childCount; i++) //(Transform m in monsters_transform)
        {
            Transform m = monsters_transform.GetChild(i);
            //   if (!(m.gameObject.tag == "Enemy" || m.gameObject.tag == "Decoy")) continue;
            if (!EnemyOk(m)) continue;
            float dist = getDistance (m);			
			float new_age = enemy_list.getID(m.GetInstanceID());
			
			if (dist > current_range){
				continue;
			}
			only_child = m;
			in_range ++;
			if (monsters_transform.childCount == 1){			
				best = m;				
				best_age = 1;
				break;
			}
			
			if (new_age > best_age && m.GetInstanceID() != last_target){
				best_age = new_age;
				best = m;				
			}							
		}
		
		if (in_range == 1 && best == null){
			best = only_child;
			best_age = 1f;
		}
		
		if (best_age != -1) {
           // incrementNextFireTime();
            myTarget = best;
			last_target = best.GetInstanceID();
		} else {
			myTarget = null;		
		}
		
		
	}


    
    
    void getNewestEnemy()
    {
        Transform best = null;
        float best_age = 999999;
        Transform only_child = null;
        int in_range = 0;

        for (int i = 0; i < monsters_transform.childCount; i++) //(Transform m in monsters_transform)
        {
            Transform m = monsters_transform.GetChild(i);
            //if (!(m.gameObject.CompareTag("Enemy") || m.gameObject.CompareTag("Decoy"))) continue;
            if (!EnemyOk(m)) continue;
            float dist = getDistance(m);            
            if (dist > current_range) continue;

            only_child = m;
            in_range++;
            if (monsters_transform.childCount == 1)
            {
                best = m;
                best_age = 1;
                break;
            }
            int id = m.GetInstanceID();
            float new_age = enemy_list.getID(id);
            if (new_age < best_age && id != last_target)
            {
                best_age = new_age;
                best = m;
            }
        }

        if (in_range == 1 && best == null)
        {
            best = only_child;
            best_age = 1f;
        }

        if (best_age != 999999)
        {
            myTarget = best;
            last_target = best.GetInstanceID();
        }
        else myTarget = null;

    }

    float getDistance(Transform monster)
    {        
        return Vector2.Distance((Vector2)(monster.position), (Vector2)this.transform.position);
    }

    void getRandomEnemy()
    {
        int total_count = monsters_transform.childCount;
        Transform[] ok = new Transform[total_count];

        int ok_i = 0;
        for (int i = 0; i < total_count; i++)
        {
            
            Transform me = monsters_transform.GetChild(i);
            if (!EnemyOk(me)) continue;
            if (getDistance(me) < current_range)
            {
                ok[ok_i] = me;
                ok_i++;
            }
        }

        if (ok_i == 0)
        {
            myTarget = null;
            return;
        }
        
        if (ok_i == 1) {
            myTarget = ok[0];
            last_target = myTarget.GetInstanceID();
            return;
        }
     //   Debug.Log(this.gameObject.name + " is checking " + ok_i + " targets\n");

        int num = 0;
        int tries_left = (ok_i+1) * 5;
        while (tries_left > 0)
        {            
            num = Mathf.FloorToInt(Random.Range(0, ok_i));
            if (num >= ok_i) num = ok_i - 1;
            tries_left--;
            int ID = ok[num].GetInstanceID();
            if (ID != last_target)
            {
                myTarget = ok[num];
                last_target = ID;            
                return;
            }
        }
        if (ok_i > 0) UnityEngine.Debug.Log("Could not find a random enemy, " + ok_i + " in range?!\n");
        myTarget = null;
    }

    bool EnemyOk(Transform m)
    {
        int layer = m.gameObject.layer;
        bool ok = (layer == Get.flyingMonsterLayer || layer == Get.regularMonsterLayer);
       
        return ok;
    }

    void getClosestEnemy ()
	{
		Transform best = null;
		float best_distance = Mathf.Infinity;		
		Transform only_child = null;
		int in_range = 0;
		
		foreach (Transform m in monsters_transform) {
            if (!EnemyOk(m)) continue;
			float new_dist = getDistance (m);			
			if (new_dist > current_range) continue;
			
			only_child = m;
			in_range ++;
			if (monsters_transform.childCount == 1){			
				best = m;				
				best_distance = 0;
				break;
			}
					
			if (new_dist < best_distance && m.GetInstanceID() != last_target){
				best_distance = new_dist;
				best = m;				
			}							
		}
		
		if (in_range == 1 && best == null){
			best = only_child;
			best_distance = 1f;
		}
		
		if (best_distance != Mathf.Infinity) {
          //  incrementNextFireTime();
            myTarget = best;
			last_target = best.GetInstanceID();
		} else {
			myTarget = null;		
		}
		
		
	}

    void CalcAimPosition(Vector3 targetPos)
    {
        Vector2 aimPoint = new Vector3(targetPos.x + aimError - transform.position.x,
                                        targetPos.y + aimError - transform.position.y);
        aimPoint = Vector2.ClampMagnitude(aimPoint, 1);
        
        Vector2 lerp = Vector2.Lerp (myAngle, aimPoint, turnSpeed);
        //Vector2 lerp = aimPoint;

        //Debug.Log("CalcAimPosition want to go to " + aimPoint + " previous angle " + myAngle + " going to " + lerp + "\n");

        myAngle = lerp;
        
        float angle = Vector2.Angle (new Vector2 (0, 1), lerp);
		
		
		if (Vector3.Cross(new Vector2(0, 1), lerp).y > 0) {
			angle = 360 - angle;
		}
		
		
		Quaternion turn2 = Quaternion.AngleAxis (angle, new Vector3 (0, 0, -1));
		

		turn = turn2;
	}
}
