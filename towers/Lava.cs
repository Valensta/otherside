using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class Lava : MonoBehaviour {

    MyArray<HitMe> monsters;	
	bool am_enabled = true;	
	public StatSum my_stats;
    public EffectType primary_effecttype;
    public int primary_level = 0;
	float every_so_often;
	float range;
	
    public bool ignore_flying = false;
	public bool box;
	float tileSize = -1;
	float my_x;
	float my_y;
    public bool updateMyPosition = false;
    public Firearm my_firearm; //if lava belongs to a tower
    public float lifespan = 0f;
    public bool auto_return = false;
    public Animator animator;
    public MonsterType visuals = MonsterType.Burning;
    public GuidedRandomWalk walk;
    bool locationSet = false;
	public delegate void OnLavaBurnHandler(EffectType type);
	public static event OnLavaBurnHandler OnLavaBurn;
    Vector3 position;
    public float TIME;
    public float time_to_next_lava_damage;

    void Start()
    {        
        if (am_enabled)
        {
            if (!locationSet) SetLocation(null, this.transform.position, -1, Quaternion.identity);
            InvokeRepeating("GetVictims", 0f, every_so_often);


        }
    }

    

    public void Init(EffectType primary_effecttype, int primary_level, StatSum stats, float lifespan, bool auto_return, Firearm firearm)
    {
        if (!locationSet) SetLocation(null, this.transform.position, 1, Quaternion.identity) ;
        
        this.my_stats = stats;
        this.primary_effecttype = primary_effecttype;
        this.lifespan = lifespan;
        this.auto_return = auto_return;
        this.primary_level = primary_level;
        foreach (StatBit s in stats.stats)
        {
            if (primary_effecttype != EffectType.Diffuse && !s.dumb) { Debug.LogError("Lava " + this.gameObject.name + " is using a smart StatBit!\n"); }
            s.dumb = true;
        }
        am_enabled = true;
        my_firearm = firearm;
        monsters = null;
        every_so_often = Get.lava_damage_frequency;
        //CancelInvoke();
        //InvokeRepeating("GetVictims", 0f, every_so_often);
        TIME = 0f;
        time_to_next_lava_damage = 0f;



    }

    
   
    public void SetFactor(float factor)
    {
        
        my_stats.factor = Get.getModLavaFactor(factor,every_so_often, lifespan,range);
       // Debug.Log(primary_effecttype + " factor " + factor + " every " + every_so_often + " lifespan " + lifespan + " range " + range + " = " + my_stats.factor + "\n");
    }

    public void SetLocation(Transform parent, Vector3 _position, float size, Quaternion rotation)
    {
   //     Debug.Log("Setting location for lava\n");
        this.transform.SetParent((parent == null)? Peripheral.Instance.arrows_transform : parent);
        this.transform.position = _position;
        
        Vector3 new_scale = (size > 0)? Vector3.one * size : Vector3.one;
        new_scale.z = 1f;

        this.range = size;
        this.transform.localScale = new_scale;
        this.transform.localRotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.RandomRange(0f,360f));
     //   Debug.Log("new Scale " + range + " -> " + new_scale + "\n");
        this.transform.localRotation = rotation;
        _position.z = 0f;
        position = _position;

        setScale(range);
        locationSet = true;

    }



    void setScale(float range)
    {

        Transform t = this.transform;
               
        
        if (tileSize < 0) { tileSize = Peripheral.Instance.tileSize; }
        if (box)
        {
            my_x = t.localScale.x;
            my_y = t.localScale.y;
        }

    }

    void TurnMeOff()
    {
    //    Debug.Log("Turning off " + GetInstanceID() + " " + Duration.realtimeSinceStartup + "\n");
        am_enabled = false;
        CancelInvoke();
        StopAllCoroutines();
    }

    void OnDisable()
    {
        TurnMeOff();
    }

    public void KillMe()
    {
        TurnMeOff();
        if (auto_return) Peripheral.Instance.zoo.returnObject(this.gameObject, true);
    }
	void Update(){
        if (Time.timeScale == 0) return;
        if (!am_enabled) return;
        TIME += Time.deltaTime;
    
        if (TIME > time_to_next_lava_damage)
        {
            List<HitMe> targets = GetVictims();
            BurnStuff(targets);
            
            time_to_next_lava_damage += every_so_often;
        }
        
        if (TIME > lifespan)
        {         
            KillMe();
            return;
        }

	}
	

    public void TurnMeOn()
    {
        am_enabled = true;
        position = this.transform.position;
     //   if (animator != null) animator.Play();
    }

    public List<HitMe> GetVictims(){
        if (!am_enabled) return null;

		if (monsters == null){monsters = Peripheral.Instance.targets;}
        if (updateMyPosition) position = this.transform.position;

        List <HitMe> targets = new List<HitMe>();

     //   Debug.Log("Get New " + GetInstanceID() + " " + Duration.realtimeSinceStartup + "\n");
        for (int i = 0; i < monsters.max_count; i++)
        {
            HitMe enemy = monsters.array[i];
            if (enemy == null) continue;          
            if (enemy.amDying() || !enemy.gameObject.activeSelf) continue;
            if (inRange(enemy.transform)) targets.Add(enemy);            
        }

        return targets;
    }

    void BurnStuff(List<HitMe> targets)
    {
        for (int i = 0; i < targets.Count; i++)
        {

            float xp = targets[i].HurtMe(my_stats, my_firearm, primary_effecttype, primary_level);
            if (xp == 0) continue;

            targets[i].EnableVisuals(MonsterType.Burning, 0.2f);

            targets[i].setVisuals(visuals, every_so_often, true);
            if (my_firearm == null && OnLavaBurn != null) OnLavaBurn(EffectType.Force); //only for actual lavas, used for lava intro event			
        }
    }


	bool inRange(Transform enemy){
		if (!box){
          //  Debug.Log(enemy.position + " <- -> " + position + "\n");
			return (Vector2.Distance(enemy.position, position) < range*tileSize);
		}else{
			
            Vector2 this_pos = position;
            Vector2 enemy_pos = enemy.position;
            if (enemy_pos.x < this_pos.x - my_x) return false;
			if (enemy_pos.x > this_pos.x + my_x) return false;
            if (enemy_pos.y < this_pos.y - my_y) return false;
            if (enemy_pos.y > this_pos.y + my_y) return false;

            return true;	
		}
		
		
	}


}
