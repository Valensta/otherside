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
        string effect;
        string toytype;
      

        if (GUILayout.Button("Upgrade RapidFire"))        
            ((Toy)target).rune.Upgrade(EffectType.RapidFire, false);


        if (GUILayout.Button("Upgrade (RF) Explode Force"))
            ((Toy)target).rune.Upgrade(EffectType.Explode_Force, false);


        if (GUILayout.Button("Upgrade Laser"))
            ((Toy)target).rune.Upgrade(EffectType.Laser, false);

        if (GUILayout.Button("Upgrade (Laser) DOT"))
            ((Toy)target).rune.Upgrade(EffectType.DOT, false);

        if (GUILayout.Button("Upgrade Calamity"))
            ((Toy)target).rune.Upgrade(EffectType.Calamity, false);


        if (GUILayout.Button("Upgrade (RF) Weaken"))
            ((Toy)target).rune.Upgrade(EffectType.Weaken, false);


        if (GUILayout.Button("Upgrade Wish Catcher"))
            ((Toy)target).rune.Upgrade(EffectType.WishCatcher, false);

        if (GUILayout.Button("Upgrade Swarm"))
            ((Toy)target).rune.Upgrade(EffectType.Swarm, false);

        if (GUILayout.Button("Upgrade AirAttack"))
            ((Toy)target).rune.Upgrade(EffectType.AirAttack, false);

    }

}
#endif
public class Firearm : Toy {
	public Transform arrow_origin;
    public bool ammo_by_time;
    float ammo_timer;
    bool start_ammo_by_time;
    public ArrowType arrow_type;
	public int max_ammo = 20;
	public float firePauseTime;
	private float nextMoveTime;
	private float nextFireTime;
	private Quaternion desiredRotation;
	private Quaternion turn;
	private float errorAmount = 0.1f;
	private float aimError = 0.1f;
	public Transform myTarget = null;
	int ammo = -1;
	Vector2 myAngle = new Vector2 (0, -1);
	public float turnSpeed = 0.10f; //(between 0 and 1, should be angle/360)
    float ghost_timer = 0.6f;
    public int restricted_path = -1; //FOR TESTING ONLY

	Transform monsters_transform;
	int last_target;
	public bool orient_me = true;
	RapidFire rapid_fire;
    float current_range;




	EnemyList enemy_list;
//	public BuildingID ammo_bonus_parent_id = new BuildingID();
	public Mini_Toy_Button_Driver ammo_panel;
	public bool areaEffect = false;
    //THIS IS AWEFUL
	public bool isLaser = false;
    public bool isCritical = false;
    public bool isSparkles = false;
	public AreaEffector area_effector;
	public Laser laser;
    public Critical critical;
    public Sparkles sparkles;
    public float reload_time = 0f;

	Material laser_material;
    float distince_bonus;

	string arrow_name;
	public int arrow_count = 0;



	public override bool isTemporary(){
		return (ammo > -1);
	}



    public float addXp(float xp){
        //if (toy_type == ToyType.Temporary) return;
        float return_xp = 0f;
        if (parent_toy != null)
        {            
            return_xp = parent_toy.rune.addXp(xp * Peripheral.Instance.XpFactor());
            if (GameStatCollector.Instance != null) my_tower_stats.Xp = parent_toy.rune.getXp();
            parent_toy.rune_buttons.UpdateMe();            
        }
        else
        {
            return_xp = rune.addXp(xp * Peripheral.Instance.XpFactor());
            if (GameStatCollector.Instance != null) my_tower_stats.Xp = rune.getXp();
            if (rune_buttons != null) rune_buttons.UpdateMe(); //else { Debug.Log("Want to update rune_buttons but they are null for " + name + "\n"); }
        }
        return return_xp;
    }

	public void SetDistanceBonus(float bonus){
        UnityEngine.Debug.Log("Setting distance bonus\n");
		rune.distance_bonus = bonus;
	}




    public override void initStats(unitStats s, Vector3 scaleV, Island_Button i, string _name, Rune _rune)
    {
      //  Debug.Log("initializing " + this.name + "\n");
        ToyInit(s,scaleV, i, _name, _rune);
        start_ammo_by_time = false;
		arrow_count = 0;        
        float dmg_base = 0;
        //dmg_base = stats.dmg;        
        last_target = 0;
        CheckForUpgrades();

        monsters_transform = Peripheral.Instance.monsters_transform;	
        enemy_list = Peripheral.Instance.enemy_list;
		transform.transform.localRotation = Quaternion.identity;     
        if (s.ammo != -1)
        {
            
            ammo = (int)(Mathf.Max(1, s.ammo)* (1f + StaticRune.GetTimeBonus(rune.runetype, rune.toy_type)));// + ammo_bonus;
            GameObject buttons = Peripheral.Instance.zoo.getObject("GUI/ammo_panel", false);
            ammo_panel = buttons.GetComponent<Mini_Toy_Button_Driver>();           
            buttons.SetActive(true);
            ammo_panel.InitMiniDriver(this);
        }        
        else{
            InitRuneButtons();
        }

        center.localPosition = Vector3.Scale(center.localPosition, scaleV);      
		if (rune.level > 0 && rune_buttons != null ) rune_buttons.UpdateMe();
		
        if (area_effector != null)
        {
            InitAreaEffector();
            active = false;
        }
        if (parent_toy != null) parent_toy.AddTargetToy(this);

        if (toy_type == ToyType.Hero)
        {
            rune.InitHero(Central.Instance.current_lvl);//for testing, gives hero some xp based on current level, if xp is 0  
            // also sensible hero comes with airattack for free 
            rune.setMaxLevel((int)Mathf.Max(Central.Instance.getToy(my_name).getMaxLvl(), rune.getMaxLevel()));
        }

        if (rune_buttons != null) rune_buttons.UpdateMe(); // ghost towers don't get their own rune buttons

    }

    void InitAreaEffector(){			
		area_effector.initStats(this);
	}

	public Material GetLaserMaterial(){
		if (laser_material == null){
            UnityEngine.Debug.Log("Laser material not defined! Resources.Loading default material!\n");
			laser_material = (Material) Resources.Load("Materials/default_laser_material", typeof(Material));
		}
		
		return laser_material;
	}
	

    public override ToySaver getSnapshot()
    {			
		
        return new ToySaver(my_name,(float) ammo, rune, toy_type);
    }



    void Update()
	{
	
        if (!active) { return; }

        ToyUpdate();

        if (myTarget && ammo_by_time && start_ammo_by_time)
        {
            ammo_timer += Time.deltaTime;

            if (ammo_timer >= ghost_timer)
            {
                UseAmmo();
                ammo_timer = 0f;
            }
        }

        //CheckForUpgrades(false);
        if (areaEffect) return;


        if (!(myTarget && myTarget.gameObject.activeSelf)
            && (TIME > nextFireTime || isLaser)
            && Peripheral.Instance.monsters_transform.childCount > 0) getEnemy();            
        

        if (myTarget && myTarget.gameObject.activeSelf) {			
			
            if (TIME > nextFireTime && !isLaser)
            {
                if (orient_me) center.transform.rotation = turn;
                CalcAimPosition(myTarget.transform.position);
                FireProjectile();
            }
            if (isLaser) FireLaser();
            
		} 

	}

    protected override void onUpgrade(EffectType type, int ID)
    {
        if (ID == this.gameObject.GetInstanceID())
        {
            if (type == EffectType.Laser) CheckIfLaser();
            else if (type == EffectType.Range) updateRange();
            else if (type == EffectType.Sparkles) CheckIfSparklesSkill();
            else if (type == EffectType.Critical) CheckIfCriticalSkill();
            else if (type == EffectType.Focus)
            {
                reload_time = rune.stat_sum.getReloadTime(false);
                updateRange();
            }
        }
    }

    void CheckForUpgrades()
    {        
            updateRange();
            reload_time = rune.stat_sum.getReloadTime(false);
            CheckIfLaser();
            CheckIfSparklesSkill();
            CheckIfCriticalSkill();
            
            return;                
    }

    void CheckIfLaser()
    {
        if (rune.runetype == RuneType.Sensible && rune.getLevel(EffectType.Laser) > 0)
        {
            if (!isLaser) laser.initLaser();
            isLaser = true;
            laser.initStats(this);            
        }
        else isLaser = false;        
    }

    void CheckIfCriticalSkill()
    {
        if (rune.runetype == RuneType.Vexing && rune.getLevel(EffectType.Critical) > 0)
        {
            isCritical = true;
            critical.Init(rune.getStatBit(EffectType.Critical).getStats(), rune.get(EffectType.VexingForce));
        }
        else isCritical = false;        
    }

    void CheckIfSparklesSkill()
    {
        if (sparkles == null) return;
        if (rune.runetype == RuneType.Sensible && rune.getLevel(EffectType.Sparkles) > 0)
        {
            sparkles.initStats(rune.getStatBit(EffectType.Sparkles).getStats(), this.gameObject.GetInstanceID());
            isSparkles = true;
        }
        else isSparkles = false;
    }


    void getEnemy()
    {

        if (rune.runetype == RuneType.Vexing)        
            getRandomEnemy();        
        else if (isLaser)        
            getNewestEnemy();
        else
            getOldestEnemy();
        
    }

    void OnDisable()
    {
        active = false;
        myTarget = null;
    }

	

    public override void loadSnapshot(ToySaver saver)
    {
        ammo = (int)saver.ammo;        
        rune = saver.rune;	
        if (rune_buttons != null) rune_buttons.UpdateMe();
		if (ammo_panel != null){ammo_panel.SetAmmo();}
        rune.UpdateStats();
        
    }


	public void UseAmmo(){
		if (ammo != -1) ammo--;
		
		if (ammo_panel != null) ammo_panel.SetAmmo();

        if (ammo == 0)
        {
            active = false;
            if (parent_toy != null) parent_toy.RemoveTargetToy(this);

            Die(reload_time*.8f);
        }
    }


	public int Ammo(){
		return ammo;		
	}
	
	public bool CanAddAmmo(int a){
	//	Debug.Log("Can add ammo? " + ammo + "\n");
		if (ammo >= max_ammo) {return false;}
		return Peripheral.Instance.HaveResource(new Cost(CostType.Wishes, a));
	}
	
	public void AddAmmo(int a){

		if (ammo >= max_ammo) { UnityEngine.Debug.Log("no good\n");return;}
		if (Peripheral.Instance.UseResource((new Cost(CostType.Wishes, a)), Vector3.zero)){
			ammo += a*stats.ammo;
			if (ammo > max_ammo) ammo = max_ammo;
			if (ammo_panel != null){ammo_panel.SetAmmo();}

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

    
	void FireProjectile ()
	{
		if (myTarget == null){ return;}        
		arrow_count++;
        if (!ammo_by_time) {
            UseAmmo();
        } else start_ammo_by_time = true;       

		CalculateAimError ();
		
		Vector3 firefrom = transform.position;
		if (arrow_origin != null) firefrom = arrow_origin.position;
		
		
        if (rune.runetype == RuneType.Vexing && rune.get(EffectType.Diffuse) > 0)
            arrow_name = "Arrows/diffuse_arrow";          
        else if (rune.runetype == RuneType.Vexing && rune.get(EffectType.Focus) > 0)        
            arrow_name = "Arrows/focus_arrow";        
        else
            arrow_name = "Arrows/" + arrow_type + "_arrow";


        
        StatSum statsum = rune.GetStats(false);
        statsum.towerID = this.gameObject.GetInstanceID();

        float arrow_speed = -1;
        Arrow arrow = Get.MakeArrow(arrow_name, firefrom, myTarget, statsum, arrow_speed, this, true);
        
        CalcAimPosition(myTarget.position);
        //modifiers on regular arrows
        if (rune.runetype == RuneType.Sensible && rune.get(EffectType.RapidFire) > 0)
        {
            InitRapidFire();
            rapid_fire.Init(rune.getStatBit(EffectType.RapidFire).getStats(), arrow.initial_speed, rune.get(EffectType.Force));
            StatBit force = statsum.GetStatBit(EffectType.Force);            
            force.updateStat(rapid_fire.GetMass());
            arrow.speed = rapid_fire.GetSpeed();
        }   
		if (isCritical) {
            StatBit force = statsum.GetStatBit(EffectType.VexingForce);
			force.updateStat(critical.getCriticalForce());    
		}

        my_tower_stats.Shots_fired++;
		arrow.gameObject.SetActive (true);
        if (!EnemyInRange(myTarget)) myTarget = null;
        incrementNextFireTime();
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
        current_range = rune.stat_sum.getRange();

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
            if (!(m.gameObject.tag == "Enemy" || m.gameObject.tag == "Decoy")) continue;
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
        float best_age = 99;
        Transform only_child = null;
        int in_range = 0;

        for (int i = 0; i < monsters_transform.childCount; i++) //(Transform m in monsters_transform)
        {
            Transform m = monsters_transform.GetChild(i);
            if (!(m.gameObject.tag == "Enemy" || m.gameObject.tag == "Decoy")) continue;
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

            float new_age = enemy_list.getID(m.GetInstanceID());
            if (new_age < best_age && m.GetInstanceID() != last_target)
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

        if (best_age != 99)
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
            if (getDistance(me) < current_range)
            {
                ok[ok_i] = me;
                ok_i++;
            }
        }

        if (ok_i == 0) return;
        
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

    void incrementNextFireTime()
    {
        if (rune.get(EffectType.RapidFire) > 0 && rapid_fire != null)
        {
            nextFireTime = TIME + (rapid_fire.GetTimeToNextArrow());
        }
        else
        {
            nextFireTime = TIME + reload_time;
        }
    }

    void getClosestEnemy ()
	{
		Transform best = null;
		float best_distance = Mathf.Infinity;		
		Transform only_child = null;
		int in_range = 0;
		
		foreach (Transform m in monsters_transform) {
			if (!(m.gameObject.tag == "Enemy" || m.gameObject.tag == "Decoy")) continue;
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

	void CalcAimPosition (Vector3 targetPos)
	{
		Vector2 aimPoint = new Vector3 (targetPos.x + aimError - transform.position.x, 
		                                targetPos.y + aimError - transform.position.y);
		aimPoint = Vector2.ClampMagnitude (aimPoint, 1);
		Vector2 lerp = Vector2.Lerp (myAngle, aimPoint, turnSpeed);
		
		float angle = Vector2.Angle (new Vector2 (0, 1), lerp);
		
		Vector3 cross = Vector3.Cross (new Vector2 (0, 1), lerp);
		if (cross.y > 0) {
			angle = 360 - angle;
		}
		
		myAngle = lerp;
		Quaternion turn2 = Quaternion.AngleAxis (angle, new Vector3 (0, 0, -1));
		
		turn = turn2;
	}
}
