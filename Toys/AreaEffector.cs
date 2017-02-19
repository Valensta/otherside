using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class AreaEffector : MonoBehaviour {
    public float period = -1; //in seconds, how often and how long the pulse lasts, -1 is forever
    public float duration = -1;
    public float duration_factor = StaticRune.getAiryDurationFactor();
    MyArray<HitMe> monsters = null;
    float tween_time = 0.025f;
    bool am_enabled = false;
    float tileSize = -1;
    GameObject halo = null;
    float TIME = 0;
    Firearm firearm = null;
    bool halo_active = false;
    Firearm my_firearm;
    Peripheral my_peripheral;
    HitMe my_hitme;
    float retry_time = StaticRune.getAiryDamageFrequency(); //retry get victims every so often while halo is turned on
    float retry_timer; //retry get victims every so often while halo is turned on
    bool halo_status = false;
    bool previous_status;
    

    public Calamity my_calamity;
    public Calamity my_swarm;
    public WishCatcher my_wish_catcher;


    public StatSum type;

    private float range;

    public float stat_mult = 0.1f;


    public void initStats(Firearm _firearm) {
        my_firearm = _firearm;
        StatSum statsum = my_firearm.rune.GetStats(false);
        
        period = statsum.getReloadTime(false);
        duration = period * duration_factor;
        range = statsum.getRange();
        type = statsum;
        firearm = _firearm;
        previous_status = false;
        retry_time = 0f;
            
        monsters = Peripheral.Instance.targets;
        if (tileSize < 0) { tileSize = Peripheral.Instance.tileSize; }
        if (duration > period) {
            period = -1;
            duration = -1;
        }//lol what		
        halo_active = false;
        if (halo == null) {
            halo = Zoo.Instance.getObject("Surfaces/Units/toy_halo", true);
            halo.transform.parent = this.transform;
            halo.transform.position = this.transform.position;
            halo.transform.localScale = tileSize * Vector3.one * range;
            //	Debug.Log("Setting halo size to " + tileSize*Vector3.one*range+"\n");
        }


        ShowHalo(false);

    }


    void Update()
    {
        TIME += Time.deltaTime;
        previous_status = am_enabled;

        if (Mathf.Repeat(TIME, period) < duration)
        {            

            if (retry_timer >= 0) retry_timer -= Time.deltaTime;
            if (am_enabled == true && retry_timer <= 0) am_enabled = false;
            

            if (am_enabled == false)
            {
                am_enabled = true;
                GetVictims();                
                retry_timer = retry_time;
            }
        }
        else if (Mathf.Repeat(TIME, period) >= duration && am_enabled == true)
        {
            if (halo_active == true)
            {
                ShowHalo(false);
                halo_active = false;
                //	Debug.Log("tweening alpha to 0\n");
            }
            am_enabled = false;
        }

    }

    private void ShowHalo(bool set)
    {
        if (halo_status == set) return;
        halo_status = set;
      //  Debug.Log("Tweening toy halo " + set);
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
        //Debug.Log("Airy getting victims\n");
        if (monsters == null) { monsters = Peripheral.Instance.targets; }

        Transform closest_target = null;
        float closest_distance = 999f;

        List<HitMe> targets = new List<HitMe>();
        //  Debug.Log("Potential targets " + monsters.Count + " range is " + range + " tilesize " + tileSize + "\n");
        for (int i = 0; i < monsters.max_count; i++)
        {
            HitMe enemy = monsters.array[i];
            if (enemy == null || enemy.amDying() || !enemy.gameObject.activeSelf) continue;

            float distance = Vector2.Distance(enemy.transform.position, this.transform.position);
            if (distance < closest_distance)
            {
                closest_target = enemy.transform;
                closest_distance = distance;
            }
            if (distance < range * tileSize)
            {
                targets.Add(enemy);
                halo_active = true;
            }

        }
      //  Debug.Log("Got " + targets.Count + " victims, previous status is " + previous_status + "\n");
        if (targets.Count > 0) {
            type = my_firearm.rune.GetStats(false);
            

            if (previous_status == false)
            {
                if (my_calamity != null)
                {
                    StatBit bit = type.GetStatBit(EffectType.Calamity);
                    if (bit != null) my_calamity.Init(bit, closest_target, firearm);
                }

                if (my_swarm != null)
                {
                    StatBit bit = type.GetStatBit(EffectType.Swarm);
                    if (bit != null) my_swarm.Init(bit, this.transform, firearm);
                }
            }
        }

        for (int i = 0; i < targets.Count; i++)
        {
            float x = targets[i].HurtMe(type);
        //    Debug.Log("areaeffector got xp " + x + " times " + retry_time + "\n");
            firearm.addXp(x);

            //if (previous_status == false)
            //{
                
                if (my_wish_catcher != null)
                {
                    StatBit bit = type.GetStatBit(EffectType.WishCatcher);
                    if (bit != null) my_wish_catcher.Init(targets[i], bit.getStats());
                }
            //}
        }


        if (halo_active)
        {
            ShowHalo(true);
            if (!previous_status) firearm.UseAmmo();
        }

    }

}
