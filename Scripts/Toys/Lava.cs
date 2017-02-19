using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class Lava : MonoBehaviour {

    MyArray<HitMe> monsters;	
	bool am_enabled = true;	
	public StatSum my_stats;	
	public float every_so_often = 0.25f;
	float range;
	float current_time = 0f;
    public bool ignore_flying = false;
	public bool box;
	float tileSize = -1;
	float my_x;
	float my_y;
    public Firearm my_firearm; //if lava belongs to a tower
    public float lifespan = 0f;
    public bool auto_return = false;
    public Animator anim;
    public MonsterType visuals = MonsterType.Burning;
    public GuidedRandomWalk walk;
    bool locationSet = false;
	public delegate void OnLavaBurnHandler(EffectType type);
	public static event OnLavaBurnHandler OnLavaBurn;
    Vector3 position;

    void Start()
    {        
        if (am_enabled)
        {
            if (!locationSet) SetLocation(null, this.transform.position, -1, Quaternion.identity);
            InvokeRepeating("GetVictims", 0f, every_so_often);


        }
    }

    void setScale()
    {

        Transform t = this.transform;
        range = t.localScale.magnitude;
        if (tileSize < 0) { tileSize = Peripheral.Instance.tileSize; }
        if (box)        
        {
            my_x = t.localScale.x;
            my_y = t.localScale.y;
        }

    }
    

    public void Init(StatSum stats, float lifespan, bool auto_return, Firearm firearm)
    {
        if (!locationSet) SetLocation(null, this.transform.position, 1, Quaternion.identity) ;

        this.my_stats = stats;
        this.lifespan = lifespan;
        this.auto_return = auto_return;
        am_enabled = true;
        my_firearm = firearm;
        monsters = null;
        //  Debug.Log(this.gameObject.transform.parent.name + " lava initialized, lifespan " + this.lifespan + "\n");
    //    Debug.Log("Started lava " + my_firearm.my_tower_stats.name + "\n");
        InvokeRepeating("GetVictims", 0f, every_so_often);
    }

    
   
    public void SetFactor(float factor)
    {
        my_stats.factor = factor * every_so_often / (lifespan * range);
    }

    public void SetLocation(Transform parent, Vector3 _position, float size, Quaternion rotation)
    {
   //     Debug.Log("Setting location for lava\n");
        this.transform.parent = (parent == null)? Peripheral.Instance.arrows_transform : this.transform.parent = parent;
        this.transform.position = _position;

        if (size > 0) this.transform.localScale = Vector3.one * size;
        this.transform.localRotation = rotation;
        position = _position;

        setScale();
        locationSet = true;

    }

    void OnDisable()
    {
        am_enabled = false;
        StopAllCoroutines();
    }

    public void KillMe()
    {
        am_enabled = false;
        //   Debug.Log(this.gameObject.transform.parent.name + " dying\n");
        CancelInvoke();
       // Debug.Log("Killed lava " + my_firearm.my_tower_stats.name + "\n");
        if (auto_return) Peripheral.Instance.zoo.returnObject(this.gameObject, true);
    }
	void Update(){

        if (!am_enabled) return;


        if (lifespan > 0f) lifespan -= Time.deltaTime;
        if (lifespan < 0f)
        {
            KillMe();
            return;
        }

        

		current_time += Time.deltaTime;
		
		
		if (current_time >= every_so_often) {
			current_time = 0f;
			
									
		} 		
	}
	

    public void TurnMeOn()
    {
        am_enabled = true;
        position = this.transform.position;
    }
	
	void GetVictims(){
        
		if (monsters == null){monsters = Peripheral.Instance.targets;}

    


        List <HitMe> targets = new List<HitMe>();



        for (int i = 0; i < monsters.max_count; i++)
        {
            HitMe enemy = monsters.array[i];
            if (enemy == null) continue;
          //  if (!box) Debug.Log("Checking " + enemy.gameObject.name + "\n");
            if (enemy.amDying() || !enemy.gameObject.activeSelf) continue;
            
            if (inRange(enemy.transform)) targets.Add(enemy);

        }

      //  if (!box) Debug.Log("Lava getting victims " + position + " range " + range + " got " + targets.Count + " out of " + monsters.max_count + "\n");

        for (int i = 0; i < targets.Count; i++){
			float xp = targets[i].HurtMe(my_stats);
            targets[i].EnableVisuals(MonsterType.Burning, 0.2f);
          //  Debug.Log("hey " + position +  "\n");
             //    Debug.Log(gameObject.name + " got " + xp + "\n");
            //    Debug.Log("Lava stat " + my_stats.getModifiedPrimaryStats(0)[0] + " hurt for " + xp + " xp\n");
            if (my_firearm != null)
            {
                float return_xp = my_firearm.addXp(xp);
                my_firearm.my_tower_stats.Lava_xp += xp;
                my_firearm.my_tower_stats.Lava_count++;
            }
            targets[i].setVisuals(visuals, every_so_often, true);
			if (OnLavaBurn != null) {
			//	Debug.Log("firing lava event\n");
				OnLavaBurn(EffectType.Force);
				
			}
		}
        
	}


	bool inRange(Transform enemy){
		if (!box){
          //  Debug.Log(enemy.position + " <- -> " + position + "\n");
			return (Vector2.Distance(enemy.position, position) < range*tileSize);
		}else{
			bool inrange = true;
            Vector2 this_pos = position;
            Vector2 enemy_pos = enemy.position;
			if (enemy_pos.x < this_pos.x - my_x) inrange = false;
			if (enemy_pos.x > this_pos.x + my_x) inrange = false;
			if (enemy_pos.y < this_pos.y - my_y) inrange = false;
			if (enemy_pos.y > this_pos.y + my_y) inrange = false;
			
			return inrange;			
		}
		
		
	}


}
