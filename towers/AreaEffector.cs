using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class AreaEffector : MonoBehaviour {
  //  public float period = -1; //in seconds, how often and how long the pulse lasts, -1 is forever
   // public float duration = -1;
    public float duration_factor = StaticRune.getAiryDurationFactor();
    MyArray<HitMe> monsters = null;
    float tween_time = 0.025f;
    bool am_enabled = false;
    float tileSize = -1;
    GameObject halo = null;    
    bool halo_active = false;
    Firearm my_firearm;
    Peripheral my_peripheral;
    HitMe my_hitme;
    float retry_time = StaticRune.getAiryDamageFrequency(); //retry get victims every so often while halo is turned on
    float retry_timer; //retry get victims every so often while halo is turned on
    bool halo_status = false;
    bool previous_status;
    Vector3 init_halo_size;

    float TIME = 0f;
    public Calamity my_calamity;
    public Calamity my_swarm;
    public Calamity my_foil;
    public WishCatcher my_wish_catcher;
    public StatSum type;
    public float stat_mult = 0.1f;
    public float time_to_next_pulse;
    public float time_to_turn_off_pulse;
    public float pulse_length;
    public float time_between_pulses;
    public float time_to_inflict_damage;

    

    public void initStats(Firearm _firearm) {
        my_firearm = _firearm;
        StatSum statsum = my_firearm.toy.rune.GetStats(false);
        
        //period = statsum.getReloadTime(false);
        //duration = period * duration_factor;    
        time_between_pulses = statsum.getReloadTime(false);
        pulse_length = time_between_pulses * duration_factor;
        type = statsum;        
        previous_status = false;
       // retry_time = 0f;
             
        monsters = Peripheral.Instance.targets;
        if (tileSize < 0) { tileSize = Peripheral.Instance.tileSize; }
        /*
        if (duration > period) {
            period = -1;
            duration = -1;
        }//lol what		
        */

        if (pulse_length > time_between_pulses)
        {
            time_between_pulses = -1;
            pulse_length = -1;
        }
        halo_active = false;
        if (halo == null) {
            halo = Zoo.Instance.getObject("Surfaces/Units/toy_halo", true);
            halo.transform.SetParent(transform);
            halo.transform.position = this.transform.position;
            updateHaloSize();
        }


        ShowHalo(false);

      //  StopAllCoroutines();
       // StartCoroutine(RunMe());
    }



 

    void OnDisable()
    {
    //    StopAllCoroutines();
        CancelInvoke();
       
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;
        TIME += Time.deltaTime;
        
        if (!my_firearm) return; 
        if (!my_firearm.Active) return;

        
        
        //previous_status = am_enabled;



        if (TIME >= time_to_next_pulse) // do the thing
        {
            if (!am_enabled)
            {
                time_to_turn_off_pulse = time_to_next_pulse + pulse_length;
                time_to_next_pulse += time_between_pulses;
            }
            am_enabled = true;
            InvokeRepeating("GetVictims", 0f, retry_time);
        }
        
        if (TIME >= time_to_turn_off_pulse && am_enabled)
        {
            if (halo_active)
            {
                ShowHalo(false);
                halo_active = false;
            }
            am_enabled = false;
            CancelInvoke();
        }

    }
   
    public void updateHaloSize() {
        if (halo == null) return;
        halo.transform.localScale = tileSize * Vector3.one * my_firearm.getCurrentRange();
    }


    private void ShowHalo(bool set)
    {
        if (halo_status == set) return;
        halo_status = set;
      


        if (set)
        {            
            LeanTween.cancel(halo);         
            LeanTween.alpha(halo.transform.GetChild(0).gameObject, 0.3f, tween_time);
        }
        else {
            LeanTween.cancel(halo);
            LeanTween.alpha(halo.transform.GetChild(0).gameObject, 0f, tween_time);            
        }
    }


    void GetVictims() {
        
        if (monsters == null) { monsters = Peripheral.Instance.targets; }

        HitMe closest_target = null;
        float closest_distance = 999f;

        bool previous_status = halo_active;

        List<HitMe> targets = new List<HitMe>();
        //  Debug.Log("Potential targets " + monsters.Count + " range is " + range + " tilesize " + tileSize + "\n");
        for (int i = 0; i < monsters.max_count; i++)
        {
            HitMe enemy = monsters.array[i];
            if (enemy == null || enemy.amDying() || !enemy.gameObject.activeSelf) continue;

            float distance = Vector2.Distance(enemy.transform.position, transform.position);
            if (distance < closest_distance)
            {
                closest_target = enemy;
                closest_distance = distance;
            }
            if (distance < my_firearm.getCurrentRange() * tileSize)
            {
                targets.Add(enemy);
                halo_active = true;
            }

        }
      //  Debug.Log("Got " + targets.Count + " victims, previous status is " + previous_status + "\n");
        if (targets.Count > 0) {
            type = my_firearm.toy.rune.GetStats(false);
            

            if (previous_status == false)
            {
                if (my_calamity != null)
                {
                    StatBit bit = type.GetStatBit(EffectType.Calamity);
                    if (bit != null) my_calamity.Init(bit, closest_target, my_firearm, EffectType.Calamity);                        
                }

                if (my_foil != null)
                {
                    StatBit bit = type.GetStatBit(EffectType.Foil);
                    if (bit != null) my_foil.Init(bit, closest_target, my_firearm, EffectType.Foil);
                }

                if (my_swarm != null)
                {
                    StatBit bit = type.GetStatBit(EffectType.Swarm);
                    if (bit != null) my_swarm.Init(bit, transform, my_firearm, EffectType.Swarm);
                }
            }
        }
    //    Debug.Log("Gonna hurt " + targets.Count + " victims\n");
        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].HurtMe(type, my_firearm, EffectType.Null);

            if (my_wish_catcher != null)
            {
                StatBit bit = type.GetStatBit(EffectType.WishCatcher);
                if (bit != null) my_wish_catcher.Init(targets[i], bit.getStats());
            }

        }

        if (!previous_status && halo_active)
        {
            ShowHalo(true);
            if (!previous_status) my_firearm.UseAmmo();
            if (previous_status == false)
            {
           
            }
        }

    }

}
