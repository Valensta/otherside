using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;
//using Cloner;

[System.Serializable]
public class TransformTo
{
    public string name;
    public float percent;
}

[System.Serializable]
public class HitMe : MonoBehaviour
{

    public int lvl;
    public Vector3 start_blah;
    public float velocity;
    public float timer = 0f;
    public GameObject parent;
    public GameObject actor;
    public unitStats stats;
    bool dying; // if true, hitme is dead, dying, don't hit with arrow
    public EffectVisuals aura;
    public Vector3 status_bar_location = new Vector3(0, 0, 0.5f);
    float mass_factor = 2f; //die when reach mass/mass_factor - for hitmestatusbar
    float tileSize;
    public SpriteRenderer sprite_renderer;
    public TransformTo[] transform_to;
    Vector3 init_scale;
    float init_speed = -1;
    float init_mass = -1;

    public string death_effect;
    GameObject death_effect_object;
    WishType[] wish_list;
    public EffectVisuals my_visuals;
    public AI my_ai;
    public Body my_body;
    Collider2D my_collider;
    public Rigidbody2D my_rigidbody;
    bool modified_defenses; //if defenses are not same as init defenses because of some skill
    bool normalize_defenses_is_running = false;
    Mass mass = null;
    
    public Transform scale_transform = null;
    float check_if_alive_repeat_rate = 1f;
    EMP emp;
    Teleport teleport;
    
  

    public List<Modifier> modifiers;//things that are done to the enemies, except for mass. Mass is special.
    public Modifier[] enemy_tech;//things the enemies can do
    

    public delegate void onMakeWishHandler(List<Wish> inventory, Vector3 pos);
    public static event onMakeWishHandler onMakeWish;

    [SerializeField]
    public List<Defense> init_defenses = new List<Defense>();    
    public List<Defense> defenses = new List<Defense>();

    public T getModifier<T>() where T : Modifier
    {
        foreach(Modifier m in modifiers)        
            if (m is T) return (T)m;
                
        return null;
    }

    public T addModifier<T>() where T : Modifier
    {
        T thing = null;
        thing = getModifier<T>();
        if (thing != null) return thing;

        thing = gameObject.AddComponent<T>() as T;
        modifiers.Add(thing);
        return thing;
    }

    public bool Runed(RuneType rune) {
        bool status = false;
        switch (rune) {
            case RuneType.Airy:
                Speed speed = getModifier<Speed>();
                if (speed != null && speed.enabled == true) status = true;                
                break;

            default:
                break;
        }
        return status;

    }

    public float GetInitMass() {
        return init_mass;
    }

    public Collider2D getCollider() {
        return my_collider;
    }

    public bool IsHurt() {
        if (mass != null) return mass.IsHurt();
        return false;
    }

    public void setVisuals(MonsterType type, float time, bool gentle) {
        if (dying) return;
        if (my_visuals == null || !my_visuals.gameObject.activeSelf) initVisuals();        
        my_visuals.Enable(type, time);

    }

    public void initVisuals() {
        GameObject thing = Peripheral.Instance.zoo.getObject("Core/EffectVisuals", true);
        my_visuals = thing.GetComponent<EffectVisuals>();
        my_visuals.transform.parent = this.transform.GetChild(0).transform;
        my_visuals.transform.localPosition = Vector3.zero;

    }

    public void initStats(unitStats s)
    {
        //	if (tween == null) {tween = this.gameObject.AddComponent<Tweener>();}	
        if (my_ai == null)        my_ai = GetComponent<AI>(); 
        if (my_body == null)      my_body = GetComponent<Body>(); 
        if (my_rigidbody == null) my_rigidbody = GetComponentInChildren<Rigidbody2D>(); 
        if (my_collider == null)  my_collider = GetComponentInChildren<Collider2D>(); 

        if (my_visuals == null || !my_visuals.gameObject.activeSelf) initVisuals();
        

        modified_defenses = false;
        normalize_defenses_is_running = false;

        if (mass != null) mass.gameObject.transform.parent = this.transform; 
        if (mass == null) mass = Peripheral.Instance.zoo.getObject("Monsters/Helpers/Mass", true).GetComponent<Mass>();
        if (!mass.gameObject.activeSelf) mass.gameObject.SetActive(true);
        
        _initMass();
        mass.gameObject.transform.parent = this.transform;
        mass.gameObject.transform.localPosition = Vector3.zero;
        mass.gameObject.transform.localScale = Vector3.one;
        mass.gameObject.transform.localRotation = Quaternion.identity;
        defenses = CloneUtil.copyList<Defense>(init_defenses);
    
        stats = s.DeepClone();        
        tileSize = Peripheral.Instance.tileSize;

        init_scale = s.getScale();
        this.transform.localScale = init_scale;

        if (init_mass == -1) {
            init_mass = my_rigidbody.mass;
        } else {
            my_rigidbody.mass = init_mass;
            mass.Define(my_rigidbody, this.transform.GetChild(0).transform, mass_factor, status_bar_location, scale_transform);
        }

        if (init_speed == -1) init_speed = my_ai.speed;
        else my_ai.speed = init_speed;
        

        wish_list = new WishType[stats.inventory.Count];

        for (int i = 0; i < stats.inventory.Count; i++)
            wish_list[i] = stats.inventory[i].type;        

        mass.SetPosition(status_bar_location);
        dying = false;
        InvokeRepeating("CheckIfAlive", 0, check_if_alive_repeat_rate);

        for (int i = 0; i < enemy_tech.Length; i++)
        {
            enemy_tech[i].setActive(true);
        }
    }

    public bool amDying() { return dying; }
    

    //1 is max
    float GetDefense(EffectType t) {
        float v = 0;
        foreach (Defense d in defenses)        
            if (d.type == t) v = d.strength;        

        return v;
    }

    void IncrementDefense(EffectType t, float v)
    {
        foreach (Defense d in defenses)
        {
            if (d.type == t)
            {
                d.strength = Mathf.Min(1f, d.strength + v);
                return;
            }
        }
        defenses.Add(new Defense(t, v));
        return;
    }

    float GetInitDefense(EffectType type)
    {
        foreach (Defense d in init_defenses)        
            if (d.type == type) return d.strength;            

        return 0;
    }

    IEnumerator NormalizeDefense()
    {
        //15 = 7.09
        //8 ~ 3.6?
        float timer = 0f;
        bool all_good = false;
        //Debug.Log("Starting normalization for " + this.gameObject.name + "\n");
        while (!all_good)
        {
            all_good = true;
            foreach (Defense d in defenses)
            {
                if (!(d.type == EffectType.Teleport || d.type == EffectType.Fear)) continue;
                float init_d = GetInitDefense(d.type);
                float goodie = (init_d + 24 * d.strength) / 25f;
              //  Debug.Log("want " + goodie + " from " + d.strength + " to " + init_d + "\n");

                if (Mathf.Abs(init_d - goodie) > 0.01)
                {
              //      Debug.Log("normalizing " + this.gameObject.name + " from " + d.strength + " to " + goodie + "\n");
                    d.strength = goodie;
                    all_good = false;


                }
            }
            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

    //    Debug.Log("DONE normalizing " + this.gameObject.name + " in " + timer + "\n");
        modified_defenses = false;
        normalize_defenses_is_running = false;
        yield return null;
    }

    public void SetDefense(EffectType t, float v)
    {
        foreach (Defense d in defenses)
        {
            if (d.type == t)
            {
                d.strength = v;
                return;
            }
        }
        defenses.Add(new Defense(t, v));
        return;
    }

    public void returnXp(float x)
    {
        stats.returnXp(x);
    }




    //1 is fully defended
    public float HurtMe(StatSum statsum)
    {
        if (dying) return 0;

        float factor = statsum.factor;//does anybody know what this is for? laser?
        float defense;
        float xp = 0f;


        StatBit[] statbits = statsum.stats;

        bool is_laser = (statsum.GetStatBit(EffectType.Laser) != null);

        for (int i = 0; i < statbits.Length; i++)
        {
            StatBit skill_stats = statbits[i];
            switch (statsum.runetype)
            {
                case RuneType.Sensible:

                    if (is_laser)
                    {

                        defense = GetDefense(EffectType.Force);
                        if (statbits[i].effect_type == EffectType.Laser && defense < 1)
                            xp += stats.getXp(ForceMe(skill_stats.getModifiedStats(factor, defense)));

                    }
                    else {
                        
                        if (statbits[i].effect_type == EffectType.Stun)
                        {
                            defense = GetDefense(EffectType.Stun);
                            if (defense < 1f) xp += StunMe(skill_stats.getModifiedStats(factor, defense)) / 10f;
                        }

                        float force_defense = GetDefense(EffectType.Force);
                        if (force_defense < 1) {
                            if (statbits[i].effect_type == EffectType.DOT)
                                xp += DOTMe(skill_stats.getModifiedStats(factor, force_defense), statsum.towerID);

                            if (statbits[i].effect_type == EffectType.Force)
                                xp += stats.getXp(ForceMe(statsum.getModifiedPrimaryStats(force_defense)));


                            if (statbits[i].effect_type == EffectType.Explode_Force)
                                xp += stats.getXp(ForceMe(skill_stats.getModifiedStats(factor, force_defense)));
                        }
                    }

                    break;

                case RuneType.Airy:
                    
                    if (statbits[i].effect_type == EffectType.Weaken) {
                        defense = GetDefense(EffectType.Weaken);                        
                        if (defense < 1) WeakenMe(skill_stats.getModifiedStats(factor, defense));
                    }


                    float speed_defense = GetDefense(EffectType.Speed);
                    if (statbits[i].effect_type == EffectType.Speed && speed_defense < 1){                        
                        xp += 
                            SpeedMe(skill_stats.getModifiedStats(factor, speed_defense)); //this xp factor is to balance out xp to  match other towers that do more damage
                    }

                    float hey_defense = GetDefense(EffectType.Force);
                    if (statbits[i].effect_type == EffectType.Force && speed_defense < 1)
                    {
                        xp += stats.getXp(ForceMe(skill_stats.getModifiedStats(factor, hey_defense)));
                    }

                    if (statbits[i].effect_type == EffectType.DirectDamage)
                        xp += stats.getXp(ForceMe(skill_stats.getModifiedStats(factor, 0)));                    

                    if (statbits[i].effect_type == EffectType.EMP)
                    {
                        defense = GetDefense(EffectType.EMP);
                        if (defense < 1) EMPMe(skill_stats.getModifiedStats(factor, defense));                    
                    }                    

                    break;

                case RuneType.Time:                    
                    
                    if (statbits[i].effect_type == EffectType.TimeSpeed)                    
                        xp += SpeedMe(skill_stats.getModifiedStats(factor, 0)); 
                    
                    break;

                case RuneType.Vexing:                    
                    if (statbits[i].effect_type == EffectType.VexingForce)
                    {
                        defense = GetDefense(EffectType.VexingForce);
                        if (defense < 1) xp += stats.getXp(ForceMe(statsum.getModifiedPrimaryStats(defense)));
                    }


                    if (statbits[i].effect_type == EffectType.Teleport)
                    {
                        defense = GetDefense(EffectType.Teleport);
                        if (defense < 1) xp += TeleportMe(skill_stats.getModifiedStats(factor, defense));
                    }


                    if (statbits[i].effect_type == EffectType.Fear)
                    {
                        defense = GetDefense(EffectType.Fear);
                        if (defense < 1) FearMe(skill_stats.getModifiedStats(factor, defense));
                    }


                    if (statbits[i].effect_type == EffectType.Transform)
                    {
                        defense = GetDefense(EffectType.Transform);
                        if (defense < 1) TransformMe(skill_stats.getModifiedStats(factor, defense));
                    }

                    break;

                default:
                    
                    if (statbits[i].effect_type == EffectType.Force)
                    {
                        defense = GetDefense(EffectType.Force);
                        if (defense < 1) xp += stats.getXp(ForceMe(skill_stats.getModifiedStats(factor, defense)));
                    }
                    break;

            }
        }

      //  Debug.Log("Total xp " + xp + "\n");
        return xp;

    }



    void AuraOn(MonsterType addme, float time) {
        if (aura != null) {
            aura.Enable(addme, time);
        }

    }
    // Update is called once per frame
    void Update()
    {
        
        if (timer == 0) start_blah = this.transform.position;        

        if (timer > 1)
        {
            Vector3 position = this.transform.position;
            velocity = Vector3.Distance(position, start_blah) / timer;
            start_blah = position;
            timer = 0;
        }
        timer += Time.deltaTime;
    

        if (modified_defenses && normalize_defenses_is_running == false)
        {
            normalize_defenses_is_running = true;
            StartCoroutine(NormalizeDefense());
        }

        if (my_rigidbody.mass <= init_mass / mass_factor) {
            //	Debug.Log(this.name + " is dying, not enough mass\n");
            if (this.gameObject.activeSelf) StartCoroutine("DiePretty");
        }

    }



    void CheckIfAlive()
    {
        if (!Get.checkPosition(this.transform.position))
        {
            Debug.Log("Died on position\n");
            Die(false);
            return;
        }


        if (my_ai.Path.Count > 0 && transform.position.y < my_ai.Path[0].position.y - 10f)
        {
            Debug.Log("Died on whatever that other check is\n");
            Die(false);
            return;
        }
    }


    public void setPercentMass(float percent)
    {
        mass.setPercentMass(percent);
    }
    public float getPercentMass()
    {
        return mass.getPercentMass();
    }


    //gets used by dot
    public float MassMe(float[] stats)
    {        
        float xp = mass.Init(stats);        
        return xp;
    }


    public void DieSpecial()
    {//Debug.Log (this.name + " is dying special\n");
     //Peripheral.Instance.monsters--;
        StartCoroutine(this.DieSlowly());
    }

    IEnumerator DieSlowly()
    {
        float total = 1f;
        int steps = 10;
        for (int i = 0; i < steps; i++) {
            SizeMe(0.85f);
            yield return new WaitForSeconds(total / steps);
        }


        Die(false);

    }


    IEnumerator DiePretty() {
        CancelInvoke();
        dying = true;

        if (death_effect == "") {
            death_effect = "Monsters/Helpers/monster_death_puffy_pale_blue";
        }
        if (death_effect_object == null) {
            death_effect_object = Peripheral.Instance.zoo.getObject(death_effect, false);
        }

        Vector3 pos = Get.fixPosition(this.transform.position);
        pos.z = 2f;
        death_effect_object.transform.position = pos;
        death_effect_object.SetActive(true);
        //yield return new WaitForSeconds (.5f);
        Die(true);

        yield return null;

    }

    void disableModifiers()
    {
        foreach (Modifier m in modifiers) m.setActive(false);        
    }

    void disableEnemyTech(bool iwaskilled)
    {
        foreach (Modifier m in enemy_tech)
        {
            if (iwaskilled) m.DisableAction();
            m.setActive(false);
        }
    }

    public void Die(bool iwaskilled)
    {
        CancelInvoke();
        dying = true;
        disableModifiers();
        disableEnemyTech(iwaskilled);
        my_ai.Die();
        my_visuals.Die();
        if (Noisemaker.Instance != null) Noisemaker.Instance.Play("monster_died");
        if (iwaskilled && this.tag.Equals("Enemy"))
        {
            float yay = ((float)stats.cost_type.Amount);
       //     Debug.Log(this.gameObject.name + " : " + yay + "\n");
            Peripheral.Instance.addDreams(yay, this.transform.position, true);
            if (onMakeWish != null) onMakeWish(stats.inventory, this.transform.position);
        }
        StopAllCoroutines();
        CancelInvoke();
        Regenerator health_over_time = addModifier<Regenerator>();
        if (health_over_time != null) health_over_time.StopMe();
        Peripheral.Instance.targets.removeByID(this);
        this.transform.parent = null;
        Peripheral.Instance.zoo.returnObject(this.gameObject, true);

    }


    public float SizeMe(float aff)
    {
        if ((aff < 1 && this.transform.localScale.magnitude * aff > init_scale.magnitude * 0.15f) ||
            (aff > 1 && this.transform.localScale.magnitude * aff < init_scale.magnitude * 3f)
            ) {
            this.transform.localScale = this.transform.localScale * aff;

        }
        return 0;
    }



    public float ForceMe(float[] stats)
    {
        float a = mass.Init(stats);
        return a;
    }

    public float StunMe(float[] stats)
    {
        Speed speed = addModifier<Speed>();        
        float xp = speed.Init(this, stats, true);
        return xp;
    }

    public float DOTMe(float[] stats, int ID)
    {
        Regenerator health_over_time = addModifier<Regenerator>();
        float xp = health_over_time.Init(this, stats, RegeneratorType.Self, ID);
        return xp;
    }



    public float SpeedMe(float[] stats)//(float aff, float time)
    {
        Speed speed = addModifier<Speed>();
        float xp = speed.Init(this, stats, false);
        return xp;
    }

  
    public float TeleportMe(float[] stats) //(float aff, float where)
    {

        if (teleport == null) {
            teleport = gameObject.AddComponent<Teleport>() as Teleport;
            teleport.Init(this);
        }

        modified_defenses = true;
 
        float def = teleport.TeleportMe(stats);
        IncrementDefense(EffectType.Teleport, def);
        return def;

    }


    public void EMPMe(float[] stats) //(float aff, float where)
    {
        if (emp == null) emp = gameObject.AddComponent<EMP>() as EMP;
        emp.Init(stats, enemy_tech);    
    }

    public float TransformMe(float[] stats)
    {
        Converter transform = addModifier<Converter>();
        bool yay = transform.Init(this, stats);    
        return 0f;
    }

    public float FearMe(float[] stats)
    {
        Fear fear = addModifier<Fear>();              
        float def = fear.Init(this, stats);
        modified_defenses = true;
        IncrementDefense(EffectType.Fear, def);
        return def;
    }


    void _initMass()
    {
        mass.Define(my_rigidbody, this.transform.GetChild(0).transform, mass_factor, status_bar_location, scale_transform);
    }


    public float WeakenMe(float[] stats) {
                                          
        Weaken weaken = addModifier<Weaken>();
		float xp = weaken.Init(this, stats);		
		return xp;
	}

    public void EnableVisuals(MonsterType type, float timer)
    {
        if (my_visuals != null || !my_visuals.gameObject.activeSelf) my_visuals.Enable(type, timer); 
    }

}


//strength 0.5 will not necessarily translate to 50% weaker due to how the effect's math may work
[System.Serializable]
public class Defense : IDeepCloneable<Defense>{
	[SerializeField]
	public EffectType type;
	[Range(0f,1f)]
	public float strength;
	
	public Defense (EffectType _type, float _strength){
		type = _type;
        strength = _strength;
	}
	
	public bool ValidateMe(){
		if (strength < 0f || strength > 1f || Get.isGeneric(type) == true)
			throw new System.ArgumentException("Invalid Defense @ " + type + " " + strength + "\n");

		return true;
	}


    object IDeepCloneable.DeepClone()
    {
        return this.DeepClone();
    }

    public Defense DeepClone()
    {
        Defense f = new Defense(this.type, this.strength);        
        return f;
    }
}