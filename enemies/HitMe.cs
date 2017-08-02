using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class HitMe : DistractorObject
{

    public int lvl;   
    public GameObject parent;
    public GameObject actor;
    public enemyStats stats;
    bool dying; // if true, hitme is dead, dying, don't hit with arrow
    public EffectVisuals aura;
    public Vector3 status_bar_location = new Vector3(0, 0, 0);
    float mass_factor = 2f; //die when reach mass/mass_factor - for hitmestatusbar
    //float tileSize;
    public SpriteRenderer sprite_renderer;
    //public TransformTo[] transform_to;
    Vector3 init_scale;
    float init_speed = -1;
    float init_mass = -1;

    public string death_effect;
    GameObject death_effect_object;
    WishType[] wish_list;
    public EffectVisuals my_visuals;
    public AI my_ai;
    public Body my_body;
    public Collider2D my_collider;
    public Rigidbody2D my_rigidbody;
    bool modified_defenses; //if defenses are not same as init defenses because of some skill
    bool normalize_defenses_is_running = false;
    Mass mass = null;
    
    public Transform scale_transform = null;
    float check_if_alive_repeat_rate = 1f;
    EMP emp;
    Teleport teleport;

    public List<Lava> lavas; //any lava that is attached to the enemy by an attack, for proper cleanup upon death

    public List<Modifier> modifiers;//things that are done to the enemies, except for mass. Mass is special.
    public Modifier[] enemy_tech;//things the enemies can do
    

    public delegate void onMakeWishHandler(List<Wish> inventory, Vector3 pos);
    public static event onMakeWishHandler onMakeWish;

    [SerializeField]
    public List<Defense> init_defenses = new List<Defense>();     //need this for transform. this is not always the same as stats.defenses
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

    public float getHP()
    {
        return mass.getPercentMass();
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
        my_visuals.auto_stabilize = my_ai.orient;
        my_ai.visuals = my_visuals;

    }

    public void CheckBasicStats(enemyStats s)
    {
        //if (my_rigidbody.mass != s.mass) Debug.LogError(this.name + " mass Prefab != init file: " + my_rigidbody.mass + " vs " + s.mass + "\n");
        //if (my_ai.speed != s.speed) Debug.LogError(this.name + " speed Prefab != init file: " + my_ai.speed + " vs " + s.speed + "\n");
        //if (Get.GetDefense(defenses,EffectType.Force) != s.defense) Debug.LogError(this.name + " defense Prefab != init file: " + Get.GetDefense(defenses, EffectType.Force) + " vs " + s.defense + "\n");
       // if (Get.GetDefense(defenses,EffectType.VexingForce) != s.vf_defense) Debug.LogError(this.name + " vf_defense Prefab != init file: " + Get.GetDefense(defenses, EffectType.VexingForce) + " vs " + s.vf_defense + "\n");

    }

    public void initStats(enemyStats s)
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
        stats = s.DeepClone();

        init_defenses = CloneUtil.copyList<Defense>(stats.defenses);
        defenses = CloneUtil.copyList<Defense>(stats.defenses);
    
     
        //tileSize = Peripheral.Instance.tileSize;

        init_scale = Vector3.one;
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

        lavas = new List<Lava>();
     //   CheckBasicStats(s); //for wave balancing purposes, reminder that these stats are now in the init file, not on the prefab
    }

    public bool amDying() { return dying; }
    

    //1 is max

        //fear, teleport etc = magic


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
        if (Time.timeScale == 0) yield return new WaitForSeconds(0.1f);
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


    public float HurtMe(StatSum statsum, Firearm firearm, EffectType primary_effecttype)
    {
        return HurtMe(statsum, firearm, primary_effecttype, -1);
    }

    //1 is fully defended
    public float HurtMe(StatSum statsum, Firearm firearm, EffectType primary_effecttype, int primary_level)
    {
        if (dying) return 0;
        if (checkIfWasKilled()) return 0;

        float factor = statsum.factor;//does anybody know what this is for? laser?
        float defense;
        float xp = 0f;     



        StatBit[] statbits = statsum.stats;
        

        for (int i = 0; i < statbits.Length; i++)
        {
            StatBit skill_stats = statbits[i];
            if (statbits[i].effect_type == EffectType.Range) continue;
            if (statbits[i].effect_type == EffectType.ReloadTime) continue;

            switch (statsum.runetype)
            {
                case RuneType.Sensible:
                    bool is_laser = (statsum.GetStatBit(EffectType.Laser) != null);

                    if (is_laser)
                    {

                        defense = Get.GetDefense(defenses, EffectType.Force);
                        if (statbits[i].effect_type == EffectType.Laser && defense < 1) {
                           float add = stats.getXp(ForceMe(skill_stats.getModifiedStats(factor, defense)));
                            xp += add;                            
                            assignXp(add, firearm, primary_effecttype, EffectType.Laser, primary_level, skill_stats.level);
                        }
                    }
                    else {

                        if (statbits[i].effect_type == EffectType.Stun)
                        {
                            float add = StunMe(skill_stats.getModifiedStats(factor, 0)) / 10f;
                            xp += add;
                            assignXp(add, firearm, primary_effecttype, EffectType.Stun, primary_level,
                                skill_stats.level);

                        }

                        if (statbits[i].effect_type == EffectType.Transform)
                        {
                            defense = Get.GetDefense(defenses, EffectType.Transform);
                            //applied with diffuse, which has a factor. this is not susceptible to that factor
                            if (defense < 1) TransformMe(skill_stats.getModifiedStats(factor, defense), factor);
                        }

                        float force_defense = Get.GetDefense(defenses, EffectType.Force);
                        if (force_defense < 1) {                          

                            if (statbits[i].effect_type == EffectType.Force)
                            {
             //                   Debug.Log("factor " + statsum.factor + "\n");
                                float add = stats.getXp(ForceMe(statsum.getModifiedPrimaryStats(force_defense)));
                                xp += add;            
                                assignXp(add, firearm, primary_effecttype, EffectType.Force, primary_level, skill_stats.level);
                            }


                            if (statbits[i].effect_type == EffectType.Explode_Force)
                            {
                                float add = stats.getXp(ForceMe(skill_stats.getModifiedStats(factor, force_defense)));
                                xp += add;                             
                                assignXp(add, firearm, primary_effecttype, EffectType.Explode_Force, primary_level, skill_stats.level);
                            }
                        }


                        if (statbits[i].effect_type == EffectType.Speed && statbits[i].effect_sub_type == EffectSubType.Ultra)
                        {
                            float add = SpeedMe(skill_stats.getModifiedStats(factor, 0), statbits[i].effect_type, statbits[i].effect_sub_type);                            
                            xp += add * (1 + firearm.toy.rune.level * 0.5f);
                            assignXp(add, firearm, primary_effecttype, EffectType.Meteor, primary_level, skill_stats.level);
                        }
                    }

                    break;

                case RuneType.Airy:
                    
                    if (statbits[i].effect_type == EffectType.Weaken) {
                        defense = (statbits[i].effect_sub_type == EffectSubType.Null) ? Get.GetDefense(defenses, EffectType.Weaken) : 0;
                        if (defense < 1)
                        {
                            float add = WeakenMe(skill_stats.getModifiedStats(factor, defense));
                            xp += add;                         
                            assignXp(add, firearm, primary_effecttype, EffectType.Weaken, primary_level, skill_stats.level);
                        }
                    }


                    float speed_defense = Get.GetDefense(defenses, EffectType.Speed)/2f; //otherwise they seem like they are very resistant to speed
                    if (statbits[i].effect_type == EffectType.Speed && speed_defense < 1){                        
                        float add = SpeedMe(skill_stats.getModifiedStats(factor, speed_defense), statbits[i].effect_type, statbits[i].effect_sub_type); //this xp factor is to balance out xp to  match other towers that do more damage
                        xp += add;
                     
                        assignXp(add, firearm, primary_effecttype, EffectType.Speed, primary_level, skill_stats.level);
                    }

                    if (statbits[i].effect_type == EffectType.Speed && statbits[i].effect_sub_type == EffectSubType.Freeze)
                    {
                        float add = SpeedMe(skill_stats.getModifiedStats(factor, 0), statbits[i].effect_type, statbits[i].effect_sub_type);
                      //  Debug.Log("timefreeze factor is " + factor + "\n");
                        xp += add;
                        assignXp(add, firearm, primary_effecttype, EffectType.Frost, primary_level, skill_stats.level);
                    }
                    

                    float hey_defense = Get.GetDefense(defenses, EffectType.Force);
                    if (statbits[i].effect_type == EffectType.Force && speed_defense < 1)
                    {
             
                        float add = stats.getXp(ForceMe(skill_stats.getModifiedStats(factor, hey_defense)));
                        xp += add;        
                        assignXp(add, firearm, primary_effecttype, EffectType.Force, primary_level, skill_stats.level);
                    }
                                        

                    if (statbits[i].effect_type == EffectType.DirectDamage)
                    {
                       float add = stats.getXp(ForceMe(skill_stats.getModifiedStats(factor, 0)));
                        xp += add;                     
                        assignXp(add, firearm, primary_effecttype, EffectType.DirectDamage, primary_level, skill_stats.level);
                    }

                    //SSkill
                    if (statbits[i].effect_type == EffectType.EMP) EMPMe(skill_stats.getModifiedStats(factor, 0), false); 

                    //Regular Skill: Foil -> Foil Lava -> summons EMP
                    if (primary_effecttype == EffectType.Foil && statbits[i].effect_type == EffectType.Foil) EMPMe(skill_stats.getModifiedStats(factor, 0), true);

                    break;


                case RuneType.Vexing:
                    float vf_defense = Get.GetDefense(defenses, EffectType.VexingForce);
                    if ((primary_effecttype != EffectType.Focus && statbits[i].effect_type == EffectType.VexingForce) ||
                        (primary_effecttype == EffectType.Focus && primary_effecttype == statbits[i].effect_type)) // for Focus, use focus statbits, not VF
                    {
                        
                        if (vf_defense < 1)
                        {
                            float add = stats.getXp(ForceMe(statsum.getModifiedPrimaryStats(vf_defense)));
                            xp += add;                          
                            assignXp(add, firearm, primary_effecttype, EffectType.VexingForce, primary_level, skill_stats.level);

                        }
                        
                    }

                    if (statbits[i].effect_type == EffectType.DOT)
                    {
                        DOTMe(skill_stats.getModifiedStats(factor, vf_defense), firearm, skill_stats.level);
                        //does damage through an invoke which can't get back to this
                        //invoke calls MassMe, which assigns XP
                    }

                    if (statbits[i].effect_type == EffectType.Critical)
                    {
                        float critical_factor = firearm.critical.getCriticalForce();//so that we can do correct xp attribution for it
                        if (critical_factor > 0)
                        {
                            float new_defense = vf_defense / (1 + critical_factor);
                            float[] new_stats = statsum.getModifiedPrimaryStats(vf_defense / (1 + critical_factor));
                            float add = stats.getXp(ForceMe(new_stats));
                            xp += add;
                            assignXp(add, firearm, EffectType.Critical, EffectType.Critical, primary_level, skill_stats.level);
                        }
                    }

                    if (statbits[i].effect_type == EffectType.Teleport)
                    {
                        defense = Get.GetDefense(defenses, EffectType.Teleport);
                        if (defense >= 1) return xp;
                        float add = TeleportMe(skill_stats.getModifiedStats(factor, defense));
                        xp += add;
                        assignXp(add, firearm, primary_effecttype, EffectType.Teleport, primary_level, skill_stats.level);

                    }

                    if (statbits[i].effect_type == EffectType.Fear)
                    {
                        defense = Get.GetDefense(defenses, EffectType.Fear);
                        if (defense < 1) FearMe(skill_stats.getModifiedStats(factor, defense));
                    }

                    break;

                    
                case RuneType.Time:

                    if (statbits[i].effect_type == EffectType.Speed && statbits[i].effect_sub_type == EffectSubType.Ultra)
                    {
                        defense = Get.GetDefense(defenses, StaticRune.getPrimaryDamageType(statsum.runetype));
                        if (defense >= 1) return xp;

                        float add = SpeedMe(skill_stats.getModifiedStats(factor, defense), EffectType.Speed, statbits[i].effect_sub_type);
                        xp += add;
                        assignXp(add, firearm, primary_effecttype, EffectType.Speed, primary_level, skill_stats.level);
                    }
                    break;
                    
                case RuneType.Fast:
                case RuneType.Slow:
                    if (statbits[i].effect_type == EffectType.Force)
                    {
                        defense = Get.GetDefense(defenses, StaticRune.getPrimaryDamageType(statsum.runetype));
                        if (defense >= 1) return xp;

                        float add = stats.getXp(ForceMe(skill_stats.getModifiedStats(factor, defense)));
                        xp += add;
                        assignXp(add, firearm, primary_effecttype, EffectType.Force, primary_level, skill_stats.level);

                    }

                    break;
                default:

                    if (statbits[i].effect_type == EffectType.Force)
                    {
                        defense = Get.GetDefense(defenses, EffectType.Force);
                        if (defense >= 1) return xp;
                        float add = stats.getXp(ForceMe(skill_stats.getModifiedStats(factor, defense)));
                        xp += add;
                        assignXp(add, firearm, primary_effecttype, EffectType.Force, primary_level, skill_stats.level);

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
    

        if (modified_defenses && normalize_defenses_is_running == false)
        {
            normalize_defenses_is_running = true;
            StartCoroutine(NormalizeDefense());
        }

        checkIfWasKilled();

    }

    public bool checkIfWasKilled()
    {
        //if (my_rigidbody.mass <= init_mass / mass_factor)
        if (mass.amIDead())
        {            
            if (this.gameObject.activeSelf) StartCoroutine("DiePretty");
            return true;
        }
        return false;
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


    public float MassMe(float[] stats,Firearm firearm, EffectType effect_type, int level)
    {
        float xp = mass.Init(stats);
        if (xp > 0) Get.assignXP(xp, level, this, firearm, this.transform.position, effect_type);

        return xp;
    }

    void assignXp(float add, Firearm firearm, EffectType primary_effecttype, EffectType override_effecttype, int primary_level, int override_level)
    {
        EffectType use_effecttype = (primary_effecttype == EffectType.Null) ? override_effecttype : primary_effecttype;
        int use_level = (primary_level == -1) ? override_level : primary_level;
        if (firearm != null) Get.assignXP(add, use_level, this, firearm, this.transform.position, use_effecttype);
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

        if (death_effect.Equals("")) {
            death_effect = "Monsters/Helpers/monster_death_puffy_pale_blue";
        }
        if (death_effect_object == null) {
            death_effect_object = Peripheral.Instance.zoo.getObject(death_effect, false);
        }

        //Vector3 pos = Get.fixPosition(this.transform.position);
        Vector3 pos = this.transform.position;
        pos.z = 4f;
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
            float yay = stats.point_factor;       
            Peripheral.Instance.addDreams(yay, this.transform.position, true);
            if (onMakeWish != null) onMakeWish(stats.inventory, this.transform.position);
            
        }
        StopAllCoroutines();
        CancelInvoke();
        foreach (Lava lava in lavas)
        {
            if (lava.gameObject.activeSelf)  lava.KillMe();
        }
        Regenerator health_over_time = addModifier<Regenerator>();
        if (health_over_time != null) health_over_time.StopMe();
        Peripheral.Instance.targets.removeByID(this);
        Moon.Instance.enemyDied(my_ai.my_dogtag.wave);
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
        float xp = speed.Init(this, stats, EffectType.Stun, EffectSubType.Null);
        return xp;
    }

    public float DOTMe(float[] stats, Firearm firearm, int level)
    {
        Regenerator health_over_time = addModifier<Regenerator>();
        float xp = health_over_time.Init(this, stats, RegeneratorType.Self, firearm, level);
        return xp;
    }



    public float SpeedMe(float[] stats, EffectType type, EffectSubType sub_type)//(float aff, float time)
    {
        Speed speed = addModifier<Speed>();
        float xp = speed.Init(this, stats, type, sub_type);
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


    public void EMPMe(float[] stats, bool foil) //(float aff, float where)
    {
        if (emp == null) emp = gameObject.AddComponent<EMP>() as EMP;
    //    Debug.Log("Hitme initializing EMP\n");
        emp.Init(stats, enemy_tech, foil);    
    }

    public float TransformMe(float[] stats, float factor)
    {
        Converter transform = addModifier<Converter>();
        transform.Init(this, stats, factor);    
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